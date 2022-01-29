using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionDebugManager : MonoBehaviour
{
    private GameState _gameState;

    private void Start()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    public void StartLevel(int levelIndex)
    {
        _gameState.CurrentLevelIndex = levelIndex;
        SceneManager.LoadScene("Gameplay");
    }
}