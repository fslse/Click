using Scripts.Framework.Log;
using UnityEngine;

namespace Scripts.Framework;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
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