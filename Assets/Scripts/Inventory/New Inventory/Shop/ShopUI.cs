using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopUI : MonoBehaviour
{
    public GameObject[] slots;              // Array of UI slots for each shop item
    public Button claimButtonPrefab;         // Prefab for the claim button
    public Transform shopPanel;              // Parent object to hold all the items
    public List<ShopItem> shopItems;         // List of all ShopItems in your shop
    public GameObject itemDetailPanel;       // The panel to show item details
    public TMP_Text itemDescriptionText;     // Text to display the item description

    public InventoryObject inventory;

    private Dictionary<GameObject, ShopItem> slotsOnInterface;

    void Start()
    {
        // Initialize the dictionary to store slot references
        slotsOnInterface = new Dictionary<GameObject, ShopItem>();

        // Populate the shop with items and set up buttons
        for (int i = 0; i < shopItems.Count; i++)
        {
            // Instantiate a slot from the slots array for each shop item
            var shopItem = shopItems[i];
            var slot = slots[i];

            // Set up only the click event for the slot (OnClick)
            AddEvent(slot, EventTriggerType.PointerClick, delegate { OnSlotClick(slot,shopItem); });

            // Set up claim button
            Button claimButton = Instantiate(claimButtonPrefab, slot.transform);
            claimButton.onClick.AddListener(() => OnClaimButtonClick(shopItem));

            // Optionally, set the position manually in the script (if you want a default setting):
            RectTransform claimButtonRect = claimButton.GetComponent<RectTransform>();
            claimButtonRect.anchoredPosition = new Vector2(1, -70);
            claimButtonRect.localScale = new Vector3(0.5f, 0.5f, 0.4f);

            // Store the slot and associated ShopItem in the dictionary
            slotsOnInterface.Add(slot, shopItem);
        }
    }

    // Add event to slot (modified for OnClick only)
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

    // Called when a claim button is clicked
    private void OnClaimButtonClick(ShopItem shopItem)
    {
        OnClaim(shopItem);
    }

    // Display item details when clicked
    public void OnItemClicked(ShopItem shopItem)
    {
         Debug.Log("Slot Clicked: " + shopItem.name);

        
        

    }

    // Called when clicking a slot (logs the slot click)
    private void OnSlotClick(GameObject slot,ShopItem shopItem)
    {   
        Debug.Log("Slot Clicked: " + slot.name);  // Logs the name of the clicked slot
        Debug.Log("Item Name: " + shopItem.item.name);  // Logs the name of the item  // Logs the ID of the item
        Debug.Log("Item Type: " + shopItem.item.type);  // Logs the type of the item
        Debug.Log("Is Stackable: " + shopItem.item.stackable);
        Debug.Log("Slot Clicked: " + slot.name);
        itemDescriptionText.text = shopItem.item.description;
    }

    // Called when claiming an item
    public void OnClaim(ShopItem shopItem)
    {
        if (shopItem != null)
        {
            // Create a new item from the ShopItem to add to the inventory
            inventory.Load();
            Item _item = new Item(shopItem.item);

            // If the item is successfully added to the inventory
            if (inventory.AddItem(_item, 1))
            {
                // Save the inventory data
                inventory.Save();
            }
        }
    }
}
