using System.IO;
using Cysharp.Threading.Tasks;
using Framework.Singleton;
using UnityEngine;

namespace Framework.Manager;

public class AssetManager : MonoSingleton<AssetManager>
{
    private AssetBundleManifest manifest;

    public async UniTask Initialize()
    {
        // 总包
        AssetBundle ab = await LoadAssetBundle(AppConst.AssetsDir);
    }


    private async UniTask<AssetBundle> LoadAssetBundle(string abName)
    {
        string path = AppConst.PersistentDataPath + abName;
        if (!File.Exists(path))
        {
            path = AppConst.StreamingAssetsPath + abName;
        }
        else
        {
        }

        return null!;
    }

    private void LoadAsset(string abName, string assetName)
    {
    }
}