using UnityEngine;

namespace BToolkit {
    [RequireComponent(typeof(SphereCollider))]
    public abstract class Bullet: MonoBehaviour {

        public struct Config {
            public Actor shooter;
            public Actor target;
            public int shooterTeam;
            public ActorCombat.Effect hitEffect, endEffect;
            public float flySpeed, atkRange;
            public HurtData hurtData;
            public Vector3 spawnPos, shootDirection;
        }
        protected SphereCollider sphereCollider;
        protected Config config;
        Vector3 previousPos;


        void Awake() {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.center = Vector3.zero;
            sphereCollider.radius = 0.5f;
            previousPos = transform.position;
        }

        internal virtual void SetInfo(Actor shooter,Vector3 shootDirection,Actor target,ActorCombat.Effect hitEffect,ActorCombat.Effect endEffect,float flySpeed,float atkRange,HurtData hurtData) {
            config = new Config();
            config.shooter = shooter;
            config.target = target;
            config.shooterTeam = shooter.actorProperties.Team;
            config.hitEffect = hitEffect;
            config.endEffect = endEffect;
            config.flySpeed = flySpeed;
            config.atkRange = atkRange;
            config.hurtData = hurtData;
            config.spawnPos = transform.position;
            config.shootDirection = shootDirection;
            base.transform.eulerAngles = new Vector3(0, Mathf.Atan2(shootDirection.x, shootDirection.z) * 180f / Mathf.PI,0);
        }

        void Update() {
            OnFlyUpdate(config);
        }

        protected abstract void OnFlyUpdate(Config config);

        protected abstract void OnTriggerEnter(Collider other);

        /// <summary>
        /// 将此函数放在Update()里，子弹将始终Z轴朝向飞行方向
        /// </summary>
        protected void AngleForwardUpdate() {
            if(previousPos != transform.position) {
                transform.rotation = Quaternion.LookRotation(transform.position - previousPos);
                previousPos = transform.position;
            }
        }

        /// <summary>
        /// 创建从SetInfo参数里传进来的hitEffect
        /// </summary>
        protected virtual void CreateHitEffectOnClient() {
            if(config.target) {
                config.target.HurtOnClient(config.shooter,config.hurtData);
            }
            if(config.hitEffect != null) {
                if(config.hitEffect.particle) {
                    GameObject go = Instantiate(config.hitEffect.particle,transform.position,Quaternion.Euler(transform.eulerAngles)) as GameObject;
                    Destroy(go,config.hitEffect.lifeTime);
                }
                if(config.hitEffect.sounds.Count > 0) {
                    SoundPlayer.PlayAndDestroy(0,config.hitEffect.sounds[Random.Range(0,config.hitEffect.sounds.Count)]);
                }
            }
        }

        /// <summary>
        /// 创建从SetInfo参数里传进来的endEffect
        /// </summary>
        protected virtual void CreateEndEffectOnClient() {
            if(config.endEffect.particle) {
                GameObject go = Instantiate(config.endEffect.particle,transform.position,Quaternion.Euler(transform.eulerAngles)) as GameObject;
                Destroy(go,config.endEffect.lifeTime);
            }
            if(config.endEffect.sounds.Count > 0) {
                SoundPlayer.PlayAndDestroy(0,config.endEffect.sounds[Random.Range(0,config.endEffect.sounds.Count)]);
            }
        }
    }
}