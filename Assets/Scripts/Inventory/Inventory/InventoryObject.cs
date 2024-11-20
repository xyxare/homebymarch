using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> container = new List<InventorySlot>();

    public void AddItem(ItemSO item, int amount){

        

        for (int i = 0; i < container.Count; i++){

            if(container[i].item == item){
                container[i].AddAmount(amount);
                
                return;
            }
            
        }


        container.Add(new InventorySlot(item, amount));



    }
    
}
