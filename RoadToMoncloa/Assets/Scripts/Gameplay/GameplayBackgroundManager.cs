using UnityEngine;

public class GameplayBackgroundManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _leftZoneBackgroundPrefabs;
    [SerializeField] private GameObject[] _rightZoneBackgroundPrefabs;
    [SerializeField] private GameObject _leftZoneParent;
    [SerializeField] private GameObject _rightZoneParent;

    private void Start()
    {
        var gameState = FindObjectOfType<GameState>();
        Instantiate(_leftZoneBackgroundPrefabs[gameState.CurrentLevelIndex], _leftZoneParent.transform);
        Instantiate(_rightZoneBackgroundPrefabs[gameState.CurrentLevelIndex], _rightZoneParent.transform);
    }
}