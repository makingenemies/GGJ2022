using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickbaitManager : MonoBehaviour
{
    [SerializeField] private string[] _questions;
    [SerializeField] private string[] _firstOptionReplies;
    [SerializeField] private string[] _secondOptionReplies;
    [SerializeField] private string[] _firstOptionHeadlines;
    [SerializeField] private string[] _secondOptionHeadlines;
    [SerializeField] private GameObject _questionPanel;
    [SerializeField] private GameObject _responsePanel;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TextMeshProUGUI _firstOptionReplyText;
    [SerializeField] private TextMeshProUGUI _secondOptionReplyText;
    [SerializeField] private TextMeshProUGUI _headlineTitleText;
    [SerializeField] private TextMeshProUGUI _headlineText;
    [SerializeField] private Button _continueButton;
    [SerializeField] private SpriteRenderer _newspaper;

    private GameState _gameState;

    private int levelIndex;

    private Dictionary<int, string[]> _headlinesByOptionIndex;

    private void Start()
    {
        _gameState = GameState.Instance;

        _responsePanel.SetActive(false);
        _headlineTitleText.gameObject.SetActive(false);
        _headlineText.gameObject.SetActive(false);
        _continueButton.interactable = false;
        _newspaper.gameObject.SetActive(false);

        levelIndex = _gameState.CurrentLevelIndex - 1; // We've already increased current level index
        _questionText.text = $"La prensa pregunta lo siguiente: {_questions[levelIndex]}";
        _firstOptionReplyText.text = _firstOptionReplies[levelIndex];
        _secondOptionReplyText.text = _secondOptionReplies[levelIndex];

        _headlinesByOptionIndex = new Dictionary<int, string[]>();
        _headlinesByOptionIndex[0] = _firstOptionHeadlines;
        _headlinesByOptionIndex[1] = _secondOptionHeadlines;
    }

    public void Reply(int optionIndex)
    {
        _questionPanel.SetActive(false);
        _responsePanel.SetActive(true);
        _headlineTitleText.gameObject.SetActive(true);

        _headlineText.text = _headlinesByOptionIndex[optionIndex][levelIndex];
        StartCoroutine(ShowHeadlineAfterTwoSeconds());
    }

    private IEnumerator ShowHeadlineAfterTwoSeconds()
    {
        yield return new WaitForSeconds(2);
        _headlineTitleText.gameObject.SetActive(false);
        _headlineText.gameObject.SetActive(true);
        _newspaper.gameObject.SetActive(true);
        _continueButton.interactable = true;
    }
}