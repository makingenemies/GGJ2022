using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    [SerializeField] private int _moneyAmount;
    [SerializeField] private int _votersCount;
    [SerializeField] private int _roundsToPayDebt;

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
        _roundsToPayDebt = GeneralSettings.Instance.RoundsToPayDebt;
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

    public CardData[] BAccountOwnedCards { get; set; } = new CardData[0];

    public CardData[] BAccountShopCurrentCards { get; set; }

    public bool IsBShopInitialized { get; set; }

    public bool OwesMoney { get; set; }

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

    public void RemoveBAccountOwnedCard(CardData cardData)
    {
        var bCardsList = BAccountOwnedCards.ToList();
        bCardsList.RemoveAll(c => c.CardId == cardData.CardId);
        BAccountOwnedCards = bCardsList.ToArray();
    }

    public void RemoveBAccountShopCard(CardData cardData)
    {
        var bCardsList = BAccountShopCurrentCards.ToList();
        bCardsList.RemoveAll(c => c.CardId == cardData.CardId);
        BAccountShopCurrentCards = bCardsList.ToArray();
    }
}
