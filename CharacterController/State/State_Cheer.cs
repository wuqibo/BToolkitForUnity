using UnityEngine;
using System.Collections;
using System;

namespace BToolkit {
    internal class State_Cheer:State {

        internal State_Cheer(Actor actor)
            : base(actor) {
        }

        internal override void OnEnter() {
            PlayAnim("win",0);
        }

        internal override void OnExit() {
        }

        internal override void OnUpdate() {
        }
    }
}