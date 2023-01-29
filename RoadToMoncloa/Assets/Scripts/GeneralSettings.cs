using System.Linq;
using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    [SerializeField] private Language _language;
    [SerializeField] private LevelData[] _levelsData;
    [SerializeField] private int _initialVoterCount;
    [SerializeField] private int _initialMoneyAmount;
    [SerializeField] private int _maxNumberOfCardsInBAccount;

    public static GeneralSettings Instance { get; private set; }

    public Language Language => _language;
    public LevelData[] LevelsData => _levelsData.ToArray();
    public int InitialVoterCount => _initialVoterCount;
    public int InitialMoneyAmount => _initialMoneyAmount;
    public int MaxNumberOfCardsInBAccount => _maxNumberOfCardsInBAccount;

    public void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }
}
