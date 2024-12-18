using UnityEngine;

public class OneTimePanelAccess : MonoBehaviour
{
    public GameObject panel; // Reference to the panel you want to show
    public string itemClaimedKey = "ItemClaimed"; // Key for PlayerPrefs

    void Start()
    {
        // Check if the item has been claimed before
        if (PlayerPrefs.GetInt(itemClaimedKey, 0) == 0)
        {
            // Show the panel if the item hasn't been claimed
            panel.SetActive(true);
        }
        else
        {
            // Hide the panel if the item has already been claimed
            panel.SetActive(false);
        }
    }

    // Call this method when the item is claimed
    public void ClaimItem()
    {
        // Mark the item as claimed in PlayerPrefs
        PlayerPrefs.SetInt(itemClaimedKey, 1);
        PlayerPrefs.Save();

        // Close the panel
        panel.SetActive(false);
    }
}
