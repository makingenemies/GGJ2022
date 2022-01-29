using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


/// <summary>
/// Drag and drop from https://answers.unity.com/questions/1138645/how-do-i-drag-a-sprite-around.html
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour, IEventHandler<LiePlayedEvent>, IEventHandler<LiesResetEvent>
{
    private static readonly Dictionary<string, CardPlayType> PlayTypeByTag = new Dictionary<string, CardPlayType>()
    {
        [Tags.VotersCardDropZone] = CardPlayType.Voters,
        [Tags.MoneyCardDropZone] = CardPlayType.Money,
        [Tags.LiesBox] = CardPlayType.Lies,
    };

    [SerializeField] TextMeshPro _titleText;
    [SerializeField] TextMeshPro _votersText;
    [SerializeField] TextMeshPro _negativeVotersText;
    [SerializeField] TextMeshPro _moneyText;
    [SerializeField] TextMeshPro _negativeMoneyText;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] string[] _highlightableZonesTags;

    [Header("Sprites")]
    [SerializeField] Sprite _educationSprite;
    [SerializeField] Sprite _developmentSprite;
    [SerializeField] Sprite _economySprite;
    [SerializeField] Sprite _foreingPolicySprite;
    [SerializeField] Sprite _healthSprite;
    [SerializeField] Sprite _technologySprite;
    [SerializeField] Sprite _transportSprite;

    private GameplayManager _game;
    private Strings _strings;
    private EventBus _eventBus;
    private PauseManager _pauseManager;
    private LiesManager _liesManager;
    private SoundEffectPlayer _soundEffectPlayer;

    private Vector3 _screenPoint;
    private Vector3 _offset;
    private HashSet<CardPlayType> _selectedPlayTypes;
    private bool _isDragging;
    private Vector3 _originalPosition;
    CardData _cardData;
    private Dictionary<CardCategory, Sprite> _spriteByCardCategory;

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

        _selectedPlayTypes = new HashSet<CardPlayType>();
    }

    private void Start()
    {
        _game = FindObjectOfType<GameplayManager>();
        _strings = FindObjectOfType<Strings>();
        _pauseManager = FindObjectOfType<PauseManager>();
        _liesManager = FindObjectOfType<LiesManager>();
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();

        if (_eventBus == null)
        {
            _eventBus = FindObjectOfType<EventBus>();
            _eventBus.Register<LiePlayedEvent>(this);
            _eventBus.Register<LiesResetEvent>(this);
        }

        _titleText.text = _strings.GetString(_cardData.TitleId);

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

    private void OnEnable()
    {
        if (_eventBus != null)
        {
            _eventBus.Register<LiePlayedEvent>(this);
            _eventBus.Register<LiesResetEvent>(this);
        }
    }

    private void OnDisable()
    {
        _eventBus.Unregister<LiePlayedEvent>(this);
        _eventBus.Unregister<LiesResetEvent>(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(var tag in PlayTypeByTag.Keys)
        {
            if (collision.CompareTag(tag))
            {
                _selectedPlayTypes.Add(PlayTypeByTag[tag]);

                if (_highlightableZonesTags.Contains(tag))
                {
                    var spriteRenderer = collision.gameObject.GetComponentInChildren<SpriteRenderer>();
                    SetSpriteRendererAlpha(spriteRenderer, 1);
                }

                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var tag in PlayTypeByTag.Keys)
        {
            if (collision.CompareTag(tag))
            {
                _selectedPlayTypes.Remove(PlayTypeByTag[tag]);

                if (_highlightableZonesTags.Contains(tag))
                {
                    var spriteRenderer = collision.gameObject.GetComponentInChildren<SpriteRenderer>();
                    SetSpriteRendererAlpha(spriteRenderer, .8f);
                }

                return;
            }
        }
    }

    private void SetSpriteRendererAlpha(SpriteRenderer spriteRenderer, float alpha)
    {
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    private void OnMouseEnter()
    {
        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.MouseHoverCard);
    }

    void OnMouseDown()
    {
        if (_pauseManager.IsPaused)
        {
            return;
        }

        _soundEffectPlayer.PlayClip(SoundNames.Gameplay.SelectCard);

        _isDragging = true;
        _originalPosition = transform.position;
        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (_pauseManager.IsPaused || !_isDragging)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isDragging = false;
            transform.position = _originalPosition;
            return;
        }

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        _isDragging = false;

        if (_pauseManager.IsPaused)
        {
            return;
        }

        if (_selectedPlayTypes.Count != 1)
        {
            transform.position = _originalPosition;
        }
        else if (_game.PlayCard(_cardData, _selectedPlayTypes.First()))
        {
            _game.DestroyCard(this);
        }
        else
        {
            transform.position = _originalPosition;
        }
    }

    public void HandleEvent(LiePlayedEvent @event)
    {
        if (!@event.IsLiesCounterFull)
        {
            return;
        }

        _votersText.text = $"+{_cardData.VotersWon - 1}";
    }

    public void HandleEvent(LiesResetEvent @event)
    {
        _votersText.text = $"+{_cardData.VotersWon}";
    }

    public void SetCardData(CardData cardData)
    {
        _cardData = cardData;
    }
}
