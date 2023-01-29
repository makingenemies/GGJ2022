using TMPro;
using UnityEngine;

public class BMoneyCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    private int _currentAmount;
    private int _maxAmount;
    private GameState _gameState;

    public int CurrentAmount => _currentAmount;

    private void Awake()
    {
        RefreshText();
    }

    private void Start()
    {
        _gameState = GameState.Instance;
        _currentAmount = _gameState.MoneyAmount;
        RefreshText();
    }

    public void UpdateCurrentAmount(int currentAmountDelta)
    {
        _currentAmount += currentAmountDelta;
        RefreshText();
    }

    public void SetMaxAmount(int maxAmount)
    {
        _maxAmount = maxAmount;
        RefreshText();
    }

    private void RefreshText()
    {
        _text.text = $"Fondos Caja B: {_currentAmount} €";
    }
}