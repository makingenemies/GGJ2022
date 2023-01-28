using System;
using UnityEngine;


/// <summary>
/// Drag and drop from https://answers.unity.com/questions/1138645/how-do-i-drag-a-sprite-around.html
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class PlayStageCard : MonoBehaviour, IEventHandler<LiePlayedEvent>, IEventHandler<LiesResetEvent>, IEventHandler<PausedEvent>, IEventHandler<UnpausedEvent>
{
    [Header("Selected card")]
    [SerializeField] int _selectedCardSpriteSortingOrder;
    [SerializeField] int _selectedCardTextSortingOrder;

    private PlayCardsStageGameplayManager _game;
    private EventBus _eventBus;
    private PauseManager _pauseManager;
    private LiesManager _liesManager;
    private SoundEffectPlayer _soundEffectPlayer;

    private CardUI _cardUI;

    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool _isDragging;
    private Vector3 _originalPosition;
    CardData _cardData;
    BoxCollider2D _boxCollider;
    private bool _mouseOver;
    private bool _onPreview;
    private string _id;
    private bool _isUIOnTopOfOtherCards;

    public string Id => _id;
    public CardData CardData => _cardData;
    public int MoneyWonModifier { get; set;}
    public int VotersWonModifier { get; set; }
    public int MoneyLostModifier { get; set; }
    public int VotersLostModifier { get; set; }
    public int MoneyWon => CardData.MoneyWon + MoneyWonModifier + (_liesManager.IsLiesCountersFull ? -1 : 0);
    public int VotersWon => CardData.VotersWon + VotersWonModifier;
    public int MoneyLost => CardData.MoneyLost + MoneyLostModifier;
    public int VotersLost => CardData.VotersLost + VotersLostModifier;

    private void Awake()
    {
        _id = Guid.NewGuid().ToString();
        _cardUI = GetComponent<CardUI>();
    }

    private void Start()
    {
        _game = FindObjectOfType<PlayCardsStageGameplayManager>();
        _pauseManager = FindObjectOfType<PauseManager>();
        _liesManager = FindObjectOfType<LiesManager>();
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();
        _boxCollider = GetComponent<BoxCollider2D>();

        if (_eventBus == null)
        {
            _eventBus = FindObjectOfType<EventBus>();
            _eventBus.Register<LiePlayedEvent>(this);
            _eventBus.Register<LiesResetEvent>(this);
            _eventBus.Register<PausedEvent>(this);
            _eventBus.Register<UnpausedEvent>(this);
        }

        _originalPosition = transform.position;
    }

    private void OnEnable()
    {
        if (_eventBus != null)
        {
            _eventBus.Register<LiePlayedEvent>(this);
            _eventBus.Register<LiesResetEvent>(this);
            _eventBus.Register<PausedEvent>(this);
            _eventBus.Register<UnpausedEvent>(this);
        }
    }

    private void OnDisable()
    {
        _eventBus.Unregister<LiePlayedEvent>(this);
        _eventBus.Unregister<LiesResetEvent>(this);
        _eventBus.Unregister<PausedEvent>(this);
        _eventBus.Unregister<UnpausedEvent>(this);
    }

    private void OnMouseEnter()
    {
        _mouseOver = true;

        if (_pauseManager?.IsPaused ?? true)
        {
            return;
        }

        TryEnterPreview();
    }

    private void TryEnterPreview()
    {
        if (!_game.IsAnyCardSelected)
        {
            EnterPreview();
        }
    }

    private void EnterPreview()
    {
        _onPreview = true;

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.MouseHoverCard);

        gameObject.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        MoveCardUp();
        PutUIOnTopOfOtherCards();
    }

    private void PutUIOnTopOfOtherCards()
    {
        _cardUI.SetSpriteSortingOrder(_selectedCardSpriteSortingOrder);
        _cardUI.SetTextsSortingOrder(_selectedCardTextSortingOrder);
        _isUIOnTopOfOtherCards = true;
    }

    private void RestoreUISortingLayer()
    {
        _cardUI.ResetSpriteSortingOrder();
        _cardUI.ResetTextsSortingOrder();
        _isUIOnTopOfOtherCards = false;
    }

    private void MoveCardUp()
    {
        var position = gameObject.transform.position;
        position.y += .6f;
        gameObject.transform.position = position;
    }

    private void OnMouseExit()
    {
        _mouseOver = false;

        if (_pauseManager.IsPaused)
        {
            return;
        }

        ExitPreview();
    }

    private void ExitPreview()
    {
        _onPreview = false;

        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.transform.position = _originalPosition;

        RestoreUISortingLayer();
    }

    void OnMouseDown()
    {
        if (_pauseManager.IsPaused)
        {
            return;
        }

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);

        _isDragging = true;
        _eventBus.PublishEvent(new CardDragStartedEvent());

        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
        _boxCollider.enabled = false;

        PutUIOnTopOfOtherCards();
    }

    void OnMouseDrag()
    {
        if (_pauseManager.IsPaused || !_isDragging)
        {
            return;
        }

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
        transform.position = curPosition;

        if (!_isUIOnTopOfOtherCards)
        {
            PutUIOnTopOfOtherCards();
        }
    }

    private void OnMouseUp()
    {
        StopDraggingCard();
    }

    private void StopDraggingCard()
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        _eventBus.PublishEvent(new CardDragFinishedEvent());

        _boxCollider.enabled = true;

        RestoreUISortingLayer();

        if (_pauseManager.IsPaused)
        {
            RestoreCardPosition();
            return;
        }

        if (_game.PlayCard(this))
        {
            _boxCollider.enabled = false;
        }
        else
        {
            RestoreCardPosition();
        }
    }

    public void SetCardPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void RestoreCardPosition()
    {
        transform.position = _originalPosition;
        if (_mouseOver)
        {
            MoveCardUp();
        }
    }

    public void SetCardScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 0f);
    }

    public void SetParent(Transform parentTransform)
    {
        gameObject.transform.parent = parentTransform;
    }

    public void HandleEvent(LiePlayedEvent @event)
    {
        if (!@event.IsLiesCounterFull)
        {
            return;
        }
        UpdateVotersWonText();
    }

    public void HandleEvent(LiesResetEvent @event)
    {
        UpdateVotersWonText();
    }

    public void UpdateVotersWonText()
    {
        _cardUI.SetVotersWonText(VotersWon);
    }

    public void UpdateVotersLostText()
    {
        _cardUI.SetVotersLostText(VotersLost);
    }

    public void UpdateMoneyWonText()
    {
        _cardUI.SetMoneyWonText(MoneyWon);
    }

    public void UpdateMoneyLostText()
    {
        _cardUI.SetMoneyLostText(MoneyLost);
    }

    public void SetCardData(CardData cardData)
    {
        _cardData = cardData;
        _cardUI.SetCardData(cardData);
    }

    public void HandleEvent(PausedEvent @event)
    {
        if (_isDragging)
        {
            StopDraggingCard();
        }
        else if (_onPreview)
        {
            ExitPreview();
        }
    }

    public void HandleEvent(UnpausedEvent @event)
    {
        if (_mouseOver)
        {
            TryEnterPreview();
        }
    }
}
