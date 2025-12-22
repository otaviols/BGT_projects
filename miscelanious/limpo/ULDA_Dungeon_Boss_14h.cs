using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_14h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_BossPoisonWeapon_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_BossPoisonWeapon_01.prefab:f4027680ee899b149b43d5db73bd9b43");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_BossStolenSteel_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_BossStolenSteel_01.prefab:33a3a3bd1a15fca49a97649753868ac5");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_Death_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_Death_01.prefab:607940c64b781ca4a89437e6c9ad888b");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_DeathALT_01.prefab:5b05801540a9fd4428d3447edab589e4");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_DefeatPlayer_01.prefab:bb91a3b9dffc0424096097091d23232b");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_EmoteResponse_01.prefab:d9891ebde053f3a42a9deb6393f52cce");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerPlayShuffledWeapons_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerPlayShuffledWeapons_01.prefab:e16f2345bbd61ee439b44aefdc111026");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerPlayShuffledWeapons_02 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerPlayShuffledWeapons_02.prefab:5b153a9dc205b764196d2cc93f777f93");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_01.prefab:734128a7941a0fe4c8be600a6858f2b3");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_02 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_02.prefab:11a5db4d6f820b241ad8570f2fc3e3e2");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_03 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_03.prefab:e35b47ef5762d1845baf42cdb479d4ee");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_Idle_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_Idle_01.prefab:0b4433893f0165c479d109835b61dc7e");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_Idle_02 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_Idle_02.prefab:a5a6f0372a1612e49a5d4d00c1118ea5");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_Idle_03 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_Idle_03.prefab:61840672e1922d44e96c5211bfcc1ffe");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_Intro_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_Intro_01.prefab:fa9042398dc64744a87ebbf9ef9e2fa8");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_PlayerDestroyWeapon_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_PlayerDestroyWeapon_01.prefab:a0e91f0f7e0e3654e817defda411a009");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_PlayerKingsbane_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_PlayerKingsbane_01.prefab:3e5552f032facd5459fd184fa78e35c5");
  private static readonly AssetReference VO_ULDA_BOSS_14h_Female_Sethrak_PlayerStolenSteel_01 = new AssetReference("VO_ULDA_BOSS_14h_Female_Sethrak_PlayerStolenSteel_01.prefab:78c415b4ee63479488fae89fd2f047cb");
  private List<string> m_HeroPowerPlayShuffledWeaponsLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerPlayShuffledWeapons_01,
    (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerPlayShuffledWeapons_02
  };
  private List<string> m_HeroPowerTriggerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_01,
    (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_02,
    (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_03
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Idle_01,
    (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Idle_02,
    (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_BossPoisonWeapon_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_BossStolenSteel_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Death_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_DeathALT_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerPlayShuffledWeapons_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerPlayShuffledWeapons_02,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_02,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_HeroPowerTrigger_03,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Idle_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Idle_02,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Idle_03,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Intro_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_PlayerDestroyWeapon_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_PlayerKingsbane_01,
      (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_PlayerStolenSteel_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
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
    ULDA_Dungeon_Boss_14h uldaDungeonBoss14h = this;
    while (uldaDungeonBoss14h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss14h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss14h.m_HeroPowerTriggerLines);
        break;
      case 102:
        yield return (object) uldaDungeonBoss14h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss14h.m_HeroPowerPlayShuffledWeaponsLines);
        break;
      case 103:
        yield return (object) uldaDungeonBoss14h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_PlayerDestroyWeapon_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss14h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_14h uldaDungeonBoss14h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss14h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss14h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss14h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss14h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss14h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "LOOT_542"))
      {
        if (cardId == "TRL_156")
          yield return (object) uldaDungeonBoss14h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_PlayerStolenSteel_01);
      }
      else
        yield return (object) uldaDungeonBoss14h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_PlayerKingsbane_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_14h uldaDungeonBoss14h = this;
    while (uldaDungeonBoss14h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss14h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss14h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss14h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss14h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_074"))
      {
        if (cardId == "TRL_156")
          yield return (object) uldaDungeonBoss14h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_BossStolenSteel_01);
      }
      else
        yield return (object) uldaDungeonBoss14h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_14h.VO_ULDA_BOSS_14h_Female_Sethrak_BossPoisonWeapon_01);
    }
  }
}
