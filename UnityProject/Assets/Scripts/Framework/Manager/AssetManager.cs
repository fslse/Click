using System;
using System.IO;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Framework.Log;
using Framework.Singleton;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Manager;

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
        AssetBundle ab = await LoadAssetBundle(AppConst.AssetsDir);
        abManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] abs = abManifest.GetAllAssetBundles();
        GameLog.LogDebug(null, abs);
    }

    private async UniTask<AssetBundle> LoadAssetBundle(string abName)
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
        AssetBundle ab = await AssetBundle.LoadFromFileAsync(path);
        loadedAssetBundles.Add(abName, ab);

        // 加载依赖包
        string[] dependencies = abManifest.GetAllDependencies(abName);
        foreach (string dependency in dependencies)
            if (!loadedAssetBundles.ContainsKey(dependency))
            {
                loadedAssetBundles.Add(dependency, await LoadAssetBundle(dependency));
            }

        return ab;
    }

    public async UniTask<T> LoadAsset<T>(string path) where T : Object
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<T>(path);
#else
        var bundleName = GetAssetBundleName(path);
        AssetBundle ab = await LoadAssetBundle(bundleName);

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