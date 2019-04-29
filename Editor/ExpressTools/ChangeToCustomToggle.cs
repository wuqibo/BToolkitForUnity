using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BToolkit
{
    public class ChangeToCustomToggle
    {

        [MenuItem("BToolkit/Change To/Custom Toggle")]
        static void Execute()
        {
            GameObject[] objs = Selection.gameObjects;
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请在场景中选择包含Toggle组件的对象", "确定");
                return;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                SetFileProperties(objs[i]);
            }
        }

        static void SetFileProperties(GameObject obj)
        {
            Toggle toggle = obj.GetComponent<Toggle>();
            if (toggle)
            {
                (toggle.transform as RectTransform).pivot = new Vector2(0, 0.5f);
                (toggle.transform as RectTransform).sizeDelta = new Vector2(250, 70);

                RectTransform bg = toggle.transform.Find("Background") as RectTransform;
                bg.anchorMin = new Vector2(0, 0.5f);
                bg.anchorMax = new Vector2(0, 0.5f);
                bg.sizeDelta = new Vector2(60, 60);
                bg.pivot = new Vector2(0, 0.5f);
                bg.anchoredPosition = Vector2.zero;

                RectTransform checkmark = bg.Find("Checkmark") as RectTransform;
                checkmark.anchorMin = new Vector2(0, 0);
                checkmark.anchorMax = new Vector2(1, 1);
                checkmark.sizeDelta = Vector2.zero;

                Text label = toggle.transform.Find("Label").GetComponent<Text>();
                label.rectTransform.anchorMin = new Vector2(0, 0.5f);
                label.rectTransform.anchorMax = new Vector2(0, 0.5f);
                label.rectTransform.anchoredPosition = new Vector2(70, 0);
                label.rectTransform.sizeDelta = Vector2.zero;
                label.fontSize = 42;
                label.alignment = TextAnchor.MiddleLeft;
                label.horizontalOverflow = HorizontalWrapMode.Overflow;
                label.verticalOverflow = VerticalWrapMode.Overflow;
            }
            else
            {
                Debuger.LogError("所选对象不是一个Toggle");
            }
        }
    }
}