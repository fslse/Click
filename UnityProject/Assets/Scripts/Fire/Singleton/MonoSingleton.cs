using UnityEngine;

namespace Scripts.Fire.Singleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance) return instance;
                GameObject go = new GameObject(typeof(T).Name);
                if (!go.transform.parent) DontDestroyOnLoad(go); // DontDestroyOnLoad方法仅对根节点有效
                return instance = go.AddComponent<T>();
            }
        }
    }
}