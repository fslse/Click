using System.IO;
using System.Linq;
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
        public static string ComputeHash<T>(string filePath) where T : System.Security.Cryptography.HashAlgorithm
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open);
            using T hash = System.Activator.CreateInstance<T>();
            byte[] retVal = hash.ComputeHash(fs);
            return ZString.Join("", retVal.Select(v => v.ToString("x2")));
        }
    }
}