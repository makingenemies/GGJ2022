using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectCardsStageGameplayManager : MonoBehaviour, IEventHandler<SelectStageCardClickedEvent>, IEventHandler<CardsSelectionConfirmEvent>
{
    private const int NumberOfRequiredCards = 3;

    [SerializeField] private SelectCardsPanel _selectCardsPanel;
    [SerializeField] private SelectStageCard _cardPrefab;

    private GameplayManager _gameplayManager;
    private EventBus _eventBus;
    private SoundEffectPlayer _soundEffectPlayer;

    private int _usedCardsCounter = 0;
    private HashSet<string> _selectedCardsIds = new HashSet<string>();
    private Dictionary<string, SelectStageCard> _cardsById = new Dictionary<string, SelectStageCard>();

    private int SelectedCardsCount => _selectedCardsIds.Count;

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

        _usedCardsCounter = 0;
        _selectedCardsIds.Clear();
        _cardsById.Clear();

        _selectCardsPanel.SetActive(true);

        foreach (var cardPlaceHolder in _selectCardsPanel.CardPlaceHolders)
        {
            var card = Instantiate(_cardPrefab, cardPlaceHolder);
            card.SetCardData(_gameplayManager.CurrentLevelData.Cards[_usedCardsCounter]);
            _cardsById[card.Id] = card;

            _usedCardsCounter++;
            _usedCardsCounter %= _gameplayManager.CurrentLevelData.Cards.Length;
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
        if (SelectedCardsCount >= NumberOfRequiredCards)
        {
            return;
        }

        _selectedCardsIds.Add(cardId);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
        _cardsById[cardId].MoveCardDown();

        if (SelectedCardsCount >= NumberOfRequiredCards)
        {
            _selectCardsPanel.EnableConfirmSelectionButton();
        }
    }

    private void UnselectCard(string cardId)
    {
        _selectedCardsIds.Remove(cardId);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
        _cardsById[cardId].MoveCardUp();

        _selectCardsPanel.DisableConfirmSelectionButton();
    }

    public void HandleEvent(CardsSelectionConfirmEvent @event)
    {
        foreach (var card in _cardsById.Values)
        {
            Destroy(card.gameObject);
        }
        _selectCardsPanel.gameObject.SetActive(false);

        _gameplayManager.StartPlayCardsStage(_selectedCardsIds.Select(cardId => _cardsById[cardId].CardData).ToList());
    }
}