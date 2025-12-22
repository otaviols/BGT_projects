using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_47h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_47h_Lavanthor_Death = new AssetReference("VO_DALA_BOSS_47h_Lavanthor_Death.prefab:7e208dd4154b0654a870063a4d336090");
  private static readonly AssetReference VO_DALA_BOSS_47h_Lavanthor_DefeatPlayer = new AssetReference("VO_DALA_BOSS_47h_Lavanthor_DefeatPlayer.prefab:c200bd9d856ebfd4e8c6469989200ec1");
  private static readonly AssetReference VO_DALA_BOSS_47h_Lavanthor_EmoteResponse = new AssetReference("VO_DALA_BOSS_47h_Lavanthor_EmoteResponse.prefab:15154e48f0b0d1e4dbc63ccfe61b0284");
  private static readonly AssetReference VO_DALA_BOSS_47h_Lavanthor_Intro = new AssetReference("VO_DALA_BOSS_47h_Lavanthor_Intro.prefab:b2e242f69f7a7e44c8fc14ed07d35736");
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_47h.VO_DALA_BOSS_47h_Lavanthor_Death,
      (string) DALA_Dungeon_Boss_47h.VO_DALA_BOSS_47h_Lavanthor_DefeatPlayer,
      (string) DALA_Dungeon_Boss_47h.VO_DALA_BOSS_47h_Lavanthor_EmoteResponse,
      (string) DALA_Dungeon_Boss_47h.VO_DALA_BOSS_47h_Lavanthor_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_47h.VO_DALA_BOSS_47h_Lavanthor_Intro;
    this.m_deathLine = (string) DALA_Dungeon_Boss_47h.VO_DALA_BOSS_47h_Lavanthor_Death;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_47h.VO_DALA_BOSS_47h_Lavanthor_EmoteResponse;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_47h dalaDungeonBoss47h = this;
    while (dalaDungeonBoss47h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss47h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_47h dalaDungeonBoss47h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss47h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss47h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss47h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss47h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss47h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_47h dalaDungeonBoss47h = this;
    while (dalaDungeonBoss47h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss47h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss47h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss47h.m_playedLines.Add(cardId);
    }
  }
}
