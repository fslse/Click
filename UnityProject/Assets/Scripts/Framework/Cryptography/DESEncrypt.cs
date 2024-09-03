using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Framework.Cryptography;

public static class DESEncrypt
{
    private const string key = "qtel&#78";
    private const string iv = "x03642!@";

    public static byte[] Encrypt(byte[] originalValue)
    {
        if (originalValue == null)
        {
            throw new ArgumentNullException(nameof(originalValue));
        }

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, new DESCryptoServiceProvider
               {
                   Key = Encoding.UTF8.GetBytes(key),
                   IV = Encoding.UTF8.GetBytes(iv),
                   Mode = CipherMode.CBC
               }.CreateEncryptor(), CryptoStreamMode.Write))
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

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, new DESCryptoServiceProvider
               {
                   Key = Encoding.UTF8.GetBytes(key),
                   IV = Encoding.UTF8.GetBytes(iv),
                   Mode = CipherMode.CBC
               }.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cs.Write(encryptedValue, 0, encryptedValue.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }
}