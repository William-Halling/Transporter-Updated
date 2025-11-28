using System.Collections.Generic;


using UnityEngine;


public class BlockTypeLookup
{
    private readonly Dictionary<BlockId, Block_Type> _lookupById;


    public BlockTypeLookup(Block_Type[] assets)
    {
        _lookupById = new Dictionary<BlockId, Block_Type>();

        if (assets == null)
        {  
            return;
        }

        foreach (Block_Type asset in assets)
        {
            if (asset == null)
            {
                // FIX: This now correctly uses Unity's Debug class
                Debug.LogWarning("BlockTypeLookup: null entry in assets array.");

                continue;
            }

            BlockId key = asset.BlockId;


            if (!_lookupById.ContainsKey(key))
            {
                _lookupById.Add(key, asset);
            }
            else
            {
                // FIX: This now correctly uses Unity's Debug class
                Debug.LogWarning($"BlockTypeLookup: duplicate asset for {key} (asset name: {asset.name}).");
            }
        }
    }


    public Block_Type Get(BlockId id)
    {
        _lookupById.TryGetValue(id, out Block_Type foundType);


        return foundType;
    }


    public bool TryGetBlockType(BlockId id, out Block_Type blockType)
    {
        return _lookupById.TryGetValue(id, out blockType);
    }
}