using UnityEngine;


public enum BlockId { Air = 0, Dirt = 1, Stone = 2 }
public enum ToolType { None = 0, Hand = 1, Pickaxe = 2, dynamite = 3 }


[CreateAssetMenu(fileName = "BlockType_", menuName = "World/Block Type")]

public class Block_Type : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private BlockId Id;
    [SerializeField] private string displayName;


    [Header("Visuals")]
    [SerializeField] private Material blockMaterial;


    [Tooltip("If true, this block has collision and a mesh. If false (like Air), it is ignored.")]
    [SerializeField] private bool isSolid = true;


    [Header("Mining")]
    [Tooltip("Tool required to obtain the block drop when mined.")]
    [SerializeField] private ToolType requiredMiningTool = ToolType.Hand;
    [SerializeField] private bool isDropped = true;


    public BlockId BlockId             => Id;
    public string DisplayName          => displayName;
    public Material BlockMaterial      => blockMaterial;
    public bool IsSolid                => isSolid;
    public ToolType RequiredMiningTool => requiredMiningTool;
    public bool IsDropped              => isDropped;
}