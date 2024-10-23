using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Framework.Extension;
using Scripts.Fire.Log;

namespace Framework.Modules.Setting
{
    /// <summary>
    /// 默认游戏配置。
    /// </summary>
    public sealed class DefaultSetting
    {
        private readonly SortedDictionary<string, string> settings = new(StringComparer.Ordinal);

        /// <summary>
        /// 初始化本地版本资源列表的新实例。
        /// </summary>
        public DefaultSetting()
        {
        }

        /// <summary>
        /// 获取游戏配置项数量。
        /// </summary>
        public int Count => settings.Count;

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <returns>所有游戏配置项的名称。</returns>
        public string[] GetAllSettingNames()
        {
            int index = 0;
            string[] allSettingNames = new string[settings.Count];
            foreach (KeyValuePair<string, string> setting in settings)
            {
                allSettingNames[index++] = setting.Key;
            }

            return allSettingNames;
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <param name="results">所有游戏配置项的名称。</param>
        public void GetAllSettingNames(List<string> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            results.AddRange(settings.Select(setting => setting.Key));
        }

        /// <summary>
        /// 检查是否存在指定游戏配置项。
        /// </summary>
        /// <param name="settingName">要检查游戏配置项的名称。</param>
        /// <returns>指定的游戏配置项是否存在。</returns>
        public bool HasSetting(string settingName)
        {
            return settings.ContainsKey(settingName);
        }

        /// <summary>
        /// 移除指定游戏配置项。
        /// </summary>
        /// <param name="settingName">要移除游戏配置项的名称。</param>
        /// <returns>是否移除指定游戏配置项成功。</returns>
        public bool RemoveSetting(string settingName)
        {
            return settings.Remove(settingName);
        }

        /// <summary>
        /// 清空所有游戏配置项。
        /// </summary>
        public void RemoveAllSettings()
        {
            settings.Clear();
        }

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string settingName)
        {
            if (!settings.TryGetValue(settingName, out string value))
            {
                GameLog.LogWarning($"Setting '{settingName}' is not exist.");
                return false;
            }

            return int.Parse(value) != 0;
        }

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string settingName, bool defaultValue)
        {
            if (!settings.TryGetValue(settingName, out string value))
            {
                return defaultValue;
            }

            return int.Parse(value) != 0;
        }

        /// <summary>
        /// 向指定游戏配置项写入布尔值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的布尔值。</param>
        public void SetBool(string settingName, bool value)
        {
            settings[settingName] = value ? "1" : "0";
        }

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string settingName)
        {
            if (!settings.TryGetValue(settingName, out string value))
            {
                GameLog.LogWarning($"Setting '{settingName}' is not exist.");
                return 0;
            }

            return int.Parse(value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string settingName, int defaultValue)
        {
            return !settings.TryGetValue(settingName, out string value) ? defaultValue : int.Parse(value);
        }

        /// <summary>
        /// 向指定游戏配置项写入整数值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的整数值。</param>
        public void SetInt(string settingName, int value)
        {
            settings[settingName] = value.ToString();
        }

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string settingName)
        {
            if (!settings.TryGetValue(settingName, out string value))
            {
                GameLog.LogWarning($"Setting '{settingName}' is not exist.");
                return 0f;
            }

            return float.Parse(value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string settingName, float defaultValue)
        {
            return !settings.TryGetValue(settingName, out string value) ? defaultValue : float.Parse(value);
        }

        /// <summary>
        /// 向指定游戏配置项写入浮点数值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的浮点数值。</param>
        public void SetFloat(string settingName, float value)
        {
            settings[settingName] = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string settingName)
        {
            if (!settings.TryGetValue(settingName, out string value))
            {
                GameLog.LogWarning($"Setting '{settingName}' is not exist.");
                return null;
            }

            return value;
        }

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string settingName, string defaultValue)
        {
            return settings.GetValueOrDefault(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入字符串值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的字符串值。</param>
        public void SetString(string settingName, string value)
        {
            settings[settingName] = value;
        }

        /// <summary>
        /// 序列化数据。
        /// </summary>
        /// <param name="stream">目标流。</param>
        public void Serialize(Stream stream)
        {
            using BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8);
            binaryWriter.Write7BitEncodedInt32(settings.Count);
            foreach (KeyValuePair<string, string> setting in settings)
            {
                binaryWriter.Write(setting.Key);
                binaryWriter.Write(setting.Value);
            }
        }

        /// <summary>
        /// 反序列化数据。
        /// </summary>
        /// <param name="stream">指定流。</param>
        public void Deserialize(Stream stream)
        {
            settings.Clear();
            using BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8);
            int settingCount = binaryReader.Read7BitEncodedInt32();
            for (int i = 0; i < settingCount; i++)
            {
                settings.Add(binaryReader.ReadString(), binaryReader.ReadString());
            }
        }
    }
}