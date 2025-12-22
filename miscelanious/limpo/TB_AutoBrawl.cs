using Blizzard.T5.Core;

public class TB_AutoBrawl : MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = TB_AutoBrawl.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = TB_AutoBrawl.InitStringOptions();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public TB_AutoBrawl()
    : base()
  {
    this.m_gameOptions.AddOptions(TB_AutoBrawl.s_booleanOptions, TB_AutoBrawl.s_stringOptions);
  }

  public override bool ShouldDoAlternateMulliganIntro() => true;
}
