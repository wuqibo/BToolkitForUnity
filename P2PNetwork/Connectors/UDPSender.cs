using System;
using System.Net;
using System.Net.Sockets;

namespace BToolkit.P2PNetwork {
    /// <summary>
    /// UDP发送器
    /// </summary>
    public class UDPSender {

        private Socket socket;
        private IPEndPoint ipEndPoint;

        /// <summary>
        /// UDP发送器
        /// </summary>
        public UDPSender(string targetIP,int targetPort) {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(targetIP),targetPort);
        }

        /// <summary>
        /// 开始启动UDP发送器
        /// </summary>
        public void Start() {
			if (P2PUtility.isIPV6) {
				socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			} else {
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			}
        }

        /// <summary>
        /// 发出消息
        /// </summary>
        public void Send(byte[] bytes) {
            if(socket != null) {
                //P2PDebug.Log("UDPSender : " + P2PUtility.BytesToJson(bytes) + " targetIP: " + ipEndPoint.Address + ":" + ipEndPoint.Port);
                socket.SendTo(bytes,ipEndPoint);
            }
        }

        /// <summary>
        /// 关闭发送器
        /// </summary>
        public void Stop() {
            try {
                socket.Close();
                socket = null;
            } catch(SocketException e) {
                P2PDebug.LogError("UDPSender Socket 关闭出错:" + e);
            } catch(Exception e) {
                P2PDebug.LogError("UDPSender Socket 关闭出错:" + e);
            }
        }

    }
}
