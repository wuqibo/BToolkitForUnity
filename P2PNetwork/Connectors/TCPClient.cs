using System;
using System.Net;
using System.Net.Sockets;

namespace BToolkit.P2PNetwork
{
    /// <summary>
    /// 负责连接到服务器的连接器，用于接收消息。房间内有几个Peer（包括本机），本地就对应有几个TCPClient进行一对一连接，存储在room.localClients字典里
    /// </summary>
    public class TCPClient
    {
        private UDPSender udpSender;
        private Socket socket;
        private bool canListenConn;
        public PeerInfo serverInfo;
        /// <summary>
        /// 是否本地Peer
        /// </summary>
        public bool isLocal { get { return serverInfo.peerId.Equals(P2PNetwork.localPeerId); } }

        public static void ConnectToServer(string serverPeerId, string serverIP, int serverPort)
        {
            //P2PDebug.Log(serverPeerId + "/" + serverIP + "/" + serverPort + "/" + OnReceive);
            int length = P2PNetwork.room.clients.Count;
            for (int i = 0; i < length; i++)
            {
                TCPClient client = P2PNetwork.room.clients[i];
                if (client != null)
                {
                    if (client.serverInfo.peerId.Equals(serverPeerId))
                    {
                        P2PDebug.LogError("ConnectToServer -> Client(" + serverPeerId + ")已经存在");
                        return;
                    }
                }
            }
            new TCPClient(serverPeerId, serverIP, serverPort);
        }

        /// <summary>
        /// 建立双向链接（在收到有客户端连过来的时候，服务器反连）
        /// </summary>
        public static void ServerConnectBackToClient(string clientPeerId, string clientIP, int clientPort)
        {
            new TCPClient(clientPeerId, clientIP, clientPort);
        }

