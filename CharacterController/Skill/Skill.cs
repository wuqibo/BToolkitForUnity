using UnityEngine;
using System.Collections;

namespace BToolkit {
    public abstract class Skill {

        /// <summary>
        /// 普攻还是技能
        /// </summary>
        public enum Type {
            NormalAttack,
            Skill
        }
        public struct Properties {
            public string skillName;
            public int value1, value2;
        }

        public Properties properties { protected set; get; }
        protected Actor actor;
        public bool canInterrput { protected set; get; }
        protected ActorCombat.SkillConfig config;
        protected float animFinishTime = 0.95f;
        public bool hasFinish;
        protected Type type;
        protected int currSkillIndex = -1;
        bool finishSkillAtAnimEnd;

        /// <summary>
        /// skillIndex由ActorCombat.ExecuteSkill()指定
        /// </summary>
        public Skill(Type type) : this(type,-1) {
        }

        /// <summary>
        /// skillIndex指定为大于等于0的索引之后,ActorCombat.ExecuteSkill()指定的skillIndex将被忽略
        /// </summary>
        public Skill(Type type,int skillIndex) {
            this.type = type;
            if(skillIndex >= 0) {
                this.currSkillIndex = skillIndex;
            }
        }

        public virtual void Update() {
            OnUpdate();
            if(finishSkillAtAnimEnd) {
                AnimatorStateInfo mAnimatorStateInfo = actor.animator.GetCurrentAnimatorStateInfo(0);
                if(mAnimatorStateInfo.IsName(config.animName)) {
                    if(mAnimatorStateInfo.normalizedTime > animFinishTime) {
                        Finish();
                    }
                }
            }
        }

        /// <summary>
        /// 当forEditorMode为false时,优先使用技能例化时指定的索引,这里设置无效
        /// </summary>
        internal void Start(Actor actor,int skillIndex,bool forEditorMode) {
            this.actor = actor;
            if(forEditorMode) {
                this.config = actor.actorCombat.skillConfigs[skillIndex];
            } else {
                if(this.currSkillIndex >= 0) {
                    this.config = actor.actorCombat.skillConfigs[this.currSkillIndex];
                } else if(skillIndex >= 0) {
                    this.config = actor.actorCombat.skillConfigs[skillIndex];
                } else {
                    Debug.LogError(actor.name + " " + GetType().ToString() + "未指定skillIndex");
                }
            }
            hasFinish = false;
            canInterrput = false;
            if(actor.actorState) {
                actor.actorState.SwitchToStateOnClient(ActorState.StateEnum.Attack);
            }
            this.finishSkillAtAnimEnd = true;
            if(config.words.Count > 0) {
                actor.actorCombat.PlayWordSound(config.words[Random.Range(0,config.words.Count)],false);
            }
            OnStart();
        }

        /// <summary>
        /// 该函数必须确保所有客户端一起执行
        /// </summary>
        protected void ChangeSkillIndexTo(int index) {
            this.currSkillIndex = index;
            this.config = actor.actorCombat.skillConfigs[this.currSkillIndex];
        }

        internal void Interrupt() {
            if(canInterrput) {
                OnInterrupt();
                actor.actorCombat.currSkill = null;
                actor.actorState.SwitchToState(ActorState.StateEnum.Idle);
                hasFinish = true;
            }
        }

        protected void Finish() {
            OnFinish();
            actor.actorCombat.currSkill = null;
            if(type == Type.NormalAttack) {
                for(int i = 0;i < actor.buffs.Count;i++) {
                    actor.buffs[i].OnOneNorAtkUsed();
                }
            } else {
                for(int i = 0;i < actor.buffs.Count;i++) {
                    actor.buffs[i].OnOneSkillUsed();
                }
            }
            actor.actorState.SwitchToStateOnClient(ActorState.StateEnum.Idle);
            hasFinish = true;
        }
        protected abstract void OnStart();
        protected abstract void OnUpdate();
        protected abstract void OnInterrupt();
        protected abstract void OnFinish();

        protected void PlaySkillAnim(bool finishSkillAtAnimEnd) {
            this.finishSkillAtAnimEnd = finishSkillAtAnimEnd;
            actor.PlayAnim(config.animName,0f);
        }

        /// <summary>
        /// <para>该方法广播（离线模式直接执行本地函数）并在客户端回调SetParamsOnClient()函数。</para>
        /// </summary>
        protected void BroadcastParams(int parame) {
            actor.actorCombat.BroadcastSkillParams(parame);
        }

        /// <summary>
        /// 本地玩家执行BroadcastParams()之后由系统回调
        /// </summary>
        internal virtual void SetParamsOnClient(int parame) {
        }

        /// <summary>
        /// 该方法用于请求服务器广播（离线模式直接执行本地函数）触发客户端执行OnClientPlayBodyEffect()或者ClientSpawnEffect()其中之一。
        /// </summary>
        protected void BroadcastPlayOrSpawnEffect(int effectIndex,Vector3 shootDirection,string[] targetNetIds,HurtData hurtData) {
            //Debug.LogError("BroadcastPlayOrSpawnEffect");
            actor.actorCombat.PlayOrSpawnEffect(effectIndex,shootDirection,targetNetIds,hurtData);
        }

        /// <summary>
        /// 系统调用
        /// </summary>
        internal void PlayOrSpawnEffectOnClient(int effectIndex,Vector3 shootDirection,string[] targetNetIds,HurtData hurtData) {
            if(config.effects != null && effectIndex < config.effects.Count) {
                ActorCombat.Effect effect = config.effects[effectIndex];
                if(effect.particle && effect.particle.transform.parent) {
                    actor.StartCoroutine(PlayerTrail(effect.particle,effect.lifeTime));
                    if(effect.sounds.Count > 0) {
                        SoundPlayer.PlayAndDestroy(0,effect.sounds[Random.Range(0,effect.sounds.Count)]);
                    }
                    OnClientPlayBodyEffect(effectIndex,shootDirection,targetNetIds,hurtData);
                } else {
                    ClientSpawnEffect(effectIndex,shootDirection,targetNetIds,hurtData);
                }
            }
        }

        IEnumerator PlayerTrail(GameObject particle,float timelife) {
            ParticleSystem[] particles = particle.GetComponentsInChildren<ParticleSystem>();
            for(int i = 0;i < particles.Length;i++) {
                particles[i].Play();
            }
            TrailRenderer[] trails = particle.GetComponentsInChildren<TrailRenderer>();
            for(int i = 0;i < trails.Length;i++) {
                trails[i].enabled = true;
            }
            yield return new WaitForSeconds(timelife);
            for(int i = 0;i < trails.Length;i++) {
                trails[i].enabled = false;
            }
        }

        /// <summary>
        /// 本地玩家调用BroadcastPlayOrSpawnEffect()后,如果是播放身体子集里的特效，将在每个客户端播放身体子集里的特效后触发。
        /// </summary>
        protected abstract void OnClientPlayBodyEffect(int effectIndex,Vector3 shootDirection,string[] targetNetIds,HurtData hurtData);

        /// <summary>
        /// 本地玩家调用BroadcastPlayOrSpawnEffect()后,如果特效不属于身体里的粒子，将在每个客户端触发，可在这里编写创建特效的逻辑代码
        /// </summary>
        protected abstract void ClientSpawnEffect(int effectIndex,Vector3 shootDirection,string[] targetNetIds,HurtData hurtData);

        protected ActorCombat.Effect GetEffectByIndex(int index) {
            return config.effects[index];
        }
    }
}