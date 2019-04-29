using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    [CustomEditor(typeof(Fish))]
    public class FishEditor : Editor
    {

        Fish fish;
        SerializedObject fishPro;
        SerializedProperty limitMax, limitMin, canMove, canRotate, canSway, ctrlBone, idle, chase;

        void OnEnable()
        {
            fish = (Fish)target;
            fishPro = new SerializedObject(target);
            limitMax = fishPro.FindProperty("limitMax");
            limitMin = fishPro.FindProperty("limitMin");
            canMove = fishPro.FindProperty("canMove");
            canRotate = fishPro.FindProperty("canRotate");
            canSway = fishPro.FindProperty("canSway");
            ctrlBone = fishPro.FindProperty("ctrlBone");
            idle = fishPro.FindProperty("idle");
            chase = fishPro.FindProperty("chase");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(limitMax);
            EditorGUILayout.PropertyField(limitMin);
            EditorGUILayout.PropertyField(canMove);
            EditorGUILayout.PropertyField(canRotate);
            EditorGUILayout.PropertyField(canSway);
            EditorGUILayout.PropertyField(ctrlBone);
            fish.dataProgress = EditorGUILayout.Slider("Idle to Chase", fish.dataProgress, 0f, 1f);
            EditorGUILayout.PropertyField(idle, true);
            EditorGUILayout.PropertyField(chase, true);
            fishPro.ApplyModifiedProperties();
        }

    }
}