using UnityEngine;
using System.Collections;
using BToolkit.P2PNetwork;

namespace BToolkit {
    internal class State_Move:State {

        float animatorSpeed;

        internal State_Move(Actor actor)
            : base(actor) {
        }

        internal override void OnEnter() {
            animatorSpeed = actor.animator.speed;
            PlayAnim("move",0.2f);
        }

        internal override void OnUpdate() {
            if(actor.actorNetwork) {
                if(P2PNetwork.P2PNetwork.Instance.IsConnected && !actor.actorNetwork.networkObject.isLocal) {
                    return;
                }
            }
            float angle = -actor.joystickRadian * 180f / Mathf.PI + 90;
            actor.trans.rotation = Quaternion.Euler(0,angle,0);
            actor.trans.Translate(Vector3.forward * actor.actorProperties.MoveSpeed * Time.deltaTime);
            actor.animator.speed = actor.actorProperties.MoveSpeed / actor.actorProperties.MoveSpeedNormal;
        }

        internal override void OnExit() {
            actor.animator.speed = animatorSpeed;
        }

    }
}