using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class AssetBundleEditor : EditorWindow
    {
#if UNITY_5

        [MenuItem("BToolkit/Export AssetBundle")]
        static void ExportAssetBundle()
        {
            EditorWindow.GetWindow<AssetBundleEditor>("Export");
        }

        string assetBundleName = "assets_android";
        string remind = "";
        Color remindColor;
        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("1. Select the resources you need to export!");
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            assetBundleName = EditorGUILayout.TextField("2. AssetBundleName", assetBundleName);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export For Android"))
            {
                Export(assetBundleName, BuildTarget.Android);
            }
            if (GUILayout.Button("Export For IOS"))
            {
                Export(assetBundleName, BuildTarget.iOS);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.color = remindColor;
            EditorGUILayout.LabelField(remind);
        }

        void Export(string assetBundleName, BuildTarget buildTarget)
        {
            remind = "";
            if (string.IsNullOrEmpty(assetBundleName))
            {
                remindColor = Color.red;
                remind = "Please Input assetBundle Name.";
                return;
            }
            Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            int count = selectedAsset.Length;
            if (count > 0)
            {
                string[] selectedAssetPaths = new string[count];
                for (int i = 0; i < count; i++)
                {
                    selectedAssetPaths[i] = AssetDatabase.GetAssetOrScenePath(selectedAsset[i]);
                }
                string[] allDependenciesPaths = AssetDatabase.GetDependencies(selectedAssetPaths);
                AssetBundleBuild[] builds = new AssetBundleBuild[1];
                builds[0].assetBundleName = assetBundleName + ".zip";
                builds[0].assetNames = allDependenciesPaths;
                string outPutPath = EditorUtility.SaveFolderPanel("Save AssetBundle", "", "");
                BuildPipeline.BuildAssetBundles(outPutPath, builds, BuildAssetBundleOptions.None, buildTarget);
                remindColor = Color.green;
                remind = "Export successful! Upload the zip files to your server.";
                Debug.Log("Export AssetBundle By Selected Finished");
            }
        }

#else

[MenuItem("BToolkit/Export/Export AssetBunlde For Android")]
    static void ExportJiMuAssetBunldesForAndroid() {
        string targetPath = EditorUtility.SaveFilePanel("Save Android Assets", "", "assetbundle", "zip");
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object obj in SelectedAsset) {
            Debug.Log("Exporting AssetBunldes : " + obj.name);
        }
        if (BuildPipeline.BuildAssetBundle(null, SelectedAsset, targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.Android)) {
            AssetDatabase.Refresh();
            Debug.Log("Has Exported In : " + targetPath);
        }
    }

    [MenuItem("BToolkit/Export/Export AssetBunlde For iOS")]
    static void ExportJiMuAssetBunldesForIOS() {
        string targetPath = EditorUtility.SaveFilePanel("Save IOS Assets", "", "assetbundle", "zip");
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object obj in SelectedAsset) {
            Debug.Log("Exporting AssetBunldes : " + obj.name);
        }
        if (BuildPipeline.BuildAssetBundle(null, SelectedAsset, targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.iPhone)) {
            AssetDatabase.Refresh();
            Debug.Log("Has Exported In : " + targetPath);
        }
    }

    [MenuItem("BToolkit/Export/Export Scene For iOS")]
    static void ExportRunningCarSceneIOS() {
        string targetPath = EditorUtility.SaveFilePanel("Save Scene", "", "unity3d", "zip");
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        string[] paths = new string[SelectedAsset.Length];
        int i = 0;
        foreach (Object obj in SelectedAsset) {
            string originalSceneNPath = AssetDatabase.GetAssetPath(obj);
            paths[i] = originalSceneNPath;
            i++;
        }
        BuildPipeline.BuildPlayer(paths, targetPath, BuildTarget.iPhone, BuildOptions.BuildAdditionalStreamedScenes);
        AssetDatabase.Refresh();
        Debug.Log("Has Exported In :" + targetPath);
    }

    [MenuItem("BToolkit/Export/Export Scene For Android")]
    static void ExportRunningCarSceneAndroid() {
        string targetPath = EditorUtility.SaveFilePanel("Save Scene", "", "unity3d", "zip");
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        string[] paths = new string[SelectedAsset.Length];
        int i = 0;
        foreach (Object obj in SelectedAsset) {
            string originalSceneNPath = AssetDatabase.GetAssetPath(obj);
            paths[i] = originalSceneNPath;
            i++;
        }
        BuildPipeline.BuildPlayer(paths, targetPath, BuildTarget.Android, BuildOptions.BuildAdditionalStreamedScenes);
        AssetDatabase.Refresh();
        Debug.Log("Has Exported In :" + targetPath);
    }

#endif
    }
}