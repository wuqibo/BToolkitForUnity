using UnityEngine;
using System.Collections.Generic;
using BToolkit.P2PNetwork;

namespace BToolkit {
    public class ActorState: MonoBehaviour {

        public enum StateEnum {
            None,
            Appear,
            Idle,
            IdleBreak,
            Move,
            Attack,
            Hurt,
            Cheer,
            Die
        }
        internal Dictionary<StateEnum,State> states = new Dictionary<StateEnum,State>();
        internal State currState;
        internal StateEnum currStateEnum = StateEnum.Idle;
        Actor actor;

        void Awake() {
            actor = GetComponent<Actor>();
        }

        void Update() {
            if(currState != null) {
                currState.OnUpdate();
                //Debug.Log(">>>>>>>>> currState: " + currStateEnum);
            }
        }

        /// <summary>
        /// 尝试通过网络通道切换角色状态
        /// </summary>
        internal void SwitchToState(StateEnum toStateEnum) {
            if(!actor) {
                return;
            }
            if(actor.actorNetwork && P2PNetwork.P2PNetwork.Instance.IsConnected) {
                actor.actorNetwork.SwitchToState(toStateEnum);
            } else {
                SwitchToStateOnClient(toStateEnum);
            }
        }
        /// <summary>
        /// 不通过网络切换角色状态
        /// </summary>
        internal void SwitchToStateOnClient(StateEnum toStateEnum) {
            //Debug.Log(actor.actorNetwork.hasAuthority + ">>>>>>>>> SwitchToStateOnClient: " + toStateEnum);
            if(currStateEnum == StateEnum.Cheer) {
                return;
            }
            if(currStateEnum == StateEnum.Die) {
                return;
            }
            if(toStateEnum == StateEnum.Idle || toStateEnum == StateEnum.Appear) {
                if(actor.actorCombat.currSkill != null && !actor.actorCombat.currSkill.canInterrput) {
                    return;
                }
            }
            State newState = null;
            if(states.ContainsKey(toStateEnum)) {
                newState = states[toStateEnum];
            } else {
                switch(toStateEnum) {
                    case StateEnum.Appear:
                        newState = new State_Appear(actor);
                        break;
                    case StateEnum.Idle:
                        newState = new State_Idle(actor);
                        break;
                    case StateEnum.Move:
                        newState = new State_Move(actor);
                        break;
                    case StateEnum.Attack:
                        newState = new State_Attack(actor);
                        break;
                    case StateEnum.Hurt:
                        newState = new State_Hurt(actor);
                        break;
                    case StateEnum.Cheer:
                        newState = new State_Cheer(actor);
                        break;
                    case StateEnum.Die:
                        newState = new State_Die(actor);
                        break;
                }
                states.Add(toStateEnum,newState);
            }
            if(currState != null) {
                currState.OnExit();
            }
            currStateEnum = toStateEnum;
            currState = newState;
            currState.OnEnter();
        }

    }
}