        /// <summary>
        /// 判断当前客户端列表里是否也存在某个Client
        /// </summary>
        public static bool ContainsClient(string clientPeerId)
        {
            int length = P2PNetwork.room.clients.Count;
            for (int i = 0; i < length; i++)
            {
                TCPClient client = P2PNetwork.room.clients[i];
                if (client != null)
                {
                    if (client.serverInfo.peerId.Equals(clientPeerId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 负责连接到服务器的连接器，用于接收消息。房间内有几个Peer（包括本机），本地就对应有几个TCPClient进行一对一连接，存储在room.localClients字典里
        /// </summary>
        private TCPClient(string serverPeerId, string serverIP, int serverPort)
        {
            serverInfo.peerId = serverPeerId;
            serverInfo.ip = serverIP;
            serverInfo.port = serverPort;
            udpSender = new UDPSender(serverIP, serverPort);
            if (P2PUtility.isIPV6)
            {
                socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            socket.BeginConnect(new IPEndPoint(IPAddress.Parse(serverInfo.ip), serverInfo.port), (IAsyncResult ar) =>
            {
                Socket socket = (Socket)ar.AsyncState;
                try
                {
                    socket.EndConnect(ar);
                    P2PDebug.Log("TCPClient连接服务器(" + serverInfo.ip + ":" + serverInfo.port + ")成功");
                    P2PNetwork.room.AddLocalClient(this);//成功后将自己加入列表
                    SendLocalInfoToServer();
                    P2PNetwork.room.JoinStateCallback(ConnState.Success);
                    canListenConn = true;
                    if (P2PNetwork.room.isMaster)
                    {
                        P2PNetwork.room.MasterTellOtherPeers(serverInfo.peerId);
                        //选定一个人充当房主候选人并告知连入的人
                        if (string.IsNullOrEmpty(P2PNetwork.room.masterCandidatePeerId))
                        {
                            if (!P2PNetwork.localPeerId.Equals(serverInfo.peerId))
                            {
                                P2PNetwork.room.masterCandidatePeerId = serverInfo.peerId;
                                //将房主候选人的房间设定位断线重连的房间目标
                                P2PNetwork.room.MasterSetLocalRoomInfoForReJoin(serverInfo.ip, serverInfo.port);
                            }
                        }
                        //广播房主候选人
                        P2PNetwork.room.MasterBroadcastCandidatePeerId(serverInfo.peerId);
                        //更新房间内人数
                        P2PNetwork.room.info.currNum = P2PNetwork.room.clients.Count;
                        P2PNetwork.room.ShareRoomInfo();
                    }
                }
                catch (Exception e)
                {
                    P2PDebug.Log("TCPClient连接服务器失败" + e.Message);
                    OnSocketConnectWrong();
                    P2PNetwork.room.JoinStateCallback(ConnState.Failed);
                }
            }, socket);
            udpSender.Start();
        }

        public void Update()
        {
            if (canListenConn)
            {
                //监听本地网络是否断开
                if (isLocal)
                {
                    if (!socket.Connected)
                    {
                        OnLocalTCPClientDisconnect();
                        canListenConn = false;
                    }
                }
            }
        }

        /// <summary>
        /// 发送心跳确保监听连接状态
        /// </summary>
        public void SendHeartbeat()
        {
            SynDataBase baseData = new SynDataBase(P2PMsgType.Heartbeat, null, null, P2PNetwork.localPeerId, null);
            Send(baseData, null, Channel.TCP);
        }

        void OnLocalTCPClientDisconnect()
        {
            P2PNetwork.room.Leave(true, true);
            P2PNetwork.room = null;
        }

        void OnSocketConnectWrong()
        {
            Destroy();
            if (P2PNetwork.room != null)
            {
                P2PNetwork.room.Leave(false, false);
                P2PNetwork.room = null;
            }
        }

        /// <summary>
        /// 第一次连上服务器，则向服务器发送本地数据以便让服务器建立P2P双向连接
        /// </summary>
        private void SendLocalInfoToServer()
        {
            try
            {
                SynDataBase dataBase = new SynDataBase(P2PMsgType.TCPClientOrginalInfo, null, null, P2PNetwork.localPeerId, null);
                PeerInfo localPeerInfo = new PeerInfo();
                localPeerInfo.peerId = P2PNetwork.localPeerId;
                localPeerInfo.ip = P2PNetwork.localIP;
                localPeerInfo.port = P2PNetwork.localPort;
                Send(dataBase, localPeerInfo, Channel.TCP);
            }
            catch (Exception e)
            {
                P2PDebug.LogError(e);
            }
        }

        /// <summary>
        /// 向服务端发送数据
        /// </summary>
        public void Send(SynDataBase baseData, object customData, Channel channel)
        {
            try
            {
                //两个数据拼接成一个消息包
                string json = DataToJson(baseData, customData);
                switch (channel)
                {
                    case Channel.UDP:
                        byte[] bytes = P2PUtility.JsonToBytes(json);
                        udpSender.Send(bytes);
                        break;
                    case Channel.TCP:
                        //TCP要加消息尾
                        json = json + MsgDivid.MsgEOF;
                        bytes = P2PUtility.JsonToBytes(json);
                        if (baseData.msgType == 101)
                        {
                            P2PDebug.LogError("房主广播座位：" + json);
                        }
                        socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, (IAsyncResult asyncResult) => { socket.EndSend(asyncResult); }, socket);
                        break;
                }
            }
            catch (SocketException e)
            {
                P2PDebug.LogError(e);
            }
            catch (Exception e)
            {
                P2PDebug.LogError(e);
            }
        }

        /// <summary>
        /// 将SynDataBase和CustomData转为Json
        /// </summary>
        public static string DataToJson(SynDataBase baseData, object customData)
        {
            string json = P2PUtility.ObjectToJson(baseData);
            json += MsgDivid.BaseData;
            if (customData != null)
            {
                json += P2PUtility.ParseCustomData(customData);
            }
            return json;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            P2PNetwork.room.RemoveLocalClient(this);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Destroy()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
                P2PDebug.Log("TCPClient Close");
            }
            if (udpSender != null)
            {
                udpSender.Stop();
                udpSender = null;
            }
        }
    }
}