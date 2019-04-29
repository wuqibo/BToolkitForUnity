using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class ChangeToUICamera
    {

        [MenuItem("BToolkit/Change To/UICamera")]
        static void Execute()
        {
            GameObject[] objs = Selection.gameObjects;
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请在场景中选择包含Camera组件的对象", "确定");
                return;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                SetFileProperties(objs[i]);
            }
        }

        static void SetFileProperties(GameObject obj)
        {
            Camera camera = obj.GetComponent<Camera>();
            if (camera)
            {
                camera.transform.localPosition = new Vector3(0, 0, -100);
                camera.clearFlags = CameraClearFlags.Depth;
                camera.orthographic = true;
                camera.nearClipPlane = 0.3f;
                camera.farClipPlane = 110;
                GameObject.DestroyImmediate(camera.GetComponent<FlareLayer>());
                GameObject.DestroyImmediate(camera.GetComponent<GUILayer>());
                GameObject.DestroyImmediate(camera.GetComponent<AudioListener>());
            }
            else
            {
                Debuger.LogError("所选对象不是一个Camera");
            }
        }
    }
}