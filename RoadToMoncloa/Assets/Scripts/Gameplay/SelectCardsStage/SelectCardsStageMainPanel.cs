using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardsStageMainPanel : MonoBehaviour
{
    [SerializeField] private Button _confirmSelectionButton;
    [SerializeField] private TextMeshProUGUI _selectNCardsText;
    [SerializeField] private SelectStageCard _cardPrefab;

    [Header("Cards panel prefabs")]
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel3CardsPrefab;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel4CardsPrefab;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel5CardsPrefab;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel6CardsPrefab;
    [SerializeField] private SelectCardsStageCardsPanel _cardsPanel7CardsPrefab;

    private EventBus _eventBus;

    private SelectCardsStageCardsPanel _cardsPanelInstance;

    private int _cardsCounter;
    private Dictionary<int, SelectCardsStageCardsPanel> _selectCardsPanelPrefabByNumberOfCards = new Dictionary<int, SelectCardsStageCardsPanel>();

    private void Awake()
    {
        _selectCardsPanelPrefabByNumberOfCards[3] = _cardsPanel3CardsPrefab;
        _selectCardsPanelPrefabByNumberOfCards[4] = _cardsPanel4CardsPrefab;
        _selectCardsPanelPrefabByNumberOfCards[5] = _cardsPanel5CardsPrefab;
        _selectCardsPanelPrefabByNumberOfCards[6] = _cardsPanel6CardsPrefab;
        _selectCardsPanelPrefabByNumberOfCards[7] = _cardsPanel7CardsPrefab;
    }

    private void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        DisableConfirmSelectionButton();
    }

    public void SetUp(int numberOfCards)
    {
        var selectCardsPanelPrefab = _selectCardsPanelPrefabByNumberOfCards[numberOfCards];
        _cardsPanelInstance = Instantiate(selectCardsPanelPrefab, transform).GetComponent<SelectCardsStageCardsPanel>();
    }

    public SelectStageCard InstantiateCard()
    {
        return Instantiate(_cardPrefab, _cardsPanelInstance.CardPlaceHolders[_cardsCounter++]).GetComponent<SelectStageCard>();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void EnableConfirmSelectionButton()
    {
        _confirmSelectionButton.interactable = true;
    }

    public void DisableConfirmSelectionButton()
    {
        _confirmSelectionButton.interactable = false;
    }

    public void ConfirmSelection()
    {
        _eventBus.PublishEvent(new CardsSelectionConfirmEvent());
    }

    public void UpdateTextOfNumberOfCardsToSelect(int numberOfCardsToSelect)
    {
        _selectNCardsText.text = $"Elige {numberOfCardsToSelect} cartas";
    }

    public void Teardown()
    {
        Destroy(_cardsPanelInstance.gameObject);
        _cardsCounter = 0;
    }
}