using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHolder : MonoBehaviour
{

    public Inventory inventory;
    public Transform display;

    private int returnAmount;


    #region Testing
[Header("Testing")]
public Items item;
public int stack = 32;

private void Update()
{
    // Check for left mouse button click
    // if (Input.GetMouseButtonDown(0)) // 0 = Left mouse button
    // {
    //     ItemData thisData = new ItemData();
    //     thisData.items = item;
    //     thisData.stack = stack;
    //     AddItem(thisData);
    // }

    // // Check for right mouse button click
    // if (Input.GetMouseButtonDown(1)) // 1 = Right mouse button
    // {
    //     ItemData tempItem = RemoveItem(1, false);
    //     Debug.Log(tempItem.items + " (" + tempItem.stack.ToString() + ")");
    // }
}
#endregion

    public void UpdateUISlot(int index, ItemData data)
    {
        if (display.childCount <= 0)
        {
            return;
        }

        InventorySlot slot = display.GetChild(index).GetComponent<InventorySlot>();
        slot.onUpdateSlotDisplay(data);
    }


    public ItemData RemoveItem(int index, bool isFullStack)
    {
        ItemData currentSelectedSlot = inventory.itemlist[index];
        ItemData tempData = new ItemData();
        tempData.items = currentSelectedSlot.items;
        tempData.stack = currentSelectedSlot.stack;


        switch (isFullStack)
        {
            case true:
                currentSelectedSlot = null;
                tempData = null;
                UpdateUISlot(index, currentSelectedSlot);
                break;

            case false:
                int halfStack = Mathf.RoundToInt(currentSelectedSlot.stack / 2);
                tempData.stack = halfStack;
                currentSelectedSlot.stack -= halfStack;
                UpdateUISlot(index, currentSelectedSlot);

                break;

        }
        return tempData;

    }

    public bool AddItem(ItemData itemDataToAdd)
    {
        for (int i = 0; i < inventory.itemlist.Length; i++)
        {
            ItemData currentSelectedSlot = inventory.itemlist[i];
            if (currentSelectedSlot.items == itemDataToAdd.items && currentSelectedSlot.stack != currentSelectedSlot.items.maxStackSize)
            {
                int totalCount = currentSelectedSlot.stack + itemDataToAdd.stack;
                if (totalCount <= currentSelectedSlot.items.maxStackSize)
                {
                    currentSelectedSlot.stack += itemDataToAdd.stack;
                    UpdateUISlot(i, currentSelectedSlot);
                    returnAmount = 0;
                    return true;
                }
                else
                {
                    int desiredAmountToAdd = currentSelectedSlot.items.maxStackSize - currentSelectedSlot.stack;
                    currentSelectedSlot.stack = currentSelectedSlot.items.maxStackSize;
                    UpdateUISlot(i, currentSelectedSlot);
                    returnAmount = itemDataToAdd.stack - desiredAmountToAdd;

                    if (returnAmount > 0)
                    {
                        ItemData tempData = new ItemData();
                        tempData.items = itemDataToAdd.items;
                        tempData.stack = returnAmount;
                        returnAmount = 0;
                        AddItem(tempData);
                        return true;
                    }
                    return true;
                }
            }
            else if (currentSelectedSlot.items == null)
            {
                currentSelectedSlot.items = itemDataToAdd.items;
                currentSelectedSlot.stack = itemDataToAdd.stack;
                UpdateUISlot(i, currentSelectedSlot);
                returnAmount = 0;
                return true;
            }
        }
        return false;
    }
}


