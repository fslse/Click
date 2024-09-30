using System;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Defective.JSON;
using Scripts.Fire.Cryptography;
using Scripts.Fire.Log;
using UnityEngine.Networking;

namespace Scripts.Fire.Startup
{
    public class CheckVersion : StartupTask
    {
        public CheckVersion(Workflow workflow, string taskName, int power) : base(workflow, taskName, power)
        {
        }

        public override void Start()
        {
            base.Start();
            Execute().Forget();
        }

        private async UniTaskVoid Execute()
        {
            workflow.localVersion = await GetVersion(AppConst.StreamingAssetsPath + "version");
            workflow.remoteVersion = await GetVersion(AppConst.RemoteAssetsPath + "version");

            GameLog.LogDebug($"Local Version: {workflow.localVersion}");
            GameLog.LogDebug($"Remote Version: {workflow.remoteVersion}");

            long _ = Math.Max(workflow.localVersion, workflow.remoteVersion);
            GameManager.Instance.version = $"{_ / 1000000000 % 1000}.{_ / 1000000 % 1000}.{_ / 1000 % 1000}";
            GameLog.LogDebug("Game Version", GameManager.Instance.version);

            if (workflow.localVersion > workflow.remoteVersion && Directory.Exists(AppConst.PersistentDataPath))
            {
                Directory.Delete(AppConst.PersistentDataPath, true);
            }

            workflow.OnTaskFinished(this);
            return;

            async UniTask<long> GetVersion(string url)
            {
                try
                {
                    using var request = UnityWebRequest.Get(url);
                    await request.SendWebRequest();
                    var obj = JSONObject.Create(Encoding.UTF8.GetString(AESEncrypt.Decrypt(request.downloadHandler.data)));
                    GameLog.LogDebug(url, obj.GetField("AppVersion").ToString());
                    long version = obj.GetField("AppVersion").stringValue.Split('.').Aggregate(0L, (res, s) => (res + int.Parse(s)) * 1000);
                    version += int.Parse(obj.GetField("ResVersion").ToString());
                    return version;
                }
                catch
                {
                    GameLog.LogWarning($"Failed to get version from {url}");
                    return -1;
                }
            }
        }
    }
}