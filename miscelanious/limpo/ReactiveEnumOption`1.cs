using System;

public class ReactiveEnumOption<T> : ReactiveOption<T> where T : struct, IConvertible
{
  public ReactiveEnumOption(Option val)
    : base(val)
  {
    if (!typeof (T).IsEnum)
      throw new Exception("T must be an enumerated type");
  }

  public static ReactiveEnumOption<T> CreateInstance(Option opt) => (ReactiveObject<T>.GetExistingInstance(ReactiveOption<T>.GetOptionId(opt)) ?? (ReactiveObject<T>) new ReactiveEnumOption<T>(opt)) as ReactiveEnumOption<T>;

  protected override T DoFetchValue() => Options.Get().GetEnum<T>(this.m_option);

  public override void Set(T newValue) => Options.Get().SetEnum<T>(this.m_option, newValue);
}
