using System;
using System.Collections.Generic;
using Scripts.Fire.Log;

namespace Framework.Core.Event
{
    /// <summary>
    /// 事件委托数据类。
    /// </summary>
    internal class EventDelegateData
    {
        private readonly int eventType;
        private readonly List<Delegate> existList = new(); // 已存在
        private readonly List<Delegate> addList = new(); // 待添加
        private readonly List<Delegate> deleteList = new(); // 待删除
        private bool isExecute; // 是否正在执行
        private bool dirty; // 脏标记，延迟处理添加和删除操作

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        internal EventDelegateData(int eventType)
        {
            this.eventType = eventType;
        }

        /// <summary>
        /// 添加注册委托。
        /// </summary>
        /// <param name="handler">事件处理回调。</param>
        /// <returns>是否添加回调成功。</returns>
        internal bool AddHandler(Delegate handler)
        {
            if (existList.Contains(handler))
            {
                GameLog.LogWarning("Repeated Add Handler");
                return false;
            }

            if (isExecute)
            {
                dirty = true;
                addList.Add(handler);
            }
            else
            {
                existList.Add(handler);
            }

            return true;
        }

        /// <summary>
        /// 移除反注册委托。
        /// </summary>
        /// <param name="handler">事件处理回调。</param>
        internal void RmvHandler(Delegate handler)
        {
            if (isExecute)
            {
                dirty = true;
                deleteList.Add(handler);
            }
            else
            {
                if (!existList.Remove(handler))
                {
                    GameLog.LogWarning("Delete handle failed, not exist, EventId: {0}", RuntimeId.ToString(eventType));
                }
            }
        }

        /// <summary>
        /// 检测脏数据修正。
        /// </summary>
        private void CheckModify()
        {
            isExecute = false;
            if (dirty)
            {
                for (int i = 0; i < addList.Count; i++)
                {
                    existList.Add(addList[i]);
                }

                addList.Clear();

                for (int i = 0; i < deleteList.Count; i++)
                {
                    existList.Remove(deleteList[i]);
                }

                deleteList.Clear();
            }
        }

        /// <summary>
        /// 回调调用。
        /// </summary>
        public void Callback()
        {
            isExecute = true;
            for (var i = 0; i < existList.Count; i++)
            {
                var d = existList[i];
                if (d is Action action)
                {
                    action();
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 回调调用。
        /// </summary>
        /// <param name="arg1">事件参数1。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        public void Callback<TArg1>(TArg1 arg1)
        {
            isExecute = true;
            for (var i = 0; i < existList.Count; i++)
            {
                var d = existList[i];
                if (d is Action<TArg1> action)
                {
                    action(arg1);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 回调调用。
        /// </summary>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        public void Callback<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            isExecute = true;
            for (var i = 0; i < existList.Count; i++)
            {
                var d = existList[i];
                if (d is Action<TArg1, TArg2> action)
                {
                    action(arg1, arg2);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 回调调用。
        /// </summary>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <param name="arg3">事件参数3。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        /// <typeparam name="TArg3">事件参数3类型。</typeparam>
        public void Callback<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            isExecute = true;
            for (var i = 0; i < existList.Count; i++)
            {
                var d = existList[i];
                if (d is Action<TArg1, TArg2, TArg3> action)
                {
                    action(arg1, arg2, arg3);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 回调调用。
        /// </summary>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <param name="arg3">事件参数3。</param>
        /// <param name="arg4">事件参数4。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        /// <typeparam name="TArg3">事件参数3类型。</typeparam>
        /// <typeparam name="TArg4">事件参数4类型。</typeparam>
        public void Callback<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            isExecute = true;
            for (var i = 0; i < existList.Count; i++)
            {
                var d = existList[i];
                if (d is Action<TArg1, TArg2, TArg3, TArg4> action)
                {
                    action(arg1, arg2, arg3, arg4);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 回调调用。
        /// </summary>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <param name="arg3">事件参数3。</param>
        /// <param name="arg4">事件参数4。</param>
        /// <param name="arg5">事件参数5。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        /// <typeparam name="TArg3">事件参数3类型。</typeparam>
        /// <typeparam name="TArg4">事件参数4类型。</typeparam>
        /// <typeparam name="TArg5">事件参数5类型。</typeparam>
        public void Callback<TArg1, TArg2, TArg3, TArg4, TArg5>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            isExecute = true;
            for (var i = 0; i < existList.Count; i++)
            {
                var d = existList[i];
                if (d is Action<TArg1, TArg2, TArg3, TArg4, TArg5> action)
                {
                    action(arg1, arg2, arg3, arg4, arg5);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 回调调用。
        /// </summary>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <param name="arg3">事件参数3。</param>
        /// <param name="arg4">事件参数4。</param>
        /// <param name="arg5">事件参数5。</param>
        /// <param name="arg6">事件参数6。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        /// <typeparam name="TArg3">事件参数3类型。</typeparam>
        /// <typeparam name="TArg4">事件参数4类型。</typeparam>
        /// <typeparam name="TArg5">事件参数5类型。</typeparam>
        /// <typeparam name="TArg6">事件参数6类型。</typeparam>
        public void Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            isExecute = true;
            for (var i = 0; i < existList.Count; i++)
            {
                var d = existList[i];
                if (d is Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> action)
                {
                    action(arg1, arg2, arg3, arg4, arg5, arg6);
                }
            }

            CheckModify();
        }
    }
}