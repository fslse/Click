using System;
using System.Reflection;

namespace Framework.Singleton;

public abstract class Singleton<T> where T : Singleton<T>
{
    private static T? instance;

    // ReSharper disable once StaticMemberInGenericType
    // ReSharper disable once MemberCanBePrivate.Global
    protected static readonly object lockObj = new();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        // 获取非公有无参构造函数
                        var type = typeof(T);
                        var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                        var constructor = Array.Find(constructors, c => c.GetParameters().Length == 0);
                        if (constructor == null)
                        {
                            throw new Exception(type.Name);
                        }

                        instance = constructor.Invoke(null) as T;
                    }
                }
            }

            return instance ?? throw new Exception(typeof(T).Name);
        }
    }
}