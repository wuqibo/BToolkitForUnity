using UnityEngine;
using System.Collections.Generic;
using BToolkit.P2PNetwork;

namespace BToolkit {
    public class ActorNetwork:NetworkBehaviour {

        protected Actor actor;

        void Awake() {
            P2PNetwork.P2PNetwork.RegisterSynDataType(typeof(ActorProperties.Properties));
            P2PNetwork.P2PNetwork.RegisterSynDataType(typeof(ExecuteSkillStruct));
            P2PNetwork.P2PNetwork.RegisterSynDataType(typeof(SpawnEffectStruct));
            P2PNetwork.P2PNetwork.RegisterSynDataType(typeof(SetHPStruct));
            actor = GetComponent<Actor>();
        }

        #region 通过网络通道广播属性初始化
        internal void InitProperties(ActorProperties.Properties properties) {
            if(networkObject.isLocal) {
                Send(ReceiveTarget.All,properties,InitPropertiesOnClient,Channel.TCP);
            }
        }
        void InitPropertiesOnClient(ActorProperties.Properties properties) {
            actor.actorProperties.properties = properties;
            actor.actorProperties.OnPropertiesInitOnClient();
        }
        #endregion

        #region 通过网络通道广播切换角色状态
        internal void SwitchToState(ActorState.StateEnum toStateEnum) {
            if(networkObject.isLocal) {
                Send(ReceiveTarget.All,toStateEnum,SwitchToStateOnClient,Channel.TCP);
            }
        }
        void SwitchToStateOnClient(ActorState.StateEnum toStateEnum) {
            actor.actorState.SwitchToStateOnClient(toStateEnum);
        }
        #endregion

        #region 通过网络通道广播释放技能
        struct ExecuteSkillStruct {
            public string skillId;
            public int skillIndex;
            public bool forEditorMode;
        }
        internal void ExecuteSkill(string skillId,int skillIndex,bool forEditorMode) {
            if(networkObject.isLocal) {
                ExecuteSkillStruct data = new ExecuteSkillStruct();
                data.skillId = skillId;
                data.skillIndex = skillIndex;
                data.forEditorMode = forEditorMode;
                Send(ReceiveTarget.All,data,ExecuteSkillOnClientByNet,Channel.TCP);
            }
        }
        void ExecuteSkillOnClientByNet(ExecuteSkillStruct data) {
            actor.actorCombat.ExecuteSkillOnClient(data.skillId,data.skillIndex,data.forEditorMode);
        }
        #endregion

        #region 通过网络通道广播敌人ID
        internal void BroadcastSkillParams(int parame) {
            if(networkObject.isLocal) {
                Send(ReceiveTarget.All,parame,ReceiveSkillParamsOnClient,Channel.TCP);
            }
        }
        void ReceiveSkillParamsOnClient(int parame) {
            if(actor.actorCombat.currSkill != null) {
                actor.actorCombat.currSkill.SetParamsOnClient(parame);
            }
        }
        #endregion

        #region 通过网络通道广播释放特效
        struct SpawnEffectStruct {
            public int effectIndex;
            public Vector3 shootDirection;
            public string[] targetNetIds;
            public int HurtData_type;
            public int HurtData_skillNum;
            public int HurtData_damage;
            public int HurtData_penetration;
        }
        internal void PlayOrSpawnEffect(int effectIndex,Vector3 shootDirection,string[] targetNetIds,HurtData hurtData) {
            if(networkObject.isLocal) {
                SpawnEffectStruct data = new SpawnEffectStruct();
                data.effectIndex = effectIndex;
                data.shootDirection = shootDirection;
                data.targetNetIds = targetNetIds;
                data.HurtData_type = (int)hurtData.type;
                data.HurtData_skillNum = hurtData.skillNum;
                data.HurtData_damage = hurtData.damage;
                data.HurtData_penetration = hurtData.penetration;
                Send(ReceiveTarget.All,data,PlayOrSpawnEffectOnClient,Channel.TCP);
            }
        }

