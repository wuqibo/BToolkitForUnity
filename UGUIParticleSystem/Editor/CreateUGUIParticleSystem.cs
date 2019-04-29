using UnityEngine;
using UnityEditor;

namespace BToolkit.UGUIParticle
{
    public class CreateUGUIParticleSystem
    {

        [MenuItem("BToolkit/Create/UGUIParticleSystem")]
        static void Execute()
        {
            GameObject[] objs = Selection.gameObjects;
            if (objs.Length != 1)
            {
                EditorUtility.DisplayDialog("提示", "请在场景中选择父级", "确定");
                return;
            }
            if(!objs[0].GetComponent<RectTransform>())
            {
                EditorUtility.DisplayDialog("提示","必须选择一个UGUI对象","确定");
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
            GameObject go = new GameObject("UGUIParticleSystem");
            if (obj)
            {
                go.transform.SetParent(obj.transform, false);
            }
            go.AddComponent<UGUIParticleSystem>();
        }
    }
}