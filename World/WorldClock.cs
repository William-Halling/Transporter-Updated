using UnityEngine;
using System;

public sealed class WorldClock : MonoBehaviour
{
    public static WorldClock Instance { get; private set; }

   
    public float GameTime { get; private set; }


    [SerializeField]
    private float timeScale = 60f;


    public float TimeScale
    {
        get => timeScale;
        set => timeScale = Mathf.Max(0f, value);
    }

    private int _lastDay = -1;
    private int _lastHour = -1;


    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ServiceLocator.Instance?.RegisterService(this);
    }


    private void Update()
    {
            // Only advance while playing
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Play)
            return;

        GameTime += Time.deltaTime * timeScale;
        CheckTimeEvents();
    }


    private void CheckTimeEvents()
    {
            // Compute current day/hour from GameTime
        int day = Mathf.FloorToInt(GameTime / 86400f);
        int hour = Mathf.FloorToInt((GameTime % 86400f) / 3600f);

        if (hour != _lastHour)
        {
            _lastHour = hour;
                // Notify listeners: hour changed
            EventSystem.Instance?.InvokeEvent(GameEventType.NewHour, hour);
        }


        if (day != _lastDay)
        {
            _lastDay = day;
                // Notify listeners: new day
            EventSystem.Instance?.InvokeEvent(GameEventType.NewDay, day);
        }
    }


    public void AdvanceTime(float seconds)
    {
        GameTime += seconds;
        CheckTimeEvents();
    }


    public void SetGameTime(float seconds)
    {
        GameTime = Mathf.Max(0f, seconds);
            // Force next tick notifications to re-fire appropriately
        _lastHour = -1;
        _lastDay = -1;
        CheckTimeEvents();
    }


    public DateTime GetInGameDateTime()
    {
        var epoch = new DateTime(1800, 1, 1);

        return epoch.AddSeconds(GameTime);
    }


    public string GetFormattedTime() => GetInGameDateTime().ToString("yyyy-MM-dd HH:mm");
}
