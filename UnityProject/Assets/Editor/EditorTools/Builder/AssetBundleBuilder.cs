using System.IO;
using System.Linq;
using Framework;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 资源打包
/// </summary>
public static class AssetBundleBuilder
{
    // AssetBundle 自定义扩展名
    private const string extension = "ab";

    // 资源目录
    private const string sourceDirectory = "Assets/AssetPackages/";

    // 输出目录
    private static readonly string destinationDirectory = AppConst.AssetsPath;

    // 打包选项
    private const BuildAssetBundleOptions buildOptions =
        BuildAssetBundleOptions.ForceRebuildAssetBundle |

        // 保证包的唯一
        BuildAssetBundleOptions.AppendHashToAssetBundleName |

        // 递归处理依赖
        BuildAssetBundleOptions.RecurseDependencies |

        // Uncompressed bundles are faster to build and load, but, because they are larger, take longer to download.
        // Uncompressed AssetBundles are 16-byte aligned.
        BuildAssetBundleOptions.UncompressedAssetBundle |

        // This option saves runtime memory and loading performance for asset bundles.
        BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

    /// <summary>
    /// 打包的核心逻辑
    /// </summary>
    public static void SetAssetBundleNames()
    {
        string[] paths = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
        paths = paths.Concat(Directory.GetFiles("Assets/Scenes/", "*.unity", SearchOption.AllDirectories)).ToArray();

        int count = 0;
        foreach (string path in paths)
        {
            if (path.EndsWith(".meta"))
            {
                continue;
            }

            string p = path.Replace(@"\", "/");
            AssetImporter asset = AssetImporter.GetAtPath(p);
            if (asset == null)
            {
                continue;
            }

            string assetBundleName = GetAssetBundleName(p);
            if (assetBundleName == string.Empty)
            {
                continue;
            }

            count += 1;

            asset.SetAssetBundleNameAndVariant(assetBundleName, extension);
            asset.SaveAndReimport();
        }

        AssetDatabase.Refresh();
        Debug.Log("Assets number: " + count);
    }

    /// <summary>
    /// 打包
    /// </summary>
    /// <param name="buildTarget"></param>
    public static void BuildAssetBundles(BuildTarget buildTarget)
    {
        Debug.LogWarning("Build AssetBundles for " + buildTarget + " in " + destinationDirectory);
        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(destinationDirectory, buildOptions, buildTarget);
        if (manifest != null)
        {
            foreach (string assetBundleName in manifest.GetAllAssetBundles())
            {
                Debug.Log("AssetBundle: " + Path.Combine(destinationDirectory, assetBundleName));
                // todo: 加密
            }
        }
    }

    /// <summary>
    /// 根据资源所在路径获取AssetBundleName
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string GetAssetBundleName(string path)
    {
        string[] levels = path.Split(new[] { '/' });
        string bundleName = string.Empty;
        if (levels.Length == 0)
        {
            return bundleName;
        }

        int end = levels.Length - 1; // 去掉资源名
        for (int i = 1; i < end; i++)
        {
            bundleName += levels[i];
            if (i < end - 1)
            {
                bundleName += "_";
            }
        }

        return bundleName.ToLower();
    }

    public static void RemoveAssetBundleName()
    {
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        int length = assetBundleNames.Length;

        if (length > 0)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                AssetDatabase.RemoveAssetBundleName(assetBundleNames[i], true);
            }
        }

        AssetDatabase.Refresh();
        Debug.LogWarning("Remove AssetBundleName: " + length);
    }
}