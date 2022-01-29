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

    [SerializeField] TextMeshPro _titleText;
    [SerializeField] TextMeshPro _votersText;
    [SerializeField] TextMeshPro _moneyText;
    [SerializeField] TextMeshPro _negativeVotersText;
    [SerializeField] TextMeshPro _negativeMoneyText;

    private GameplayManager _game;
    private Strings _strings;
    private EventBus _eventBus;
    private PauseManager _pauseManager;
    private LiesManager _liesManager;

    private Vector3 _screenPoint;
    private Vector3 _offset;
    private int _potentialPlayTypesCount;
    private CardPlayType _latestSelectedPlayType;
    private Vector3 _originalPosition;
    CardData _cardData;

    private void Start()
    {
        _game = FindObjectOfType<GameplayManager>();
        _strings = FindObjectOfType<Strings>();
        _pauseManager = FindObjectOfType<PauseManager>();
        _liesManager = FindObjectOfType<LiesManager>();

        if (_eventBus == null)
        {
            _eventBus = FindObjectOfType<EventBus>();
            _eventBus.Register<LiePlayedEvent>(this);
            _eventBus.Register<LiesResetEvent>(this);
        }

        _titleText.text = _strings.GetString(_cardData.TitleId);

        _moneyText.text = _cardData.MoneyWon.ToString();

        var votersWon = _cardData.VotersWon;
        if (_liesManager.IsLiesCountersFull)
        {
            votersWon--;
        }
        _votersText.text = votersWon.ToString();

        _negativeVotersText.text = $"-{_cardData.VotersLost}";
        _negativeMoneyText.text = $"-{_cardData.MoneyLost}";
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

        _originalPosition = transform.position;
        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (_pauseManager.IsPaused)
        {
            return;
        }

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
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
            transform.position = _originalPosition;
        }
        else if (_game.PlayCard(_cardData, _latestSelectedPlayType))
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

        _votersText.text = (_cardData.VotersWon - 1).ToString();
    }

    public void HandleEvent(LiesResetEvent @event)
    {
        _votersText.text = _cardData.VotersWon.ToString();
    }

    public void SetCardData(CardData cardData)
    {
        _cardData = cardData;
    }
}
