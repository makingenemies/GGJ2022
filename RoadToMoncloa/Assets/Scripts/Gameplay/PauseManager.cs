using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _debugCommandsPausePanel;
    [SerializeField] private TextMeshProUGUI _exitToMainMenuConfirmationText;
    [SerializeField] private TextMeshProUGUI _restartConfirmationText;

    private EventBus _eventBus;

    private bool _exitToMainMenuButtonClicked;
    private bool _restartButtonClicked;
    private bool _isPaused;

    public bool IsPaused => _isPaused;

    private void Start()
    {
#if !UNITY_EDITOR
        _debugCommandsPausePanel.SetActive(false);
#endif
        _eventBus = FindObjectOfType<EventBus>();

        _exitToMainMenuConfirmationText?.gameObject.SetActive(false);
        _restartConfirmationText?.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsPaused)
            {
                Pause();
                CancelExitToMainMenu();
                CancelRestartLevel();
                _pausePanel.SetActive(true);
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void Pause()
    {
        _isPaused = true;
        _eventBus.PublishEvent(new PausedEvent());
    }

    public void Unpause()
    {
        _isPaused = false;
        _eventBus.PublishEvent(new UnpausedEvent());
    }

    public void OpenLevelSelectionDebugScene()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void ExitToMainMenu()
    {
        CancelRestartLevel();

        if (_exitToMainMenuButtonClicked)
        {
            SceneManager.LoadScene("StartGame");
            return;
        }

        _exitToMainMenuConfirmationText.gameObject.SetActive(true);
        _exitToMainMenuButtonClicked = true;
    }

    private void CancelRestartLevel()
    {
        _restartButtonClicked = false;
        _restartConfirmationText?.gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        CancelExitToMainMenu();

        if (_restartButtonClicked)
        {
            SceneManager.LoadScene("Gameplay");
            return;
        }

        _restartConfirmationText.gameObject.SetActive(true);
        _restartButtonClicked = true;
    }

    private void CancelExitToMainMenu()
    {
        _exitToMainMenuButtonClicked = false;
        _exitToMainMenuConfirmationText?.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        Unpause();
        _pausePanel.SetActive(false);
    }
}