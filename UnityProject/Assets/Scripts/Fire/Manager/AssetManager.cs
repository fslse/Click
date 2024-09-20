using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using Scripts.Fire.Singleton;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Fire.Manager
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        private AssetBundleManifest abManifest;
        private readonly Dictionary<string, AssetBundle> loadedAssetBundles = new();
        private readonly Dictionary<string, Object> loadedAssets = new();

        /// <summary>
        /// Get AssetBundleManifest.
        /// </summary>
        public async UniTask Initialize()
        {
            // 主包
            AssetBundle ab = await LoadAssetBundleAsync(AppConst.AssetsDir);
            abManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] abs = abManifest.GetAllAssetBundles();
            GameLog.LogDebug($"AssetManager.Instance.Initialize()\n{ZString.Join('\n', abs)}");
        }

        /// <summary>
        /// 异步加载AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private async UniTask<AssetBundle> LoadAssetBundleAsync(string abName)
        {
            if (loadedAssetBundles.TryGetValue(abName, out var bundle))
                return bundle;

            string path = AppConst.PersistentDataPath + abName;
            if (!File.Exists(path)) // PersistentData目录不存在该包
                path = AppConst.StreamingAssetsPath + abName;

            // 主包直接加载并返回
            if (abName == AppConst.AssetsDir)
                return await AssetBundle.LoadFromFileAsync(path);

            // 加载目标包
            AssetBundle ab = await AssetBundle.LoadFromFileAsync(path);
            loadedAssetBundles.Add(abName, ab);

            // 加载依赖包
            string[] dependencies = abManifest.GetAllDependencies(abName);
            foreach (string dependency in dependencies)
                if (!loadedAssetBundles.ContainsKey(dependency))
                    await LoadAssetBundleAsync(dependency);

            return ab;
        }

        /// <summary>
        /// 同步加载AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string abName)
        {
            if (loadedAssetBundles.TryGetValue(abName, out var bundle))
                return bundle;

            string path = AppConst.PersistentDataPath + abName;
            if (!File.Exists(path)) // PersistentData目录不存在该包
                path = AppConst.StreamingAssetsPath + abName;

            // 主包直接加载并返回
            if (abName == AppConst.AssetsDir)
                return AssetBundle.LoadFromFile(path);

            // 加载目标包
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            loadedAssetBundles.Add(abName, ab);

            // 加载依赖包
            string[] dependencies = abManifest.GetAllDependencies(abName);
            foreach (string dependency in dependencies)
                if (!loadedAssetBundles.ContainsKey(dependency))
                    LoadAssetBundle(dependency);

            return ab;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> LoadAssetAsync<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            var bundleName = GetAssetBundleName(path);
            AssetBundle ab = await LoadAssetBundleAsync(bundleName);

            if (!loadedAssets.TryGetValue(path, out var asset))
            {
                asset = await ab.LoadAssetAsync(path);
                if (asset == null)
                    throw new Exception(path);
                loadedAssets.Add(path, asset);
            }

            return (asset as T)!;
#endif
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadAsset<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            var bundleName = GetAssetBundleName(path);
            AssetBundle ab = LoadAssetBundle(bundleName);

            if (!loadedAssets.TryGetValue(path, out var asset))
            {
                asset = ab.LoadAsset<T>(path);
                if (asset == null)
                    throw new Exception(path);
                loadedAssets.Add(path, asset);
            }

            return (asset as T)!;
#endif
        }


        private static string GetAssetBundleName(string path)
        {
            string[] levels = path.Split(new[] { '/' });
            string bundleName = string.Empty;
            if (levels.Length == 0)
            {
                return bundleName;
            }

            int end = levels.Length - 1;
            for (int i = 1; i < end; i++) // 去掉资源名
            {
                bundleName += levels[i];
                if (i < end - 1)
                {
                    bundleName += "_";
                }
            }

            if (!bundleName.EndsWith("ab"))
                bundleName += ".ab";

            return bundleName.ToLower();
        }
    }
}