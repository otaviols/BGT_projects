using Blizzard.T5.Jobs;
using UnityEngine;

public class WaitForBox : IJobDependency, IAsyncJobResult
{
  public bool IsReady() => (Object) Box.Get() != (Object) null;
}
