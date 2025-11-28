using UnityEngine;


namespace Transporter.Inputs
{
        // Renamed to GameInput to avoid conflict with UnityEngine.InputSystem.PlayerInput
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; private set; }

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        
            // --- Action Inputs ---
        public bool JumpInputDown { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool InteractPressed { get; private set; }

            // --- Block Interaction Inputs ---
        public bool PrimaryAction { get; private set; }   // Left Click (Break)
        public bool SecondaryAction { get; private set; } // Right Click (Place)


            // --- Sensitivity Inputs ---
        public bool VerticalSensUp { get; private set; }
        public bool VerticalSensDown { get; private set; }
        public bool HorizontalSensUp { get; private set; }
        public bool HorizontalSensDown { get; private set; }

        private bool _jumpBuffered;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
        
                return;
            }

            Instance = this;
        }


        private void Update()
        {
                // Standard Movement
            MoveInput   = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            LookInput   = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            IsSprinting = Input.GetKey(KeyCode.LeftShift);


                // Actions
            if (Input.GetKeyDown(KeyCode.Space))
            { 
                _jumpBuffered = true;
            }

            InteractPressed = Input.GetKeyDown(KeyCode.E);


                // Mouse Actions
            PrimaryAction   = Input.GetMouseButtonDown(0);
            SecondaryAction = Input.GetMouseButtonDown(1);


                // Sensitivity
            VerticalSensUp      = Input.GetKeyDown(KeyCode.PageUp);
            VerticalSensDown    = Input.GetKeyDown(KeyCode.PageDown);
            HorizontalSensUp    = Input.GetKeyDown(KeyCode.Home);
            HorizontalSensDown  = Input.GetKeyDown(KeyCode.End);
        }


        public bool ConsumeJump()
        {
            if (_jumpBuffered)
            {
                _jumpBuffered = false;

                return true;
            }
            return false;
        }
    }
}