using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BToolkit
{
    public class ChangeToImgText
    {

        [MenuItem("BToolkit/Change To/ImgText")]
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
            Text text = obj.GetComponent<Text>();
            if (text)
            {
                MonoBehaviour.DestroyImmediate(text);
            }
            if (!obj.GetComponent<ImgText>())
            {
                obj.AddComponent<ImgText>();
            }
        }
    }
}