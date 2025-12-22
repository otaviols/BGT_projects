using Blizzard.T5.Jobs;
using UnityEngine;

public class WaitForSplashScreen : IJobDependency, IAsyncJobResult
{
  public bool IsReady() => (Object) SplashScreen.Get() != (Object) null;
}
