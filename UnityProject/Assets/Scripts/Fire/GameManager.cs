using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using Scripts.Fire.Startup;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Scripts.Fire
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public readonly string[] assemblyAssetName =
        {
            "GameConfig.dll.bytes",
            "GameProtocol.dll.bytes",
            "Framework.dll.bytes",
            "Assembly-CSharp.dll.bytes"
        };

        public System.Reflection.Assembly[] assembly;

        // AfterAssembliesLoaded 表示将会在 BeforeSceneLoad之前调用
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitUniTaskLoop()
        {
            GameLog.LogDebug("InitUniTaskLoop");
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref loop);
        }

        private void Awake()
        {
            GameLog.LogDebug("GameManager Awake");
            DontDestroyOnLoad(gameObject);
            Instance = this;
            // Application.logMessageReceived += GameLog.HandleLog;
            Application.logMessageReceivedThreaded += GameLog.HandleLog; // 多线程
            Application.targetFrameRate = AppConst.GameFrameRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Start()
        {
            GameLog.LogDebug("GameManager Start");

            // 构建启动流程
            // 40% + 20% + 40% 
            // 资源下载 + 加载DLL + 框架初始化
            var workflow = new Workflow();
            workflow.AddTask(new CheckVersion(workflow, "CheckVersion", 5));
            workflow.AddTask(new DownloadAssets(workflow, "DownloadAssets", 35));
            workflow.AddTask(new Preload(workflow, "Preload", 5));
            workflow.AddTask(new LoadDLL(workflow, "LoadDLL", 15));
            workflow.StartFlow();
        }

#if VERSION_DEV

        private const double MaxFrameTime = 1d / 30;
        private void Update()
        {
            if (Time.deltaTime > MaxFrameTime)
            {
                GameLog.LogWarning($"第{Time.frameCount - 1}帧耗时过长: {Time.deltaTime}");
            }
        }
#endif

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}