using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    private int AmountOfRounds = 2;

    [SerializeField] Button _restartButton;
    [SerializeField] GameObject _successfulLevelEndPanel;
    [SerializeField] GameObject _successfulGameEndPanel;
    [SerializeField] GameObject _defeatPanel;
    [SerializeField] GameObject _liesZone;
    [SerializeField] Button _donateButton;
    [SerializeField] Card _cardPrefab;
    [SerializeField] GameObject _cardsPlaceholderParent;
    [SerializeField] GameObject _cards4SpotsPrefab;
    [SerializeField] GameObject _cards5SpotsPrefab;
    [SerializeField] GameObject _cards6SpotsPrefab;
    [SerializeField] string[] _zoneAnimationTriggerNames;
    [SerializeField] string _wrongVotersCardMessage;
    [SerializeField] string _wrongMoneyCardMessage;
    [SerializeField] TextMeshPro _wrongCardText;
    [SerializeField] TextMeshProUGUI _roundCounterText;

    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;
    private LiesManager _liesManager;
    private GameState _gameState;
    private GeneralSettings _generalSettings;
    private DonationManager _donationManager;
    private SoundEffectPlayer _soundEffectPlayer;
    private Animator _moneyZoneAnimator;
    private Animator _votersZoneAnimator;

    private System.Random _random;

    private int _cardsCount;
    private bool _liesDisabled;
    private Coroutine _wrongCardMessageCoroutine;
    private int _playedRoundsCounter;

    private LevelData CurrentLevelData => _generalSettings.LevelsData[_gameState.CurrentLevelIndex];

    private void Awake()
    {
        _random = new System.Random();
    }

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();
        _liesManager = FindObjectOfType<LiesManager>();
        _gameState = FindObjectOfType<GameState>();
        _generalSettings = FindObjectOfType<GeneralSettings>();
        _donationManager = FindObjectOfType<DonationManager>();
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();

        _votersZoneAnimator = GameObject.FindGameObjectWithTag(Tags.VotersCardDropZone).GetComponentInChildren<Animator>();
        _moneyZoneAnimator = GameObject.FindGameObjectWithTag(Tags.MoneyCardDropZone).GetComponentInChildren<Animator>();

        StartNextRound();
    }

    private void SetUpCards()
    {
        if (CurrentLevelData.Cards.Length < 4 || CurrentLevelData.Cards.Length > 6)
        {
            throw new System.Exception("Invalid amount of cards. A level needs to have 4 to 6 cards");
        }

        var cardsPlaceholderPrefabByNumberOfCards = new Dictionary<int, GameObject>
        {
            [4] = _cards4SpotsPrefab,
            [5] = _cards5SpotsPrefab,
            [6] = _cards6SpotsPrefab,
        };

        var cardsPlaceholder = Instantiate(cardsPlaceholderPrefabByNumberOfCards[CurrentLevelData.Cards.Length], _cardsPlaceholderParent.transform);

        for (var i = 0; i < CurrentLevelData.Cards.Length; i++)
        {
            var card = Instantiate(_cardPrefab, cardsPlaceholder.transform.GetChild(i));
            card.SetCardData(CurrentLevelData.Cards[i]);
        }

        _votersCounter.SetMaxAmount(CurrentLevelData.VotersGoal);
        _cardsCount = FindObjectsOfType<Card>().Length;
    }

    /// <summary>
    /// This is temporary, setting up the same data for both rounds
    /// until we have the layout for 3 cards and the card selection stage.
    /// </summary>
    private void StartNextRound()
    {
        if (_roundCounterText != null)
        {
            _roundCounterText.text = $"{_playedRoundsCounter + 1}";
        }

        _moneyCounter.UpdateCurrentAmount(_gameState.CurrentLevelIndex == 0
            ? _generalSettings.InitialMoneyAmount
            : _gameState.MoneyAmount);
        _votersCounter.UpdateCurrentAmount(_gameState.CurrentLevelIndex == 0
            ? _generalSettings.InitialVoterCount
            : _gameState.VotersCount);

        SetUpCards();

        _donationManager.SetDonationAmount(CurrentLevelData.DonationCost);

        if (_gameState.LiesDisabled)
        {
            DisableLies();
        }
        else
        {
            _liesManager.SetPlayedLiesCount(_gameState.LiesCount);
        }

        _donateButton.gameObject.SetActive(_gameState.CurrentLevelIndex > 0);
        _liesZone.SetActive(_gameState.CurrentLevelIndex > 0);

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.ShuffleCards);
    }

    public bool PlayCard(CardData cardData, CardPlayType playType)
    {
        switch(playType)
        {
            case CardPlayType.Voters:
                return TryPlayVotersCard(cardData);
            case CardPlayType.Money:
                return TryPlayMoneyCard(cardData);
            case CardPlayType.Lies:
                var liePlayed = _liesManager.PlayLie();
                if (liePlayed)
                {
                    _votersCounter.UpdateCurrentAmount(cardData.VotersWon);
                }
                return liePlayed;
            default:
                return false;
        }
    }

    private bool TryPlayVotersCard(CardData cardData)
    {
        if (cardData.MoneyLost > _moneyCounter.CurrentAmount)
        {
            ShowWrongCardMessage(_wrongVotersCardMessage);
            _soundEffectPlayer.PlayClip(SoundNames.Gameplay.WrongPlay);
            return false;
        }

        var votersWon = cardData.VotersWon;
        if (_liesManager.IsLiesCountersFull)
        {
            votersWon = Math.Max(0, votersWon - 1);
        }
        _votersCounter.UpdateCurrentAmount(votersWon);
        _moneyCounter.UpdateCurrentAmount(-cardData.MoneyLost);
        _votersZoneAnimator.SetTrigger(GetRandomZoneAnimationTriggerName());

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.GetVotes);
        return true;
    }

    private bool TryPlayMoneyCard(CardData cardData)
    {
        if (cardData.VotersLost > _votersCounter.CurrentAmount)
        {
            ShowWrongCardMessage(_wrongMoneyCardMessage);
            _soundEffectPlayer.PlayClip(SoundNames.Gameplay.WrongPlay);
            return false;
        }
        _moneyCounter.UpdateCurrentAmount(cardData.MoneyWon);
        _votersCounter.UpdateCurrentAmount(-cardData.VotersLost);
        _moneyZoneAnimator.SetTrigger(GetRandomZoneAnimationTriggerName());

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.GetMoney);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.LoseVotes);
        return true;
    }

    private void ShowWrongCardMessage(string message)
    {
        if (_wrongCardMessageCoroutine != null)
        {
            StopCoroutine(_wrongCardMessageCoroutine);
        }

        _wrongCardText.gameObject.SetActive(true);
        _wrongCardText.text = message;

        _wrongCardMessageCoroutine = StartCoroutine(HideWrongCardMessageAfter2Seconds());
    }

    private IEnumerator HideWrongCardMessageAfter2Seconds()
    {
        yield return new WaitForSeconds(2);
        _wrongCardText.gameObject.SetActive(false);
    }

    
    private string GetRandomZoneAnimationTriggerName()
    {
        return _zoneAnimationTriggerNames[_random.Next(0, _zoneAnimationTriggerNames.Length)];
    }

    public void DestroyCard(Card card)
    {
        Destroy(card.gameObject);
        _cardsCount--;
        if (_cardsCount <= 0)
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        _playedRoundsCounter++;
        if (_playedRoundsCounter >= AmountOfRounds)
        {
            EndLevel();
        }
        else
        {
            StartNextRound();
        }
    }

    private void EndLevel()
    {
        if (_votersCounter.CurrentAmount >= CurrentLevelData.VotersGoal)
        {
            if (_gameState.CurrentLevelIndex < _generalSettings.LevelsData.Length - 1)
            {
                _soundEffectPlayer.PlayClip(SoundNames.Gameplay.BeatLevel);
                _successfulLevelEndPanel.SetActive(true);
            }
            else
            {
                _successfulGameEndPanel.SetActive(true);
            }
        }
        else
        {
            _defeatPanel.SetActive(true);
        }
    }

    // Used from UI
    public void RestartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    // Used from UI
    public void MoveToNextLevel()
    {
        _gameState.CurrentLevelIndex++;
        _gameState.MoneyAmount = _moneyCounter.CurrentAmount;
        _gameState.VotersCount = _votersCounter.CurrentAmount;
        _gameState.LiesCount = _liesManager.PlayedLiesCount;
        _gameState.LiesDisabled = _liesDisabled;
        SceneManager.LoadScene("LevelSummary");
    }

    // Used from UI
    public void GoToLevelSelectionDebugScene()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void DisableLies()
    {
        _liesManager.DisableLies();
        _donationManager.DisableDonations();
        _liesDisabled = true;
    }
}
