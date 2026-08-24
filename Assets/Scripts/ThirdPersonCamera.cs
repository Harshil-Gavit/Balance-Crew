using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private Transform root;   // Drag 'root' here (handles player horizontal turn)
    [SerializeField] private Transform target; // Drag 'Target' here (handles camera vertical tilt)
    private float mouseX, mouseY;

    [Header("Joint Settings")]
    [SerializeField] private float stomachOffSet;
    [SerializeField] private ConfigurableJoint hipjoint, stomachJoint;

    [Header("Camera Collision Settings")]
    [SerializeField] private Transform cameraTransform; // Drag 'Main Camera' here
    [SerializeField] private float maxDistance = 3.5f;   // Normal distance behind player
    [SerializeField] private float minDistance = 0.5f;   // Minimum distance when pressed against a wall
    [SerializeField] private float cameraRadius = 0.2f;  // Sphere thickness to prevent wall clipping
    [SerializeField] private LayerMask obstacleLayers;   // Layers camera should hit (Default, Ground, Wall, etc.)
    [SerializeField] private float smoothSpeed = 25f;

    private float currentDistance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentDistance = maxDistance;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void FixedUpdate()
    {
        CamControl();
    }

    void CamControl()
    {
        mouseX += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -45f, 45f);
        
        // 1. Rotate 'root' on Y-axis ONLY (Turns character left/right without tilting up/down)
        if (root != null)
        {
            root.rotation = Quaternion.Euler(0f, mouseX, 0f);
        }

        // 2. Rotate 'target' on X-axis ONLY (Tilts camera up/down relative to root)
        if (target != null)
        {
            target.localRotation = Quaternion.Euler(mouseY, 0f, 0f);
        }
        
        // 3. Joint alignment relative to root
        if (hipjoint != null)
        {
            hipjoint.targetRotation = Quaternion.identity;
        }

        if (stomachJoint != null)
        {
            stomachJoint.targetRotation = Quaternion.Euler(stomachOffSet, 0f, 0f);
        }

        // 4. Adjust camera position dynamically based on physics obstacles
        HandleCameraCollision();
    }

    private void HandleCameraCollision()
    {
        if (target == null || cameraTransform == null) return;

        // Cast ray backward along camera target angle
        Vector3 castDirection = -target.forward;
        float desiredDistance = maxDistance;

        // SphereCast checks for walls between Target pivot and max camera distance
        if (Physics.SphereCast(target.position, cameraRadius, castDirection, out RaycastHit hit, maxDistance, obstacleLayers))
        {
            desiredDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        // Smooth distance adjustments to avoid instant snapping
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.fixedDeltaTime * smoothSpeed);

        // Apply distance to local Z offset of camera
        cameraTransform.localPosition = new Vector3(0f, 0f, -currentDistance);
    }
}