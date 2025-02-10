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

    void PurchaseItem(ShopItem item, Button purchaseButton)
    {
        if(item.itemName == "Health Potion") {
            if(playerLifeStats.healthPotionCount >= 10) 
        {
            Debug.Log("You have reacher potion Limit");
            purchaseButton.interactable = false;
            return;
        }
    }


        // Implement purchasing logic here
        if(playerLifeStats.currentCoins >= item.itemPrice) {
            playerLifeStats.currentCoins -= item.itemPrice;
            
            if(item.itemName == "Health Potion") {
                playerLifeStats.healthPotionCount++;
            }
            playerStats.OnStatsChanged(); 
            Debug.Log($"Purchased: {item.itemName} for {item.itemPrice} currency! Remaining: { playerLifeStats.currentCoins}");
        }
        else{ 
             Debug.Log($"You cannot purchase that item.");
        }
    }
}