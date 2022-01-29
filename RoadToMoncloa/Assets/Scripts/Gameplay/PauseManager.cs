using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _debugCommandsPausePanel;

    private bool _isPaused;

    public bool IsPaused => _isPaused;

    private void Start()
    {
#if !UNITY_EDITOR
        _debugCommandsPausePanel.SetActive(false);
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsPaused)
            {
                Pause();
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
}