using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button _exitButton;

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _exitButton.gameObject.SetActive(false);
#endif
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}