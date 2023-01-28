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
    [SerializeField] string[] _highlightableZonesTags;

    [Header("Selected card")]
    [SerializeField] int _selectedCardSpriteSortingOrder;
    [SerializeField] int _selectedCardTextSortingOrder;

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

        _moneyText.text = $"+{_cardData.MoneyWon}";

        var votersWon = _cardData.VotersWon;
        if (_liesManager.IsLiesCountersFull)
        {
            votersWon--;
        }
        _votersText.text = $"+{votersWon}";

        _negativeVotersText.text = $"-{_cardData.VotersLost}";
        _negativeMoneyText.text = $"-{_cardData.MoneyLost}";

        _spriteRenderer.sprite = _spriteByCardCategory[_cardData.Category];
    }

    public void SetCardData(CardData cardData)
    {
        _cardData = cardData;
    }
}