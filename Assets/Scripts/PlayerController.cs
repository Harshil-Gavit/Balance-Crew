using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float strafeSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("References")]
    [SerializeField] private Rigidbody hips;
    public bool isGrounded = true;

    // Movement State Properties (Read-only for external scripts)
    public bool IsWalking { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsGrounded => isGrounded;

    // Input Caching
    private float moveForward;
    private float moveStrafe;
    private bool jumpRequested;

    private void Start()
    {
        if (hips == null)
        {
            hips = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        // Sample inputs in Update to prevent missed button presses
        moveForward = Input.GetAxisRaw("Vertical");
        moveStrafe = Input.GetAxisRaw("Horizontal");

        IsWalking = Mathf.Abs(moveForward) > 0.1f || Mathf.Abs(moveStrafe) > 0.1f;
        IsSprinting = IsWalking && moveForward > 0.1f && Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyJump();
    }

    private void ApplyMovement()
    {
        float currentSpeed = IsSprinting ? (speed * sprintMultiplier) : speed;

        // Forward / Backward
        if (Mathf.Abs(moveForward) > 0.1f)
        {
            hips.AddForce(hips.transform.forward * (moveForward * currentSpeed));
        }

        // Strafe Left / Right
        if (Mathf.Abs(moveStrafe) > 0.1f)
        {
            hips.AddForce(hips.transform.right * (moveStrafe * strafeSpeed));
        }
    }

    private void ApplyJump()
    {
        if (jumpRequested)
        {
            hips.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpRequested = false;
        }
    }
}