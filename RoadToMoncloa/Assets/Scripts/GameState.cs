using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    [SerializeField] private int _moneyAmount;
    [SerializeField] private int _votersCount;
    [SerializeField] private CardData[] _bAccountCards;

    public static GameState Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
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
            Instance = null;
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

    public int LiesCount { get; set; }

    public bool LiesDisabled { get; set; }

    public void RemoveBAccountCard(CardData cardData)
    {
        var bCardsList = BAccountCards.ToList();
        bCardsList.RemoveAll(c => c.CardId == cardData.CardId);
        BAccountCards = bCardsList.ToArray();
    }
}
