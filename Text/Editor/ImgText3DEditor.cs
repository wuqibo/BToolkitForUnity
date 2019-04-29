using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    [CustomEditor(typeof(ImgText3D))]
    public class ImgText3DEditor : Editor
    {
        ImgText3D text3D;
        int activeIndex;
        Material material;

        void OnEnable()
        {
            text3D = (ImgText3D)target;
        }

        public override void OnInspectorGUI()
        {
            text3D.space = EditorGUILayout.FloatField("Space", text3D.space);
            text3D.center = EditorGUILayout.Toggle("Center", text3D.center);
            text3D.colorProperty = EditorGUILayout.TextField("Color Property", text3D.colorProperty);
            text3D.texProperty = EditorGUILayout.TextField("Tex Property", text3D.texProperty);
            text3D.texScale = EditorGUILayout.FloatField("Tex Scale", text3D.texScale);
            int length = text3D.zeroToNine.Length;
            for (int i = 0; i < length; i++)
            {
                ImgText3D.TextureConfig config = text3D.zeroToNine[i];
                bool originActive = config.active;
                config.active = EditorGUILayout.Foldout(config.active, "Num " + i);
                if (!originActive && config.active)
                {
                    activeIndex = i;
                }
                if (config.active)
                {
                    config.offset = EditorGUILayout.FloatField("    Offset", config.offset);
                }
            }
            int extraCount = text3D.extras.Count;
            for (int i = 0; i < extraCount; i++)
            {
                ImgText3D.Extra extra = text3D.extras[i];
                bool originActive = extra.config.active;
                extra.config.active = EditorGUILayout.Foldout(extra.config.active, "Extra " + extra.text);
                if (!originActive && extra.config.active)
                {
                    activeIndex = length + i;
                }
                if (extra.config.active)
                {
                    extra.text = EditorGUILayout.TextField("    Text", extra.text);
                    extra.config.offset = EditorGUILayout.FloatField("    Offset", extra.config.offset);
                }
            }
            if (GUILayout.Button("Add Extra"))
            {
                text3D.extras.Add(new ImgText3D.Extra());
            }

            //确保只开当前配置
            for (int i = 0; i < length; i++)
            {
                text3D.zeroToNine[i].active = (activeIndex == i);
            }
            for (int i = 0; i < extraCount; i++)
            {
                text3D.extras[i].config.active = (activeIndex == length + i);
            }

            //当前编辑的对象
            ImgText3D.TextureConfig currConfig = null;
            if (activeIndex < length)
            {
                currConfig = text3D.zeroToNine[activeIndex];
            }
            else
            {
                currConfig = text3D.extras[activeIndex - length].config;
            }
            if (!material)
            {
                material = text3D.GetComponent<MeshRenderer>().sharedMaterial;
            }
            if (material)
            {
                material.SetTextureScale(text3D.texProperty, new Vector2(text3D.texScale * 0.1f, 1));
                material.SetTextureOffset(text3D.texProperty, new Vector2(currConfig.offset * 0.1f, 0));
            }
            else
            {
                Debug.LogError("material为空");
            }
        }
    }
}