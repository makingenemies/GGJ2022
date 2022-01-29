using System.Linq;
using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    private static GeneralSettings _instance;

    [SerializeField] private Language _language;
    [SerializeField] private LevelData[] _levelsData;

    public Language Language => _language;
    public LevelData[] LevelsData => _levelsData.ToArray();

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
