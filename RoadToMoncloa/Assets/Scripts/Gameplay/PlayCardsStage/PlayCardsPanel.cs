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
    [SerializeField] GameObject _boardSlotsParent;
    [SerializeField] GameObject _votersArea3SlotsPrefab;
    [SerializeField] GameObject _votersArea2SlotsPrefab;
    [SerializeField] GameObject _votersArea1SlotsPrefab;
    [SerializeField] GameObject _moneyArea3SlotsPrefab;
    [SerializeField] GameObject _moneyArea2SlotsPrefab;
    [SerializeField] GameObject _moneyArea1SlotsPrefab;
    [SerializeField] private Button _pickMoreCardsButton;
    [SerializeField] private PlayCardsStageBCardsPanel _bCardsPanel;
    [SerializeField] private float _distanceBetweenCardRows;

    private GameState _gameState;
    private GameplayDebugManager _gameplayDebugManager;

    private GameObject _cardsPlaceholder;
    private int _cardsCounter;

    private Dictionary<int, GameObject> _cardsPlaceholderPrefabByNumberOfCards;

    public PlayCardsStageBCardsPanel BCardsPanel => _bCardsPanel;

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
        _gameState = GameState.Instance;
        _gameplayDebugManager = FindObjectOfType<GameplayDebugManager>();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetUpSlots(LevelData levelData)
    {
        foreach (var child in _boardSlotsParent.transform)
        {
            Destroy((child as Transform).gameObject);
        }

        var votersAreaSlotsPrefabByNumberOfSlots = new Dictionary<int, GameObject>
        {
            [1] = _votersArea1SlotsPrefab,
            [2] = _votersArea2SlotsPrefab,
            [3] = _votersArea3SlotsPrefab,
        };
        SetUpSlotsArea(levelData.VotersSideConfig.CardSlotsRowsUpToDown, votersAreaSlotsPrefabByNumberOfSlots);

        var moneyAreaSlotsPrefabByNumberOfSlots = new Dictionary<int, GameObject>
        {
            [1] = _moneyArea1SlotsPrefab,
            [2] = _moneyArea2SlotsPrefab,
            [3] = _moneyArea3SlotsPrefab,
        };
        SetUpSlotsArea(levelData.MoneySideConfig.CardSlotsRowsUpToDown, moneyAreaSlotsPrefabByNumberOfSlots);
    }

    private void SetUpSlotsArea(CardSlotRowConfig[] cardSlotsRowsUpToDown, Dictionary<int, GameObject> slotsRowPrefabsByNumberOfSlots)
    {
        var yPosition = 0f;
        foreach (var row in cardSlotsRowsUpToDown)
        {
            var slotsRow = Instantiate(slotsRowPrefabsByNumberOfSlots[row.CardSlotsLeftToRight.Length], _boardSlotsParent.transform);
            slotsRow.transform.Move(0, yPosition, 0);
            yPosition -= _distanceBetweenCardRows;
            for (var i = 0; i < slotsRow.transform.childCount; i++)
            {
                var slot = slotsRow.transform.GetChild(i).GetComponentInChildren<BoardCardSlot>();
                slot.SetModifier(row.CardSlotsLeftToRight[i].Modifier);
            }
        }
    }

    public void SetUpHand(int numberOfCards)
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