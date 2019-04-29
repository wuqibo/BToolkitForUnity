using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class IAP : MonoBehaviour
    {
        public enum Result {
            PurchaseRestore = -1,//支付还原
            PurchaseSucceed = 0,//支付成功
            CanNotMakePayment = 1,//APP内支付开关未打开
            RequestProductFailed = 2,//查询商品失败
            PurchaseFailed = 3//支付失败
        }

        private static IAP instance;
        public static IAP Instance {
            get {
                if(!instance){
                    instance = FindObjectOfType<IAP>();
                    if(!instance){
                        GameObject go = new GameObject("IAP");
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<IAP>();
                    }
                }
                return instance; 
            } 
        }
        Action InitedEvent;
        Action<Result> PurchaseEvent;
        Action<bool> RestoreEvent;
        List<string> canRestoreProductIds = new List<string>();
        string currRestoreProductId;
        bool hadInit;

        void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// 初始化（完成后回调，然后可以用方法“CanRestore()”设置哪些按钮需要把文字变成恢复购买）
        /// </summary>
        public void Init(Action OnInitedCallback)
        {
            if (!hadInit)
            {
                Debug.Log("IAP >>>>>>>> Init");
                InitedEvent = OnInitedCallback;
#if UNITY_IOS && !UNITY_EDITOR
                Init(gameObject.name, "OnGetedRestoreProductIds");
#endif
                hadInit = true;
            }
        }

        /// <summary>
        /// 执行购买
        /// </summary>
        public void Purchase(string productId, Action<Result> OnPurchaseCallback)
        {
            if (!hadInit)
            {
                Init(null);
            }
            Debug.Log("IAP >>>>>>>> Buy: " + productId);
            PurchaseEvent = OnPurchaseCallback;
#if UNITY_IOS && !UNITY_EDITOR
            Purchase(productId, gameObject.name, "OnPurchaseCallback");
#endif
        }

        /// <summary>
        /// 已买过，恢复购买
        /// </summary>
        public void Restore(string productId, Action<bool> OnRestoreCallback)
        {
            if (!hadInit)
            {
                Init(null);
            }
            Debug.Log("IAP >>>>>>>> Restore");
            currRestoreProductId = productId;
            RestoreEvent = OnRestoreCallback;
#if UNITY_IOS && !UNITY_EDITOR
            Restore();
#endif
        }

        /// <summary>
        /// 判断ProductID是否可以恢复购买，初始化Init()完成后可用
        /// </summary>
        public bool CanRestore(string productId)
        {
            return canRestoreProductIds.Contains(productId);
        }

        void OnGetedRestoreProductIds(string result){
        	Debug.Log("IAP >>>>>>>> OnGetedRestoreProductIds:" + result);
        	string[] resultArr = result.Split('|');
        	if(resultArr.Length>1){
                canRestoreProductIds.Clear();
                for(int i=1;i<resultArr.Length;i++){
                    canRestoreProductIds.Add(resultArr[i]);
                }
        	}
            if (InitedEvent != null)
            {
                InitedEvent();
                InitedEvent = null;
            }
            if(currRestoreProductId !=null && RestoreEvent!=null){
                RestoreEvent(canRestoreProductIds.Contains(currRestoreProductId));
            }
            currRestoreProductId = null;
            RestoreEvent = null;
        }

        void OnPurchaseCallback(string result)
        {
            Debug.Log("IAP >>>>>>>> OnPurchaseCallback:" + result);
            if(PurchaseEvent!=null){
                PurchaseEvent((Result)int.Parse(result));
            }
        }

        [DllImport("__Internal")]
        static extern void Init(string callbackGo, string callbackFun);
        [DllImport("__Internal")]
        static extern void Purchase(string productId, string callbackGo, string callbackFun);
        [DllImport("__Internal")]
        static extern void Restore();
    }
}
