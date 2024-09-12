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

            var remoteManifest = (await UnityWebRequest.Get(AppConst.RemoteAssetsPath + "manifest.txt").SendWebRequest()).downloadHandler.text;


            if (localManifest == remoteManifest)
            {
                Skip();
                return;
            }

            var localAssetPackages = localManifest.Replace(" ", "").Split('\n').ToHashSet();
            var remoteAssetPackages = remoteManifest.Replace(" ", "").Split('\n').ToList();

            GameLog.LogDebug("Local Manifest", localAssetPackages.Count.ToString());
            GameLog.LogDebug("Remote Manifest", ZString.Join("\n", remoteAssetPackages));

            int total = 1 + remoteAssetPackages.Count(assetPackage => !localAssetPackages.Contains(assetPackage));
            workflow.OnProgress(this, (int)(100f / total));

            int count = 1;
            foreach (string name in from assetPackage in remoteAssetPackages where !localAssetPackages.Contains(assetPackage) select assetPackage[..assetPackage.IndexOf('|')])
            {
                var request = await UnityWebRequest.Get(AppConst.RemoteAssetsPath + name).SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    await File.WriteAllBytesAsync(AppConst.PersistentDataPath + name, request.downloadHandler.data);
                    ++count;
                    workflow.OnProgress(this, (int)(100f * count / total));
                }
                else
                {
                    GameLog.LogError($"Download {name}", request.error);
                }
            }

            await File.WriteAllTextAsync(AppConst.PersistentDataPath + "manifest.txt", remoteManifest);

            GameLog.LogDebug("Download Assets Num", count.ToString());
            workflow.OnTaskFinished(this);
        }
    }
}