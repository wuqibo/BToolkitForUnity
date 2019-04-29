using UnityEngine;
using System.Collections;
using BToolkit;
using System;

namespace BToolkit {
    public class Skill_Demo:Skill {

        Actor target;
        string[] targetIds;

        public Skill_Demo() : base(Type.Skill) { }

        protected override void OnStart() {
            PlaySkillAnim(true);
            Actor[] actors = Actor.FindAllActorsInScene();
            for(int i = 0;i < actors.Length;i++) {
                if(actors[i].actorProperties.Team != actor.actorProperties.Team) {
                    target = actors[i];
                    targetIds = new string[] { target.actorNetwork.networkObject.netId };
                }
            }
            if(actor.HasAuthority) {
                ActorCombat.Effect effect0 = config.effects[0];
                if(effect0.particle) {
                    BTimer.Invoke(effect0.spawnDelay, BroadcastSpawnEffect, 0);
                }
                ActorCombat.Effect effect1 = config.effects[1];
                if(effect1.particle) {
                    BTimer.Invoke(effect1.spawnDelay, BroadcastSpawnEffect, 1);
                }
            }
        }

        void BroadcastSpawnEffect(int effectIndex) {
            if(actor && !actor.HasDie) {
                HurtData hurtData = new HurtData();
                hurtData.damage = actor.actorProperties.Atk;
                BroadcastPlayOrSpawnEffect(effectIndex,actor.trans.forward,targetIds,hurtData);
            }
        }

        protected override void OnInterrupt() {
        }

        protected override void OnFinish() {
        }

        protected override void ClientSpawnEffect(int effectIndex,Vector3 shootDirection,string[] targetNetIds,HurtData hurtData) {
            targetIds = targetNetIds;
            Actor[] actors = Actor.FindNetActorsByIds(targetNetIds);
            if(actors.Length > 0) {
                target = actors[0];
            }
            ActorCombat.Effect effect = GetEffectByIndex(effectIndex);
            if(effect.particle) {
                Vector3 pos = effect.createPoint ? effect.createPoint.position : actor.HurtPos;
                GameObject go = GameObject.Instantiate(effect.particle,pos,Quaternion.identity) as GameObject;
                if(effect.lifeTime > 0f) {
                    GameObject.Destroy(go,effect.lifeTime);
                }
                if(effect.sounds.Count > 0) {
                    SoundPlayer.Play(0,effect.sounds[UnityEngine.Random.Range(0,effect.sounds.Count)]);
                }
                Bullet bullet = go.AddComponent<Bullet_Demo>();
                bullet.SetInfo(actor,shootDirection,target,config.effects[2],null,18,15,hurtData);
                canInterrput = true;
            }
        }

        protected override void OnUpdate() {
        }

        protected override void OnClientPlayBodyEffect(int effectIndex,Vector3 shootDirection,string[] targetNetIds,HurtData hurtData) {
        }
    }
}