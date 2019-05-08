using System.Runtime.InteropServices;
using UnityEngine;

namespace BToolkit
{
    public class SavePhotoToAlbum
    {

        /// <summary>
        /// 保存到相册，Android需指定路径并会自动刷新相册，iOS将不会使用该路径
        /// </summary>
        public static void Save(byte[] bytes, string androidSaveFullPath = null)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidUtils.CallAndroidStaticFunction("cn.btoolkit.savephoto.SavePhotoToAlbum", "save", bytes, androidSaveFullPath);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _SaveImgToPhotosAlbum(bytes, bytes.Length);
            }
        }


        /// <summary>
        /// Andriod新保存到相册的照片需要刷新相册后才能显示（Save()方法会自动刷新）
        /// </summary>
        public static void AndroidRefreshAlbum(string fileFullPath)
        {
            AndroidUtils.CallAndroidStaticFunction("cn.btoolkit.savephoto.SavePhotoToAlbum", "refreshAlbum", fileFullPath);
        }


        [DllImport("__Internal")]
        static extern void _SaveImgToPhotosAlbum(byte[] bytes, int bytesLength);

    }
}