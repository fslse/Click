using Scripts.Fire.Log;
using Scripts.Fire.Singleton;

namespace Scripts.Fire.Manager
{
    public interface IManager
    {
    }

    public abstract class Manager<T> : MonoSingleton<T>, IManager where T : Manager<T>
    {
        private void Awake()
        {
            GameLog.LogDebug($"{gameObject.name} Awake");
            transform.SetParent(GameManager.Instance.transform);
        }
    }
}