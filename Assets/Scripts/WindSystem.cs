using System.Collections;
using UnityEngine;

public class WindSystem : MonoBehaviour
{
    [Header("Indoor Safety State")]
    public bool isPlayerInside = false; // Set automatically by doors, or check manually in Inspector for testing

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

    private bool isGusting = false;
    private bool isStrongGust = false;
    private Vector3 windDirection;
    private Rigidbody[] ragdollBodies;

    private void Start()
    {
        if (playerHips == null) return;
        ragdollBodies = playerHips.transform.root.GetComponentsInChildren<Rigidbody>();
        StartCoroutine(WindRoutine());
    }

    private IEnumerator WindRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            isStrongGust = Random.value < strongWindChance;

            float randomAngle = Random.Range(0f, 360f);
            windDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;

            float gustDuration = Random.Range(minGustDuration, maxGustDuration);
            isGusting = true;

            yield return new WaitForSeconds(gustDuration);

            isGusting = false;
        }
    }

    private void FixedUpdate()
    {
        // Safe from wind if inside, if no gust is active, or if hips reference is missing
        if (isPlayerInside || !isGusting || playerHips == null) return;

        float currentForce = isStrongGust ? strongForce : normalForce;
        float currentTorque = isStrongGust ? strongTorque : normalTorque;

        playerHips.AddForce(windDirection * currentForce, ForceMode.Force);

        Vector3 tiltAxis = Vector3.Cross(Vector3.up, windDirection);
        playerHips.AddTorque(tiltAxis * currentTorque, ForceMode.Force);

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
        if (!isPlayerInside && isGusting && playerHips != null)
        {
            Gizmos.color = isStrongGust ? Color.red : Color.cyan;
            Gizmos.DrawRay(playerHips.position + Vector3.up, windDirection * 3f);
        }
    }
}