using UnityEngine;
using System.Collections.Generic;
using BToolkit.P2PNetwork;

namespace BToolkit {
    [RequireComponent(typeof(ActorCombat))]
    [RequireComponent(typeof(ActorState))]
    [RequireComponent(typeof(ActorProperties))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Actor: MonoBehaviour {

        public float headBarHeight = 2.3f;
        internal ActorCombat actorCombat;
        internal ActorProperties actorProperties;
        internal ActorState actorState;
        internal ActorNetwork actorNetwork;
        internal CapsuleCollider capsuleCollider;
        internal Rigidbody mRigidbody;
        internal Animator animator;
        Transform _trans;
        internal Transform trans { get { return _trans ?? (_trans = transform); } }
        internal float joystickRadian;
        internal List<Buff> buffs = new List<Buff>();
        internal virtual Transform HurtPoint { get { return actorCombat.HurtPoint; } }
        internal virtual Vector3 HurtPos { get { return actorCombat.HurtPos; } }
        internal virtual int Team { get { return actorProperties.Team; } }
        internal bool HasDie { get { return actorProperties.HasDie; } }
        internal bool HasAuthority { get { return P2PNetwork.P2PNetwork.Instance.IsConnected ? actorNetwork.networkObject.isLocal : true; } }
        public string actorId { get; protected set; }
        static Actor myActor;
        internal static Actor MyCtrlActor {
            get { return myActor; }
            set {
                myActor = value;
                if(myActor == null) {
                    UGUIJoystick.UnregisterEvent();
                } else {
                    UGUIJoystick.RegisterEvent(myActor.OnJoystickDrag);
                }
            }
        }

        protected virtual void Awake() {
            actorCombat = GetComponent<ActorCombat>();
            actorProperties = GetComponent<ActorProperties>();
            actorState = GetComponent<ActorState>();
            actorNetwork = GetComponent<ActorNetwork>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            mRigidbody = GetComponent<Rigidbody>();
            mRigidbody.useGravity = false;
            animator = GetComponentInChildren<Animator>();
        }

        protected virtual void OnDestroy() {
            ClearBuffs();
        }

        protected virtual void Start() {
            actorCombat.InitSkills();
        }

        void Update() {
            //Buff Running
            for(int i = 0;i < buffs.Count;i++) {
                buffs[i].Update();
            }
        }

        void LateUpdate() {
            mRigidbody.Sleep();
        }

        void OnJoystickDrag(bool isActive,float radian) {
            if(isActive) {
                joystickRadian = radian;
                if(actorState.currStateEnum == ActorState.StateEnum.Idle) {
                    actorState.SwitchToState(ActorState.StateEnum.Move);
                } else if(actorState.currStateEnum == ActorState.StateEnum.Attack) {
                    if(actorCombat.currSkill != null) {
                        if(actorCombat.currSkill.canInterrput) {
                            actorCombat.currSkill.Interrupt();
                        }
                    }
                }
            } else {
                actorState.SwitchToState(ActorState.StateEnum.Idle);
            }
        }

        internal void PlayAnim(string clipName,float transitionDuration) {
            //Debug.Log("PlayAnim:" + clipName);
            if(animator) {
                if(transitionDuration <= 0) {
                    animator.Play(clipName,0,0f);
                } else {
                    if(animator.GetNextAnimatorStateInfo(0).fullPathHash == 0) {
                        animator.CrossFade(clipName,transitionDuration);
                    } else {
                        animator.Play(clipName,0,0f);
                    }
                }
            }
        }

        internal void AddBuff(Buff buff) {
            buffs.Add(buff);
            buff.Start(this);
        }
        internal void RemoveBuff(Buff buff) {
            if(buffs.Contains(buff)) {
                buffs.Remove(buff);
                buff.Finish();
                buff = null;
            }
        }
        internal void ClearBuffs() {
            List<Buff> buffsTemp = new List<Buff>(buffs);
            buffs.Clear();
            for(int i = 0;i < buffsTemp.Count;i++) {
                buffsTemp[i].Finish();
            }
            buffsTemp = null;
        }

        internal bool HasBuff(System.Type type) {
            for(int i = 0;i < buffs.Count;i++) {
                if(buffs[i].GetType() == type) {
                    return true;
                }
            }
            return false;
        }

        internal virtual void HurtOnClient(Actor attacker,HurtData hurtData) {
            //Debug.LogError("ExcuteHurt");
            actorProperties.HurtOnClient(attacker,hurtData);
            for(int i = 0;i < buffs.Count;i++) {
                buffs[i].OnOneHurtUsed();
            }
            if(actorState.currStateEnum == ActorState.StateEnum.Idle || actorState.currStateEnum == ActorState.StateEnum.IdleBreak || actorState.currStateEnum == ActorState.StateEnum.Hurt) {
                actorState.SwitchToStateOnClient(ActorState.StateEnum.Hurt);
            }
        }

        protected virtual void OnEnemyBeKillOnClient() {
            actorCombat.PlayWinWord();
        }

        internal virtual void DieOnClient(Actor attacker) {
            ClearBuffs();
            actorCombat.PlayDieWord();
            actorState.SwitchToStateOnClient(ActorState.StateEnum.Die);
            //攻击者函数被触发
            if(attacker) {
                attacker.OnEnemyBeKillOnClient();
            }
        }

        /// <summary>
        /// 通过NetId查找场景中的角色
        /// </summary>
        internal static Actor FindNetActorById(string actorNetId) {
            return ActorNetwork.FindNetActorById(actorNetId);
        }
        internal static Actor[] FindNetActorsByIds(string[] actortNetIds) {
            return ActorNetwork.FindNetActorsByIds(actortNetIds);
        }

        /// <summary>
        /// 查找场景中的全部角色
        /// </summary>
        internal static Actor[] FindAllActorsInScene() {
            return GameObject.FindObjectsOfType<Actor>();
        }

    }
}