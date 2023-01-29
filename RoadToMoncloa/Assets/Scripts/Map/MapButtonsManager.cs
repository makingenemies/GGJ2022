using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapButtonsManager : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;

    private GameState _gameState;

    void Start()
    {
        _gameState = GameState.Instance;
        SetUpButtons();
    }

    private void SetUpButtons()
    {
        DisableAllButtons();
        EnableCurrentLevelButton();
    }

    private void DisableAllButtons()
    {
        foreach (var button in _buttons)
        {
            button.interactable = false;
        }
    }

    private void EnableCurrentLevelButton()
    {
        _buttons[_gameState.CurrentLevelIndex].interactable = true;
    }
}
