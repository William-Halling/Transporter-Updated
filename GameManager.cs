using System;
using System.Collections.Generic;
using Transporter.Data;
using UnityEngine;



public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private static GameData _pendingLoadData = null;
    public static GameData PendingLoadData => _pendingLoadData;


    [Header("State")]
    [SerializeField] private GameState currentState = GameState.MainMenu;

    public GameState CurrentState { get => currentState; set => ApplyState(value); }


    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            
            return; 
        }
    
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
            // subscribe to scene loaded so we can apply pending data when GameScene finishes loading
        GameEventBus.Instance?.AddListener(GameEventType.SceneLoaded, OnSceneLoaded);
    }


    private void OnDestroy()
    {
        GameEventBus.Instance?.RemoveListener(GameEventType.SceneLoaded, OnSceneLoaded);
    }


    private void OnSceneLoaded(object payload)
    {
        string sceneName = payload as string;

        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }

            // If we loaded into the GameScene, call the method to apply pending data
        if (sceneName.Equals("GameScene", StringComparison.OrdinalIgnoreCase))
        {
            OnGameSceneLoaded();
        }
        else if (sceneName.Equals("MainMenu", StringComparison.OrdinalIgnoreCase))
        {
            ApplyState(GameState.MainMenu);
        }
    }


    public void StartNewGame()
    {
        _pendingLoadData = new GameData
        {
            playerData = new PlayerData { playerName = "Captain", credits = 1000f, position = Vector3.zero },
            worldData  = new WorldData { worldSeed = UnityEngine.Random.Range(0, 1_000_000), gameTime = 0f },
            
            saveTime = DateTime.Now,
            version  = Application.version
        };


        SceneLoader.Instance?.LoadScene("GameScene");
        ApplyState(GameState.Loading);
    }


    public void LoadGame(string slotName)
    {
        _pendingLoadData = SaveManager.Instance?.LoadGameData(slotName);

        if (_pendingLoadData == null)
        {
            Debug.LogWarning($"[GameManager] Load failed for slot: {slotName}");
           
            return;
        }


        SceneLoader.Instance?.LoadScene("GameScene");
        ApplyState(GameState.Loading);
    }


    public void SaveGame(string slotName) => SaveManager.Instance?.SaveGame(slotName);


    public void ExitGame()
    {
        SaveManager.Instance?.SaveGame("autosave");
        

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    public void TogglePause()
    {
        if (currentState == GameState.Play || currentState == GameState.Paused)
        { 
            ApplyState(currentState == GameState.Play ? GameState.Paused : GameState.Play);
        }
    }


        // called when GameScene is loaded and systems are online
    public void OnGameSceneLoaded()
    {
        ApplyState(GameState.Play);

        GameEventBus.Instance?.Publish(GameEventType.GameDataReady, _pendingLoadData);
            // leaving pending data in place gives systems a chance to confirm they applied it before clearing
    }


    private void ApplyState(GameState newState)
    {
        if (currentState == newState)
        { 
            return;
        }

        currentState = newState;
        GameEventBus.Instance?.Publish(GameEventType.GameStateChanged, newState);

        switch (newState)
        {
            case GameState.Play:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            
            case GameState.Loading:
                Time.timeScale = 1f;
                break;
            
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;
            
            case GameState.Quit:
                Time.timeScale = 0.5f;
                break;
        }
    }
}


public class GameEventBus : MonoBehaviour
{
    public static GameEventBus Instance { get; private set; }

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