using System.Collections;
using UnityEngine;

public class WindSystem : MonoBehaviour
{
    [Header("Indoor Safety State")]
    public bool isPlayerInside = false;

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

    [Header("Fish Spawning Settings")]
    [SerializeField] private GameObject smallFishPrefab;
    [SerializeField] private GameObject largeFishPrefab;
    [SerializeField] private GameObject goldFishPrefab;
    [SerializeField] private Transform[] shipSpawnPoints; // Random deck locations for fish landings

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

            // Roll 50% chance to spawn a fish when wind starts
            TrySpawnFish();

            yield return new WaitForSeconds(gustDuration);

            isGusting = false;
        }
    }

    private void TrySpawnFish()
    {
        // 50% chance check (Random.value returns 0.0 to 1.0)
        if (Random.value > 0.50f) return;

        // Select fish prefab based on probability weights
        GameObject fishToSpawn = ChooseFishPrefab();
        if (fishToSpawn == null) return;

        // Pick a spawn position
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        if (shipSpawnPoints != null && shipSpawnPoints.Length > 0)
        {
            Transform randomPoint = shipSpawnPoints[Random.Range(0, shipSpawnPoints.Length)];
            spawnPosition = randomPoint.position;
            spawnRotation = randomPoint.rotation;
        }
        else if (playerHips != null)
        {
            // Fallback spawn near player if spawn points aren't set up
            spawnPosition = playerHips.position + Vector3.up * 2f + Random.insideUnitSphere * 2f;
        }

        Instantiate(fishToSpawn, spawnPosition, spawnRotation);
    }

    private GameObject ChooseFishPrefab()
    {
        float roll = Random.value; // Returns float between 0.0 and 1.0

        if (roll < 0.40f)
        {
            return smallFishPrefab; // 40% Chance (0.00 - 0.39)
        }
        else if (roll < 0.80f)
        {
            return largeFishPrefab; // 40% Chance (0.40 - 0.79)
        }
        else
        {
            return goldFishPrefab;  // 20% Chance (0.80 - 0.99)
        }
    }

    private void FixedUpdate()
    {
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