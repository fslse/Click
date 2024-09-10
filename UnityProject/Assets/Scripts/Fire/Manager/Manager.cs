using Scripts.Fire.Singleton;

namespace Scripts.Fire.Manager
{
    public interface IManager
    {
    }

    public class Manager : MonoSingleton<Manager>, IManager
    {
    }
}