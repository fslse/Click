using System;
using Scripts.Framework.Singleton;
using UnityEngine.Events;

namespace Scripts.Framework.Manager
{
    /// <summary>
    /// 适用于对时序不敏感的帧更新事件
    /// </summary>
    public class MonoManager : MonoSingleton<MonoManager>
    {
        private event UnityAction OnFixedUpdate;
        private event UnityAction OnUpdate;
        private event UnityAction OnLateUpdate;

        public void AddListener(UnityAction action, UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.FixedUpdate:
                    OnFixedUpdate += action;
                    break;
                case UpdateType.Update:
                    OnUpdate += action;
                    break;
                case UpdateType.LateUpdate:
                    OnLateUpdate += action;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null);
            }
        }

        public void RemoveListener(UnityAction action, UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.FixedUpdate:
                    OnFixedUpdate -= action;
                    break;
                case UpdateType.Update:
                    OnUpdate -= action;
                    break;
                case UpdateType.LateUpdate:
                    OnLateUpdate -= action;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null);
            }
        }

        public void RemoveAllListeners(UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.FixedUpdate:
                    OnFixedUpdate = null;
                    break;
                case UpdateType.Update:
                    OnUpdate = null;
                    break;
                case UpdateType.LateUpdate:
                    OnLateUpdate = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null);
            }
        }

        public void RemoveAllListeners()
        {
            OnFixedUpdate = null;
            OnUpdate = null;
            OnLateUpdate = null;
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
    }

    public enum UpdateType
    {
        FixedUpdate,
        Update,
        LateUpdate
    }
}