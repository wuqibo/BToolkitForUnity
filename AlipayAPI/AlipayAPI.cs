using LitJson;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace BToolkit
{
    public class AlipayAPI : SingletonMonoBehaviour<AlipayAPI>
    {

        public delegate void PaySuccessDelegate(int totalFee, string productTitle, int buyCount, string orderId, string recvName, string recvPhone, string recvAdd);
        PaySuccessDelegate PaySuccessEvent;
        Action OrderInvalidAction;
        string currAppId, currOrderId;

        public void Init(string appId)
        {
            this.currAppId = appId;
        }

        //支付-发送订单(WWW和RSA加密不支持太长的参数值，拆分)
        struct OrderStruct
        {
            public string orderId, account, productId, spbillCreateIp, isIos;
            public int buyCount;
        }
        struct OrderStruct2
        {
            public string receiverName, receiverPhone, receiverAddress;
        }
        /// <summary>
        /// orderId:由APP自定义生成(建议由时间戳加随机数不加密)，支付失败时必须使用同一个，否则会二次付款,订单结束后发起新订单才可以生成新orderId
        /// <parm>fee:支付金额，单位：分</parm>
        /// </summary>
        public void Pay(string account, string productId, int buyCount, string productTitle, string receiverName, string receiverPhone, string receiverAddress, string remark, PaySuccessDelegate OnPaySuccessCallback, Action OnOrderInvalid)
        {
            //不用判断安装客户端，如果没装支付宝会自动跳到H5网页收银台执行支付
            this.PaySuccessEvent = OnPaySuccessCallback;
            this.OrderInvalidAction = OnOrderInvalid;
            //订单数据
            OrderStruct data = new OrderStruct();
            data.orderId = currOrderId;
            data.account = account;
            data.productId = productId;
            data.buyCount = buyCount;
            data.spbillCreateIp = Http.GetExternalIP();
            data.isIos = (Application.platform == RuntimePlatform.IPhonePlayer) ? "true" : "false";
            string dataStr = JsonMapper.ToJson(data);
            string dataCrypto = RSAUtils.EncryptWithPublicPemKey(dataStr);
            //产品标题
            string productTitleCrypto = RSAUtils.EncryptWithPublicPemKey(ReplaceStr(productTitle));
            //收货人信息
            OrderStruct2 recv = new OrderStruct2();
            recv.receiverName = ReplaceStr(receiverName);
            recv.receiverPhone = ReplaceStr(receiverPhone);
            recv.receiverAddress = ReplaceStr(receiverAddress);
            string recvStr = JsonMapper.ToJson(recv);
            string recvCrypto = RSAUtils.EncryptWithPublicPemKey(recvStr);
            //收货人备注
            string remarkCrypto = "null";
            if (!string.IsNullOrEmpty(remark))
            {
                remarkCrypto = RSAUtils.EncryptWithPublicPemKey(ReplaceStr(remark));
            }
            /*
            Dictionary<string, string> formData = new Dictionary<string, string>();
            formData.Add("executekey", AccountManager.ExecuteKey);
            formData.Add("data", dataCrypto);
            formData.Add("productTitle", productTitleCrypto);
            formData.Add("receiver", recvCrypto);
            formData.Add("remark", remarkCrypto);
            Wait.Show(true);
            Http.Post(AccountManager.ServerURL + "apppayserver/alipay/requestOrder.php", formData, RequestOrderHttpCallback);
            */
        }

        string ReplaceStr(string inputStr)
        {
            if (inputStr == null)
            {
                return null;
            }
            return inputStr.Replace("@", "").Replace("#", "").Replace("$", "").Replace("&", "").Replace("*", "");
        }

        private void RequestOrderHttpCallback(Http.ResultType resultType, string result)
        {
            if (resultType == Http.ResultType.Success)
            {
                Debug.Log("RequestOrderHttpCallback:" + result);
                //这里最好验证签名
                ////////////////////////
                bool getOrderIdSuccess = false;
                bool appIdValid = false;
                string[] keyValues = result.Split('&');
                for (int i = 0; i < keyValues.Length; i++)
                {
                    string _keyValues = keyValues[i];
                    if (_keyValues.Contains("="))
                    {
                        string[] keyValue = _keyValues.Split('=');
                        if ("app_id".Equals(keyValue[0]))
                        {
                            if (currAppId.Equals(keyValue[1]))
                            {
                                appIdValid = true;
                            }
                        }
                        else if ("biz_content".Equals(keyValue[0]))
                        {
                            JsonData bizContentJson = JsonMapper.ToObject(UrlCodeUtils.UrlDecode(keyValue[1]));
                            currOrderId = (string)bizContentJson["out_trade_no"];
                            getOrderIdSuccess = true;
                        }
                    }
                }
                if (appIdValid && getOrderIdSuccess)
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        AndroidUtils.CallAndroidStaticFunction("cn.wuqibo.tools.alipayopen.AliPay", "pay", result, gameObject.name, "AliPayCallback");
                    }
                    else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        _AliPay(result, gameObject.name, "AliPayCallback");
                    }
                    return;
                }
            }
            Wait.Show(false);
            DialogAlert.Show("提示", "该订单已失效!", OnCloseAlert);
        }
        void AliPayCallback(string msg)
        {
            Debug.Log("AliPayCallback:" + msg);
            JsonData json = JsonMapper.ToObject(msg);
            string resultStatus = (string)json["resultStatus"];
            if ("9000".Equals(resultStatus))
            {
                CheckPayment();
                return;
            }
            DelOrderOnServer(currOrderId);
            Wait.Show(false);
            string errorMsg = (string)json["memo"];
            if (string.IsNullOrEmpty(errorMsg))
            {
                if ("6001".Equals(resultStatus))
                {
                    errorMsg = "支付已取消";
                }
            }
            DialogAlert.Show("[" + resultStatus + "]", errorMsg, OnCloseAlert);
        }
        void OnCloseAlert()
        {
            if (OrderInvalidAction != null)
            {
                OrderInvalidAction();
            }
        }

        void DelOrderOnServer(string orderId)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                /*
                Dictionary<string, string> formData = new Dictionary<string, string>();
                formData.Add("executekey", AccountManager.ExecuteKey);
                formData.Add("data", RSATool.EncryptWithPublicPemKey(orderId));
                Wait.Show(true);
                Http.Post(AccountManager.ServerURL + "apppayserver/delOrder.php", formData, DelOrderHttpCallback);
                */
            }
        }
        private void DelOrderHttpCallback(Http.ResultType resultType, string result)
        {
            Debug.Log("DelOrderHttpCallback:" + result);
        }

        void CheckPayment()
        {
            /*
            Debug.Log(">>>>>>>>>>>>>CheckPayment：开始查询支付结果:" + currOrderId);
            Dictionary<string, string> formData = new Dictionary<string, string>();
            formData.Add("executekey", AccountManager.ExecuteKey);
            formData.Add("account", RSATool.EncryptWithPublicPemKey(AccountManager.Instance.Account));
            formData.Add("orderid", RSATool.EncryptWithPublicPemKey(currOrderId));
            Wait.Show(true);
            Http.Post(AccountManager.ServerURL + "apppayserver/alipay/checkPayment.php", formData, CheckPaymentHttpCallback);
            */
        }

        private void CheckPaymentHttpCallback(Http.ResultType resultType, string result)
        {
            Wait.Show(false);
            if (resultType == Http.ResultType.Success)
            {
                Debug.Log("CheckPaymentHttpCallback:" + result);
                JsonData json = JsonMapper.ToObject(result);
                if (json != null)
                {
                    int error = (int)json["error"];
                    if (error == 0)
                    {
                        if (PaySuccessEvent != null)
                        {
                            string productTitle = (string)json["product_title"];
                            int buyCount = int.Parse(RSAUtils.DecryptWithPublicPemKey((string)json["buy_count"]));
                            string recvName = (string)json["recv_name"];
                            string recvPhone = (string)json["recv_phone"];
                            string recvAdd = (string)json["recv_add"];
                            int totalFee = int.Parse((string)json["total_fee"]);
                            string orderId = (string)json["order_id"];
                            PaySuccessEvent(totalFee, productTitle, buyCount, orderId, recvName, recvPhone, recvAdd);
                        }
                        return;
                    }
                }
            }
        }

        [DllImport("__Internal")]
        static extern void _AliPay(string orderInfo, string callbackGo, string callbackFun);

    }
}