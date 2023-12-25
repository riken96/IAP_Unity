# Unity IAP Manager

This Unity project includes a versatile In-App Purchase (IAP) Manager script for handling consumable, non-consumable, and subscription products. The script is designed to work with Unity IAP and provides easy integration into your Unity games.

## Features

- Supports Consumable, Non-Consumable, and Subscription products.
- Easy-to-use methods for initiating purchases for different types of products.
- Local validation option for enhanced security.
- Update UI functions for displaying product-related information.
- Integration with PlayerPrefs for storing purchase information locally.

## Getting Started

Follow these steps to integrate the IAP Manager into your Unity project:

1. Clone the repository to your local machine:

   ```bash
   git clone https://github.com/your-username/your-repository.git
   ```

2. Open the Unity project and ensure that Unity IAP is set up correctly.

3. Drag and drop the `IAPManager` script into your project's relevant folder.

4. Customize the script with your product IDs and UI elements.

5. Implement necessary UI components and call the appropriate methods to trigger purchases.

## Configuration

Configure the IAPManager script by modifying the product IDs and other relevant variables:

```csharp
// Product IDs for Consumable, Non-Consumable, and Subscription products
public string PRODUCT_CONSUMABLE1 = "com.example.game.gold1";
public string PRODUCT_CONSUMABLE2 = "com.example.game.diamond1";
public string PRODUCT_SUBSCRIPTION = "com.example.game.vip";
public string PRODUCT_NON_CONSUMABLE = "com.example.game.noads";

// UI Text components for displaying product-related information
public Text NoadsText;
public Text GoldText;
public Text DiamondText;
public Text SubscriptionText;

// PlayerPrefs keys for storing local purchase information
public int NoAds { get; set; }
public int Subscribe { get; set; }
```

## Usage

Use the provided methods to initiate purchases for different product types:

```csharp
// Example: Initiating a purchase for a consumable product (Gold)
public void BuyGold()
{
    BuyProductID(PRODUCT_CONSUMABLE1);
}

// Example: Initiating a purchase for a non-consumable product (No Ads)
public void BuyNoAds()
{
    BuyProductID(PRODUCT_NON_CONSUMABLE);
}

// Example: Initiating a purchase for a subscription product (VIP)
public void BuySubscription()
{
    BuyProductID(PRODUCT_SUBSCRIPTION);
}
```

## License

This project is licensed under the [MIT License](LICENSE).

## Acknowledgments

- Unity IAP documentation: [Unity IAP Documentation](https://docs.unity3d.com/Manual/UnityIAP.html)
- [OpenAI](https://www.openai.com/) for GPT-3.5 language model.
```

Replace the placeholder product IDs, text components, and other details with your specific information. Additionally, you may want to provide more details about the project structure, dependencies, or any special instructions for users.
