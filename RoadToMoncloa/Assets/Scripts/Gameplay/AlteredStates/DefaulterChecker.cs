using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DefaulterChecker : MonoBehaviour
{
    private GameState _gameState;
    private GeneralSettings _generalSettings;

    private void Start()
    {
        _gameState = GameState.Instance;
        _generalSettings = GeneralSettings.Instance;
    }

    public bool PayDebts()
    {
        if (_gameState.MoneyAmount >= _gameState.DebtAmount)
        {
            _gameState.MoneyAmount -= _gameState.DebtAmount;
            _gameState.DebtAmount = 0;
            _gameState.RoundsToPayDebt = _generalSettings.RoundsToPayDebt; 
            _gameState.OwesMoney = false;
            return true;
            
        }
        else
        {
            _gameState.MoneyAmount = 0;
            _gameState.DebtAmount = 0;
            _gameState.RoundsToPayDebt = _generalSettings.RoundsToPayDebt;
            _gameState.OwesMoney = false;
            //reset cards bought when you was already defaulter
            return false;
        }
    }

    public bool MustPayDebts()
    {
        if(_gameState.RoundsToPayDebt <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateDebt()
    {
        _gameState.RoundsToPayDebt--;
    }

    //tests purposes only
    public void GetIntoDebt()
    {
        _gameState.OwesMoney = true;
        _gameState.DebtAmount = 1000000;
    }
}
