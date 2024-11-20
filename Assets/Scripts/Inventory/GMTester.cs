using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for Button

public class GMTester : MonoBehaviour
{
    public DynamicInventoryDisplay display;
    public GameObject prefab; // Reference to the prefab or GameObject to toggle
    private bool isOpen;

    public Button toggleButton; // Reference to the button to trigger the toggle

    private void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleInventory); // Add listener to button click
        }
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;

        // Toggle the inventory display
        display.onDisplay(isOpen);

        // Toggle the prefab's active state
        if (prefab != null)
        {
            prefab.SetActive(isOpen);
        }

        // Open/Close the inventory based on the state
        if (isOpen)
        {
            // Set the game state to UI_Only when opening the inventory
            GameManagers.instance.SetGameState(GameState.UI_Only);
        }
        else
        {
            // Set the game state to Game_Only when closing the inventory
            GameManagers.instance.SetGameState(GameState.Game_Only);
        }
    }
}
