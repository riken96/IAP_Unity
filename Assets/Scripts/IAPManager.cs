using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
using UnityEngine.UI;

namespace SuperStar.IAP
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        IStoreController m_StoreController;
        IExtensionProvider m_StoreExtensionProvider;
        public bool isLocalValidation = false;
        [Header("CONSUMABLE")]
        //Your products IDs. They should match the ids of your products in your store.
        //public string PRODUCT_CONSUMABLE1 = "com.superstar.riken.gold1";
        //public string PRODUCT_CONSUMABLE2 = "com.superstar.riken.diamond1";

        //[Header("SUBSCRIPTION")]
        //public string PRODUCT_SUBSCRIPTION = "com.superstar.riken.vip";

        //[Header("NON CONSUMABLE")]
        //public string PRODUCT_NON_CONSUMABLE = "com.superstar.riken.noads";
        public string PRODUCT_CONSUMABLE1 = "com.superstar.riken.gold1";
        public string PRODUCT_CONSUMABLE2 = "com.superstar.riken.diamond1";

        [Header("SUBSCRIPTION")]
        public string PRODUCT_SUBSCRIPTION = "com.randomlogicgames.solitaire.499month";

        [Header("NON CONSUMABLE")]
        public string PRODUCT_NON_CONSUMABLE = "randomlogicgames.solitaireinfinite.removeads";

        public Text NoadsText;
        public Text GoldText;
        public Text DiamondText;
        public Text SubscriptionText;
        public int NoAds
        {
            get
            {
                return PlayerPrefs.GetInt("NoAds", 0);
            }
            set
            {
                PlayerPrefs.SetInt("NoAds", value);
            }
        }
        public int Subscribe
        {
            get
            {
                return PlayerPrefs.GetInt("Subscribe", 0);
            }
            set
            {
                PlayerPrefs.SetInt("Subscribe", value);
            }
        }

        int m_GoldCount;
        int m_DiamondCount;

        void Start()
        {
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
            //UpdateUIConsumable();
            //UpdateUISubscription();
            //UpdateUIRemoveAds();
        }

        void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //Add products that will be purchasable and indicate its type.
            builder.AddProduct(PRODUCT_SUBSCRIPTION, ProductType.Subscription);
            builder.AddProduct(PRODUCT_NON_CONSUMABLE, ProductType.NonConsumable);
            builder.AddProduct(PRODUCT_CONSUMABLE1, ProductType.Consumable);
            builder.AddProduct(PRODUCT_CONSUMABLE2, ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }
        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        void BuyProductID(string productId)
        {
            Debug.LogError("BuyProductID => " + productId);
            if (IsInitialized())
            {
                m_StoreController.InitiatePurchase(productId);
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        public void BuyGold()
        {
            BuyProductID(PRODUCT_CONSUMABLE1);
        }

        public void BuyDiamond()
        {
            BuyProductID(PRODUCT_CONSUMABLE2);
        }

        public void BuyNoAds()
        {
            BuyProductID(PRODUCT_NON_CONSUMABLE);
        }

        public void BuySubscription()
        {
            BuyProductID(PRODUCT_SUBSCRIPTION);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
            UpdateUIConsumable();
             UpdateUIRemoveAds();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
            UpdateUIRemoveAds();
            UpdateUISubscription();
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            if (isLocalValidation)
            {
                var isPurchaseValid = IsPurchaseValid(product);
                if (isPurchaseValid)
                {
                    //Add the purchased product to the players inventory
                    if (product.definition.id == PRODUCT_CONSUMABLE1)
                    {
                        AddGold();
                    }
                    else if (product.definition.id == PRODUCT_CONSUMABLE2)
                    {
                        AddDiamond();
                    }
                    else if (product.definition.id == PRODUCT_NON_CONSUMABLE)
                    {
                        NoAds = 1;
                        //ApplovinAdmanager.Instance.hideBanner();
                        Debug.LogError("No Ads Purchased");
                        Debug.LogError("Ad Hide");
                        NoadsText.text = "NO ADS PURCHASED";
                        UpdateUIRemoveAds();
                    }
                    else if (product.definition.id == PRODUCT_SUBSCRIPTION)
                    {
                        UpdateUISubscription();
                        SubscriptionText.text = "SUBSCRIPTION PURCHASED";
                    }


                    // UpdateUISubscription();

                    Debug.Log("Valid receipt, unlocking content.");
                }
                else
                {
                    Debug.Log("Invalid receipt, not unlocking content.");
                }

            }
            else
            {

                if (product.definition.id == PRODUCT_CONSUMABLE1)
                {
                    AddGold();
                }
                else if (product.definition.id == PRODUCT_CONSUMABLE2)
                {
                    AddDiamond();
                }
                else if (product.definition.id == PRODUCT_NON_CONSUMABLE)
                {
                    NoAds = 1;
                    //ApplovinAdmanager.Instance.hideBanner();
                    Debug.LogError("No Ads Purchased");
                    Debug.LogError("Ad Hide");
                    NoadsText.text = "ADS PURCHASED";
                    UpdateUIRemoveAds();
                }
                else if (product.definition.id == PRODUCT_SUBSCRIPTION)
                {
                    UpdateUISubscription();
                    SubscriptionText.text = "SUBSCRIPTION PURCHASED";
                }
                //subscription Data
               // UpdateUISubscription();
            }
            Debug.Log($"Purchase Complete - Product: {product.definition.id}");
            //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
                $" Purchase failure reason: {failureDescription.reason}," +
                $" Purchase failure details: {failureDescription.message}");
        }
        public void UpdateUIRemoveAds()
        {
            Debug.LogError("UpdateUIRemoveAds");
            var subscriptionProduct = m_StoreController.products.WithID(PRODUCT_NON_CONSUMABLE);
            try
            {
                var isSubscribed = IsPurchasedTo(subscriptionProduct);
                isSubscribed = isSubscribed ? true : false;
                if (isSubscribed)
                {
                    NoAds = 1;
                    //ApplovinAdmanager.Instance.hideBanner();
                    Debug.LogError("No Ads Purchased");
                    Debug.LogError("Ad Hide");
                    NoadsText.text = "NO ADS PURCHASED";
                }
                else
                {

                    Debug.LogError("No Ads Not Purchased");
                    NoadsText.text = "NOT ADS PURCHASED";
                }
            }
            catch (StoreSubscriptionInfoNotSupportedException)
            {
                Debug.LogError("Some exception occureed");
                // NoAds
                //isSubscribedText.text =
                //    "Couldn't retrieve subscription information because your current store is not supported.\n" +
                //    $"Your store: \"{store}\"\n\n" +
                //    "You must use the App Store, Google Play Store or Amazon Store to be able to retrieve subscription information.\n\n" +
                //    "For more information, see README.md";
            }

        }
        bool IsPurchasedTo(Product subscription)
        {
            // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
            if (subscription.receipt == null)
            {
                return false;
            }
            Debug.LogError("Receipt not null");


            return true;
        }
        void AddGold()
        {
            m_GoldCount++;
            UpdateUIConsumable();
            GoldText.text ="Gold "+ m_GoldCount.ToString();
        }

        void AddDiamond()
        {
            m_DiamondCount++;
            UpdateUIConsumable();
            DiamondText.text ="Diamond "+ m_DiamondCount.ToString();
        }

        void UpdateUIConsumable()
        {
           
                Debug.Log("Your Gold:" + m_GoldCount);
            Debug.Log("Your Diamonds:" + m_DiamondCount);
            //consumable UI
        }

        void UpdateUISubscription()
        {
            var subscriptionProduct = m_StoreController.products.WithID(PRODUCT_SUBSCRIPTION);
            try
            {
                var isSubscribed = IsSubscribedTo(subscriptionProduct);
                isSubscribed = isSubscribed ? true : false;

                if (isSubscribed)
                {
                    print("Subscribed");
                    Subscribe = 1;
                    SubscriptionText.text = "SUBSCRIPTION PURCHASED";
                    Debug.Log("Active Acubscription");
                }
                else
                {
                    print("Un subscribed");
                    Subscribe = 0;
                    Debug.Log("DeActive Acubscription");
                    SubscriptionText.text = " NOT SUBSCRIPTION PURCHASED";

                }
            }
            catch (StoreSubscriptionInfoNotSupportedException)
            {
                var receipt = (Dictionary<string, object>)MiniJson.JsonDecode(subscriptionProduct.receipt);
                var store = receipt["Store"];
                Debug.Log("DeActive Acubscription");
                Debug.LogError("Exception occure");
            }

        }

        bool IsSubscribedTo(Product subscription)
        {
            // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
            if (subscription.receipt == null)
            {
                return false;
            }

            //The intro_json parameter is optional and is only used for the App Store to get introductory information.
            var subscriptionManager = new SubscriptionManager(subscription, null);

            // The SubscriptionInfo contains all of the information about the subscription.
            // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
            var info = subscriptionManager.getSubscriptionInfo();

            return info.isSubscribed() == Result.True;
        }

        #region LOCALVALIDATION
        CrossPlatformValidator m_Validator = null;


//        void InitializeValidator()
//        {
//            if (IsCurrentStoreSupportedByValidator())
//            {
//#if !UNITY_EDITOR
//                var appleTangleData = m_UseAppleStoreKitTestCertificate ? AppleStoreKitTestTangle.Data() : AppleTangle.Data();
//                m_Validator = new CrossPlatformValidator(GooglePlayTangle.Data(), appleTangleData, Application.identifier);
//#endif
//            }
//            else
//            {
//                WarnInvalidStore(StandardPurchasingModule.Instance().appStore);
//            }
//        }

        bool IsPurchaseValid(Product product)
        {
            //If we the validator doesn't support the current store, we assume the purchase is valid
            if (IsCurrentStoreSupportedByValidator())
            {
                try
                {
                    var result = m_Validator.Validate(product.receipt);

                    //The validator returns parsed receipts.
                    LogReceipts(result);
                }

                //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
                catch (IAPSecurityException reason)
                {
                    Debug.Log($"Invalid receipt: {reason}");
                    return false;
                }
            }

            return true;
        }

        static bool IsCurrentStoreSupportedByValidator()
        {
            //The CrossPlatform validator only supports the GooglePlayStore and Apple's App Stores.
            return IsGooglePlayStoreSelected() || IsAppleAppStoreSelected();
        }

        static bool IsGooglePlayStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.GooglePlay;
        }

        static bool IsAppleAppStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.AppleAppStore ||
                currentAppStore == AppStore.MacAppStore;
        }

        public void WarnInvalidStore(AppStore currentAppStore)
        {
            var warningMsg = $"The cross-platform validator is not implemented for the currently selected store: {currentAppStore}. \n" +
                             "Build the project for Android, iOS, macOS, or tvOS and use the Google Play Store or Apple App Store. See README for more information.";
            Debug.LogWarning(warningMsg);

        }

        static void LogReceipts(IEnumerable<IPurchaseReceipt> receipts)
        {
            Debug.Log("Receipt is valid. Contents:");
            foreach (var receipt in receipts)
            {
                LogReceipt(receipt);
            }
        }

        static void LogReceipt(IPurchaseReceipt receipt)
        {
            Debug.Log($"Product ID: {receipt.productID}\n" +
                $"Purchase Date: {receipt.purchaseDate}\n" +
                $"Transaction ID: {receipt.transactionID}");

            if (receipt is GooglePlayReceipt googleReceipt)
            {
                Debug.Log($"Purchase State: {googleReceipt.purchaseState}\n" +
                    $"Purchase Token: {googleReceipt.purchaseToken}");
            }

            if (receipt is AppleInAppPurchaseReceipt appleReceipt)
            {
                Debug.Log($"Original Transaction ID: {appleReceipt.originalTransactionIdentifier}\n" +
                    $"Subscription Expiration Date: {appleReceipt.subscriptionExpirationDate}\n" +
                    $"Cancellation Date: {appleReceipt.cancellationDate}\n" +
                    $"Quantity: {appleReceipt.quantity}");
            }
        }

        #endregion
    }
}