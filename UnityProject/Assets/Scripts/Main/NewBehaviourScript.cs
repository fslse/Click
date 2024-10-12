using Cysharp.Threading.Tasks;
using Framework;
using Scripts.Fire.Startup;
using UnityEngine;

namespace Main
{
    public class NewBehaviourScript : MonoBehaviour
    {
        private void Awake()
        {
            Init().Forget();
        }

        /// <summary>
        /// 业务初始化 = 20%
        /// </summary>
        private async UniTaskVoid Init()
        {
            // todo: sdk初始化、向服务器发送请求、用户数据初始化 20%

            UniRx.MessageBroker.Default.Publish(StartupProgressMessage.Instance.Message(0.9f));
            GameApp.Instance.StartGame().Forget();
        }
    }
}