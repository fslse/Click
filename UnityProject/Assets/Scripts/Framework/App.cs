using System;
using Scripts.Fire.Log;

namespace Framework
{
    public class App
    {
        public static int Main()
        {
            GameLog.LogWarning("启动");

            GC.Collect();

            if (GameApp.Instance is null)
                GameLog.LogError("GameApp.Instance is null");

            return 0;
        }
    }
}