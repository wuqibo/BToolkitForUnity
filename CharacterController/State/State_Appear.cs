using UnityEngine;
using System.Collections;
using System;

namespace BToolkit {
    internal class State_Appear:State {

        internal State_Appear(Actor actor)
            : base(actor) {
        }

        internal override void OnEnter() {
            PlayAnim("appear",0);
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