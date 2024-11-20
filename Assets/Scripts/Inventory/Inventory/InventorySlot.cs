using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySlot
{
    public ItemSO item;
    public int amount;
    public InventorySlot(ItemSO item, int amount){
        this.item = item;
        this.amount = amount;
    }

    public void AddAmount(int value){
        this.amount += value;
    }
}
