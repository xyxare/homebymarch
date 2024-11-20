using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    [SerializeField] public ItemSO testItem; //for testing purposes
    [SerializeField] public ItemSO testItem2; //for testing purposes
    [SerializeField] public ItemSO testItem3; //for testing purposes

    public InventoryObject inventory;

    void Start(){
        inventory.AddItem(testItem, 6);
        inventory.AddItem(testItem2, 12);
        inventory.AddItem(testItem3, 34);
    }

    private void OnApplicationQuit(){

        inventory.container.Clear();

    }

}