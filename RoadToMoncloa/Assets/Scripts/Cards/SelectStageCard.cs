using System;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class SelectStageCard : MonoBehaviour, IEventHandler<PausedEvent>, IEventHandler<UnpausedEvent>
{
    [SerializeField] private CardUI _cardUI;

    private EventBus _eventBus;
    private PauseManager _pauseManager;
    private SoundEffectPlayer _soundEffectPlayer;

    private CardData _cardData;

    private bool _mouseOver;
    private bool _onPreview;

    public string Id { get; private set; }
    public CardData CardData => _cardData;

    private void Awake()
    {
        Id = Guid.NewGuid().ToString();
    }

    private void Start()
    {
        _pauseManager = FindObjectOfType<PauseManager>();
        _soundEffectPlayer = SoundEffectPlayer.Instance;

        if (_eventBus == null)
        {
            _eventBus = FindObjectOfType<EventBus>();
            _eventBus.Register<PausedEvent>(this);
            _eventBus.Register<UnpausedEvent>(this);
        }
    }

    private void OnEnable()
    {
        if (_eventBus != null)
        {
            _eventBus.Register<PausedEvent>(this);
            _eventBus.Register<UnpausedEvent>(this);
        }
    }

    private void OnDisable()
    {
        _eventBus.Unregister<PausedEvent>(this);
        _eventBus.Unregister<UnpausedEvent>(this);
    }

    public void SetCardData(CardData cardData)
    {
        _cardData = cardData;

        _cardUI.SetCardData(cardData);
    }

    private void OnMouseEnter()
    {
        _mouseOver = true;

        if (_pauseManager?.IsPaused ?? true)
        {
            return;
        }

        EnterPreview();
    }

    private void EnterPreview()
    {
        _onPreview = true;

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.MouseHoverCard);

        _cardUI.ShowComboDetailBox();
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

        _cardUI.HideComboDetailBox();
    }

    void OnMouseDown()
    {
        if (_pauseManager.IsPaused)
        {
            return;
        }

        _eventBus.PublishEvent(new SelectStageCardClickedEvent
        {
            CardId = Id,
        });
    }

    public void HandleEvent(PausedEvent @event)
    {
        if (_onPreview)
        {
            ExitPreview();
        }
    }

    public void HandleEvent(UnpausedEvent @event)
    {
        if (_mouseOver)
        {
            EnterPreview();
        }
    }

    public void MoveCardUp()
    {
        var position = gameObject.transform.position;
        position.y += .6f;
        gameObject.transform.position = position;
    }

    public void MoveCardDown()
    {
        var position = gameObject.transform.position;
        position.y -= .6f;
        gameObject.transform.position = position;
    }
}
