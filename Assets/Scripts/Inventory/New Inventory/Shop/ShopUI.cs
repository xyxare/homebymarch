using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopUI : MonoBehaviour
{
    public GameObject[] slots;                  // Array of UI slots for each shop item
    public Button claimButtonPrefab;           // Prefab for the claim button
    public Transform shopPanel;                // Parent object to hold all the items
    public List<ShopItem> shopItems;           // List of all ShopItems in your shop
    public GameObject itemDetailPanel;         // The panel to show item details
    public TMP_Text itemDescriptionText;       // Text to display the item description

    public TMP_Text itemName;
    public TMP_Text itemStats;
    public TMP_Text itemPriceText;             // Text to display the item price
    public Image itemImage;                    // UI Image to display the item's image

    public InventoryObject inventory;
    public PlayerData playerData;              // Reference to the PlayerData object

    public GameObject confirmPurchasePanel;    // Panel to confirm the purchase
    public Button confirmButton;               // Button to confirm the purchase
    public GameObject insufficientGoldPanel;   // Panel to show insufficient gold message
    private ShopItem currentShopItem;          // The current shop item being claimed

    private Dictionary<GameObject, ShopItem> slotsOnInterface;

    void Start()
    {
        slotsOnInterface = new Dictionary<GameObject, ShopItem>();

        for (int i = 0; i < shopItems.Count; i++)
        {
            var shopItem = shopItems[i];
            var slot = slots[i];

            if (slot == null || shopItem == null)
            {
                Debug.LogError("Slot or ShopItem is null!");
                continue;
            }

            // Set up only the click event for the slot (OnClick)
            AddEvent(slot, EventTriggerType.PointerClick, delegate { OnSlotClick(slot, shopItem); });

            // Set up claim button
            Button claimButton = Instantiate(claimButtonPrefab, slot.transform);
            claimButton.onClick.AddListener(() => OnClaimButtonClick(shopItem));

            // Optionally, set the position manually in the script (if you want a default setting):
            RectTransform claimButtonRect = claimButton.GetComponent<RectTransform>();
            claimButtonRect.anchoredPosition = new Vector2(1, -70);
            claimButtonRect.localScale = new Vector3(0.5f, 0.5f, 0.4f);

            // Update the image inside the slot
            Transform childImage = slot.transform.Find("Image");
            if (childImage != null)
            {
                Image slotImage = childImage.GetComponent<Image>();
                if (slotImage != null && shopItem.item != null && shopItem.item.uiDisplay != null)
                {
                    slotImage.sprite = shopItem.item.uiDisplay;
                }
                else
                {
                    Debug.LogError("Slot image or shopItem.item.uiDisplay is null!");
                }
            }
            else
            {
                Debug.LogError("Child object 'Image' not found under slot.");
            }

            slotsOnInterface.Add(slot, shopItem);
        }

        // Hide the confirm purchase panel and insufficient gold panel initially
        confirmPurchasePanel.SetActive(false);
        insufficientGoldPanel.SetActive(false);

        // Add listener to the confirm button
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    private void AddEvent(GameObject slot, EventTriggerType triggerType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = slot.GetComponent<EventTrigger>();
        if (trigger == null) trigger = slot.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = triggerType
        };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void OnClaimButtonClick(ShopItem shopItem)
    {
        // Check if player has enough gold
        if (playerData.gold >= shopItem.price)
        {
            currentShopItem = shopItem;
            confirmPurchasePanel.SetActive(true); // Show the confirmation panel
        }
        else
        {
            insufficientGoldPanel.SetActive(true); // Show the insufficient gold panel
        }
    }

    private void OnConfirmButtonClick()
    {
        OnClaim(currentShopItem);
        confirmPurchasePanel.SetActive(false); // Hide the confirmation panel
    }

    private void OnSlotClick(GameObject slot, ShopItem shopItem)
    {
        Debug.Log("Slot Clicked: " + slot.name);
        
        if (shopItem?.item != null)
        {
            Debug.Log("Item Name: " + shopItem.item.name);
            Debug.Log("Item Type: " + shopItem.item.type);
            Debug.Log("Is Stackable: " + shopItem.item.stackable);

            if (shopItem.item.data != null)
            {
                Item data = shopItem.item.data;
                Debug.Log("Item Data:");
                Debug.Log("  Name: " + data.Name);
                Debug.Log("  ID: " + data.Id);

                if (data.buffs != null && data.buffs.Length > 0)
                {
                    Debug.Log("  Buffs:");
                    foreach (var buff in data.buffs)
                    {
                        Debug.Log("    Attribute: " + buff.attribute);
                        Debug.Log("    Value: " + buff.value);

                        if (itemStats != null)
                            itemStats.text = $"Attribute: {buff.attribute} Value: {buff.value}";
                        else
                            Debug.LogError("itemStats is not assigned!");
                    }
                }
                else
                {
                    Debug.Log("  No Buffs");
                }
            }
            else
            {
                Debug.LogError("shopItem.item.data is null!");
            }
        }
        else
        {
            Debug.LogError("shopItem.item is null!");
        }

        if (itemDescriptionText != null && itemName != null && itemPriceText != null)
        {
            itemDescriptionText.text = shopItem.item.description;
            itemName.text = shopItem.item.name;
            itemPriceText.text = $"Price: {shopItem.price}";
        }
        else
        {
            Debug.LogError("UI text components are not assigned!");
        }

        if (shopItem.item != null && shopItem.item.uiDisplay != null)
        {
            itemImage.sprite = shopItem.item.uiDisplay;
        }
        else
        {
            Debug.LogError("shopItem.item or shopItem.item.uiDisplay is null!");
        }
    }

    public void OnClaim(ShopItem shopItem)
    {
        if (shopItem != null)
        {
            inventory.Load();
            Item _item = new Item(shopItem.item);

            if (inventory.AddItem(_item, 1))
            {

                inventory.Save();
                playerData.SubtractGold(shopItem.price);
            }
        }
    }
}