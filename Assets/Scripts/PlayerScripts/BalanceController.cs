using UnityEngine;

namespace PlayerScripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class BalanceController : MonoBehaviour
    {
        [Header("Automatic Balance")]
        [SerializeField] private float uprightStrength = 20f;
        [SerializeField] private float damping = 5f;

        [Header("Player Balance")]
        [SerializeField] private float balanceForce = 8f;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            KeepUpright();
            ApplyBalanceInput();
        }

        private void KeepUpright()
        {
            Vector3 currentUp = transform.up;
            Vector3 targetUp = Vector3.up;

            Vector3 torque =
                Vector3.Cross(currentUp, targetUp) * uprightStrength;

            torque -= rb.angularVelocity * damping;

            rb.AddTorque(torque, ForceMode.Acceleration);
        }

        private void ApplyBalanceInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 force =
                transform.right * horizontal +
                transform.forward * vertical;

            rb.AddTorque(force * balanceForce, ForceMode.Acceleration);
        }
    }
}