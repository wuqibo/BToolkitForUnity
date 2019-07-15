using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BToolkit
{
    public class ChangeToStateButton
    {

        [MenuItem("BToolkit/Change To/StateButton")]
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
            if (!obj.GetComponent<StateButton>())
            {
                obj.AddComponent<StateButton>();
            }
            if (!obj.transform.Find("OnState"))
            {
                GameObject onStateGo = new GameObject("OnState", typeof(RectTransform));
                onStateGo.transform.SetParent(obj.transform, false);
            }
            if (!obj.transform.Find("OffState"))
            {
                GameObject onStateGo = new GameObject("OffState", typeof(RectTransform));
                onStateGo.transform.SetParent(obj.transform, false);
            }
            if (!obj.GetComponent<ButtonChange>())
            {
                obj.AddComponent<ButtonChange>();
            }
        }
    }
}