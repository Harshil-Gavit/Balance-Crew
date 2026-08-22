using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Camera")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 3f;

    [Header("Look")]
    [SerializeField] private float sensitivity = 0.1f;

    private PlayerControls controls;
    private Vector2 lookInput;

    private float yaw = 0f;
    private float pitch = 15f;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Look.performed += ctx =>
        {
            lookInput = ctx.ReadValue<Vector2>();
        };

        controls.Player.Look.canceled += ctx =>
        {
            lookInput = Vector2.zero;
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;

        pitch = Mathf.Clamp(pitch, -10f, 60f);

        Quaternion rotation =
            Quaternion.Euler(pitch, yaw, 0f);

        Vector3 offset =
            rotation * new Vector3(0f, 0f, -distance);

        transform.position =
            target.position +
            Vector3.up * height +
            offset;

        transform.LookAt(
            target.position +
            Vector3.up * height
        );
    }
}