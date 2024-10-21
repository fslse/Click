using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

public static class Define
{
    public const string NormalAtlasDir = "Assets/AssetArt/Atlas";
    public const string SpritePath = "Assets/AssetPackages/Texture/Sprite";
    public const string AtlasPath = "Assets/AssetPackages/Texture/Atlas";
}

/// <summary>
/// Sprite导入管线。
/// </summary>
public class SpritePostprocessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool flag = false;

        deletedAssets = deletedAssets.Concat(movedFromAssetPaths).ToArray();
        deletedAssets = new HashSet<string>(deletedAssets).Where(s => s.StartsWith(Define.AtlasPath) || s.StartsWith(Define.SpritePath)).ToArray();
        deletedAssets = deletedAssets.Where(s => s.Contains('.')).ToArray();
        foreach (string s in deletedAssets)
        {
            EditorSpriteSaveInfo.OnDeleteSprite(s);
        }

        if (deletedAssets.Length > 0)
        {
            flag = true;
            Debug.Log($"Delete Sprite Count: {deletedAssets.Length}\nINFO:\n{string.Join("\n", deletedAssets)}");
        }

        importedAssets = importedAssets.Concat(movedAssets).ToArray();
        importedAssets = new HashSet<string>(importedAssets).Where(s => s.StartsWith(Define.AtlasPath) || s.StartsWith(Define.SpritePath)).ToArray();
        importedAssets = importedAssets.Where(s => s.Contains('.')).ToArray();
        foreach (string s in importedAssets)
        {
            EditorSpriteSaveInfo.OnImportSprite(s);
        }

        if (importedAssets.Length > 0)
        {
            flag = true;
            Debug.Log($"Import Sprite Count: {importedAssets.Length}\nINFO:\n{string.Join("\n", importedAssets)}");
        }

        if (flag)
            Debug.LogWarning("SpritePostprocessor");

        foreach (var s in movedAssets)
        {
            if (s.StartsWith(Define.AtlasPath) || s.StartsWith(Define.SpritePath)) continue;
            var name = Path.GetFileNameWithoutExtension(s);
            int index = name.IndexOf('`');
            if (index == -1) continue;
            var _ = name[..index];
            if (_.StartsWith("Atlas_") || _.StartsWith("Sprite_"))
            {
                AssetDatabase.RenameAsset(s, name[(index + 1)..]);
                AssetDatabase.Refresh();
            }
        }
    }
}

public static class EditorSpriteSaveInfo
{
    private static readonly string[] platformNames = { "Android", "iPhone", "WebGL" };

    private static readonly Dictionary<string, List<string>> allSprites = new();
    private static readonly List<string> dirtyAtlasList = new();

    private static bool isInit;
    private static bool dirty;

    private static void Init()
    {
        if (isInit)
        {
            return;
        }

        EditorApplication.update += CheckDirty;

        //读取所有图集信息 
        string[] findAssets = AssetDatabase.FindAssets("t:SpriteAtlas", new[] { Define.NormalAtlasDir }); // 返回的是GUID
        foreach (var findAsset in findAssets)
        {
            var path = AssetDatabase.GUIDToAssetPath(findAsset); // 路径 Assets/AssetArt/Atlas/*.spriteatlas
            SpriteAtlas sa = AssetDatabase.LoadAssetAtPath(path, typeof(SpriteAtlas)) as SpriteAtlas;
            if (sa == null)
            {
                Debug.LogError($"Failed to load {path}");
                continue;
            }

            string atlasName = Path.GetFileNameWithoutExtension(path); // 图集文件名 不含.spriteatlas
            var objects = sa.GetPackables(); // Return all the current packed packable objects in the atlas.
            foreach (var o in objects)
            {
                if (!allSprites.TryGetValue(atlasName, out var list))
                {
                    list = new List<string>();
                    allSprites.Add(atlasName, list);
                }

                list.Add(AssetDatabase.GetAssetPath(o)); // 路径 Assets/AssetPackages/Images/Atlas/*
            }
        }

        isInit = true;
    }

