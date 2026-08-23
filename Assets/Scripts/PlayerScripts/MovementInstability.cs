using UnityEngine;

namespace PlayerScripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementInstability : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private BalanceDetector balanceDetector;
        [SerializeField] private Rigidbody hips;

        [Header("Sprint Turning")]
        [SerializeField] private float turnAngle = 45f;
        [SerializeField] private float turnForce = 15f;

        private Vector3 previousDirection;

        private void Awake()
        {
            if (movement == null)
                movement = GetComponent<PlayerMovement>();

            if (balanceDetector == null)
                balanceDetector = GetComponent<BalanceDetector>();

            if (hips == null)
            {
                Transform hipsTransform =
                    transform.Find("mixamorig:Hips");

                if (hipsTransform != null)
                    hips = hipsTransform.GetComponent<Rigidbody>();
            }
        }

        private void FixedUpdate()
        {
            if (movement == null || hips == null)
                return;

            Vector3 currentDirection =
                movement.MoveDirection;

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
            Vector3 torqueDirection =
                Vector3.Cross(
                    hips.transform.up,
                    direction
                );

            hips.AddTorque(
                torqueDirection * turnForce,
                ForceMode.Impulse
            );
        }
    }
}