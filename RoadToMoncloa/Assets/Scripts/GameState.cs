using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private int _moneyAmount;
    [SerializeField] private int _votersCount;

    private static GameState _instance;

    public void Start()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        else
        {
            if (_instance != this)
            {
                Destroy(this.gameObject);
            }
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
}