    private static void CheckDirty()
    {
        if (dirty)
        {
            dirty = false;

            AssetDatabase.Refresh();
            float lastProgress = -1;
            for (int i = 0; i < dirtyAtlasList.Count; i++)
            {
                string atlasName = dirtyAtlasList[i];
                Debug.LogWarning("Update atlas: " + atlasName);
                var curProgress = (float)i / dirtyAtlasList.Count;
                if (curProgress > lastProgress + 0.01f)
                {
                    lastProgress = curProgress;
                    var progressText = $"Progress: {i}/{dirtyAtlasList.Count} {atlasName}";
                    bool cancel = EditorUtility.DisplayCancelableProgressBar($"Update atlas: {atlasName}", progressText, curProgress);
                    if (cancel)
                    {
                        break;
                    }
                }

                UpdateAtlas(atlasName);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            dirtyAtlasList.Clear();
        }
    }

    public static void OnImportSprite(string assetPath)
    {
        TextureImporter ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (ti != null)
        {
            var modify = false;

            if (ti.textureType != TextureImporterType.Sprite) // 纹理类型修改为 Sprite
            {
                ti.textureType = TextureImporterType.Sprite;
                modify = true;
            }

            var setting = new TextureImporterSettings();
            ti.ReadTextureSettings(setting);
            if (setting.spriteGenerateFallbackPhysicsShape) // inspector中的 Generate Physics Shape 是否勾选
            {
                setting.spriteGenerateFallbackPhysicsShape = false;
                ti.SetTextureSettings(setting);
                modify = true;
            }

            if (assetPath.StartsWith(Define.SpritePath)) //如果保持散图
            {
                foreach (var name in platformNames)
                {
                    var platformSettings = ti.GetPlatformTextureSettings(name);

                    if (!platformSettings.overridden)
                    {
                        platformSettings.overridden = true;
                        modify = true;
                    }

                    if (platformSettings.compressionQuality != 50)
                    {
                        platformSettings.compressionQuality = 50;
                        modify = true;
                    }

                    switch (name)
                    {
                        case "Android":
                        {
                            if (platformSettings.format != TextureImporterFormat.ASTC_6x6)
                            {
                                platformSettings.format = TextureImporterFormat.ASTC_6x6;
                                modify = true;
                            }

                            break;
                        }
                        case "iPhone":
                        {
                            if (platformSettings.format != TextureImporterFormat.ASTC_5x5)
                            {
                                platformSettings.format = TextureImporterFormat.ASTC_5x5;
                                modify = true;
                            }

                            break;
                        }
                        case "WebGL":
                        {
                            if (platformSettings.format != TextureImporterFormat.ASTC_6x6)
                            {
                                platformSettings.format = TextureImporterFormat.ASTC_6x6;
                                modify = true;
                            }

                            break;
                        }
                    }

                    ti.SetPlatformTextureSettings(platformSettings);
                }
            }
            else if (assetPath.StartsWith(Define.AtlasPath)) //如果是图集
            {
                foreach (var name in platformNames)
                {
                    var platformSettings = ti.GetPlatformTextureSettings(name);

                    if (platformSettings.overridden)
                    {
                        platformSettings.overridden = false;
                        modify = true;
                    }

                    ti.SetPlatformTextureSettings(platformSettings);
                }

                if (ti.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    ti.textureCompression = TextureImporterCompression.Uncompressed;
                    modify = true;
                }
            }

            if (modify)
            {
                ti.SaveAndReimport();
                AssetDatabase.Refresh(); // 刷新后似乎可以避免上一步触发的 OnPostprocessAllAssets
            }

            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            string _ = assetPath[..assetPath.LastIndexOf('/')].Replace("Assets/AssetPackages/Texture/", "").Replace('/', '_');

            var s = assetName[(assetName.IndexOf('`') + 1)..].Split('_');
            var newAssetName = $"{_}`{string.Join("_", s.Select(t => t[0].ToString().ToUpper() + t[1..]))}";

            if (assetName != newAssetName)
            {
                AssetDatabase.RenameAsset(assetPath, newAssetName);
                AssetDatabase.Refresh();
                assetPath = assetPath.Replace(assetName, newAssetName);
            }

            OnProcessSprite(assetPath);
        }
    }

    private static void OnProcessSprite(string assetPath)
    {
        if (!assetPath.StartsWith(Define.AtlasPath))
        {
            return;
        }

        Init();

        string atlasName = GetAtlasName(assetPath);
        if (string.IsNullOrEmpty(atlasName))
        {
            Debug.LogError($"Failed to get atlas name: {assetPath}");
            return;
        }

        if (!allSprites.TryGetValue(atlasName, out var ret))
        {
            ret = new List<string>();
            allSprites.Add(atlasName, ret);
        }

        if (!ret.Contains(assetPath))
        {
            ret.Add(assetPath);
            dirty = true;
            if (!dirtyAtlasList.Contains(atlasName))
            {
                dirtyAtlasList.Add(atlasName);
            }
        }
    }

    public static void OnDeleteSprite(string assetPath)
    {
        if (!assetPath.StartsWith(Define.AtlasPath))
        {
            return;
        }

        Init();

        string atlasName = GetAtlasName(assetPath);
        if (string.IsNullOrEmpty(atlasName))
        {
            Debug.LogError($"Failed to get atlas name: {assetPath}");
            return;
        }

        if (!allSprites.TryGetValue(atlasName, out var ret))
        {
            return;
        }

        // 文件名匹配
        if (!ret.Exists(s => Path.GetFileName(s) == Path.GetFileName(assetPath)))
        {
            return;
        }

        ret.Remove(assetPath);
        dirty = true;
        if (!dirtyAtlasList.Contains(atlasName))
        {
            dirtyAtlasList.Add(atlasName);
        }
    }

    /// <summary>
    /// 根据文件路径，返回图集名称
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    private static string GetAtlasName(string assetPath)
    {
        if (!assetPath.StartsWith(Define.AtlasPath))
        {
            return "";
        }

        var atlasName = assetPath[assetPath.IndexOf("Atlas", StringComparison.Ordinal)..];
        return atlasName[..atlasName.LastIndexOf('/')].Replace("/", "_");
    }

    #region 更新图集

    private static void UpdateAtlas(string atlasName)
    {
        List<Object> spriteList = new List<Object>();
        if (allSprites.TryGetValue(atlasName, out var list))
        {
            list.Sort(StringComparer.Ordinal);
            spriteList.AddRange(list.Select(AssetDatabase.LoadAssetAtPath<Sprite>).Where(sprite => sprite != null));
        }

        var path = $"{Define.NormalAtlasDir}/{atlasName}.spriteatlas";

        if (spriteList.Count == 0)
        {
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }

            return;
        }

        var atlas = new SpriteAtlas();
        var setting = new SpriteAtlasPackingSettings
        {
            enableRotation = true,
            enableTightPacking = false, // Determines if sprites should be packed tightly during packing.
            enableAlphaDilation = false, // Sets the boundary padding pixels alpha to 0 when packed into a Sprite Atlas if true.
            blockOffset = 1,
            padding = 2 // Value to add boundary (padding) to sprites when packing into the atlas.
        };

        var textureSetting = new SpriteAtlasTextureSettings
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear
        };
        atlas.SetTextureSettings(textureSetting);

