using Assets;
using Blizzard.T5.Core;

public class RichPresence
{
  public static readonly FourCC STATUS_STREAMID = new FourCC("stat");
  public static readonly FourCC TUTORIAL_STREAMID = new FourCC("tut");
  public static readonly FourCC SCENARIOS_STREAMID = new FourCC("scen");
  public static readonly Map<System.Type, FourCC> s_streamIds = new Map<System.Type, FourCC>()
  {
    {
      typeof (Global.PresenceStatus),
      RichPresence.STATUS_STREAMID
    },
    {
      typeof (PresenceTutorial),
      RichPresence.TUTORIAL_STREAMID
    },
    {
      typeof (ScenarioDbId),
      RichPresence.SCENARIOS_STREAMID
    }
  };
}
