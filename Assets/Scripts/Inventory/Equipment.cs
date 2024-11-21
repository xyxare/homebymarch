using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/New Equipment")]
public class Equipment : Items
{
    public EquipmentType _type;

    
}
[System.Serializable]
public enum EquipmentType{
    Head, Body, Pants, Feet
}