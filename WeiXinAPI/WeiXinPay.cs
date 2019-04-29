using LitJson;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class WeiXinPay : SingletonMonoBehaviour<WeiXinPay>
    {
        //支付-发送订单(WWW和RSA加密不支持太长的参数值，拆分1和2)
        struct OrderStruct1
        {
            public string orderId, account, productId, spbillCreateIp, isIos;
            public int buyCount;
        }
        struct OrderStruct2
        {
            public string receiverName, receiverPhone, receiverAddress;
        }
        public delegate void PaySuccessDelegate(int totalFee, string productTitle, int buyCount, string orderId, string recvName, string recvPhone, string recvAdd);
        PaySuccessDelegate PaySuccessEvent = null;
        Action OrderInvalidAction = null;
        string currOrderId;
        
        /// <summary>
        /// orderId:由APP自定义生成(建议由时间戳加随机数不加密)，支付失败时必须使用同一个，否则会二次付款,订单结束后发起新订单才可以生成新orderId
        /// <parm>fee:支付金额，单位：分</parm>
        /// </summary>
        public void Pay(string account, string productId, int buyCount, string productTitle, string receiverName, string receiverPhone, string receiverAddress, string remark, PaySuccessDelegate OnPaySuccessCallback, Action OnOrderInvalid)
        {
            if (!WeiXin.IsAppInstalled())
            {
                Wait.Show(false);
                DialogAlert.Show("提示", "请先安装微信客户端");
                return;
            }
            this.PaySuccessEvent = OnPaySuccessCallback;
            this.OrderInvalidAction = OnOrderInvalid;
            //订单数据
            OrderStruct1 data = new OrderStruct1();
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
            Http.Post(AccountManager.ServerURL + "apppayserver/weixin/requestOrder.php", formData, RequestOrderHttpCallback);
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
                JsonData json = JsonMapper.ToObject(result);
                if (json != null)
                {
                    //这里最好验证签名
                    ////////////////////////
                    this.currOrderId = (string)json["orderid"];
                    string sign = (string)json["sign"];
                    int error = (int)json["error"];
                    if (error == 0)
                    {
                        JsonData jsonData = json["data"];
                        string appId = (string)jsonData["appid"];
                        string partnerId = (string)jsonData["partnerid"];
                        string prepayId = (string)jsonData["prepayid"];
                        string packageValue = (string)jsonData["package"];
                        string nonceStr = (string)jsonData["noncestr"];
                        string timeStamp = (string)jsonData["timestamp"];
                        if (WeiXin.AppId.Equals(appId) && !string.IsNullOrEmpty(currOrderId))
                        {
                            Debug.Log(">>>>>>>>>>>>>订单生成，开始支付");
                            if (Application.platform == RuntimePlatform.Android)
                            {
                                AndroidUtils.CallAndroidStaticFunction("cn.btoolkit.weixinapi.WeiXinPay", "pay", appId, partnerId, prepayId, packageValue, nonceStr, timeStamp, sign, gameObject.name, "WeiXinPayCallback");
                            }
                            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                            {
                                _WeiXinPay(partnerId, prepayId, packageValue, nonceStr, int.Parse(timeStamp), sign, gameObject.name, "WeiXinPayCallback");
                            }
                            return;
                        }
                    }
                    else if (error == 100)
                    {
                        DialogAlert.Show("提示", "该订单已支付成功");
                        Wait.Show(false);
                        return;
                    }
                }
            }
            Wait.Show(false);
            DialogAlert.Show("提示", "该订单已失效!", OnCloseAlert);
        }

        void OnCloseAlert()
        {
            if (OrderInvalidAction != null)
            {
                OrderInvalidAction();
            }
        }

        void WeiXinPayCallback(string msg)
        {
            Wait.Show(false);
            Debug.Log(">>>>>>>>>>>>>WeiXinPayCallback:" + msg);
            if (!string.IsNullOrEmpty(msg) && msg.Contains("success"))
            {
                CheckPayment();
            }
            else
            {
                DelOrderOnServer(currOrderId);
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
            Http.Post(AccountManager.ServerURL + "apppayserver/weixin/checkPayment.php", formData, CheckPaymentHttpCallback);
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
        static extern void _WeiXinPay(string partnerId, string prepayId, string package, string nonceStr, int timeStamp, string sign, string callbackGo, string callbackFun);

    }
}