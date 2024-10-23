using Framework.Modules.Audio;
using Framework.Modules.Pool;
using Framework.Modules.Setting;
using Framework.Modules.UI;
using Scripts.Fire.Singleton;

namespace Framework
{
    public class GameSystem : MonoSingleton<GameSystem>
    {
        /// <summary>
        /// 框架初始化
        /// </summary>
        private void Awake()
        {
            // UIPanelManager 初始化
            DontDestroyOnLoad(UIPanelManager.Instance.UIRoot);

            // AudioModule 初始化
            AudioModule.Instance.InstanceRoot.SetParent(transform);
            // ObjectPoolModule 初始化
            ObjectPoolModule.Instance.InstanceRoot.SetParent(transform);

            // SettingModule 初始化
            if (!SettingModule.Instance.Load())
            {
                throw new GameFrameworkException("SettingModule 初始化失败");
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}