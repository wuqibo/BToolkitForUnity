using System.Security.Cryptography;
using System.Text;

namespace BToolkit
{
    public class MD5Utils
    {

        public static string GetMD5_32(string inputStr, bool upperCase = true)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
            return GetMD5_32(inputBytes, upperCase);
        }

        public static string GetMD5_32(byte[] inputBytes, bool upperCase = true)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                if (upperCase)
                {
                    sb.Append(hash[i].ToString("X2"));//字母大写
                }
                else
                {
                    sb.Append(hash[i].ToString("x2"));//字母小写
                }
            }
            return sb.ToString();
        }
    }
}