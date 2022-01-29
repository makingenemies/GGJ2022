using System.Linq;
using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    [SerializeField] private Language _language;
    [SerializeField] private LevelData[] _levelsData;

    public Language Language => _language;
    public LevelData[] LevelsData => _levelsData.ToArray();
}
