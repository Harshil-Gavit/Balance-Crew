using UnityEngine;

public class DoorTeleporter : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private string promptText = "[E] Enter Building";
    [SerializeField] private Transform destinationPoint;

    [Header("Indoor / Outdoor State")]
    [Tooltip("Check TRUE if this door leads inside (safe from wind). Check FALSE if it leads outside.")]
    [SerializeField] private bool takesPlayerInside = true;

    public string Prompt => promptText;

    public void Interact(Transform playerRoot)
    {
        if (destinationPoint == null || playerRoot == null) return;

        // 1. Calculate offset and teleport player
        Rigidbody hipsRb = playerRoot.GetComponentInChildren<Rigidbody>();

        if (hipsRb != null)
        {
            Vector3 hipsOffset = hipsRb.position - playerRoot.position;
            playerRoot.position = destinationPoint.position - hipsOffset;
            playerRoot.rotation = destinationPoint.rotation;

            Rigidbody[] childRigidbodies = playerRoot.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in childRigidbodies)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            playerRoot.position = destinationPoint.position;
            playerRoot.rotation = destinationPoint.rotation;
        }

        Physics.SyncTransforms();

        // 2. Update Wind System protection status
        WindSystem wind = FindFirstObjectByType<WindSystem>();
        if (wind != null)
        {
            wind.isPlayerInside = takesPlayerInside;
        }
    }
}