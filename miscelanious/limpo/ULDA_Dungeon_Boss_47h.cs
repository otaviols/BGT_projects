using System.Collections.Generic;

public class ULDA_Dungeon_Boss_47h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_47h_TotallyNormalJar_Death = new AssetReference("VO_ULDA_BOSS_47h_TotallyNormalJar_Death.prefab:d3fe1495da0da89419f95e57ee3ec3a6");
  private static readonly AssetReference VO_ULDA_BOSS_47h_TotallyNormalJar_Defeat = new AssetReference("VO_ULDA_BOSS_47h_TotallyNormalJar_Defeat.prefab:7d565277f5790d14382ab754560d458f");
  private static readonly AssetReference VO_ULDA_BOSS_47h_TotallyNormalJar_EmoteResponse = new AssetReference("VO_ULDA_BOSS_47h_TotallyNormalJar_EmoteResponse.prefab:0dd98ef8348c0bc43bc619d5ca49f753");
  private static readonly AssetReference VO_ULDA_BOSS_47h_TotallyNormalJar_Intro = new AssetReference("VO_ULDA_BOSS_47h_TotallyNormalJar_Intro.prefab:753c137fc7681b142889fce3f6298c16");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_47h.VO_ULDA_BOSS_47h_TotallyNormalJar_Death,
      (string) ULDA_Dungeon_Boss_47h.VO_ULDA_BOSS_47h_TotallyNormalJar_Defeat,
      (string) ULDA_Dungeon_Boss_47h.VO_ULDA_BOSS_47h_TotallyNormalJar_EmoteResponse,
      (string) ULDA_Dungeon_Boss_47h.VO_ULDA_BOSS_47h_TotallyNormalJar_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_47h.VO_ULDA_BOSS_47h_TotallyNormalJar_Intro;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_47h.VO_ULDA_BOSS_47h_TotallyNormalJar_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_47h.VO_ULDA_BOSS_47h_TotallyNormalJar_EmoteResponse;
  }
}
