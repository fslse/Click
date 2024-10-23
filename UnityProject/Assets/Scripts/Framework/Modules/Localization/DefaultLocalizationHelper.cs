using System.Collections.Generic;
using UnityEngine;

namespace Framework.Modules.Localization
{
    /// <summary>
    /// 默认本地化辅助器。
    /// </summary>
    public class DefaultLocalizationHelper
    {
#if UNITY_EDITOR
        public const string I2GlobalSourcesEditorPath = "Assets/Editor/I2/I2Languages.asset";
#endif

        public const string I2ResAssetNamePrefix = "I2_";

        /// <summary>
        /// 获取系统语言。
        /// </summary>
        public static Language SystemLanguage
        {
            get
            {
                return Application.systemLanguage switch
                {
                    UnityEngine.SystemLanguage.Afrikaans => Language.Afrikaans,
                    UnityEngine.SystemLanguage.Arabic => Language.Arabic,
                    UnityEngine.SystemLanguage.Basque => Language.Basque,
                    UnityEngine.SystemLanguage.Belarusian => Language.Belarusian,
                    UnityEngine.SystemLanguage.Bulgarian => Language.Bulgarian,
                    UnityEngine.SystemLanguage.Catalan => Language.Catalan,
                    UnityEngine.SystemLanguage.Chinese => Language.ChineseSimplified,
                    UnityEngine.SystemLanguage.ChineseSimplified => Language.ChineseSimplified,
                    UnityEngine.SystemLanguage.ChineseTraditional => Language.ChineseTraditional,
                    UnityEngine.SystemLanguage.Czech => Language.Czech,
                    UnityEngine.SystemLanguage.Danish => Language.Danish,
                    UnityEngine.SystemLanguage.Dutch => Language.Dutch,
                    UnityEngine.SystemLanguage.English => Language.English,
                    UnityEngine.SystemLanguage.Estonian => Language.Estonian,
                    UnityEngine.SystemLanguage.Faroese => Language.Faroese,
                    UnityEngine.SystemLanguage.Finnish => Language.Finnish,
                    UnityEngine.SystemLanguage.French => Language.French,
                    UnityEngine.SystemLanguage.German => Language.German,
                    UnityEngine.SystemLanguage.Greek => Language.Greek,
                    UnityEngine.SystemLanguage.Hebrew => Language.Hebrew,
                    UnityEngine.SystemLanguage.Hungarian => Language.Hungarian,
                    UnityEngine.SystemLanguage.Icelandic => Language.Icelandic,
                    UnityEngine.SystemLanguage.Indonesian => Language.Indonesian,
                    UnityEngine.SystemLanguage.Italian => Language.Italian,
                    UnityEngine.SystemLanguage.Japanese => Language.Japanese,
                    UnityEngine.SystemLanguage.Korean => Language.Korean,
                    UnityEngine.SystemLanguage.Latvian => Language.Latvian,
                    UnityEngine.SystemLanguage.Lithuanian => Language.Lithuanian,
                    UnityEngine.SystemLanguage.Norwegian => Language.Norwegian,
                    UnityEngine.SystemLanguage.Polish => Language.Polish,
                    UnityEngine.SystemLanguage.Portuguese => Language.PortuguesePortugal,
                    UnityEngine.SystemLanguage.Romanian => Language.Romanian,
                    UnityEngine.SystemLanguage.Russian => Language.Russian,
                    UnityEngine.SystemLanguage.SerboCroatian => Language.SerboCroatian,
                    UnityEngine.SystemLanguage.Slovak => Language.Slovak,
                    UnityEngine.SystemLanguage.Slovenian => Language.Slovenian,
                    UnityEngine.SystemLanguage.Spanish => Language.Spanish,
                    UnityEngine.SystemLanguage.Swedish => Language.Swedish,
                    UnityEngine.SystemLanguage.Thai => Language.Thai,
                    UnityEngine.SystemLanguage.Turkish => Language.Turkish,
                    UnityEngine.SystemLanguage.Ukrainian => Language.Ukrainian,
                    UnityEngine.SystemLanguage.Unknown => Language.Unspecified,
                    UnityEngine.SystemLanguage.Vietnamese => Language.Vietnamese,
                    _ => Language.Unspecified
                };
            }
        }

        private static readonly Dictionary<Language, string> languageMap = new();
        private static readonly Dictionary<string, Language> languageStrMap = new();

        static DefaultLocalizationHelper()
        {
            RegisterLanguageMap(Language.English, "English (United States)");
            RegisterLanguageMap(Language.ChineseSimplified, "Chinese (Simplified)");
            RegisterLanguageMap(Language.ChineseTraditional, "Chinese (Traditional)");
            RegisterLanguageMap(Language.Japanese);
        }

        private static void RegisterLanguageMap(Language language, string str = "")
        {
            if (string.IsNullOrEmpty(str))
            {
                str = language.ToString();
            }

            languageMap[language] = str;
            languageStrMap[str] = language;
        }

        /// <summary>
        /// 根据语言字符串获取语言枚举。
        /// </summary>
        /// <param name="str">语言字符串。</param>
        /// <returns>语言枚举。</returns>
        public static Language GetLanguage(string str)
        {
            return string.IsNullOrEmpty(str) ? Language.Unspecified : languageStrMap.GetValueOrDefault(str, Language.English);
        }

        /// <summary>
        /// 根据语言枚举获取语言字符串。
        /// </summary>
        /// <param name="language">语言枚举。</param>
        /// <returns>语言字符串。</returns>
        public static string GetLanguageStr(Language language)
        {
            return languageMap.GetValueOrDefault(language, "English");
        }
    }
}