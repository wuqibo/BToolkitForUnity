using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    [CustomEditor(typeof(ImgText))]
    public class ImgTextEditor : Editor
    {

        ImgText text;

        void OnEnable()
        {
            text = (ImgText)target;
        }

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.TextField("Text", "播放模式下编辑预览");
            }
            else
            {
                text.text = EditorGUILayout.TextField("Text", text.text);
            }
            base.OnInspectorGUI();
        }
    }
}