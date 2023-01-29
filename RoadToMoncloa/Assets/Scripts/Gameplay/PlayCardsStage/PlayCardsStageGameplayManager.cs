using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayCardsStageGameplayManager : 
    MonoBehaviour, 
    IEventHandler<BoardCardSlotEnteredEvent>, 
    IEventHandler<BoardCardSlotExitedEvent>,
    IEventHandler<CardDragStartedEvent>,
    IEventHandler<CardDragFinishedEvent>
{
    [SerializeField] float _playedCardScale;
    [SerializeField] string[] _zoneAnimationTriggerNames;
    [SerializeField] string _wrongVotersCardMessage;
    [SerializeField] string _wrongMoneyCardMessage;
    [SerializeField] TextMeshPro _wrongCardText;
    [SerializeField] private bool _areModifiersActive;

    private GameplayManager _gameplayManager;
    private GameState _gameState;
    private GeneralSettings _generalSettings;
    private GameplayDebugManager _gameplayDebugManager;
    private MoneyCounter _moneyCounter;
    private VotersCounter _votersCounter;
    private LiesManager _liesManager;
    private EventBus _eventBus;
    private SoundEffectPlayer _soundEffectPlayer;
    private PlayCardsPanel _playCardsPanel;

    private Animator _moneyZoneAnimator;
    private Animator _votersZoneAnimator;

    private Dictionary<string, BoardCardSlot> _boardCardSlotsById;
    private Dictionary<string, PlayStageCard> _cardsById;
    private Dictionary<CardPlayType, List<PlayStageCard>> _cardsPlayedByPlayType = new Dictionary<CardPlayType, List<PlayStageCard>>();
    private Dictionary<Type, IComboProcessor> _comboProcessorsByComboSOType = new Dictionary<Type, IComboProcessor>();
    private System.Random _random;

    private int _cardsCount;
    private BoardCardSlot _selectedSlot;
    private Coroutine _wrongCardMessageCoroutine;

    public LevelData CurrentLevelData => _generalSettings.LevelsData[_gameState.CurrentLevelIndex];

    public bool AreModifiersActive => _areModifiersActive;

    public bool IsAnyCardSelected { get; private set; }

    public Dictionary<CardPlayType, List<PlayStageCard>> CardsPlayedByPlayType => _cardsPlayedByPlayType;

    private void Awake()
    {
        _random = new System.Random();
        InitializeCombos();
    }

    private void Start()
    {
        _gameplayManager = FindObjectOfType<GameplayManager>();
        _gameState = FindObjectOfType<GameState>();
        _generalSettings = FindObjectOfType<GeneralSettings>();
        _gameplayDebugManager = FindObjectOfType<GameplayDebugManager>();
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _votersCounter = FindObjectOfType<VotersCounter>();
        _liesManager = FindObjectOfType<LiesManager>();
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();
        _playCardsPanel = FindObjectOfType<PlayCardsPanel>();

        var cards = FindObjectsOfType<PlayStageCard>();
        _cardsById = new Dictionary<string, PlayStageCard>();

        foreach (var card in cards)
        {
            _cardsById[card.Id] = card;
        }

        RegisterToEvents();

        _votersZoneAnimator = GameObject.FindGameObjectWithTag(Tags.VotersCardDropZone).GetComponentInChildren<Animator>();
        _moneyZoneAnimator = GameObject.FindGameObjectWithTag(Tags.MoneyCardDropZone).GetComponentInChildren<Animator>();

        InitializePlayedCardsLists();

        _playCardsPanel.SetUpLiesUI();
        _boardCardSlotsById = new Dictionary<string, BoardCardSlot>();
        var boardCardSlots = FindObjectsOfType<BoardCardSlot>();
        foreach (var slot in boardCardSlots)
        {
            _boardCardSlotsById[slot.Id] = slot;
        }

        SetUpBCards();

        _playCardsPanel.SetActive(false);
    }

    private void RegisterToEvents()
    {
        if (_eventBus != null)
        {
            return;
        }

        _eventBus = FindObjectOfType<EventBus>();

        _eventBus.Register<BoardCardSlotEnteredEvent>(this);
        _eventBus.Register<BoardCardSlotExitedEvent>(this);
        _eventBus.Register<CardDragStartedEvent>(this);
        _eventBus.Register<CardDragFinishedEvent>(this);
    }

    private void UnregisterFromEvents()
    {
        _eventBus.Unregister<BoardCardSlotEnteredEvent>(this);
        _eventBus.Unregister<BoardCardSlotExitedEvent>(this);
        _eventBus.Unregister<CardDragStartedEvent>(this);
        _eventBus.Unregister<CardDragFinishedEvent>(this);
    }

    private void InitializePlayedCardsLists()
    {
        foreach (var cardPlayType in (CardPlayType[])Enum.GetValues(typeof(CardPlayType)))
        {
            _cardsPlayedByPlayType[cardPlayType] = new List<PlayStageCard>();
        }
    }

    private void SetUpBCards()
    {
        var bCards = _gameplayDebugManager.BCardsInGameplayOverride;
        if (bCards is null || bCards.Length == 0)
        {
            bCards = _gameState.BAccountCards;
        }

        for (var i = 0; i < bCards.Length; i++)
        {
            var card = _playCardsPanel.BCardsPanel.InstantiateCard();
            card.SetCardData(bCards[i]);
            card.PlayStageCardType = PlayStageCardType.BCard;
        }
    }

    public void EnterStage(List<CardData> _cardDatas)
    {
        _playCardsPanel.SetActive(true);
        _playCardsPanel.DisableRoundEndedUIComponents();

        _selectedSlot = null;

        SetUpCards(_cardDatas);
    }

    private void SetUpCards(List<CardData> _cardDatas)
    {
        _playCardsPanel.SetUp(_cardDatas.Count);

        for (var i = 0; i < _cardDatas.Count; i++)
        {
            var card = _playCardsPanel.InstantiateCard();
            card.SetCardData(_cardDatas[i]);
            card.PlayStageCardType = PlayStageCardType.NormalCard;
        }

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.ShuffleCards);

        _cardsCount = _cardDatas.Count;
    }

    public bool PlayCard(PlayStageCard card)
    {
        if (_selectedSlot == null || _selectedSlot.IsUsed)
        {
            return false;
        }

        bool cardPlayed;

        switch (_selectedSlot.PlayType)
        {
            case CardPlayType.Voters:
                cardPlayed = TryPlayVotersCard(card);
                if (cardPlayed)
                {
                    TrackPlayedActionCard(card);
                }
                break;
            case CardPlayType.Money:
                cardPlayed = TryPlayMoneyCard(card);
                if (cardPlayed)
                {
                    TrackPlayedActionCard(card);
                }
                break;
            case CardPlayType.Lies:
                cardPlayed = _liesManager.PlayLie();
                if (cardPlayed)
                {
                    _votersCounter.UpdateCurrentAmount(card.CardData.VotersWon);
                    DestroyCard(card);
                }
                break;
            default:
                return false;
        }
        if (cardPlayed)
        {
            ProcessCombos(card, _selectedSlot.PlayType);
            TrackPlayedCard(card, _selectedSlot.PlayType);
        }
        return cardPlayed;
    }

    private bool TryPlayVotersCard(PlayStageCard card)
    {
        if (card.MoneyLost > _moneyCounter.CurrentAmount)
        {
            ShowWrongCardMessage(_wrongVotersCardMessage);
            _soundEffectPlayer.PlayClip(SoundNames.Gameplay.WrongPlay);
            return false;
        }

        ApplyModifiers(card);

        _votersCounter.UpdateCurrentAmount(card.VotersWon);
        _moneyCounter.UpdateCurrentAmount(-card.MoneyLost);
        _votersZoneAnimator.SetTrigger(GetRandomZoneAnimationTriggerName());

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.GetVotes);
        return true;
    }

    private bool TryPlayMoneyCard(PlayStageCard card)
    {
        if (card.VotersLost > _votersCounter.CurrentAmount)
        {
            ShowWrongCardMessage(_wrongMoneyCardMessage);
            _soundEffectPlayer.PlayClip(SoundNames.Gameplay.WrongPlay);
            return false;
        }
        ApplyModifiers(card);
        _moneyCounter.UpdateCurrentAmount(card.MoneyWon);
        _votersCounter.UpdateCurrentAmount(-card.VotersLost);
        _moneyZoneAnimator.SetTrigger(GetRandomZoneAnimationTriggerName());

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.GetMoney);
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.LoseVotes);
        return true;
    }

    private void ShowWrongCardMessage(string message)
    {
        if (_wrongCardMessageCoroutine != null)
        {
            StopCoroutine(_wrongCardMessageCoroutine);
        }

        _wrongCardText.gameObject.SetActive(true);
        _wrongCardText.text = message;

        _wrongCardMessageCoroutine = StartCoroutine(HideWrongCardMessageAfter2Seconds());
    }

    private IEnumerator HideWrongCardMessageAfter2Seconds()
    {
        yield return new WaitForSeconds(2);
        _wrongCardText.gameObject.SetActive(false);
    }


    private string GetRandomZoneAnimationTriggerName()
    {
        return _zoneAnimationTriggerNames[_random.Next(0, _zoneAnimationTriggerNames.Length)];
    }

    private void DestroyCard(PlayStageCard card)
    {
        Destroy(card.gameObject);
    }

    private void TrackPlayedCard(PlayStageCard card, CardPlayType playType)
    {
        switch (card.PlayStageCardType)
        {
            case PlayStageCardType.NormalCard:
                TrackPlayedNormalCard();
                break;
            case PlayStageCardType.BCard:
                TrackPlayedBCard(card);
                break;
            default:
                break;
        }
        _cardsPlayedByPlayType[playType].Add(card);
    }

    private void TrackPlayedNormalCard()
    {
        _cardsCount--;
        if (_cardsCount <= 0)
        {
            _gameplayManager.EndRound();
        }
    }

    private void TrackPlayedBCard(PlayStageCard card)
    {
        _gameState.RemoveBAccountCard(card.CardData);
    }

    public void ExitStage()
    {
        _playCardsPanel.Teardown();
        _playCardsPanel.SetActive(false);
    }

    private void TrackPlayedActionCard(PlayStageCard card)
    {
        card.SetCardPosition(_selectedSlot.transform.position);
        card.SetCardScale(_playedCardScale);
        card.SetParent(_playCardsPanel.transform);
        _selectedSlot.IsUsed = true;
    }

    private void ApplyModifiers(PlayStageCard card)
    {
        if (!_areModifiersActive)
        {
            return;
        };

        switch (_selectedSlot.PlayType)
        {
            case CardPlayType.Voters:
                card.VotersWonModifier = _selectedSlot.Modifier;
                card.UpdateVotersWonText();
                break;
            case CardPlayType.Money:
                card.MoneyWonModifier = _selectedSlot.Modifier;
                card.UpdateMoneyWonText();
                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        RegisterToEvents();
    }

    private void OnDisable()
    {
        UnregisterFromEvents();
    }

    public void HandleEvent(BoardCardSlotEnteredEvent @event)
    {
        Debug.Log($"Slot {@event.SlotId}");
        _selectedSlot = _boardCardSlotsById[@event.SlotId];
    }

    public void HandleEvent(BoardCardSlotExitedEvent @event)
    {
        _selectedSlot = null;
        Debug.Log($"Slot exit");
    }

    public void HandleEvent(CardDragStartedEvent @event)
    {
        IsAnyCardSelected = true;
    }

    public void HandleEvent(CardDragFinishedEvent @event)
    {
        IsAnyCardSelected = false;
    }

    private void InitializeCombos()
    {
        _comboProcessorsByComboSOType[typeof(SameCardOnSameSpecificPlayTypeComboSO)] = new SameCardOnSameSpecificPlayTypeComboProcessor(this);
    }

    private void ProcessCombos(PlayStageCard playedCard, CardPlayType cardPlayType)
    {
        foreach (var combo in playedCard.CardData.Combos)
        {
            if (_comboProcessorsByComboSOType.TryGetValue(combo.GetType(), out var processor))
            {
                processor.ProcessComboAfterPlayImpl(combo, playedCard, cardPlayType);
            }
        }
    }

    public void UpdateVotersCounter(int votersCounterDelta)
    {
        _votersCounter.UpdateCurrentAmount(votersCounterDelta);
    }

    public void UpdateMoneyCounter(int moneyCounterDelta)
    {
        _moneyCounter.UpdateCurrentAmount(moneyCounterDelta);
    }

    public void EnableButtonToMoveToNextStage()
    {
        _playCardsPanel.EnableRoundEndedUIComponents();
    }
}
