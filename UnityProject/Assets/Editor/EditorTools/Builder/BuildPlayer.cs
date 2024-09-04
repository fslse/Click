using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildPlayer
{
    private const string VERSION_DEV = "VERSION_DEV";
    private const string VERSION_RELEASE = "VERSION_RELEASE";

#if VERSION_DEV // 开发环境
    private const string versionFlag = VERSION_DEV;
#elif VERSION_RELEASE // 发布环境
    private const string versionFlag = VERSION_RELEASE;
#endif

    private static readonly string PACKAGE_PATH = string.Format("Build/" + Application.productName + "_{0}_{1}_v{2}",
        DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), versionFlag, Application.version);

    [MenuItem("Builder/切换版本/开发环境", false, 1)]
    private static void ChangeToDev()
    {
        ChangeVersion(VERSION_DEV);
    }

    [MenuItem("Builder/切换版本/发布环境", false, 2)]
    private static void ChangeToRelease()
    {
        ChangeVersion(VERSION_RELEASE);
    }

    private static void ChangeVersion(string version)
    {
        ChangeBuildVersionForPlatform(version, BuildTargetGroup.Standalone);
        ChangeBuildVersionForPlatform(version, BuildTargetGroup.Android);
        ChangeBuildVersionForPlatform(version, BuildTargetGroup.iOS);
    }

    private static void ChangeBuildVersionForPlatform(string version, BuildTargetGroup platform)
    {
        string macro = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
        Debug.Log("platform: " + platform + "\nmacro_old: " + macro);
        string old_version = string.Empty;
        if (macro.Contains(VERSION_DEV))
        {
            old_version = VERSION_DEV;
        }
        else if (macro.Contains(VERSION_RELEASE))
        {
            old_version = VERSION_RELEASE;
        }

        if (!string.IsNullOrEmpty(old_version))
            macro = macro.Replace(old_version, version);
        else
            macro = macro + ";" + version;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, macro);
        Debug.Log("platform: " + platform + "\nmacro_new: " + macro);
        Debug.Log("platform: " + platform + "\nversion: " + old_version + " > " + version);
    }


    [MenuItem("Builder/Android/Build APK (empty)", false, 3)]
    private static void BuildAndroidApk()
    {
        BuildAndroid();
    }

    [MenuItem("Builder/Android/Build APK", false, 4)]
    private static void BuildAndroidApk_()
    {
        BuildAndroid(false);
    }

    [MenuItem("Builder/Android/Build AAB (empty)", false, 5)]
    private static void BuildAndroidAAB()
    {
        BuildAndroid(true, true);
    }

    [MenuItem("Builder/Android/Build AAB", false, 6)]
    private static void BuildAndroidAAB_()
    {
        BuildAndroid(false, true);
    }

    private static void BuildAndroid(bool empty = true, bool buildAppBundle = false)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle = buildAppBundle;

        var tempFolder = Environment.CurrentDirectory + "/StreamingAssets";
        var streamingAssetsFolder = Application.dataPath + "/StreamingAssets";

        if (empty)
        {
            if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
            MoveStreamingAssets(streamingAssetsFolder, tempFolder);

            BuildPipeline.BuildPlayer(GetScenePaths(),
                PACKAGE_PATH + (buildAppBundle ? "_Empty.aab" : "_Empty.apk"),
                BuildTarget.Android, BuildOptions.ShowBuiltPlayer);

            MoveStreamingAssets(tempFolder, streamingAssetsFolder);
            if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
        }
        else
        {
            BuildPipeline.BuildPlayer(GetScenePaths(),
                PACKAGE_PATH + (buildAppBundle ? ".aab" : ".apk"),
                BuildTarget.Android, BuildOptions.ShowBuiltPlayer);
        }
    }

    private static void MoveStreamingAssets(string sourcePath, string destPath)
    {
        if (Directory.Exists(sourcePath))
        {
            MoveDirectory(sourcePath, destPath);
        }
    }

    private static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    private class Folders
    {
        public string Source { get; private set; }
        public string Target { get; private set; }

        public Folders(string source, string target)
        {
            Source = source;
            Target = target;
        }
    }

    /// <summary>
    /// 移动整个文件夹（包含内部的所有文件及文件夹），覆盖同名文件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    private static void MoveDirectory(string source, string target)
    {
        var stack = new Stack<Folders>();
        stack.Push(new Folders(source, target));

        while (stack.Count > 0)
        {
            var folders = stack.Pop();
            Directory.CreateDirectory(folders.Target);
            foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
            {
                string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }

                File.Move(file, targetFile);
            }

            foreach (var folder in Directory.GetDirectories(folders.Source))
            {
                stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
            }
        }

        Directory.Delete(source, true);
    }
}