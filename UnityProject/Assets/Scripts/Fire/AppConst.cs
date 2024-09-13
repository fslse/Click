using UnityEngine;

namespace Scripts.Fire
{
    public static class AppConst
    {
        private const string HTTP_SERVER_TEST = "https://d1ox7sz49rwsv1.cloudfront.net";
        private const string HTTP_SERVER_RELEASE = "https://d1ox7sz49rwsv1.cloudfront.net";

#if VERSION_DEV
        private const string HTTP_SERVER_ADDRESS = HTTP_SERVER_TEST;
        public static bool DevMode = true;
#elif VERSION_RELEASE
        private const string HTTP_SERVER_ADDRESS = HTTP_SERVER_RELEASE;
        public static bool DevMode = false;
#endif

        public const string AssetsDir = "assets";

        // 资源路径（编辑器下打包用）
        public static readonly string AssetsPath = $"{Application.streamingAssetsPath}/{AssetsDir}/";

        // 资源路径（运行时）
        public static readonly string StreamingAssetsPath = $"{Application.streamingAssetsPath}/{AssetsDir}/";
        public static readonly string PersistentDataPath = $"{Application.persistentDataPath}/{AssetsDir}/";

        // 远程资源路径
        public const string RemoteAssetsPath = $"{HTTP_SERVER_ADDRESS}/test/{AssetsDir}/";

        // 是否开启热更新
        public const bool HotUpdate = true;

        // 帧率
        public const int GameFrameRate = 60;

        // todo: 设备唯一标识码（Android、IOS） 要求卸载重装不变化
    }
}