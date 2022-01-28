using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();

        _votersCounter.SetMaxAmount(5);
    }

    public void UpdateVotersCount(int votersCountDelta)
    {
        _votersCounter.UpdateCurrentAmount(votersCountDelta);
    }

    public void UpdateMoneyCount(int moneyCountDelta)
    {
        _moneyCounter.UpdateCurrentAmount(moneyCountDelta);
    }
}
