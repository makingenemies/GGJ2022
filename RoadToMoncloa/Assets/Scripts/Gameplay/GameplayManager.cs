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
    [SerializeField] TextMeshProUGUI _roundCounterText;

    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;
    private LiesManager _liesManager;
    private GameState _gameState;
    private GeneralSettings _generalSettings;
    private DonationManager _donationManager;
    private PlayCardsStageGameplayManager _playCardsStage;
    private SelectCardsStageGameplayManager _selectCardsStage;
    private SoundEffectPlayer _soundEffectPlayer;

    private bool _liesDisabled;
    private int _playedRoundsCounter;

    private int CurrentRoundIndex => _playedRoundsCounter;
    public LevelData CurrentLevelData => _generalSettings.LevelsData[_gameState.CurrentLevelIndex];

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();
        _liesManager = FindObjectOfType<LiesManager>();
        _gameState = FindObjectOfType<GameState>();
        _playCardsStage = FindObjectOfType<PlayCardsStageGameplayManager>();
        _selectCardsStage = FindObjectOfType<SelectCardsStageGameplayManager>();
        _generalSettings = FindObjectOfType<GeneralSettings>();
        _donationManager = FindObjectOfType<DonationManager>();
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();

        _votersCounter.SetMaxAmount(CurrentLevelData.VotersGoal);

        _moneyCounter.UpdateCurrentAmount(_gameState.CurrentLevelIndex == 0
            ? _generalSettings.InitialMoneyAmount
            : _gameState.MoneyAmount);
        _votersCounter.UpdateCurrentAmount(_gameState.CurrentLevelIndex == 0
            ? _generalSettings.InitialVoterCount
            : _gameState.VotersCount);

        _donationManager.SetDonationAmount(CurrentLevelData.DonationCost);

        if (_gameState.LiesDisabled)
        {
            DisableLies();
        }
        else
        {
            _liesManager.SetPlayedLiesCount(_gameState.LiesCount);
        }

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.ShuffleCards);

        StartNextRound();
    }

    /// <summary>
    /// This is temporary, setting up the same data for both rounds
    /// until we have the layout for 3 cards and the card selection stage.
    /// </summary>
    private void StartNextRound()
    {
        _playCardsStage.ExitStage();

        if (_roundCounterText != null)
        {
            _roundCounterText.text = $"{_playedRoundsCounter + 1}";
        }

        _selectCardsStage.EnterStage();
    }

    public void EndRound()
    {
        _playedRoundsCounter++;
        if (_playedRoundsCounter >= AmountOfRounds)
        {
            EndLevel();
        }
        else
        {
            _playCardsStage.EnableButtonToMoveToNextStage();
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

    public void StartPlayCardsStage(List<CardData> _cardDatas)
    {
        _playCardsStage.EnterStage(_cardDatas);
    }

    public CardsSelectionRoundConfig GetCurrentRoundCardSelectionConfig()
    {
        return CurrentRoundIndex < CurrentLevelData.CardSelectionRoundConfigs.Length
            ? CurrentLevelData.CardSelectionRoundConfigs[CurrentRoundIndex]
            : CardsSelectionRoundConfig.GetDefaultConfig();
    }

    public void StartSelectCardsStage()
    {
        StartNextRound();
    }
}
