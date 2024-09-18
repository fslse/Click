using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using Scripts.Fire;
using Scripts.Fire.Cryptography;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AssetPackager
{
    [MenuItem("Builder/AssetBundle/Build AssetBundle", false, 101)]
    private static void BuildAssetBundle()
    {
        Debug.LogWarning($"Build AssetBundle for {EditorUserBuildSettings.activeBuildTarget}");
        BuildAssetBundle(EditorUserBuildSettings.activeBuildTarget);
    }

    private static void BuildAssetBundle(BuildTarget target)
    {
        DateTime timeStamp = DateTime.Now;

        // 清空目录
        if (Directory.Exists(AppConst.AssetsPath))
            Directory.Delete(AppConst.AssetsPath, true);
        Directory.CreateDirectory(AppConst.AssetsPath);

        // 构建版本文件
        BuildVersionFile();

        // 资源（不含代码）打包
        AssetBundleBuilder.SetAssetBundleNames();
        AssetBundleBuilder.BuildAssetBundles(target);

        // 代码打包
        HybridCLRHelper.BuildAssetBundleByTarget(EditorUserBuildSettings.activeBuildTarget);

        // 资源清单
        BuildManifest();

        // 刷新
        AssetDatabase.Refresh();

        Debug.LogWarning($"Success && Time: {(DateTime.Now - timeStamp).TotalSeconds}");
    }

    private static void BuildVersionFile()
    {
        string targetPath = AppConst.AssetsPath + "version";
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }

        string data = File.ReadAllText("Assets/Scenes/version.json");
        byte[] bytes = AESEncrypt.Encrypt(Encoding.UTF8.GetBytes(data));
        Debug.LogWarning(Encoding.UTF8.GetString(AESEncrypt.Decrypt(bytes)));
        File.WriteAllBytes(targetPath, bytes);
    }

    private static void BuildManifest()
    {
        string path = AppConst.AssetsPath + "manifest.txt";
        if (File.Exists(path)) File.Delete(path);

        string[] files = Directory.GetFiles(Application.streamingAssetsPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
            files[i] = files[i].Replace("\\", "/");

        using FileStream fs = new FileStream(path, FileMode.CreateNew);
        using StreamWriter sw = new StreamWriter(fs);
        foreach (var file in files)
        {
            if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;
            sw.WriteLine(file.Replace(AppConst.StreamingAssetsPath, string.Empty) + "|" + HashHelper.ComputeHash<MD5>(file));
        }
    }

    [MenuItem("Builder/AssetBundle/Remove AssetBundleName", false, 102)]
    public static void RemoveAssetBundleName()
    {
        AssetBundleBuilder.RemoveAssetBundleName();
    }

    [MenuItem("Builder/StreamingAssets/Open", false, 121)]
    private static void OpenStreamingAssetsFolder()
    {
        Debug.Log(Application.streamingAssetsPath);
        if (Directory.Exists(Application.streamingAssetsPath))
            Process.Start(new ProcessStartInfo(Application.streamingAssetsPath, "explorer.exe"));
        else
        {
            Debug.LogError("StreamingAssets folder does not exist!");
            Directory.CreateDirectory(Application.streamingAssetsPath);
            Debug.Log("StreamingAssets folder has been created!");
        }
    }

    [MenuItem("Builder/StreamingAssets/Clean", false, 122)]
    private static void CleanStreamingAssetsFolder()
    {
        Debug.Log(Application.streamingAssetsPath);
        if (Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.Delete(Application.streamingAssetsPath, true);
            Directory.CreateDirectory(Application.streamingAssetsPath);
            AssetDatabase.Refresh();
        }
        else Debug.LogError("StreamingAssets folder does not exist!");
    }

    [MenuItem("Builder/PersistentData/Open", false, 131)]
    private static void OpenPersistentDataFolder()
    {
        Debug.Log(Application.persistentDataPath);
        if (Directory.Exists(Application.persistentDataPath))
            Process.Start(new ProcessStartInfo(Application.persistentDataPath, "explorer.exe"));
        else
        {
            Debug.LogError("PersistentData folder does not exist!");
            Directory.CreateDirectory(Application.persistentDataPath);
            Debug.Log("PersistentData folder has been created!");
        }
    }

    [MenuItem("Builder/PersistentData/Clean", false, 132)]
    private static void CleanPersistentDataFolder()
    {
        Debug.Log(Application.persistentDataPath);
        if (Directory.Exists(Application.persistentDataPath))
        {
            Directory.Delete(Application.persistentDataPath, true);
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        else Debug.LogError("PersistentData folder does not exist!");
    }
}