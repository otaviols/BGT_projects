using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_20h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_BossTriggerAncestralGuardian_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_BossTriggerAncestralGuardian_01.prefab:206a03ce91069e244b9cb2b04ff90125");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_BossTriggerWeapon_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_BossTriggerWeapon_01.prefab:0ed3c3973dec4e241a3871330f4e3d0a");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_Death_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_Death_01.prefab:42e1b8e1228d7ac41adc8d8bbdb2b831");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_DefeatPlayer_01.prefab:072e9fc9889729c40850763e922535a7");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_EmoteResponse_01.prefab:22461a25d9693684eb7c70dae90d78ce");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_01.prefab:d5342977bbefe3a4a9e56bce1e0ac2ef");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_02.prefab:b94cdc25ceaae574a918661ec2229123");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_03.prefab:588bd8cb07dc6c94e85d1d9a3103c9f3");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_04.prefab:31863d17b9bce3e41a4201e49a9c376b");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_05.prefab:57ce66b312db4f5418e9585c29855ac8");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_Idle_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_Idle_01.prefab:764c23a359c072f478666c42e4409efb");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_Idle_03 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_Idle_03.prefab:37cc8e56e16c26144a2197df83d95c90");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_Intro_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_Intro_01.prefab:02a752d5d9a8cbe478b7c332251e1348");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Bone_Wraith_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Bone_Wraith_01.prefab:2ce9e0a1acab4ce49981cb6b064c81d0");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Brazen_Zealot_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Brazen_Zealot_01.prefab:a4dace3165843af438c842f132889e1e");
  private static readonly AssetReference VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Tomb_Warden_01 = new AssetReference("VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Tomb_Warden_01.prefab:4cd4e0aefc494f349a0c64f9741d85dd");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_01,
    (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_02,
    (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_03,
    (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_04,
    (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_Idle_01,
    (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_BossTriggerAncestralGuardian_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_BossTriggerWeapon_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_Death_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_02,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_03,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_04,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_HeroPower_05,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_Idle_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_Idle_03,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_Intro_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Bone_Wraith_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Brazen_Zealot_01,
      (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Tomb_Warden_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "ULDA_Reno"))
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
    ULDA_Dungeon_Boss_20h uldaDungeonBoss20h = this;
    while (uldaDungeonBoss20h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss20h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss20h.m_HeroPowerLines);
        break;
      case 102:
        yield return (object) uldaDungeonBoss20h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_BossTriggerWeapon_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss20h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_20h uldaDungeonBoss20h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss20h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss20h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss20h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss20h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss20h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_253"))
      {
        if (!(cardId == "ULD_275"))
        {
          if (cardId == "ULD_145")
            yield return (object) uldaDungeonBoss20h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Brazen_Zealot_01);
        }
        else
          yield return (object) uldaDungeonBoss20h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Bone_Wraith_01);
      }
      else
        yield return (object) uldaDungeonBoss20h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_PlayerTrigger_Tomb_Warden_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_20h uldaDungeonBoss20h = this;
    while (uldaDungeonBoss20h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss20h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss20h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss20h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss20h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "ULD_207")
        yield return (object) uldaDungeonBoss20h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_20h.VO_ULDA_BOSS_20h_Male_Ghost_BossTriggerAncestralGuardian_01);
    }
  }
}
