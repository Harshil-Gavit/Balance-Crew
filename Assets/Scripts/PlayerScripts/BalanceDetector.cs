using UnityEngine;

public enum BalanceState
{
    Stable,
    Unstable,
    Falling,
    Fallen
}

public enum FallDirection
{
    None,
    Left,
    Right,
    Forward,
    Backward
}

public class BalanceDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform balancePoint;

    [Header("Angle Settings")]
    [SerializeField] private float unstableAngle = 10f;
    [SerializeField] private float fallingAngle = 30f;
    [SerializeField] private float fallenAngle = 70f;

    [Header("Output")]
    public BalanceState state { get; private set; }
    public FallDirection direction { get; private set; }

    public float tiltAngle { get; private set; }

    // -1 to +1
    public float balanceX { get; private set; }
    public float balanceZ { get; private set; }

    // 0 = upright
    // 1 = completely fallen
    public float fallAmount { get; private set; }

    private void Update()
    {
        CalculateBalance();
        CalculateState();
        CalculateDirection();
    }

    private void CalculateBalance()
    {
        // How far are we tilted from upright?
        tiltAngle = Vector3.Angle(
            balancePoint.up,
            Vector3.up
        );

        // Convert world UP into local character space
        Vector3 localUp =
            balancePoint.InverseTransformDirection(Vector3.up);

        balanceX = Mathf.Clamp(localUp.x, -1f, 1f);
        balanceZ = Mathf.Clamp(localUp.z, -1f, 1f);

        // Convert angle to 0-1
        fallAmount = Mathf.InverseLerp(
            0f,
            fallenAngle,
            tiltAngle
        );
    }

    private void CalculateState()
    {
        if (tiltAngle >= fallenAngle)
        {
            state = BalanceState.Fallen;
        }
        else if (tiltAngle >= fallingAngle)
        {
            state = BalanceState.Falling;
        }
        else if (tiltAngle >= unstableAngle)
        {
            state = BalanceState.Unstable;
        }
        else
        {
            state = BalanceState.Stable;
        }
    }

    private void CalculateDirection()
    {
        if (state == BalanceState.Stable)
        {
            direction = FallDirection.None;
            return;
        }

        if (Mathf.Abs(balanceX) > Mathf.Abs(balanceZ))
        {
            if (balanceX > 0)
                direction = FallDirection.Right;
            else
                direction = FallDirection.Left;
        }
        else
        {
            if (balanceZ > 0)
                direction = FallDirection.Forward;
            else
                direction = FallDirection.Backward;
        }
    }
}