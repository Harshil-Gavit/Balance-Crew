using UnityEngine;
using TMPro; // Change to UnityEngine.UI if using legacy Text

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRadius = 2.5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask interactableLayer;

    [Header("UI Floating Text Setup")]
    [SerializeField] private RectTransform promptUIContainer; // UI Panel/Text object on your Canvas
    [SerializeField] private TextMeshProUGUI promptTextUI;   // Text component inside promptUIContainer
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 1.2f, 0); // Height above the object

    [Header("References")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Camera mainCamera;

    private void Start()
    {
        if (playerRoot == null) playerRoot = transform.root;
        if (mainCamera == null) mainCamera = Camera.main;

        if (promptUIContainer != null) promptUIContainer.gameObject.SetActive(false);
    }

    private void Update()
    {
        IInteractable nearest = GetNearestInteractable(out Transform targetTransform);

        if (nearest != null)
        {
            // 1. Show UI container
            if (promptUIContainer != null)
            {
                promptUIContainer.gameObject.SetActive(true);

                // 2. Set unique prompt text from target script
                if (promptTextUI != null)
                {
                    promptTextUI.text = nearest.Prompt;
                }

                // 3. Teleport UI element to object's screen position
                Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetTransform.position + worldOffset);
                
                // Hide prompt if target is behind camera
                if (screenPoint.z > 0)
                {
                    promptUIContainer.position = screenPoint;
                }
                else
                {
                    promptUIContainer.gameObject.SetActive(false);
                }
            }

            // 4. Handle Interaction input
            if (Input.GetKeyDown(interactKey))
            {
                nearest.Interact(playerRoot);
            }
        }
        else
        {
            if (promptUIContainer != null) promptUIContainer.gameObject.SetActive(false);
        }
    }

    private IInteractable GetNearestInteractable(out Transform targetTransform)
    {
        targetTransform = null;
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IInteractable>(out var interactable))
            {
                targetTransform = hit.transform;
                return interactable;
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}