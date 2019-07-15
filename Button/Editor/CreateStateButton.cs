using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class CreateStateButton
    {

        [MenuItem("BToolkit/Create/StateButton")]
        static void Execute()
        {
            GameObject[] objs = Selection.gameObjects;
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请在场景中选择父级", "确定");
                return;
            }
            else
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    SetFileProperties(objs[i]);
                }
            }
        }

        static void SetFileProperties(GameObject obj)
        {
            GameObject go = new GameObject("StateBtn");
            if (obj)
            {
                go.transform.SetParent(obj.transform, false);
            }
            go.AddComponent<StateButton>();
            go.AddComponent<ButtonChange>();
            if (!go.transform.Find("OnState"))
            {
                GameObject onStateGo = new GameObject("OnState", typeof(RectTransform));
                onStateGo.transform.SetParent(go.transform, false);
            }
            if (!go.transform.Find("OffState"))
            {
                GameObject onStateGo = new GameObject("OffState", typeof(RectTransform));
                onStateGo.transform.SetParent(go.transform, false);
            }
        }
    }
}