using Scripts.Fire.Singleton;

namespace Framework
{
    public abstract class GameModule<T> : MonoSingleton<T> where T : GameModule<T>
    {
        private void Awake()
        {
            transform.SetParent(GameApp.Instance.transform);
        }
    }
}