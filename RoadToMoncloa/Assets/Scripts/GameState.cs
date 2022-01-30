using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    [SerializeField] private int _moneyAmount;
    [SerializeField] private int _votersCount;

    private static GameState _instance;

    public void Awake()
    {
        if (_instance == null)
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.Equals(scene.name, "Gameplay", System.StringComparison.InvariantCultureIgnoreCase))
        {
            Destroy(gameObject);
        }
    }

    public int CurrentLevelIndex { get; set; }

    public int MoneyAmount 
    { 
        get => _moneyAmount;
        set => _moneyAmount = value;
    }

    public int VotersCount
    {
        get => _votersCount;
        set => _votersCount = value;
    }

    public int LiesCount { get; set; }

    public bool LiesDisabled { get; set; }
}
