using System;

namespace Framework.ObjectPool
{
    internal sealed partial class ObjectPoolManager
    {
        private sealed class ObjectPool<T> : ObjectPoolBase, IObjectPool<T> where T : ObjectBase
        {
            public override Type ObjectType { get; }
            public override int Count { get; }
            public override int CanReleaseCount { get; }
            public override bool AllowMultiSpawn { get; }
            public override float AutoReleaseInterval { get; set; }
            public override int Capacity { get; set; }
            public override float ExpireTime { get; set; }
            public override int Priority { get; set; }
            public void Register(T obj, bool spawned)
            {
                throw new NotImplementedException();
            }

            public bool CanSpawn()
            {
                throw new NotImplementedException();
            }

            public bool CanSpawn(string name)
            {
                throw new NotImplementedException();
            }

            public T Spawn()
            {
                throw new NotImplementedException();
            }

            public T Spawn(string name)
            {
                throw new NotImplementedException();
            }

            public void Unspawn(T obj)
            {
                throw new NotImplementedException();
            }

            public void Unspawn(object target)
            {
                throw new NotImplementedException();
            }

            public void SetLocked(T obj, bool locked)
            {
                throw new NotImplementedException();
            }

            public void SetLocked(object target, bool locked)
            {
                throw new NotImplementedException();
            }

            public void SetPriority(T obj, int priority)
            {
                throw new NotImplementedException();
            }

            public void SetPriority(object target, int priority)
            {
                throw new NotImplementedException();
            }

            public bool ReleaseObject(T obj)
            {
                throw new NotImplementedException();
            }

            public bool ReleaseObject(object target)
            {
                throw new NotImplementedException();
            }

            public override void Release()
            {
                throw new NotImplementedException();
            }

            public override void Release(int toReleaseCount)
            {
                throw new NotImplementedException();
            }

            public void Release(ReleaseObjectFilterCallback<T> releaseObjectFilterCallback)
            {
                throw new NotImplementedException();
            }

            public void Release(int toReleaseCount, ReleaseObjectFilterCallback<T> releaseObjectFilterCallback)
            {
                throw new NotImplementedException();
            }

            public override void ReleaseAllUnused()
            {
                throw new NotImplementedException();
            }

            public override ObjectInfo[] GetAllObjectInfos()
            {
                throw new NotImplementedException();
            }

            internal override void Update(float elapseSeconds, float realElapseSeconds)
            {
                throw new NotImplementedException();
            }

            internal override void Shutdown()
            {
                throw new NotImplementedException();
            }
        }
    }
}