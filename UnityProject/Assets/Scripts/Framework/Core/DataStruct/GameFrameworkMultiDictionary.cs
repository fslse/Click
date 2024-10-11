﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Framework.Core.DataStruct
{
    /// <summary>
    /// 游戏框架多值字典类。
    /// </summary>
    /// <typeparam name="TKey">指定多值字典的主键类型。</typeparam>
    /// <typeparam name="TValue">指定多值字典的值类型。</typeparam>
    public sealed class GameFrameworkMultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, GameFrameworkLinkedListRange<TValue>>>
    {
        private readonly GameFrameworkLinkedList<TValue> linkedList;
        private readonly Dictionary<TKey, GameFrameworkLinkedListRange<TValue>> dictionary;

        /// <summary>
        /// 初始化游戏框架多值字典类的新实例。
        /// </summary>
        public GameFrameworkMultiDictionary()
        {
            linkedList = new GameFrameworkLinkedList<TValue>();
            dictionary = new Dictionary<TKey, GameFrameworkLinkedListRange<TValue>>();
        }

        /// <summary>
        /// 获取多值字典中实际包含的主键数量。
        /// </summary>
        public int Count => dictionary.Count;

        /// <summary>
        /// 获取多值字典中指定主键的范围。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <returns>指定主键的范围。</returns>
        public GameFrameworkLinkedListRange<TValue> this[TKey key]
        {
            get
            {
                dictionary.TryGetValue(key, out var range);
                return range;
            }
        }

        /// <summary>
        /// 清理多值字典。
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
            linkedList.Clear();
        }

        /// <summary>
        /// 检查多值字典中是否包含指定主键。
        /// </summary>
        /// <param name="key">要检查的主键。</param>
        /// <returns>多值字典中是否包含指定主键。</returns>
        public bool Contains(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 检查多值字典中是否包含指定值。
        /// </summary>
        /// <param name="key">要检查的主键。</param>
        /// <param name="value">要检查的值。</param>
        /// <returns>多值字典中是否包含指定值。</returns>
        public bool Contains(TKey key, TValue value)
        {
            return dictionary.TryGetValue(key, out var range) && range.Contains(value);
        }

        /// <summary>
        /// 尝试获取多值字典中指定主键的范围。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="range">指定主键的范围。</param>
        /// <returns>是否获取成功。</returns>
        public bool TryGetValue(TKey key, out GameFrameworkLinkedListRange<TValue> range)
        {
            return dictionary.TryGetValue(key, out range);
        }

        /// <summary>
        /// 向指定的主键增加指定的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="value">指定的值。</param>
        public void Add(TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out var range))
            {
                linkedList.AddBefore(range.Terminal, value);
            }
            else
            {
                LinkedListNode<TValue> first = linkedList.AddLast(value);
                LinkedListNode<TValue> terminal = linkedList.AddLast(default(TValue));
                dictionary.Add(key, new GameFrameworkLinkedListRange<TValue>(first, terminal));
            }
        }

        /// <summary>
        /// 从指定的主键中移除指定的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="value">指定的值。</param>
        /// <returns>是否移除成功。</returns>
        public bool Remove(TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out var range))
            {
                for (LinkedListNode<TValue> current = range.First; current != null && current != range.Terminal; current = current.Next)
                {
                    if (current.Value.Equals(value))
                    {
                        if (current == range.First)
                        {
                            LinkedListNode<TValue> next = current.Next;
                            if (next == range.Terminal)
                            {
                                linkedList.Remove(next);
                                dictionary.Remove(key);
                            }
                            else
                            {
                                dictionary[key] = new GameFrameworkLinkedListRange<TValue>(next, range.Terminal);
                            }
                        }

                        linkedList.Remove(current);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 从指定的主键中移除所有的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <returns>是否移除成功。</returns>
        public bool RemoveAll(TKey key)
        {
            if (dictionary.Remove(key, out var range))
            {
                LinkedListNode<TValue> current = range.First;
                while (current != null)
                {
                    LinkedListNode<TValue> next = current != range.Terminal ? current.Next : null;
                    linkedList.Remove(current);
                    current = next;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator<KeyValuePair<TKey, GameFrameworkLinkedListRange<TValue>>> IEnumerable<KeyValuePair<TKey, GameFrameworkLinkedListRange<TValue>>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 循环访问集合的枚举数。
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, GameFrameworkLinkedListRange<TValue>>>
        {
            private Dictionary<TKey, GameFrameworkLinkedListRange<TValue>>.Enumerator enumerator;

            internal Enumerator(Dictionary<TKey, GameFrameworkLinkedListRange<TValue>> dictionary)
            {
                if (dictionary == null)
                {
                    throw new Exception("Dictionary is invalid.");
                }

                enumerator = dictionary.GetEnumerator();
            }

            /// <summary>
            /// 获取当前结点。
            /// </summary>
            public KeyValuePair<TKey, GameFrameworkLinkedListRange<TValue>> Current => enumerator.Current;

            /// <summary>
            /// 获取当前的枚举数。
            /// </summary>
            object IEnumerator.Current => enumerator.Current;

            /// <summary>
            /// 清理枚举数。
            /// </summary>
            public void Dispose()
            {
                enumerator.Dispose();
            }

            /// <summary>
            /// 获取下一个结点。
            /// </summary>
            /// <returns>返回下一个结点。</returns>
            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            /// <summary>
            /// 重置枚举数。
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<KeyValuePair<TKey, GameFrameworkLinkedListRange<TValue>>>)enumerator).Reset();
            }
        }
    }
}