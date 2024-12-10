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
    public Image itemImage;                    // UI Image to display the item's image

    public InventoryObject inventory;

    private Dictionary<GameObject, ShopItem> slotsOnInterface;

    void Start()
    {
        slotsOnInterface = new Dictionary<GameObject, ShopItem>();

        for (int i = 0; i < shopItems.Count; i++)
        {
            var shopItem = shopItems[i];
            var slot = slots[i];

            // Set up only the click event for the slot (OnClick)
            AddEvent(slot, EventTriggerType.PointerClick, delegate { OnSlotClick(slot, shopItem); });

            // Set up claim button
            Button claimButton = Instantiate(claimButtonPrefab, slot.transform);
            claimButton.onClick.AddListener(() => OnClaimButtonClick(shopItem));

            // Optionally, set the position manually in the script (if you want a default setting):
            RectTransform claimButtonRect = claimButton.GetComponent<RectTransform>();
            claimButtonRect.anchoredPosition = new Vector2(1, -70);
            claimButtonRect.localScale = new Vector3(0.5f, 0.5f, 0.4f);

            slotsOnInterface.Add(slot, shopItem);
        }
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
        OnClaim(shopItem);
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

    if (itemDescriptionText != null && itemName != null)
    {
        itemDescriptionText.text = shopItem.item.description;
        itemName.text = shopItem.item.name;
    }
    else
    {
        Debug.LogError("UI text components are not assigned!");
    }

    Transform childImage = slot.transform.Find("Image");
    if (childImage != null)
    {
        Image slotImage = childImage.GetComponent<Image>();
        if (slotImage != null && itemImage != null)
        {
            itemImage.sprite = slotImage.sprite;
        }
        else
        {
            Debug.LogError("Slot image or itemImage is null!");
        }
    }
    else
    {
        Debug.LogError("Child object 'Image' not found under slot.");
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
            }
        }
    }
}
