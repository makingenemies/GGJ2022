using UnityEngine;
using UnityEngine.UI;

public class PlayCardsPanel : MonoBehaviour
{
    [SerializeField] private Button _pickMoreCardsButton;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void EnableRoundEndedUIComponents()
    {
        _pickMoreCardsButton.gameObject.SetActive(true);
    }

    public void DisableRoundEndedUIComponents()
    {
        _pickMoreCardsButton.gameObject.SetActive(false);
    }
}