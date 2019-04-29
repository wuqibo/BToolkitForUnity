using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace BToolkit {
    [CustomEditor(typeof(ActorCombat))]
    public class ActorCombatEditor:Editor {

        ActorCombat actorCombat;
        Color defaultColor;

        void OnEnable() {
            actorCombat = (ActorCombat)target;
            defaultColor = GUI.color;
        }

        public override void OnInspectorGUI() {
            for(int i = 0;i < actorCombat.skillConfigs.Count;i++) {
                OneSkill(actorCombat.skillConfigs[i],i);
                GUI.color = new Color(0.5f,0.5f,0.5f);
                EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------");
                GUI.color = defaultColor;
            }
            EditorGUILayout.Space();
            if(GUILayout.Button("  Add New Skill  ")) {
                actorCombat.skillConfigs.Add(new ActorCombat.SkillConfig());
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------");
            actorCombat.hurtPoint = (Transform)EditorGUILayout.ObjectField("Hurt Point",actorCombat.hurtPoint,typeof(Transform),true);
            EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------");
            Words(actorCombat.randomWords,"Random Word");
            EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------");
            Words(actorCombat.winWords,"Win Word");
            EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------");
            Words(actorCombat.dieWords,"Die Word");
        }

        void OneSkill(ActorCombat.SkillConfig skillConfig,int skillIndex) {
            EditorGUILayout.BeginHorizontal();
            skillConfig.openEdit = EditorGUILayout.Foldout(skillConfig.openEdit,skillIndex + ":" + skillConfig.skillName);
            if(GUILayout.Button("  Remove Skill")) {
                if(EditorUtility.DisplayDialog("提示","确定要移除该技能吗?","确定","取消")) {
                    actorCombat.skillConfigs.RemoveAt(skillIndex);
                }
            }
            if(GUILayout.Button("Play")) {
                if(!Application.isPlaying) {
                    string remind = "注：\n\n1.启动场后因无网络，角色会自动隐藏，直接点击技能Play即可显示。";
                    if(EditorUtility.DisplayDialog("提示","必须先启动场景，是否现在启动?\n\n" + remind,"启动场景")) {
                        EditorApplication.isPlaying = true;
                    }
                    return;
                }
                if(!actorCombat.gameObject.activeInHierarchy) {
                    actorCombat.gameObject.SetActive(true);
                }
                actorCombat.InitSkills();
                if(string.IsNullOrEmpty(skillConfig.skillId)) {
                    actorCombat.GetComponentInChildren<Animator>().Play(skillConfig.animName,0,0f);
                } else {
                    actorCombat.ExecuteSkillForEditorMode(skillConfig.skillId,skillIndex);
                }
            }
            EditorGUILayout.EndHorizontal();
            if(skillConfig.openEdit) {
                EditorGUILayout.Space();
                skillConfig.skillName = EditorGUILayout.TextField("    Skill Name",skillConfig.skillName);
                skillConfig.skillId = EditorGUILayout.TextField("    Skill ID",skillConfig.skillId);
                skillConfig.animName = EditorGUILayout.TextField("    Anim Name",skillConfig.animName);
                EditorGUILayout.Space();
                for(int i = 0;i < skillConfig.effects.Count;i++) {
                    ActorCombat.Effect effect = skillConfig.effects[i];
                    EditorGUILayout.BeginHorizontal();
                    effect.openEdit = EditorGUILayout.Foldout(effect.openEdit,"    [" + i + "]:" + effect.name);
                    if(GUILayout.Button("Remove Effect")) {
                        if(EditorUtility.DisplayDialog("提示","确定要移除该特效吗?","确定","取消")) {
                            skillConfig.effects.RemoveAt(i);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if(effect.openEdit) {
                        if(string.IsNullOrEmpty(effect.name)) {
                            effect.name = "Effect";
                        }
                        effect.name = EditorGUILayout.TextField("        Name",effect.name);
                        effect.particle = (GameObject)EditorGUILayout.ObjectField("        Particle",effect.particle,typeof(GameObject),true);
                        if(effect.particle) {
                            ParticleSystem particleSystem = effect.particle.GetComponentInChildren<ParticleSystem>();
                            if(particleSystem) {
                                if(effect.particle.transform.parent) {
                                    if(particleSystem.main.playOnAwake) {
                                        particleSystem.playOnAwake = false;
                                    }
                                } else {
                                    if(!particleSystem.main.playOnAwake) {
                                        particleSystem.playOnAwake = true;
                                    }
                                }
                            }
                            TrailRenderer trailRenderer = effect.particle.GetComponentInChildren<TrailRenderer>();
                            if(trailRenderer) {
                                if(effect.particle.transform.parent) {
                                    if(trailRenderer.enabled) {
                                        trailRenderer.enabled = false;
                                    }
                                } else {
                                    if(!trailRenderer.enabled) {
                                        trailRenderer.enabled = true;
                                    }
                                }
                            }
                        }
                        effect.createPoint = (Transform)EditorGUILayout.ObjectField("        Position",effect.createPoint,typeof(Transform),true);
                        effect.spawnDelay = EditorGUILayout.FloatField("        Spawn Delay",effect.spawnDelay);
                        effect.lifeTime = EditorGUILayout.FloatField("        Life Time",effect.lifeTime);
                        effect.flySpeed = EditorGUILayout.FloatField("        Fly Speed",effect.flySpeed);
                        Words(effect.sounds,"        Sound");
                    }
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("");
                if(GUILayout.Button("Add Effect")) {
                    skillConfig.effects.Add(new ActorCombat.Effect());
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                Words(skillConfig.words,"    Skill Word");
            }
        }

        void Words(List<AudioClip> wordsList,string name) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            for(int i = 0;i < wordsList.Count;i++) {
                EditorGUILayout.BeginHorizontal();
                wordsList[i] = (AudioClip)EditorGUILayout.ObjectField(name + " " + (i + 1),wordsList[i],typeof(AudioClip),true);
                if(GUILayout.Button("-")) {
                    if(wordsList.Count > 1) {
                        wordsList.RemoveAt(i);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            if(GUILayout.Button("+")) {
                wordsList.Add(null);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}