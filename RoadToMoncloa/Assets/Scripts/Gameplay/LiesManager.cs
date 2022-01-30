using TMPro;
using UnityEngine;

public class LiesManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] _lieMarkers;
    [SerializeField] SpriteRenderer _mainSprite;
    [SerializeField] TextMeshPro _label;

    private EventBus _eventBus;

    private int _playedLiesCount;
    private bool _disabled;

    public int PlayedLiesCount => _playedLiesCount;

    public bool IsLiesCountersFull => _playedLiesCount >= _lieMarkers.Length;

    private void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
    }

    public bool PlayLie()
    {
        if (_disabled || _playedLiesCount >= _lieMarkers.Length)
        {
            return false;
        }

        SetPlayedLiesCount(_playedLiesCount + 1);

        _eventBus.PublishEvent(new LiePlayedEvent
        {
            IsLiesCounterFull = _playedLiesCount >= _lieMarkers.Length
        });
        return true;
    }

    public void DisableLies()
    {
        _disabled = true;
        SetPlayedLiesCount(0);

        var color = _mainSprite.color;
        color.a = .1f;
        _mainSprite.color = color;

        _label.gameObject.SetActive(false);
    }

    public void SetPlayedLiesCount(int playedLiesCount)
    {
        _playedLiesCount = playedLiesCount;
        for (var i = 0; i < _lieMarkers.Length; i++)
        {
            _lieMarkers[i].gameObject.SetActive(false);
        }
        if (playedLiesCount > 0)
        {
            _lieMarkers[playedLiesCount - 1].gameObject.SetActive(true);
        }
    }
}