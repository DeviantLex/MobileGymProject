using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject panelPrefab; // Reference to the panel prefab
    public Transform panelParent; // Parent transform to hold the panels
    public ShopItem[] shopItems;  // Array of ShopItem data
    public PlayerLifeStats playerLifeStats; 
    public PlayerStats playerStats;
    public bool itemPurchased = false;
    public DailyChallengeManager dailyChallengeManager;

    void Start()
    {
        GenerateShopPanels();
    }

    void GenerateShopPanels()
    {
        foreach (ShopItem item in shopItems)
        {
            // Instantiate a new panel
            GameObject newPanel = Instantiate(panelPrefab, panelParent);

            // Set the image
            Image itemImage = newPanel.transform.Find("ItemImage").GetComponent<Image>();
            if (itemImage != null)
                itemImage.sprite = item.itemImage;
                

            // Set the description
            TextMeshProUGUI itemDescription = newPanel.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();     
            if (itemDescription != null)
                itemDescription.text = $"{item.itemName}\n{item.itemDescription}\nPrice: {item.itemPrice}";

            // Optional: Add button functionality
            Button purchaseButton = newPanel.transform.Find("PurchaseButton").GetComponent<Button>();
            if (purchaseButton != null)
            {
                purchaseButton.onClick.AddListener(() => PurchaseItem(item, purchaseButton));
            }
        }
    }

    public void PurchaseItem(ShopItem item, Button purchaseButton)
    {
        // Ensure purchasing conditions are met
        if (item.itemName == "Health Potion" && playerLifeStats.healthPotionCount >= 10)
        {
            Debug.Log("You have reached the potion limit!");
            purchaseButton.interactable = false;
            return;
        }

        if (playerLifeStats.currentCoins >= item.itemPrice)
        {
            // Deduct coins
            playerLifeStats.currentCoins -= item.itemPrice;

            // Handle item-specific logic
            if (item.itemName == "Health Potion")
            {
                playerLifeStats.healthPotionCount++;
            }

            playerStats.OnStatsChanged(); // Update stats
            Debug.Log($"Purchased: {item.itemName} for {item.itemPrice} currency! Remaining: {playerLifeStats.currentCoins}");

            // ✅ Mark the purchase and update the challenge system
            itemPurchased = true;
            dailyChallengeManager.CheckChallenges(); // 🔹 Now this will trigger properly!

            // Disable the purchase button to prevent re-buying
            purchaseButton.interactable = false;
        }
        else
        {
            Debug.Log("You cannot purchase that item.");
        }
    }
}