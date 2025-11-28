using Transporter.Data;
using UnityEngine;


namespace Transporter.Gameplay
{
    public class PlayerManager : MonoBehaviour
    {

        public static PlayerManager Instance { get; private set; }

        [Header("Sub-Components")]
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerLook look;
        [SerializeField] private PlayerBlockInteractor interactor;


            // This acts as the runtime container for your persistent data
        public PlayerData RuntimeData { get; private set; }


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;


                // Auto-find components if not assigned
            if (movement == null)
            {
                movement = GetComponent<PlayerMovement>();
            }


            if (look == null)
            { 
                look = GetComponent<PlayerLook>();
            }
            
            if (interactor == null)
            {    
                interactor = GetComponent<PlayerBlockInteractor>();
            }
        }


        public void LoadState(PlayerData data)
        {
            RuntimeData = data;

                // Apply Position
            if (movement != null)
            {
                movement.Teleport(RuntimeData.position);
            }

                // Future: Apply rotation, health, inventory from RuntimeData
            Debug.Log($"[PlayerManager] Loaded state for {RuntimeData.playerName}");
        }


        public PlayerData SaveState()
        {
            if (RuntimeData == null)
            {
                RuntimeData = new PlayerData();
            }

            // Capture Position
            RuntimeData.position = transform.position;


                // Capture any other stats (Credits, XP, etc.)
                // ActiveData.rotation = transform.rotation (you need to add rotation to PlayerData first)
            return RuntimeData;
        }


            /// <summary>
            /// Called by your SaveSystem before writing to disk.
            /// Updates the data object with the current state of the world.
            /// </summary>
        public PlayerData PrepareSaveData()
        {
            if (movement != null)
            {
                    // Update the data container with current transform position
                RuntimeData.position = transform.position;
            }


            return RuntimeData;
        }
    }
}