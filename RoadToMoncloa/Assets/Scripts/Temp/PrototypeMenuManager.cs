using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeMenuManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _moneyText;
    [SerializeField] private TMPro.TextMeshProUGUI _votersText;

    private GameState _gameState;

    private int _previousMoneyAmount;
    private int _previousVotersAmount;

    private void Start()
    {
        _gameState = GameState.Instance;
        UpdateTexts();
    }

    private void Update()
    {
        if (_gameState.MoneyAmount != _previousMoneyAmount || _gameState.VotersCount != _previousVotersAmount)
        {
            _previousMoneyAmount = _gameState.MoneyAmount;
            _previousVotersAmount = _gameState.VotersCount;
            UpdateTexts();
        }
    }

    public void StartGameplayScene()
    {
        SceneManager.LoadScene(SceneNames.Gameplay);
    }

    public void StartBAccountShopScene()
    {
        SceneManager.LoadScene(SceneNames.BAccountShop);
    }

    private void UpdateTexts()
    {
        _moneyText.text = $"Money: {_gameState.MoneyAmount} €";
        _votersText.text = $"Voters: {_gameState.VotersCount}";
    }
}