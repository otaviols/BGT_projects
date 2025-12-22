using Hearthstone.Attribution;
using System;

public static class AppLaunchTracker
{
  public static int LaunchCount
  {
    get => Options.Get().GetInt(Option.LAUNCH_COUNT, 0);
    private set => Options.Get().SetInt(Option.LAUNCH_COUNT, value);
  }

  public static bool IsInstallReported
  {
    get => Options.Get().GetBool(Option.IS_INSTALL_REPORTED, false);
    set => Options.Get().SetBool(Option.IS_INSTALL_REPORTED, value);
  }

  public static ulong FirstInstallTimeMilliseconds => Options.Get().GetULong(Option.FIRST_INSTALL_TIME, 0UL);

  private static void SetInstallTimeIfNotSet()
  {
    if (Options.Get().HasOption(Option.FIRST_INSTALL_TIME))
      return;
    Options.Get().SetULong(Option.FIRST_INSTALL_TIME, TimeUtils.DateTimeToUnixTimeStampMilliseconds(DateTime.UtcNow));
  }

  public static void TrackAppLaunch()
  {
    AppLaunchTracker.SetInstallTimeIfNotSet();
    ++AppLaunchTracker.LaunchCount;
    if (!AppLaunchTracker.IsInstallReported)
    {
      if (AppLaunchTracker.FirstInstallTimeMilliseconds > 1572984000000UL)
        BlizzardAttributionManager.Get().SendEvent_Install();
      else
        AppLaunchTracker.IsInstallReported = true;
    }
    BlizzardAttributionManager.Get().SendEvent_Launch();
  }
}
