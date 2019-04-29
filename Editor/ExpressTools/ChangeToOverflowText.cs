using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BToolkit
{
    public class ChangeToOverflowText
    {

        [MenuItem("BToolkit/Change To/OverflowText")]
        static void Execute()
        {
            GameObject[] objs = Selection.gameObjects;
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请在场景中选择包含Text组件的对象", "确定");
                return;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                SetFileProperties(objs[i]);
            }
        }

        static void SetFileProperties(GameObject obj)
        {
            Text text = obj.GetComponent<Text>();
            if (!text)
            {
                text = obj.AddComponent<Text>();
            }
            text.rectTransform.sizeDelta = Vector2.zero;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.fontSize = 38;
            text.color = new Color(0.2f, 0.2f, 0.2f);
            text.supportRichText = false;
            text.raycastTarget = false;
        }
    }
}