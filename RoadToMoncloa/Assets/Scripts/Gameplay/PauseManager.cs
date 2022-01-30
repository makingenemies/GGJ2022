using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _debugCommandsPausePanel;
    [SerializeField] private TextMeshProUGUI _exitToMainMenuConfirmationText;

    private bool _exitToMainMenuButtonClicked;
    private bool _isPaused;

    public bool IsPaused => _isPaused;

    private void Start()
    {
#if !UNITY_EDITOR
        _debugCommandsPausePanel.SetActive(false);
#endif
        _exitToMainMenuConfirmationText?.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsPaused)
            {
                Pause();
                _exitToMainMenuButtonClicked = false;
                _exitToMainMenuConfirmationText?.gameObject.SetActive(false);
                _pausePanel.SetActive(true);
            }
            else
            {
                Unpause();
                _pausePanel.SetActive(false);
            }
        }
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Unpause()
    {
        _isPaused = false;
    }

    public void OpenLevelSelectionDebugScene()
    {
        SceneManager.LoadScene("LevelSelection");
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
}