using System;
using System.Net;
using System.Net.Sockets;

namespace BToolkit.P2PNetwork {
    /// <summary>
    /// UDP广播器
    /// </summary>
    public class UDPBroadcaster {

        private Socket socket;
        private IPEndPoint ipEndPoint;

        /// <summary>
        /// UDP广播器
        /// </summary>
        public UDPBroadcaster(int port) {
            ipEndPoint = new IPEndPoint(IPAddress.Broadcast,port);
        }

        /// <summary>
        /// 开始启动UDP广播器
        /// </summary>
        public void Start() {
			if (P2PUtility.isIPV6) {
				socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			} else {
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			}
            socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.Broadcast,1);
        }

        /// <summary>
        /// 发出广播
        /// </summary>
        public void Send(byte[] bytes) {
            if(socket != null) {
                socket.SendTo(bytes,ipEndPoint);
            }
        }

        /// <summary>
        /// 关闭广播器
        /// </summary>
        public virtual void Stop() {
            if(socket != null) {
                try {
                    socket.Close();
                    socket = null;
                } catch(SocketException e) {
                    P2PDebug.LogError("TCPServer Socket 关闭出错:" + e);
                } catch(Exception e) {
                    P2PDebug.LogError("TCPServer Socket 关闭出错:" + e);
                }
            }
        }
    }
}
