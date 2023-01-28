using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    [SerializeField] TextMeshPro _titleText;
    [SerializeField] TextMeshPro _leftAttributeText;
    [SerializeField] TextMeshPro _rightAttributeText;
    [SerializeField] TextMeshPro _votersText;
    [SerializeField] TextMeshPro _negativeVotersText;
    [SerializeField] TextMeshPro _moneyText;
    [SerializeField] TextMeshPro _negativeMoneyText;
    [SerializeField] SpriteRenderer _spriteRenderer;

    [Header("Sprites")]
    [SerializeField] Sprite _educationSprite;
    [SerializeField] Sprite _developmentSprite;
    [SerializeField] Sprite _economySprite;
    [SerializeField] Sprite _foreingPolicySprite;
    [SerializeField] Sprite _healthSprite;
    [SerializeField] Sprite _technologySprite;
    [SerializeField] Sprite _transportSprite;

    private Strings _strings;
    private LiesManager _liesManager;

    private Dictionary<CardCategory, Sprite> _spriteByCardCategory;
    CardData _cardData;
    private int _spriteSortingOrder;
    private int _textSortingOrder;

    private void Awake()
    {
        _spriteByCardCategory = new Dictionary<CardCategory, Sprite>
        {
            [CardCategory.Education] = _educationSprite,
            [CardCategory.Development] = _developmentSprite,
            [CardCategory.Economy] = _economySprite,
            [CardCategory.ForeignPolicy] = _foreingPolicySprite,
            [CardCategory.Health] = _healthSprite,
            [CardCategory.Technology] = _technologySprite,
            [CardCategory.Transport] = _transportSprite,
        };
    }

    private void Start()
    {
        _strings = FindObjectOfType<Strings>();
        _liesManager = FindObjectOfType<LiesManager>();

        _titleText.text = _strings.GetString(_cardData.TitleId);
        _leftAttributeText.text = _strings.GetString(_cardData.LeftAttributeId);
        _rightAttributeText.text = _strings.GetString(_cardData.RightAttributeId);

        var votersWon = _cardData.VotersWon;
        if (_liesManager.IsLiesCountersFull)
        {
            votersWon--;
        }
        SetVotersWonText(votersWon);
        SetMoneyWonText(_cardData.MoneyWon);

        _negativeVotersText.text = $"-{_cardData.VotersLost}";
        _negativeMoneyText.text = $"-{_cardData.MoneyLost}";

        _spriteRenderer.sprite = _spriteByCardCategory[_cardData.Category];

        _spriteSortingOrder = _spriteRenderer.sortingOrder;
        _textSortingOrder = _titleText.sortingOrder;
    }

    public void SetVotersWonText(int votersWon)
    {
        if (votersWon > 0)
        {
            _votersText.text = $"+{votersWon}";
        }
        else
        {
            _votersText.text = $"{votersWon}";
        }
    }

    public void SetMoneyWonText(int moneyWon)
    {
        if (moneyWon > 0)
        {
            _moneyText.text = $"+{moneyWon}";
        }
        else
        {
            _moneyText.text = $"{moneyWon}";
        }
    }

    public void SetCardData(CardData cardData)
    {
        _cardData = cardData;
    }

    public void SetSpriteSortingOrder(int sortingOrder)
    {
        _spriteRenderer.sortingOrder = sortingOrder;
    }

    public void ResetSpriteSortingOrder()
    {
        SetSpriteSortingOrder(_spriteSortingOrder);
    }

    public void SetTextsSortingOrder(int sortingOrder)
    {
        _titleText.sortingOrder = sortingOrder;
        _leftAttributeText.sortingOrder = sortingOrder;
        _rightAttributeText.sortingOrder = sortingOrder;
        _votersText.sortingOrder = sortingOrder;
        _negativeVotersText.sortingOrder = sortingOrder;
        _moneyText.sortingOrder = sortingOrder;
        _negativeMoneyText.sortingOrder = sortingOrder;
    }

    public void ResetTextsSortingOrder()
    {
        SetSpriteSortingOrder(_textSortingOrder);
    }
}