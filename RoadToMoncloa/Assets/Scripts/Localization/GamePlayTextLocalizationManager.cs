using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Settings;

public class GamePlayTextLocalizationManager : MonoBehaviour
{
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
}
