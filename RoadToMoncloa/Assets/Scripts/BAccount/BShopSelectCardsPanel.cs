using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BShopSelectCardsPanel : MonoBehaviour
{
    [SerializeField] private Button _confirmSelectionButton;
    [SerializeField] private List<Transform> _cardPlaceHolders;

    public List<Transform> CardPlaceHolders => _cardPlaceHolders;

    private EventBus _eventBus;

    private void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        DisableConfirmSelectionButton();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void EnableConfirmSelectionButton()
    {
        _confirmSelectionButton.interactable = true;
    }

    public void DisableConfirmSelectionButton()
    {
        _confirmSelectionButton.interactable = false;
    }

    public void ConfirmSelection()
    {
        _eventBus.PublishEvent(new CardsSelectionConfirmEvent());
    }
}