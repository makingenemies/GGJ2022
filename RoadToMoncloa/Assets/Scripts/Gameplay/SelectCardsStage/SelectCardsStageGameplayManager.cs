using UnityEngine;

public class SelectCardsStageGameplayManager : MonoBehaviour
{
    [SerializeField] private SelectCardsPanel _selectCardsPanel;
    [SerializeField] SelectStageCard _cardPrefab;

    private GameplayManager _gameplayManager;

    private int _usedCardsCounter = 0;

    private void Start()
    {
        _gameplayManager = FindObjectOfType<GameplayManager>();

        EnterStage();
    }

    public void EnterStage()
    {
        _selectCardsPanel.SetActive(true);

        foreach (var cardPlaceHolder in _selectCardsPanel.CardPlaceHolders)
        {
            var card = Instantiate(_cardPrefab, cardPlaceHolder);
            card.SetCardData(_gameplayManager.CurrentLevelData.Cards[_usedCardsCounter]);

            _usedCardsCounter++;
            _usedCardsCounter %= _gameplayManager.CurrentLevelData.Cards.Length;
        }
    }
}