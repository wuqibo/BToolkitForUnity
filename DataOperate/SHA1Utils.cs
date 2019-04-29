using System;
using System.Security.Cryptography;
using System.Text;

namespace BToolkit
{
    public class SHA1Utils
    {

        public static string HashCode(string str, bool upperCase = true)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(str);
            byte[] dataHashed = SHA1.Create().ComputeHash(dataToHash);
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");
            if (upperCase)
            {
                return hash;
            }
            else
            {
                return hash.ToLower();
            }
        }
    }
}