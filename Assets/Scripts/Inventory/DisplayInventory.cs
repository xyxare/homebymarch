using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{

    public InventoryObject inventory;
    public int startingXPosition;
    public int startingYPosition;
    public int horizontalIconSpacing;
    public int columnCount;
    public int verticalIconSpacing;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    public void CreateDisplay(){
        for (int i = 0; i < inventory.container.Count; i++){
            CreateIcon(i);
        }
    }

    public void UpdateDisplay(){
        for (int i = 0; i < inventory.container.Count; i++){

            if(itemsDisplayed.ContainsKey(inventory.container[i])){

                itemsDisplayed[inventory.container[i]].GetComponentInChildren<TMP_Text>().text = inventory.container[i].amount.ToString("n0");
            } else {
                CreateIcon(i);
            }
        }
    }

    public Vector3 GetPosition(int i){
        float finalXPosition = startingXPosition + (horizontalIconSpacing * (i % columnCount));
        float finalYPosition = startingYPosition - (verticalIconSpacing * (i / columnCount));
        return new Vector3 (finalXPosition, finalYPosition, 0f);
    }

    public void CreateIcon(int i){
        GameObject iconObject = Instantiate(inventory.container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
        iconObject.GetComponent<RectTransform>().localPosition = GetPosition(i);
        iconObject.GetComponentInChildren<TMP_Text>().text = inventory.container[i].amount.ToString("n0");
        itemsDisplayed.Add(inventory.container[i], iconObject);
    }
}
