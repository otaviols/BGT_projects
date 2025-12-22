using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;

public class HearthstoneLocalization
{
  public static readonly PlatformDependentValue<bool> LOCALE_FROM_OPTIONS = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    iOS = true,
    Android = true,
    PC = false,
    Mac = false
  };

  public static void Initialize()
  {
    Locale? nullable = new Locale?();
    Locale outVal1;
    if ((bool) HearthstoneLocalization.LOCALE_FROM_OPTIONS && Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<Locale>(Options.Get().GetString(Option.LOCALE), out outVal1))
      nullable = new Locale?(outVal1);
    if (!nullable.HasValue)
    {
      string str1 = (string) null;
      if (HearthstoneApplication.IsPublic())
        str1 = BattleNet.GetLaunchOption("LOCALE", false);
      if (string.IsNullOrEmpty(str1))
        str1 = Vars.Key("Localization.Locale").GetStr(Localization.DEFAULT_LOCALE_NAME);
      if (HearthstoneApplication.IsInternal())
      {
        string str2 = Vars.Key("Localization.OverrideBnetLocale").GetStr("");
        if (!string.IsNullOrEmpty(str2))
          str1 = str2;
      }
      Locale outVal2;
      nullable = !Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<Locale>(str1, out outVal2) ? new Locale?(Locale.enUS) : new Locale?(outVal2);
    }
    Localization.RegisterSetLocaleDoneCallback(new System.Action<Locale>(HearthstoneLocalization.OnSetLocalDone));
    Localization.SetLocale(nullable.Value);
  }

  private static void OnSetLocalDone(Locale locale)
  {
    DataContext dataContext = GlobalDataContext.Get();
    IDataModel dataModel = (IDataModel) null;
    ref IDataModel local = ref dataModel;
    if (!dataContext.GetDataModel(153, out local))
      return;
    (dataModel as AccountDataModel).Language = Localization.GetLocale();
  }
}
