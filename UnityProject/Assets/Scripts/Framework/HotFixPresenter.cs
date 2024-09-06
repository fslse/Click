using Cysharp.Threading.Tasks;
using Scripts.Framework.Log;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Framework;

public class HotFixPresenter : MonoBehaviour
{
    private void Start()
    {
        // 
        GameLog.LogDebug("HotFixPresenter Start");
        StartGame().Forget();
    }

    private static async UniTaskVoid StartGame()
    {
        await UniTask.Delay(3000);
        SceneManager.LoadSceneAsync("Scenes/Game");
    }
}