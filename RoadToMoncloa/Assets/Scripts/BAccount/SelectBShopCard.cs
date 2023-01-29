using System;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class SelectBShopCard : MonoBehaviour, IEventHandler<PausedEvent>, IEventHandler<UnpausedEvent>
{
    private EventBus _eventBus;
    private SoundEffectPlayer _soundEffectPlayer;
    private CardPriceTextController _cardPriceTextController;

    private CardUI _cardUI;

    private CardData _cardData;

    private bool _mouseOver;
    private bool _onPreview;

    public string Id { get; private set; }
    public CardData CardData => _cardData;


    private void Awake()
    {
        Id = Guid.NewGuid().ToString();
        _cardUI = GetComponent<CardUI>();
    }

    private void Start()
    {
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();
        

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
        var price = cardData.BCardPrice;
        _cardPriceTextController = GetComponentInParent<CardPriceTextController>();

        _cardPriceTextController.UpdateCardPriceText(price);
    }

    private void OnMouseEnter()
    {
        _mouseOver = true;
        EnterPreview();
    }

    private void EnterPreview()
    {
        _onPreview = true;

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.MouseHoverCard);
    }

    private void OnMouseExit()
    {
        _mouseOver = false;
        ExitPreview();
    }

    private void ExitPreview()
    {
        _onPreview = false;
    }

    void OnMouseDown()
    {
        _eventBus.PublishEvent(new BShopCardSelectedEvent
        {
            CardId = _cardData.CardId,
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
