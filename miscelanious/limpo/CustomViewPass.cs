using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class CustomViewPass : ScriptableRenderPass
{
  private static readonly List<CustomViewPass>[] queues = new List<CustomViewPass>[6];
  private CustomViewEntryPoint whereScheduled = CustomViewEntryPoint.Count;

  public bool isScheduled => this.whereScheduled != CustomViewEntryPoint.Count;

  static CustomViewPass()
  {
    for (int index = 0; index < 6; ++index)
      CustomViewPass.queues[index] = new List<CustomViewPass>(5);
  }

  public static List<CustomViewPass> GetQueue(CustomViewEntryPoint whenToRender)
  {
    if (whenToRender != CustomViewEntryPoint.Count)
      return CustomViewPass.queues[(int) whenToRender];
    Debug.LogError((object) "Invalid entrypoint");
    return (List<CustomViewPass>) null;
  }

  public void ChangeSchedule(CustomViewEntryPoint whenToRender)
  {
    this.Unschedule();
    this.Schedule(whenToRender);
  }

  public void Schedule(CustomViewEntryPoint whenToRender)
  {
    if (whenToRender == this.whereScheduled)
      return;
    if (whenToRender == CustomViewEntryPoint.Count)
      Debug.LogError((object) "Invalid entrypoint");
    else if (this.isScheduled)
    {
      Debug.LogError((object) ("Pass Already in Queue:" + this.whereScheduled.ToString()));
    }
    else
    {
      CustomViewPass.queues[(int) whenToRender].Add(this);
      this.whereScheduled = whenToRender;
    }
  }

  public void Unschedule()
  {
    if (!this.isScheduled)
      return;
    List<CustomViewPass> queue = CustomViewPass.queues[(int) this.whereScheduled];
    if (queue == null)
      return;
    this.whereScheduled = CustomViewEntryPoint.Count;
    queue.Remove(this);
  }
}
