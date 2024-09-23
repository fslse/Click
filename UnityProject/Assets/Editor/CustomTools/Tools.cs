using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class Tools
{
    [MenuItem("Tools/Customer Tools/Configuration Tables", false, -999999)]
    private static void Gen()
    {
        Process process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = $"{Application.dataPath}/../Luban-Tools/Luban/Luban.dll " +
                                      $"-t client " +
                                      $"-c cs-newtonsoft-json " +
                                      $"-d json " +
                                      $"--conf {Application.dataPath}/AssetPackages/Config/DataTables/config.json " +
                                      $"-x outputCodeDir={Application.dataPath}/Scripts/HotFix/Config " +
                                      $"-x outputDataDir={Application.dataPath}/AssetPackages/Config/Json";

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode == 0)
            UnityEngine.Debug.Log($"Success\n{output}");
        else
            UnityEngine.Debug.LogError($"Error\n{error}");

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Customer Tools/Protocol Buffers", false, -888888)]
    private static void Gen1()
    {
        string path = $"{Application.dataPath}/Scripts/HotFix/Protocol";
        DirectoryInfo dir = Directory.CreateDirectory(path);
        FileInfo[] files = dir.GetFiles();
        foreach (var file in files)
            if (file.Extension == ".proto")
            {
                Process process = new Process();
                process.StartInfo.FileName = $"{Application.dataPath}/../Google.Protobuf_28.2/protoc-28.2-win64/bin/protoc.exe";
                process.StartInfo.Arguments = $"--proto_path={path}/ --csharp_out={path}/cs {file.Name}";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode == 0)
                    UnityEngine.Debug.Log($"{file.Name}\n{output}");
                else
                    UnityEngine.Debug.LogError($"Error\n{error}");
            }

        AssetDatabase.Refresh();
    }
}