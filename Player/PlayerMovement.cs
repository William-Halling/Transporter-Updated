using UnityEngine;
using Transporter.Inputs;


namespace Transporter.Gameplay
{


    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 9f;
        [SerializeField] private float acceleration = 12f;


        [Header("Jump & Gravity")]
        [SerializeField] private float jumpHeight = 1.6f;
        [SerializeField] private float gravity = -18f;
        [SerializeField, Range(0f, 1f)] private float airControlMultiplier = 0.5f;

        
        [Header("Grounding")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundMask = ~0;


        [Header("Timers")]
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;



        private float _coyoteTimer;
        private float _jumpBufferTimer;

        private Vector3 _currentHorizontalVelocity;
        private Vector3 _verticalVelocity;

        
        private GameInput            _playerInput;
        private CharacterController _controller;


        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }


        private void Start()
        {
            _playerInput = GameInput.Instance;


            if (_playerInput == null)
            {
                Debug.LogError("PlayerInput missing!");
            }
        }


            /// <summary>
            /// Call this when loading GameData to safely move the player.
            /// </summary>
        public void Teleport(Vector3 position)
        {
            bool wasEnabled = _controller.enabled;
            _controller.enabled = false;
            transform.position  = position;
            _controller.enabled = wasEnabled;

                // Reset velocities so we don't carry momentum after a teleport
            _currentHorizontalVelocity = Vector3.zero;
            _verticalVelocity = Vector3.zero;
        }


        private void Update()
        {
            if (_playerInput == null)
            {
                return;
            }


            // Coyote Time
            if (IsGrounded())
            {
                _coyoteTimer = coyoteTime;
            }
            else
            {
                _coyoteTimer -= Time.deltaTime;
            }


            // Jump Buffer
            if (_playerInput.ConsumeJump())
            {
                _jumpBufferTimer = jumpBufferTime;
            }
            else
            {
                _jumpBufferTimer -= Time.deltaTime;
            }
        }


        private void FixedUpdate()
        {
            if (_playerInput == null)
            {
                return;
            }

            HandleMovement();
        }


        private void HandleMovement()
        {
            bool grounded = IsGrounded();

            // Horizontal
            Vector3 inputDir = new Vector3(_playerInput.MoveInput.x, 0f, _playerInput.MoveInput.y);
            inputDir = Vector3.ClampMagnitude(inputDir, 1f);
            Vector3 moveDir = transform.TransformDirection(inputDir);


            float targetSpeed = _playerInput.IsSprinting ? sprintSpeed : walkSpeed;
            float control = grounded ? 1f : airControlMultiplier;
            Vector3 targetVel = moveDir * (targetSpeed * control);


            _currentHorizontalVelocity = Vector3.MoveTowards(_currentHorizontalVelocity, targetVel, acceleration * Time.fixedDeltaTime);


            // Vertical
            if (grounded && _verticalVelocity.y < 0f)
            {
                _verticalVelocity.y = -2f;
            }


            if (_coyoteTimer > 0f && _jumpBufferTimer > 0f)
            {
                _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _jumpBufferTimer    = 0f;
                _coyoteTimer        = 0f;
            }


            _verticalVelocity.y += gravity * Time.fixedDeltaTime;

            // Apply
            Vector3 finalVel = _currentHorizontalVelocity;
            finalVel.y       = _verticalVelocity.y;
            _controller.Move(finalVel * Time.fixedDeltaTime);
        }


        private bool IsGrounded()
        {
            if (groundCheck == null)
            {
                return _controller.isGrounded;

            }


            return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        }
    }
}