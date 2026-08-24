using UnityEngine;

public class DoorTeleporter : MonoBehaviour, IInteractable
{
    [SerializeField] private string promptText = "[E] Enter Door";
    [SerializeField] private Transform destinationPoint;

    // Interface Implementation
    public string Prompt => promptText;

    public void Interact(Transform playerRoot)
    {
        if (destinationPoint == null || playerRoot == null) return;

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
    }
}