using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPValidationExampleUsage : MonoBehaviour
{
    private PurchaseValidator purchaseValidator;

    private void Start()
    {
        purchaseValidator = GetComponent<PurchaseValidator>();
        StartCoroutine(ValidatePurchase());
    }

    private IEnumerator ValidatePurchase()
    {
        string receipt = "YOUR_RECEIPT_DATA"; // Replace with your actual receipt data

        purchaseValidator.ValidatePurchase(PurchaseValidator.PlatformType.Android, receipt, OnPurchaseValidated);
        yield return null; // Yielding here to allow asynchronous operation
    }

    private void OnPurchaseValidated(bool isValid, string error)
    {
        if (isValid)
        {
            Debug.Log("Purchase validated successfully");
            // Handle valid purchase
        }
        else
        {
            Debug.LogError("Purchase validation failed: " + error);
            // Handle invalid purchase
        }
    }
}
