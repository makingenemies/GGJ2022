using TMPro;
using UnityEngine;

public class LevelSummaryManager : MonoBehaviour
{
    [SerializeField] GameObject[] _dayInfoPanels;
    [SerializeField] TextMeshProUGUI _dayVotersText;
    [SerializeField] TextMeshProUGUI _totalVotersText;
    [SerializeField] TextMeshProUGUI _dayMoneyText;
    [SerializeField] TextMeshProUGUI _totalMoneyText;

    private GameState _gameState;

    private void Start()
    {
        _gameState = FindObjectOfType<GameState>();

        foreach (var dayInfoPanel in _dayInfoPanels)
        {
            dayInfoPanel.SetActive(false);
        }

        var dayIndex = _gameState == null ? 0 : _gameState.CurrentLevelIndex;
        _dayInfoPanels[dayIndex].SetActive(true);

        DisplayResults();
    }

    private void DisplayResults()
    {
        _dayVotersText.text = $"Nuevos votantes: {_gameState.VotersCount - _gameState.PreviousVotersCount}M";
        _totalVotersText.text = $"Votantes totales: {_gameState.VotersCount}M";
        _dayMoneyText.text = $"Dinero conseguido: {_gameState.MoneyAmount - _gameState.PreviousMoneyAmount}M";
        _totalMoneyText.text = $"Fondos totales: {_gameState.MoneyAmount}M";
    }
}
