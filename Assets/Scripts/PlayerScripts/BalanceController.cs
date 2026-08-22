using UnityEngine;

namespace PlayerScripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class BalanceController : MonoBehaviour
    {
        [Header("Balance")]
        [SerializeField] private float uprightStrength = 20f;
        [SerializeField] private float damping = 5f;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            KeepUpright();
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

        public void DisableBalance()
        {
            enabled = false;
        }
    }
}