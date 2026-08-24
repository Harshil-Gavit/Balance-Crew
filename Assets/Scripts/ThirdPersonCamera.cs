using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 1f;
    [SerializeField] private Transform target;
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
        
        Quaternion rootRotation = Quaternion.Euler(mouseY, mouseX, 0);
        
        target.rotation = rootRotation;
        
        hipjoint.targetRotation = Quaternion.Euler(0, -mouseX, 0);
        stomachJoint.targetRotation = Quaternion.Euler(-mouseY + stomachOffSet, 0, 0);
    }
}