using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;

public static class HybridCLRBuilder
{
    public static string HybridCLRBuildCacheDir => Application.dataPath + "/HybridCLRBuildCache";

    public static string AssetBundleOutputDir => $"{HybridCLRBuildCacheDir}/AssetBundleOutput";

    public static string AssetBundleSourceDataTempDir => $"{HybridCLRBuildCacheDir}/AssetBundleSourceData";

    public static void BuildAssetBundleByTarget(BuildTarget target)
    {
        BuildAssetBundles(GetAssetBundleTempDirByTarget(target), GetAssetBundleOutputDirByTarget(target), target);
    }


    /// <summary>
    /// 将HotFix.dll和HotUpdatePrefab.prefab打入common包.
    /// 将HotUpdateScene.unity打入scene包.
    /// </summary>
    /// <param name="tempDir"></param>
    /// <param name="outputDir"></param>
    /// <param name="target"></param>
    private static void BuildAssetBundles(string tempDir, string outputDir, BuildTarget target)
    {
        if (Directory.Exists(outputDir))
            Directory.Delete(outputDir, true);
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);
        Directory.CreateDirectory(outputDir);
        Directory.CreateDirectory(tempDir);

        // 编译DLL
        CompileDllCommand.CompileDll(target);

        List<string> assets = new List<string>();
        string hotUpdateDllsDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target); // DLL路径
        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved) // 遍历热更新dll列表
        {
            string dllPath = $"{hotUpdateDllsDir}/{dll}";
            string dllBytesPath = $"{tempDir}/{dll}.bytes";
            File.Copy(dllPath, dllBytesPath, true); // 复制dll到临时目录
            assets.Add(dllBytesPath);
            Debug.Log($"[BuildAssetBundles] Copy HotUpdate Dll : {dllPath} -> {dllBytesPath}");
        }

        // 裁减后AOT dll输出根目录
        // "HybridCLRData/AssembliesPostIl2CppStrip/{target}"
        string aotDllDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);

        // 补充元数据AOT dlls
        foreach (var dll in SettingsUtil.AOTAssemblyNames)
        {
            string dllPath = $"{aotDllDir}/{dll}.dll";
            if (!File.Exists(dllPath))
            {
                Debug.LogError($"AOT DLL : {dllPath} 缺失");
                Debug.LogError("打包过程生成的裁剪后的AOT dll可以用于补充元数据 || 使用HybridCLR/Generate/AotDlls命令也可以立即生成裁剪后的AOT dll");
                continue;
            }

            string dllBytesPath = $"{tempDir}/{dll}.bytes";
            File.Copy(dllPath, dllBytesPath, true); // 复制dll到临时目录
            assets.Add(dllBytesPath);
            Debug.Log($"[BuildAssetBundles] Copy AOT DLL : {dllPath} -> {dllBytesPath}");
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        List<AssetBundleBuild> abs = new List<AssetBundleBuild>();
        AssetBundleBuild common = new AssetBundleBuild
        {
            assetBundleName = "common",
            assetNames = assets.Select(ToRelativeAssetPath).ToArray()
        };
        abs.Add(common);

        BuildPipeline.BuildAssetBundles(outputDir, abs.ToArray(), BuildAssetBundleOptions.None, target); // DLL打包
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);
        foreach (var ab in abs) // 复制打包后的DLL到StreamingAssets文件夹
        {
            AssetDatabase.CopyAsset(ToRelativeAssetPath($"{outputDir}/{ab.assetBundleName}"),
                ToRelativeAssetPath($"{Application.streamingAssetsPath}/{ab.assetBundleName}"));
        }
    }

    public static string ToRelativeAssetPath(string str)
    {
        return str[str.IndexOf("Assets/", StringComparison.Ordinal)..];
    }

    public static string GetAssetBundleOutputDirByTarget(BuildTarget target)
    {
        return $"{AssetBundleOutputDir}/{target}";
    }

    public static string GetAssetBundleTempDirByTarget(BuildTarget target)
    {
        return $"{AssetBundleSourceDataTempDir}/{target}";
    }
}