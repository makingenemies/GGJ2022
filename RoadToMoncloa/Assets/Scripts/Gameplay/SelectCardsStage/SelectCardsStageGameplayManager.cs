using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectCardsStageGameplayManager : 
    MonoBehaviour, 
    IEventHandler<SelectStageCardClickedEvent>, 
    IEventHandler<CardsSelectionConfirmEvent>,
    IEventHandler<PausedEvent>,
    IEventHandler<UnpausedEvent>
{
    [SerializeField] private SelectCardsStageMainPanel _selectCardsMainPanel;

    private GameplayManager _gameplayManager;
    private EventBus _eventBus;
    private SoundEffectPlayer _soundEffectPlayer;

    private RoundConfig _cardSelectionConfig;
    private List<CardData> _nonUsedCards;
    private bool _isStageActive;
    private LevelData _levelData;
    private Dictionary<string, CardData> _cardDatasById = new Dictionary<string, CardData>();
    private Dictionary<string, SelectStageCard> _cardsById = new Dictionary<string, SelectStageCard>();
    private HashSet<string> _selectedCardsIds = new HashSet<string>();
    private HashSet<string> _selectedCardDatasIds = new HashSet<string>();

    private int SelectedCardsCount => _selectedCardsIds.Count;

    private void Start()
    {
        _gameplayManager = FindObjectOfType<GameplayManager>();
        _soundEffectPlayer = SoundEffectPlayer.Instance;

        foreach (CardData cardData in _gameplayManager.CurrentLevelData.Cards)
        {
            _cardDatasById.Add(cardData.CardId, cardData);
        }
        RegisterToEvents();
    }

    private void RegisterToEvents()
    {
        if (_eventBus is null)
        {
            _eventBus = FindObjectOfType<EventBus>();

            _eventBus.Register<SelectStageCardClickedEvent>(this);
            _eventBus.Register<CardsSelectionConfirmEvent>(this);
            _eventBus.Register<PausedEvent>(this);
            _eventBus.Register<UnpausedEvent>(this);
        }
    }

    private void UnregisterFromEvents()
    {
        _eventBus.Unregister<SelectStageCardClickedEvent>(this);
        _eventBus.Unregister<CardsSelectionConfirmEvent>(this);
        _eventBus.Unregister<PausedEvent>(this);
        _eventBus.Unregister<UnpausedEvent>(this);
    }

    private void OnEnable()
    {
        RegisterToEvents();
    }

    private void OnDisable()
    {
        UnregisterFromEvents();
    }

    public void EnterStage()
    {
        // As EnterStage is invoked by GameplayManager on Start the first time,
        // _gameplayManager might not have been assigned yet.
        if (_gameplayManager is null)
        {
            _gameplayManager = FindObjectOfType<GameplayManager>();
        }
        if (_nonUsedCards is null)
        {
            _nonUsedCards = _gameplayManager.CurrentLevelData.Cards.ToList();
        }

        _nonUsedCards.Shuffle();

        _selectCardsMainPanel.SetActive(true);
        _selectCardsMainPanel.DisableConfirmSelectionButton();

        _cardSelectionConfig = _gameplayManager.GetCurrentRoundCardSelectionConfig();
        ValidateCardSelectionConfig();

        _selectedCardsIds.Clear();
        _selectedCardDatasIds.Clear();
        _cardsById.Clear();

        SetUpCards();

        _isStageActive = true;
    }

    private void ValidateCardSelectionConfig()
    {
        const int minCardsToOffer = 3;
        const int maxCardsToOffer = 7;
        const int minCardsToSelect = 2;
        const int maxCardsToSelect = 6;

        if (_cardSelectionConfig.NumberOfOfferedCards < minCardsToOffer || _cardSelectionConfig.NumberOfOfferedCards > maxCardsToOffer)
        {
            throw new Exception($"Invalid number of offered cards. It should be between {minCardsToOffer} and {maxCardsToOffer} but it was {_cardSelectionConfig.NumberOfOfferedCards}");
        }

        if (_cardSelectionConfig.NumberOfCardsToSelect < minCardsToSelect || _cardSelectionConfig.NumberOfCardsToSelect > maxCardsToSelect)
        {
            throw new Exception($"Invalid number of cards to select. It should be between {minCardsToSelect} and {maxCardsToSelect} but it was {_cardSelectionConfig.NumberOfCardsToSelect}");
        }

        if (_cardSelectionConfig.NumberOfCardsToSelect > _cardSelectionConfig.NumberOfOfferedCards)
        {
            throw new Exception($"There are not enough cards to select. We need to select {_cardSelectionConfig.NumberOfCardsToSelect} but only {_cardSelectionConfig.NumberOfOfferedCards} were offered");
        }
    }

    private void SetUpCards()
    {
        _selectCardsMainPanel.SetUp(_cardSelectionConfig.NumberOfOfferedCards);
        for (var i = 0; i < _cardSelectionConfig.NumberOfOfferedCards; i++)
        {
            var cardData = _nonUsedCards[i];

            var card = _selectCardsMainPanel.InstantiateCard();
            card.SetCardData(cardData);
            _cardsById[card.Id] = card;
        }

        _selectCardsMainPanel.UpdateTextOfNumberOfCardsToSelect(_cardSelectionConfig.NumberOfCardsToSelect);
    }

    public void HandleEvent(SelectStageCardClickedEvent @event)
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
        if (SelectedCardsCount >= _cardSelectionConfig.NumberOfCardsToSelect)
        {
            return;
        }

        _selectedCardsIds.Add(cardId);
        _selectedCardDatasIds.Add(_cardsById[cardId].CardData.CardId);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
        _cardsById[cardId].MoveCardDown();

        if (SelectedCardsCount >= _cardSelectionConfig.NumberOfCardsToSelect)
        {
            _selectCardsMainPanel.EnableConfirmSelectionButton();
        }
    }

    private void UnselectCard(string cardId)
    {
        _selectedCardsIds.Remove(cardId);
        _selectedCardDatasIds.Remove(_cardsById[cardId].CardData.CardId);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
        _cardsById[cardId].MoveCardUp();

        _selectCardsMainPanel.DisableConfirmSelectionButton();
    }

    public void HandleEvent(CardsSelectionConfirmEvent @event)
    {
        foreach (var card in _cardsById.Values)
        {
            Destroy(card.gameObject);
        }

        _selectCardsMainPanel.Teardown();
        _selectCardsMainPanel.SetActive(false);

        _isStageActive = false;
        InsertRandomCards();
        TrackUsedCards();
        _gameplayManager.StartPlayCardsStage(_selectedCardDatasIds.Select(cardId => _cardDatasById[cardId]).ToList());
    }

    private void InsertRandomCards()
    {
        for (int i = _cardSelectionConfig.NumberOfOfferedCards; i < _cardSelectionConfig.NumberOfOfferedCards + _cardSelectionConfig.NumberOfRandomCards; i++)
        {
            _selectedCardDatasIds.Add(_nonUsedCards[i].CardId);
        }
    }

    private void TrackUsedCards()
    {
        foreach (var cardDataId in _selectedCardDatasIds)
        {
            _nonUsedCards.RemoveAll(cardData => cardData.CardId == cardDataId);
        }
    }

    public void HandleEvent(PausedEvent @event)
    {
        if (_isStageActive)
        {
            _selectCardsMainPanel.SetActive(false);
        }
    }

    public void HandleEvent(UnpausedEvent @event)
    {
        if (_isStageActive)
        {
            _selectCardsMainPanel.SetActive(true);
        }
    }
}