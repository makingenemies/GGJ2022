using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    private Dictionary<Type, List<IEventHandler>> _eventHandlersByEventType = new Dictionary<Type, List<IEventHandler>>();

    public void Register<TEvent>(IEventHandler<TEvent> _eventHandler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        if (!_eventHandlersByEventType.ContainsKey(eventType))
        {
            _eventHandlersByEventType[eventType] = new List<IEventHandler>();
        }

        _eventHandlersByEventType[eventType].Add(_eventHandler);
    }

    public void Unregister<TEvent>(IEventHandler<TEvent> _eventHandler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        _eventHandlersByEventType[eventType].Remove(_eventHandler);
    }

    public void PublishEvent<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        if (_eventHandlersByEventType.TryGetValue(eventType, out var eventHandlers))
        {
            foreach(var eventHandler in eventHandlers)
            {
                (eventHandler as IEventHandler<TEvent>).HandleEvent(@event);
            }
        }
    }
}