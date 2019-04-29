//#define USE_IAP
using System;
using UnityEngine;
#if USE_IAP
using UnityEngine.Purchasing;
#endif

namespace BToolkit
{
#if USE_IAP
    public class IAPUnity : IStoreListener
    {

        private static IAP instance;
        public static IAP Instance { get { return instance ?? (instance = new IAP()); } }
        IStoreController m_StoreController;
        IExtensionProvider m_StoreExtensionProvider;
        bool IsInitialized { get { return m_StoreController != null && m_StoreExtensionProvider != null; } }
        Action<bool> InitCallbackEvent;
        Action<string> PurchaseCallbackEvent;
        public const string product_activateAPP = "com.fancyar.mhppkmr.activateapp";
        private IAP() { }

        public void Init(Action<bool> InitCallback)
        {
            InitCallbackEvent = InitCallback;
            if (!IsInitialized)
            {
                Debug.Log("IAP >>>>>>>> 初始化");
                /*
                ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                Debug.Log ("Add Product:" + product_activateAPP);
                builder.AddProduct(product_activateAPP, ProductType.NonConsumable);
                UnityPurchasing.Initialize(this, builder);
                */
            }
            else
            {
                Debug.Log("IAP >>>>>>>> 已初始化过了");
                if (InitCallbackEvent != null)
                {
                    InitCallbackEvent(true);
                }
            }
        }

        public void Buy(string productId, Action<string> PurchaseCallback)
        {
            Debug.Log("IAP >>>>>>>> Buy: " + productId);
            PurchaseCallbackEvent = PurchaseCallback;
            if (IsInitialized)
            {
                Product product = m_StoreController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    m_StoreController.InitiatePurchase(product);
                    return;
                }
                else
                {
                    Debug.LogError("IAP >>>>>>>> 商品无法购买");
                }
            }
            else
            {
                Debug.LogError("IAP >>>>>>>> IAP未初始化");
            }
            if (PurchaseCallbackEvent != null)
            {
                PurchaseCallbackEvent(null);
            }
        }

        public void Restore(Action<string> RestoreCallback)
        {
            Debug.Log("IAP >>>>>>>> Restore");
            PurchaseCallbackEvent = RestoreCallback;
            if (IsInitialized)
            {
                /*
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                Debug.Log("IAP >>>>>>>>  RestoreTransactions");
                apple.RestoreTransactions((success) => {
                    //返回一个bool值，如果成功，则会多次调用支付回调，然后根据支付回调中的参数得到商品id，最后做处理(ProcessPurchase)  
                    Debug.Log("IAP >>>>>>>>  success:"+success);
                    if (!success) {
                        if (PurchaseCallbackEvent != null) {
                            PurchaseCallbackEvent(null);
                        }
                    }
                });
                */
            }
            else
            {
                Debug.LogError("IAP >>>>>>>> 未初始化");
                if (PurchaseCallbackEvent != null)
                {
                    PurchaseCallbackEvent(null);
                }
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("IAP >>>>>>>> 初始化成功");
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
            if (InitCallbackEvent != null)
            {
                InitCallbackEvent(true);
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError("IAP >>>>>>>> 初始化失败:" + error);
            if (InitCallbackEvent != null)
            {
                InitCallbackEvent(false);
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log("IAP >>>>>>>> 支付成功:" + e.purchasedProduct.definition.id);
            if (PurchaseCallbackEvent != null)
            {
                PurchaseCallbackEvent(e.purchasedProduct.definition.id);
            }
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason p)
        {
            Debug.LogError("IAP >>>>>>>> 支付失败 StoreSpecificId:" + product.definition.storeSpecificId + " PurchaseFailureReason:" + p);
            if (PurchaseCallbackEvent != null)
            {
                PurchaseCallbackEvent(null);
            }
        }

    }
#endif
}
