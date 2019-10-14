using UnityEngine;

namespace BToolkit.ActorCtrl
{
    [RequireComponent(typeof(SphereCollider))]
    public abstract class Bullet : MonoBehaviour
    {
        public float flySpeed = 25;
        public AudioClip hitSound;

        protected Actor attacker;
        protected Actor[] targets;
        protected int attackerTeam;
        protected float atkRange;
        protected HurtData hurtData;
        protected Vector3 createPos, actorAngle;
        protected SphereCollider sphereCollider;

        protected virtual void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.center = Vector3.zero;
            sphereCollider.radius = 0.5f;
        }

        public virtual void SetInfo(Actor attacker, Vector3 actorAngle, Actor[] targets, float atkRange, HurtData hurtData)
        {
            this.attacker = attacker;
            this.actorAngle = actorAngle;
            this.targets = targets;
            this.atkRange = atkRange;
            this.hurtData = hurtData;
            this.attackerTeam = attacker.actorProperties.Team;
            this.createPos = transform.position;
        }

        protected virtual void Update()
        {
            OnFlyUpdate(attacker, actorAngle, targets, atkRange, hurtData);
        }

        protected abstract void OnFlyUpdate(Actor attacker, Vector3 actorAngle, Actor[] targets, float atkRange, HurtData hurtData);

        protected virtual void OnTriggerEnter(Collider other)
        {
            SoundPlayer.PlayAndDestroy(0, hitSound);
        }

    }
}