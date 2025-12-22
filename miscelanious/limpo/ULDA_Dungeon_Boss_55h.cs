using System.Collections.Generic;

public class ULDA_Dungeon_Boss_55h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_55h_WaterTotingMurlocs_Birth = new AssetReference("VO_ULDA_BOSS_55h_WaterTotingMurlocs_Birth.prefab:01476de4e3df6c043ba1294641446c14");
  private static readonly AssetReference VO_ULDA_BOSS_55h_WaterTotingMurlocs_Death = new AssetReference("VO_ULDA_BOSS_55h_WaterTotingMurlocs_Death.prefab:bccb469a8623a77459b83b9d9490edad");
  private static readonly AssetReference VO_ULDA_BOSS_55h_WaterTotingMurlocs_Defeat = new AssetReference("VO_ULDA_BOSS_55h_WaterTotingMurlocs_Defeat.prefab:b2bab368c56dcd44bb2d82f0f1067dd8");
  private static readonly AssetReference VO_ULDA_BOSS_55h_WaterTotingMurlocs_EmoteResponse = new AssetReference("VO_ULDA_BOSS_55h_WaterTotingMurlocs_EmoteResponse.prefab:dfaeb48349404bd498ce431ebc244cd4");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_55h.VO_ULDA_BOSS_55h_WaterTotingMurlocs_Birth,
      (string) ULDA_Dungeon_Boss_55h.VO_ULDA_BOSS_55h_WaterTotingMurlocs_Death,
      (string) ULDA_Dungeon_Boss_55h.VO_ULDA_BOSS_55h_WaterTotingMurlocs_Defeat,
      (string) ULDA_Dungeon_Boss_55h.VO_ULDA_BOSS_55h_WaterTotingMurlocs_EmoteResponse
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_55h.VO_ULDA_BOSS_55h_WaterTotingMurlocs_Birth;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_55h.VO_ULDA_BOSS_55h_WaterTotingMurlocs_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_55h.VO_ULDA_BOSS_55h_WaterTotingMurlocs_EmoteResponse;
  }
}
