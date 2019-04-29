using UnityEngine;

namespace BToolkit {
    public class ReadStreamAssetsPathSyn {

        /// <summary>
        /// 用同步方式（非WWW方式）读取Android原生工程的assets目录（即Unity的StreamingAssets目录）,文件带后缀名。
        /// </summary>
        public static byte[] GetBytes(string filePath)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                try {
                    byte[] bytes = AndroidUtils.CallAndroidStaticFunction<byte[]>("cn.btoolkit.readstreamassetspathsyn.ReadStreamAssetsPathSyn", "getBytes", filePath);
                    if (bytes!=null && bytes.Length > 0)
                    {
                        return bytes;
                    }
                } catch{}
                return null;
            }
            else
            {
                Debuger.LogError("本方法仅限Android平台");
                return null;
            }
        }
    }
}