        void PlayOrSpawnEffectOnClient(SpawnEffectStruct data) {
            if(actor.actorCombat.currSkill != null) {
                HurtData hurtData = new HurtData();
                hurtData.type = (HurtData.Type)data.HurtData_type;
                hurtData.skillNum = data.HurtData_skillNum;
                hurtData.damage = data.HurtData_damage;
                hurtData.penetration = data.HurtData_penetration;
                actor.actorCombat.currSkill.PlayOrSpawnEffectOnClient(data.effectIndex,data.shootDirection,data.targetNetIds,hurtData);
            }
        }
        #endregion

        #region 通过网络通道同步血量
        struct SetHPStruct {
            public int hp;
            public string attackerNetId;
        }
        internal void SetHP(int hp,Actor attacker) {
            if(networkObject.isLocal) {
                string attackerNetId = "";//初始一个场景里绝对没有的ID
                if(attacker) {
                    attackerNetId = attacker.actorNetwork.networkObject.netId;
                }
                SetHPStruct data = new SetHPStruct();
                data.hp = hp;
                data.attackerNetId = attackerNetId;
                Send(ReceiveTarget.All,data,SetHPOnClient,Channel.TCP);
            }
        }

        void SetHPOnClient(SetHPStruct data) {
            Actor attacker = FindNetActorById(data.attackerNetId);
            actor.actorProperties.OnHPUpdateOnClient(data.hp,attacker);
        }
        #endregion

        #region 通过网络通道执行掉血特效
        internal void DropHP(int value) {
            if(networkObject.isLocal) {
                Send(ReceiveTarget.All,value,DropHPOnClient,Channel.TCP);
            }
        }
        void DropHPOnClient(int value) {
            actor.actorProperties.DropHPOnClient(value);
        }
        #endregion


        #region 通过NetId查找场景中的角色
        /// <summary>
        /// 通过NetId查找场景中的对应Actor
        /// </summary>
        internal static Actor FindNetActorById(string actorNetId) {
            Actor[] actors = FindNetActorsByIds(new string[] { actorNetId });
            if(actors.Length > 0) {
                return actors[0];
            }
            return null;
        }
        /// <summary>
        /// 通过NetId查找场景中的全部Actor
        /// </summary>
        internal static Actor[] FindNetActorsByIds(string[] actortNetIds) {
            List<Actor> targets = new List<Actor>();
            if(actortNetIds != null && actortNetIds.Length > 0) {
                foreach(NetworkObject networkIdentity in P2PNetwork.P2PNetwork.Instance.networkObjectsInScene) {
                    if(networkIdentity) {
                        for(int j = 0;j < actortNetIds.Length;j++) {
                            if(networkIdentity.netId == actortNetIds[j]) {
                                targets.Add(networkIdentity.GetComponent<Actor>());
                                break;
                            }
                        }
                    }
                }
            }
            return targets.ToArray();
        }
        #endregion

        protected override void OnInstantiated(string msg) {
            if(!networkObject.isLocal) {
                Debug.Log(">>>>>>>>>>>>>>>>0000000000");
                PeerInfo peerInfo = new PeerInfo();
                peerInfo.peerId = P2PNetwork.P2PNetwork.localPeerId;
                Send(networkObject.peerId,peerInfo,LocalGetNewSpawnOnNetAndReSendProperty);
            }
        }
        //有新客户端连进来时生成了英雄的镜像，此时需给他发送属性信息
        void LocalGetNewSpawnOnNetAndReSendProperty(PeerInfo data) {
            string peerId = data.peerId;
            Send(peerId,actor.actorProperties.properties,InitPropertiesOnClient,Channel.TCP);
        }

        protected override void OnSomeoneJoinedLobby(string peerId) {

        }

        protected override void OnSomeoneLeavedLobby(string peerId,bool isException) {

        }

        protected override void OnSomeoneJoinedRoom(string peerId) {

        }

        protected override void OnSomeoneLeavedRoom(string peerId,bool isException) {

        }
    }
}