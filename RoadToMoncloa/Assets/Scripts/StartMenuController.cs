using UnityEngine.SceneManagement;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public string nextScene;

    public void LoadScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}