using Blizzard.T5.Jobs;
using UnityEngine;

public class WaitForOverlayUI : IJobDependency, IAsyncJobResult
{
  public bool IsReady() => (Object) OverlayUI.Get() != (Object) null;
}
