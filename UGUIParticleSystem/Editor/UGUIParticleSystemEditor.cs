using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BToolkit.UGUIParticle
{
    [CustomEditor(typeof(UGUIParticleSystem))]
    public class UGUIParticleSystemEditor:Editor
    {
        UGUIParticleSystem particleSystem;

        void OnEnable()
        {
            particleSystem = (UGUIParticleSystem)target;
            EditorApplication.update += particleSystem.UpdateOnEditMode;
        }

        void OnDisable()
        {
            EditorApplication.update -= particleSystem.UpdateOnEditMode;
        }

        void OnDestroy()
        {
            EditorApplication.update -= particleSystem.UpdateOnEditMode;
        }

        public override void OnInspectorGUI()
        {
            if(!particleSystem.hadInitSettings)
            {
                particleSystem.SetParentAndChild();
                particleSystem.trans.sizeDelta = Vector2.zero;
                particleSystem.duration = 1;
                particleSystem.looping = true;
                particleSystem.startDelay = 0;
                particleSystem.startLifetime.single = 1;
                particleSystem.startLifetime.min = 1;
                particleSystem.startLifetime.max = 2;
                particleSystem.startSpeed.single = particleSystem.startSpeed.min = 10;
                particleSystem.startSpeed.max = 20;
                particleSystem.startScale.single = particleSystem.startScale.min = 1f;
                particleSystem.startScale.max = 2f;
                particleSystem.startRotation.single = particleSystem.startRotation.min = 0;
                particleSystem.startRotation.max = 360;
                particleSystem.startColor.single = particleSystem.startColor.min = particleSystem.startColor.max = Color.white;
                particleSystem.emission.isUse = true;
                particleSystem.shape.isUse = true;
                particleSystem.hadInitSettings = true;
                particleSystem.playOnAwake = true;
                particleSystem.maxParticles = 100;
                Dirty();
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Clear"))
            {
                particleSystem.Clear();
            }
            if(!particleSystem.isPlaying)
            {
                if(GUILayout.Button("Play"))
                {
                    particleSystem.Play();
                }
            }
            else
            {
                if(GUILayout.Button("Stop"))
                {
                    particleSystem.Stop();
                }
            }
            EditorGUILayout.EndHorizontal();
            particleSystem.isMainOpen = CustomFolder.FoldoutTitle("UGUI Particle System",particleSystem.isMainOpen);
            if(particleSystem.isMainOpen)
            {
                particleSystem.duration = EditorGUILayout.FloatField("Duration",particleSystem.duration);
                if(particleSystem.duration < 0.01f)
                {
                    particleSystem.duration = 0.01f;
                }
                particleSystem.looping = EditorGUILayout.Toggle("Looping",particleSystem.looping);
                particleSystem.startDelay = EditorGUILayout.FloatField("Start Delay",particleSystem.startDelay);
                if(particleSystem.startDelay < 0f)
                {
                    particleSystem.startDelay = 0f;
                }
                particleSystem.startLifetime = CustomFloatField.FloatField("Start Lifetime",particleSystem.startLifetime);
                particleSystem.startSpeed = CustomFloatField.FloatField("Start Speed",particleSystem.startSpeed);
                particleSystem.startScale = CustomFloatField.FloatField("Start Scale",particleSystem.startScale);
                particleSystem.startRotation = CustomFloatField.FloatField("Start Rotation",particleSystem.startRotation);
                particleSystem.startColor = CustomColorField.ColorField("Start Color",particleSystem.startColor);
                particleSystem.gravityModifier = EditorGUILayout.FloatField("Gravity Modifier",particleSystem.gravityModifier);
                particleSystem.space = (Space)EditorGUILayout.EnumPopup("Simulation Space",particleSystem.space);
                particleSystem.playOnAwake = EditorGUILayout.Toggle("Play On Awake",particleSystem.playOnAwake);
                particleSystem.maxParticles = EditorGUILayout.IntField("Max Particles",particleSystem.maxParticles);
                if(particleSystem.maxParticles < 1)
                {
                    particleSystem.maxParticles = 1;
                }
            }
            //喷发
            particleSystem.emission.isOpen = CustomFolder.Foldout("Emission",particleSystem.emission.isOpen,ref particleSystem.emission.isUse);
            if(particleSystem.emission.isOpen)
            {
                particleSystem.emission.rate = EditorGUILayout.FloatField("Rate",particleSystem.emission.rate);
                if(particleSystem.emission.rate < 0.01f)
                {
                    particleSystem.emission.rate = 0.01f;
                }
            }
            //发射形状
            particleSystem.shape.isOpen = CustomFolder.Foldout("Shape",particleSystem.shape.isOpen,ref particleSystem.shape.isUse);
            if(particleSystem.shape.isOpen)
            {
                particleSystem.shape.type = (ShapeType)EditorGUILayout.EnumPopup("Type",particleSystem.shape.type);
                if(particleSystem.shape.type == ShapeType.Circle)
                {
                    particleSystem.shape.radius = EditorGUILayout.FloatField("Radius",particleSystem.shape.radius);
                    if(particleSystem.shape.radius < 0f)
                    {
                        particleSystem.shape.radius = 0f;
                    }
                    particleSystem.shape.arc = EditorGUILayout.IntSlider("Arc",particleSystem.shape.arc,0,360);
                }
                else
                {
                    particleSystem.shape.size = EditorGUILayout.Vector2Field("Size",particleSystem.shape.size);
                }
            }
            //风
            particleSystem.wind.isOpen = CustomFolder.Foldout("Wind",particleSystem.wind.isOpen,ref particleSystem.wind.isUse);
            if(particleSystem.wind.isOpen)
            {
                particleSystem.wind.type = (ValueType)EditorGUILayout.EnumPopup("Type",particleSystem.wind.type);
                if(particleSystem.wind.type == ValueType.Single)
                {
                    particleSystem.wind.single = EditorGUILayout.Vector2Field("Direction",particleSystem.wind.single);
                }
                else
                {
                    particleSystem.wind.min = EditorGUILayout.Vector2Field("Direction1",particleSystem.wind.min);
                    particleSystem.wind.max = EditorGUILayout.Vector2Field("Direction2",particleSystem.wind.max);
                }
            }
            //颜色
            particleSystem.colorOverLifetime.isOpen = CustomFolder.Foldout("Color over Lifetime",particleSystem.colorOverLifetime.isOpen,ref particleSystem.colorOverLifetime.isUse);
            if(particleSystem.colorOverLifetime.isOpen)
            {
                particleSystem.colorOverLifetime.gradient = CustomGradientField.GradientField("Color",particleSystem.colorOverLifetime.gradient);
            }
            //缩放
            particleSystem.scaleOverLifetime.isOpen = CustomFolder.Foldout("Scale over Lifetime",particleSystem.scaleOverLifetime.isOpen,ref particleSystem.scaleOverLifetime.isUse);
            if(particleSystem.scaleOverLifetime.isOpen)
            {
                particleSystem.scaleOverLifetime.curve = EditorGUILayout.CurveField("Scale",particleSystem.scaleOverLifetime.curve);
            }
            //旋转
            particleSystem.rotationOverLifetime.isOpen = CustomFolder.Foldout("Rotation over Lifetime",particleSystem.rotationOverLifetime.isOpen,ref particleSystem.rotationOverLifetime.isUse);
            if(particleSystem.rotationOverLifetime.isOpen)
            {
                particleSystem.rotationOverLifetime.type = (ValueType)EditorGUILayout.EnumPopup("Type",particleSystem.rotationOverLifetime.type);
                if(particleSystem.rotationOverLifetime.type == ValueType.Single)
                {
                    particleSystem.rotationOverLifetime.single = EditorGUILayout.FloatField("Angular Velocity",particleSystem.rotationOverLifetime.single);
                }
                else
                {
                    CustomTwoFloatField.FloatField("Angular Velocity",ref particleSystem.rotationOverLifetime.min,ref particleSystem.rotationOverLifetime.max);
                }
            }
            //渲染
            particleSystem.mRenderer.isOpen = CustomFolder.FoldoutTitle("Renderer",particleSystem.mRenderer.isOpen);
            if(particleSystem.mRenderer.isOpen)
            {
                particleSystem.mRenderer.textureSheetAnimation = EditorGUILayout.Toggle("TextureSheetAnimation",particleSystem.mRenderer.textureSheetAnimation);
                if(!particleSystem.mRenderer.textureSheetAnimation)
                {
                    particleSystem.mRenderer.sprite = CustomObjectField.ObjectField<Sprite>("Sprite",particleSystem.mRenderer.sprite);
                }
                else
                {
                    particleSystem.mRenderer.fps = EditorGUILayout.IntSlider("FPS",particleSystem.mRenderer.fps,1,30);
                    particleSystem.mRenderer.openArr = EditorGUILayout.Foldout(particleSystem.mRenderer.openArr,"Frames");
                    if(particleSystem.mRenderer.openArr)
                    {
                        Rect rect = EditorGUILayout.GetControlRect(false,16f);
                        rect.x += 12;
                        rect.width -= 30;
                        particleSystem.mRenderer.frameCount = EditorGUI.IntField(rect, "Size", particleSystem.mRenderer.frameCount);
                        if(particleSystem.mRenderer.frameCount != particleSystem.mRenderer.sprites.Length)
                        {
                            particleSystem.mRenderer.sprites = new Sprite[particleSystem.mRenderer.frameCount];
                        }
                        for(int i = 0;i < particleSystem.mRenderer.frameCount;i++)
                        {
                            particleSystem.mRenderer.sprites[i] = CustomObjectField.ObjectField<Sprite>("   Sprite " + i,particleSystem.mRenderer.sprites[i]);
                        }
                    }
                }
                particleSystem.mRenderer.material = CustomObjectField.ObjectField<Material>("Material",particleSystem.mRenderer.material);
            }
            if(GUI.changed)
            {
                Dirty();
            }
        }

        void Dirty()
        {
            Undo.RecordObject(target,"Changed");
            EditorUtility.SetDirty(target);
            if(!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

    }
}