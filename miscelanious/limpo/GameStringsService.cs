using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;

public class GameStringsService : IGameStringsService, IService
{
  public System.Type[] GetDependencies() => (System.Type[]) null;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public void Shutdown()
  {
  }

  public string Get(string key) => GameStrings.Get(key);

  public string Format(string key, params object[] args) => GameStrings.Format(key, args);
}
