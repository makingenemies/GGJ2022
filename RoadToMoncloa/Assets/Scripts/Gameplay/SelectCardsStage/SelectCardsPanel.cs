using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardsPanel : MonoBehaviour
{
    [SerializeField] private List<Transform> _cardPlaceHolders;
    [SerializeField] private Button _confirmSelectionButton;

    public List<Transform> CardPlaceHolders => _cardPlaceHolders;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        DisableConfirmSelectionButton();
    }

    public void EnableConfirmSelectionButton()
    {
        _confirmSelectionButton.interactable = true;
    }

    public void DisableConfirmSelectionButton()
    {
        _confirmSelectionButton.interactable = false;
    }
}