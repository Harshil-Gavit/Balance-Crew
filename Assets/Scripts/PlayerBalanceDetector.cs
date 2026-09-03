using UnityEngine;

public class PlayerBalanceDetector : MonoBehaviour
{
    public enum BalanceState { Stable, Tilting, Fallen }
    public enum TiltDirection { None, Forward, Backward, Left, Right }

    [Header("References")]
    [SerializeField] private Rigidbody hipsRigidbody; 
    [SerializeField] private Transform root;          

    [Header("Angle Thresholds (Degrees)")]
    [SerializeField] private float tiltThreshold = 15f;   
    [SerializeField] private float fallenThreshold = 60f; 

    [Header("Tilt Timing")]
    [Tooltip("Time (in seconds) the player must continuously tilt before triggering the Tilting state.")]
    [SerializeField] private float tiltDelay = 0.5f; 

    [Header("Impact & Sudden Velocity Thresholds")]
    [SerializeField] private float maxAccelerationThreshold = 25f; 
    [SerializeField] private float inertiaKnockdownForce = 150f;   

    [Header("Current Status (Read-Only)")]
    public BalanceState currentState = BalanceState.Stable;
    public TiltDirection currentTiltDirection = TiltDirection.None;
    public float currentTiltAngle = 0f;
    public float currentAccelerationMagnitude = 0f;

    [Header("Hardware Feed Vector")]
    public Vector2 normalizedTiltVector; 

    private Vector3 lastVelocity;
    private BalanceState previousState = BalanceState.Stable;
    private float tiltTimer = 0f;

    private void Start()
    {
        if (hipsRigidbody != null)
        {
            lastVelocity = hipsRigidbody.linearVelocity;
        }
    }

    private void FixedUpdate()
    {
        if (hipsRigidbody == null || root == null) return;

        DetectSuddenVelocityChange();
        DetectBalance();
    }

    private void DetectSuddenVelocityChange()
    {
        Vector3 currentVelocity = hipsRigidbody.linearVelocity;
        Vector3 acceleration = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;
        currentAccelerationMagnitude = acceleration.magnitude;

        if (currentAccelerationMagnitude >= maxAccelerationThreshold && currentState != BalanceState.Fallen)
        {
            Debug.Log($"SUDDEN VELOCITY CHANGE! Force: {currentAccelerationMagnitude:F1} m/s²");
            
            Vector3 inertiaDirection = (lastVelocity.sqrMagnitude > currentVelocity.sqrMagnitude) 
                ? lastVelocity.normalized   
                : -acceleration.normalized; 

            hipsRigidbody.AddForce(inertiaDirection * inertiaKnockdownForce, ForceMode.Impulse);
            currentState = BalanceState.Fallen;
        }

        lastVelocity = currentVelocity;
    }

    private void DetectBalance()
    {
        currentTiltAngle = Vector3.Angle(hipsRigidbody.transform.up, Vector3.up);
        Vector3 localHipsUp = root.InverseTransformDirection(hipsRigidbody.transform.up);
        normalizedTiltVector = new Vector2(localHipsUp.x, localHipsUp.z).normalized;

        // 1. Check for full fall
        if (currentTiltAngle >= fallenThreshold || currentState == BalanceState.Fallen)
        {
            currentState = BalanceState.Fallen;
            currentTiltDirection = CalculateDominantDirection(localHipsUp.x, localHipsUp.z);
            tiltTimer = 0f;

            if (previousState != BalanceState.Fallen)
            {
                Debug.Log("PLAYER FELL OVER!");
            }
        }
        // 2. Check for leaning past threshold (with delay timer)
        else if (currentTiltAngle >= tiltThreshold)
        {
            tiltTimer += Time.fixedDeltaTime;

            if (tiltTimer >= tiltDelay)
            {
                currentState = BalanceState.Tilting;
                currentTiltDirection = CalculateDominantDirection(localHipsUp.x, localHipsUp.z);
            }
            else
            {
                // Still within grace period
                currentState = BalanceState.Stable;
                currentTiltDirection = TiltDirection.None;
            }
        }
        // 3. Player is upright and balanced
        else
        {
            tiltTimer = 0f;
            currentState = BalanceState.Stable;
            currentTiltDirection = TiltDirection.None;
        }

        previousState = currentState;
    }

    private TiltDirection CalculateDominantDirection(float localX, float localZ)
    {
        if (Mathf.Abs(localZ) >= Mathf.Abs(localX))
        {
            return localZ > 0 ? TiltDirection.Forward : TiltDirection.Backward;
        }
        else
        {
            return localX > 0 ? TiltDirection.Right : TiltDirection.Left;
        }
    }
}