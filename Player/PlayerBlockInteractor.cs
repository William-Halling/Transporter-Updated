using System;
using System.Security.Permissions;
using UnityEngine;



public class PlayerBlockInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject blockPrefab;


    [Tooltip("A semi-transparent prefab to show placement position.")]
    [SerializeField] private GameObject ghostBlockPrefab;


    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 5.0f;


    private GameObject currentGhostBlock;
    private Vector3 currentGhostBlockLocation;
    private bool canPlaceBlock;


    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }


        if (blockPrefab == null)
        {
            Debug.LogWarning("PlayerBlockInteractor: blockPrefab not assigned.");
        }


        if (ghostBlockPrefab != null)
        {
            currentGhostBlock = Instantiate(ghostBlockPrefab);
            currentGhostBlock.SetActive(false); // Start hidden
        }
    }


    private void Update()
    {
        if (playerCamera == null)
        {
            return;
        }

        UpdateGhostBlockPreview();

        if (Input.GetMouseButtonDown(0))
        {
            TryBreakBlock();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryPlaceBlock();
        }
    }


    private void TryBreakBlock()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Block"))
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }


    private void UpdateGhostBlockPreview()
    {
        if (currentGhostBlock == null)
        { 
            return;
        }

            // Perform a raycast to find where we're looking
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Block"))
            {
                    // Calculate the new block's snapped position
                Vector3 snappedPosition = hit.collider.transform.position + hit.normal;

                currentGhostBlockLocation = snappedPosition;
                canPlaceBlock = true;


                    // Position and display the ghost block
                currentGhostBlock.transform.position = snappedPosition;
                currentGhostBlock.SetActive(true);


                return;
            }
        }

            // If we didn't hit a block, hide the ghost
        canPlaceBlock = false;
        currentGhostBlock.SetActive(false);
    }


    private void TryPlaceBlock()
    {
        if (blockPrefab == null) 
        {      
            return;
        }


        if (canPlaceBlock)
        {
            Instantiate(blockPrefab, currentGhostBlockLocation, Quaternion.identity);
        }
    }
}
