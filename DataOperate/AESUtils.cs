using System.Security.Cryptography;
using System.Text;
using System;

namespace BToolkit
{
    public class AESUtils
    {
        const string KEY = "12wsdfcvbghjnm,kjuyhgfde456789iu";

        /// <summary>
        /// 加密(key为32位字符串)
        /// </summary>
        public static string Encrypt(string toEncrypt, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = KEY;
            }
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            byte[] resultArray = Encrypt(toEncryptArray, key);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// 解密(key为32位加密时使用的字符串)
        /// </summary>
        public static string Decrypt(string toDecrypt, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = KEY;
            }
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            byte[] resultArray = Decrypt(toEncryptArray, key);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 加密(key为32位字符串)
        /// </summary>
        public static byte[] Encrypt(byte[] toEncrypt, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = KEY;
            }
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);
            return resultArray;
        }

        /// <summary>
        /// 解密(key为32位加密时使用的字符串)
        /// </summary>
        public static byte[] Decrypt(byte[] toDecrypt, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = KEY;
            }
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toDecrypt, 0, toDecrypt.Length);
            return resultArray;
        }
    }
}