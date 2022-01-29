using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] Button _restartButton;

    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;
    private LiesManager _liesManager;

    private int _cardsCount;

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();
        _liesManager = FindObjectOfType<LiesManager>();

        _votersCounter.SetMaxAmount(5);
        _moneyCounter.UpdateCurrentAmount(4);

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
            _restartButton.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
