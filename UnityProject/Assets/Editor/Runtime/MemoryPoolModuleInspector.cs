using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework;
using Framework.Core.MemoryManagement;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameApp))]
internal sealed class MemoryPoolModuleInspector : GameFrameworkInspector
{
    private readonly Dictionary<string, List<MemoryPoolInfo>> memoryPoolInfos = new(StringComparer.Ordinal);
    private readonly HashSet<string> openedItems = new();
    private bool showFullClassName;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
                { richText = true, fontSize = 16, fontStyle = FontStyle.BoldAndItalic, fixedHeight = 32, alignment = TextAnchor.UpperLeft };
            EditorGUILayout.LabelField("<color=white>Memory Pool</color>", style);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Memory Pool Count", MemoryPool.Count.ToString());
            showFullClassName = EditorGUILayout.Toggle("Show Full Class Name", showFullClassName);

            // 收集内存池信息 按对象类型所属程序集分类
            memoryPoolInfos.Clear();
            MemoryPoolInfo[] infos = MemoryPool.GetAllMemoryPoolInfos();
            foreach (MemoryPoolInfo memoryPoolInfo in infos)
            {
                string assemblyName = memoryPoolInfo.Type.Assembly.GetName().Name;
                if (!memoryPoolInfos.TryGetValue(assemblyName, out var results))
                {
                    results = new List<MemoryPoolInfo>();
                    memoryPoolInfos.Add(assemblyName, results);
                }

                results.Add(memoryPoolInfo);
            }

            // 遍历程序集 显示内存池信息
            foreach (KeyValuePair<string, List<MemoryPoolInfo>> assemblyMemoryPoolInfo in memoryPoolInfos)
            {
                bool lastState = openedItems.Contains(assemblyMemoryPoolInfo.Key);
                bool currentState = EditorGUILayout.Foldout(lastState, assemblyMemoryPoolInfo.Key);
                if (currentState != lastState)
                {
                    if (currentState)
                    {
                        openedItems.Add(assemblyMemoryPoolInfo.Key);
                    }
                    else
                    {
                        openedItems.Remove(assemblyMemoryPoolInfo.Key);
                    }
                }

                if (currentState)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.LabelField(showFullClassName ? "Full Class Name" : "Class Name", "Unused\tUsing\tAcquire\tRelease\tAdd\tRemove");
                        assemblyMemoryPoolInfo.Value.Sort(Comparison);
                        foreach (MemoryPoolInfo memoryPoolInfo in assemblyMemoryPoolInfo.Value)
                        {
                            DrawMemoryPoolInfo(memoryPoolInfo);
                        }

                        if (GUILayout.Button("Export CSV Data"))
                        {
                            string exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", string.Empty,
                                $"Memory Pool Data - {assemblyMemoryPoolInfo.Key}.csv", string.Empty);
                            if (!string.IsNullOrEmpty(exportFileName))
                            {
                                try
                                {
                                    int index = 0;
                                    string[] data = new string[assemblyMemoryPoolInfo.Value.Count + 1];
                                    data[index++] = "Class Name,Full Class Name,Unused,Using,Acquire,Release,Add,Remove";
                                    foreach (MemoryPoolInfo memoryPoolInfo in assemblyMemoryPoolInfo.Value)
                                    {
                                        data[index++] =
                                            $"{memoryPoolInfo.Type.Name},{memoryPoolInfo.Type.FullName}," +
                                            $"{memoryPoolInfo.UnusedMemoryCount.ToString()},{memoryPoolInfo.UsingMemoryCount.ToString()}," +
                                            $"{memoryPoolInfo.AcquireMemoryCount.ToString()},{memoryPoolInfo.ReleaseMemoryCount.ToString()}," +
                                            $"{memoryPoolInfo.AddMemoryCount.ToString()},{memoryPoolInfo.RemoveMemoryCount.ToString()}";
                                    }

                                    File.WriteAllLines(exportFileName, data, Encoding.UTF8);
                                    Debug.Log($"Export memory pool CSV data to '{exportFileName}' success.");
                                }
                                catch (Exception exception)
                                {
                                    Debug.LogError($"Export memory pool CSV data to '{exportFileName}' failure, exception is '{exception}'.");
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Separator();
                }
            }
        }

        Repaint();
    }

    private void DrawMemoryPoolInfo(MemoryPoolInfo memoryPoolInfo)
    {
        EditorGUILayout.LabelField(showFullClassName ? memoryPoolInfo.Type.FullName : memoryPoolInfo.Type.Name,
            $"{memoryPoolInfo.UnusedMemoryCount.ToString()}\t{memoryPoolInfo.UsingMemoryCount.ToString()}\t" +
            $"{memoryPoolInfo.AcquireMemoryCount.ToString()}\t{memoryPoolInfo.ReleaseMemoryCount.ToString()}\t" +
            $"{memoryPoolInfo.AddMemoryCount.ToString()}\t{memoryPoolInfo.RemoveMemoryCount.ToString()}");
    }

    private int Comparison(MemoryPoolInfo a, MemoryPoolInfo b)
    {
        return showFullClassName ? string.Compare(a.Type.FullName, b.Type.FullName, StringComparison.Ordinal) : string.Compare(a.Type.Name, b.Type.Name, StringComparison.Ordinal);
    }
}