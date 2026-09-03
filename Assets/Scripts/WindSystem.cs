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
    [SerializeField] private float warningDuration = 2f; // Time warning flashes before gust hits
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
    [SerializeField] private Transform[] shipSpawnPoints;

    [Header("Wind Visual Effects Settings")]
    [SerializeField] private ParticleSystem windParticles;
    [SerializeField] private float normalEmissionRate = 250f;
    [SerializeField] private float strongEmissionRate = 750f;
    [SerializeField] private float normalParticleSpeed = 20f;
    [SerializeField] private float strongParticleSpeed = 45f;
    [SerializeField] private float normalParticleSize = 0.15f;
    [SerializeField] private float strongParticleSize = 0.35f;

    [Header("Screen Warning UI Settings")]
    [SerializeField] private CanvasGroup sideWarningGroup;      // Left & Right Red Edges
    [SerializeField] private CanvasGroup topBottomWarningGroup; // Top & Bottom Red Edges
    [SerializeField] private float pulseSpeed = 6f;

    private bool isGusting = false;
    private bool isStrongGust = false;
    private bool isWarningActive = false;
    private Vector3 windDirection;
    private Rigidbody[] ragdollBodies;

    private void Start()
    {
        if (playerHips == null) return;
        ragdollBodies = playerHips.transform.root.GetComponentsInChildren<Rigidbody>();

        if (windParticles != null) windParticles.Stop();

        // Reset UI warnings on start
        SetCanvasAlpha(sideWarningGroup, 0f);
        SetCanvasAlpha(topBottomWarningGroup, 0f);

        StartCoroutine(WindRoutine());
    }

    private void Update()
    {
        // Pulse red glow during warning state
        if (isWarningActive && !isPlayerInside)
        {
            float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // Oscillates between 0 and 1

            SetCanvasAlpha(sideWarningGroup, alpha);

            if (isStrongGust)
            {
                SetCanvasAlpha(topBottomWarningGroup, alpha);
            }
        }
    }

    private IEnumerator WindRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);

            // Wait for cooldown minus warning time
            if (waitTime > warningDuration)
            {
                yield return new WaitForSeconds(waitTime - warningDuration);
            }

            // 1. Roll strength and direction BEFORE warning starts
            isStrongGust = Random.value < strongWindChance;
            float randomAngle = Random.Range(0f, 360f);
            windDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;

            // 2. Start Warning Phase
            if (!isPlayerInside)
            {
                isWarningActive = true;
            }

            yield return new WaitForSeconds(warningDuration);

            // 3. End Warning & Start Gust
            isWarningActive = false;
            ClearUIWarnings();

            float gustDuration = Random.Range(minGustDuration, maxGustDuration);
            isGusting = true;
            UpdateWindParticles();
            TrySpawnFish();

            yield return new WaitForSeconds(gustDuration);

            // 4. End Gust
            isGusting = false;
            StopWindParticles();
        }
    }

    private void UpdateWindParticles()
    {
        if (windParticles == null || isPlayerInside) return;

        windParticles.transform.rotation = Quaternion.LookRotation(windDirection);

        var main = windParticles.main;
        var emission = windParticles.emission;

        main.maxParticles = 3000;

        if (isStrongGust)
        {
            emission.rateOverTime = strongEmissionRate;
            main.startSpeed = strongParticleSpeed;
            main.startSize = strongParticleSize;
        }
        else
        {
            emission.rateOverTime = normalEmissionRate;
            main.startSpeed = normalParticleSpeed;
            main.startSize = normalParticleSize;
        }

        windParticles.Play();
    }

    private void StopWindParticles()
    {
        if (windParticles != null && windParticles.isPlaying)
        {
            windParticles.Stop();
        }
    }

    private void ClearUIWarnings()
    {
        SetCanvasAlpha(sideWarningGroup, 0f);
        SetCanvasAlpha(topBottomWarningGroup, 0f);
    }

    private void SetCanvasAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
        {
            group.alpha = alpha;
        }
    }

    private void TrySpawnFish()
    {
        if (Random.value > 0.50f) return;

        GameObject fishToSpawn = ChooseFishPrefab();
        if (fishToSpawn == null) return;

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
            spawnPosition = playerHips.position + Vector3.up * 2f + Random.insideUnitSphere * 2f;
        }

        Instantiate(fishToSpawn, spawnPosition, spawnRotation);
    }

    private GameObject ChooseFishPrefab()
    {
        float roll = Random.value;
        if (roll < 0.40f) return smallFishPrefab;
        else if (roll < 0.80f) return largeFishPrefab;
        else return goldFishPrefab;
    }

    private void FixedUpdate()
    {
        if (isPlayerInside)
        {
            if (isWarningActive)
            {
                isWarningActive = false;
                ClearUIWarnings();
            }

            if (windParticles != null && windParticles.isPlaying)
            {
                StopWindParticles();
            }

            return;
        }

        if (!isGusting || playerHips == null) return;

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