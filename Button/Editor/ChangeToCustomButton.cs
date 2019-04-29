using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BToolkit
{
    public class ChangeToCustomButton
    {

        [MenuItem("BToolkit/Change To/Custom Button")]
        static void Execute()
        {
            GameObject[] objs = Selection.gameObjects;
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请在场景中选择包含Button组件的对象", "确定");
                return;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                SetFileProperties(objs[i]);
            }
        }

        static void SetFileProperties(GameObject obj)
        {
            Button button = obj.GetComponent<Button>();
            if (!button)
            {
                button = obj.AddComponent<Button>();
            }
            button.transition = Selectable.Transition.None;
            if (!obj.GetComponent<Image>())
            {
                obj.AddComponent<Image>();
            }
            if (!obj.GetComponent<ButtonChange>())
            {
                obj.AddComponent<ButtonChange>();
            }
        }
    }
}