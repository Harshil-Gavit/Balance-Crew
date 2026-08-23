using UnityEngine;

namespace PlayerScripts
{
    public class BalanceController : MonoBehaviour
    {
        [Header("Balance")]
        [SerializeField] private float uprightStrength = 10f;
        [SerializeField] private float damping = 3f;

        [Header("Physics")]
        [SerializeField] private Rigidbody hips;

        private void Awake()
        {
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
            if (hips == null)
                return;

            KeepUpright();
        }

        private void KeepUpright()
        {
            Vector3 currentUp = hips.transform.up;

            Vector3 torque =
                Vector3.Cross(
                    currentUp,
                    Vector3.up
                ) * uprightStrength;

            torque -= hips.angularVelocity * damping;

            hips.AddTorque(
                torque,
                ForceMode.Acceleration
            );
        }
    }
}