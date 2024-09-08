using Cysharp.Threading.Tasks;
using Scripts.Framework.Log;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Scripts.Framework
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
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}