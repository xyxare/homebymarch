using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour
{
    private string coins50 = "coins50";
    private string coins100 = "coins100";
    private string coins150 = "coins150";
    private string coins1000 = "coins1000";

    // Reference to the ShopController
    public ShopController _shopController;

    // Called when a purchase is successfully completed
    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == coins50)
        {
            _shopController.Coin50Button();
        }
        else if (product.definition.id == coins100)
        {
            _shopController.Coin100Button();
        }
        else if (product.definition.id == coins150)
        {
            _shopController.Coin150Button();
        }
        else if (product.definition.id == coins1000)
        {
            _shopController.Coin1000Button();
        }
        else
        {
            Debug.LogWarning($"Unrecognized product ID: {product.definition.id}");
        }
    }

    // Called when a purchase fails
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase failed for product: {product.definition.id}. Reason: {failureDescription.reason}");
    }
}
