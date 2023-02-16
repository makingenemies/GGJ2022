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
    [SerializeField] GameObject _defaulterDefeatPanel;
    [SerializeField] TextMeshProUGUI _roundCounterText;

    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;
    private LiesManager _liesManager;
    private DefaulterChecker _defaulterChecker;
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
        _defaulterChecker= FindObjectOfType<DefaulterChecker>();
        _gameState = GameState.Instance;
        _playCardsStage = FindObjectOfType<PlayCardsStageGameplayManager>();
        _selectCardsStage = FindObjectOfType<SelectCardsStageGameplayManager>();
        _generalSettings = GeneralSettings.Instance;
        _donationManager = FindObjectOfType<DonationManager>();
        _soundEffectPlayer = SoundEffectPlayer.Instance;

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

        //Debug.Log($"Using {_soundEffectPlayer.GetInstanceID()}");
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.ShuffleCards);

        StartNextRound();
    }

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
        if (_gameState.OwesMoney)
        {
            _defaulterChecker.UpdateDebt();

            if (_defaulterChecker.MustPayDebts() && !_defaulterChecker.PayDebts())
            {
                _defaulterDefeatPanel.SetActive(true);
                return;
            }
        }
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

    public RoundConfig GetCurrentRoundCardSelectionConfig()
    {
        return CurrentRoundIndex < CurrentLevelData.CardSelectionRoundConfigs.Length
            ? CurrentLevelData.CardSelectionRoundConfigs[CurrentRoundIndex]
            : RoundConfig.GetDefaultConfig();
    }

    public void StartSelectCardsStage()
    {
        StartNextRound();
    }
}
