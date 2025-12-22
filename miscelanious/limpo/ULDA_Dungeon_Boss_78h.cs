using System.Collections.Generic;

public class ULDA_Dungeon_Boss_78h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_78h_Octosari_Death = new AssetReference("VO_ULDA_BOSS_78h_Octosari_Death.prefab:ad50902e1822c7f43b9afc99efcb0e84");
  private static readonly AssetReference VO_ULDA_BOSS_78h_Octosari_DefeatPlayer = new AssetReference("VO_ULDA_BOSS_78h_Octosari_DefeatPlayer.prefab:97391cfd5487cdb44a8fd0a67af589d8");
  private static readonly AssetReference VO_ULDA_BOSS_78h_Octosari_EmoteResponse = new AssetReference("VO_ULDA_BOSS_78h_Octosari_EmoteResponse.prefab:66a84be4d1ebacc47b89cc6e63ff7a32");
  private static readonly AssetReference VO_ULDA_BOSS_78h_Octosari_Intro = new AssetReference("VO_ULDA_BOSS_78h_Octosari_Intro.prefab:cff7e88ec4c0dde4a99502bd038678a5");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_78h.VO_ULDA_BOSS_78h_Octosari_Death,
      (string) ULDA_Dungeon_Boss_78h.VO_ULDA_BOSS_78h_Octosari_DefeatPlayer,
      (string) ULDA_Dungeon_Boss_78h.VO_ULDA_BOSS_78h_Octosari_EmoteResponse,
      (string) ULDA_Dungeon_Boss_78h.VO_ULDA_BOSS_78h_Octosari_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_78h.VO_ULDA_BOSS_78h_Octosari_Intro;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_78h.VO_ULDA_BOSS_78h_Octosari_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_78h.VO_ULDA_BOSS_78h_Octosari_EmoteResponse;
  }
}
