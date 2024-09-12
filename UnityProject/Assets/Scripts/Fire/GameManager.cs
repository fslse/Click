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
            var workflow = new Workflow();
            workflow.AddTask(new CheckVersion(workflow, "CheckVersion", 20));
            workflow.AddTask(new DownloadAssets(workflow, "DownloadAssets", 30));
            workflow.AddTask(new Preload(workflow, "Preload", 20));
            workflow.AddTask(new LoadDLL(workflow, "LoadDLL", 30));
            workflow.StartFlow();
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}