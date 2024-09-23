using System;
using Scripts.Fire.Log;
using Scripts.Fire.Singleton;

namespace Game
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