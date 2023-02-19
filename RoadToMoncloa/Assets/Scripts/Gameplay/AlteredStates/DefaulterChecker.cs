using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class DefaulterChecker : MonoBehaviour
{
    private GameState _gameState;
    private GeneralSettings _generalSettings;
    [SerializeField] GameObject _defaulterPanel;
    [SerializeField] TextMeshProUGUI _defaulterCounterText;
    [SerializeField] TextMeshProUGUI _moneyInAccountText;

    private void Start()
    {
        _gameState = GameState.Instance;
        _generalSettings = GeneralSettings.Instance;

        if (_gameState.OwesMoney)
        {
            _defaulterPanel.SetActive(true);
            _defaulterCounterText.text = $"Partidas restantes: {_gameState.RoundsToPayDebt}";
            _moneyInAccountText.text = $"Dinero en la cuenta: {_gameState.MoneyAmount} / {_gameState.IncrementedDebtAmount}";
        }
    }

    public bool PayDebts()
    {
        if (_gameState.MoneyAmount >= _gameState.IncrementedDebtAmount)
        {
            _gameState.MoneyAmount -= _gameState.IncrementedDebtAmount;
            _gameState.IncrementedDebtAmount = 0;
            _gameState.RoundsToPayDebt = _generalSettings.RoundsToPayDebt; 
            _gameState.OwesMoney = false;
            return true;
            
        }
        else
        {
            _gameState.MoneyAmount = 0;
            _gameState.IncrementedDebtAmount = 0;
            _gameState.RoundsToPayDebt = _generalSettings.RoundsToPayDebt;
            _gameState.OwesMoney = false;
            _gameState.BAccountOwnedCards = new CardData [0]; 
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
}
