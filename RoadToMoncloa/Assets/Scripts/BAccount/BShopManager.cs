using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BShopManager : MonoBehaviour, IEventHandler<SelectStageCardClickedEvent>, IEventHandler<CardsSelectionConfirmEvent>
{
    private const int NumberOfRequiredCards = 3;

    [SerializeField] private SelectCardsPanel _selectCardsPanel;
    [SerializeField] private SelectBShopCard _cardPrefab;
    [SerializeField] CardData[] _cards;

    private EventBus _eventBus;
    private GameState _gameState;
    private SoundEffectPlayer _soundEffectPlayer;
    private GeneralSettings _generalSettings;

    private int _usedCardsCounter = 0;
    private HashSet<string> _selectedCardsIds = new HashSet<string>();
    private Dictionary<string, SelectBShopCard> _cardsById = new Dictionary<string, SelectBShopCard>();

    private int SelectedCardsCount => _selectedCardsIds.Count;

    public LevelData CurrentLevelData => _generalSettings.LevelsData[_gameState.CurrentLevelIndex];

    private void Start()
    {
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();
        _gameState= FindObjectOfType<GameState>();
        _generalSettings = FindObjectOfType<GeneralSettings>();

        SetUpCards();
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

    public void SetUpCards()
    {
        _usedCardsCounter = 0;
        _selectCardsPanel.SetActive(true);

        foreach (var cardPlaceHolder in _selectCardsPanel.CardPlaceHolders)
        {
            var card = Instantiate(_cardPrefab, cardPlaceHolder);
            card.SetCardData(_cards[_usedCardsCounter]);
            _cardsById[card.Id] = card;

            _usedCardsCounter++;
            _usedCardsCounter %= CurrentLevelData.Cards.Length;
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
    }
}
