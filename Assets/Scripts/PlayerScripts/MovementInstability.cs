using UnityEngine;

namespace PlayerScripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementInstability : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private BalanceDetector balanceDetector;

        [Header("Sprint Turning")]
        [SerializeField] private float turnAngle = 60f;
        [SerializeField] private float turnForce = 8f;

        private Rigidbody rb;

        private Vector3 previousDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            if (movement == null)
                movement = GetComponent<PlayerMovement>();

            if (balanceDetector == null)
                balanceDetector = GetComponent<BalanceDetector>();
        }

        private void FixedUpdate()
        {
            Vector3 currentDirection = movement.MoveDirection;

            if (!movement.IsSprinting)
            {
                previousDirection = currentDirection;
                return;
            }

            if (previousDirection.sqrMagnitude > 0.01f &&
                currentDirection.sqrMagnitude > 0.01f)
            {
                float angle = Vector3.Angle(
                    previousDirection,
                    currentDirection
                );

                if (angle >= turnAngle)
                {
                    MakeUnstable(currentDirection);
                }
            }

            previousDirection = currentDirection;
        }

        private void MakeUnstable(Vector3 direction)
        {
            // Push the upper body sideways by applying torque.
            Vector3 torqueDirection =
                Vector3.Cross(transform.up, direction);

            rb.AddTorque(
                torqueDirection * turnForce,
                ForceMode.Impulse
            );
        }
    }
}