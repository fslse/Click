using Framework.Singleton;

namespace Framework.Manager;

public interface IManager
{
}

public class Manager : MonoSingleton<Manager>, IManager
{
}