using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 1f;
    [SerializeField] private Transform root;   // Drag 'root' here (handles player horizontal turn)
    [SerializeField] private Transform target; // Drag 'Target' here (handles camera vertical tilt)
    private float mouseX, mouseY;

    [SerializeField] private float stomachOffSet;
    [SerializeField] ConfigurableJoint hipjoint, stomachJoint;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
    }
}