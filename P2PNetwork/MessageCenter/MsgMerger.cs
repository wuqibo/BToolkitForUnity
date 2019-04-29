using System;
using System.Text;

namespace BToolkit.P2PNetwork {
    /// <summary>
    /// Socket接收消息有时候会将一条消息拆包发送，需判断封包后再传给MessageManager处理
    /// </summary>
    public class MsgMerger {

        private StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// Socket接收消息有时候会将一条消息拆包发送，需判断封包后再传给MessageManager处理
        /// </summary>
        public MsgMerger() { }

        /// <summary>
        /// 收到多个拆包判断封包后执行handler
        /// </summary>
        public string Receive(byte[] bytes, int index, int count, Action<string> handler) {
            string receiveStr = P2PUtility.BytesToJson(bytes, index, count);
            //P2PDebug.LogWarning(receiveStr);
            stringBuilder.Append(receiveStr);
            //如果发现消息尾，循环取出完整消息包
            while (stringBuilder.ToString().Contains(MsgDivid.MsgEOF)) {
                int msgEOFIndex = stringBuilder.ToString().IndexOf(MsgDivid.MsgEOF);
                string fullMsg = stringBuilder.ToString().Substring(0, msgEOFIndex);
                if (!string.IsNullOrEmpty(fullMsg)) {
                    stringBuilder.Remove(0, msgEOFIndex + MsgDivid.MsgEOF.Length);
                    if (handler != null) {
                        handler(fullMsg);
                    }
                    return fullMsg;
                }
            }
            return string.Empty;
        }
    }
}
