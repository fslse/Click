using System;
using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;

#endif

namespace HotFix
{
    public class App
    {
        public static int Main()
        {
            GameLog.LogWarning("启动");

            GC.Collect();

            // if (GameApp.Instance == null) ;

#if UNITY_EDITOR
            EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/Game.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
            AssetManager.Instance.LoadAssetBundle("scenes.ab");
            SceneManager.LoadScene("Scenes/Game");
#endif
            return 0;
        }
    }
}