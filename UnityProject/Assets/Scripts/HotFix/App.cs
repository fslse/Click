using System;
using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace HotFix
{
    public class App
    {
        public static int Main()
        {
            GameLog.LogWarning("启动");

            GC.Collect();

            Startup().Forget();

            return 0;
        }

        private static async UniTaskVoid Startup()
        {
            if (GameApp.Instance == null) ;

#if UNITY_EDITOR
            await EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Game.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
            await AssetManager.Instance.LoadAssetBundle("scenes.ab");
            SceneManager.LoadScene("Scenes/Game");
#endif
        }
    }
}