using System;

public abstract class ReactiveObject<T> : ReactiveObject
{
  private static Guid s_guid;
  private T m_value;
  private bool m_init;
  private bool m_registeredCallbacks;

  public T Value
  {
    get
    {
      if (!this.m_init)
        this.Init();
      return this.m_value;
    }
  }

  public void Init()
  {
    if (!this.m_init)
      this.SetValue(this.FetchValue());
    if (this.m_registeredCallbacks)
      return;
    this.RegisterCallbacks();
  }

  protected ReactiveObject()
    : this(ReactiveObject<T>.GetId())
  {
  }

  protected ReactiveObject(Guid guid) => ReactiveObjectManager.Get().RegisterReactiveObject((ReactiveObject) this, guid);

  protected static ReactiveObject<T> GetExistingInstance() => ReactiveObject<T>.GetExistingInstance(ReactiveObject<T>.GetId());

  protected static ReactiveObject<T> GetExistingInstance(Guid guid) => ReactiveObjectManager.Get().GetReactiveObjectById(guid) as ReactiveObject<T>;

  protected abstract T FetchValue();

  protected abstract bool RegisterChangeCallback();

  protected void OnObjectChanged() => this.SetValue(this.FetchValue());

  protected static Guid GetId()
  {
    if (ReactiveObject<T>.s_guid == Guid.Empty)
      ReactiveObject<T>.s_guid = Guid.NewGuid();
    return ReactiveObject<T>.s_guid;
  }

  private void SetValue(T val)
  {
    this.m_value = val;
    if ((object) this.m_value != null && !this.m_init)
      this.m_init = true;
    if (this.m_registeredCallbacks)
      return;
    this.RegisterCallbacks();
  }

  private void RegisterCallbacks()
  {
    if (this.m_registeredCallbacks || !this.RegisterChangeCallback())
      return;
    this.m_registeredCallbacks = true;
  }
}
