using UnityEngine;

namespace Framework.Singleton;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance) return instance;
            GameObject go = new GameObject(typeof(T).Name);
            instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            return instance;
        }
    }
}