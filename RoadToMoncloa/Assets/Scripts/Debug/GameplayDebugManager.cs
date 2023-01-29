using UnityEngine;

public class GameplayDebugManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField _moneyAmountInputField;
    [SerializeField] private TMPro.TMP_InputField _votersCountInputField;
    [SerializeField] private bool _liesEnabledInFirstLevel;
    [SerializeField] private CardData[] _bCardsInGameplayOverride;

    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;

    public bool LiesEnabledInFirstLevel => _liesEnabledInFirstLevel;

    public CardData[] BCardsInGameplayOverride => _bCardsInGameplayOverride;

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();
    }

    public void SetMoneyAmount()
    {
        var delta = int.Parse(_moneyAmountInputField.text) - _moneyCounter.CurrentAmount;
        _moneyCounter.UpdateCurrentAmount(delta);
    }

    public void SetVotersCount()
    {
        var delta = int.Parse(_votersCountInputField.text) - _votersCounter.CurrentAmount;
        _votersCounter.UpdateCurrentAmount(delta);
    }
}