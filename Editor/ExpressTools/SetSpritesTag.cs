using UnityEngine;
using UnityEditor;
using System.IO;

namespace BToolkit
{
    public class SetSpritesTag : EditorWindow
    {
        static SetSpritesTag window;
        static string tagName;

        [MenuItem("BToolkit/Set Sprites Tag")]
        static void OpenWindow()
        {
            window = GetWindow<SetSpritesTag>();
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            tagName = EditorGUILayout.TextField("Tag Name", tagName);
            EditorGUILayout.Space();
            if (GUILayout.Button("OK"))
            {
                Execute();
                window.Close();
            }
        }

        static void Execute()
        {
            Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            if (objs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择文件夹或贴图", "确定");
                return;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                Object obj = objs[i];
                if (obj is DefaultAsset)
                {
                    string dirPath = AssetDatabase.GetAssetPath(obj);
                    foreach (string childPath in Directory.GetFiles(dirPath))
                    {
                        Object child = AssetDatabase.LoadAssetAtPath(childPath, typeof(Texture));
                        if (child)
                        {
                            SetFileProperties(child);
                        }
                    }
                }
                else
                {
                    SetFileProperties(obj);
                }
            }
        }

        static void SetFileProperties(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;
#if UNITY_5_3
            texture.spritePackingTag = tagName;
#endif
            AssetDatabase.ImportAsset(path);
        }
    }
}