using UnityEngine;
using System.Collections;
using System;

namespace BToolkit {
    public class Bullet_Demo:Bullet {

        internal override void SetInfo(Actor shooter,Vector3 shootDirection,Actor target,ActorCombat.Effect hitEffect,ActorCombat.Effect endEffect,float flySpeed,float atkRange,HurtData hurtData) {
            base.SetInfo(shooter,shootDirection,target,hitEffect,endEffect,flySpeed,atkRange,hurtData);
            //发射前写点啥
        }

        protected override void OnFlyUpdate(Config config) {
            transform.position += config.shootDirection * config.flySpeed * Time.deltaTime;
            float dis = Vector3.Distance(config.spawnPos,transform.position);
            if(dis >= config.atkRange) {
                this.enabled = false;
                Destroy(gameObject,0.2f);
            }
        }

        protected override void OnTriggerEnter(Collider other) {
            Actor otherActor = other.GetComponent<Actor>();
            if(otherActor && otherActor.actorProperties.Team != config.shooterTeam) {
                CreateHitEffectOnClient();
                this.enabled = false;
                Destroy(gameObject,0.2f);
            }
        }
    }
}