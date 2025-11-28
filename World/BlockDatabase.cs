using System.Collections.Generic;
using System.Linq;
using UnityEngine;



[CreateAssetMenu(fileName = "BlockDatabase", menuName = "World/Block Database")]
public class BlockDatabase : ScriptableObject
{
    // Singleton Instance for global access
    public static BlockDatabase Instance { get; private set; }


    [Tooltip("Assign Block_Type assets here (Dirt, Stone, ...).")]
    [SerializeField] private Block_Type[] environmentTypes; // The source of truth


    // Runtime structures
    private BlockTypeLookup _lookupByBlockId;
    private Block_Type[] _blockTypesByIdArray;


    private void OnEnable()
    {
        Instance = this;

        Initialize();
    }


        // Ensure Instance is set if we start the game without reloading domain
    private void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
        }

        Initialize();
    }


    private void Initialize()
    {
        _lookupByBlockId = new BlockTypeLookup(environmentTypes);

        _blockTypesByIdArray = BuildArrayById(environmentTypes);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        // Editor-time validation
        new BlockTypeLookup(environmentTypes); 
    }
#endif



    public Block_Type Get(BlockId id)
    {
        if (_blockTypesByIdArray == null)
        { 
            Initialize();
        }

        int idx = (int)id;


        if (idx >= 0 && idx < _blockTypesByIdArray.Length)
        {
            return _blockTypesByIdArray[idx];
        }


        return _lookupByBlockId.Get(id);
    }



    public bool TryGetBlockType(BlockId id, out Block_Type blockType)
    {
        blockType = Get(id);

        return blockType != null;
    }



    private static Block_Type[] BuildArrayById(Block_Type[] assets)
    {
        if (assets == null || assets.Length == 0)
        { 
            return null;
        }


            // Uses public BlockId property
        int maxId = assets.Where(a => a != null).Select(a => (int)a.BlockId).DefaultIfEmpty(0).Max();
        var arr = new Block_Type[maxId + 1];


        foreach (var bt in assets)
        {
            if (bt != null)
            { 
                arr[(int)bt.BlockId] = bt;
            }
        }


        return arr;
    }
}