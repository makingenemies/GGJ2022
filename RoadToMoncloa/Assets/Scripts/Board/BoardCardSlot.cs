using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BoardCardSlot : MonoBehaviour
{
    [SerializeField] private bool _isSelected;
    [SerializeField] private int _modifier;
    [SerializeField] private CardPlayType _playType;
    
    private string _id;
    private bool _isInitialized;

    private BoxCollider2D _slotCollider;
    private EventBus _eventBus;
    private PlayCardsStageGameplayManager _gameplayManager;
    public TextMeshPro _modifierText;

    public string Id => _id;
    public bool IsUsed { get; set; }
    public CardPlayType PlayType => _playType;
    public int Modifier => _modifier;

    private void Awake()
    {
        _slotCollider = GetComponent<BoxCollider2D>();
        _id = Guid.NewGuid().ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        _gameplayManager= FindObjectOfType<PlayCardsStageGameplayManager>();

        _isInitialized = true;
        
        RefreshModifier();
    }

    private void OnMouseExit()
    {
        _isSelected = false;
        _eventBus.PublishEvent(new BoardCardSlotExitedEvent{});
    }

    private void OnMouseEnter()
    {
        _isSelected = true;
        _eventBus.PublishEvent(new BoardCardSlotEnteredEvent
        {
            SlotId = Id,
            //whatever happens when event is called
        });
    }

    public bool TryPlayCardSlot(CardData cardData)
    {
        if (!_isSelected)
        {
            return false;
        }

        
        return true;
    }

    private void RefreshModifier() {
        if (!_isInitialized || !_gameplayManager.AreModifiersActive || Modifier == 0) {
            _modifierText.enabled = false;
            return;
        }

        _modifierText.enabled = true;
        _modifierText.text = Modifier > 0 ? $"+{Modifier}" : $"{Modifier}";
        Debug.Log($"Setting modifier text to {_modifierText.text}");
    }
    
    public void SetModifier(int modifier) {
        Debug.Log($"Setting Modifier to {modifier}");
        _modifier = modifier;
        RefreshModifier();
    }
}
