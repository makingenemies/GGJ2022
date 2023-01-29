using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BShopManager : MonoBehaviour, IEventHandler<BShopCardSelectedEvent>, IEventHandler<CardsSelectionConfirmEvent>
{
    [SerializeField] private BShopSelectCardsPanel _selectCardsPanel;
    [SerializeField] CardData[] _cards;

    private EventBus _eventBus;
    private GameState _gameState;
    private SoundEffectPlayer _soundEffectPlayer;
    private GeneralSettings _generalSettings;
    private BMoneyCounter _bMoneyCounter;

    private int _cardsSelectedCost;
    private HashSet<string> _selectedCardsIds = new HashSet<string>();
    private Dictionary<string, SelectBShopCard> _cardsById = new Dictionary<string, SelectBShopCard>();

    public LevelData CurrentLevelData => _generalSettings.LevelsData[_gameState.CurrentLevelIndex];

    private void Start()
    {
        _soundEffectPlayer = SoundEffectPlayer.Instance;
        _gameState = GameState.Instance;
        _generalSettings = FindObjectOfType<GeneralSettings>();
        _bMoneyCounter = FindObjectOfType<BMoneyCounter>();

        UpdateUI();
        SetUpCards();
        RegisterToEvents();
    }

    private void UpdateUI()
    {
        _selectCardsPanel.UpdateUI(_gameState.BAccountCards.Length + _selectedCardsIds.Count, _cardsSelectedCost);
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

        for(int i = 0; i<_cards.Length; i++)
        {
            var card = _selectCardsPanel.InstantiateCard();
            card.SetCardData(_cards[i]);
            _cardsById[card.CardData.CardId] = card;
        }
    }

    public void HandleEvent(BShopCardSelectedEvent @event)
    {
        if (!IsCardSelected(@event.CardId))
        {

            TrySelectCard(@event.CardId);
        }
        else
        {
            UnselectCard(@event.CardId);
        }
    }

    private bool IsCardSelected(string cardId) => _selectedCardsIds.Contains(cardId);

    private void TrySelectCard(string cardId)
    {
        _selectedCardsIds.Add(cardId);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
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
        _selectedCardsIds.Remove(cardId);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
        _cardsById[cardId].MoveCardUp();
        _cardsSelectedCost -= _cardsById[cardId].CardData.BCardPrice;
        UpdateUI();
    }

    public void ConfirmPurchase()
    {
        UpdateBMoney();
        _bMoneyCounter.UpdateCurrentAmount(-_cardsSelectedCost);
        var ownedCardsList = _gameState.BAccountCards.ToList();

        foreach (var cardId in _selectedCardsIds)
        {
            ownedCardsList.Add(_cardsById[cardId].CardData);
            Destroy(_cardsById[cardId].gameObject.GetComponentInParent<CardPriceTextController>().gameObject);
        }

        _gameState.BAccountCards = ownedCardsList.ToArray();
        _cardsSelectedCost = 0;
        _selectedCardsIds.Clear();
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
}
