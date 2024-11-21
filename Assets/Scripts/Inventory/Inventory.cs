using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/New Inventory")]
public class Inventory : ScriptableObject
{
    // Changed the class to a Scriptable Object
    public ItemData[] itemlist;
}
