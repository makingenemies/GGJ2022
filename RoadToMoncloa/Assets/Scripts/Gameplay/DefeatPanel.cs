using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _exitToMainMenuConfirmationText;

    private bool _exitToMainMenuButtonClicked;

    private void Start()
    {
        _exitToMainMenuConfirmationText?.gameObject.SetActive(false);
    }

    public void ExitToMainMenu()
    {
        if (_exitToMainMenuButtonClicked)
        {
            SceneManager.LoadScene("StartGame");
            return;
        }

        _exitToMainMenuConfirmationText.gameObject.SetActive(true);
        _exitToMainMenuButtonClicked = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Gameplay");
    }
}