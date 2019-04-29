using System;
using UnityEngine;

namespace BToolkit.P2PNetwork
{
    public class MessageManager
    {

        public static bool haveInstance { get { return instance != null; } }
        private static MessageManager instance;
        public static MessageManager Instance { get { return instance ?? (instance = new MessageManager()); } }
        public ReceiveMsgEvent OnReceiveCustomMsg;
        private MessageManager() { }


        /// <summary>
        /// 接收消息并进行分类处理（大厅和房间共用此方法）
        /// </summary>
        public void Receive(string json)
        {
            MainThread.Run(() =>
            {
                bool isLobbyReceiveTargetIsOther = false;
                if (json.Contains(MsgDivid.LobbyReceiveTarget))
                {
                    int index = json.IndexOf(MsgDivid.LobbyReceiveTarget);
                    string lobbyReceiveTargetJson = json.Substring(index + MsgDivid.LobbyReceiveTarget.Length);
                    LobbyReceiveTarget lobbyReceiveTarget = P2PUtility.JsonToObject<LobbyReceiveTarget>(lobbyReceiveTargetJson);
                    if (!string.IsNullOrEmpty(lobbyReceiveTarget.receivePeerId))
                    {
                        if (!P2PNetwork.localPeerId.Equals(lobbyReceiveTarget.receivePeerId))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (lobbyReceiveTarget.receiveTarget == (int)ReceiveTarget.Other)
                        {
                            isLobbyReceiveTargetIsOther = true;
                        }
                    }
                    json = json.Remove(index);
                }
                string baseDataStr = json.Substring(0, json.IndexOf(MsgDivid.BaseData));
                string customDataStr = json.Remove(0, json.IndexOf(MsgDivid.BaseData) + MsgDivid.BaseData.Length);
                SynDataBase baseData = P2PUtility.JsonToObject<SynDataBase>(baseDataStr);
                if (isLobbyReceiveTargetIsOther)
                {
                    if (baseData.peerId.Equals(P2PNetwork.localPeerId))
                    {
                        return;
                    }
                }
                switch (baseData.msgType)
                {
                    case P2PMsgType.SomeoneJoined:
                        //大厅状态下触发
                        if (P2PNetwork.Instance.currState == State.InLobby)
                        {
                            if (P2PNetwork.Instance.SomeoneJoinedLobbyEvent != null)
                            {
                                P2PNetwork.Instance.SomeoneJoinedLobbyEvent(baseData.peerId);
                            }
                            int length = P2PNetwork.Instance.networkObjectsInScene.Count;
                            for (int i = 0; i < length; i++)
                            {
                                P2PNetwork.Instance.networkObjectsInScene[i].SomeoneJoinedLobby(baseData.peerId);
                            }
                        }
                        break;
                    case P2PMsgType.InstantiateInfo:
                        FindAndSetInstantiateInfo(baseData, customDataStr);
                        break;
                    case P2PMsgType.NetworkBehaviourMsg:
                        FindAndSetNetworkBehaviourInfo(baseData, customDataStr);
                        break;
                    case P2PMsgType.RoomInfoToLobby:
                        if (P2PNetwork.Instance.currState == State.InLobby)
                        {
                            //接收大厅中广播的房间信息，显示房间列表
                            RoomInfo roomInfo = P2PUtility.JsonToObject<RoomInfo>(customDataStr);
                            P2PNetwork.lobby.ReceiveRoomInfo(roomInfo);
                        }
                        break;
                    case P2PMsgType.RoomInfoToRoom:
                        if (P2PNetwork.Instance.currState == State.InRoom)
                        {
                            //接收房间信息的改变
                            RoomInfo roomInfo = P2PUtility.JsonToObject<RoomInfo>(customDataStr);
                            P2PNetwork.room.ReceiveRoomInfo(roomInfo);
                        }
                        break;
                    case P2PMsgType.OtherPeersInRoom:
                        PeersInRoom peersInRoom = P2PUtility.JsonToObject<PeersInRoom>(customDataStr);
                        P2PNetwork.room.ReceiveAndConnectOtherPeers(peersInRoom);
                        break;
                    case P2PMsgType.MasterCandidate:
                        //接收房主房主候选人
                        MasterInfo masterInfo = P2PUtility.JsonToObject<MasterInfo>(customDataStr);
                        P2PNetwork.room.ReceiveMasterCandidate(masterInfo);
                        break;
                    case P2PMsgType.Heartbeat:
                        P2PNetwork.room.tcpServer.KeepClientAlive(baseData.peerId);
                        break;
                    case P2PMsgType.LeaveRoom:
                        P2PNetwork.room.tcpServer.ReceiveLeave(baseData.peerId);
                        break;
                    default:
                        if (OnReceiveCustomMsg != null)
                        {
                            if (customDataStr.Contains(MsgDivid.IsInt))
                            {
                                customDataStr = customDataStr.Remove(0, MsgDivid.IsInt.Length);
                                OnReceiveCustomMsg(baseData.msgType, int.Parse(customDataStr));
                            }
                            else if (customDataStr.Contains(MsgDivid.IsBool))
                            {
                                customDataStr = customDataStr.Remove(0, MsgDivid.IsBool.Length);
                                OnReceiveCustomMsg(baseData.msgType, bool.Parse(customDataStr));
                            }
                            else if (customDataStr.Contains(MsgDivid.IsFloat))
                            {
                                customDataStr = customDataStr.Remove(0, MsgDivid.IsFloat.Length);
                                OnReceiveCustomMsg(baseData.msgType, float.Parse(customDataStr));
                            }
                            else if (customDataStr.Contains(MsgDivid.IsString))
                            {
                                customDataStr = customDataStr.Remove(0, MsgDivid.IsString.Length);
                                OnReceiveCustomMsg(baseData.msgType, customDataStr);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(baseData.dataType))
                                {
                                    object data = P2PUtility.JsonToObject(baseDataStr, typeof(SynDataBase));
                                    OnReceiveCustomMsg(baseData.msgType, data);
                                }
                                else
                                {
                                    if (P2PNetwork.Instance.synDataTypes.ContainsKey(baseData.dataType))
                                    {
                                        Type type = P2PNetwork.Instance.synDataTypes[baseData.dataType];
                                        object data = P2PUtility.JsonToObject(customDataStr, type);
                                        OnReceiveCustomMsg(baseData.msgType, data);
                                    }
                                    else
                                    {
                                        TipRegisterClass(baseData.dataType);
                                    }
                                }
                            }
                        }
                        break;
                }
            });
        }

