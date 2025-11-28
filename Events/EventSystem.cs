using System;
using System.Collections.Generic;
using UnityEngine;


public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }

    private Dictionary<GameEventType, List<Action<object>>> eventListeners;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
    
            return;
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);

        eventListeners = new Dictionary<GameEventType, List<Action<object>>>();

        // Register with Service Locator
        ServiceLocator.Instance.RegisterService(this);
    }


    public void AddListener(GameEventType eventType, Action<object> listener)
    {
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = new List<Action<object>>();
        }


        if (!eventListeners[eventType].Contains(listener))
        {
            eventListeners[eventType].Add(listener);
        }
    }


    public void RemoveListener(GameEventType eventType, Action<object> listener)
    {
        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove(listener);
        }
    }


    public void Publish(GameEventType eventType, object eventData = null)
    {
        if (eventListeners.ContainsKey(eventType))
        {
            // Create a copy to avoid modification during iteration
            var listeners = new List<Action<object>>(eventListeners[eventType]);


            foreach (var listener in listeners)
            {
                try
                {
                    listener?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error publishing event {eventType}: {e.Message}");
                }
            }
        }
    }


    public void InvokeEvent(GameEventType eventType, object eventData = null)
    {
        if (eventListeners.ContainsKey(eventType))
        {
            // Create a copy to avoid modification during iteration
            var listeners = new List<Action<object>>(eventListeners[eventType]);

            foreach (var listener in listeners)
            {
                try
                {
                    listener?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error invoking event {eventType}: {e.Message}");
                }
            }
        }
    }
}