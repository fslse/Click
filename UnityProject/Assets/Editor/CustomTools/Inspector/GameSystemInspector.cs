using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Framework;
using Framework.Core.Memory;
using Framework.Modules.Pool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameSystem))]
internal sealed class GameSystemInspector : GameFrameworkInspector
{
    // MemoryPool
    private readonly Dictionary<string, List<MemoryPoolInfo>> memoryPoolInfos = new(StringComparer.Ordinal);
    private readonly HashSet<string> openedItemsInMemoryPool = new();
    private bool showFullClassName;

    // ObjectPool
    private readonly HashSet<string> openedItemsInObjectPool = new();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            richText = true,
            fontSize = 16,
            fixedHeight = 32,
            alignment = TextAnchor.UpperLeft,
            fontStyle = FontStyle.BoldAndItalic
        };

        EditorGUILayout.LabelField("<color=white>Memory Pool</color>", style);
        EditorGUILayout.Separator();
        DrawMemoryPool();

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("<color=white>Object Pool</color>", style);
        EditorGUILayout.Separator();
        DrawObjectPool();

        Repaint();
    }

    private void DrawObjectPool()
    {
        EditorGUILayout.LabelField("Object Pool Count", ObjectPoolModule.Instance.Count.ToString());
        ObjectPoolBase[] objectPools = ObjectPoolModule.Instance.GetAllObjectPools(true);
        foreach (ObjectPoolBase objectPool in objectPools)
        {
            bool lastState = openedItemsInObjectPool.Contains(objectPool.FullName);
            bool currentState = EditorGUILayout.Foldout(lastState, objectPool.FullName);
            if (currentState != lastState)
            {
                if (currentState)
                {
                    openedItemsInObjectPool.Add(objectPool.FullName);
                }
                else
                {
                    openedItemsInObjectPool.Remove(objectPool.FullName);
                }
            }

            if (currentState)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("Name", objectPool.Name);
                    EditorGUILayout.LabelField("Type", objectPool.ObjectType.FullName);
                    EditorGUILayout.LabelField("Auto Release Interval", objectPool.AutoReleaseInterval.ToString(CultureInfo.InvariantCulture));
                    EditorGUILayout.LabelField("Capacity", objectPool.Capacity.ToString());
                    EditorGUILayout.LabelField("Used Count", objectPool.Count.ToString());
                    EditorGUILayout.LabelField("Can Release Count", objectPool.CanReleaseCount.ToString());
                    EditorGUILayout.LabelField("Expire Time", objectPool.ExpireTime.ToString(CultureInfo.InvariantCulture));
                    EditorGUILayout.LabelField("Priority", objectPool.Priority.ToString());
                    ObjectInfo[] objectInfos = objectPool.GetAllObjectInfos();
                    if (objectInfos.Length > 0)
                    {
                        EditorGUILayout.LabelField("Name",
                            objectPool.AllowMultiSpawn ? "Locked\tCount\tFlag\tPriority\tLast Use Time" : "Locked\tIn Use\tFlag\tPriority\tLast Use Time");
                        foreach (ObjectInfo objectInfo in objectInfos)
                        {
                            EditorGUILayout.LabelField(string.IsNullOrEmpty(objectInfo.Name) ? "<None>" : objectInfo.Name,
                                objectPool.AllowMultiSpawn
                                    ? $"{objectInfo.Locked}\t{objectInfo.SpawnCount}\t{objectInfo.CustomCanReleaseFlag}\t{objectInfo.Priority}\t{objectInfo.LastUseTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}"
                                    : $"{objectInfo.Locked}\t{objectInfo.IsInUse}\t{objectInfo.CustomCanReleaseFlag}\t{objectInfo.Priority}\t{objectInfo.LastUseTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
                        }

                        if (GUILayout.Button("Release"))
                        {
                            objectPool.Release();
                        }

                        if (GUILayout.Button("Release All Unused"))
                        {
                            objectPool.ReleaseAllUnused();
                        }

                        if (GUILayout.Button("Export CSV Data"))
                        {
                            string exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", string.Empty,
                                $"Object Pool Data - {objectPool.Name}.csv",
                                string.Empty);
                            if (!string.IsNullOrEmpty(exportFileName))
                            {
                                try
                                {
                                    int index = 0;
                                    string[] data = new string[objectInfos.Length + 1];
                                    data[index++] = $"Name,Locked,{(objectPool.AllowMultiSpawn ? "Count" : "In Use")},Custom Can Release Flag,Priority,Last Use Time";
                                    foreach (ObjectInfo objectInfo in objectInfos)
                                    {
                                        data[index++] = objectPool.AllowMultiSpawn
                                            ? $"{objectInfo.Name},{objectInfo.Locked},{objectInfo.SpawnCount},{objectInfo.CustomCanReleaseFlag},{objectInfo.Priority},{objectInfo.LastUseTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}"
                                            : $"{objectInfo.Name},{objectInfo.Locked},{objectInfo.IsInUse},{objectInfo.CustomCanReleaseFlag},{objectInfo.Priority},{objectInfo.LastUseTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}";
                                    }

                                    File.WriteAllLines(exportFileName, data, Encoding.UTF8);
                                    Debug.Log($"Export object pool CSV data to '{exportFileName}' success.");
                                }
                                catch (Exception exception)
                                {
                                    Debug.LogError($"Export object pool CSV data to '{exportFileName}' failure, exception is '{exception}'.");
                                }
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label("Object Pool is Empty ...");
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }
        }
    }

    private void DrawMemoryPool()
    {
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
            bool lastState = openedItemsInMemoryPool.Contains(assemblyMemoryPoolInfo.Key);
            bool currentState = EditorGUILayout.Foldout(lastState, assemblyMemoryPoolInfo.Key);
            if (currentState != lastState)
            {
                if (currentState)
                {
                    openedItemsInMemoryPool.Add(assemblyMemoryPoolInfo.Key);
                }
                else
                {
                    openedItemsInMemoryPool.Remove(assemblyMemoryPoolInfo.Key);
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
                                    data[index++] = $"{memoryPoolInfo.Type.Name},{memoryPoolInfo.Type.FullName}," +
                                                    $"{memoryPoolInfo.UnusedMemoryCount},{memoryPoolInfo.UsingMemoryCount}," +
                                                    $"{memoryPoolInfo.AcquireMemoryCount},{memoryPoolInfo.ReleaseMemoryCount}," +
                                                    $"{memoryPoolInfo.AddMemoryCount},{memoryPoolInfo.RemoveMemoryCount}";
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

    private void DrawMemoryPoolInfo(MemoryPoolInfo memoryPoolInfo)
    {
        EditorGUILayout.LabelField(showFullClassName ? memoryPoolInfo.Type.FullName : memoryPoolInfo.Type.Name,
            $"{memoryPoolInfo.UnusedMemoryCount}\t{memoryPoolInfo.UsingMemoryCount}\t" +
            $"{memoryPoolInfo.AcquireMemoryCount}\t{memoryPoolInfo.ReleaseMemoryCount}\t" +
            $"{memoryPoolInfo.AddMemoryCount}\t{memoryPoolInfo.RemoveMemoryCount}");
    }

    private int Comparison(MemoryPoolInfo a, MemoryPoolInfo b)
    {
        return showFullClassName ? string.Compare(a.Type.FullName, b.Type.FullName, StringComparison.Ordinal) : string.Compare(a.Type.Name, b.Type.Name, StringComparison.Ordinal);
    }
}