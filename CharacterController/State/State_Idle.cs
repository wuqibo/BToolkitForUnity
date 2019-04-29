using UnityEngine;
using System.Collections;

namespace BToolkit {
    internal class State_Idle:State {

        internal State_Idle(Actor actor)
            : base(actor) {
        }

        internal override void OnEnter() {
            PlayAnim("idle",0.2f);
            if(Random.Range(0,2) == 0) {
                actor.actorCombat.PlayRandomWord();
            }
        }

        internal override void OnExit() {
        }

        internal override void OnUpdate() {
        }
    }
}