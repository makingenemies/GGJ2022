using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class GamePlayTextLocalizationManager : MonoBehaviour
{
    public LocalizeStringEvent eventString;
    private GameState _gameState;
    [SerializeField]private GameplayManager _gameplayManager;
    [SerializeField]private VotersCounter _votersCounter;
    public int MoneyAmount => _gameState.MoneyAmount;
    public int VotersCount => _votersCounter.CurrentAmount;
    public int VotersMax => _gameplayManager.CurrentLevelData.VotersGoal;


    private void Start()
    {
        _gameState = GameState.Instance;
    }

    public void RefreshVotersText()
    {
        eventString.StringReference.RefreshString();
    }
}
