using System.Linq;
using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    private static GeneralSettings _instance;

    [SerializeField] private Language _language;
    [SerializeField] private LevelData[] _levelsData;
    [SerializeField] private int _initialVoterCount;
    [SerializeField] private int _initialMoneyAmount;

    public Language Language => _language;
    public LevelData[] LevelsData => _levelsData.ToArray();
    public int InitialVoterCount => _initialVoterCount;
    public int InitialMoneyAmount => _initialMoneyAmount;

    public void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }
}
