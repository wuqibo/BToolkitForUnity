using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace BToolkit.P2PNetwork
{
    public class Room
    {
        /// <summary>
        /// 房间自检销毁类
        /// </summary>
        public class RoomAlive
        {
            public RoomInfo info;
            private int time;
            public RoomAlive(RoomInfo roomInfo)
            {
                this.info = roomInfo;
                KeepAlive();
            }
            public void KeepAlive()
            {
                time = (int)broadcastInterval + 1;
            }
            public bool Check()
            {
                if (time > 0)
                {
                    time--;
                }
                return time > 0;
            }
        }
        /// <summary>
        /// TCP服务器,接受多个客户端连接
        /// </summary>
        public TCPServer tcpServer;
        /// <summary>
        /// UDP接收器
        /// </summary>
        public UDPReceiver udpReceiver;
        /// <summary>
        /// TCP客户端列表，负责连接多个远程Peer
        /// </summary>
        public List<TCPClient> clients = new List<TCPClient>();
        public ConnResultEvent JoinCallback;
        /// <summary>
        /// 房间信息广播器，广播房间信息到大厅，以便其他玩家发现和在房间列表里显示
        /// </summary>
        private UDPBroadcaster broadcasterToLobby;
        public RoomInfo info;
        private byte[] roomInfoBytes;
        public const float broadcastInterval = 2;
        private float broadcastTimer;
        private bool canBroadcast;
        public string masterPeerId { get { return info.masterPeerId; } }//房主ID和房主候选人ID
        public string masterCandidatePeerId;//房主ID和房主候选人ID
        public static RoomInfo roomInfoForReJoin;//候选人的房间信息，静态记录不随房间清除而清除，用于断线重连
        /// <summary>
        /// 房主身份，负责广播房间信息以及控制房间内游戏进程。当房主掉线后房主身份自动转移，以确保房间内始终有一位房主。
        /// </summary>
        public bool isMaster { private set; get; }
        public int currMemberCount { get { return clients.Count; } }
        public int maxMemberCount { get { return info.maxNum; } }
        private bool isExceptionLeave;
        public bool hadTriggerJoinedEvent { private set; get; }

        private Room()
        {
            isExceptionLeave = true;
            tcpServer = new TCPServer();
            broadcasterToLobby = new UDPBroadcaster(P2PConfig.Instance.Port);
            udpReceiver = new UDPReceiver(P2PNetwork.localPort, MessageManager.Instance.Receive);
        }

        /// <summary>
        /// 创建一个房间
        /// </summary>
        public static Room Create(string customProperty, int maxNum)
        {
            Room room = new Room();
            room.isMaster = true;
            room.info = new RoomInfo();
            room.info.roomId = P2PUtility.GetMD5_32(P2PUtility.deviceId + DateTime.Now.ToString("HHmmssfff"), true);
            room.info.customProperty = customProperty;
            room.info.maxNum = maxNum;
            room.info.currNum = 1;
            room.info.isOpen = true;
            room.info.peerId = P2PNetwork.localPeerId;
            room.info.masterPeerId = P2PNetwork.localPeerId;
            room.info.localIP = P2PNetwork.localIP;
            room.info.port = P2PNetwork.localPort;
            room.BroadcastToLobby();
            room.canBroadcast = true;
            if (room.tcpServer.Start())
            {
                room.broadcasterToLobby.Start();
                room.udpReceiver.Start();
                return room;
            }
            return null;
        }

        /// <summary>
        /// 房主收到新的成员连入时，告知房间内所有成员信息，以便全部建立连接,接收方法：ReceivePeers
        /// </summary>
        public void MasterTellOtherPeers(string targetClientPeerId)
        {
            SynDataBase baseData = new SynDataBase(P2PMsgType.OtherPeersInRoom, null, null, null, null);
            List<string> peerIds = new List<string>();
            List<string> ips = new List<string>();
            List<int> ports = new List<int>();
            int length = tcpServer.sessions.Count;
            for (int i = 0; i < length; i++)
            {
                TCPServer.Session session = tcpServer.sessions[i];
                //排除掉房主
                if (session != null && !session.sessionInfo.peerId.Equals(P2PNetwork.localPeerId))
                {
                    peerIds.Add(session.sessionInfo.peerId);
                    ips.Add(session.sessionInfo.ip);
                    ports.Add(session.sessionInfo.port);
                }
            }
            PeersInRoom otherPeers = new PeersInRoom();
            otherPeers.peerIds = peerIds.ToArray();
            otherPeers.ips = ips.ToArray();
            otherPeers.ports = ports.ToArray();
            SendMsg(ReceiveTarget.All, targetClientPeerId, baseData, otherPeers, Channel.TCP);
        }

        /// <summary>
        /// 接收房主广播的当前房间内其他成员列表并连接
        /// </summary>
        public void ReceiveAndConnectOtherPeers(PeersInRoom peersInRoomData)
        {
            int currCount = peersInRoomData.peerIds.Length;
            for (int i = 0; i < currCount; i++)
            {
                if (!P2PNetwork.localPeerId.Equals(peersInRoomData.peerIds[i]))
                {
                    TCPClient.ConnectToServer(peersInRoomData.peerIds[i], peersInRoomData.ips[i], peersInRoomData.ports[i]);
                }
            }
        }

        /// <summary>
        /// 广播当前房主候选人,参数peerId不为null则只广播指定一个人，参数为null则广播其他所有人
        /// </summary>
        public void MasterBroadcastCandidatePeerId(string receivePeerId)
        {
            SynDataBase baseData = new SynDataBase(P2PMsgType.MasterCandidate, null, null, null, null);
            MasterInfo masterInfo = new MasterInfo();
            masterInfo.masterPeerId = this.masterPeerId;
            masterInfo.candidatePeerId = this.masterCandidatePeerId;
            SendMsg(ReceiveTarget.All, receivePeerId, baseData, masterInfo, Channel.TCP);
        }

        /// <summary>
        /// 房主设置本地的断线重连目标房间
        /// </summary>
        public void MasterSetLocalRoomInfoForReJoin(string ip, int port)
        {
            roomInfoForReJoin = info;
            roomInfoForReJoin.localIP = ip;
            roomInfoForReJoin.port = port;
        }

        /// <summary>
        /// 非房主接收房主候选人，并设置断线重连目标房间
        /// </summary>
        public void ReceiveMasterCandidate(MasterInfo masterInfo)
        {
            this.info.masterPeerId = masterInfo.masterPeerId;
            this.masterCandidatePeerId = masterInfo.candidatePeerId;
            roomInfoForReJoin = info;
            int length = clients.Count;
            for (int i = 0; i < length; i++)
            {
                TCPClient client = clients[i];
                if (client != null)
                {
                    if (client.serverInfo.peerId.Equals(this.masterPeerId))
                    {
                        roomInfoForReJoin.localIP = client.serverInfo.ip;
                        roomInfoForReJoin.port = client.serverInfo.port;
                    }
                }
            }
        }

        /// <summary>
        /// 加入一个网络已存在的房间
        /// </summary>
        public static Room Join(RoomInfo roomInfo, ConnResultEvent Callback)
        {
            Room room = new Room();
            room.info = roomInfo;
            room.info.peerId = P2PNetwork.localPeerId;
            room.info.localIP = P2PNetwork.localIP;
            room.info.port = P2PNetwork.localPort;
            room.BroadcastToLobby();
            room.isMaster = false;
            room.JoinCallback = Callback;
            if (room.tcpServer.Start())
            {
                room.udpReceiver.Start();
                return room;
            }
            room.tcpServer.Stop();
            room.udpReceiver.Stop();
            return null;
        }

        public void JoinStateCallback(ConnState connState)
        {
            MainThread.Run(() =>
            {
                if (connState == ConnState.Success && P2PNetwork.Instance.currState != State.InRoom)
                {
                    P2PNetwork.Instance.currState = State.InRoom;
                }
                if (JoinCallback != null)
                {
                    JoinCallback(connState);
                    JoinCallback = null;
                }
            });
        }

        private void BecomeNewMaster()
        {
            isMaster = true;
            if (info.isOpen)
            {
                canBroadcast = true;
                broadcasterToLobby.Start();
            }
            info.masterPeerId = P2PNetwork.localPeerId;
            //选定一个候选人
            int length = tcpServer.sessions.Count;
            if (length == 1)
            {
                masterCandidatePeerId = null;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    TCPServer.Session session = tcpServer.sessions[i];
                    if (session != null)
                    {
                        if (!session.sessionInfo.peerId.Equals(P2PNetwork.localPeerId) && !session.sessionInfo.peerId.Equals(masterCandidatePeerId))
                        {
                            masterCandidatePeerId = session.sessionInfo.peerId;
                            MasterBroadcastCandidatePeerId(null);
                            break;
                        }
                    }
                }
            }
            if (P2PNetwork.Instance.BecomeRoomMasterEvent != null)
            {
                MainThread.Run(P2PNetwork.Instance.BecomeRoomMasterEvent);
            }
            //更新房间内人数
            info.currNum = tcpServer.GetClientNumber();
            ShareRoomInfo();
        }

        /// <summary>
        /// 发送房间消息，当receivePeerId不为空时，只发给指定Id的Peer
        /// <para>receiveTarget：接收的对象群</para>
        /// <para>targetPeerId：客户端的Peer ID(由Device Id充当),当该值不为空时，将忽略receiveTarget参数只发给特定的Peer</para>
        /// <para>data：要发送的数据</para>
        /// </summary>
        public void SendMsg(ReceiveTarget receiveTarget, string receivePeerId, SynDataBase baseData, object customData, Channel channel = Channel.TCP)
        {
            int length = clients.Count;
            if (!string.IsNullOrEmpty(receivePeerId))
            {
                for (int i = 0; i < length; i++)
                {
                    TCPClient client = clients[i];
                    if (client != null)
                    {
                        if (client.serverInfo.peerId.Equals(receivePeerId))
                        {
                            if (client.isLocal)
                            {
                                MessageManager.Instance.Receive(TCPClient.DataToJson(baseData, customData));
                            }
                            else
                            {
                                client.Send(baseData, customData, channel);
                            }
                        }
                    }
                }
            }
            else
            {
                if (receiveTarget == ReceiveTarget.All)
                {
                    for (int i = 0; i < length; i++)
                    {
                        TCPClient client = clients[i];
                        if (client != null)
                        {
                            if (client.isLocal)
                            {
                                MessageManager.Instance.Receive(TCPClient.DataToJson(baseData, customData));
                            }
                            else
                            {
                                client.Send(baseData, customData, channel);
                            }
                        }
                    }
                }
                else if (receiveTarget == ReceiveTarget.Other)
                {
                    for (int i = 0; i < length; i++)
                    {
                        TCPClient client = clients[i];
                        if (client != null)
                        {
                            if (!client.isLocal)
                            {
                                client.Send(baseData, customData, channel);
                            }
                        }
                    }
                }
                else if (receiveTarget == ReceiveTarget.RoomMaster)
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        TCPClient client = clients[i];
                        if (client != null)
                        {
                            if (client.serverInfo.peerId.Equals(P2PNetwork.room.masterPeerId))
                            {
                                if (client.isLocal)
                                {
                                    MessageManager.Instance.Receive(TCPClient.DataToJson(baseData, customData));
                                }
                                else
                                {
                                    client.Send(baseData, customData, channel);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当成功连接到一个远程Server后，本地会将Client加入列表
        /// </summary>
        public void AddLocalClient(TCPClient client)
        {
            int length = clients.Count;
            for (int i = 0; i < length; i++)
            {
                TCPClient c = clients[i];
                if (c != null)
                {
                    if (c.serverInfo.peerId.Equals(client.serverInfo.peerId))
                    {
                        return;
                    }
                }
            }
            clients.Add(client);
        }

        /// <summary>
        /// 当与远程Server断开后，移除本地对应Client
        /// </summary>
        public void RemoveLocalClient(TCPClient client)
        {
            int length = clients.Count;
            for (int i = 0; i < length; i++)
            {
                TCPClient c = clients[i];
                if (c != null)
                {
                    if (c.serverInfo.peerId.Equals(client.serverInfo.peerId))
                    {
                        clients.Remove(client);
                        client.Destroy();
                        client = null;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 有人连入房间时触发OnSomeoneJoined事件
        /// </summary>
        public void TriggerJoinedEvent(string peerId)
        {
            MainThread.Run(() =>
            {
                //房间状态下触发
                if (P2PNetwork.Instance.SomeoneJoinedRoomEvent != null)
                {
                    P2PNetwork.Instance.SomeoneJoinedRoomEvent(peerId);
                }
                int count = P2PNetwork.Instance.networkObjectsInScene.Count;
                for (int j = 0; j < count; j++)
                {
                    P2PNetwork.Instance.networkObjectsInScene[j].SomeoneJoinedRoom(peerId);
                }
            });
            hadTriggerJoinedEvent = true;
        }

        /// <summary>
        /// 有玩家从房间断开时触发OnSomeoneLeaved事件
        /// </summary>
        public void ResetRoomMaster(string peerId)
        {
            if (isMaster)
            {
                //房主处理：如果是房主候选人掉线，则重新选定候选人
                if (masterCandidatePeerId.Equals(peerId))
                {
                    masterCandidatePeerId = null;
                    int length = tcpServer.sessions.Count;
                    for (int i = 0; i < length; i++)
                    {
                        TCPServer.Session session = tcpServer.sessions[i];
                        if (session != null)
                        {
                            if (!session.sessionInfo.peerId.Equals(P2PNetwork.localPeerId))
                            {
                                masterCandidatePeerId = session.sessionInfo.peerId;
                                MasterBroadcastCandidatePeerId(null);
                                //重新设置本地用于断线重连的目标房间信息
                                roomInfoForReJoin.localIP = session.sessionInfo.ip;
                                roomInfoForReJoin.port = session.sessionInfo.port;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                //非房主处理：判断是否自己接任新房主
                //P2PDebug.LogError(peerId.Equals(masterPeerId) + " | " + P2PNetwork.localPeerId.Equals(masterCandidatePeerId));
                if (peerId.Equals(masterPeerId))
                {
                    if (P2PNetwork.localPeerId.Equals(masterCandidatePeerId))
                    {
                        //P2PDebug.LogError("BecomeNewMaster");
                        BecomeNewMaster();
                    }
                }
            }
        }

        private void BroadcastToLobby()
        {
            SynDataBase baseData = new SynDataBase(P2PMsgType.RoomInfoToLobby, null, null, null, null);
            string baseJson = P2PUtility.ObjectToJson(baseData);
            string roomInfoJson = P2PUtility.ObjectToJson(info);
            //接收群组加到尾部
            LobbyReceiveTarget lobbyReceiveTarget = new LobbyReceiveTarget();
            lobbyReceiveTarget.receiveTarget = (int)ReceiveTarget.Other;
            string receiveTargetStr = MsgDivid.LobbyReceiveTarget + P2PUtility.ObjectToJson(lobbyReceiveTarget);
            roomInfoBytes = P2PUtility.JsonToBytes(baseJson + MsgDivid.BaseData + roomInfoJson + receiveTargetStr);
            //修改后立即第一时间发一条
            if (broadcasterToLobby != null)
            {
                broadcasterToLobby.Send(roomInfoBytes);
            }
        }

        public void Update()
        {
            if (tcpServer != null)
            {
                tcpServer.Update();
            }
            //P2PDebug.Log(canBroadcast + " | " + isMaster);
            if (canBroadcast)
            {
                if (isMaster)
                {
                    broadcastTimer -= Time.deltaTime;
                    if (broadcastTimer <= 0f)
                    {
                        broadcastTimer = broadcastInterval;
                        //P2PDebug.LogError("房间广播:" + P2PUtility.BytesToJson(roomInfoBytes));
                        broadcasterToLobby.Send(roomInfoBytes);
                    }
                }
            }
            int length = clients.Count;
            for (int i = 0; i < length; i++)
            {
                TCPClient client = clients[i];
                if (client != null)
                {
                    client.Update();
                }
            }
        }

        /// <summary>
        /// 更新房间信息，并在网络上同步
        /// </summary>
        public void ShareRoomInfo()
        {
            SynDataBase baseInfo = new SynDataBase(P2PMsgType.RoomInfoToRoom, null, null, null, null);
            SendMsg(ReceiveTarget.All, null, baseInfo, info, Channel.TCP);
        }
        /// <summary>
        /// 系统网络层调用，接收到房间自定义属性的更改
        /// </summary>
        public void ReceiveRoomInfo(RoomInfo roomInfo)
        {
            roomInfo.localIP = P2PNetwork.localIP;
            roomInfo.port = P2PNetwork.localPort;
            this.info = roomInfo;
            BroadcastToLobby();
        }

        /// <summary>
        /// 创建房间时传入的定义属性
        /// </summary>
        public string CustomProperty
        {
            get
            {
                return info.customProperty;
            }
        }

        /// <summary>
        /// 锁定房间，用于进入战斗之后将房间隔绝，不再对大厅广播信息
        /// </summary>
        public void LockRoom()
        {
            SendLockMsg();
        }

        private void SendLockMsg()
        {
            info.isOpen = false;
            ShareRoomInfo();
            BroadcastToLobby();
            canBroadcast = false;
            broadcasterToLobby.Stop();
        }

        /// <summary>
        /// 告诉对方正常退出，否则对方认为异常退出
        /// </summary>
        void BroadcastLeaveRoom()
        {
            P2PDebug.Log("BroadcastLeaveRoom");
            SynDataBase baseInfo = new SynDataBase(P2PMsgType.LeaveRoom, null, null, P2PNetwork.localPeerId, null);
            SendMsg(ReceiveTarget.Other, null, baseInfo, null, Channel.TCP);
        }

        /// <summary>
        /// 离开房间（可能主动离开也可能异常离开）
        /// </summary>
        public void Leave(bool canSendEvent, bool isException)
        {
            hadTriggerJoinedEvent = false;
            if (tcpServer != null)
            {
                isExceptionLeave = isException;
                if (canSendEvent && !isExceptionLeave)
                {
                    BroadcastLeaveRoom();
                }
                if (udpReceiver != null)
                {
                    udpReceiver.Stop();
                    udpReceiver = null;
                }
                int length = clients.Count;
                if (length == 1)
                {
                    //当我是房间里最后一个人时，立即关闭房间的广播
                    SendLockMsg();
                }
                canBroadcast = false;
                if (broadcasterToLobby != null)
                {
                    broadcasterToLobby.Stop();
                    broadcasterToLobby = null;
                }
                for (int i = 0; i < length; i++)
                {
                    TCPClient client = clients[i];
                    if (client != null)
                    {
                        client.Destroy();
                    }
                }
                clients = null;
                tcpServer.Stop();
                tcpServer = null;
                if (P2PNetwork.haveInstance)
                {
                    P2PNetwork.Instance.currState = State.Offline;
                    if (canSendEvent && hadTriggerJoinedEvent)
                    {
                        if (P2PNetwork.Instance.SomeoneLeavedRoomEvent != null)
                        {
                            P2PNetwork.Instance.SomeoneLeavedRoomEvent(P2PNetwork.localPeerId, isException);
                        }
                        for (int i = 0; i < P2PNetwork.Instance.networkObjectsInScene.Count; i++)
                        {
                            P2PNetwork.Instance.networkObjectsInScene[i].SomeoneLeavedRoom(P2PNetwork.localPeerId, isException);
                        }
                    }
                }
                P2PDebug.Log("已退出房间");
            }
        }
    }
}