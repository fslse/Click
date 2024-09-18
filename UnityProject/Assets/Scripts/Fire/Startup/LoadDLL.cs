using System;
using System.Linq;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using HybridCLR;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using UnityEngine;

namespace Scripts.Fire.Startup
{
    public class LoadDLL : StartupTask
    {
        public LoadDLL(Workflow workflow, string taskName, int power) : base(workflow, taskName, power)
        {
        }

        public override void Start()
        {
            base.Start();
            Execute().Forget();
        }

        private System.Reflection.Assembly mainAssembly;

        private async UniTask Execute()
        {
#if UNITY_EDITOR // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。直接查找获得HotUpdate程序集
            mainAssembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
#else
            // / 实机
            var ab = await AssetManager.Instance.LoadAssetBundle("scripts.ab");
            GameLog.LogWarning("DLL AB\n", ZString.Join("\n", ab.GetAllAssetNames()));

            // 加载热更新dll
            // 如果有多个热更新dll，按照依赖顺序加载，先加载被依赖的assembly
            var dllBytes = ab.LoadAsset<TextAsset>("Assembly-CSharp.dll.bytes").bytes;
            mainAssembly = System.Reflection.Assembly.Load(dllBytes);

            // 为 AOT Assembly 补充元数据
            foreach (var assembly in AOTGenericReferences.PatchedAOTAssemblyList)
            {
                try
                {
                    dllBytes = ab.LoadAsset<TextAsset>(assembly.Replace("dll", "bytes")).bytes;
                }
                catch (Exception e)
                {
                    GameLog.LogError("Failed to load metadata for AOTAssembly", $"{assembly.Replace("dll", "bytes")}\n{e.Message}");
                    continue;
                }

                // HomologousImageMode.SuperSet: This mode relaxes the requirements for AOT dll, you can use either the cut AOT dll or the original AOT dll.
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode errorCode = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                GameLog.LogDebug($"[LoadMetadataForAOTAssembly {assembly}] ErrorCode: {errorCode}");
            }
#endif

            // 热更新程序集入口
            try
            {
                mainAssembly!.GetType("HotFix.App")!.GetMethod("Main")!.Invoke(null, null);
                workflow.OnTaskFinished(this);
            }
            catch (Exception e)
            {
                GameLog.LogError("Failed to run HotFix.App.Main", e.Message);
            }
        }
    }
}