        foreach (var name in platformNames)
        {
            var platformSetting = atlas.GetPlatformSettings(name);
            if (!platformSetting.overridden)
            {
                platformSetting.overridden = true;
                platformSetting.format = name switch
                {
                    "Android" => TextureImporterFormat.ASTC_6x6,
                    "iPhone" => TextureImporterFormat.ASTC_5x5,
                    "WebGL" => TextureImporterFormat.ASTC_6x6,
                    _ => TextureImporterFormat.ASTC_6x6
                };
                platformSetting.compressionQuality = name switch
                {
                    "Android" => 100,
                    "iPhone" => 100,
                    "WebGL" => 50,
                    _ => 50
                };

                atlas.SetPlatformSettings(platformSetting);
            }
        }

        atlas.SetPackingSettings(setting);
        atlas.Add(spriteList.ToArray());

        AssetDatabase.CreateAsset(atlas, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #endregion

    #region 生成图集

    private static readonly Dictionary<string, List<string>> tempAllASprites = new();

    public static void GenAtlas(bool force = false)
    {
        Init();

        if (force)
            allSprites.Clear();

        tempAllASprites.Clear();
        var findAssets = AssetDatabase.FindAssets("t:sprite", new[] { Define.AtlasPath });
        foreach (var findAsset in findAssets)
        {
            var path = AssetDatabase.GUIDToAssetPath(findAsset);
            var atlasName = GetAtlasName(path);
            if (!tempAllASprites.TryGetValue(atlasName, out var spriteList))
            {
                spriteList = new List<string>();
                tempAllASprites[atlasName] = spriteList;
            }

            if (!spriteList.Contains(path))
            {
                spriteList.Add(path);
            }
        }

        List<string> needSaveAtlas = new List<string>();

        // 有变化才刷新
        var it = tempAllASprites.GetEnumerator();
        while (it.MoveNext())
        {
            bool needSave = false;
            var atlasName = it.Current.Key;
            var spriteList = it.Current.Value;

            if (allSprites.TryGetValue(atlasName, out var existedSprites))
            {
                if (existedSprites.Count != spriteList.Count)
                {
                    needSave = true;
                    existedSprites.Clear();
                    existedSprites.AddRange(spriteList);
                }
                else
                {
                    if (spriteList.Any(t => !existedSprites.Contains(t)))
                    {
                        needSave = true;
                        existedSprites.Clear();
                        existedSprites.AddRange(spriteList);
                    }
                }
            }
            else
            {
                needSave = true;
                allSprites.Add(atlasName, new List<string>(spriteList));
            }

            if (needSave && !needSaveAtlas.Contains(atlasName))
            {
                needSaveAtlas.Add(atlasName);
            }
        }

        it.Dispose();
        foreach (var atlas in needSaveAtlas)
        {
            Debug.LogFormat("Gen atlas: {0}", atlas);
            UpdateAtlas(atlas);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget);
        Debug.LogWarning("Complete");
    }

    #endregion
}