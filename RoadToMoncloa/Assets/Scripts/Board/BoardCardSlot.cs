using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BoardCardSlot : MonoBehaviour
{
    [SerializeField] private bool _isSelected;
    [SerializeField] private bool _areModifiersActive;
    [SerializeField] private int _votersModifier;
    [SerializeField] private int _moneyModifier;
    [SerializeField] private CardPlayType _playType;

    private BoxCollider2D _slotCollider;
    private EventBus _eventBus;
    private string _id;

    public TextMeshPro _moneyModifierText;
    public TextMeshPro _votersModifierText;

    public string Id => _id;
    public CardPlayType PlayType => _playType;

    private void Awake()
    {
        _slotCollider = GetComponent<BoxCollider2D>();
        _id = Guid.NewGuid().ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        
        if (_areModifiersActive)
        {
            ShowModifiers();
        }
        else
        {
            HideModifiers();
        }
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

    public void disableModifiers()
    {
        _areModifiersActive = false;
        HideModifiers();
    }

    public void enableModifiers()
    {
        _areModifiersActive = true;
        ShowModifiers();
    }

    private void ShowModifiers()
    {
        if (_votersModifier != 0 || _moneyModifier != 0)
        {
            //modifiers text update
            _moneyModifierText.enabled = true;
            _votersModifierText.enabled = true;
        }
    }
    private void HideModifiers()
    {
        _moneyModifierText.enabled = false;
        _votersModifierText.enabled = false;
    }
}
