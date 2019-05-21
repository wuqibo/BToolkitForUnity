using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class SavePhotoToAlbum
    {

        /// <summary>
        /// 保存到相册，Android需指定路径并会自动刷新相册，iOS无需指定文件名(iOS无存储路径返回)
        /// </summary>
        public static string Save(byte[] bytes, string androidFileName = null)
        {
            if (Application.isEditor)
            {
                string saveFullPath = Application.persistentDataPath + "/" + androidFileName;
                Storage.SaveBytesToFile(bytes, saveFullPath);
                return saveFullPath;
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                //手机自带存储卡（非SD卡）根目录
                string saveFullPath = "/storage/emulated/0/DCIM/" + androidFileName;
                AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.savephoto.SavePhotoToAlbum", "save", bytes, saveFullPath);
                return saveFullPath;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _SaveImgToPhotosAlbum(bytes, bytes.Length);
            }
            return "";
        }


        /// <summary>
        /// Andriod新保存到相册的照片需要刷新相册后才能显示（Save()方法会自动刷新）
        /// </summary>
        public static void AndroidRefreshAlbum(string fileFullPath)
        {
            AndroidHelper.CallAndroidStaticFunction("cn.btoolkit.savephoto.SavePhotoToAlbum", "refreshAlbum", fileFullPath);
        }


        [DllImport("__Internal")]
        static extern void _SaveImgToPhotosAlbum(byte[] bytes, int bytesLength);

    }
}