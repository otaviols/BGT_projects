using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_23h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_23h_SharkyMcFin_Death = new AssetReference("VO_DALA_BOSS_23h_SharkyMcFin_Death.prefab:19fffba6da6499444a0c0895b3e26307");
  private static readonly AssetReference VO_DALA_BOSS_23h_SharkyMcFin_DefeatPlayer = new AssetReference("VO_DALA_BOSS_23h_SharkyMcFin_DefeatPlayer.prefab:78db84ca05e10a749b5653311fe40572");
  private static readonly AssetReference VO_DALA_BOSS_23h_SharkyMcFin_EmoteResponse = new AssetReference("VO_DALA_BOSS_23h_SharkyMcFin_EmoteResponse.prefab:d8ccfc2635d6b3d4e9b0a26b2ac3bc41");
  private static readonly AssetReference VO_DALA_BOSS_23h_SharkyMcFin_Intro = new AssetReference("VO_DALA_BOSS_23h_SharkyMcFin_Intro.prefab:0ef7854270498b843825a831374135e7");
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_23h.VO_DALA_BOSS_23h_SharkyMcFin_Death,
      (string) DALA_Dungeon_Boss_23h.VO_DALA_BOSS_23h_SharkyMcFin_DefeatPlayer,
      (string) DALA_Dungeon_Boss_23h.VO_DALA_BOSS_23h_SharkyMcFin_EmoteResponse,
      (string) DALA_Dungeon_Boss_23h.VO_DALA_BOSS_23h_SharkyMcFin_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_23h.VO_DALA_BOSS_23h_SharkyMcFin_Intro;
    this.m_deathLine = (string) DALA_Dungeon_Boss_23h.VO_DALA_BOSS_23h_SharkyMcFin_Death;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_23h.VO_DALA_BOSS_23h_SharkyMcFin_EmoteResponse;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_23h dalaDungeonBoss23h = this;
    while (dalaDungeonBoss23h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss23h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_23h dalaDungeonBoss23h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss23h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss23h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss23h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss23h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss23h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_23h dalaDungeonBoss23h = this;
    while (dalaDungeonBoss23h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss23h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss23h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss23h.m_playedLines.Add(cardId);
    }
  }
}
