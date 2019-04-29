using UnityEngine;
using UnityEditor;

namespace BToolkit.UGUIParticle
{
    public class CustomColorField
    {
        public static ColorValue ColorField(string title, ColorValue value)
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
                value.single = EditorGUI.ColorField(rect, value.single);
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
                value.min = EditorGUI.ColorField(rect, value.min);
                rect.x += rect.width + 10;
                value.max = EditorGUI.ColorField(rect, value.max);
                rect = lastRect;
                rect.x = lastRect.width + 10;
                rect.width = 50f;
                value.type = (ValueType)EditorGUI.EnumPopup(rect, value.type);
            }
            return value;
        }
    }
}