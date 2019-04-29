using System;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit.P2PNetwork
{
    public class Lobby
    {

        private UDPBroadcaster broadcaster;
        private UDPReceiver udpReceiver;
        public ConnResultEvent JoinLobbyCallback;
        /// <summary>
        /// 存储当前大厅中的房间列表，Key为房间创建者创建时的一个随机的GUID
        /// </summary>
        public Dictionary<string, Room.RoomAlive> rooms = new Dictionary<string, Room.RoomAlive>();
        private float roomsCheckTimer;

        public void Join(ConnResultEvent Callback)
        {
            this.JoinLobbyCallback = Callback;
            broadcaster = new UDPBroadcaster(P2PConfig.Instance.Port);
            udpReceiver = new UDPReceiver(P2PConfig.Instance.Port, MessageManager.Instance.Receive);
            broadcaster.Start();
            udpReceiver.Start();
            P2PNetwork.Instance.currState = State.InLobby;
            BroadcastJoinedEvent();
            P2PDebug.Log("已加入大厅");
            if (JoinLobbyCallback != null)
            {
                JoinLobbyCallback(ConnState.Success);
                JoinLobbyCallback = null;
            }
        }

        /// <summary>
        /// 让房间里的老成员触发OnSomeoneJoined事件
        /// </summary>
        public void BroadcastJoinedEvent()
        {
            SynDataBase synData = new SynDataBase(P2PMsgType.SomeoneJoined, null, null, P2PNetwork.localPeerId, null);
            Broadcast(ReceiveTarget.All, null, synData, new SynDataBase());
        }

        public void Update()
        {
            roomsCheckTimer -= Time.deltaTime;
            if (roomsCheckTimer <= 0f)
            {
                roomsCheckTimer = Room.broadcastInterval;
                CheckClosedRooms();
            }
        }

        /// <summary>
        /// 检查清除非活动状态的房间
        /// </summary>
        private void CheckClosedRooms()
        {
            List<string> closedRoomsKey = new List<string>();
            foreach (var room in rooms.Values)
            {
                if (!room.Check())
                {
                    closedRoomsKey.Add(room.info.roomId);
                }
            }
            if (closedRoomsKey.Count > 0)
            {
                foreach (var key in closedRoomsKey)
                {
                    rooms.Remove(key);
                }
                if (P2PNetwork.Instance.RoomListUpdateEvent != null)
                {
                    P2PNetwork.Instance.RoomListUpdateEvent(RoomInfos(rooms));
                }
            }
        }

        /// <summary>
        /// 发送UDP广播
        /// </summary>
        public void Broadcast(ReceiveTarget receiveTarget, string receivePeerId, SynDataBase baseData, object customData)
        {
            string json = P2PUtility.ObjectToJson(baseData);
            json += MsgDivid.BaseData;
            if (customData != null)
            {
                json += P2PUtility.ParseCustomData(customData);
            }
            LobbyReceiveTarget lobbyReceiveTarget = new LobbyReceiveTarget();
            if (!string.IsNullOrEmpty(receivePeerId))
            {
                lobbyReceiveTarget.receivePeerId = receivePeerId;
            }
            else
            {
                lobbyReceiveTarget.receiveTarget = (int)receiveTarget;
            }
            //接收群组加到尾部
            json += MsgDivid.LobbyReceiveTarget + P2PUtility.ObjectToJson(lobbyReceiveTarget);
            byte[] bytes = P2PUtility.JsonToBytes(json);
            broadcaster.Send(bytes);
        }

        /// <summary>
        /// 系统函数，消息处理中心调用
        /// </summary>
        public void ReceiveRoomInfo(RoomInfo roomInfo)
        {
            if (roomInfo.isOpen)
            {
                if (rooms.ContainsKey(roomInfo.roomId))
                {
                    Room.RoomAlive roomAlive = rooms[roomInfo.roomId];
                    roomAlive.KeepAlive();
                    //如果房间人数有变化则重新刷新
                    if (roomAlive.info.currNum != roomInfo.currNum)
                    {
                        roomAlive.info = roomInfo;
                        if (P2PNetwork.Instance.RoomListUpdateEvent != null)
                        {
                            P2PNetwork.Instance.RoomListUpdateEvent(RoomInfos(rooms));
                        }
                    }
                }
                else
                {
                    rooms.Add(roomInfo.roomId, new Room.RoomAlive(roomInfo));
                    if (P2PNetwork.Instance.RoomListUpdateEvent != null)
                    {
                        P2PNetwork.Instance.RoomListUpdateEvent(RoomInfos(rooms));
                    }
                }
            }
            else
            {
                if (rooms.ContainsKey(roomInfo.roomId))
                {
                    rooms.Remove(roomInfo.roomId);
                    if (P2PNetwork.Instance.RoomListUpdateEvent != null)
                    {
                        P2PNetwork.Instance.RoomListUpdateEvent(RoomInfos(rooms));
                    }
                }
            }
        }

        /// <summary>
        /// 刷新房间列表，系统将再次触发RoomListUpdateEvent事件，显示房间列表UI时刷新一次
        /// </summary>
        public void RefreshRoomList()
        {
            if (P2PNetwork.Instance.RoomListUpdateEvent != null)
            {
                P2PNetwork.Instance.RoomListUpdateEvent(RoomInfos(rooms));
            }
        }

        private RoomInfo[] RoomInfos(Dictionary<string, Room.RoomAlive> rooms)
        {
            List<RoomInfo> roomInfos = new List<RoomInfo>();
            foreach (var room in rooms.Values)
            {
                roomInfos.Add(room.info);
            }
            return roomInfos.ToArray();
        }

        /// <summary>
        /// 离开大厅（APP退出时和进入房间之前调用）
        /// </summary>
        public void Leave()
        {
            if (broadcaster != null)
            {
                broadcaster.Stop();
                broadcaster = null;
            }
            if (udpReceiver != null)
            {
                udpReceiver.Stop();
                udpReceiver = null;
            }
            P2PNetwork.Instance.currState = State.Offline;
            if (P2PNetwork.Instance.SomeoneLeavedLobbyEvent != null)
            {
                P2PNetwork.Instance.SomeoneLeavedLobbyEvent(P2PNetwork.localPeerId, false);
            }
            int length = P2PNetwork.Instance.networkObjectsInScene.Count;
            for (int i = 0; i < length; i++)
            {
                P2PNetwork.Instance.networkObjectsInScene[i].SomeoneLeavedLobby(P2PNetwork.localPeerId, false);
            }
            rooms.Clear();
            if (P2PNetwork.Instance.RoomListUpdateEvent != null)
            {
                P2PNetwork.Instance.RoomListUpdateEvent(new RoomInfo[0]);
            }
            P2PDebug.Log("已离开大厅");
        }
    }
}