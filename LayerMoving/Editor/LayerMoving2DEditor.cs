using UnityEngine;
using UnityEditor;
using System.Collections;

namespace BToolkit
{
    [CustomEditor(typeof(LayerMoving2D))]
    public class LayerMoving2DEditor : Editor
    {

        SerializedObject sceneryLayerMove;
        SerializedProperty isMainOfCurrLayer, layerIndex, backLayerMainObj, closely, randomPos, randomPosXMin, randomPosXMax, randomScale, randomScaleMin, randomScaleMax, backLayerSpeed;

        void OnEnable()
        {
            sceneryLayerMove = new SerializedObject(target);
            isMainOfCurrLayer = sceneryLayerMove.FindProperty("isMainOfCurrLayer");
            layerIndex = sceneryLayerMove.FindProperty("layerIndex");
            closely = sceneryLayerMove.FindProperty("closely");
            backLayerMainObj = sceneryLayerMove.FindProperty("backLayerMainObj");
            randomPos = sceneryLayerMove.FindProperty("randomPos");
            randomPosXMin = sceneryLayerMove.FindProperty("randomPosXMin");
            randomPosXMax = sceneryLayerMove.FindProperty("randomPosXMax");
            randomScale = sceneryLayerMove.FindProperty("randomScale");
            randomScaleMin = sceneryLayerMove.FindProperty("randomScaleMin");
            randomScaleMax = sceneryLayerMove.FindProperty("randomScaleMax");
            backLayerSpeed = sceneryLayerMove.FindProperty("backLayerSpeed");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(layerIndex);
            EditorGUILayout.PropertyField(isMainOfCurrLayer);
            if (isMainOfCurrLayer.boolValue)
            {
                EditorGUILayout.PropertyField(closely);
                EditorGUILayout.PropertyField(backLayerMainObj);
                if (backLayerMainObj.objectReferenceValue)
                {
                    EditorGUILayout.PropertyField(backLayerSpeed);
                }
                if (!closely.boolValue)
                {
                    EditorGUILayout.PropertyField(randomPos);
                    if (randomPos.boolValue)
                    {
                        EditorGUILayout.PropertyField(randomPosXMin);
                        EditorGUILayout.PropertyField(randomPosXMax);
                    }
                    EditorGUILayout.PropertyField(randomScale);
                    if (randomScale.boolValue)
                    {
                        EditorGUILayout.PropertyField(randomScaleMin);
                        EditorGUILayout.PropertyField(randomScaleMax);
                    }
                }
            }
            sceneryLayerMove.ApplyModifiedProperties();
        }
    }
}