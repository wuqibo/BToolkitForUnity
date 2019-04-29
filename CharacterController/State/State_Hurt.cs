using UnityEngine;
using System.Collections;
using System;

namespace BToolkit {
    internal class State_Hurt:State {

        internal State_Hurt(Actor actor)
            : base(actor) {
        }

        internal override void OnEnter() {
            //PlayAnim("hurt",0);
            actor.actorState.SwitchToStateOnClient(ActorState.StateEnum.Idle);//没有Hurt动画的时候临时处理
        }

        internal override void OnExit() {
        }

        internal override void OnUpdate() {
            AnimatorStateInfo mAnimatorStateInfo = actor.animator.GetCurrentAnimatorStateInfo(0);
            if(mAnimatorStateInfo.IsName(animName)) {
                if(mAnimatorStateInfo.normalizedTime > 0.95f) {
                    actor.actorState.SwitchToStateOnClient(ActorState.StateEnum.Idle);
                }
            }
        }
    }
}