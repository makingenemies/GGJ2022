using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    private Dictionary<Type, List<IEventHandler>> _eventHandlersByEventType = new Dictionary<Type, List<IEventHandler>>();
    private Dictionary<Type, List<IEventHandler>> _pendingRegistrationsByEventType = new Dictionary<Type, List<IEventHandler>>();
    private Dictionary<Type, List<IEventHandler>> _pendingUnegistrationsByEventType = new Dictionary<Type, List<IEventHandler>>();

    private void Update()
    {
        foreach(var eventTypeAndPendingRegistrations in _pendingRegistrationsByEventType)
        {
            foreach(var registration in eventTypeAndPendingRegistrations.Value)
            {
                RegisterImpl(eventTypeAndPendingRegistrations.Key, registration);
            }
        }

        _pendingRegistrationsByEventType.Clear();

        foreach (var eventTypeAndPendingUnregistrations in _pendingUnegistrationsByEventType)
        {
            foreach (var unregistration in eventTypeAndPendingUnregistrations.Value)
            {
                UnregisterImpl(eventTypeAndPendingUnregistrations.Key, unregistration);
            }
        }

        _pendingUnegistrationsByEventType.Clear();
    }

    public void Register<TEvent>(IEventHandler<TEvent> _eventHandler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        if (!_pendingRegistrationsByEventType.ContainsKey(eventType))
        {
            _pendingRegistrationsByEventType[eventType] = new List<IEventHandler>();
        }

        _pendingRegistrationsByEventType[eventType].Add(_eventHandler);
    }

    private void RegisterImpl(Type eventType, IEventHandler eventHandler)
    {
        if (!_eventHandlersByEventType.ContainsKey(eventType))
        {
            _eventHandlersByEventType[eventType] = new List<IEventHandler>();
        }

        _eventHandlersByEventType[eventType].Add(eventHandler);
    }

    public void Unregister<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        if (!_pendingUnegistrationsByEventType.ContainsKey(eventType))
        {
            _pendingUnegistrationsByEventType[eventType] = new List<IEventHandler>();
        }

        _pendingUnegistrationsByEventType[eventType].Add(eventHandler);
    }

    private void UnregisterImpl(Type eventType, IEventHandler eventHandler)
    {
        _eventHandlersByEventType[eventType].Remove(eventHandler);
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