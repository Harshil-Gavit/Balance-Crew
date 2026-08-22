using UnityEngine;

namespace PlayerScripts
{
    public class FallTester : MonoBehaviour
    {
        [SerializeField] private float pushForce = 10f;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                rb.AddForce(
                    -transform.right * pushForce,
                    ForceMode.Impulse
                );
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                rb.AddForce(
                    transform.right * pushForce,
                    ForceMode.Impulse
                );
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                rb.AddForce(
                    transform.forward * pushForce,
                    ForceMode.Impulse
                );
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                rb.AddForce(
                    -transform.forward * pushForce,
                    ForceMode.Impulse
                );
            }
        }
    }
}