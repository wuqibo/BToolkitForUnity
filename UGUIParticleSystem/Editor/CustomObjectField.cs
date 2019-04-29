using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class CustomObjectField
    {
        public static T ObjectField<T>(string title,Object obj) where T : Object
        {
            Rect position = EditorGUILayout.GetControlRect(false,16f);
            return (T)EditorGUI.ObjectField(position,title,obj,typeof(T),true);
        }
    }
}