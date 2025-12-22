using System;
using System.Collections.Generic;

public class ReactiveObjectManager
{
  private static ReactiveObjectManager s_instance;
  private Dictionary<Guid, ReactiveObject> m_entries;

  public static ReactiveObjectManager Get()
  {
    if (ReactiveObjectManager.s_instance == null)
      ReactiveObjectManager.s_instance = new ReactiveObjectManager();
    return ReactiveObjectManager.s_instance;
  }

  public void RegisterReactiveObject(ReactiveObject robj, Guid id) => this.m_entries.Add(id, robj);

  public ReactiveObject GetReactiveObjectById(Guid id)
  {
    ReactiveObject reactiveObjectById = (ReactiveObject) null;
    this.m_entries.TryGetValue(id, out reactiveObjectById);
    return reactiveObjectById;
  }

  private ReactiveObjectManager() => this.m_entries = new Dictionary<Guid, ReactiveObject>();
}
