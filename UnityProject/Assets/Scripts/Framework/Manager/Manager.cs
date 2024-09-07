using Scripts.Framework.Singleton;

namespace Scripts.Framework.Manager
{
    public interface IManager
    {
    }

    public class Manager : MonoSingleton<Manager>, IManager
    {
    }
}