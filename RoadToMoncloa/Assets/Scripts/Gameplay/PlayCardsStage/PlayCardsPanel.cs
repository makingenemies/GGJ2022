using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayCardsPanel : MonoBehaviour
{
    [SerializeField] GameObject _liesZone;
    [SerializeField] Button _donateButton;
    [SerializeField] PlayStageCard _cardPrefab;
    [SerializeField] GameObject _cardsPlaceholderParent;
    [SerializeField] GameObject _cards3SpotsPrefab;
    [SerializeField] GameObject _cards4SpotsPrefab;
    [SerializeField] GameObject _cards5SpotsPrefab;
    [SerializeField] GameObject _cards6SpotsPrefab;
    [SerializeField] private Button _pickMoreCardsButton;

    private GameState _gameState;
    private GameplayDebugManager _gameplayDebugManager;

    private GameObject _cardsPlaceholder;
    private int _cardsCounter;

    private Dictionary<int, GameObject> _cardsPlaceholderPrefabByNumberOfCards;

    private void Awake()
    {
        _cardsPlaceholderPrefabByNumberOfCards = new Dictionary<int, GameObject>
        {
            [3] = _cards3SpotsPrefab,
            [4] = _cards4SpotsPrefab,
            [5] = _cards5SpotsPrefab,
            [6] = _cards6SpotsPrefab,
        };
    }

    private void Start()
    {
        _gameState = FindObjectOfType<GameState>();
        _gameplayDebugManager = FindObjectOfType<GameplayDebugManager>();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetUp(int numberOfCards)
    {
        _cardsPlaceholder = Instantiate(_cardsPlaceholderPrefabByNumberOfCards[numberOfCards], _cardsPlaceholderParent.transform);
    }

    public PlayStageCard InstantiateCard()
    {
        return Instantiate(_cardPrefab, _cardsPlaceholder.transform.GetChild(_cardsCounter++)).GetComponent<PlayStageCard>();
    }

    public void EnableRoundEndedUIComponents()
    {
        _pickMoreCardsButton.gameObject.SetActive(true);
    }

    public void DisableRoundEndedUIComponents()
    {
        _pickMoreCardsButton.gameObject.SetActive(false);
    }

    public void SetUpLiesUI()
    {
        _donateButton.gameObject.SetActive(_gameState.CurrentLevelIndex > 0 || _gameplayDebugManager.LiesEnabledInFirstLevel);
        _liesZone.SetActive(_gameState.CurrentLevelIndex > 0 || _gameplayDebugManager.LiesEnabledInFirstLevel);

        if (_liesZone.activeInHierarchy)
        {
            var liesSlot = _liesZone.GetComponentInChildren<BoardCardSlot>();
        }
    }

    public void Teardown()
    {
        Destroy(_cardsPlaceholder);
        _cardsCounter = 0;
    }
}