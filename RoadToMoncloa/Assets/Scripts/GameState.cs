using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private int _moneyAmount;

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

    public int MoneyAmount { 
        get => _moneyAmount;
        set => _moneyAmount = value;
    }
}
