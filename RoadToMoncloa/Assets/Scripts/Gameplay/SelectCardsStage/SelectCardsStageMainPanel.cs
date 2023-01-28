using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardsStageMainPanel : MonoBehaviour
{
    [SerializeField] private Button _confirmSelectionButton;
    [SerializeField] private TextMeshProUGUI _selectNCardsText;

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

    public void UpdateTextOfNumberOfCardsToSelect(int numberOfCardsToSelect)
    {
        _selectNCardsText.text = $"Pick {numberOfCardsToSelect} cards";
    }
}