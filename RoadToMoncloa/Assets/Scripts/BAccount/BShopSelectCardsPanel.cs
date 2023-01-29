using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BShopSelectCardsPanel : MonoBehaviour
{
    [SerializeField] private Button _confirmSelectionButton;
    [SerializeField] private List<Transform> _cardPlaceHolders;
    [SerializeField] private SelectBShopCard _cardPrefab;
    [SerializeField] private TextMeshProUGUI _cardsSelectedCostText;

    private EventBus _eventBus;
    private GameState _gameState;

    private int _cardsCounter;

    private void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        _gameState = FindObjectOfType<GameState>();
        _confirmSelectionButton.interactable = false;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public SelectBShopCard InstantiateCard()
    {
        return Instantiate(_cardPrefab, _cardPlaceHolders[_cardsCounter++]).GetComponent<SelectBShopCard>();
    }

    public void UpdateCostText(int selectedCardsCost)
    {
        _cardsSelectedCostText.text = $"Coste Actual: {selectedCardsCost} €";
        ToggleTextColor(selectedCardsCost);
        ToggleButton(selectedCardsCost);
    }

    private void ToggleTextColor(int selectedCardsCost)
    {
        var textColor = selectedCardsCost <= _gameState.BMoneyAmount ? Color.black : Color.red;
        _cardsSelectedCostText.color = textColor;
    }

    private void ToggleButton(int selectedCardsCost)
    {
        if (selectedCardsCost > 0 && selectedCardsCost <= _gameState.BMoneyAmount)
        {
            _confirmSelectionButton.interactable = true;
        }
        else
        {
            _confirmSelectionButton.interactable = false;
        }
    }

    public void ConfirmSelection()
    {
        _eventBus.PublishEvent(new CardsSelectionConfirmEvent());
    }
}