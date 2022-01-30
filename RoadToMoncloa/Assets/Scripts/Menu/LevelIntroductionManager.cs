using UnityEngine;

public class LevelIntroductionManager : MonoBehaviour
{
    [SerializeField] GameObject[] _dayInfoPanels;

    private void Start()
    {
        var gameState = FindObjectOfType<GameState>();

        foreach (var dayInfoPanel in _dayInfoPanels)
        {
            dayInfoPanel.SetActive(false);
        }

        var dayIndex = gameState == null ? 0 : gameState.CurrentLevelIndex;
        _dayInfoPanels[dayIndex].SetActive(true);
    }
}
