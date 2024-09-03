using UnityEngine;

namespace Framework;

public class AppConst
{
    private const string HTTP_SERVER_TEST = "";
    private const string HTTP_SERVER_RELEASE = "";

    public const string HTTP_SERVER_IP_DEV = HTTP_SERVER_TEST;
    public const string HTTP_SERVER_IP_RELEASE = HTTP_SERVER_RELEASE;

#if VERSION_DEV
    public static string HTTP_SERVER_IP = HTTP_SERVER_IP_DEV;
    public static bool DevMode = true;
#elif VERSION_RELEASE
    public static string HTTP_SERVER_IP = HTTP_SERVER_IP_RELEASE;
    public static bool DevMode = false;
#endif

    // 是否开启热更新
    public const bool HotUpdate = true;

    // 帧率
    public const int GameFrameRate = 60;

#if UNITY_EDITOR
    public static string AssetDir = Application.streamingAssetsPath + "/assets/";
#else // 资源路径
    public static string AssetDir = Application.persistentDataPath + "/assets/";
#endif

    // todo: 设备唯一标识码（Android、IOS） 要求卸载重装不变化
}