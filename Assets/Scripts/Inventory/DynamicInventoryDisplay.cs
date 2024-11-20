using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicInventoryDisplay : MonoBehaviour
{

    public InventoryHolder inventoryHolder;
    public InventorySlot slotPrefab;
    public void onDisplay(bool display){
        switch (display){
            case true:
                for(int i = 0; i < inventoryHolder.inventory.itemlist.Length; i++)
                {
                    InventorySlot slot = Instantiate(slotPrefab, transform) as InventorySlot;
                    slot.onUpdateSlotDisplay(inventoryHolder.inventory.itemlist[i]);
                }
                break;
            case false:
                foreach (Transform child in transform){
                    Destroy(child.gameObject);
                }
                break;
                
        }
    }
}
