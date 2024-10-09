using System;
using System.Collections.Generic;

namespace Framework.Modules.ObjectPool
{
    internal sealed partial class ObjectPoolManager : IObjectPoolManager
    {
        public int Count { get; }
        public bool HasObjectPool<T>() where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public bool HasObjectPool(Type objectType)
        {
            throw new NotImplementedException();
        }

        public bool HasObjectPool<T>(string name) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public bool HasObjectPool(Type objectType, string name)
        {
            throw new NotImplementedException();
        }

        public bool HasObjectPool(Predicate<ObjectPoolBase> condition)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> GetObjectPool<T>() where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase GetObjectPool(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> GetObjectPool<T>(string name) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase GetObjectPool(Type objectType, string name)
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase GetObjectPool(Predicate<ObjectPoolBase> condition)
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase[] GetObjectPools(Predicate<ObjectPoolBase> condition)
        {
            throw new NotImplementedException();
        }

        public void GetObjectPools(Predicate<ObjectPoolBase> condition, List<ObjectPoolBase> results)
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase[] GetAllObjectPools()
        {
            throw new NotImplementedException();
        }

        public void GetAllObjectPools(List<ObjectPoolBase> results)
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase[] GetAllObjectPools(bool sort)
        {
            throw new NotImplementedException();
        }

        public void GetAllObjectPools(bool sort, List<ObjectPoolBase> results)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>() where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(float expireTime) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, float expireTime)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float expireTime) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float expireTime)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, float expireTime) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, float expireTime)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, float expireTime)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>() where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(float expireTime) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, float expireTime)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float expireTime) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float expireTime)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, float expireTime) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, float expireTime)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, float expireTime)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            throw new NotImplementedException();
        }

        public bool DestroyObjectPool<T>() where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public bool DestroyObjectPool(Type objectType)
        {
            throw new NotImplementedException();
        }

        public bool DestroyObjectPool<T>(string name) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public bool DestroyObjectPool(Type objectType, string name)
        {
            throw new NotImplementedException();
        }

        public bool DestroyObjectPool<T>(IObjectPool<T> objectPool) where T : ObjectBase
        {
            throw new NotImplementedException();
        }

        public bool DestroyObjectPool(ObjectPoolBase objectPool)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        public void ReleaseAllUnused()
        {
            throw new NotImplementedException();
        }
    }
}