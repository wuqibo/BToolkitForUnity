using UnityEngine;

namespace BToolkit.P2PNetwork
{
    public class P2PMsgType
    {
        /// <summary>
        /// TCPClient首次连接时像服务器发送双向连接信息
        /// </summary>
        public const short TCPClientOrginalInfo = 0;
        /// <summary>
        /// 有人加入大厅或房间
        /// </summary>
        public const short SomeoneJoined = 1;
        /// <summary>
        /// 向大厅里广播房间参数，表示有人创建了房间，仅在大厅状态下收到
        /// </summary>
        public const short RoomInfoToLobby = 2;
        /// <summary>
        /// 创建房间之后更新房间自定义信息
        /// </summary>
        public const short RoomInfoToRoom = 3;
        /// <summary>
        /// 房主广播当前房间内其他玩家信息
        /// </summary>
        public const short OtherPeersInRoom = 4;
        /// <summary>
        /// 房主候选人
        /// </summary>
        public const short MasterCandidate = 5;
        /// <summary>
        /// 创建网络对象附带的创建属性
        /// </summary>
        public const short InstantiateInfo = 6;
        /// <summary>
        /// 继承NetworkBehaviour类之后所使用的通用消息收到机制，支持指定接收函数
        /// </summary>
        public const short NetworkBehaviourMsg = 7;
        /// <summary>
        /// 发送心跳，已确保知道异常断开
        /// </summary>
        public const short Heartbeat = 8;
        /// <summary>
        /// 主动离开房间时，告知对方
        /// </summary>
        public const short LeaveRoom = 9;
    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public enum ConnState
    {
        NoNetwork,
        TimeOut,
        Failed,
        Success
    }
    /// <summary>
    /// 当前网络所处的阶段
    /// </summary>
    public enum State
    {
        Offline,
        InLobby,
        InRoom
    }
    /// <summary>
    /// 消息接收群组,All则包括自己的全部端都收到,Other则除自己外的所有端
    /// </summary>
    public enum ReceiveTarget
    {
        All,
        Other,
        Creater,
        RoomMaster
    }
    /// <summary>
    /// 当前网络所处的阶段
    /// </summary>
    public enum Channel
    {
        UDP,
        TCP
    }

    public class MsgDivid {
        public const string MsgEOF = "<!--MsgEOF-->";
        public const string BaseData = "<!--BaseData-->";
        public const string LobbyReceiveTarget = "<!--LobbyReceiveTarget->";
        public const string IsInt = "<!--IsInt->";
        public const string IsBool = "<!--IsBool->";
        public const string IsFloat = "<!--IsFloat->";
        public const string IsString = "<!--IsString->";
    }

    /// <summary>
    /// 基础数据包结构
    /// </summary>
    public struct SynDataBase {
        public short msgType;
        public string dataType, receiveFun, peerId, netId;
        public SynDataBase(short msgType,string dataType,string receiveFun,string peerId,string netId) {
            this.msgType = msgType;
            this.dataType = dataType;
            this.receiveFun = receiveFun;
            this.peerId = peerId;
            this.netId = netId;
        }
    }

    /// <summary>
    /// 房间信息基础数据包结构
    /// </summary>
    public struct RoomInfo {
        public string roomId;
        public string peerId;
        public string masterPeerId;
        public string localIP;
        public int port;
        public int maxNum;
        public int currNum;
        public string customProperty;
        public bool isOpen;
    }

    /// <summary>
    /// 房主广播当前房间内所有玩家信息
    /// </summary>
    public struct PeersInRoom {
        public string[] peerIds;
        public string[] ips;
        public int[] ports;
    }

    /// <summary>
    /// 房主及房主候选人
    /// </summary>
    public struct MasterInfo {
        public string masterPeerId;
        public string candidatePeerId;
    }

    /// <summary>
    /// Peer信息，用于Peer建立双向连接
    /// </summary>
    public struct PeerInfo {
        public string peerId;
        public string ip;
        public int port;
    }

    /// <summary>
    /// 创建NetObject时所用的数据包结构
    /// </summary>
    public struct InstantiateInfo {
        public string prefabPath, msg;
        public bool isPublic;
        public Vector3 position;
        public Quaternion rotation;
    }

    /// <summary>
    /// UDP广播附带的消息，定义接收对象
    /// </summary>
    public struct LobbyReceiveTarget {
        public int receiveTarget;
        public string receivePeerId;
    }
}
