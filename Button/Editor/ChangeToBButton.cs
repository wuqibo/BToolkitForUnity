using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BToolkit
{
    public class ChangeToBButton
    {

        [MenuItem("BToolkit/Change To/BButton")]
        static void Execute()
        {
            GameObject[] objs = Selection.gameObjects;
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请在场景中选择一个对象", "确定");
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
            if (button)
            {
                MonoBehaviour.DestroyImmediate(button);
            }
            if (!obj.GetComponent<BButton>())
            {
                obj.AddComponent<BButton>();
            }
            if (!obj.GetComponent<ButtonChange>())
            {
                obj.AddComponent<ButtonChange>();
            }
        }
    }
}