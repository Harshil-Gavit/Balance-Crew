using UnityEngine;

namespace PlayerScripts
{
    public class RagdollController : MonoBehaviour
    {
        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;

        private Animator animator;
        private Rigidbody rootRigidbody;

        private void Awake()
        {
            rootRigidbody = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();

            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();

            SetRagdoll(false);
        }

        public void SetRagdoll(bool enabled)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                // Don't change Ty's main Rigidbody
                if (rb == rootRigidbody)
                    continue;

                rb.isKinematic = !enabled;
            }

            foreach (Collider col in ragdollColliders)
            {
                // Don't change Ty's own collider
                if (col.transform == transform)
                    continue;

                col.enabled = enabled;
            }

            if (animator != null)
            {
                animator.enabled = !enabled;
            }

            // Ty remains controlled by the normal physics system
            if (rootRigidbody != null)
            {
                rootRigidbody.isKinematic = false;
            }
        }

        public void EnableRagdoll()
        {
            SetRagdoll(true);
        }
    }
}