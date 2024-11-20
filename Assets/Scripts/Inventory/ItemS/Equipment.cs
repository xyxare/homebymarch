using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Items/Equipment")]
public class Equipment : ItemSO
{
    public EquipmentType equipmentType;

    void Awake(){
        itemType = ItemType.Equipment;
    }

}

public enum EquipmentType{
    Weapon,
    Armor

}