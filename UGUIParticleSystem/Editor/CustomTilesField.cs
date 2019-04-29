using UnityEngine;
using UnityEditor;

namespace BToolkit.UGUIParticle
{
    public class CustomTilesField
    {
        public static Tiles TilesField(string title,Tiles value)
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
            value.x = EditorGUI.IntField(rect,value.x);
            rect.x += rect.width + 10;
            value.y = EditorGUI.IntField(rect,value.y);
            if(value.x < 1)
            {
                value.x = 1;
            }
            if(value.y < 1)
            {
                value.y = 1;
            }
            return value;
        }
    }
}