using UnityEngine;
using UnityEngine.UI;

public class DonationManager : MonoBehaviour, IEventHandler<LiePlayedEvent>
{
    [SerializeField] private GameObject _donationPanel;
    [SerializeField] private Button _openPanelButton;

    private MoneyCounter _moneyCounter;
    private LiesManager _liesManager;
    private EventBus _eventBus;
    private PauseManager _pauseManager;

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _liesManager = FindObjectOfType<LiesManager>();
        _eventBus = FindObjectOfType<EventBus>();
        _pauseManager = FindObjectOfType<PauseManager>();

        _eventBus.Register(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _donationPanel.activeSelf)
        {
            HideDonationPanel();
        }
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
        _pauseManager.Pause();
    }

    public void HideDonationPanel()
    {
        _donationPanel.SetActive(false);
        _pauseManager.Unpause();
    }

    public void Donate(int donationMoneyAmount)
    {
        _moneyCounter.UpdateCurrentAmount(-donationMoneyAmount);
        _liesManager.ResetPlayedLies();

        HideDonationPanel();
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