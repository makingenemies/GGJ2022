public interface IEventHandler
{
}

public interface IEventHandler<TEvent> : IEventHandler where TEvent : IEvent
{
    void HandleEvent(TEvent @event);
}