using System;
using System.Collections.Generic;

public abstract class ReactiveOption<T> : ReactiveObject<T>
{
  private static Dictionary<Option, Guid> s_guids;
  protected Option m_option;

  public abstract void Set(T value);

  protected ReactiveOption(Option opt)
    : base(ReactiveOption<T>.GetOptionId(opt))
  {
    this.m_option = opt;
  }

  protected override T FetchValue() => this.m_option == Option.INVALID ? default (T) : this.DoFetchValue();

  protected override bool RegisterChangeCallback()
  {
    if (this.m_option == Option.INVALID)
      return false;
    Options.Get().RegisterChangedListener(this.m_option, (Options.ChangedCallback) ((option, value, existed, data) => this.OnObjectChanged()));
    return true;
  }

  protected static Guid GetOptionId(Option opt)
  {
    if (ReactiveOption<T>.s_guids == null)
      ReactiveOption<T>.s_guids = new Dictionary<Option, Guid>();
    Guid optionId;
    if (!ReactiveOption<T>.s_guids.TryGetValue(opt, out optionId))
    {
      optionId = Guid.NewGuid();
      ReactiveOption<T>.s_guids.Add(opt, optionId);
    }
    return optionId;
  }

  protected abstract T DoFetchValue();
}
