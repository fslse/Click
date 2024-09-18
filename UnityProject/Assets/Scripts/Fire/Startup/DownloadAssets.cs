using System;
using System.IO;
using System.Linq;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using UnityEngine.Networking;

namespace Scripts.Fire.Startup
{
    public class DownloadAssets : StartupTask
    {
        public DownloadAssets(Workflow workflow, string taskName, int power) : base(workflow, taskName, power)
        {
        }

        public override void Start()
        {
            base.Start();
            if (workflow.localVersion > workflow.remoteVersion) Skip();
            else Execute().Forget();
        }

        private async UniTaskVoid Execute()
        {
            var localManifest = string.Empty;
            if (File.Exists(AppConst.PersistentDataPath + "manifest.txt"))
            {
                localManifest = await File.ReadAllTextAsync(AppConst.PersistentDataPath + "manifest.txt");
            }

            var remoteManifest = string.Empty;
            try
            {
                remoteManifest = (await UnityWebRequest.Get(AppConst.RemoteAssetsPath + "manifest.txt").SendWebRequest()).downloadHandler.text;
            }
            catch (Exception e)
            {
                GameLog.LogError("Failed to download manifest.txt", e.Message);
            }

            if (string.IsNullOrEmpty(remoteManifest) || localManifest == remoteManifest)
            {
                Skip();
                return;
            }

            var localAssetPackages = localManifest.Replace(" ", "").Split('\n').ToHashSet();
            var remoteAssetPackages = remoteManifest.Replace(" ", "").Split('\n').ToList();

            GameLog.LogDebug($"Local Manifest\n{ZString.Join("\n", localAssetPackages.ToArray())}");
            GameLog.LogDebug($"Remote Manifest\n{ZString.Join("\n", remoteAssetPackages)}");

            int total = 1 + remoteAssetPackages.Count(assetPackage => !localAssetPackages.Contains(assetPackage));
            workflow.OnProgress(this, (int)(100f / total));

            int count = 1;
            foreach (string name in from assetPackage in remoteAssetPackages where !localAssetPackages.Contains(assetPackage) select assetPackage[..assetPackage.IndexOf('|')])
            {
                int _ = count;
                var request = await UnityWebRequest.Get(AppConst.RemoteAssetsPath + name).SendWebRequest().ToUniTask(Progress.Create<float>(x =>
                {
                    // 下载进度
                    workflow.OnProgress(this, (int)(100f * _ / total + 100f * x / total));
                }));

                if (request.result == UnityWebRequest.Result.Success)
                {
                    await File.WriteAllBytesAsync(AppConst.PersistentDataPath + name, request.downloadHandler.data);
                    ++count;
                    workflow.OnProgress(this, (int)(100f * count / total));
                }
                else
                {
                    GameLog.LogError($"Failed to download {name}", request.error);
                }
            }

            await File.WriteAllTextAsync(AppConst.PersistentDataPath + "manifest.txt", remoteManifest);

            GameLog.LogDebug("Number of downloaded assets", count.ToString());
            workflow.OnTaskFinished(this);
        }
    }
}