using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    [CustomEditor(typeof(Scroll))]
    public class ScrollEditor : Editor
    {

        SerializedObject scroll;
        SerializedProperty type;
        SerializedProperty itemSize, colSpace, rowSpace, pageSpace, rowCount, colCount, pageCount;
        SerializedProperty endLimit;
        SerializedProperty dragSpeed;

        void OnEnable()
        {
            scroll = new SerializedObject(target);
            type = scroll.FindProperty("type");
            itemSize = scroll.FindProperty("itemSize");
            colSpace = scroll.FindProperty("colSpace");
            rowSpace = scroll.FindProperty("rowSpace");
            pageSpace = scroll.FindProperty("pageSpace");
            rowCount = scroll.FindProperty("rowCount");
            colCount = scroll.FindProperty("colCount");
            pageCount = scroll.FindProperty("pageCount");
            endLimit = scroll.FindProperty("endLimit");
            dragSpeed = scroll.FindProperty("dragSpeed");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(type);
            EditorGUILayout.PropertyField(itemSize);
            if (type.intValue == (int)Scroll.Type.Page)
            {
                EditorGUILayout.PropertyField(colSpace);
                EditorGUILayout.PropertyField(rowSpace);
                EditorGUILayout.PropertyField(pageSpace);
                EditorGUILayout.PropertyField(colCount);
                EditorGUILayout.PropertyField(rowCount);
                EditorGUILayout.PropertyField(pageCount);
            }
            else if (type.intValue == (int)Scroll.Type.FreeHorizontal)
            {
                EditorGUILayout.PropertyField(colSpace);
                EditorGUILayout.PropertyField(endLimit);
            }
            else if (type.intValue == (int)Scroll.Type.FreeVertical)
            {
                EditorGUILayout.PropertyField(colSpace);
                EditorGUILayout.PropertyField(rowSpace);
                EditorGUILayout.PropertyField(colCount);
                EditorGUILayout.PropertyField(endLimit);
            }
            EditorGUILayout.PropertyField(dragSpeed);
            scroll.ApplyModifiedProperties();
        }

    }
}