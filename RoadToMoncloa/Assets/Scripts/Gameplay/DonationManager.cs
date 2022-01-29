using UnityEngine;

public class DonationManager : MonoBehaviour
{
    [SerializeField] private GameObject _donationPanel;

    private MoneyCounter _moneyCounter;
    private LiesManager _liesManager;

    private void Start()
    {
        _moneyCounter = FindObjectOfType<MoneyCounter>();
        _liesManager = FindObjectOfType<LiesManager>();
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
    }
}