using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardUI : MonoBehaviour
{
    public GameObject[] slots;                  // Array of UI slots for each shop item (you may not need this anymore)
    public Button claimButtonPrefab;            // Prefab for the claim button
    public Transform shopPanel;                 // Parent object to hold all the items
    public List<RewardItem> shopItems;         // List of all ShopItems in your shop
    public GameObject itemDetailPanel;         // The panel to show item details
    public TMP_Text itemDescriptionText;       // Text to display the item description

    public TMP_Text itemName;
    public TMP_Text itemStats;
    public TMP_Text itemQuantityText;          // Text to display the item quantity
    public Image itemImage;                    // UI Image to display the item's image

    public InventoryObject inventory;
    public PlayerData playerData;              // Reference to the PlayerData object

    public GameObject confirmPurchasePanel;    // Panel to confirm the purchase
    public Button confirmButton;               // Button to confirm the purchase
    public GameObject insufficientGoldPanel;   // Panel to show insufficient gold message
    private RewardItem currentShopItem;        // The current shop item being claimed

    void Start()
    {
        // Check if shopItems is null or empty
        if (shopItems == null || shopItems.Count == 0)
        {
            Debug.LogError("Shop items list is empty or not assigned!");
            return;
        }

        // Set up the claim button to claim all items
        claimButtonPrefab.onClick.AddListener(OnClaimButtonClick);
        // Button claimButton = Instantiate(claimButtonPrefab, shopPanel);

        // // Optionally, set up the button's position or appearance
        // RectTransform claimButtonRect = claimButton.GetComponent<RectTransform>();
        // claimButtonRect.anchoredPosition = new Vector2(0, -100);  // Example position, adjust as needed
        // claimButtonRect.localScale = new Vector3(1, 1, 1);          // Scale the button

        // Update all slot images
        UpdateSlotImages();

        // Display the name and description of the first item
        if (shopItems.Count > 0 && shopItems[0].item != null)
        {
            itemName.text = shopItems[0].item.name;
            itemDescriptionText.text = shopItems[0].item.description;
        }
    }

    // Called when the claim button is clicked
    private void OnClaimButtonClick()
    {
        // Proceed with claiming all items
        ClaimAllItems();

        // Hide the confirm purchase panel
        confirmPurchasePanel.SetActive(false);
    }

    // Claims all the items and adds them to the player's inventory
    private void ClaimAllItems()
    {
        foreach (var shopItem in shopItems)
        {
            if (shopItem != null && shopItem.item != null)
            {
                inventory.Load();  // Load the current inventory

                Item _item = new Item(shopItem.item);

                // Add the specified quantity of the item to the inventory
                if (inventory.AddItem(_item, shopItem.quantity))
                {
                    // Save the inventory after the item is added
                    inventory.Save();
                    Debug.Log("Claimed " + shopItem.quantity + " of item: " + shopItem.item.name);
                }
                else
                {
                    Debug.LogWarning("Failed to add item to inventory: " + shopItem.item.name);
                }
            }
            else
            {
                Debug.LogWarning("Shop item or item is null.");
            }
        }

        // After claiming the items, update the slot images
        UpdateSlotImages();
    }

    // Updates the slot images to match the items in the shop
    private void UpdateSlotImages()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            var shopItem = shopItems[i];
            var slot = slots[i];

            if (slot == null || shopItem == null || shopItem.item == null || shopItem.item.uiDisplay == null)
            {
                Debug.LogWarning("Slot or ShopItem or item image is null.");
                continue;
            }

            // Find the child image component in the slot
            Transform childImage = slot.transform.Find("Image");
            if (childImage != null)
            {
                Image slotImage = childImage.GetComponent<Image>();
                if (slotImage != null)
                {
                    // Update the image to match the item
                    slotImage.sprite = shopItem.item.uiDisplay;  // Set the image sprite
                }
                else
                {
                    Debug.LogWarning("Slot image component is missing.");
                }
            }
            else
            {
                Debug.LogError("Child object 'Image' not found under slot.");
            }
        }
    }
}