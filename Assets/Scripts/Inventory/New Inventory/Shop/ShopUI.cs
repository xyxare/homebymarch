using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopUI : MonoBehaviour
{
    public GameObject[] helmetSlots;
    public GameObject[] chestSlots;
    public GameObject[] weaponSlots;
    public GameObject[] shieldSlots;
    public GameObject[] bootsSlots;

    public Button claimButtonPrefab;
    public Transform shopPanel;
    public List<ShopItem> helmetItems;
    public List<ShopItem> chestItems;
    public List<ShopItem> weaponItems;
    public List<ShopItem> shieldItems;
    public List<ShopItem> bootsItems;

    public GameObject itemDetailPanel;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemSkillDescriptionText;
    public TMP_Text itemName;
    public TMP_Text itemSkillName;
    public TMP_Text itemStats;
    public TMP_Text itemPriceText;
    public Image itemImage;
    public Image itemSkillImage;
    public GameObject skillInfo;
    public InventoryObject inventory;
    public PlayerData playerData;

    public GameObject confirmPurchasePanel;
    public Button confirmButton;
    public GameObject insufficientGoldPanel;
    private ShopItem currentShopItem;

    private Dictionary<GameObject, ShopItem> slotsOnInterface;

    public GameObject helmetPanel;
    public GameObject chestPanel;
    public GameObject weaponPanel;
    public GameObject shieldPanel;
    public GameObject bootsPanel;

    // Function to switch between panels
    public void SwitchPanel(string panelName)
    {
        helmetPanel.SetActive(false);
        chestPanel.SetActive(false);
        weaponPanel.SetActive(false);
        shieldPanel.SetActive(false);
        bootsPanel.SetActive(false);

        switch (panelName)
        {
            case "Helmet":
                helmetPanel.SetActive(true);
                InitializeSlots(helmetSlots, helmetItems);
                break;
            case "Chest":
                chestPanel.SetActive(true);
                InitializeSlots(chestSlots, chestItems);
                break;
            case "Weapon":
                weaponPanel.SetActive(true);
                InitializeSlots(weaponSlots, weaponItems);
                break;
            case "Shield":
                shieldPanel.SetActive(true);
                InitializeSlots(shieldSlots, shieldItems);
                break;
            case "Boots":
                bootsPanel.SetActive(true);
                InitializeSlots(bootsSlots, bootsItems);
                break;
        }
    }

    // Button click event handlers
    public void OnHelmetButtonClick()
    {
        SwitchPanel("Helmet");
    }

    public void OnChestButtonClick()
    {
        SwitchPanel("Chest");
    }

    public void OnWeaponButtonClick()
    {
        SwitchPanel("Weapon");
    }

    public void OnShieldButtonClick()
    {
        SwitchPanel("Shield");
    }

    public void OnBootsButtonClick()
    {
        SwitchPanel("Boots");
    }

    void Start()
    {
        slotsOnInterface = new Dictionary<GameObject, ShopItem>();

        // Initialize the default panel (e.g., Helmet)
        SwitchPanel("Helmet");

        // Hide the confirm purchase panel and insufficient gold panel initially
        confirmPurchasePanel.SetActive(false);
        insufficientGoldPanel.SetActive(false);

        // Add listener to the confirm button
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    private void OnEnable()
    {
        if (helmetItems.Count > 0 && helmetSlots.Length > 0)
        {
            OnSlotClick(helmetSlots[0], helmetItems[0]);
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

            // Check if skillData is of type SpellStrategy and display its description
            // Check if skillData is null
            if (shopItem.item.skillData == null)
            {
                skillInfo.SetActive(false);
            }
            else if (shopItem.item.skillData is SpellStrategy spell)
            {
                skillInfo.SetActive(true);
                itemSkillDescriptionText.text = $"{spell.description}";
                itemSkillImage.sprite = spell.uiDisplay;
                itemSkillName.text = spell.spellName;
            }
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
            itemImage.sprite = itemImage.sprite;
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

    private void InitializeSlots(GameObject[] slots, List<ShopItem> items)
    {
        slotsOnInterface.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            var shopItem = items[i];
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
                    slotImage.sprite = slotImage.sprite;
                }
            }
            else
            {
                Debug.LogError("Child object 'Image' not found under slot.");
            }

            slotsOnInterface.Add(slot, shopItem);
        }
    }
}