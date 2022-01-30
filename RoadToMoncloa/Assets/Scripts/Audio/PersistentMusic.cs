using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic _instance;

    [SerializeField] private string[] _destroyOnTheseScenes;

    public void Awake()
    {
        if (_instance == null || _instance.TryDestroy())
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public bool TryDestroy()
    {
        if (_destroyOnTheseScenes.Contains(SceneManager.GetActiveScene().name))
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_destroyOnTheseScenes.Contains(scene.name))
        {
            Destroy(gameObject);
            _instance = null;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}