using System.Collections.Generic;

namespace Framework.Core.GameEvent
{
    /// <summary>
    /// 运行时Id。
    /// <remarks>提供给事件分发的运行时Id。</remarks>
    /// <example> public static readonly int ExampleEventId = RuntimeId.ToRuntimeId("ExampleEvent.ExampleEventId"); </example>
    /// </summary>
    public static class RuntimeId
    {
        /// <summary>
        /// string -> RuntimeId
        /// </summary>
        private static readonly Dictionary<string, int> eventString2RuntimeMap = new();

        /// <summary>
        /// RuntimeId -> string
        /// </summary>
        private static readonly Dictionary<int, string> eventRuntimeToStringMap = new();

        /// <summary>
        /// 当前运行时Id。
        /// </summary>
        private static int currentRuntimeId;

        /// <summary>
        /// 字符串转RuntimeId。
        /// </summary>
        /// <param name="value">字符串Value。</param>
        /// <returns>RuntimeId。</returns>
        public static int ToRuntimeId(string value)
        {
            if (eventString2RuntimeMap.TryGetValue(value, out var runtimeId))
            {
                return runtimeId;
            }

            runtimeId = ++currentRuntimeId;
            eventString2RuntimeMap[value] = runtimeId;
            eventRuntimeToStringMap[runtimeId] = value;

            return runtimeId;
        }

        /// <summary>
        /// RuntimeId转字符串。
        /// </summary>
        /// <param name="runtimeId">RuntimeId。</param>
        /// <returns>字符串。</returns>
        public static string ToString(int runtimeId)
        {
            return eventRuntimeToStringMap.TryGetValue(runtimeId, out var value) ? value : string.Empty;
        }
    }
}