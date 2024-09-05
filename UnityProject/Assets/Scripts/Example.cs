using System;
using System.IO;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Framework;
using Framework.Log;
using Framework.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZLogger;

public class Example : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private TMP_Text tmp1;

    [SerializeField] private Image img;
    [SerializeField] private Image img1;

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

        try
        {
            tmp.text = Directory.GetFiles(AppConst.PersistentDataPath).Length.ToString();
            var info = ZString.CreateStringBuilder();
            var collection = Directory.GetFiles(AppConst.PersistentDataPath);
            foreach (var _ in collection)
            {
                info.Append(_);
                info.Append("\n");
            }

            tmp1.text = info.ToString();
        }
        catch (Exception e)
        {
            GameLog.LogError(e.Message);
        }

        Init().Forget();
    }

    private async UniTaskVoid Init()
    {
        await AssetManager.Instance.Initialize();
        await AssetManager.Instance.LoadAsset<Sprite>("Assets/AssetPackages/Game/panel_border_brown.png");
        img.sprite = await AssetManager.Instance.LoadAsset<Sprite>("Assets/AssetPackages/Game/panel_border_brown.png");
        img1.sprite = await AssetManager.Instance.LoadAsset<Sprite>("Assets/AssetPackages/Game/panel_border_grey_detail.png");
    }
}