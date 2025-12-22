using Blizzard.T5.Jobs;
using Hearthstone.UI;
using UnityEngine;

public class WaitForWidget : IJobDependency, IAsyncJobResult
{
  private Widget m_widget;

  public WaitForWidget(Widget widget) => this.m_widget = widget;

  public bool IsReady() => (Object) this.m_widget == (Object) null || this.m_widget.IsReady;
}
