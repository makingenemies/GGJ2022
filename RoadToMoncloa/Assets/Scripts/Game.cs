using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] Button _restartButton;

    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;

    private int _cardsCount;

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();

        _votersCounter.SetMaxAmount(5);
        _moneyCounter.UpdateCurrentAmount(4);

        _cardsCount = 4;
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
                _votersCounter.UpdateCurrentAmount(cardData.VotersWon);
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