using System;
using System.Collections.Generic;
using Framework.Core.MemoryPool;

namespace Framework.Core.GameEvent
{
    /// <summary>
    /// 游戏事件管理器。
    /// </summary>
    public class UIEventManager : IMemory
    {
        private readonly List<int> listEventTypes;
        private readonly List<Delegate> listHandles;
        private readonly bool isInit;

        /// <summary>
        /// 游戏事件管理器构造函数。
        /// </summary>
        public UIEventManager()
        {
            if (isInit)
            {
                return;
            }

            isInit = true;
            listEventTypes = new List<int>();
            listHandles = new List<Delegate>();
        }

        /// <summary>
        /// 清理内存对象回收入池。
        /// </summary>
        public void Clear()
        {
            if (!isInit)
            {
                return;
            }

            for (int i = 0; i < listEventTypes.Count; ++i)
            {
                var eventType = listEventTypes[i];
                var handle = listHandles[i];
                GameEvent.RemoveEventListener(eventType, handle);
            }

            listEventTypes.Clear();
            listHandles.Clear();
        }

        private void AddEventImp(int eventType, Delegate handler)
        {
            listEventTypes.Add(eventType);
            listHandles.Add(handler);
        }

        #region UIEvent

        public void AddEvent(int eventType, Action handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                AddEventImp(eventType, handler);
            }
        }

        public void AddEvent<T>(int eventType, Action<T> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                AddEventImp(eventType, handler);
            }
        }

        public void AddEvent<T1, T2>(int eventType, Action<T1, T2> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                AddEventImp(eventType, handler);
            }
        }

        public void AddEvent<T1, T2, T3>(int eventType, Action<T1, T2, T3> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                AddEventImp(eventType, handler);
            }
        }

        public void AddEvent<T1, T2, T3, T4>(int eventType, Action<T1, T2, T3, T4> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                AddEventImp(eventType, handler);
            }
        }

        public void AddEvent<T1, T2, T3, T4, T5>(int eventType, Action<T1, T2, T3, T4, T5> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                AddEventImp(eventType, handler);
            }
        }

        #endregion
    }
}