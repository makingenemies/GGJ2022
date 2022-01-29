﻿using UnityEngine;

public class LiesManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] _lieMarkers;

    private EventBus _eventBus;

    private int _playedLiesCount;

    public int PlayedLiesCount => _playedLiesCount;

    public bool IsLiesCountersFull => _playedLiesCount >= _lieMarkers.Length;

    private void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
    }

    public bool PlayLie()
    {
        if (_playedLiesCount >= _lieMarkers.Length)
        {
            return false;
        }

        _lieMarkers[_playedLiesCount].color = Color.red;
        _playedLiesCount++;
        _eventBus.PublishEvent(new LiePlayedEvent
        {
            IsLiesCounterFull = _playedLiesCount >= _lieMarkers.Length
        });
        return true;
    }

    public void ResetPlayedLies()
    {
        _playedLiesCount = 0;
        _eventBus.PublishEvent(new LiesResetEvent());
    }

    public void SetPlayedLiesCount(int playedLiesCount)
    {
        _playedLiesCount = playedLiesCount;
        for (var i = 0; i < _playedLiesCount; i++)
        {
            _lieMarkers[i].color = Color.red;
        }
        for (var i = _playedLiesCount; i < _lieMarkers.Length; i++)
        {
            _lieMarkers[i].color = Color.white;
        }
    }
}