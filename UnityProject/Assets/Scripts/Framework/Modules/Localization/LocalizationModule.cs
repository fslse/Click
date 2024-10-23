using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Text;
using I2.Loc;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using Scripts.Fire.Singleton;
using UnityEngine;

namespace Framework.Modules.Localization
{
    public class LocalizationModule : Singleton<LocalizationModule>
    {
        // 默认语言。 初始化时切换系统语言失败时使用此语言。
        private const string defaultLanguage = "English (United States)";

        // 当前语言。 初始化时切换为系统语言或默认语言。
        private string currentLanguage;

        private readonly LanguageSourceData sourceData; // 语言数据
        private readonly List<string> allLanguages = new(); // 支持的所有语言

        /// <summary>
        /// 获取或设置当前语言。
        /// </summary>
        public Language Language
        {
            get => DefaultLocalizationHelper.GetLanguage(currentLanguage);
            set => SetLanguage(DefaultLocalizationHelper.GetLanguageStr(value));
        }

        /// <summary>
        /// 获取系统语言。
        /// </summary>
        public Language SystemLanguage => DefaultLocalizationHelper.SystemLanguage;

        /// <summary>
        /// 场景上本地化模块根节点。
        /// </summary>
        public Transform InstanceRoot { get; }

        private LocalizationModule()
        {
            InstanceRoot = new GameObject("Localization").transform;
            InstanceRoot.localPosition = Vector3.zero;
            Object.DontDestroyOnLoad(InstanceRoot.gameObject);

            sourceData = InstanceRoot.gameObject.AddComponent<LanguageSource>().SourceData;
            sourceData.Awake();

            UseLocalizationCSV(AssetManager.Instance.LoadAsset<TextAsset>("Assets/AssetPackages/Localization/I2_Localization.csv").text, true);
            if (!SetLanguage(SystemLanguage))
                SetLanguage(defaultLanguage);
        }

        /// <summary>
        /// 检查语言是否支持。
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool CheckLanguage(string language)
        {
            return allLanguages.Contains(language);
        }

        /// <summary>
        /// 设置语言。
        /// </summary>
        /// <param name="language"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        public bool SetLanguage(Language language, bool load = false)
        {
            return SetLanguage(DefaultLocalizationHelper.GetLanguageStr(language), load);
        }

        /// <summary>
        /// 设置语言。
        /// </summary>
        /// <param name="language"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        public bool SetLanguage(string language, bool load = false)
        {
            if (!CheckLanguage(language))
            {
                return false;
            }

            if (currentLanguage == language)
                return true;
            LocalizationManager.CurrentLanguage = language;
            currentLanguage = language;
            return true;
        }

        private void UseLocalizationCSV(string CSVfile, bool isLocalizeAll = false)
        {
            sourceData.Import_CSV(string.Empty, CSVfile, eSpreadsheetUpdateMode.Merge);
            if (isLocalizeAll)
                LocalizationManager.LocalizeAll();
            UpdateAllLanguages();
        }

        private void UpdateAllLanguages()
        {
            allLanguages.Clear();
            var languages = LocalizationManager.GetAllLanguages();
            allLanguages.AddRange(languages.Select(languageStr => Regex.Replace(languageStr, @"[\r\n]", "")));
            GameLog.LogWarning($"Update all languages\n{ZString.Join("\n", allLanguages)}");
        }
    }
}