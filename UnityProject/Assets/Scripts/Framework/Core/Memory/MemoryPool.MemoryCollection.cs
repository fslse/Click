using System;
using System.Collections.Generic;

namespace Framework.Core.Memory
{
    public static partial class MemoryPool
    {
        /// <summary>
        /// 内存池对象容器。
        /// </summary>
        private sealed class MemoryCollection
        {
            private readonly Queue<IMemory> memories;

            public MemoryCollection(Type memoryType)
            {
                memories = new Queue<IMemory>();

                // public properties
                MemoryType = memoryType;
                UsingMemoryCount = 0;
                AcquireMemoryCount = 0;
                ReleaseMemoryCount = 0;
                AddMemoryCount = 0;
                RemoveMemoryCount = 0;
            }

            #region public properties

            public Type MemoryType { get; }

            // ReSharper disable once InconsistentlySynchronizedField
            public int UnusedMemoryCount => memories.Count;

            public int UsingMemoryCount { get; private set; }

            public int AcquireMemoryCount { get; private set; }

            public int ReleaseMemoryCount { get; private set; }

            public int AddMemoryCount { get; private set; }

            public int RemoveMemoryCount { get; private set; }

            #endregion

            public T Acquire<T>() where T : class, IMemory, new()
            {
                if (typeof(T) != MemoryType)
                {
                    throw new Exception("Type is invalid.");
                }

                UsingMemoryCount++;
                AcquireMemoryCount++;
                lock (memories)
                {
                    if (memories.Count > 0)
                    {
                        return (T)memories.Dequeue();
                    }
                }

                AddMemoryCount++;
                return new T();
            }

            public IMemory Acquire()
            {
                UsingMemoryCount++;
                AcquireMemoryCount++;
                lock (memories)
                {
                    if (memories.Count > 0)
                    {
                        return memories.Dequeue();
                    }
                }

                AddMemoryCount++;
                return (IMemory)Activator.CreateInstance(MemoryType);
            }

            public void Release(IMemory memory)
            {
                memory.Clear();
                lock (memories)
                {
                    if (EnableStrictCheck && memories.Contains(memory))
                    {
                        throw new Exception("The memory has been released.");
                    }

                    memories.Enqueue(memory);
                }

                ReleaseMemoryCount++;
                UsingMemoryCount--;
            }

            public void Add<T>(int count) where T : class, IMemory, new()
            {
                if (typeof(T) != MemoryType)
                {
                    throw new Exception("Type is invalid.");
                }

                lock (memories)
                {
                    AddMemoryCount += count;
                    while (count-- > 0)
                    {
                        memories.Enqueue(new T());
                    }
                }
            }

            public void Add(int count)
            {
                lock (memories)
                {
                    AddMemoryCount += count;
                    while (count-- > 0)
                    {
                        memories.Enqueue((IMemory)Activator.CreateInstance(MemoryType));
                    }
                }
            }

            public void Remove(int count)
            {
                lock (memories)
                {
                    if (count > memories.Count)
                    {
                        count = memories.Count;
                    }

                    RemoveMemoryCount += count;
                    while (count-- > 0)
                    {
                        memories.Dequeue();
                    }
                }
            }

            public void RemoveAll()
            {
                lock (memories)
                {
                    RemoveMemoryCount += memories.Count;
                    memories.Clear();
                }
            }
        }
    }
}