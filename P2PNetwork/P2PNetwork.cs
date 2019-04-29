using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit.P2PNetwork
{
    public delegate void ConnResultEvent(ConnState state);
    public delegate void ReceiveMsgEvent(short msgType, object data);
    public delegate void RoomListUpdateEvent(RoomInfo[] roomsInfo);
    public delegate void PeerConnEvent(string peerId);
    public delegate void PeerDisconnEvent(string peerId, bool isException);

    public class P2PNetwork : MonoBehaviour
    {
        /// <summary>
        /// 用于在OnDestroy和OnDisable中引用P2PNetwork.Instance时判断，如果haveInstance返回false，则不能再继续调用P2PNetwork.Instance。
        /// </summary>
        public static bool haveInstance { get { return instance; } }
        private static P2PNetwork instance;
        private static bool hadCreated;
        public static P2PNetwork Instance
        {
            get
            {
                if (!instance && !hadCreated)
                {
                    instance = GameObject.FindObjectOfType<P2PNetwork>();
                    if (!instance)
                    {
                        GameObject go = new GameObject("P2PNetwork");
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<P2PNetwork>();
                    }
                    hadCreated = true;
                }
                return instance;
            }
        }
        public PeerConnEvent SomeoneJoinedLobbyEvent, SomeoneJoinedRoomEvent;
        public PeerDisconnEvent SomeoneLeavedLobbyEvent, SomeoneLeavedRoomEvent;
        public RoomListUpdateEvent RoomListUpdateEvent;
        public Action BecomeRoomMasterEvent;
        public State currState = State.Offline;
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsConnected { get { return currState != State.Offline; } }
        /// <summary>
        /// 场景中所有的NetworkObject对象
        /// </summary>
        public List<NetworkObject> networkObjectsInScene = new List<NetworkObject>();
        /// <summary>
        /// 所有注册过的共享数据类列表
        /// </summary>
        public Dictionary<string, Type> synDataTypes = new Dictionary<string, Type>();
        public static string localPeerId;
        public static string localIP;
        public static int localPort;//房间被创建之后可能有变化
        public static Lobby lobby;
        public static Room room;
        public static bool isNetworkActive;

        #region Unity Method
        void OnDestroy()
        {
            Destroy();
        }

        void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = true;
            P2PDebug.canDebug = P2PConfig.Instance.canDebug;
            P2PUtility.Init();
            if (P2PUtility.isEditor)
            {
                localPeerId = P2PUtility.deviceId + "_" + UnityEngine.Random.Range(10, 100);
            }
            else
            {
                localPeerId = P2PUtility.deviceId;
            }
            localIP = P2PUtility.internalIP;
            localPort = P2PConfig.Instance.Port;
            isNetworkActive = true;
            RegisterSynDataType(typeof(Vector2));
            RegisterSynDataType(typeof(Vector3));
            RegisterSynDataType(typeof(Quaternion));
            RegisterSynDataType(typeof(SynDataBase));
            RegisterSynDataType(typeof(RoomInfo));
            RegisterSynDataType(typeof(PeerInfo));
            RegisterSynDataType(typeof(InstantiateInfo));
        }

        void Update()
        {
            MainThread.Update();
            if (IsConnected)
            {
                if (lobby != null)
                {
                    lobby.Update();
                }
                if (room != null)
                {
                    room.Update();
                }
            }
        }

        void OnGUI()
        {
            if (P2PConfig.Instance.showGUIInfo)
            {
                if (room != null)
                {
                    GUI.color = Color.white;
                    GUILayout.Label("本机:" + localPeerId);
                    GUILayout.Label("本机:" + room.info.localIP + ":" + room.info.port);
                    GUI.color = Color.green;
                    int length = room.tcpServer.sessions.Count;
                    for (int i = 0; i < length; i++)
                    {
                        TCPServer.Session session = room.tcpServer.sessions[i];
                        if (session != null)
                        {
                            GUILayout.Label("Session " + (i + 1) + ":" + session.sessionInfo.peerId);
                        }
                    }
                    GUI.color = Color.red;
                    length = room.clients.Count;
                    for (int i = 0; i < length; i++)
                    {
                        TCPClient client = room.clients[i];
                        if (client != null)
                        {
                            GUILayout.Label("Client " + (i + 1) + ":" + client.serverInfo.peerId);
                        }
                    }
                    GUI.color = Color.blue;
                    GUILayout.Label("房主:" + room.masterPeerId);
                    GUILayout.Label("备选房主:" + room.masterCandidatePeerId);
                    GUI.color = Color.yellow;
                    GUILayout.Label("当前在线人数:" + room.info.currNum);
                }
            }
        }

        /// <summary>
        /// APP从后台切换回来时
        /// </summary>
        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                P2PUtility.Init();
            }
        }
        #endregion

        #region P2PNetwork Register
        /// <summary>
        /// 在最早时刻注册数据共享类，否则不能传输
        /// </summary>
        public static void RegisterSynDataType(Type type)
        {
            string typeName = type.Name;
            if (!Instance.synDataTypes.ContainsKey(typeName))
            {
                Instance.synDataTypes.Add(typeName, type);
            }
        }
        /// <summary>
        /// 注册事件，接收大厅下的P2PNetwork.P2PNetwork.Instance.SendLobbyMsg()和房间下的P2PNetwork.P2PNetwork.Instance.SendRoomMsg(),传false则注销指定回调,参数1传null加参数2传false则清除所有已注册过的回调
        /// </summary>
        public static void RegisterCustomMsgEvent(ReceiveMsgEvent OnReceiveCustomMsg, bool regist = true)
        {
            if (regist)
            {
                MessageManager.Instance.OnReceiveCustomMsg += OnReceiveCustomMsg;
            }
            else
            {
                if (OnReceiveCustomMsg != null)
                {
                    MessageManager.Instance.OnReceiveCustomMsg -= OnReceiveCustomMsg;
                }
                else if (MessageManager.haveInstance)
                {
                    MessageManager.Instance.OnReceiveCustomMsg = null;
                }
            }
        }
        /// <summary>
        /// 注册事件，有人加入大厅则触发,传false则注销指定回调,参数1传null加参数2传false则清除所有已注册过的回调
        /// </summary>
        public static void RegisterSomeoneJoinedLobbyEvent(PeerConnEvent OnSomeoneJoinedLobby, bool regist = true)
        {
            if (regist)
            {
                Instance.SomeoneJoinedLobbyEvent += OnSomeoneJoinedLobby;
            }
            else
            {
                if (instance)
                {
                    if (OnSomeoneJoinedLobby != null)
                    {
                        instance.SomeoneJoinedLobbyEvent -= OnSomeoneJoinedLobby;
                    }
                    else
                    {
                        instance.SomeoneJoinedLobbyEvent = null;
                    }
                }
            }
        }
        /// <summary>
        /// 注册事件，有人离开大厅则触发,传false则注销指定回调,参数1传null加参数2传false则清除所有已注册过的回调
        /// </summary>
        public static void RegisterSomeoneLeavedLobbyEvent(PeerDisconnEvent OnSomeoneLeavedLobby, bool regist = true)
        {
            if (regist)
            {
                Instance.SomeoneLeavedLobbyEvent += OnSomeoneLeavedLobby;
            }
            else
            {
                if (instance)
                {
                    if (OnSomeoneLeavedLobby != null)
                    {
                        instance.SomeoneLeavedLobbyEvent -= OnSomeoneLeavedLobby;
                    }
                    else
                    {
                        instance.SomeoneLeavedLobbyEvent = null;
                    }
                }
            }
        }
        /// <summary>
        /// 注册事件，有人加入房间则触发,传false则注销指定回调,参数1传null加参数2传false则清除所有已注册过的回调
        /// </summary>
        public static void RegisterSomeoneJoinedRoomEvent(PeerConnEvent OnSomeoneJoinedRoom, bool regist = true)
        {
            if (regist)
            {
                Instance.SomeoneJoinedRoomEvent += OnSomeoneJoinedRoom;
            }
            else
            {
                if (instance)
                {
                    if (OnSomeoneJoinedRoom != null)
                    {
                        instance.SomeoneJoinedRoomEvent -= OnSomeoneJoinedRoom;
                    }
                    else
                    {
                        instance.SomeoneJoinedRoomEvent = null;
                    }
                }
            }
        }
        /// <summary>
        /// 注册事件，有人离开房间则触发,传false则注销指定回调,参数1传null加参数2传false则清除所有已注册过的回调
        /// </summary>
        public static void RegisterSomeoneLeavedRoomEvent(PeerDisconnEvent OnSomeoneLeavedRoom, bool regist = true)
        {
            if (regist)
            {
                Instance.SomeoneLeavedRoomEvent += OnSomeoneLeavedRoom;
            }
            else
            {
                if (instance)
                {
                    if (OnSomeoneLeavedRoom != null)
                    {
                        instance.SomeoneLeavedRoomEvent -= OnSomeoneLeavedRoom;
                    }
                    else
                    {
                        instance.SomeoneLeavedRoomEvent = null;
                    }
                }
            }
        }
        /// <summary>
        /// 注册事件，由非房主变成房主的时刻触发,传false则注销指定回调,参数1传null加参数2传false则清除所有已注册过的回调
        /// </summary>
        public static void RegisterBecomeRoomMasterEvent(Action OnBecomeRoomMaster, bool regist = true)
        {
            if (regist)
            {
                Instance.BecomeRoomMasterEvent += OnBecomeRoomMaster;
            }
            else
            {
                if (instance)
                {
                    if (OnBecomeRoomMaster != null)
                    {
                        instance.BecomeRoomMasterEvent -= OnBecomeRoomMaster;
                    }
                    else
                    {
                        instance.BecomeRoomMasterEvent = null;
                    }
                }
            }
        }
        /// <summary>
        /// 注册事件，大厅监听房间列表更新,传false则注销指定回调,参数1传null加参数2传false则清除所有已注册过的回调
        /// </summary>
        public static void RegisterRoomListUpdateEvent(RoomListUpdateEvent OnRoomListUpdate, bool regist = true)
        {
            if (regist)
            {
                Instance.RoomListUpdateEvent += OnRoomListUpdate;
            }
            else
            {
                if (instance)
                {
                    if (OnRoomListUpdate != null)
                    {
                        instance.RoomListUpdateEvent -= OnRoomListUpdate;
                    }
                    else
                    {
                        instance.RoomListUpdateEvent = null;
                    }
                }
            }
        }
        #endregion

        #region P2PNetwork Command
        /// <summary>
        /// 房间发送消息,structData传空时可以接收到SynDataBase。（TCP传输,对应P2PNetwork.P2PNetwork.Instance.RegisterReceiveEvent()注册接收方法）
        /// <para>receiveTarget：指定接收的群组</para>
        /// <para>msgType：消息类型(小于100为系统参数，自定义请使用大于100的值)</para>
        /// <para>data：消息数据</para>
        /// </summary>
        public void SendRoomMsg(ReceiveTarget receiveTarget, short msgType, object customData = null, Channel channel = Channel.TCP)
        {
            if (room != null)
            {
                string dataTypeName = (customData == null) ? "" : customData.GetType().Name;
                SynDataBase baseData = new SynDataBase(msgType, dataTypeName, null, localPeerId, null);
                room.SendMsg(receiveTarget, null, baseData, customData, channel);
            }
            else
            {
                P2PDebug.LogWarning("未加入房间不能发送房间消息");
            }
        }
        /// <summary>
        /// 房间发送消息,structData传空时可以接收到SynDataBase。（TCP传输,对应P2PNetwork.P2PNetwork.Instance.RegisterReceiveEvent()注册接收方法）
        /// <para>receiveTarget：指定接收的群组</para>
        /// <para>msgType：消息类型(小于100为系统参数，自定义请使用大于100的值)</para>
        /// <para>data：消息数据</para>
        /// </summary>
        public void SendRoomMsg(string receivePeerId, short msgType, object customData, Channel channel = Channel.TCP)
        {
            if (room != null)
            {
                string dataTypeName = (customData == null) ? "" : customData.GetType().Name;
                SynDataBase baseData = new SynDataBase(msgType, dataTypeName, null, localPeerId, null);
                room.SendMsg(ReceiveTarget.All, receivePeerId, baseData, customData, channel);
            }
            else
            {
                P2PDebug.LogWarning("未加入房间不能发送房间消息");
            }
        }

        /// <summary>
        /// 进入大厅，将自动退出房间
        /// </summary>
        public void JoinLobby(ConnResultEvent Callback)
        {
            if (room != null)
            {
                room.Leave(true, false);
                room = null;
            }
            lobby = new Lobby();
            lobby.Join(Callback);
        }

        /// <summary>
        /// 离开大厅，相当于断网状态
        /// </summary>
        public void LeaveLobby()
        {
            lobby.Leave();
            lobby = null;
        }

        /// <summary>
        /// 创建房间，将自动退出大厅
        /// </summary>
        public void CreateRoom(string customProperty, int maxNum, ConnResultEvent Callback)
        {
            if (lobby != null)
            {
                lobby.Leave();
                lobby = null;
            }
            if (room == null)
            {
                room = Room.Create(customProperty, maxNum);
                if (room != null)
                {
                    room.JoinCallback = Callback;
                    P2PDebug.Log("创建房间成功");
                    //本地客户端优先连入本地服务器
                    TCPClient.ConnectToServer(localPeerId, localIP, localPort);
                }
                else
                {
                    Callback(ConnState.Failed);
                    P2PDebug.Log("创建房间失败");
                }
            }
            else
            {
                P2PDebug.LogError("原房间没有销毁，不能创建房间");
            }
        }

        /// <summary>
        /// 加入房间，将自动退出大厅
        /// </summary>
        public void JoinRoom(RoomInfo roomInfo, ConnResultEvent Callback)
        {
            if (lobby != null)
            {
                lobby.Leave();
                lobby = null;
            }
            if (room == null)
            {
                room = Room.Join(roomInfo, Callback);
                if (room != null)
                {
                    if (localIP.Equals(roomInfo.localIP) && localPort == roomInfo.port)
                    {
                        P2PDebug.LogError("本地IP和远程IP一样，且端口也一样，无法创建房间连接！");
                        return;
                    }
                    //本地客户端优先连入本地服务器
                    P2PDebug.Log("加入本地服务器：" + localIP + ":" + localPort);
                    TCPClient.ConnectToServer(localPeerId, localIP, localPort);
                    //本地客户端开始连接指定房间
                    P2PDebug.Log("加入远程服务器：" + roomInfo.localIP + ":" + +roomInfo.port);
                    TCPClient.ConnectToServer(roomInfo.peerId, roomInfo.localIP, roomInfo.port);
                }
                else
                {
                    P2PDebug.LogWarning("加入房间失败");
                    Callback(ConnState.Failed);
                }
            }
            else
            {
                P2PDebug.LogError("原房间没有销毁，不能创建房间");
            }
        }

        /// <summary>
        /// 加入原来的房间，用于断线重连
        /// </summary>
        public void ReJoinRoom(ConnResultEvent Callback)
        {
            JoinRoom(Room.roomInfoForReJoin, Callback);
        }

        /// <summary>
        /// 锁定房间，用于进入战斗之后将房间隔绝，不再对大厅广播信息
        /// </summary>
        public void LockRoom()
        {
            if (room != null)
            {
                room.LockRoom();
            }
        }

        /// <summary>
        /// 开始或停止心跳监听，可在场景跳转前停止，确定双方都进入新场景后重新开始监听
        /// </summary>
        public void SetListenHeartbeatRun(bool b)
        {
            TCPServer.canListenHeartbeat = b;
        }

        /// <summary>
        /// 离开房间，将自动回到大厅
        /// </summary>
        public void LeaveRoom()
        {
            if (room != null)
            {
                room.Leave(true, false);
                room = null;
            }
            lobby = new Lobby();
            lobby.Join(null);
        }
        #endregion

        #region InstantiateNetObject
        /// <summary>
        /// 在调用InstantiateNetworkObject()方法创建网络对象时，默认从Resources目录获取，若改成从AssetBundle获取可使用此方法实现
        /// </summary>
        public static void SetPrefabPathToPrefabMethod(NetworkObject.GetNetworkObjectPrefabDelegate Method)
        {
            NetworkObject.GetNetworkObjectPrefabMethod = Method;
        }

        /// <summary>
        /// 创建网络对象，远程Peer端将同步创建
        /// <para>isPublic: true则任何客户端都有操作权限，false则只有创建者有操作权限</para>
        /// </summary>
        public GameObject InstantiateNetworkObject(string prefabPath, bool isPublic = false, string msg = null)
        {
            return InstantiateNetworkObject(prefabPath, new Vector3(float.MinValue, float.MinValue, float.MinValue), Quaternion.identity, isPublic, msg);
        }

        /// <summary>
        /// 创建网络对象，远程Peer端将同步创建。
        /// <para>isPublic: true则任何客户端都有操作权限，false则只有创建者有操作权限</para>
        /// </summary>
        public GameObject InstantiateNetworkObject(string prefabPath, Vector3 position, Quaternion rotation, bool isPublic = false, string msg = null)
        {
            //创建本地对象，镜像对象在MessageManager.cs行171处创建
            GameObject prefab = NetworkObject.GetNetworkObjectPrefabMethod(prefabPath);
            if (prefab)
            {
                if (position == new Vector3(float.MinValue, float.MinValue, float.MinValue))
                {
                    position = prefab.transform.position;
                }
                GameObject go = Instantiate(prefab, position, rotation) as GameObject;
                string[] peerIdArr = localPeerId.Split('_');
                go.name = go.name + "_" + peerIdArr[1];
                NetworkObject networkObject = go.GetComponent<NetworkObject>();
                if (!networkObject)
                {
                    networkObject = go.AddComponent<NetworkObject>();
                }
                string newNetId = P2PUtility.GetMD5_32(P2PUtility.deviceId + DateTime.Now.ToString("HHmmssfff"), true);
                networkObject.SendInstantiateInfo(prefabPath, position, rotation, newNetId, isPublic, msg, true);
                return go;
            }
            else
            {
                P2PDebug.LogError("创建本地 Net Object 失败：无法找到路径为 " + prefabPath + " 的 Prafab");
                return null;
            }
        }
        #endregion

        #region Destroy
        /// <summary>
        /// 主动断开网络,并销毁所有连接对象
        /// </summary>
        public void Destroy()
        {
            isNetworkActive = false;
            synDataTypes.Clear();
            synDataTypes = null;
            if (room != null)
            {
                room.Leave(true, false);
                room = null;
            }
            if (lobby != null)
            {
                lobby.Leave();
                lobby = null;
            }
            instance = null;
        }
        #endregion
    }
}