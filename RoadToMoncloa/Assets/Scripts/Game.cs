using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private int _votersCount;
    private int _moneyCount;

    public void UpdateVotersCount(int votersCountDelta)
    {
        _votersCount += votersCountDelta;
    }

    public void UpdateMoneyCount(int moneyCountDelta)
    {
        _moneyCount += moneyCountDelta;
    }
}
