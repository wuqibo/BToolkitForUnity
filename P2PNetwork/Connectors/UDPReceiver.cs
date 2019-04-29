using System;
using System.Net;
using System.Net.Sockets;

namespace BToolkit.P2PNetwork {
	/// <summary>
	/// UDP接收器
	/// </summary>
	public class UDPReceiver {

		private Socket socket;
		private IPEndPoint ipEndPoint;
		private byte[] buffer;
		private int bufferSize = 1024;
		private int port;
		private Action<string> OnReceive;

		/// <summary>
		/// UDP接收器
		/// </summary>
		public UDPReceiver(int port, Action<string> OnReceive) {
			this.port = port;
			this.OnReceive = OnReceive;
		}

		/// <summary>
		/// 开始监听广播
		/// </summary>
		public bool Start() {
			P2PDebug.Log("UDPReceiver Start");
			if (P2PUtility.isIPV6) {
				socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
				ipEndPoint = new IPEndPoint(IPAddress.IPv6Any, port);
			} else {
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				ipEndPoint = new IPEndPoint(IPAddress.Any, port);
			}
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			try {
				socket.Bind(ipEndPoint);
                buffer = new byte[bufferSize];
                socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
				return true;
			} catch (Exception e) {
				P2PDebug.LogError(e);
			}
			P2PDebug.LogError("UDPReceiver 启动失败");
			return false;
		}

		private void ReceiveCallback(IAsyncResult ar) {
			int bytesRead = socket.EndReceive(ar);
			if (bytesRead > 0) {
				string receiveJson = P2PUtility.BytesToJson(buffer, 0, bytesRead);
                //P2PDebug.Log("UDPReceiver ：" + receiveJson);
                if (OnReceive != null) {
					OnReceive(receiveJson);
				}
			}
            buffer = new byte[bufferSize];
            socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
		}

		/// <summary>
		/// 停止监听广播
		/// </summary>
		public void Stop() {
			if (socket != null) {
				try {
					socket.Close();
					socket = null;
				} catch (Exception e) {
					P2PDebug.LogError("UDPReceive Socket 关闭出错:" + e);
				}
			}
		}
	}
}
