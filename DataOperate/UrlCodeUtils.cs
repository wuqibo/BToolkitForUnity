using UnityEngine;

namespace BToolkit
{
    public class UrlCodeUtils
    {

        /// <summary>
        /// 将文本转为Url编码
        /// </summary>
        public static string UrlEncode(string str)
        {
            return WWW.EscapeURL(str);
        }

        /// <summary>
        /// 将Url编码转成普通文本
        /// </summary>
        public static string UrlDecode(string str)
        {
            return WWW.UnEscapeURL(str);
        }
    }
}