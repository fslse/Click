using System;
using System.IO;
using System.Linq;
using System.Text;
using Defective.JSON;
using Scripts.Fire.Cryptography;
using Scripts.Fire.Log;

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

            long streamingAssetsVersion = -1, persistentDataVersion = -1;
            string streamingAssetsVersionFile = AppConst.StreamingAssetsPath + "/version";
            string persistentDataVersionFile = AppConst.PersistentDataPath + "/version";

            GetVersion(streamingAssetsVersionFile, ref streamingAssetsVersion);
            GetVersion(persistentDataVersionFile, ref persistentDataVersion);

            if (streamingAssetsVersion > persistentDataVersion && Directory.Exists(AppConst.PersistentDataPath))
            {
                Directory.Delete(AppConst.PersistentDataPath, true);
            }

            workflow.localVersion = Math.Max(streamingAssetsVersion, persistentDataVersion);
            GameLog.LogDebug($"Streaming Assets Version: {streamingAssetsVersion}");
            GameLog.LogDebug($"Persistent Data Version: {persistentDataVersion}");
            GameLog.LogWarning($"Local Version: {workflow.localVersion}");
            workflow.OnTaskFinished(this);
            return;

            void GetVersion(string path, ref long version)
            {
                if (File.Exists(path))
                {
                    var obj = JSONObject.Create(Encoding.UTF8.GetString(AESEncrypt.Decrypt(File.ReadAllBytes(AppConst.StreamingAssetsPath + "/version"))));
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