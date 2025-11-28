using System;

public static class EventSystemExtensions
{
    public static void PublishGame(this EventSystem bus, GameEventType evt, object payload = null)
        => bus.Publish(evt, payload);

    public static void SubscribeGame(this EventSystem bus, GameEventType evt, Action<object> callback)
        => bus.AddListener(evt, callback);

    public static void UnsubscribeGame(this EventSystem bus, GameEventType evt, Action<object> callback)
        => bus.RemoveListener(evt, callback);
}
