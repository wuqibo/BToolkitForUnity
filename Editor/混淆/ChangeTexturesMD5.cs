using System.IO;
using UnityEditor;
using UnityEngine;

namespace BToolkit
{
    public class ChangeTexturesMD5
    {
        [MenuItem("BToolkit/混淆/改变所有图片资源MD5值")]
        public static void ToChangeMD5()
        {
#if UNITY_IOS
            ToChangeMD5ByPath(Application.dataPath + "/Prefabs/MainControl.prefab");
            ToChangeMD5ByPath(Application.dataPath + "/Prefabs/NGUIRoot.prefab");
#else
            EditorUtility.DisplayDialog("警告", "仅iOS平台需要重命名静态资源", "确定", "取消");
#endif
        }

        static void ToChangeMD5ByPath(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                fullPath = fullPath.Remove(0, fullPath.IndexOf("Assets"));
                if (fullPath.EndsWith(".prefab"))
                {
                    string[] allDependenciesPaths = AssetDatabase.GetDependencies(fullPath);
                    for (int j = 0; j < allDependenciesPaths.Length; j++)
                    {
                        string dependenciesPath = allDependenciesPaths[j];
                        if (CanChange(dependenciesPath))
                        {
                            TextureImporter textureImporter = AssetImporter.GetAtPath(dependenciesPath) as TextureImporter;
                            if (!textureImporter.isReadable)
                            {
                                textureImporter.isReadable = true;
                                AssetDatabase.ImportAsset(dependenciesPath);
                            }
                            Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(dependenciesPath);
                            //Debuger.Log("MD5:" + MD5Utils.GetMD5_32(texture2D.EncodeToPNG()));
                            //Debuger.Log("Hash:" + texture2D.GetHashCode());
                            Texture2D newTexture2D = TextureUtils.ChangeMD5(texture2D);
                            File.WriteAllBytes(dependenciesPath, newTexture2D.EncodeToPNG());
                            //Debuger.Log(">>>>>MD5:" + MD5Utils.GetMD5_32(texture2D.EncodeToPNG()));
                            //Debuger.Log(">>>>>Hash:" + texture2D.GetHashCode());
                        }
                    }
                }
                else if (CanChange(fullPath))
                {
                    Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);
                    Texture2D newTexture2D = TextureUtils.ChangeMD5(texture2D);
                    File.WriteAllBytes(fullPath, newTexture2D.EncodeToPNG());
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static bool CanChange(string filePath)
        {
            if (filePath.EndsWith(".png") ||
                filePath.EndsWith(".jpg") ||
                filePath.EndsWith(".tga") ||
                filePath.EndsWith(".psd"))
            {
                return true;
            }
            return false;
        }
    }
}