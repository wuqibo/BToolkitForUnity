using UnityEngine;
using UnityEditor;
using System.IO;

namespace BToolkit
{
    public class ChangeToSprite
    {

        [MenuItem("BToolkit/Change To/Sprite")]
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
            texture.textureType = TextureImporterType.Sprite;
            texture.mipmapEnabled = false;
            texture.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#else
            texture.textureType = TextureImporterType.Sprite;
            texture.mipmapEnabled = false;
            texture.textureCompression = TextureImporterCompression.Uncompressed;
#endif
            AssetDatabase.ImportAsset(path);
        }
    }
}