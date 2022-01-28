using TMPro;
using UnityEngine;

public class VotersCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    private int _currentAmount;
    private int _maxAmount;

    private void Awake()
    {
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
        _text.text = $"{_currentAmount}M / {_maxAmount}M de votos";
    }
}
