using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] Button _restartButton;
    [SerializeField] GameObject _successfulLevelEndPanel;
    [SerializeField] GameObject _successfulGameEndPanel;
    [SerializeField] Card _cardPrefab;
    [SerializeField] GameObject _cardsPlaceholderParent;
    [SerializeField] GameObject _cards4SpotsPrefab;
    [SerializeField] GameObject _cards5SpotsPrefab;
    [SerializeField] GameObject _cards6SpotsPrefab;

    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;
    private LiesManager _liesManager;
    private GameState _gameState;
    private GeneralSettings _generalSettings;
    private DonationManager _donationManager;

    private int _cardsCount;
    private bool _liesDisabled;

    private LevelData CurrentLevelData => _generalSettings.LevelsData[_gameState.CurrentLevelIndex];

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();
        _liesManager = FindObjectOfType<LiesManager>();
        _gameState = FindObjectOfType<GameState>();
        _generalSettings = FindObjectOfType<GeneralSettings>();
        _donationManager = FindObjectOfType<DonationManager>();

        _moneyCounter.UpdateCurrentAmount(_gameState.MoneyAmount);
        _votersCounter.UpdateCurrentAmount(_gameState.VotersCount);

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

    public bool PlayCard(CardData cardData, CardPlayType playType)
    {
        switch(playType)
        {
            case CardPlayType.Voters:
                if (cardData.MoneyLost > _moneyCounter.CurrentAmount)
                {
                    return false;
                }
                var votersWon = cardData.VotersWon;
                if (_liesManager.IsLiesCountersFull)
                {
                    votersWon--;
                }
                _votersCounter.UpdateCurrentAmount(votersWon);
                _moneyCounter.UpdateCurrentAmount(-cardData.MoneyLost);
                return true;
            case CardPlayType.Money:
                if (cardData.VotersLost > _votersCounter.CurrentAmount)
                {
                    return false;
                }
                _moneyCounter.UpdateCurrentAmount(cardData.MoneyWon);
                _votersCounter.UpdateCurrentAmount(-cardData.VotersLost);
                return true;
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

    public void DestroyCard(Card card)
    {
        Destroy(card.gameObject);
        _cardsCount--;
        if (_cardsCount <= 0)
        {
            EndLevel();
        }
    }

    private void EndLevel()
    {
        if (_votersCounter.CurrentAmount >= CurrentLevelData.VotersGoal)
        {
            if (_gameState.CurrentLevelIndex < _generalSettings.LevelsData.Length - 1)
            {
                _successfulLevelEndPanel.SetActive(true);
            }
            else
            {
                _successfulGameEndPanel.SetActive(true);
            }
        }
        else
        {
            _restartButton.gameObject.SetActive(true);
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
        SceneManager.LoadScene("Gameplay");
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
