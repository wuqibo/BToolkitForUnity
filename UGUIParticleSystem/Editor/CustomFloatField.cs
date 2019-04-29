using UnityEngine;
using UnityEditor;

namespace BToolkit.UGUIParticle
{
    public class CustomFloatField
    {
        public static FloatValue FloatField(string title, FloatValue value)
        {
            EditorGUILayout.LabelField(title);
            Rect lastRect = GUILayoutUtility.GetLastRect();
            lastRect.width -= 40;
            if (value.type == ValueType.Single)
            {
                Rect rect = lastRect;
                rect.x = lastRect.width * 0.45f;
                if (rect.x < 135)
                {
                    rect.x = 135;
                }
                rect.width = lastRect.width - rect.x + 5;
                value.single = EditorGUI.FloatField(rect, value.single);
                if(value.single < 0f)
                {
                    value.single = 0f;
                }
                rect = lastRect;
                rect.x = lastRect.width + 10;
                rect.width = 50f;
                value.type = (ValueType)EditorGUI.EnumPopup(rect, value.type);
            }
            else
            {
                Rect rect = lastRect;
                rect.x = lastRect.width * 0.45f;
                if (rect.x < 135)
                {
                    rect.x = 135;
                }
                float fieldWidth = lastRect.width - rect.x + 5;
                rect.width = fieldWidth * 0.5f - 5;
                value.min = EditorGUI.FloatField(rect, value.min);
                if(value.min < 0f)
                {
                    value.min = 0f;
                }
                rect.x += rect.width + 10;
                value.max = EditorGUI.FloatField(rect, value.max);
                if(value.max < 0f)
                {
                    value.max = 0f;
                }
                rect = lastRect;
                rect.x = lastRect.width + 10;
                rect.width = 50f;
                value.type = (ValueType)EditorGUI.EnumPopup(rect, value.type);
            }
            return value;
        }
    }
}