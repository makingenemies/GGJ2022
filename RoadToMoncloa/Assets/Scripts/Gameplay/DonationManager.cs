using UnityEngine;
using UnityEngine.UI;

public class DonationManager : MonoBehaviour, IEventHandler<LiePlayedEvent>
{
    [SerializeField] private GameObject _donationPanel;
    [SerializeField] private Button _openPanelButton;

    private MoneyCounter _moneyCounter;
    private LiesManager _liesManager;
    private EventBus _eventBus;

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _liesManager = FindObjectOfType<LiesManager>();
        _eventBus = FindObjectOfType<EventBus>();

        _eventBus.Register(this);
    }

    private void OnEnable()
    {
        if (_eventBus != null)
        {
            _eventBus.Register(this);
        }
    }

    private void OnDisable()
    {
        _eventBus.Unregister(this);
    }

    public void ShowDonationPanel()
    {
        _donationPanel.SetActive(true);
    }

    public void HideDonationPanel()
    {
        _donationPanel.SetActive(false);
    }

    public void Donate(int donationMoneyAmount)
    {
        _moneyCounter.UpdateCurrentAmount(-donationMoneyAmount);
        _liesManager.ResetPlayedLies();

        _donationPanel.SetActive(false);
        _openPanelButton.interactable = false;
    }

    public void HandleEvent(LiePlayedEvent @event)
    {
        if (@event.IsLiesCounterFull)
        {
            _openPanelButton.interactable = true;
        }
    }
}