using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BShopManager : MonoBehaviour, IEventHandler<BShopCardSelectedEvent>, IEventHandler<CardsSelectionConfirmEvent>
{
    [SerializeField] private BShopSelectCardsPanel _selectCardsPanel;
    [SerializeField] CardData[] _initialCards;

    private EventBus _eventBus;
    private GameState _gameState;
    private SoundEffectPlayer _soundEffectPlayer;
    private GeneralSettings _generalSettings;
    private BMoneyCounter _bMoneyCounter;

    private int _cardsSelectedCost;
    private string _selectedCardId;
    private Dictionary<string, SelectBShopCard> _cardsById = new Dictionary<string, SelectBShopCard>();

    public LevelData CurrentLevelData => _generalSettings.LevelsData[_gameState.CurrentLevelIndex];

    private void Start()
    {
        _soundEffectPlayer = SoundEffectPlayer.Instance;
        _gameState = GameState.Instance;
        _generalSettings = GeneralSettings.Instance;
        _bMoneyCounter = FindObjectOfType<BMoneyCounter>();

        if (!_gameState.IsBShopInitialized)
        {
            _gameState.IsBShopInitialized = true;
            _gameState.BAccountShopCurrentCards = _initialCards.ToArray();
        }

        UpdateUI();
        SetUpCards();
        RegisterToEvents();
    }

    private void UpdateUI()
    {
        _selectCardsPanel.UpdateUI(_gameState.BAccountOwnedCards.Length + (string.IsNullOrEmpty(_selectedCardId) ? 0 : 1), _cardsSelectedCost);
    }

    private void RegisterToEvents()
    {
        if (_eventBus is null)
        {
            _eventBus = FindObjectOfType<EventBus>();

            _eventBus.Register<BShopCardSelectedEvent>(this);
            _eventBus.Register<CardsSelectionConfirmEvent>(this);
        }
    }

    private void UnregisterFromEvents()
    {
        _eventBus.Unregister<BShopCardSelectedEvent>(this);
        _eventBus.Unregister<CardsSelectionConfirmEvent>(this);
    }

    private void OnEnable()
    {
        RegisterToEvents();
    }

    private void OnDisable()
    {
        UnregisterFromEvents();
    }

    public void SetUpCards()
    {
        _selectCardsPanel.SetActive(true);

        var numCards = Math.Min(_gameState.BAccountShopCurrentCards.Length, _selectCardsPanel.CardSlotsCount);

        for(int i = 0; i < numCards; i++)
        {
            var card = _selectCardsPanel.InstantiateCard();
            card.SetCardData(_initialCards[i]);
            _cardsById[card.CardData.CardId] = card;
        }

        _selectCardsPanel.ClearUnusedSlots();
    }

    public void HandleEvent(BShopCardSelectedEvent @event)
    {
        if (!IsCardSelected(@event.CardId))
        {
            SelectCard(@event.CardId);
        }
        else
        {
            UnselectCard(@event.CardId);
        }

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
    }

    private bool IsCardSelected(string cardId) => _selectedCardId == cardId;

    private void SelectCard(string cardId)
    {
        if (!string.IsNullOrEmpty(_selectedCardId))
        {
            UnselectCard(_selectedCardId);
        }

        _selectedCardId = cardId;
        _cardsById[cardId].MoveCardDown();
        _cardsSelectedCost += _cardsById[cardId].CardData.BCardPrice;

        UpdateUI();
    }

    private void UpdateBMoney()
    {
        _gameState.MoneyAmount -= _cardsSelectedCost;
    }

    private void UnselectCard(string cardId)
    {
        _selectedCardId = string.Empty;
        _cardsById[cardId].MoveCardUp();
        _cardsSelectedCost -= _cardsById[cardId].CardData.BCardPrice;
        UpdateUI();
    }

    public void ConfirmPurchase()
    {
        UpdateBMoney();
        _bMoneyCounter.UpdateCurrentAmount(-_cardsSelectedCost);
        var ownedCardsList = _gameState.BAccountOwnedCards.ToList();

        ownedCardsList.Add(_cardsById[_selectedCardId].CardData);
        _gameState.RemoveBAccountShopCard(_cardsById[_selectedCardId].CardData);
        Destroy(_cardsById[_selectedCardId].gameObject.GetComponentInParent<CardPriceTextController>().gameObject);

        _gameState.BAccountOwnedCards = ownedCardsList.ToArray();
        _cardsSelectedCost = 0;
        _selectedCardId = string.Empty;
        UpdateUI();
    }

    public void HandleEvent(CardsSelectionConfirmEvent @event)
    {
        foreach (var card in _cardsById.Values)
        {
            Destroy(card.gameObject);
        }
        _selectCardsPanel.gameObject.SetActive(false);
    }

    public void GetIntoDebt()
    {
        if (!_gameState.OwesMoney)
        {
            _gameState.OwesMoney = true;
            _gameState.MoneyAmount += _generalSettings.DebtAmount;
            _bMoneyCounter.UpdateCurrentAmount(_generalSettings.DebtAmount);
            _gameState.IncrementedDebtAmount = CalculateDebt();
        }
    }
    private int CalculateDebt()
    {
        var _debtWithInterests = _generalSettings.DebtAmount + GeneralSettings.Instance.DebtIncrement;
        return _debtWithInterests;
    }

    public void Exit()
    {
        SceneManager.LoadScene(SceneNames.PrototypeMenu);
    }
}
