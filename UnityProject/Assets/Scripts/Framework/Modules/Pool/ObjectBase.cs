using System;
using Framework.Core.Memory;

namespace Framework.Modules.Pool
{
    /// <summary>
    /// 对象基类。
    /// </summary>
    public abstract class ObjectBase : IMemory
    {
        /// <summary>
        /// 初始化对象基类的新实例。
        /// </summary>
        public ObjectBase()
        {
            Name = null;
            Target = null;
            Locked = false;
            Priority = 0;
            LastUseTime = default;
        }

        /// <summary>
        /// 获取对象名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取对象。
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// 获取或设置对象是否被加锁。
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 获取或设置对象的优先级。
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 获取对象上次使用时间。
        /// </summary>
        public DateTime LastUseTime { get; internal set; }

        /// <summary>
        /// 获取自定义释放检查标记。
        /// </summary>
        public virtual bool CustomCanReleaseFlag => true;

        /// <summary>
        /// 初始化对象基类。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <param name="target">对象。</param>
        /// <param name="locked">对象是否被加锁。</param>
        /// <param name="priority">对象的优先级。</param>
        protected void Initialize(string name, object target, bool locked = false, int priority = 0)
        {
            Name = name ?? string.Empty;
            Target = target ?? throw new Exception($"Target '{name}' is invalid.");
            Locked = locked;
            Priority = priority;
            LastUseTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 清理对象基类。
        /// </summary>
        public virtual void Clear()
        {
            Name = null;
            Target = null;
            Locked = false;
            Priority = 0;
            LastUseTime = default;
        }

        /// <summary>
        /// 获取对象时的事件。
        /// </summary>
        protected internal virtual void OnSpawn()
        {
        }

        /// <summary>
        /// 回收对象时的事件。
        /// </summary>
        protected internal virtual void OnUnspawn()
        {
        }

        /// <summary>
        /// 释放对象。
        /// </summary>
        /// <param name="isShutdown">是否是关闭对象池时触发。</param>
        protected internal abstract void Release(bool isShutdown);
    }
}