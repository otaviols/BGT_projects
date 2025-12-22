using Blizzard.T5.Jobs;
using UnityEngine;

public class WaitForTooltipPanelManager : IJobDependency, IAsyncJobResult
{
  public bool IsReady() => (Object) TooltipPanelManager.Get() != (Object) null;
}
