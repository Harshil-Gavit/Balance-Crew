using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    [SerializeField] private Transform targetLimb;
    private ConfigurableJoint cj;
    private Quaternion initialLocalRotation;

    void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
        // Store the target limb's default rest pose in local space
        initialLocalRotation = targetLimb.localRotation;
    }

    void FixedUpdate()
    {
        if (targetLimb != null && cj != null)
        {
            cj.targetRotation = Quaternion.Inverse(targetLimb.localRotation) * initialLocalRotation;
        }
    }
}