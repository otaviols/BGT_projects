using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_01h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_01h_Chomper_Death = new AssetReference("VO_DALA_BOSS_01h_Chomper_Death.prefab:a67617e34bd46ad4b86ce38b27538336");
  private static readonly AssetReference VO_DALA_BOSS_01h_Chomper_DefeatPlayer = new AssetReference("VO_DALA_BOSS_01h_Chomper_DefeatPlayer.prefab:7b9e096137b452c4bb0122120a526089");
  private static readonly AssetReference VO_DALA_BOSS_01h_Chomper_EmoteResponse = new AssetReference("VO_DALA_BOSS_01h_Chomper_EmoteResponse.prefab:a3805142083d27642ab9ace616499a88");
  private static readonly AssetReference VO_DALA_BOSS_01h_Chomper_Intro = new AssetReference("VO_DALA_BOSS_01h_Chomper_Intro.prefab:a4808c11753e77b43947a481f0fa7f43");
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_01h.VO_DALA_BOSS_01h_Chomper_Death,
      (string) DALA_Dungeon_Boss_01h.VO_DALA_BOSS_01h_Chomper_DefeatPlayer,
      (string) DALA_Dungeon_Boss_01h.VO_DALA_BOSS_01h_Chomper_EmoteResponse,
      (string) DALA_Dungeon_Boss_01h.VO_DALA_BOSS_01h_Chomper_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_01h.VO_DALA_BOSS_01h_Chomper_Intro;
    this.m_deathLine = (string) DALA_Dungeon_Boss_01h.VO_DALA_BOSS_01h_Chomper_Death;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_01h.VO_DALA_BOSS_01h_Chomper_EmoteResponse;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_01h dalaDungeonBoss01h = this;
    while (dalaDungeonBoss01h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss01h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_01h dalaDungeonBoss01h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss01h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss01h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss01h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss01h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss01h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_01h dalaDungeonBoss01h = this;
    while (dalaDungeonBoss01h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss01h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss01h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss01h.m_playedLines.Add(cardId);
    }
  }
}
