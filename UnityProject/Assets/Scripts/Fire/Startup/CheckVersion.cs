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
            string persistentDataVersionFile = AppConst.PersistentDataPath + "/version";

            // 从应用程序内部资源中读取版本信息
            GetVersion(AppConst.StreamingAssetsPath + "/version", ref workflow.localVersion);

            try
            {
                if (File.Exists(persistentDataVersionFile))
                    File.Delete(persistentDataVersionFile);

                using UnityWebRequest request = UnityWebRequest.Get(AppConst.RemoteAssetsPath + "version");
                request.downloadHandler = new DownloadHandlerFile(persistentDataVersionFile);
                await request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    GameLog.LogDebug("Download VersionFile", "Success");
                    GetVersion(persistentDataVersionFile, ref workflow.remoteVersion);
                }
            }
            catch (Exception e)
            {
                GameLog.LogError("Download VersionFile", e.Message);
            }

            if (workflow.localVersion > workflow.remoteVersion && Directory.Exists(AppConst.PersistentDataPath))
            {
                Directory.Delete(AppConst.PersistentDataPath, true);
            }

            GameLog.LogDebug($"Local Version: {workflow.localVersion}");
            GameLog.LogDebug($"Remote Version: {workflow.remoteVersion}");

            workflow.OnTaskFinished(this);
            return;

            void GetVersion(string path, ref long version)
            {
                if (File.Exists(path))
                {
                    var obj = JSONObject.Create(Encoding.UTF8.GetString(AESEncrypt.Decrypt(File.ReadAllBytes(path))));
                    GameLog.LogDebug(path, obj.GetField("AppVersion").ToString());

                    version = obj.GetField("AppVersion").stringValue.Split('.').Aggregate(0L, (res, s) => (res + int.Parse(s)) * 1000);
                    version += int.Parse(obj.GetField("ResVersion").ToString());
                }
                else
                    GameLog.LogWarning(path, "File not found");
            }
        }
    }
}