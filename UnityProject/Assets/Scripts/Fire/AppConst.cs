using UnityEngine;

namespace Scripts.Fire
{
    public static class AppConst
    {
        private const string HTTP_SERVER_TEST = "http://192.168.110.35:378";
        private const string HTTP_SERVER_RELEASE = "http://192.168.110.35:378";

        public const string HTTP_SERVER_IP_DEV = HTTP_SERVER_TEST;
        public const string HTTP_SERVER_IP_RELEASE = HTTP_SERVER_RELEASE;

#if VERSION_DEV
        public static string HTTP_SERVER_IP = HTTP_SERVER_IP_DEV;
        public static bool DevMode = true;
#elif VERSION_RELEASE
    public static string HTTP_SERVER_IP = HTTP_SERVER_IP_RELEASE;
    public static bool DevMode = false;
#endif

        public const string AssetsDir = "assets";

        // 资源路径（编辑器下打包用）
        public static readonly string AssetsPath = Application.streamingAssetsPath + "/" + AssetsDir + "/";

        // 资源路径（运行时）
        public static readonly string StreamingAssetsPath = Application.streamingAssetsPath + "/" + AssetsDir + "/";
        public static readonly string PersistentDataPath = Application.persistentDataPath + "/" + AssetsDir + "/";

        // 远程资源路径
        public static readonly string RemoteAssetsPath = HTTP_SERVER_IP + "/SharedFiles/" + AssetsDir + "/";

        // 是否开启热更新
        public const bool HotUpdate = true;

        // 资源下载路径
        public static readonly string DownloadPath = Application.persistentDataPath + "/temp/";

        // 帧率
        public const int GameFrameRate = 60;

        // todo: 设备唯一标识码（Android、IOS） 要求卸载重装不变化
    }
}