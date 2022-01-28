using TMPro;
using UnityEngine;


/// <summary>
/// Drag and drop from https://answers.unity.com/questions/1138645/how-do-i-drag-a-sprite-around.html
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    [SerializeField] CardData _cardData;
    [SerializeField] SpriteRenderer[] _moneyIcons;
    [SerializeField] SpriteRenderer[] _voterIcons;
    [SerializeField] TextMeshPro _titleText;
    [SerializeField] TextMeshPro _negativeVotersText;
    [SerializeField] TextMeshPro _negativeMoneyText;

    private Game _game;
    private Strings _strings;

    private Vector3 screenPoint;
    private Vector3 offset;
    private bool isOnVotersZone;
    private bool isOnMoneyZone;
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

        _titleText.text = _strings.GetString(_cardData.TitleId);
        _negativeVotersText.text = $"-{_cardData.VotersLost}";
        _negativeMoneyText.text = $"-{_cardData.MoneyLost}";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.VotersCardDropZone))
        {
            isOnVotersZone = true;
        }
        else if (collision.CompareTag(Tags.MoneyCardDropZone))
        {
            isOnMoneyZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.VotersCardDropZone))
        {
            isOnVotersZone = false;
        }
        else if (collision.CompareTag(Tags.MoneyCardDropZone))
        {
            isOnMoneyZone = false;
        }
    }

    void OnMouseDown()
    {
        originalPosition = transform.position;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        if (isOnVotersZone && isOnMoneyZone)
        {
            Debug.Log("Dropped in the middle");
        }
        else if (isOnVotersZone)
        {
            Debug.Log("Dropped in voters zone");
            if (_game.PlayCard(_cardData, CardPlayType.Voters))
            {
                _game.DestroyCard(this);
            }
        }
        else if (isOnMoneyZone)
        {
            Debug.Log("Dropped in money zone");
            if (_game.PlayCard(_cardData, CardPlayType.Money))
            {
                _game.DestroyCard(this);
            }
        }

        transform.position = originalPosition;
    }
}
