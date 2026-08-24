using UnityEngine;
using System.Collections;

public class WindSystem : MonoBehaviour
{
    [Header("Target Rigidbody")]
    [SerializeField] private Rigidbody playerHips;

    [Header("Timing (Seconds)")]
    [SerializeField] private float minInterval = 6f;
    [SerializeField] private float maxInterval = 14f;
    [SerializeField] private float minGustDuration = 2f;
    [SerializeField] private float maxGustDuration = 4f;

    [Header("Normal Wind (Disbalance)")]
    [SerializeField] private float normalForce = 120f;
    [SerializeField] private float normalTorque = 60f;

    [Header("Strong Wind (40% Chance - Pushes Player)")]
    [SerializeField] private float strongForce = 500f;
    [SerializeField] private float strongTorque = 180f;
    [Range(0f, 1f)]
    [SerializeField] private float strongWindChance = 0.4f;

    // Internal State
    private bool isGusting = false;
    private bool isStrongGust = false;
    private Vector3 windDirection;
    private Rigidbody[] ragdollBodies;
    private void Start()
    {
        if (playerHips == null) return;

        // Gather all child rigidbodies so limbs react dynamically to wind
        ragdollBodies = playerHips.transform.root.GetComponentsInChildren<Rigidbody>();

        StartCoroutine(WindRoutine());
    }

    private IEnumerator WindRoutine()
    {
        while (true)
        {
            // 1. Wait for random cooldown
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // 2. Roll 40% chance for a strong gust
            isStrongGust = Random.value < strongWindChance;

            // 3. Generate random 360-degree horizontal wind direction
            float randomAngle = Random.Range(0f, 360f);
            windDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;

            // 4. Run gust for random duration
            float gustDuration = Random.Range(minGustDuration, maxGustDuration);
            isGusting = true;

            yield return new WaitForSeconds(gustDuration);

            isGusting = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isGusting || playerHips == null) return;

        float currentForce = isStrongGust ? strongForce : normalForce;
        float currentTorque = isStrongGust ? strongTorque : normalTorque;

        // Push Hips directly in wind direction
        playerHips.AddForce(windDirection * currentForce, ForceMode.Force);

        // Calculate perpendicular axis to tilt character sideways relative to wind direction
        Vector3 tiltAxis = Vector3.Cross(Vector3.up, windDirection);
        playerHips.AddTorque(tiltAxis * currentTorque, ForceMode.Force);

        // Apply light force to individual limbs to destabilize active ragdoll joint drives
        if (ragdollBodies != null)
        {
            float limbMultiplier = isStrongGust ? 0.25f : 0.08f;
            foreach (Rigidbody rb in ragdollBodies)
            {
                if (rb != playerHips)
                {
                    rb.AddForce(windDirection * (currentForce * limbMultiplier), ForceMode.Force);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize wind vector in Scene window when active
        if (isGusting && playerHips != null)
        {
            Gizmos.color = isStrongGust ? Color.red : Color.cyan;
            Gizmos.DrawRay(playerHips.position + Vector3.up, windDirection * 3f);
        }
    }
}
