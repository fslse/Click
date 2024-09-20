using System;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using Scripts.Fire.UniTaskTimer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZLogger;

namespace HotFix
{
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

            GameLog.LogWarning("1");
            GameLog.LogWarning("2", "3");

            GameLog.LogError("1");
            GameLog.LogError("2", "3");

            const string world = "World";
            GameLog.Logger.ZLogInformation($"Hello {world}!");

            Debug.Log("Hello World!");
            tmp.text = world;

            tmp1.text = "Hot Fix";


            var timer = new Timer(TimeSpan.FromSeconds(3), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(), _ => { tmp1.text = "Success"; });
            timer.Start();

            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            img.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>("Assets/AssetPackages/Game/panel_border_brown.png");
            img1.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>("Assets/AssetPackages/Game/panel_border_grey_detail.png");


            const string gameConfDir = "Assets/AssetPackages/Config/Json";
            var tables = new cfg.Tables(file => JsonConvert.DeserializeObject(AssetManager.Instance.LoadAsset<TextAsset>($"{gameConfDir}/{file}.json").text) as JArray);

            await UniTask.Delay(TimeSpan.FromSeconds(5));
            tmp1.text = ZString.Join("\n", tables.TbScratchCardReward.Get(1001).Prizes[1].PrizeItems[0].Cash);
        }
    }
}