        /// <summary>
        /// 找到场景中对应的NetworkObject并设置InstantiateInfo，若没有找到则创建
        /// </summary>
        private void FindAndSetInstantiateInfo(SynDataBase baseData, string customDataStr)
        {
            int length = P2PNetwork.Instance.networkObjectsInScene.Count;
            for (int i = 0; i < length; i++)
            {
                if (P2PNetwork.Instance.networkObjectsInScene[i].netId.Equals(baseData.netId))
                {
                    return;
                }
            }
            //创建镜像对象，本地对象在P2PNetwork.cs行577处创建
            InstantiateInfo instantiateInfo = P2PUtility.JsonToObject<InstantiateInfo>(customDataStr);
            GameObject prafab = NetworkObject.GetNetworkObjectPrefabMethod(instantiateInfo.prefabPath);
            if (prafab)
            {
                GameObject go = GameObject.Instantiate(prafab, instantiateInfo.position, instantiateInfo.rotation);
                string[] peerIdArr = baseData.peerId.Split('_');
                go.name = go.name + "_" + peerIdArr[1];
                NetworkObject networkObject = go.GetComponent<NetworkObject>();
                if (!networkObject)
                {
                    networkObject = go.AddComponent<NetworkObject>();
                }
                networkObject.ReceiveInstantiateInfo(baseData, instantiateInfo);
            }
            else
            {
                P2PDebug.LogError("创建镜像 Net Object 失败：无法找到 " + instantiateInfo.prefabPath + " 目录下的 Prafab");
            }
        }

        /// <summary>
        /// 找到场景中对应的NetworkBehaviour并传输CustomData
        /// </summary>
        private void FindAndSetNetworkBehaviourInfo(SynDataBase baseData, string customDataStr)
        {
            int length = P2PNetwork.Instance.networkObjectsInScene.Count;
            for (int i = 0; i < length; i++)
            {
                NetworkObject obj = P2PNetwork.Instance.networkObjectsInScene[i];
                if (obj.netId.Equals(baseData.netId))
                {
                    if (customDataStr.Contains(MsgDivid.IsInt))
                    {
                        customDataStr = customDataStr.Remove(0, MsgDivid.IsInt.Length);
                        obj.SendMessage(baseData.receiveFun, int.Parse(customDataStr), SendMessageOptions.DontRequireReceiver);
                    }
                    else if (customDataStr.Contains(MsgDivid.IsBool))
                    {
                        customDataStr = customDataStr.Remove(0, MsgDivid.IsBool.Length);
                        obj.SendMessage(baseData.receiveFun, bool.Parse(customDataStr), SendMessageOptions.DontRequireReceiver);
                    }
                    else if (customDataStr.Contains(MsgDivid.IsFloat))
                    {
                        customDataStr = customDataStr.Remove(0, MsgDivid.IsFloat.Length);
                        obj.SendMessage(baseData.receiveFun, float.Parse(customDataStr), SendMessageOptions.DontRequireReceiver);
                    }
                    else if (customDataStr.Contains(MsgDivid.IsString))
                    {
                        customDataStr = customDataStr.Remove(0, MsgDivid.IsString.Length);
                        obj.SendMessage(baseData.receiveFun, customDataStr, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(baseData.dataType))
                        {
                            obj.SendMessage(baseData.receiveFun, SendMessageOptions.DontRequireReceiver);
                        }
                        else
                        {
                            if (P2PNetwork.Instance.synDataTypes.ContainsKey(baseData.dataType))
                            {
                                Type type = P2PNetwork.Instance.synDataTypes[baseData.dataType];
                                object data = P2PUtility.JsonToObject(customDataStr, type);
                                obj.SendMessage(baseData.receiveFun, data, SendMessageOptions.DontRequireReceiver);
                            }
                            else
                            {
                                TipRegisterClass(baseData.dataType);
                            }
                        }
                    }
                    break;
                }
            }
        }

        private void TipRegisterClass(string customStructName)
        {
            P2PDebug.LogError("共享数据类[" + customStructName + "]没有注册，请先调用P2PNetwork.Instance.RegisterSynDataType(typeof(" + customStructName + "))进行注册");
        }
    }
}
