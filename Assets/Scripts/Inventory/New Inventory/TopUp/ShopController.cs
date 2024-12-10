using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText; // Reference to the coin text UI
    // [SerializeField] private GameObject adsBuyButton; // Reference to the button for ads removal

    // Method to add 50 coins
    public void Coin50Button()
    {
        AddCoin(50);
    }

    // Method to add 100 coins
    public void Coin100Button()
    {
        AddCoin(100);
    }

    // Method to add 150 coins
    public void Coin150Button()
    {
        AddCoin(500);
    }

    // Method to add 500 coins
    public void Coin500Button()
    {
        AddCoin(500);
    }

    // Method to add 1000 coins
    public void Coin1000Button()
    {
        AddCoin(1000);
    }

    // Method to handle the "Remove Ads" button functionality
    // public void RemoveAdsButton()
    // {
    //     if (adsBuyButton != null)
    //     {
    //         adsBuyButton.GetComponent<Button>().interactable = false; // Disable the button
    //         adsBuyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Active"; // Update the button text
    //     }
    //     else
    //     {
    //         Debug.LogWarning("AdsBuyButton reference is missing!");
    //     }
    // }

    // Private helper method to add coins
    private void AddCoin(int coinAmount)
    {
        int currentValue;
        if (int.TryParse(coinText.text, out currentValue))
        {
            currentValue += coinAmount; // Add the coin amount to the current value
            coinText.text = currentValue.ToString(); // Update the coin text
        }
        else
        {
            Debug.LogError("Failed to parse coin text as an integer.");
        }
    }
}
