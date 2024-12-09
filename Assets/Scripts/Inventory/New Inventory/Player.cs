using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;

    public Attribute[] attributes;

    // Buttons for saving and loading
    public Button saveButton;
    public Button loadButton;

    private void Start()
    {
        // Initialize attributes
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }

        // Subscribe to equipment slot update events
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnBeforeUpdate;
            equipment.GetSlots[i].OnAfterUpdate += OnAfterUpdate;
        }

        // Add listeners to buttons
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveGame);

        if (loadButton != null)
            loadButton.onClick.AddListener(LoadGame);
    }

    public void OnBeforeUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null) return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Removed", _slot.ItemObject, "on", _slot.parent.inventory.type, ", Allowed items:,", string.Join(", ", _slot.AllowedItems)));
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                    }
                }
                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    public void OnAfterUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null) return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Placed", _slot.ItemObject, "on", _slot.parent.inventory.type, ", Allowed items:,", string.Join(", ", _slot.AllowedItems)));
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                    }
                }
                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        var groundItem = other.GetComponent<GroundItem>();
        if (groundItem)
        {
            Item _item = new Item(groundItem.item);
            if (inventory.AddItem(_item, 1))
            {
                Destroy(other.gameObject);
            }
        }
    }

    public void SaveGame()
    {
        inventory.Save();
        equipment.Save();
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        inventory.Load();
        equipment.Load();
        Debug.Log("Game Loaded!");
    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public Player parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(Player _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
