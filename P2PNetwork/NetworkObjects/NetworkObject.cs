using UnityEngine;

namespace BToolkit.P2PNetwork
{
    [AddComponentMenu("P2PNetwork/NetworkObject")]
    public sealed class NetworkObject:MonoBehaviour
    {
        public delegate void InstantiateDelegate(string msg);
        /// <summary>
        /// 所属创建者客户端的PeerID(动态创建的网络对象等于创建者客户端的peerId，非动态创建的网络对象全部等于本地peerId)
        /// </summary>
        public string peerId { get { return _peerId; } }
        private string _peerId = "";
        /// <summary>
        /// 物体的唯一网络ID(动态创建的对象由系统赋值)
        /// </summary>
        public string netId { get { return _netId; } }
        private string _netId = "";
        /// <summary>
        /// 返回true，则表示当前物体在任何客户端上都有操作权限。在Awake之后被初始化
        /// </summary>
        public bool isPublic { get { return _isPublic; } }
        private bool _isPublic = true;//默认必须是true
        /// <summary>
        /// 对于动态创建的网络对象。返回true，则表示当前物体在创建者的客户端上，反之在其他客户端上。对于非动态创建的对象，则全部返回true。
        /// </summary>
        public bool isLocal { get { return P2PNetwork.localPeerId.Equals(peerId); } }
        private InstantiateInfo instantiateInfo;
        public delegate GameObject GetNetworkObjectPrefabDelegate(string prefabPath);
        public static GetNetworkObjectPrefabDelegate GetNetworkObjectPrefabMethod = (string prefabPath) =>
        {
            return Resources.Load<GameObject>(prefabPath);
        };
        private bool isRoomObject;

        void OnDestroy()
        {
            if(P2PNetwork.haveInstance)
            {
                int length = P2PNetwork.Instance.networkObjectsInScene.Count;
                for(int i = 0;i < length;i++)
                {
                    if(P2PNetwork.Instance.networkObjectsInScene[i].netId.Equals(netId))
                    {
                        P2PNetwork.Instance.networkObjectsInScene.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        void Awake()
        {
            _peerId = P2PNetwork.localPeerId;
            string pathToName = gameObject.name;
            Transform parent = transform.parent;
            while(parent)
            {
                pathToName += parent.name;
                parent = parent.parent;
            }
            _netId = P2PUtility.GetMD5_32(pathToName,false);
            int length = P2PNetwork.Instance.networkObjectsInScene.Count;
            bool contains = false;
            for(int i = 0;i < length;i++)
            {
                if(P2PNetwork.Instance.networkObjectsInScene[i] == this)
                {
                    contains = true;
                    break;
                }
            }
            if(!contains)
            {
                P2PNetwork.Instance.networkObjectsInScene.Add(this);
            }
            isRoomObject = (P2PNetwork.Instance.currState == State.InRoom);
        }

        /// <summary>
        /// 仅用于非动态创建的对象指定所属PeeId
        /// </summary>
        public void SetPeerId(string peerId)
        {
            this._peerId = peerId;
        }

        /// <summary>
        /// 创建角色的同时发送创建消息，该方法由系统调用，同时判断并触发远程Peer创建镜像
        /// </summary>
        public void SendInstantiateInfo(string prefabPath,Vector3 position,Quaternion rotation,string netId,bool isPublic,string msg,bool sendToSelf)
        {
            SynDataBase baseData = new SynDataBase(P2PMsgType.InstantiateInfo,typeof(InstantiateInfo).Name,null,P2PNetwork.localPeerId,netId);
            InstantiateInfo instantiateInfo = new InstantiateInfo();
            instantiateInfo.prefabPath = prefabPath;
            instantiateInfo.position = position;
            instantiateInfo.rotation = rotation;
            instantiateInfo.isPublic = isPublic;
            instantiateInfo.msg = msg;
            if(P2PNetwork.Instance.currState == State.InLobby)
            {
                P2PNetwork.lobby.Broadcast(ReceiveTarget.Other,null,baseData,instantiateInfo);//UDP广播
            }
            else if(P2PNetwork.Instance.currState == State.InRoom)
            {
                P2PNetwork.room.SendMsg(ReceiveTarget.Other,null,baseData,instantiateInfo,Channel.TCP);//TCP传输
            }
            if(sendToSelf)
            {
                ReceiveInstantiateInfo(baseData,instantiateInfo);//本地直接调用
            }
        }

        /// <summary>
        /// 接收SendInstantiateInfo()方法发送的消息，在Awake之后Start之前调用。当本地找不到相应NetId的对象时新创建的镜像才会调用
        /// </summary>
        public void ReceiveInstantiateInfo(SynDataBase baseData,InstantiateInfo instantiateInfo)
        {
            this.instantiateInfo = instantiateInfo;
            int length = P2PNetwork.Instance.networkObjectsInScene.Count;
            for(int i = 0;i < length;i++)
            {
                if(P2PNetwork.Instance.networkObjectsInScene[i].netId.Equals(netId))
                {
                    P2PNetwork.Instance.networkObjectsInScene.RemoveAt(i);
                    break;
                }
            }
            _peerId = baseData.peerId;
            _netId = baseData.netId;
            length = P2PNetwork.Instance.networkObjectsInScene.Count;
            bool contains = false;
            for(int i = 0;i < length;i++)
            {
                if(P2PNetwork.Instance.networkObjectsInScene[i] == this)
                {
                    contains = true;
                    break;
                }
            }
            if(!contains)
            {
                P2PNetwork.Instance.networkObjectsInScene.Add(this);
            }
            _isPublic = instantiateInfo.isPublic;
            if(instantiateInfo.msg == null)
            {
                instantiateInfo.msg = "";
            }
            SendMessage("OnInstantiated",instantiateInfo.msg,SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 有人加入大厅时触发
        /// </summary>
        public void SomeoneJoinedLobby(string peerId)
        {
            if(!isRoomObject)
            {
                if(isLocal)
                {
                    SendInstantiateInfo(instantiateInfo.prefabPath,transform.position,transform.rotation,netId,isPublic,instantiateInfo.msg,false);
                }
            }
            SendMessage("OnSomeoneJoinedLobby",peerId,SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 有人加入大厅时触发
        /// </summary>
        public void SomeoneLeavedLobby(string peerId,bool isException)
        {
            string mParams = peerId + "|" + isException;
            SendMessage("_OnSomeoneLeavedLobby",mParams,SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 有人加入房间时触发
        /// </summary>
        public void SomeoneJoinedRoom(string peerId)
        {
            if(isRoomObject)
            {
                if(isLocal)
                {
                    SendInstantiateInfo(instantiateInfo.prefabPath,transform.position,transform.rotation,netId,isPublic,instantiateInfo.msg,false);
                }
            }
            SendMessage("OnSomeoneJoinedRoom",peerId,SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 有人离开房间时触发
        /// </summary>
        public void SomeoneLeavedRoom(string peerId,bool isException)
        {
            string mParams = peerId + "|" + isException;
            SendMessage("_OnSomeoneLeavedRoom",mParams,SendMessageOptions.DontRequireReceiver);
        }
    }
}
