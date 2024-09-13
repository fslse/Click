using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using UnityEngine.SceneManagement;

namespace HotFix
{
    public class App
    {
        public static int Main()
        {
            GameLog.LogWarning("启动");

            // todo: Startup
            Startup().Forget();

            return 0;
        }

        private static async UniTaskVoid Startup()
        {
            await AssetManager.Instance.LoadAssetBundle("scenes.ab");
            SceneManager.LoadScene("Scenes/Game");
        }
    }
}