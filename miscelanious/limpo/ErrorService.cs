using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;

public class ErrorService : IErrorService, IService
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

  public void AddFatal(FatalErrorReason reason, string messageKey, params object[] messageArgs) => Error.AddFatal(reason, messageKey, messageArgs);
}
