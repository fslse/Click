using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using Framework.Cryptography;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AssetPackager
{
    private static DateTime timeStamp;

    [MenuItem("Builder/AssetBundle/Build AssetBundle", false, 101)]
    private static void BuildAssetBundle()
    {
#if UNITY_ANDROID
        Debug.LogWarning("Build AssetBundle for Android");
        BuildAssetBundle_(BuildTarget.Android);
#elif UNITY_IOS
        Debug.LogWarning("Build AssetBundle for IOS");
        BuildAssetBundle_(BuildTarget.iOS);
#elif UNITY_STANDALONE_WIN
        Debug.LogWarning("Build AssetBundle for Windows");
        BuildAssetBundle_(BuildTarget.StandaloneWindows64);
#endif
    }

    private static void BuildAssetBundle_(BuildTarget target)
    {
        timeStamp = DateTime.Now;

        if (Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);

        BuildVersionFile();

        AssetBundleBuilder.SetAssetBundleNames();
        AssetBundleBuilder.BuildAssetBundles(target);

        // 代码打包
        HybridCLRHelper.BuildAssetBundleByTarget(EditorUserBuildSettings.activeBuildTarget);

        BuildFileIndex();

        AssetDatabase.Refresh();

        Debug.LogWarning("Success && " + "Time: " + (DateTime.Now - timeStamp).TotalSeconds);
    }

    private static void BuildVersionFile()
    {
        const string fileName = "version.txt";
        string targetPath = Application.streamingAssetsPath + "/" + fileName;
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }

        string data = File.ReadAllText("Assets/Scenes/" + fileName);
        byte[] bytes = DESEncrypt.Encrypt(Encoding.UTF8.GetBytes(data));
        Debug.LogWarning(Encoding.UTF8.GetString(DESEncrypt.Decrypt(bytes)));
        File.WriteAllBytes(targetPath, bytes);
    }

    private static void BuildFileIndex()
    {
        string path = Application.streamingAssetsPath + "/files.txt";
        if (File.Exists(path)) File.Delete(path);

        string[] files = Directory.GetFiles(Application.streamingAssetsPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
            files[i] = files[i].Replace("\\", "/");

        using FileStream fs = new FileStream(path, FileMode.CreateNew);
        using StreamWriter sw = new StreamWriter(fs);
        foreach (var file in files)
        {
            if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

            string md5 = GetMD5(file);
            string value = file.Replace(Application.streamingAssetsPath + "/", string.Empty);
            sw.WriteLine(value + "|" + md5);
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

    private static string GetMD5(string file)
    {
        FileStream fs = new FileStream(file, FileMode.Open);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(fs);
        fs.Close();

        StringBuilder _ = new StringBuilder();
        foreach (var v in retVal)
        {
            _.Append(v.ToString("x2"));
        }

        return _.ToString();
    }
}