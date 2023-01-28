using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectCardsStageGameplayManager : MonoBehaviour, IEventHandler<SelectStageCardClickedEvent>, IEventHandler<CardsSelectionConfirmEvent>
{
    [SerializeField] private SelectCardsStageMainPanel _selectCardsMainPanel;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel3CardsPrefab;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel4CardsPrefab;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel5CardsPrefab;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel6CardsPrefab;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel7CardsPrefab;
    [SerializeField] private SelectStageCard _cardPrefab;

    private GameplayManager _gameplayManager;
    private EventBus _eventBus;
    private SoundEffectPlayer _soundEffectPlayer;

    private int _usedCardsCounter = 0;
    private HashSet<string> _selectedCardsIds = new HashSet<string>();
    private Dictionary<string, SelectStageCard> _cardsById = new Dictionary<string, SelectStageCard>();
    private Dictionary<int, SelectCardsStageCardsPanel> _selectCardsPanelPrefabByNumberOfCards = new Dictionary<int, SelectCardsStageCardsPanel>();
    private SelectCardsStageCardsPanel _cardsPanelInstance;
    private CardsSelectionRoundConfig _cardSelectionConfig;

    private int SelectedCardsCount => _selectedCardsIds.Count;

    private void Awake()
    {
        _selectCardsPanelPrefabByNumberOfCards[3] = _cardsPanel3CardsPrefab;
        _selectCardsPanelPrefabByNumberOfCards[4] = _cardsPanel4CardsPrefab;
        _selectCardsPanelPrefabByNumberOfCards[5] = _cardsPanel5CardsPrefab;
        _selectCardsPanelPrefabByNumberOfCards[6] = _cardsPanel6CardsPrefab;
        _selectCardsPanelPrefabByNumberOfCards[7] = _cardsPanel7CardsPrefab;
    }

    private void Start()
    {
        _gameplayManager = FindObjectOfType<GameplayManager>();
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();

        RegisterToEvents();
    }

    private void RegisterToEvents()
    {
        if (_eventBus is null)
        {
            _eventBus = FindObjectOfType<EventBus>();

            _eventBus.Register<SelectStageCardClickedEvent>(this);
            _eventBus.Register<CardsSelectionConfirmEvent>(this);
        }
    }

    private void UnregisterFromEvents()
    {
        _eventBus.Unregister<SelectStageCardClickedEvent>(this);
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

    public void EnterStage()
    {
        // As EnterStage is invoked by GameplayManager on Start the first time,
        // _gameplayManager might not have been assigned yet.
        if (_gameplayManager is null)
        {
            _gameplayManager = FindObjectOfType<GameplayManager>();
        }

        _selectCardsMainPanel.SetActive(true);

        _cardSelectionConfig = _gameplayManager.GetCurrentRoundCardSelectionConfig();
        ValidateCardSelectionConfig();

        _usedCardsCounter = 0;
        _selectedCardsIds.Clear();
        _cardsById.Clear();

        var selectCardsPanelPrefab = _selectCardsPanelPrefabByNumberOfCards[_cardSelectionConfig.NumberOfOfferedCards];
        _cardsPanelInstance = Instantiate(selectCardsPanelPrefab, _selectCardsMainPanel.transform).GetComponent<SelectCardsStageCardsPanel>();

        for (var i = 0; i < _cardSelectionConfig.NumberOfOfferedCards; i++)
        {
            var card = Instantiate(_cardPrefab, _cardsPanelInstance.CardPlaceHolders[i]);
            card.SetCardData(_gameplayManager.CurrentLevelData.Cards[_usedCardsCounter]);
            _cardsById[card.Id] = card;

            _usedCardsCounter++;
            _usedCardsCounter %= _gameplayManager.CurrentLevelData.Cards.Length;
        }

        _selectCardsMainPanel.UpdateTextOfNumberOfCardsToSelect(_cardSelectionConfig.NumberOfCardsToSelect);
    }

    private void ValidateCardSelectionConfig()
    {
        const int minCardsToOffer = 3;
        const int maxCardsToOffer = 7;
        const int minCardsToSelect = 3;
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

        Destroy(_cardsPanelInstance.gameObject);

        _selectCardsMainPanel.SetActive(false);

        _gameplayManager.StartPlayCardsStage(_selectedCardsIds.Select(cardId => _cardsById[cardId].CardData).ToList());
    }
}