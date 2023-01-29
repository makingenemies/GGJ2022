using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    [SerializeField] private int _moneyAmount;
    [SerializeField] private int _votersCount;
    [SerializeField] private int _bMoneyAmount;
    [SerializeField] private CardData[] _bAccountCards;

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
        if (string.Equals(scene.name, "StartGame", System.StringComparison.InvariantCultureIgnoreCase))
        {
            Destroy(gameObject);
            _instance = null;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public CardData[] BAccountCards 
    { 
        get => _bAccountCards; 
        set 
        { 
            _bAccountCards= value; 
        }
    }
    public int CurrentLevelIndex { get; set; }

    public int MoneyAmount 
    { 
        get => _moneyAmount;
        set
        {
            PreviousMoneyAmount = _moneyAmount;
            _moneyAmount = value;
        }
    }

    public int BMoneyAmount
    {
        get => _bMoneyAmount;
        set
        {
            PreviousBMoneyAmount = _bMoneyAmount;
            _bMoneyAmount = value;
        }
    }

    public int VotersCount
    {
        get => _votersCount;
        set
        {
            PreviousVotersCount = _votersCount;
            _votersCount = value;
        }
    }

    public int PreviousMoneyAmount { get; private set; }

    public int PreviousVotersCount { get; private set; }
    public int PreviousBMoneyAmount { get; private set; }

    public int LiesCount { get; set; }

    public bool LiesDisabled { get; set; }

    public void RemoveBAccountCard(CardData cardData)
    {
        var bCardsList = BAccountCards.ToList();
        bCardsList.RemoveAll(c => c.CardId == cardData.CardId);
        BAccountCards = bCardsList.ToArray();
    }
}
