using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] Button _restartButton;
    [SerializeField] GameObject _successfulLevelEndPanel;
    [SerializeField] GameObject _successfulGameEndPanel;
    [SerializeField] Transform[] _cardsPositions;
    [SerializeField] Card _cardPrefab;

    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;
    private LiesManager _liesManager;
    private GameState _gameState;
    private GeneralSettings _generalSettings;

    private int _cardsCount;

    private LevelData CurrentLevelData => _generalSettings.LevelsData[_gameState.CurrentLevelIndex];

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();
        _liesManager = FindObjectOfType<LiesManager>();
        _gameState = FindObjectOfType<GameState>();
        _generalSettings = FindObjectOfType<GeneralSettings>();

        _moneyCounter.UpdateCurrentAmount(_gameState.MoneyAmount);

        SetUpCards();
    }

    private void SetUpCards()
    {
        for (var i = 0; i < CurrentLevelData.Cards.Length; i++)
        {
            var card = Instantiate(_cardPrefab, _cardsPositions[i]);
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
        SceneManager.LoadScene("Gameplay");
    }

    // Used from UI
    public void GoToLevelSelectionDebugScene()
    {
        SceneManager.LoadScene("LevelSelection");
    }
}
