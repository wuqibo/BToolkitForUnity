using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class FindMissingScripts
    {

        static int go_count = 0, components_count = 0, missing_count = 0;

        [MenuItem("BToolkit/Find Missing Scripts")]
        static void Execute()
        {
            GameObject[] go = Selection.gameObjects;
            if (go.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择一个Prefab", "确定");
                return;
            }
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
            if (missing_count > 0)
            {
                Debuger.LogError(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
            }
            else
            {
                Debuger.Log(string.Format("Searched {0} GameObjects, {1} components, NO missing", go_count, components_count));
            }
        }

        static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debuger.LogError(s + " has an empty script attached in position: " + i, g);
                }
            }
            foreach (Transform childT in g.transform)
            {
                FindInGO(childT.gameObject);
            }
        }
    }
}