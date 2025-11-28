using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int width = 16;
    [SerializeField] private int depth = 16;
    [SerializeField] private float blockSize = 1f;
    [SerializeField] private float heightOffset = 0f;


    private void Start()
    {
        if (blockPrefab == null)
        {
            Debug.LogError("ChunkGenerator: No blockPrefab assigned.");

            return;
        }

        GenerateChunk();
    }


    private void GenerateChunk()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 position = CalculateBlockPosition(x, z);
                CreateBlockAt(position, x, z);
            }
        }
    }


    private Vector3 CalculateBlockPosition(int x, int z)
    {
        return new Vector3(x * blockSize, heightOffset, z * blockSize);
    }


    private void CreateBlockAt(Vector3 position, int x, int z)
    {
        GameObject block = Instantiate(blockPrefab, position, Quaternion.identity, transform);
        block.name = $"Block_{x}_{z}";
    }
}
