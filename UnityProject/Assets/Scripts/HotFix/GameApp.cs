using System;
using Scripts.Framework.Log;
using Scripts.Framework.Singleton;

namespace HotFix
{
    public class GameApp : MonoSingleton<GameApp>
    {
        private void Awake()
        {
            GameLog.LogDebug("GameApp Awake");
            throw new NotImplementedException();
        }
    }
}