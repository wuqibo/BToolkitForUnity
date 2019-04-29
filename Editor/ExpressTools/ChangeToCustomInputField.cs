using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BToolkit
{
    public class ChangeToCustomInputField
    {

        [MenuItem("BToolkit/Change To/Custom InputField")]
        static void Execute()
        {
            GameObject[] objs = Selection.gameObjects;
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请在场景中选择包含InputField组件的对象", "确定");
                return;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                SetFileProperties(objs[i]);
            }
        }

        static void SetFileProperties(GameObject obj)
        {
            InputField inputField = obj.GetComponent<InputField>();
            if (!inputField)
            {
                EditorUtility.DisplayDialog("提示", "该对象不是InputField组件", "确定");
                return;
            }
            (inputField.transform as RectTransform).sizeDelta = new Vector2(500, 80);
            (inputField.transform as RectTransform).pivot = new Vector2(0, 0.5f);
            foreach (var item in inputField.GetComponentsInChildren<Text>())
            {
                item.fontSize = 42;
                item.alignment = TextAnchor.MiddleCenter;
            }
        }
    }
}