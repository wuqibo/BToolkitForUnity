using UnityEngine;
using System.Collections;

namespace BToolkit {
    internal abstract class State {

        protected Actor actor;
        protected string animName;

        internal State(Actor actor) {
            this.actor = actor;
        }

        internal abstract void OnExit();
        internal abstract void OnUpdate();
        internal abstract void OnEnter();

        protected void PlayAnim(string animName,float transitionDuration) {
            this.animName = animName;
            actor.PlayAnim(animName,transitionDuration);
        }
    }
}