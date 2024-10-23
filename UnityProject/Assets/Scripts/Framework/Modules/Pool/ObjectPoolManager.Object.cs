using System;
using Framework.Core.Memory;

namespace Framework.Modules.Pool
{
    internal sealed partial class ObjectPoolManager
    {
        /// <summary>
        /// 内部对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        private sealed class Object<T> : IMemory where T : ObjectBase
        {
            private T @object;
            private int spawnCount;

            /// <summary>
            /// 初始化内部对象的新实例。
            /// </summary>
            public Object()
            {
                @object = null;
                spawnCount = 0;
            }

            /// <summary>
            /// 获取对象名称。
            /// </summary>
            public string Name => @object.Name;

            /// <summary>
            /// 获取对象是否被加锁。
            /// </summary>
            public bool Locked
            {
                get => @object.Locked;
                internal set => @object.Locked = value;
            }

            /// <summary>
            /// 获取对象的优先级。
            /// </summary>
            public int Priority
            {
                get => @object.Priority;
                internal set => @object.Priority = value;
            }

            /// <summary>
            /// 获取对象上次使用时间。
            /// </summary>
            public DateTime LastUseTime => @object.LastUseTime;

            /// <summary>
            /// 获取自定义释放检查标记。
            /// </summary>
            public bool CustomCanReleaseFlag => @object.CustomCanReleaseFlag;

            /// <summary>
            /// 获取对象是否正在使用。
            /// </summary>
            public bool IsInUse => spawnCount > 0;

            /// <summary>
            /// 获取对象的获取计数。
            /// </summary>
            public int SpawnCount => spawnCount;

            /// <summary>
            /// 创建内部对象。
            /// </summary>
            /// <param name="obj">对象。</param>
            /// <param name="spawned">对象是否已被获取。</param>
            /// <returns>创建的内部对象。</returns>
            public static Object<T> Create(T obj, bool spawned)
            {
                if (obj == null)
                {
                    throw new GameFrameworkException("Object is invalid.");
                }

                Object<T> internalObject = MemoryPool.Acquire<Object<T>>();
                internalObject.@object = obj;
                internalObject.spawnCount = spawned ? 1 : 0;
                if (spawned)
                {
                    obj.OnSpawn();
                }

                return internalObject;
            }

            /// <summary>
            /// 清理内部对象。
            /// </summary>
            public void Clear()
            {
                @object = null;
                spawnCount = 0;
            }

            /// <summary>
            /// 查看对象。
            /// </summary>
            /// <returns>对象。</returns>
            public T Peek()
            {
                return @object;
            }

            /// <summary>
            /// 获取对象。
            /// </summary>
            /// <returns>对象。</returns>
            public T Spawn()
            {
                spawnCount++;
                @object.LastUseTime = DateTime.UtcNow;
                @object.OnSpawn();
                return @object;
            }

            /// <summary>
            /// 回收对象。
            /// </summary>
            public void Unspawn()
            {
                @object.OnUnspawn();
                @object.LastUseTime = DateTime.UtcNow;
                spawnCount--;
                if (spawnCount < 0)
                {
                    throw new GameFrameworkException($"Object '{Name}' spawn count is less than 0.");
                }
            }

            /// <summary>
            /// 释放对象。
            /// </summary>
            /// <param name="isShutdown">是否是关闭对象池时触发。</param>
            public void Release(bool isShutdown)
            {
                @object.Release(isShutdown);
                MemoryPool.Release(@object);
            }
        }
    }
}