using System;
using UnityEngine;

namespace BToolkit.P2PNetwork
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetworkBehaviour:MonoBehaviour
    {

        private NetworkObject _networkObject;
        public NetworkObject networkObject { get { return _networkObject ?? (_networkObject = GetComponent<NetworkObject>()); } }
        #region 由NetworkObject发送消息
        private void _OnSomeoneLeavedLobby(string _params)
        {
            string[] paramArr = _params.Split('|');
            OnSomeoneLeavedLobby(paramArr[0],bool.Parse(paramArr[1]));
        }
        private void _OnSomeoneLeavedRoom(string _params)
        {
            string[] paramArr = _params.Split('|');
            OnSomeoneLeavedRoom(paramArr[0],bool.Parse(paramArr[1]));
        }
        #endregion
        /// 对象被创建时在网络参数初始化完成后调用（Awake之后，Start之前）
        protected abstract void OnInstantiated(string msg);
        /// 有人加入大厅时触发
        protected abstract void OnSomeoneJoinedLobby(string peerId);
        /// 有人离开大厅时触发
        protected abstract void OnSomeoneLeavedLobby(string peerId,bool isException);
        /// 有人加入房间时触发
        protected abstract void OnSomeoneJoinedRoom(string peerId);
        /// 有人离开房间时触发
        protected abstract void OnSomeoneLeavedRoom(string peerId,bool isException);

        /// <summary>
        /// 发送消息给多个客户端(无自定义数据，用于仅调用某个方法)
        /// <para>receiveTarget：目标对象群</para>
        /// <para>ReceiveFunction：接收的函数</para>
        /// </summary>
        public void Send(ReceiveTarget receiveTarget,Action ReceiveFunction,Channel channel = Channel.TCP)
        {
            if(string.IsNullOrEmpty(networkObject.netId))
            {
                P2PDebug.LogError("非动态创建的网络对象必须手动指定唯一的NetId");
                return;
            }
            SynDataBase baseData = new SynDataBase(P2PMsgType.NetworkBehaviourMsg,"",ReceiveFunction.Method.Name,networkObject.peerId,networkObject.netId);
            if(P2PNetwork.Instance.currState == State.InLobby)
            {
                if(receiveTarget == ReceiveTarget.Creater)
                {
                    if(networkObject.isLocal)
                    {
                        ReceiveFunction();
                    }
                    else
                    {
                        P2PNetwork.lobby.Broadcast(receiveTarget,networkObject.peerId,baseData,null);
                    }
                }
                else
                {
                    P2PNetwork.lobby.Broadcast(receiveTarget,null,baseData,null);
                }
            }
            else if(P2PNetwork.Instance.currState == State.InRoom)
            {
                if(receiveTarget == ReceiveTarget.Creater)
                {
                    if(networkObject.isLocal)
                    {
                        ReceiveFunction();
                    }
                    else
                    {
                        P2PNetwork.room.SendMsg(receiveTarget,networkObject.peerId,baseData,null,channel);
                    }
                }
                else
                {
                    P2PNetwork.room.SendMsg(receiveTarget,null,baseData,null,channel);
                }

            }
        }

        /// <summary>
        /// 发送消息给多个客户端
        /// <para>T：数据类型</para>
        /// <para>receiveTarget：目标对象群</para>
        /// <para>data：数据类</para>
        /// <para>ReceiveFunction：接收的函数</para>
        /// </summary>
        public void Send<T>(ReceiveTarget receiveTarget,T data,Action<T> ReceiveFunction,Channel channel = Channel.TCP)
        {
            if(string.IsNullOrEmpty(networkObject.netId))
            {
                P2PDebug.LogError("非动态创建的网络对象必须手动指定唯一的NetId");
                return;
            }
            SynDataBase baseData = new SynDataBase(P2PMsgType.NetworkBehaviourMsg,data.GetType().Name,ReceiveFunction.Method.Name,networkObject.peerId,networkObject.netId);
            if(P2PNetwork.Instance.currState == State.InLobby)
            {
                if(receiveTarget == ReceiveTarget.Creater)
                {
                    if(networkObject.isLocal)
                    {
                        ReceiveFunction(data);
                    }
                    else
                    {
                        P2PNetwork.lobby.Broadcast(receiveTarget,networkObject.peerId,baseData,data);
                    }
                }
                else
                {
                    P2PNetwork.lobby.Broadcast(receiveTarget,null,baseData,data);
                }
            }
            else if(P2PNetwork.Instance.currState == State.InRoom)
            {
                
                if(receiveTarget == ReceiveTarget.Creater)
                {
                    if(networkObject.isLocal)
                    {
                        ReceiveFunction(data);
                    }
                    else
                    {
                        P2PNetwork.room.SendMsg(receiveTarget,networkObject.peerId,baseData,data,channel);
                    }
                }
                else
                {
                    P2PNetwork.room.SendMsg(receiveTarget,null,baseData,data,channel);
                }
            }
        }

        /// <summary>
        /// 发送消息给指定客户端
        /// <para>T：数据类型</para>
        /// <para>receivePeerId：客户端的ID,即客户端的Peer Id(由Device Id充当)</para>
        /// <para>data：数据类</para>
        /// <para>ReceiveFunction：接收的函数</para>
        /// </summary>
        public void Send<T>(string receivePeerId,T data,Action<T> ReceiveFunction,Channel channel = Channel.TCP)
        {
            if(string.IsNullOrEmpty(networkObject.netId))
            {
                P2PDebug.LogError("非动态创建的网络对象必须手动指定唯一的NetId");
                return;
            }
            SynDataBase baseData = new SynDataBase(P2PMsgType.NetworkBehaviourMsg,data.GetType().Name,ReceiveFunction.Method.Name,networkObject.peerId,networkObject.netId);
            if(P2PNetwork.Instance.currState == State.InLobby)
            {
                P2PNetwork.lobby.Broadcast(ReceiveTarget.All,receivePeerId,baseData,data);
            }
            else if(P2PNetwork.Instance.currState == State.InRoom)
            {
                P2PNetwork.room.SendMsg(ReceiveTarget.All,receivePeerId,baseData,data,channel);
            }
        }

        /// <summary>
        /// 在网络上执行Destroy(gameObject)，同时删除所有客户端上的镜像
        /// </summary>
        public void DestroyOnNet()
        {
            if(!networkObject.isPublic && !networkObject.isLocal)
            {
                P2PDebug.LogWarning("当前对象所在的客户端没有权限，请在创建者的客户端上执行该方法");
                return;
            }
            SynDataBase emptyData = new SynDataBase();
            Send(ReceiveTarget.All,emptyData,DoDestroyOnNet,Channel.TCP);
        }
        private void DoDestroyOnNet(SynDataBase data)
        {
            Destroy(gameObject);
        }

    }
}
