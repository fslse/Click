﻿using System;
using Framework.Core.Utility.DefaultHelper;

namespace Framework.Core.Utility
{
    public static partial class Utility
    {
        /// <summary>
        /// JSON 相关的实用函数。
        /// </summary>
        public static partial class Json
        {
            private static IJsonHelper jsonHelper = new DefaultJsonHelper();

            /// <summary>
            /// 设置 JSON 辅助器。
            /// </summary>
            /// <param name="helper">要设置的 JSON 辅助器。</param>
            public static void SetJsonHelper(IJsonHelper helper)
            {
                jsonHelper = helper;
            }

            /// <summary>
            /// 将对象序列化为 JSON 字符串。
            /// </summary>
            /// <param name="obj">要序列化的对象。</param>
            /// <returns>序列化后的 JSON 字符串。</returns>
            public static string ToJson(object obj)
            {
                if (jsonHelper == null)
                {
                    throw new Exception("JSON helper is invalid.");
                }

                try
                {
                    return jsonHelper.ToJson(obj);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Can not convert to JSON with exception '{exception}'.", exception);
                }
            }

            /// <summary>
            /// 将 JSON 字符串反序列化为对象。
            /// </summary>
            /// <typeparam name="T">对象类型。</typeparam>
            /// <param name="json">要反序列化的 JSON 字符串。</param>
            /// <returns>反序列化后的对象。</returns>
            public static T ToObject<T>(string json)
            {
                if (jsonHelper == null)
                {
                    throw new Exception("JSON helper is invalid.");
                }

                try
                {
                    return jsonHelper.ToObject<T>(json);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Can not convert to object with exception '{exception}'.", exception);
                }
            }

            /// <summary>
            /// 将 JSON 字符串反序列化为对象。
            /// </summary>
            /// <param name="objectType">对象类型。</param>
            /// <param name="json">要反序列化的 JSON 字符串。</param>
            /// <returns>反序列化后的对象。</returns>
            public static object ToObject(Type objectType, string json)
            {
                if (jsonHelper == null)
                {
                    throw new Exception("JSON helper is invalid.");
                }

                if (objectType == null)
                {
                    throw new Exception("Object type is invalid.");
                }

                try
                {
                    return jsonHelper.ToObject(objectType, json);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Can not convert to object with exception '{exception}'.", exception);
                }
            }
        }
    }
}