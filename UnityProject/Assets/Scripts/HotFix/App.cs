using Scripts.Fire.Log;

namespace HotFix
{
    public class App
    {
        public static void Main()
        {
            if (!GameApp.Instance)
                GameLog.LogError(GameApp.Instance.gameObject.name);

            // todo: do something
        }
    }
}