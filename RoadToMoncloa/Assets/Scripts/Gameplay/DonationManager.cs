using UnityEngine;
using UnityEngine.UI;

public class DonationManager : MonoBehaviour, IEventHandler<LiePlayedEvent>
{
    [SerializeField] private GameObject _donationPanel;
    [SerializeField] private Button _openPanelButton;
    [SerializeField] private Button[] _optionButtons;
    [SerializeField] private int[] _optionMoneyAmounts;

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

        if (_optionButtons.Length != _optionMoneyAmounts.Length)
        {
            throw new System.Exception($"[{nameof(DonationManager)}] Invalid options length");
        }
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
        ShowButtonsOnlyIfEnoughMoney();
    }

    private void ShowButtonsOnlyIfEnoughMoney()
    {
        for (var i = 0; i < _optionMoneyAmounts.Length; i++)
        {
            if (_moneyCounter.CurrentAmount < _optionMoneyAmounts[i])
            {
                _optionButtons[i].interactable = false;
            }
            else
            {
                _optionButtons[i].interactable = true;
            }
        }
    }

    public void HideDonationPanel()
    {
        _donationPanel.SetActive(false);
        _pauseManager.Unpause();
    }

    public void Donate(int optionIndex)
    {
        _moneyCounter.UpdateCurrentAmount(-_optionMoneyAmounts[optionIndex]);
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