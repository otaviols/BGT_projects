using System.Collections.Generic;

public class ULDA_Dungeon_Boss_59h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_59h_Direbat_Death = new AssetReference("VO_ULDA_BOSS_59h_Direbat_Death.prefab:96c1eb6503063ec4e80c26b7a5f722e4");
  private static readonly AssetReference VO_ULDA_BOSS_59h_Direbat_Defeat = new AssetReference("VO_ULDA_BOSS_59h_Direbat_Defeat.prefab:49e88284ba7f8c940ab36228a7777596");
  private static readonly AssetReference VO_ULDA_BOSS_59h_Direbat_EmoteResponse = new AssetReference("VO_ULDA_BOSS_59h_Direbat_EmoteResponse.prefab:1a6fc68c44fc167419ec4fbd9d4fe76b");
  private static readonly AssetReference VO_ULDA_BOSS_59h_Direbat_Intro = new AssetReference("VO_ULDA_BOSS_59h_Direbat_Intro.prefab:17fcdafd96828c94a899de8668b95ce2");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_59h.VO_ULDA_BOSS_59h_Direbat_Death,
      (string) ULDA_Dungeon_Boss_59h.VO_ULDA_BOSS_59h_Direbat_Defeat,
      (string) ULDA_Dungeon_Boss_59h.VO_ULDA_BOSS_59h_Direbat_EmoteResponse,
      (string) ULDA_Dungeon_Boss_59h.VO_ULDA_BOSS_59h_Direbat_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_59h.VO_ULDA_BOSS_59h_Direbat_Intro;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_59h.VO_ULDA_BOSS_59h_Direbat_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_59h.VO_ULDA_BOSS_59h_Direbat_EmoteResponse;
  }
}
