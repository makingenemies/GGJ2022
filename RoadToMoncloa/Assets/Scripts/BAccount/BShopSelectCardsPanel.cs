﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BShopSelectCardsPanel : MonoBehaviour
{
    [SerializeField] private Button _confirmSelectionButton;
    [SerializeField] private Button _defaulterButton;
    [SerializeField] private List<Transform> _cardPlaceHolders;
    [SerializeField] private SelectBShopCard _cardPrefab;
    [SerializeField] private TextMeshProUGUI _cardsSelectedCostText;
    [SerializeField] private TextMeshProUGUI _totalNumberOfCardsText;

    private EventBus _eventBus;
    private GameState _gameState;
    private GeneralSettings _generalSettings;

    private int _cardsCounter;

    public int CardSlotsCount => _cardPlaceHolders.Count;


    private void Start()
    {
        _gameState = GameState.Instance;

        _eventBus = FindObjectOfType<EventBus>();
        _generalSettings = GeneralSettings.Instance;
        _confirmSelectionButton.interactable = false;
        ToggleDefaulterButton();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public SelectBShopCard InstantiateCard()
    {
        return Instantiate(_cardPrefab, _cardPlaceHolders[_cardsCounter++]).GetComponent<SelectBShopCard>();
    }

    public void ClearUnusedSlots()
    {
        foreach (var slot in _cardPlaceHolders)
        {
            slot.gameObject.SetActive(slot.GetComponentInChildren<SelectBShopCard>() != null);
        }
    }

    public void UpdateUI(int totalCardsCount, int selectedCardsCost)
    {
        _cardsSelectedCostText.text = $"Coste Actual: {selectedCardsCost} €";
        _cardsSelectedCostText.color = selectedCardsCost <= _gameState.MoneyAmount ? Color.black : Color.red;


        _totalNumberOfCardsText.text = $"Cartas en caja B: {totalCardsCount}";
        _totalNumberOfCardsText.color = totalCardsCount <= _generalSettings.MaxNumberOfCardsInBAccount ? Color.black : Color.red;

        ToggleButton(totalCardsCount, selectedCardsCost);
    }

    private void ToggleButton(int totalCardsCount, int selectedCardsCost)
    {
        if (selectedCardsCost > 0 && selectedCardsCost <= _gameState.MoneyAmount && totalCardsCount <= _generalSettings.MaxNumberOfCardsInBAccount)
        {
            _confirmSelectionButton.interactable = true;
        }
        else
        {
            _confirmSelectionButton.interactable = false;
        }
    }

    public void ToggleDefaulterButton()
    {
        if (_gameState.OwesMoney)
        {
            _defaulterButton.interactable = false;
        }
        else
        {
            _defaulterButton.interactable = true;
        }
    }

    public void ConfirmSelection()
    {
        _eventBus.PublishEvent(new CardsSelectionConfirmEvent());
    }
}