using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace BToolkit {
    [CustomEditor(typeof(HeadBar))]
    public class HeadBarEditor:Editor {

        HeadBar headBar;

        void OnEnable() {
            headBar = (HeadBar)target;
        }

        public override void OnInspectorGUI() {
            headBar.type = (HeadBar.Type)EditorGUILayout.EnumPopup("Type",headBar.type);
            if(headBar.type == HeadBar.Type.WorldSpace) {
                headBar.bg = (Transform)EditorGUILayout.ObjectField("Bg",headBar.bg,typeof(Transform),true);
                headBar.hpBar = (MeshRenderer)EditorGUILayout.ObjectField("HP Bar",headBar.hpBar,typeof(MeshRenderer),true);
                headBar.hpShadow = (MeshRenderer)EditorGUILayout.ObjectField("HP Shadow",headBar.hpShadow,typeof(MeshRenderer),true);
                headBar.mpBar = (MeshRenderer)EditorGUILayout.ObjectField("MP Bar",headBar.mpBar,typeof(MeshRenderer),true);
                headBar.nameText = (TextMesh)EditorGUILayout.ObjectField("Name",headBar.nameText,typeof(TextMesh),true);
                headBar.levelText = (TextMesh)EditorGUILayout.ObjectField("Level",headBar.levelText,typeof(TextMesh),true);
            } else {

            }
            headBar.HPPercent = EditorGUILayout.Slider("HP Percent",headBar.HPPercent,0f,1f);
            headBar.MPPercent = EditorGUILayout.Slider("MP Percent",headBar.MPPercent,0f,1f);
        }
    }
}