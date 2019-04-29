using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class CustomFolder
    {

        public static bool FoldoutTitle(string title, bool display)
        {
            //定义样式
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.fontSize = 9;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 25f;
            style.contentOffset = new Vector2(5, -2f);
            //画出样式
            var rect = GUILayoutUtility.GetRect(0f, 25f, style);
            GUI.Box(rect, title, style);

            //处理布尔值
            var e = Event.current;
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }
            return display;
        }

        public static bool Foldout(string title, bool display, ref bool isUse)
        {
            //定义样式
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 20f;
            style.contentOffset = new Vector2(20f, -2f);
            //画出样式
            var rect = GUILayoutUtility.GetRect(0f, 20f, style);
            GUI.Box(rect, title, style);

            //处理布尔值
            Rect iconRect = new Rect(rect.x + 4, rect.y, 20, 20);
            var e = Event.current;
            if (e.type == EventType.Repaint)
            {
                //画出Toggle
                EditorStyles.toggle.Draw(iconRect, false, true, isUse, false);
            }
            if (e.type == EventType.MouseDown)
            {
                if (iconRect.Contains(e.mousePosition))
                {
                    isUse = !isUse;
                    e.Use();
                }
                else if (rect.Contains(e.mousePosition))
                {
                    display = !display;
                    e.Use();
                }
            }
            return display;
        }
    }
}