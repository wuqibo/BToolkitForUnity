using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace BToolkit.P2PNetwork
{
    /// <summary>
    /// 监听其它玩家连入
    /// </summary>
    public class TCPServer
    {
        private Socket serverSocket;
        private IPEndPoint ipEndPoint;
        /// <summary>
        /// 所有连进来的远程客户端列表
        /// </summary>
        public List<Session> sessions = new List<Session>();
        public static bool canListenHeartbeat;
        private bool canSendHeartbeat;
        private float sendHeartbeatTimer;

        /// <summary>
        /// 监听其它玩家连入
        /// </summary>
        public TCPServer()
        {
            P2PNetwork.localIP = P2PUtility.internalIP;
            P2PNetwork.localPort = P2PConfig.Instance.Port;
            if (P2PUtility.isEditor)
            {
                if (P2PConfig.Instance.isTestInSamePC)
                {
                    P2PNetwork.localPort = UnityEngine.Random.Range(P2PConfig.Instance.Port, P2PConfig.Instance.Port + 1000);
                }
            }
        }

        /// <summary>
        /// 启动本地TCPServer
        /// </summary>
        public bool Start()
        {
            P2PDebug.Log("启动本地TCPServer:" + P2PNetwork.localIP + ":" + P2PNetwork.localPort);
            if (!P2PNetwork.localIP.Contains("0.0.0.0"))
            {
                //创建Socket
                try
                {
                    if (P2PUtility.isIPV6)
                    {
                        serverSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                        ipEndPoint = new IPEndPoint(IPAddress.IPv6Any, P2PNetwork.localPort);
                    }
                    else
                    {
                        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        ipEndPoint = new IPEndPoint(IPAddress.Any, P2PNetwork.localPort);
                    }
                    serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    serverSocket.Bind(ipEndPoint);
                    serverSocket.Listen(100);//对象池容量
                    serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), serverSocket);
                    //开始心跳线程
                    canSendHeartbeat = true;
                    canListenHeartbeat = true;
                    return true;
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
            P2PDebug.LogWarning("TCPServer 启动失败");
            return false;
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ar);
            P2PDebug.Log("TCPServer有客户端连入:" + clientSocket.RemoteEndPoint);
            Session.CreateClientOnServer(this, clientSocket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), serverSocket);
        }

        /// <summary>
        /// 添加一个客户端
        /// </summary>
        public void AddClientOnServer(Session session)
        {
            if (session != null)
            {
                int length = sessions.Count;
                for (int i = 0; i < length; i++)
                {
                    Session s = sessions[i];
                    if (s != null)
                    {
                        if (s.sessionInfo.peerId.Equals(session.sessionInfo.peerId))
                        {
                            sessions[i] = session;
                            return;
                        }
                    }
                }
                sessions.Add(session);
            }
        }

        /// <summary>
        /// 移除一个客户端,并判断是否需要查找新的Peer充当房主
        /// </summary>
        private void RemoveClientOnServer(Session session)
        {
            if (session != null)
            {
                int length = sessions.Count;
                for (int i = 0; i < length; i++)
                {
                    Session s = sessions[i];
                    if (s != null)
                    {
                        if (s.sessionInfo.peerId.Equals(session.sessionInfo.peerId))
                        {

                            sessions.Remove(s);
                            s.Destroy();
                            s = null;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断服务器是否存在某个客户端的连接
        /// </summary>
        public bool IsContainsClient(Session session)
        {
            bool isContains = false;
            int length = sessions.Count;
            for (int i = 0; i < length; i++)
            {
                Session s = sessions[i];
                if (s != null)
                {
                    if (s.sessionInfo.peerId.Equals(session.sessionInfo.peerId))
                    {
                        isContains = true;
                        break;
                    }
                }
            }
            return isContains;
        }

        public void Update()
        {
            sendHeartbeatTimer += UnityEngine.Time.deltaTime;
            if (sendHeartbeatTimer >= P2PConfig.Instance.HeartbeatInterval)
            {
                sendHeartbeatTimer = 0f;
                if (canSendHeartbeat)
                {
                    for (int i = 0; i < P2PNetwork.room.clients.Count; i++)
                    {
                        TCPClient client = P2PNetwork.room.clients[i];
                        if (client != null)
                        {
                            client.SendHeartbeat();
                        }
                    }
                }
                if (canListenHeartbeat)
                {
                    int length = sessions.Count;
                    for (int i = 0; i < length; i++)
                    {
                        Session session = sessions[i];
                        if (session != null)
                        {
                            session.HeartbeatListen();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 系统函数，由消息管理器调用
        /// </summary>
        public void KeepClientAlive(string peerId)
        {
            //P2PDebug.Log("KeepClientAlive: " + peerId);
            int length = sessions.Count;
            for (int i = 0; i < length; i++)
            {
                Session session = sessions[i];
                if (session != null)
                {
                    if (session.sessionInfo.peerId.Equals(peerId))
                    {
                        session.KeepAlive();
                        break;
                    }
                }
            }
        }

        //MessageManager调用
        public void ReceiveLeave(string peerId)
        {
            P2PDebug.Log("收到客户端主动断开消息：" + peerId);
            int length = sessions.Count;
            for (int i = 0; i < length; i++)
            {
                Session session = sessions[i];
                if (session != null)
                {
                    if (session.sessionInfo.peerId.Equals(peerId))
                    {
                        session.OneClientDisconnectOnServer(false);
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前客户端数量
        /// </summary>
        /// <returns></returns>
        public int GetClientNumber()
        {
            return sessions.Count;
        }

        /// <summary>
        /// 停止并销毁
        /// </summary>
        public void Stop()
        {
            canSendHeartbeat = false;
            int length = sessions.Count;
            for (int i = 0; i < length; i++)
            {
                Session session = sessions[i];
                if (session != null)
                {
                    session.Destroy();
                }
            }
            sessions = null;
            if (serverSocket != null)
            {
                serverSocket.Close();
                serverSocket = null;
                P2PDebug.Log("TCPServer Close");
            }
            P2PDebug.Log("本地TCPServer已关闭");
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //内部类(一个客户端连入本服务器立即新增一个Client并保存在列表TCPServer.clients里)///////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 所有连接进来的客户端，存储在tcpServer.clients列表里
        /// </summary>
        public class Session
        {
            private TCPServer tcpServer;
            private Socket clientSocket;
            public PeerInfo sessionInfo;
            private byte[] buffer;
            private const int bufferSize = 1024;
            private MsgMerger msgMerger;
            private int aliveTimer;
            /// <summary>
            /// 是否为本地客户端
            /// </summary>
            public bool isLocal
            {
                get { return sessionInfo.peerId.Equals(P2PNetwork.localPeerId); }
            }
            public static void CreateClientOnServer(TCPServer tcpServer, Socket clientSocket)
            {
                new Session(tcpServer, clientSocket);
            }

            /// <summary>
            /// 所有连接进来的客户端，存储在tcpServer.clients列表里
            /// </summary>
            private Session(TCPServer tcpServer, Socket clientSocket)
            {
                this.tcpServer = tcpServer;
                this.clientSocket = clientSocket;
                aliveTimer = P2PConfig.Instance.HeartbeatInterval + 10;
                msgMerger = new MsgMerger();
                buffer = new byte[bufferSize];
                clientSocket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
            }

            private void ReceiveCallback(IAsyncResult ar)
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                int bytesRead = clientSocket.EndReceive(ar);
                msgMerger.Receive(buffer, 0, bytesRead, (string msg) =>
                {
                    string baseDataStr = msg.Substring(0, msg.IndexOf(MsgDivid.BaseData));
                    SynDataBase baseData = P2PUtility.JsonToObject<SynDataBase>(baseDataStr);
                    if (baseData.msgType == P2PMsgType.TCPClientOrginalInfo)
                    {
                        string customDataStr = msg.Remove(0, msg.IndexOf(MsgDivid.BaseData) + MsgDivid.BaseData.Length);
                        sessionInfo = P2PUtility.JsonToObject<PeerInfo>(customDataStr);
                        P2PNetwork.room.tcpServer.AddClientOnServer(this);
                        P2PNetwork.room.TriggerJoinedEvent(sessionInfo.peerId);
                        //如果不是本机连进来，则建立P2P双向连接
                        if (!P2PNetwork.localPeerId.Equals(sessionInfo.peerId) && !TCPClient.ContainsClient(sessionInfo.peerId))
                        {
                            TCPClient.ServerConnectBackToClient(sessionInfo.peerId, sessionInfo.ip, sessionInfo.port);
                        }
                    }
                    else
                    {
                        Debuger.LogError("收到消息：" + msg);
                        MessageManager.Instance.Receive(msg);
                    }
                });
                buffer = new byte[bufferSize];
                clientSocket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
            }

            private bool IsSocketConnected(Socket s)
            {
                return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
            }

            /// <summary>
            /// 保证活动状态，否则被视为客户端已断开
            /// </summary>
            public void KeepAlive()
            {
                aliveTimer = P2PConfig.Instance.HeartbeatInterval + 2;
            }

            public void HeartbeatListen()
            {
                if (aliveTimer > 0)
                {
                    aliveTimer--;
                    if (aliveTimer <= 0)
                    {
                        //P2PDebug.Log("超时没有收到客户端心跳 P2PUtility.IsNetWorkOK : " + P2PUtility.IsNetWorkOK);
                        OneClientDisconnectOnServer(true);
                    }
                }
            }

            public void OneClientDisconnectOnServer(bool isException)
            {
                lock (P2PNetwork.room)
                {
                    if (P2PNetwork.room == null || !tcpServer.IsContainsClient(this))
                    {
                        return;
                    }
                    P2PDebug.Log("ClientDisconnectOnServer");
                    tcpServer.RemoveClientOnServer(this);
                    int length = P2PNetwork.room.clients.Count;
                    for (int i = 0; i < length; i++)
                    {
                        TCPClient tcpClient = P2PNetwork.room.clients[i];
                        if (tcpClient != null)
                        {
                            if (tcpClient.serverInfo.peerId.Equals(sessionInfo.peerId))
                            {
                                tcpClient.Disconnect();
                            }
                        }
                    }
                    //更新房间内人数
                    if (P2PNetwork.room.isMaster)
                    {
                        P2PNetwork.room.info.currNum = tcpServer.GetClientNumber();
                        P2PNetwork.room.ShareRoomInfo();
                    }
                    if (string.IsNullOrEmpty(sessionInfo.peerId))
                    {
                        P2PDebug.LogError("有客户端断开，但无法得到客户端Peer Id");
                    }
                    else
                    {
                        P2PNetwork.room.ResetRoomMaster(sessionInfo.peerId);
                    }
                    MainThread.Run(() =>
                    {
                        //如果本地没有网络，则断线的是自己，此时退出房间
                        P2PDebug.Log("Local Network:" + P2PUtility.IsNetWorkOK);
                        if (P2PUtility.IsNetWorkOK)
                        {
                            if (P2PNetwork.room != null && P2PNetwork.room.hadTriggerJoinedEvent)
                            {
                                if (P2PNetwork.Instance.SomeoneLeavedRoomEvent != null)
                                {
                                    P2PNetwork.Instance.SomeoneLeavedRoomEvent(sessionInfo.peerId, isException);
                                }
                                length = P2PNetwork.Instance.networkObjectsInScene.Count;
                                for (int i = 0; i < length; i++)
                                {
                                    P2PNetwork.Instance.networkObjectsInScene[i].SomeoneLeavedRoom(sessionInfo.peerId, isException);
                                }
                            }
                        }
                        else
                        {
                            P2PNetwork.room.Leave(true, true);
                            P2PNetwork.room = null;
                        }
                    });
                }
            }

            /// <summary>
            /// 销毁
            /// </summary>
            public void Destroy()
            {
                if (clientSocket != null)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    clientSocket = null;
                    P2PDebug.Log("Client Close");
                }
                msgMerger = null;
            }
        }
    }
}