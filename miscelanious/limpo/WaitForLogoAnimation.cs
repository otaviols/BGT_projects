using Blizzard.T5.Jobs;
using UnityEngine;

public class WaitForLogoAnimation : IJobDependency, IAsyncJobResult
{
  public bool IsReady() => (Object) LogoAnimation.Get() != (Object) null;
}
