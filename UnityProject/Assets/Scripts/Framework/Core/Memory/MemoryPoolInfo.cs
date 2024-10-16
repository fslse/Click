using System;
using System.Runtime.InteropServices;

namespace Framework.Core.Memory
{
    /// <summary>
    /// 内存池信息。
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct MemoryPoolInfo
    {
        /// <summary>
        /// 初始化内存池信息的新实例。
        /// </summary>
        /// <param name="type">内存池类型。</param>
        /// <param name="unusedMemoryCount">未使用内存对象数量。</param>
        /// <param name="usingMemoryCount">正在使用内存对象数量。</param>
        /// <param name="acquireMemoryCount">获取内存对象数量。</param>
        /// <param name="releaseMemoryCount">归还内存对象数量。</param>
        /// <param name="addMemoryCount">增加内存对象数量。</param>
        /// <param name="removeMemoryCount">移除内存对象数量。</param>
        public MemoryPoolInfo(Type type, int unusedMemoryCount, int usingMemoryCount, int acquireMemoryCount, int releaseMemoryCount, int addMemoryCount, int removeMemoryCount)
        {
            Type = type;
            UnusedMemoryCount = unusedMemoryCount;
            UsingMemoryCount = usingMemoryCount;
            AcquireMemoryCount = acquireMemoryCount;
            ReleaseMemoryCount = releaseMemoryCount;
            AddMemoryCount = addMemoryCount;
            RemoveMemoryCount = removeMemoryCount;
        }

        /// <summary>
        /// 获取内存池类型。
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 获取未使用内存对象数量。
        /// </summary>
        public int UnusedMemoryCount { get; }

        /// <summary>
        /// 获取正在使用内存对象数量。
        /// </summary>
        public int UsingMemoryCount { get; }

        /// <summary>
        /// 获取获取内存对象数量。
        /// </summary>
        public int AcquireMemoryCount { get; }

        /// <summary>
        /// 获取归还内存对象数量。
        /// </summary>
        public int ReleaseMemoryCount { get; }

        /// <summary>
        /// 获取增加内存对象数量。
        /// </summary>
        public int AddMemoryCount { get; }

        /// <summary>
        /// 获取移除内存对象数量。
        /// </summary>
        public int RemoveMemoryCount { get; }
    }
}