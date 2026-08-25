using UnityEngine;

public class CollectibleItem : MonoBehaviour, IInteractable
{
    [Header("Item Properties")]
    [SerializeField] private string itemName = "Loot";
    [SerializeField] private float itemWeight = 5f;
    [SerializeField] private int itemValue = 25;

    public string Prompt => $"[E] Pick up {itemName} ({itemWeight}kg | ${itemValue})";

    public void Interact(Transform playerRoot)
    {
        InventoryManager inventory = playerRoot.GetComponentInChildren<InventoryManager>();
        
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<InventoryManager>();
        }

        if (inventory != null)
        {
            // Attempt pickup; fails if weight limit is exceeded
            bool pickedUp = inventory.TryAddItem(itemName, itemWeight, itemValue);
            
            if (pickedUp)
            {
                Destroy(gameObject);
            }
        }
    }
}