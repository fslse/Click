using System.IO;
using Framework.Log;
using TMPro;
using UnityEngine;
using ZLogger;

public class Example : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private TMP_Text tmp1;

    private void Start()
    {
        GameLog.LogDebug("1");
        GameLog.LogDebug("2", "3");
        GameLog.LogDebug(null, "4", "5", "6");

        GameLog.LogWarning("1");
        GameLog.LogWarning("2", "3");
        GameLog.LogWarning(null, "4", "5", "6");

        GameLog.LogError("1");
        GameLog.LogError("2", "3");
        GameLog.LogError(null, "4", "5", "6");

        const string world = "World";
        GameLog.Logger.ZLogInformation($"Hello {world}!");

        tmp.text = Directory.GetFiles(Application.streamingAssetsPath).Length.ToString();
        tmp1.text = Directory.GetFiles(Application.streamingAssetsPath)[0];
    }
}