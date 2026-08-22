using System.Collections;
using UnityEngine;

namespace PlayerScripts
{
    public class RagdollController : MonoBehaviour
    {
        [Header("Ragdoll")]
        [SerializeField] private float ragdollDuration = 3f;

        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;

        private Rigidbody rootRigidbody;
        private Animator animator;

        private PlayerMovement playerMovement;
        private MovementInstability movementInstability;
        private BalanceController balanceController;
        private BalanceDetector balanceDetector;

        private bool isRagdoll;

        public bool IsRagdoll => isRagdoll;

        private void Awake()
        {
            rootRigidbody = GetComponent<Rigidbody>();

            animator = GetComponentInChildren<Animator>();

            playerMovement = GetComponent<PlayerMovement>();
            movementInstability = GetComponent<MovementInstability>();
            balanceController = GetComponent<BalanceController>();
            balanceDetector = GetComponent<BalanceDetector>();

            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();

            SetRagdoll(false);
        }

        public void EnableRagdoll()
        {
            if (isRagdoll)
                return;

            isRagdoll = true;

            DisablePlayerControl();

            SetRagdoll(true);

            StartCoroutine(RagdollRoutine());
        }

        private IEnumerator RagdollRoutine()
        {
            yield return new WaitForSeconds(ragdollDuration);

            RecoverFromRagdoll();
        }

        private void DisablePlayerControl()
        {
            if (playerMovement != null)
                playerMovement.enabled = false;

            if (movementInstability != null)
                movementInstability.enabled = false;

            if (balanceController != null)
                balanceController.enabled = false;

            if (balanceDetector != null)
                balanceDetector.enabled = false;
        }

        private void EnablePlayerControl()
        {
            if (playerMovement != null)
                playerMovement.enabled = true;

            if (movementInstability != null)
                movementInstability.enabled = true;

            if (balanceController != null)
                balanceController.enabled = true;

            if (balanceDetector != null)
                balanceDetector.enabled = true;
        }

        public void SetRagdoll(bool enabled)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb == rootRigidbody)
                    continue;

                rb.isKinematic = !enabled;
            }

            foreach (Collider col in ragdollColliders)
            {
                if (col.transform == transform)
                    continue;

                col.enabled = enabled;
            }

            if (animator != null)
                animator.enabled = !enabled;

            if (!enabled)
            {
                rootRigidbody.isKinematic = false;
            }
        }

        private void RecoverFromRagdoll()
        {
            // Stop ragdoll physics
            SetRagdoll(false);

            // Clear root physics
            rootRigidbody.linearVelocity = Vector3.zero;
            rootRigidbody.angularVelocity = Vector3.zero;

            // Stand upright
            Vector3 rotation = transform.eulerAngles;

            transform.rotation = Quaternion.Euler(
                0f,
                rotation.y,
                0f
            );

            isRagdoll = false;

            EnablePlayerControl();
        }
    }
}