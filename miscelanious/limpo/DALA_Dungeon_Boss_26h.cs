using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_26h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_26h_DalaranFountainGolem_Death = new AssetReference("VO_DALA_BOSS_26h_DalaranFountainGolem_Death.prefab:96a8fe4578eb0f24db266fc11ce39e56");
  private static readonly AssetReference VO_DALA_BOSS_26h_DalaranFountainGolem_DefeatPlayer = new AssetReference("VO_DALA_BOSS_26h_DalaranFountainGolem_DefeatPlayer.prefab:f2401c70f688a5b4d8938932beaebee4");
  private static readonly AssetReference VO_DALA_BOSS_26h_DalaranFountainGolem_EmoteResponse = new AssetReference("VO_DALA_BOSS_26h_DalaranFountainGolem_EmoteResponse.prefab:f0c832b4e45f0db4ab850fc140c1f9f3");
  private static readonly AssetReference VO_DALA_BOSS_26h_DalaranFountainGolem_Intro = new AssetReference("VO_DALA_BOSS_26h_DalaranFountainGolem_Intro.prefab:06e0e9a059c18d64dbbaa86404a76315");
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_26h.VO_DALA_BOSS_26h_DalaranFountainGolem_Death,
      (string) DALA_Dungeon_Boss_26h.VO_DALA_BOSS_26h_DalaranFountainGolem_DefeatPlayer,
      (string) DALA_Dungeon_Boss_26h.VO_DALA_BOSS_26h_DalaranFountainGolem_EmoteResponse,
      (string) DALA_Dungeon_Boss_26h.VO_DALA_BOSS_26h_DalaranFountainGolem_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_26h.VO_DALA_BOSS_26h_DalaranFountainGolem_Intro;
    this.m_deathLine = (string) DALA_Dungeon_Boss_26h.VO_DALA_BOSS_26h_DalaranFountainGolem_Death;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_26h.VO_DALA_BOSS_26h_DalaranFountainGolem_EmoteResponse;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_26h dalaDungeonBoss26h = this;
    while (dalaDungeonBoss26h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss26h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_26h dalaDungeonBoss26h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss26h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss26h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss26h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss26h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss26h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_26h dalaDungeonBoss26h = this;
    while (dalaDungeonBoss26h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss26h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss26h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss26h.m_playedLines.Add(cardId);
    }
  }
}
