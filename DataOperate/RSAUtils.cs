using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace BToolkit
{
    public class RSAUtils
    {

        //常见报错：（input data too large）当使用1024的key时，往往不能加密太长的字符串，改成2048的key即可。

        //在线RSA密钥生成器https://rsatool.org/
        private const string PublicPemKey =
    @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAukCxUaStoQdcnONPCSNN
Yby9F7ZQtcKtqCdHv3Evy6GMT9+Ruh/RztP5Q8ZBOq4P1BlsXpKaDBGaCRdyVbDk
AXEbkvL2ZC21sb1KZo3CqVbnjjiCcd+PjeeH+AQZJ0vM4oUaP9yr6CZ2/LJYKoIQ
4tkjiGQXYpIDzls8gU5hkfBzrz4M6dZZ1R4EHwfsb7rZpcACrdLKcJHYw3sAPtVa
a3C0Eunnlf6rFK/LFYPcIVxrGELSB1VaPPLJE3tMwW0LJquCo7NrQQuQ03s7fuqW
euboDDj0u40A9CA8B+2CqNcjsKxXlYOyG8VKOrr0XVPGuiWVyon/dU5VCn2tA6po
xwIDAQAB
-----END PUBLIC KEY-----";
        private const string PrivatePemKey = "";

        public static string EncryptWithPublicPemKey(string content)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(content);
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            using (var txtreader = new StringReader(PublicPemKey))
            {
                var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();
                encryptEngine.Init(true, keyParameter);
            }
            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;
        }

        public static string EncryptWithPrivatePemKey(string content)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(content);
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            using (var txtreader = new StringReader(PrivatePemKey))
            {
                var keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();
                encryptEngine.Init(true, keyPair.Private);
            }
            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;
        }

        public static string DecryptWithPrivatePemKey(string content)
        {
            var bytesToDecrypt = Convert.FromBase64String(content);
            AsymmetricCipherKeyPair keyPair;
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());
            using (var txtreader = new StringReader(PrivatePemKey))
            {
                keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();
                decryptEngine.Init(false, keyPair.Private);
            }
            var decrypted = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
            return decrypted;
        }

        public static string DecryptWithPublicPemKey(string content)
        {
            var bytesToDecrypt = Convert.FromBase64String(content);
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());
            using (var txtreader = new StringReader(PublicPemKey))
            {
                var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();
                decryptEngine.Init(false, keyParameter);
            }
            var decrypted = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
            return decrypted;
        }
    }
}