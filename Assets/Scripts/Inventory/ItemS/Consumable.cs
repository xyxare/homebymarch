using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Items/Consumable")]
public class Consumable : ItemSO
{

    //TODO: status effect
   public void Awake(){
    itemType = ItemType.Consumable;
   }
}
