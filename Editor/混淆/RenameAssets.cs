using System.IO;
using UnityEditor;
#if UNITY_IOS
using UnityEngine;
#endif

namespace BToolkit
{
    public class RenameAssets
    {

        [MenuItem("BToolkit/混淆/重命名所有静态资源")]
        public static void ToRename()
        {
#if UNITY_IOS
            ToRenameByPath(Application.dataPath + "/Prefabs/MainControl.prefab");
            ToRenameByPath(Application.dataPath + "/Prefabs/NGUIRoot.prefab");
#else
            EditorUtility.DisplayDialog("警告", "仅iOS平台需要重命名静态资源", "确定", "取消");
#endif
        }

        static void ToRenameByPath(string fullPath)
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
                        if (CanRename(dependenciesPath))
                        {
                            string newName = MD5Utils.GetMD5_32(dependenciesPath+GetAppInfo.GetAppPackageName());
                            AssetDatabase.RenameAsset(dependenciesPath, newName);
                        }
                    }
                }
                else if (CanRename(fullPath))
                {
                    string newName = MD5Utils.GetMD5_32(fullPath);
                    AssetDatabase.RenameAsset(fullPath, newName);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static bool CanRename(string filePath)
        {
            if (filePath.EndsWith(".png") ||
                filePath.EndsWith(".jpg") ||
                filePath.EndsWith(".mp3") ||
                filePath.EndsWith(".wav") ||
                filePath.EndsWith(".FBX") ||
                filePath.EndsWith(".mat") ||
                filePath.EndsWith(".prefab"))
            {
                return true;
            }
            return false;
        }
    }
}