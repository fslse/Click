using System;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Framework.Core.MemoryPool;
using Framework.Modules.Time;
using Framework.Modules.UI;
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

namespace Main.UIPanel
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
            img.sprite = await AssetManager.Instance.LoadAssetAsync<Sprite>("Assets/AssetPackages/UIPanel/ExamplePanel/Texture/panel_border_brown.png");
            img1.sprite = AssetManager.Instance.LoadAsset<Sprite>("Assets/AssetPackages/UIPanel/ExamplePanel/Texture/panel_border_grey_detail.png");

            const string GameConfDir = "Assets/AssetPackages/Config/Json";
            var tables = new cfg.Tables(file => JsonConvert.DeserializeObject(AssetManager.Instance.LoadAsset<TextAsset>($"{GameConfDir}/{file}.json").text) as JArray);

            await UniTask.Delay(TimeSpan.FromSeconds(5));
            tmp1.text = ZString.Join("\n", tables.TbScratchCardReward.Get(1001).Prizes[1].PrizeItems[0].Cash);
        }

        protected override void OnStart()
        {
            GameLog.Logger.ZLogInformation($"Hello World!");

            tmp.text = rectTransform.position.ToString();
            tmp1.text = PanelLayer.ToString();

            var timer = MemoryPoolManager.Alloc<DeltaTimer>();
            timer.SetTimer(TimeSpan.FromSeconds(3), _ =>
            {
                tmp1.text = "Success";
                MemoryPoolManager.Dealloc(timer);
            });
            timer.Restart();


            var realTimer = MemoryPoolManager.Alloc<RealTimer>();
            realTimer.SetTimer(TimeSpan.FromSeconds(3), Test);
            realTimer.Restart();
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

            var timer = MemoryPoolManager.Alloc<DeltaTimer>();
            timer.SetTimer(TimeSpan.FromSeconds(3), _ =>
            {
                tmp.text = msg.Response.Power.ToString();
                MemoryPoolManager.Dealloc(timer);
            });
            timer.Restart();
        }
    }
}