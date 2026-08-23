using System.Collections;
using UnityEngine;

namespace PlayerScripts
{
    public class RagdollController : MonoBehaviour
    {
        [Header("Ragdoll")]
        [SerializeField] private float ragdollDuration = 3f;

        [Header("Recovery")]
        [SerializeField] private Transform hips;

        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;

        private Rigidbody rootRigidbody;
        private Collider rootCollider;
        private Animator animator;

        private PlayerMovement playerMovement;
        private MovementInstability movementInstability;
        private BalanceController balanceController;
        private BalanceDetector balanceDetector;

        private bool isRagdoll;

        private Vector3 normalHipsLocalPosition;

        public bool IsRagdoll => isRagdoll;

        private void Awake()
        {
            rootRigidbody = GetComponent<Rigidbody>();
            rootCollider = GetComponent<Collider>();

            animator = GetComponentInChildren<Animator>();

            playerMovement = GetComponent<PlayerMovement>();
            movementInstability = GetComponent<MovementInstability>();
            balanceController = GetComponent<BalanceController>();
            balanceDetector = GetComponent<BalanceDetector>();

            ragdollRigidbodies =
                GetComponentsInChildren<Rigidbody>();

            ragdollColliders =
                GetComponentsInChildren<Collider>();

            if (hips == null)
            {
                Transform foundHips =
                    transform.Find("mixamorig:Hips");

                if (foundHips != null)
                    hips = foundHips;
            }

            if (hips != null)
            {
                normalHipsLocalPosition =
                    transform.InverseTransformPoint(
                        hips.position
                    );
            }

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

            yield return StartCoroutine(RecoverFromRagdoll());
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

            if (rootCollider != null)
                rootCollider.enabled = !enabled;

            if (animator != null)
                animator.enabled = !enabled;

            rootRigidbody.isKinematic = enabled;
        }

        private IEnumerator RecoverFromRagdoll()
{
    if (hips == null)
    {
        Debug.LogError("RagdollController: Hips is not assigned.");
        FinishRecovery();
        yield break;
    }

    // Remember exactly where the hips ended up
    Vector3 targetHipsPosition = hips.position;

    // Get a flat direction from the ragdoll
    Vector3 forward = hips.forward;
    forward.y = 0f;

    if (forward.sqrMagnitude < 0.01f)
    {
        forward = transform.forward;
        forward.y = 0f;
    }

    forward.Normalize();

    // ------------------------------------------------
    // 1. Stop root physics
    // ------------------------------------------------

    rootRigidbody.isKinematic = true;

    // ------------------------------------------------
    // 2. Stop ragdoll physics
    // ------------------------------------------------

    foreach (Rigidbody rb in ragdollRigidbodies)
    {
        if (rb == rootRigidbody)
            continue;

        rb.isKinematic = true;
    }

    foreach (Collider col in ragdollColliders)
    {
        if (col.transform == transform)
            continue;

        col.enabled = false;
    }

    // ------------------------------------------------
    // 3. Set the character upright
    // ------------------------------------------------

    Quaternion uprightRotation =
        Quaternion.LookRotation(
            forward,
            Vector3.up
        );

    transform.rotation = uprightRotation;

    // ------------------------------------------------
    // 4. Turn Animator back on
    // ------------------------------------------------

    if (animator != null)
    {
        animator.enabled = true;

        animator.Rebind();
        animator.Update(0f);
    }

    // Wait one frame so Animator finishes restoring
    // the character pose.
    yield return null;

    // ------------------------------------------------
    // 5. NOW align ROOT with the actual Hips
    // ------------------------------------------------

    Vector3 hipsOffset =
        hips.position - transform.position;

    transform.position +=
        targetHipsPosition - hips.position;

    // ------------------------------------------------
    // 6. Restore normal physics
    // ------------------------------------------------

    if (rootCollider != null)
        rootCollider.enabled = true;

    rootRigidbody.isKinematic = false;

    rootRigidbody.linearVelocity = Vector3.zero;
    rootRigidbody.angularVelocity = Vector3.zero;

    FinishRecovery();
}

        private void FinishRecovery()
        {
            isRagdoll = false;

            EnablePlayerControl();
        }
    }
}