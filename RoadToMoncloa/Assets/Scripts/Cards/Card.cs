using System.Collections.Generic;
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

    [SerializeField] CardData _cardData;
    [SerializeField] SpriteRenderer[] _moneyIcons;
    [SerializeField] SpriteRenderer[] _voterIcons;
    [SerializeField] TextMeshPro _titleText;
    [SerializeField] TextMeshPro _negativeVotersText;
    [SerializeField] TextMeshPro _negativeMoneyText;

    private Game _game;
    private Strings _strings;
    private EventBus _eventBus;
    private PauseManager _pauseManager;

    private Vector3 screenPoint;
    private Vector3 offset;
    private int _potentialPlayTypesCount;
    private CardPlayType _latestSelectedPlayType;
    private Vector3 originalPosition;

    private void Awake()
    {
        for (var i = _moneyIcons.Length - 1; i >= _cardData.MoneyWon; i--)
        {
            _moneyIcons[i].gameObject.SetActive(false);
        }
        for (var i = _voterIcons.Length - 1; i >= _cardData.VotersWon; i--)
        {
            _voterIcons[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        _game = FindObjectOfType<Game>();
        _strings = FindObjectOfType<Strings>();
        _pauseManager = FindObjectOfType<PauseManager>();

        _titleText.text = _strings.GetString(_cardData.TitleId);
        _negativeVotersText.text = $"-{_cardData.VotersLost}";
        _negativeMoneyText.text = $"-{_cardData.MoneyLost}";

        if (_eventBus == null)
        {
            _eventBus = FindObjectOfType<EventBus>();
            _eventBus.Register<LiePlayedEvent>(this);
            _eventBus.Register<LiesResetEvent>(this);
        }
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
                _potentialPlayTypesCount++;
                _latestSelectedPlayType = PlayTypeByTag[tag];
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
                _potentialPlayTypesCount--;
                return;
            }
        }
    }

    void OnMouseDown()
    {
        if (_pauseManager.IsPaused)
        {
            return;
        }

        originalPosition = transform.position;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (_pauseManager.IsPaused)
        {
            return;
        }

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        if (_pauseManager.IsPaused)
        {
            return;
        }

        if (_potentialPlayTypesCount != 1)
        {
            transform.position = originalPosition;
        }
        else if (_game.PlayCard(_cardData, _latestSelectedPlayType))
        {
            _game.DestroyCard(this);
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    public void HandleEvent(LiePlayedEvent @event)
    {
        if (!@event.IsLiesCounterFull)
        {
            return;
        }

        _voterIcons[_cardData.VotersWon - 1].gameObject.SetActive(false);
    }

    public void HandleEvent(LiesResetEvent @event)
    {
        _voterIcons[_cardData.VotersWon - 1].gameObject.SetActive(true);
    }
}
