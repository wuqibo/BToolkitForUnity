using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class GetPhoto
    {
        /// <summary>
        /// 拍一张照片并返回图片地址，若地址为空，则表示获取照片失败
        /// </summary>
        public static void GetPhotoFromCamera(string callbackGo, string callbackFun)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.getphoto.GetPhoto", "getPhotoFromCamera", callbackGo, callbackFun);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                string tempSavePath = Application.persistentDataPath + "/temp.jpg";
                _GetPhotoFromCamera(tempSavePath, callbackGo, callbackFun);
            }
        }

        /// <summary>
        /// 从相册选择一张照片并返回图片地址，若地址为空，则表示获取照片失败
        /// </summary>
        public static void GetPhotoFromAlbums(string callbackGo, string callbackFun)
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                string path = UnityEditor.EditorUtility.OpenFilePanelWithFilters("选择照片", "", new string[] { "图片", "jpg,jpeg,png" });
                GameObject.Find(callbackGo).SendMessage(callbackFun, path);
#endif
            }
            else
            {
                string tempSavePath = Application.persistentDataPath + "/temp.jpg";
                if (Application.platform == RuntimePlatform.Android)
                {
                    AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.getphoto.GetPhoto", "getPhotoFromAlbum", tempSavePath, callbackGo, callbackFun);
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _GetPhotoFromAlbums(tempSavePath, callbackGo, callbackFun);
                }
            }
        }

        /// <summary>
        /// 仅iOS
        /// </summary>
        public static void GetPhotoFromLibrary(string saveFullPath, string callbackGo, string callbackFun)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                //安卓没有FromLibrary操作
                Debuger.LogError("该操作仅iOS有效");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _GetPhotoFromLibrary(saveFullPath, callbackGo, callbackFun);
            }
        }

        [DllImport("__Internal")]
        static extern void _GetPhotoFromCamera(string saveFullPath, string callbackGo, string callbackFun);

        [DllImport("__Internal")]
        static extern void _GetPhotoFromAlbums(string saveFullPath, string callbackGo, string callbackFun);

        [DllImport("__Internal")]
        static extern void _GetPhotoFromLibrary(string saveFullPath, string callbackGo, string callbackFun);
    }
}