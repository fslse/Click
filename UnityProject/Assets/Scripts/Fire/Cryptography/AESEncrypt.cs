using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Scripts.Fire.Cryptography
{
    public static class AESEncrypt
    {
        private static readonly byte[] key = Encoding.UTF8.GetBytes("`qlst@eclick&!^uni*tyt_est#78fse"); // 32 bytes
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("4oat+les2o=@w0m#"); // 16 bytes

        public static byte[] Encrypt(byte[] originalValue)
        {
            if (originalValue == null)
            {
                throw new ArgumentNullException(nameof(originalValue));
            }

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(originalValue, 0, originalValue.Length);
                cs.FlushFinalBlock();
            }

            return ms.ToArray();
        }

        public static byte[] Decrypt(byte[] encryptedValue)
        {
            if (encryptedValue == null)
            {
                throw new ArgumentNullException(nameof(encryptedValue));
            }

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(encryptedValue, 0, encryptedValue.Length);
                cs.FlushFinalBlock();
            }

            return ms.ToArray();
        }
    }
}