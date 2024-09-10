using System;
using System.IO;
using System.Linq;
using System.Text;
using Defective.JSON;
using Scripts.Framework.Cryptography;

namespace Scripts.Framework.Fire
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

            if (File.Exists(streamingAssetsVersionFile))
            {
                var obj = JSONObject.Create(Encoding.UTF8.GetString(AESEncrypt.Decrypt(File.ReadAllBytes(AppConst.PersistentDataPath + "/version"))));
                streamingAssetsVersion = obj.GetField("AppVersion").ToString().Split('.').Aggregate(0L, (res, s) => (res + int.Parse(s)) * 1000);
                streamingAssetsVersion += int.Parse(obj.GetField("ResVersion").ToString());
            }

            if (File.Exists(persistentDataVersionFile))
            {
                var obj = JSONObject.Create(Encoding.UTF8.GetString(AESEncrypt.Decrypt(File.ReadAllBytes(AppConst.PersistentDataPath + "/version"))));
                persistentDataVersion = obj.GetField("AppVersion").ToString().Split('.').Aggregate(0L, (res, s) => (res + int.Parse(s)) * 1000);
                persistentDataVersion += int.Parse(obj.GetField("ResVersion").ToString());
            }

            if (streamingAssetsVersion > persistentDataVersion)
            {
                Directory.Delete(AppConst.PersistentDataPath, true);
            }

            workflow.localVersion = Math.Max(streamingAssetsVersion, persistentDataVersion);
            workflow.OnTaskFinished(this, false);
        }
    }
}