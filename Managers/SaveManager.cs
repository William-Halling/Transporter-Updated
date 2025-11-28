using System;
using System.IO;
using Transporter.Data;
using Transporter.Gameplay; // Access PlayerManager
using UnityEngine;

public sealed class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private bool useSteamCloud = false;

    public event Action<string> OnGameSaved;
    public event Action<string> OnGameLoaded;


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


    public GameData LoadGameData(string slotName)
    {
        return LoadLocally(slotName);
    }


    // --- Saving ---
    public void SaveGame(string slotName)
    {
            // 1. GATHER DATA from the live game
        GameData data = CollectGameState();


            // 2. Write to disk
        if (useSteamCloud)
        {
            SaveToSteamCloud(data, slotName);
        }
        else
        { 
            SaveLocally(data, slotName);
        }
        OnGameSaved?.Invoke(slotName);
    }



    private GameData CollectGameState()
    {
        GameData data = new GameData();
        data.saveTime = DateTime.Now;
        data.version = Application.version;

            // 1. Get Player Data
        if (PlayerManager.Instance != null)
        {
            data.playerData = PlayerManager.Instance.SaveState();
        }
        else
        {
            Debug.LogWarning("[SaveManager] PlayerManager not found. Saving default player data.");
        
            data.playerData = new PlayerData();
        }


            // 2. Get World Data (Extend WorldManager to support this later)
            // if (WorldManager.Instance != null) data.worldData = WorldManager.Instance.SaveState()
        return data;
    }


        // --- Loading ---
    public void LoadGame(string slotName)
    {
        GameData loadedData = useSteamCloud ? LoadFromSteamCloud(slotName) : LoadLocally(slotName);

        if (loadedData != null)
        {
            ApplyGameState(loadedData);

            OnGameLoaded?.Invoke(slotName);
        }
    }



    private void ApplyGameState(GameData data)
    {
            // 1. Apply to Player
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.LoadState(data.playerData);
        }

        // 2. Apply to World
        // if (WorldManager.Instance != null) WorldManager.Instance.LoadState(data.worldData);
    }



        // --- File I/O (Kept strictly identical to your implementation) ---
    private void SaveLocally(GameData data, string slotName)
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
            var path = GetSavePath(slotName);
            Directory.CreateDirectory(Path.GetDirectoryName(path));


            File.WriteAllText(path, json);
            Debug.Log($"[SaveManager] Saved -> {path}");
        }


        catch (Exception e) 
        { 
            Debug.LogError($"[SaveManager] Save failed: {e}");
        }
    }



    private GameData LoadLocally(string slotName)
    {
        try
        {
            var path = GetSavePath(slotName);


            if (!File.Exists(path))
            { 
                return null;
            }

            var json = File.ReadAllText(path);


            return JsonUtility.FromJson<GameData>(json);
        }


        catch (Exception e) 
        { 
            Debug.LogError($"[SaveManager] Load failed: {e}"); 
            
            return null; 
        }
    }



    private string GetSavePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, $"saves/{slotName}.json");
    }



        // Placeholders
    private void SaveToSteamCloud(GameData data, string slot) => SaveLocally(data, slot);
    private GameData LoadFromSteamCloud(string slot) => LoadLocally(slot);
}