using Scripts.Fire.Log;
using Scripts.Fire.Singleton;
using UnityEngine;

namespace Game
{
    public class GameApp : MonoSingleton<GameApp>
    {
        private void Awake()
        {
            GameLog.LogDebug("GameApp Awake", Time.frameCount.ToString());
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
    }
}