using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTextLocalization : MonoBehaviour
{
    private GameState _gameState;
    public int MoneyAmount => _gameState.MoneyAmount;
    public int VotersCount => _gameState.VotersCount;
    private void Start()
    {
        _gameState = GameState.Instance;
    }
}
