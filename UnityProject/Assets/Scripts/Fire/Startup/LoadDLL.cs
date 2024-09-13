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
            Startup().Forget();
        }

        private async UniTaskVoid Startup()
        {
            await Execute();
            GameLog.LogDebug("Try run App.Main");
            RunAppMain(); // 启动启动 全部启动
            workflow.OnTaskFinished(this);
        }

        private System.Reflection.Assembly mainAssembly;


        private async UniTask Execute()
        {
#if UNITY_EDITOR // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。直接查找获得HotUpdate程序集
            mainAssembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
#else
            // / 实机
            var ab = await AssetManager.Instance.LoadAssetBundle("scripts.ab");
            GameLog.LogDebug("DLL AB", ZString.Join("\n", ab.GetAllAssetNames()));

            var dllBytes = ab.LoadAsset<TextAsset>("Assembly-CSharp.dll.bytes").bytes;
            mainAssembly = System.Reflection.Assembly.Load(dllBytes);

            LoadMetadataForAOTAssembly();
            return;

            unsafe void LoadMetadataForAOTAssembly()
            {
                foreach (var assembly in AOTGenericReferences.PatchedAOTAssemblyList)
                {
                    try
                    {
                        dllBytes = ab.LoadAsset<TextAsset>(assembly.Replace("dll", "bytes")).bytes;
                    }
                    catch (Exception e)
                    {
                        GameLog.LogError("Load AOT DLL", $"{assembly.Replace("dll", "bytes")}\n{e.Message}");
                        continue;
                    }

                    fixed (byte* ptr = dllBytes)
                    {
                        // HomologousImageMode.SuperSet: This mode relaxes the requirements for AOT dll, you can use either the cut AOT dll or the original AOT dll.
                        LoadImageErrorCode errorCode = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                        GameLog.LogDebug("LoadMetadataForAOTAssembly", $"{assembly}\nErrorCode : {errorCode}");
                    }
                }
            }
#endif
        }

        private void RunAppMain()
        {
            try
            {
                mainAssembly!.GetType("HotFix.App")!.GetMethod("Main")!.Invoke(null, null);
            }
            catch (Exception e)
            {
                GameLog.LogError("Run App Main", e.Message);
            }
        }
    }
}