using UnityEngine;
using System.Collections.Generic;
using BToolkit.P2PNetwork;

namespace BToolkit {
    public class ActorCombat: MonoBehaviour {

        [System.Serializable]
        public class Effect {
            public string name = "";
            public bool openEdit = true;
            public GameObject particle;
            public Transform createPoint;
            public float spawnDelay = 0.5f;
            public float lifeTime = 2f;
            public float flySpeed = 15;
            public List<AudioClip> sounds = new List<AudioClip>() { null };
        }
        [System.Serializable]
        public class SkillConfig {
            internal Actor actor;
            internal int index;
            public bool openEdit = true;
            public string skillName = "Skill Name";
            public string skillId;
            public string animName;
            public List<Effect> effects = new List<Effect>();
            public List<AudioClip> words = new List<AudioClip>() { null };
        }
        public List<SkillConfig> skillConfigs = new List<SkillConfig>();
        public List<AudioClip> randomWords = new List<AudioClip>() { null };
        public List<AudioClip> dieWords = new List<AudioClip>() { null };
        public List<AudioClip> winWords = new List<AudioClip>() { null };
        public Transform hurtPoint;
        internal Transform HurtPoint {
            get { return hurtPoint; }
        }
        internal Vector3 HurtPos {
            get { return hurtPoint ? hurtPoint.position : actor.trans.position + new Vector3(0,actor.capsuleCollider.height * 0.5f,0); }
        }
        Skill[] skills;
        internal Skill currSkill;
        SkillManager skillManager;
        Actor actor;
        internal AudioSource wordSoundPlayer;

        void Awake() {
            actor = GetComponent<Actor>();
        }

        void OnDisable() {
            if(currSkill != null) {
                currSkill.Interrupt();
                currSkill = null;
            }
        }

        void OnDestroy() {
            if(skillManager != null) {
                skillManager.Destroy();
            }
        }

        internal void SetSkillManager(SkillManager skillManager) {
            this.skillManager = skillManager;
        }

        public void InitSkills() {
            skills = new Skill[skillConfigs.Count];
            for(int i = 0;i < skills.Length;i++) {
                skillConfigs[i].index = i;
                skillConfigs[i].actor = actor;
            }
        }

        void Update() {
            if(currSkill != null) {
                currSkill.Update();
            }
        }

        /// <summary>
        /// 尝试通过网络通道释放技能,自动读取Inspector面板上指定的Skill ID
        /// </summary>
        internal void ExecuteSkill(int skillIndex) {
            ExecuteSkill(skillConfigs[skillIndex].skillId,skillIndex);
        }

        /// <summary>
        /// 尝试通过网络通道释放技能,选择Inspector面板上的哪个技能索引由Skill程序指定
        /// </summary>
        internal void ExecuteSkill(string skillId) {
            ExecuteSkill(skillId,-1);
        }
        /// <summary>
        /// 尝试通过网络通道释放技能,当skillIndex小于0时,表示索引由Skill程序指定
        /// </summary>
        internal void ExecuteSkill(string skillId,int skillIndex) {
            //public修饰，以方便ActorCombatEditor访问
            if(actor.actorNetwork && P2PNetwork.P2PNetwork.Instance.IsConnected) {
                actor.actorNetwork.ExecuteSkill(skillId,skillIndex,false);
            } else {
                ExecuteSkillOnClient(skillId,skillIndex,false);
            }
        }
        /// <summary>
        /// 仅Editor模式可调用。尝试通过网络通道释放技能,skillIndex为当前点击的SkillConfig的索引，忽略Skill程序定义的skillIndex
        /// </summary>
        public void ExecuteSkillForEditorMode(string skillId,int skillIndex) {
            //public修饰，以方便ActorCombatEditor访问
            if(actor.actorNetwork && P2PNetwork.P2PNetwork.Instance.IsConnected) {
                actor.actorNetwork.ExecuteSkill(skillId,skillIndex,true);
            } else {
                ExecuteSkillOnClient(skillId,skillIndex,true);
            }
        }
        /// <summary>
        /// 直接在客户端执行技能释放,当skillIndex小于0时,表示索引由Skill程序指定
        /// </summary>
        internal void ExecuteSkillOnClient(string skillId,int skillIndex,bool forEditorMode) {
            if(actor.actorState.currStateEnum == ActorState.StateEnum.Cheer) {
                return;
            }
            if(actor.actorProperties.HasDie) {
                return;
            }
            if(currSkill != null) {
                if(!currSkill.canInterrput) {
                    return;
                } else {
                    currSkill.Interrupt();
                }
            }
            if(skillIndex >= skills.Length) {
                Debug.LogError("角色仅配置了 " + skills.Length + " 个技能,索引 " + skillIndex + " 超出范围");
                return;
            }
            if(skillManager == null) {
                Debug.LogError("未设置SkillManager，请创建SkillManager的子类，并在Actor的子类的Awake()中调用actorCombat.SetSkillManager()进行传入。");
                return;
            }
            currSkill = skillManager.GetSkill(skillId);
            if(currSkill != null) {
                currSkill.Start(actor,skillIndex,forEditorMode);
            }
        }

