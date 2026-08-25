using System.Collections.Generic;
using UnityEngine;
using TMPro; // Change to UnityEngine.UI if using Legacy Text

public class InventoryManager : MonoBehaviour
{
    [Header("Weight Capacity")]
    [SerializeField] private float maxWeight = 50f;
    private float currentWeight = 0f;

    [Header("Stats Tracking")]
    private int currentValueCarrying = 0;
    private int totalScore = 0;
    private int money = 0;

    [Header("UI Text Displays")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI carryingValueText;

    [System.Serializable]
    public class ItemData
    {
        public string name;
        public float weight;
        public int value;

        public ItemData(string name, float weight, int value)
        {
            this.name = name;
            this.weight = weight;
            this.value = value;
        }
    }

    public List<ItemData> inventoryList = new List<ItemData>();

    private void Start()
    {
        UpdateUI();
    }

    public bool TryAddItem(string itemName, float weight, int value)
    {
        // Rejects item if it exceeds max weight
        if (currentWeight + weight > maxWeight)
        {
            Debug.Log($"Too heavy! Cannot carry {itemName}.");
            return false;
        }

        // Add to inventory
        inventoryList.Add(new ItemData(itemName, weight, value));
        currentWeight += weight;
        currentValueCarrying += value;

        UpdateUI();
        return true;
    }

    // Called when selling at a merchant
    public void SellAllItems()
    {
        if (inventoryList.Count == 0) return;

        totalScore += currentValueCarrying;
        money += currentValueCarrying;

        // Reset carrying metrics
        inventoryList.Clear();
        currentWeight = 0f;
        currentValueCarrying = 0;

        UpdateUI();
    }

    // Spend money on upgrades/purchases
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {totalScore}";
        if (moneyText != null) moneyText.text = $"Money: ${money}";
        if (weightText != null) weightText.text = $"Weight: {currentWeight:F1} / {maxWeight:F1} kg";
        if (carryingValueText != null) carryingValueText.text = $"Carrying Value: ${currentValueCarrying}";
    }
}