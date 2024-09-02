using Cysharp.Threading.Tasks;
using Framework.Log;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadSceneAsync(Application.dataPath + "/Scenes/Game");
    }
}