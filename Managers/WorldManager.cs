using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Transporter.World;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }


    [SerializeField] private int defaultSeed = 12345;

    private readonly List<Location> _locations = new();


    public IReadOnlyList<Location> Locations => _locations;


    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            
            return; 
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);

        ServiceLocator.Instance?.RegisterService(this);
    }



    public void GenerateWorld(int seed)
    {
        UnityEngine.Random.InitState(seed);
        _locations.Clear();

        // Hard‑coded ports for now; later these can come from procedural generation or data files.
        _locations.Add(new Location("EAST", "Eastport", new Vector3(800f, 0f, 0f)));
        _locations.Add(new Location("WEST", "Westport", new Vector3(-800f, 0f, 0f)));
        _locations.Add(new Location("NORTH", "Northbay", new Vector3(0f, 0f, 900f)));
        _locations.Add(new Location("SOUTH", "Southreef", new Vector3(0f, 0f, -900f)));

        // Notify listeners the world has been generated.
        EventSystem.Instance?.Publish(GameEventType.WorldGenerated, seed);
    }


    public void GenerateDefault() => GenerateWorld(defaultSeed);

    /// <summary>Returns the location closest to the given position, or a default Location if none exist.</summary>
    public Location GetNearestLocation(Vector3 pos)
    {
        Location best = default;
        float bestDistSq = float.MaxValue;


        foreach (var loc in _locations)
        {
            float dSq = (loc.WorldPosition - pos).sqrMagnitude;


            if (dSq < bestDistSq)
            {
                bestDistSq = dSq;
                best = loc;
            }
        }
        return best;
    }
}
