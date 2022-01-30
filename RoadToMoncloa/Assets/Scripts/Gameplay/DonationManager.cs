using UnityEngine;
using UnityEngine.UI;

public class DonationManager : MonoBehaviour, IEventHandler<LiePlayedEvent>
{
    [SerializeField] private GameObject _donationPanel;
    [SerializeField] private Button _openPanelButton;
    [SerializeField] private Image _openPanelButtonImage;
    [SerializeField] private Text _openPanelButtonText;
    [SerializeField] private Button _confirmDonationButton;

    private MoneyCounter _moneyCounter;
    private EventBus _eventBus;
    private PauseManager _pauseManager;
    private GameplayManager _gameplayManager;
    private SoundEffectPlayer soundEffectPlayer;

    private int _donationAmount;

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _eventBus = FindObjectOfType<EventBus>();
        _pauseManager = FindObjectOfType<PauseManager>();
        _gameplayManager = FindObjectOfType<GameplayManager>();
        soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();

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
        ShowButtonsOnlyIfEnoughMoney();
        soundEffectPlayer.PlayClip(SoundNames.Gameplay.DonationButton);
    }

    private void ShowButtonsOnlyIfEnoughMoney()
    {
        if (_moneyCounter.CurrentAmount < _donationAmount)
        {
            _confirmDonationButton.interactable = false;
        }
        else
        {
            _confirmDonationButton.interactable = true;
        }
    }

    public void HideDonationPanel()
    {
        _donationPanel.SetActive(false);
        _pauseManager.Unpause();
    }

    public void Donate()
    {
        _moneyCounter.UpdateCurrentAmount(-_donationAmount);
        _gameplayManager.DisableLies();

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

    public void SetDonationAmount(int donationAmount)
    {
        _donationAmount = donationAmount;
        _confirmDonationButton.GetComponentInChildren<Text>().text = $"Donar - {donationAmount}M";
    }

    public void DisableDonations()
    {
        _openPanelButton.interactable = false;
        var color = _openPanelButtonImage.color;
        color.a = .1f;
        _openPanelButtonImage.color = color;
        _openPanelButtonText.gameObject.SetActive(false);
    }
}