        /// <summary>
        /// 尝试通过网络通道广播并在客户端回调OnTargetsSettedOnClient函数
        /// </summary>
        internal void BroadcastSkillParams(int parame) {
            if(actor.actorNetwork && P2PNetwork.P2PNetwork.Instance.IsConnected) {
                actor.actorNetwork.BroadcastSkillParams(parame);
            } else {
                currSkill.SetParamsOnClient(parame);
            }
        }

        /// <summary>
        /// 尝试通过网络通道创建特效，广播后客户端将执行ClientPlayOrSpawnEffect()方法
        /// </summary>
        internal void PlayOrSpawnEffect(int effectIndex,Vector3 shootDirection,string[] targetNetIds,HurtData hurtData) {
            if(currSkill != null) {
                if(actor.actorNetwork && P2PNetwork.P2PNetwork.Instance.IsConnected) {
                    actor.actorNetwork.PlayOrSpawnEffect(effectIndex,shootDirection,targetNetIds,hurtData);
                } else {
                    currSkill.PlayOrSpawnEffectOnClient(effectIndex,shootDirection,targetNetIds,hurtData);
                }
            } else {
                Debug.LogWarning("currSkill已被销毁，可能您配置了动画结束时同时结束技能");
            }
        }

        internal void PlayRandomWord() {
            if(randomWords.Count > 0) {
                PlayWordSound(randomWords[Random.Range(0,randomWords.Count)],false);
            }
        }

        internal void PlayWinWord() {
            if(winWords.Count > 0) {
                PlayWordSound(winWords[Random.Range(0,winWords.Count)],true);
            }
        }

        internal void PlayDieWord() {
            if(dieWords.Count > 0) {
                PlayWordSound(dieWords[Random.Range(0,dieWords.Count)],true);
            }
        }

        /// <summary>
        /// 台词播放函数，没有共用SoundPlayer.Play()，以确保不会有重复的台词一起播放,当reliable传入true时，暂停当前台词之后播放新台词，否则播放器在闲置时才播放。
        /// </summary>
        internal void PlayWordSound(AudioClip clip,bool reliable) {
            if(clip) {
                if(!SoundPlayer.IsSoundOn) {
                    return;
                }
                if(!wordSoundPlayer) {
                    wordSoundPlayer = gameObject.AddComponent<AudioSource>();
                    wordSoundPlayer.spatialBlend = 0f;
                    wordSoundPlayer.playOnAwake = false;
                    wordSoundPlayer.loop = false;
                }
                wordSoundPlayer.enabled = true;
                if(reliable) {
                    wordSoundPlayer.Stop();
                } else {
                    if(wordSoundPlayer.isPlaying) {
                        return;
                    }
                }
                wordSoundPlayer.clip = clip;
                wordSoundPlayer.Play();
            }
        }

    }
}