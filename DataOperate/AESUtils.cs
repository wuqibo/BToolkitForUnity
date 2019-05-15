using System.Security.Cryptography;
using System.Text;
using System;

namespace BToolkit
{
    public class AESUtils
    {
        private static string Key
        {
            get { return @"fANqOtuGLXmeKRz0dVctf25fI7wo4o40"; }
        }

        private static string IV
        {
            get { return @"7MAIlToRV5uGDB40"; }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        public static string Encrypt(string plainStr, string key, string iv)
        {
            if (plainStr == null) return null;

            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bIV = Encoding.UTF8.GetBytes(iv);
            byte[] byteArray = Encoding.UTF8.GetBytes(plainStr);

            string encrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流 
                using (MemoryStream mStream = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象  
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流  
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        encrypt = Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch { }
            aes.Clear();

            return encrypt;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="returnNull">加密失败时是否返回 null，false 返回 String.Empty</param>
        public static string Encrypt(string plainStr, bool returnNull = true)
        {
            string encrypt = Encrypt(plainStr, Key, IV);
            return returnNull ? encrypt : (encrypt == null ? String.Empty : encrypt);
        }

        /// <summary>
        /// AES加密（默认key）
        /// </summary>
        public static string AESEncrypt(string plainStr)
        {
            return Encrypt(plainStr, Key, IV);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        public static string Decrypt(string encryptStr, string key, string iv)
        {
            if (encryptStr == null) return null;

            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bIV = Encoding.UTF8.GetBytes(iv);
            byte[] byteArray = Convert.FromBase64String(encryptStr);

            string decrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流，存储密文  
                using (MemoryStream mStream = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象  
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        // 明文存储区
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
            catch { }
            aes.Clear();

            return decrypt;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="returnNull">解密失败时是否返回 null，false 返回 String.Empty</param>
        public static string Decrypt(string encryptStr, bool returnNull = true)
        {
            string decrypt = Decrypt(encryptStr, Key, IV);
            return returnNull ? decrypt : (decrypt == null ? String.Empty : decrypt);
        }

        /// <summary>
        /// AES解密（默认key）
        /// </summary>
        public static string Decrypt(string plainStr)
        {
            return Decrypt(plainStr, Key, IV);
        }
    }
}