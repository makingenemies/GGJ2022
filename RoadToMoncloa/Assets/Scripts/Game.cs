using System.Collections;
using System.Collections.Generic;
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

        _cardsCount = 4;
    }

    public void UpdateVotersCount(int votersCountDelta)
    {
        _votersCounter.UpdateCurrentAmount(votersCountDelta);
    }

    public void UpdateMoneyCount(int moneyCountDelta)
    {
        _moneyCounter.UpdateCurrentAmount(moneyCountDelta);
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
