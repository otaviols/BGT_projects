using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_13h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_Death_01 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_Death_01.prefab:7d1fa6f34da72af4cb5819da98277e7e");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_DefeatPlayer_01.prefab:f120d0002717df74da0cf6d6983aa142");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_EmoteResponse_01.prefab:c1f9b9bc10545de4d9f6cd6c64317eaf");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_HeroPower_02 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_HeroPower_02.prefab:040ee66d9bc558943abc5e79149e92d4");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_HeroPower_03 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_HeroPower_03.prefab:c8dc93b06a686104b9f38b345bd08b3b");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_HeroPower_04 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_HeroPower_04.prefab:80514398e6f47cc4baccb2e8460f7b52");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_HeroPower_05 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_HeroPower_05.prefab:6de69f513fc739d41bbcbe5136148752");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_HeroPower_06 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_HeroPower_06.prefab:279101da6ff5a4a4f8991ead8e88593f");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_HeroPowerFriendly_01 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_HeroPowerFriendly_01.prefab:5bfc1fcf7822c4c4cb709e35e1901d7b");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_HeroPowerFriendly_02 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_HeroPowerFriendly_02.prefab:38417adf93a31884aba3ef47c27b4827");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_Idle_01 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_Idle_01.prefab:b8d28c02135ef2c4082a1eeb14729708");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_Idle_02 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_Idle_02.prefab:53644bdaa2e3d6e40877cb3f10f694c3");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_Idle_05 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_Idle_05.prefab:0bd4263c7a176d94ab6c90cdd1834796");
  private static readonly AssetReference VO_DALA_BOSS_13h_Male_Worgen_Intro_01 = new AssetReference("VO_DALA_BOSS_13h_Male_Worgen_Intro_01.prefab:efd712918aa54504783f8fb8dca274f6");
  private static List<string> HeroPowerFriendly = new List<string>()
  {
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPowerFriendly_01,
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPowerFriendly_02
  };
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Idle_01,
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Idle_02,
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Idle_05
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Death_01,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_02,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_03,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_04,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_05,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_06,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPowerFriendly_01,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPowerFriendly_02,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Idle_01,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Idle_02,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Idle_05,
      (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Intro_01
    };
    this.SetBossVOLines(VOLines);
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_13h.m_IdleLines;

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_02,
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_03,
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_04,
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_05,
    (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_HeroPower_06
  };

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_13h.VO_DALA_BOSS_13h_Male_Worgen_EmoteResponse_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Eudora") || !(cardId != "DALA_Vessina"))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_13h dalaDungeonBoss13h = this;
    while (dalaDungeonBoss13h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) dalaDungeonBoss13h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_13h.HeroPowerFriendly);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss13h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_13h dalaDungeonBoss13h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss13h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss13h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss13h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss13h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss13h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_13h dalaDungeonBoss13h = this;
    while (dalaDungeonBoss13h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss13h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss13h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss13h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss13h.m_playedLines.Add(cardId);
    }
  }
}
