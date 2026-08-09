// UnityEngine using directives
// Project Common using directives
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;



    public class PlayerControllerA : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private float forwardSpeed = 5f;
        [SerializeField] private float backwardSpeed = 3f;
        [SerializeField] private float strafeSpeed = 4f;
        [SerializeField] private float jumpForce = 6f;

        [Header("References Settings")]

        [Header("SphereCast Settings")]
        [SerializeField] private float sphereCastRadius = 0.5f;
        [SerializeField] private float sphereCastDistance = 0.3f;

        [Header("Layers Settings")]
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody rb;
    private Actions _action;

        // Lifecycle Methods
        private void Awake()
        {
            if (!TryGetComponent(out rb))
            {
                Debug.LogError("Rigidbody component is missing on the player object.");
                return;
            }
            rb.freezeRotation = true;
        _action = new Actions();
        }

        private void OnEnable()
        {
        _action.Enable();
            _action.Player.Jump.performed += Jump;
        }
        
        private void FixedUpdate()
        {
        bool isGrounded = IsGrounded();

            Vector2 move = _action.Player.Move.ReadValue<Vector2>();

            float x = move.x;
            float z = move.y;

            float speedZ = z > 0 ? forwardSpeed : backwardSpeed;

            Vector3 movement = new Vector3(x * strafeSpeed, 0, z * speedZ);
            movement = transform.TransformDirection(movement);

            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }

        private void OnDisable()
        {            
            _action.Player.Jump.performed -= Jump;
            _action.Disable();
        }

        // Functional Methods
        private void Jump(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }
        }
        private bool IsGrounded()
        {
            RaycastHit hit;
            return Physics.SphereCast(transform.position, sphereCastRadius, Vector3.down, out hit, sphereCastDistance, groundLayer);
        }
    }