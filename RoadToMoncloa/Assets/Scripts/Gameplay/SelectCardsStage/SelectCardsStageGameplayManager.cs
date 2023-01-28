using System.Collections.Generic;
using UnityEngine;

public class SelectCardsStageGameplayManager : MonoBehaviour, IEventHandler<SelectStageCardClickedEvent>
{
    private const int MaxNumberOfCardsToSelect = 3;

    [SerializeField] private SelectCardsPanel _selectCardsPanel;
    [SerializeField] SelectStageCard _cardPrefab;

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

        EnterStage();
    }

    private void RegisterToEvents()
    {
        if (_eventBus is null)
        {
            _eventBus = FindObjectOfType<EventBus>();

            _eventBus.Register<SelectStageCardClickedEvent>(this);
        }
    }

    private void UnregisterFromEvents()
    {
        _eventBus.Unregister<SelectStageCardClickedEvent>(this);
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
        if (SelectedCardsCount >= MaxNumberOfCardsToSelect)
        {
            return;
        }

        _selectedCardsIds.Add(cardId);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
        _cardsById[cardId].MoveCardDown();
    }

    private void UnselectCard(string cardId)
    {
        _selectedCardsIds.Remove(cardId);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);
        _cardsById[cardId].MoveCardUp();
    }
}