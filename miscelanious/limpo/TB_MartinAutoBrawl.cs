using Blizzard.T5.Core;

public class TB_MartinAutoBrawl : MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = TB_MartinAutoBrawl.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = TB_MartinAutoBrawl.InitStringOptions();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public TB_MartinAutoBrawl()
    : base()
  {
    this.m_gameOptions.AddOptions(TB_MartinAutoBrawl.s_booleanOptions, TB_MartinAutoBrawl.s_stringOptions);
  }

  public override bool ShouldDoAlternateMulliganIntro() => true;
}
