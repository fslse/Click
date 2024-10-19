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

            return GameApp.Instance is null ? -1 : 0;
        }
    }
}