using UnityEngine;

namespace PlayerScripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        private Rigidbody rb;
        private PlayerControls controls;

        private Vector2 moveInput;
        private bool sprinting;

        public bool IsSprinting => sprinting;
        public Vector3 MoveDirection { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            controls = new PlayerControls();

            controls.Player.Move.performed += ctx =>
            {
                moveInput = ctx.ReadValue<Vector2>();
            };

            controls.Player.Move.canceled += ctx =>
            {
                moveInput = Vector2.zero;
            };

            controls.Player.Sprint.performed += ctx =>
            {
                sprinting = true;
            };

            controls.Player.Sprint.canceled += ctx =>
            {
                sprinting = false;
            };
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 direction =
                forward * moveInput.y +
                right * moveInput.x;

            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            MoveDirection = direction;

            float speed = sprinting ? sprintSpeed : walkSpeed;

            Vector3 velocity = direction * speed;

            rb.linearVelocity = new Vector3(
                velocity.x,
                rb.linearVelocity.y,
                velocity.z
            );

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                float currentSpeed = new Vector3(
                    rb.linearVelocity.x,
                    0f,
                    rb.linearVelocity.z
                ).magnitude;

                float rotationMultiplier = IsSprinting ? 0.5f : 1f;

                rb.MoveRotation(
                    Quaternion.Slerp(
                        rb.rotation,
                        targetRotation,
                        rotationSpeed *
                        rotationMultiplier *
                        Time.fixedDeltaTime
                    )
                );
            }
        }
    }
}