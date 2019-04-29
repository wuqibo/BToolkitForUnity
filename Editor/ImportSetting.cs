using UnityEngine;
using UnityEditor;

namespace BToolkit
{
    public class ImportSetting : AssetPostprocessor
    {

        //资源导入时自动设置好

        void OnPreprocessModel()
        {
            //ModelImporter modelImporter = (ModelImporter)assetImporter;
            //modelImporter.animationType = ModelImporterAnimationType.Legacy;
        }

        //TextureImporter textureImporter;
        void OnPreprocessTexture()
        {
            //TextureImporter textureImporter = (TextureImporter)assetImporter;
            //textureImporter.textureType = TextureImporterType.Sprite;
            //textureImporter.mipmapEnabled = false;
            //textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        }

        //AudioImporter audioImporter;
        void OnPreprocessAudio()
        {
            //audioImporter = (AudioImporter)assetImporter;
            //audioImporter.threeD = false;
        }
    }
}