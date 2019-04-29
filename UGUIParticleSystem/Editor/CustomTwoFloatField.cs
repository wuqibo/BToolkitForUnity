using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class CustomTwoFloatField
    {
        public static void FloatField(string title, ref float value1,ref float value2)
        {
            EditorGUILayout.LabelField(title);
            Rect lastRect = GUILayoutUtility.GetLastRect();
            lastRect.width -= 40;
            Rect rect = lastRect;
            rect.x = lastRect.width * 0.45f;
            if(rect.x < 135)
            {
                rect.x = 135;
            }
            float fieldWidth = lastRect.width - rect.x + 54;
            rect.width = fieldWidth * 0.5f - 5;
            value1 = EditorGUI.FloatField(rect,value1);
            rect.x += rect.width + 10;
            value2 = EditorGUI.FloatField(rect,value2);
        }
    }
}