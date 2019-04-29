using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class CreateBButton
    {

        [MenuItem("BToolkit/Create/BButton")]
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
            GameObject go = new GameObject("Btn");
            if (obj)
            {
                go.transform.SetParent(obj.transform, false);
            }
            go.AddComponent<BButton>();
            go.AddComponent<ButtonChange>();
        }
    }
}