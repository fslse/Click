using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Cysharp.Text;

namespace Scripts.Framework.Cryptography
{
    public static class HashHelper
    {
        /// <summary>
        /// 推荐使用SHA256
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ComputeHash<T>(string filePath) where T : HashAlgorithm
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] retVal = (HashAlgorithm.Create(typeof(T).Name) as T)!.ComputeHash(fs);
            return ZString.Join("", retVal.Select(v => v.ToString("x2")));
        }
    }
}