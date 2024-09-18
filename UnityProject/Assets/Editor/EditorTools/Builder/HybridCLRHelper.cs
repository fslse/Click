using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HybridCLR.Editor;
using HybridCLR.Editor.AOT;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using Scripts.Fire;
using UnityEditor;
using UnityEngine;

public static class HybridCLRHelper
{
    private static string HybridCLRBuildCacheDir => $"{Application.dataPath}/HybridCLRBuildCache";

    private static string AssetBundleOutputDir => $"{HybridCLRBuildCacheDir}/AssetBundleOutput";

    private static string AssetBundleSourceDataTempDir => $"{HybridCLRBuildCacheDir}/AssetBundleSourceData";

    public static void BuildAssetBundleByTarget(BuildTarget target)
    {
        BuildAssetBundles($"{AssetBundleSourceDataTempDir}/{target}", $"{AssetBundleOutputDir}/{target}", target);
    }

    /// <summary>
    /// 将 HotUpdateDLL 和 AOT DLL 打入scripts包.
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

        // 编译热更新dll
        CompileDllCommand.CompileDll(target);

        List<string> assets = new List<string>();
        // 热更新dll输出目录
        // "HybridCLRData/HotUpdateDlls/{target}"
        string hotUpdateDllsDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved) // 遍历热更新dll列表
        {
            string dllPath = $"{hotUpdateDllsDir}/{dll}";
            string dllBytesPath = $"{tempDir}/{dll}.bytes";
            File.Copy(dllPath, dllBytesPath, true); // 复制dll到临时目录
            assets.Add(dllBytesPath);
            Debug.Log($"[HybridCLRHelper] Copy HotUpdate DLL: {dllPath} -> {dllBytesPath}");
        }

        // 生成待补充元数据列表
        // AOTReferenceGeneratorCommand.CompileAndGenerateAOTGenericReference();

        // 将 AOT Assembly 加入HybridCLR的补充元数据列表 
        HybridCLRSettings.Instance.patchAOTAssemblies = AOTGenericReferences.PatchedAOTAssemblyList.Select(aotAssembly => aotAssembly[..^4]).ToArray();
        HybridCLRSettings.Save();

        // 裁减后 AOT dll 输出目录
        // "HybridCLRData/AssembliesPostIl2CppStrip/{target}"
        string aotDllDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        string aotDllDir2 = $"{SettingsUtil.HybridCLRDataDir}/StrippedAOTAssembly2/{target}";
        foreach (var dll in SettingsUtil.AOTAssemblyNames)
        {
            string dllPath = $"{aotDllDir}/{dll}.dll";
            if (!File.Exists(dllPath))
            {
                Debug.LogError($"AOT DLL: {dllPath} 缺失");
                Debug.LogError("打包过程生成的裁剪后的AOT dll可以用于补充元数据 || 使用HybridCLR/Generate/AotDlls命令也可以立即生成裁剪后的AOT dll");
                continue;
            }

            // 进一步剔除AOT dll中非泛型函数元数据，输出到StrippedAOTAssembly2目录下
            string dllPath2 = $"{aotDllDir2}/{dll}.dll";
            AOTAssemblyMetadataStripper.Strip(dllPath, dllPath2);

            string dllBytesPath = $"{tempDir}/{dll}.bytes";
            File.Copy(dllPath2, dllBytesPath, true); // 复制dll到临时目录
            assets.Add(dllBytesPath);
            Debug.Log($"[HybridCLRHelper] Copy AOT DLL: {dllPath2} -> {dllBytesPath}");
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        List<AssetBundleBuild> abs = new List<AssetBundleBuild>();
        AssetBundleBuild scripts = new AssetBundleBuild
        {
            assetBundleName = "scripts",
            assetNames = assets.Select(ToRelativePath).ToArray(),
            addressableNames = assets.Select(s => s.Replace(tempDir + '/', "")).ToArray(),
            assetBundleVariant = "ab"
        };
        abs.Add(scripts);

        BuildPipeline.BuildAssetBundles(outputDir, abs.ToArray(), BuildAssetBundleOptions.None, target); // DLL打包
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);
        foreach (var ab in abs) // 复制打包后的DLL到StreamingAssets文件夹
        {
            AssetDatabase.CopyAsset(ToRelativePath($"{outputDir}/{ab.assetBundleName}.ab"), ToRelativePath($"{AppConst.AssetsPath}/{ab.assetBundleName}.ab"));
        }
    }

    private static string ToRelativePath(string str)
    {
        return str[str.IndexOf("Assets/", StringComparison.Ordinal)..];
    }
}