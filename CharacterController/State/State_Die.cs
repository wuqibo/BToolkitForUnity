using UnityEngine;
using System.Collections;
using System;

namespace BToolkit {
    internal class State_Die:State {

        internal State_Die(Actor actor)
            : base(actor) {
        }

        internal override void OnEnter() {
            PlayAnim("die",0.2f);
        }

        internal override void OnUpdate() {
        }

        internal override void OnExit() {
        }
        
    }
}