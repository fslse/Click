using Cysharp.Threading.Tasks;
using Framework.Singleton;
using UnityEngine;

namespace Framework;

public class AssetManager : MonoSingleton<AssetManager>
{
    private AssetBundleManifest manifest;

    public async UniTask Initialize()
    {
    }
}