using System;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Framework.TimerModule;
using Framework.UIModule;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoTest;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZLogger;

namespace Game.GameLogic.UIPanel
{
    public class ExamplePanel : UIPanelBase
    {
        [SerializeField] private TMP_Text tmp;
        [SerializeField] private TMP_Text tmp1;

        [SerializeField] private Image img;
        [SerializeField] private Image img1;

        protected override void Init(object udata)
        {
            InitAsync(udata).Forget();
        }

        private async UniTaskVoid InitAsync(object udata)
        {
            img.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>("Assets/AssetPackages/Game/UIPanel/ExamplePanel/Texture/panel_border_brown.png");
            img1.sprite = AssetManager.Instance.LoadAsset<Sprite>("Assets/AssetPackages/Game/UIPanel/ExamplePanel/Texture/panel_border_grey_detail.png");

            const string gameConfDir = "Assets/AssetPackages/Config/Json";
            var tables = new cfg.Tables(file => JsonConvert.DeserializeObject(AssetManager.Instance.LoadAsset<TextAsset>($"{gameConfDir}/{file}.json").text) as JArray);

            await UniTask.Delay(TimeSpan.FromSeconds(5));
            tmp1.text = ZString.Join("\n", tables.TbScratchCardReward.Get(1001).Prizes[1].PrizeItems[0].Cash);
        }

        protected override void OnStart()
        {
            GameLog.Logger.ZLogInformation($"Hello World!");

            tmp.text = rectTransform.position.ToString();
            tmp1.text = PanelLayer.ToString();

            var timer = new Timer(TimeSpan.FromSeconds(3), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(), _ => { tmp1.text = "Success"; });
            timer.Start();

            var realTimer = new RealTimer(TimeSpan.FromSeconds(3), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(), Test);
            realTimer.Start();
        }

        private void Test(object state)
        {
            TestEnum testEnum = TestEnum.Unknown;
            TestMessage.Types.TestMessage1.Types.TestEnum1 testEnum1 = TestMessage.Types.TestMessage1.Types.TestEnum1.Begin;
            TestRequest request = new TestRequest
            {
                Id = 1,
                Name = "Test",
                Array = { "1", "2", "3", "4" },
                Map = { { "1", 1 }, { "2", 2 } }
            };

            request.Array.Add("5");
            request.Map.Add("3", 3);

            if (request.HasId)
            {
                tmp.text = "success";
            }

            TestResponse response = new TestResponse
            {
                Power = 100
            };
            TestMessage message = new TestMessage
            {
                Request = request,
                Response = response
            };

            GameLog.LogDebug(BitConverter.IsLittleEndian.ToString()); // 判断大小端

            var bytes = message.ToByteArray();

            if (BitConverter.IsLittleEndian) // 小端转大端
            {
                Array.Reverse(bytes);
            }

            Array.Reverse(bytes); // 大端转小端
            var msg = TestMessage.Parser.ParseFrom(bytes);

            var timer = new Timer(TimeSpan.FromSeconds(3), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(),
                _ => { tmp.text = msg.Response.Power.ToString(); });
            timer.Start();
        }
    }
}