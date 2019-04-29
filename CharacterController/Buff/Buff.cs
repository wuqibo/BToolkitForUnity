using UnityEngine;
using System.Collections;

namespace BToolkit {
    public abstract class Buff {

        /// <summary>
        /// Buff 还是 DeBuff
        /// </summary>
        public enum Type {
            Buff,
            DeBuff
        }
        /// <summary>
        /// Buff生命周期
        /// </summary>
        public enum WayOfLife {
            /// <summary>
            /// 永久生效,直到角色死亡
            /// </summary>
            Forever,
            /// <summary>
            /// 按时间自消
            /// </summary>
            Timer,
            /// <summary>
            /// 普攻一次后自消
            /// </summary>
            OneNorAtk,
            /// <summary>
            /// 技能一次后自消
            /// </summary>
            OneSkill,
            /// <summary>
            /// 被攻击一次后自消
            /// </summary>
            OneHurt,
            /// <summary>
            /// 下一个敌方回合结束时自消
            /// </summary>
            EnemyRoundEnd,
            /// <summary>
            /// 我方本回合结束时自消
            /// </summary>
            MyRoundEnd
        }
        /// <summary>
        /// Buff数值属性
        /// </summary>
        public enum ValueType {
            /// <summary>
            /// 对攻击力进行增减
            /// </summary>
            Atk,
            /// <summary>
            /// 对防御力进行增减
            /// </summary>
            Def,
            /// <summary>
            /// 对回复力进行增减
            /// </summary>
            Rec
        }
        public class Value {
            public ValueType type;
            public int value;
            public Value(ValueType type,int value) {
                this.type = type;
                this.value = value;
            }
        }
        protected Actor actor;
        public Type type { protected set; get; }
        public WayOfLife wayOfLife { protected set; get; }
        public float lifeTime { protected set; get; }
        public Value[] values { protected set; get; }
        public bool isActive;

        public Buff(Type type,WayOfLife wayOfLife,float lifeTime) {
            this.type = type;
            this.wayOfLife = wayOfLife;
            this.lifeTime = lifeTime;
            values = new Value[0];
            isActive = true;
        }

        internal void Start(Actor actor) {
            this.actor = actor;
            OnStart();
        }

        public virtual void Update() {
            OnUpdate();
            if(wayOfLife == WayOfLife.Timer) {
                lifeTime -= Time.deltaTime;
                if(lifeTime <= 0f) {
                    actor.RemoveBuff(this);
                }
            }
        }

        internal void Finish() {
            OnFinish();
        }

        internal void OnOneNorAtkUsed() {
            if(wayOfLife == WayOfLife.OneNorAtk) {
                actor.RemoveBuff(this);
            }
        }

        internal void OnOneSkillUsed() {
            if(wayOfLife == WayOfLife.OneSkill) {
                actor.RemoveBuff(this);
            }
        }

        internal void OnOneHurtUsed() {
            if(wayOfLife == WayOfLife.OneHurt) {
                actor.RemoveBuff(this);
            }
        }

        internal void OnEnemyRoundEndUsed() {
            if(wayOfLife == WayOfLife.EnemyRoundEnd) {
                actor.RemoveBuff(this);
            }
        }

        internal void OnMyRoundEndUsed() {
            if(wayOfLife == WayOfLife.MyRoundEnd) {
                actor.RemoveBuff(this);
            }
        }

        protected abstract void OnFinish();
        protected abstract void OnUpdate();
        protected abstract void OnStart();

    }
}