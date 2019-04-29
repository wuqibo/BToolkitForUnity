using BToolkit.P2PNetwork;
using UnityEngine;

namespace BToolkit {
    public class ActorProperties: MonoBehaviour {

        public struct Properties {
            public string finger;
            public int hpMax;
            public int mpMax;
            public int atk;
            public int def;
            public int rec;
            public float moveSpeed;
            public int team;
            public bool hasDie;
            public string actorId;
        }
        internal Properties properties;
        internal float MoveSpeedNormal { get { return properties.moveSpeed; } }
        internal float MoveSpeed { get { return MoveSpeedNormal; } }
        internal string Finger { get { return properties.finger; } }
        internal int Atk {
            get {
                int atkFromBuff = 0;
                int buffCount = actor.buffs.Count;
                for(int i = 0;i < buffCount;i++) {
                    Buff buff = actor.buffs[i];
                    if(buff.isActive) {
                        for(int j = 0;j < buff.values.Length;j++) {
                            if(buff.values[j].type == Buff.ValueType.Atk) {
                                atkFromBuff += buff.values[j].value;
                            }
                        }
                    }
                }
                return properties.atk + atkFromBuff;
            }
        }
        internal int Def {
            get {
                int defFromBuff = 0;
                int buffCount = actor.buffs.Count;
                for(int i = 0;i < buffCount;i++) {
                    Buff buff = actor.buffs[i];
                    if(buff.isActive) {
                        for(int j = 0;j < buff.values.Length;j++) {
                            if(buff.values[j].type == Buff.ValueType.Def) {
                                defFromBuff += buff.values[j].value;
                            }
                        }
                    }
                }
                return properties.def + defFromBuff;
            }
        }
        internal int Rec {
            get {
                int recFromBuff = 0;
                int buffCount = actor.buffs.Count;
                for(int i = 0;i < buffCount;i++) {
                    Buff buff = actor.buffs[i];
                    if(buff.isActive) {
                        for(int j = 0;j < buff.values.Length;j++) {
                            if(buff.values[j].type == Buff.ValueType.Rec) {
                                recFromBuff += buff.values[j].value;
                            }
                        }
                    }
                }
                return properties.rec + recFromBuff;
            }
        }
        public int HP { get; protected set; }
        public int HPMax { get { return properties.hpMax; } }
        public int MP { get; protected set; }
        public int MPMax { get { return properties.mpMax; } }
        internal int Team { get { return properties.team; } }
        public bool HasDie { get { return properties.hasDie; } }
        protected Actor actor;

        protected void Awake() {
            actor = GetComponent<Actor>();
        }

        /// <summary>
        /// 尝试通过网络通道初始化属性
        /// </summary>
        internal void InitProperties(Properties properties) {
            properties.hasDie = false;
            if(actor.actorNetwork && P2PNetwork.P2PNetwork.Instance.IsConnected) {
                actor.actorNetwork.InitProperties(properties);
            } else {
                this.properties = properties;
                OnPropertiesInitOnClient();
            }
        }

        internal virtual void OnPropertiesInitOnClient() {
            this.HP = HPMax;
        }

        /// <summary>
        /// 尝试通过网络通道修改伤害后的血量
        /// </summary>
        internal virtual void HurtOnClient(Actor attacker,HurtData hurtData) {
            //Debug.LogWarning("HurtOnClient: " + hurtData.damage+ "  actor.HasAuthority:" + actor.HasAuthority);
            //联网状态下确保只有本地玩家执行计算，然后广播
            if(actor.HasAuthority) {
                //计算防御
                hurtData.damage -= Def;
                if(hurtData.damage < 0f) {
                    hurtData.damage = 0;
                }
                //执行扣血
                if(hurtData.damage > 0) {
                    HP -= hurtData.damage;
                    if(HP < 0) {
                        HP = 0;
                    }
                    SetHP(HP,attacker);
                }
                //广播掉血数字
                if(actor.actorNetwork && P2PNetwork.P2PNetwork.Instance.IsConnected) {
                    //Debug.LogWarning("HurtOnClient: " + hurtData.damage);
                    actor.actorNetwork.DropHP(hurtData.damage);
                } else {
                    DropHPOnClient(hurtData.damage);
                }
            }
        }
        internal void DropHPOnClient(int value) {
            if(value != 0) {
                DropHP.Drop(actor.HurtPos,actor.trans.up,value,false);
            }
        }
        internal virtual void OnHPUpdateOnClient(int hp,Actor attacker) {
            this.HP = hp;
            if(this.HP > HPMax) {
                this.HP = HPMax;
            } else if(this.HP < 0) {
                this.HP = 0;
            }
            if(this.HP == 0f) {
                if(!HasDie) {
                    properties.hasDie = true;
                    actor.DieOnClient(attacker);
                }
            }
        }

        /// <summary>
        /// 尝试通过网络广播血量
        /// </summary>
        internal void SetHP(int hp,Actor attacker) {
            if(actor.actorNetwork && P2PNetwork.P2PNetwork.Instance.IsConnected) {
                actor.actorNetwork.SetHP(hp,attacker);
            } else {
                actor.actorProperties.OnHPUpdateOnClient(hp,attacker);
            }
        }

        /// <summary>
        /// 尝试通过网络广播加血
        /// </summary>
        internal void AddHP(int value) {
            if(value >= 0) {
                if(actor.HasAuthority) {
                    HP += value;
                    if(HP > HPMax) {
                        HP = HPMax;
                    }
                    SetHP(HP,null);
                }
            }
        }

    }
}