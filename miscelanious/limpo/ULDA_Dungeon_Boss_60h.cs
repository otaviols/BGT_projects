using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_60h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerCthun_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerCthun_01.prefab:832d52d6c27ef3a41ae7c11ed17e4466");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerFaceless_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerFaceless_01.prefab:b0180209467fc604f8f71c331f28579a");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerLineLieutenant_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerLineLieutenant_01.prefab:28ca15992f5c5b84f9c80726ccc37e74");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_Death_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_Death_01.prefab:8b5e93c5f15d03044b5477876329df5b");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_DefeatPlayer_01.prefab:fdbb390966fd8264d906e22a14828473");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_EmoteResponse_01.prefab:218d3d4ccfb266644ac630880e738ccd");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_02.prefab:440880063ebdac3449f1df8df66b8871");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_03.prefab:c0e0700250a91d249870f2ce5091b303");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_04.prefab:db96b1d8e1f1ffe4791fa75534b29d66");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_05.prefab:006b499fcea589947b6848dc60fefedf");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_Idle_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_Idle_01.prefab:943ddd8352ce58246a5b8ce18154e56d");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_Idle_02 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_Idle_02.prefab:2a3765d56abef1143b32b60e68ad6513");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_Idle_03 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_Idle_03.prefab:6a0576ec316d5954998a27ce1707997f");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_Intro_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_Intro_01.prefab:22e2fef3828561e46b83e176bacd2147");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_IntroResponseReno_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_IntroResponseReno_01.prefab:275ae9f9c7ac3a845ac36ade2fd10446");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Cthun_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Cthun_01.prefab:9696080ec36f628499cac7d24e1c5a66");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Doomsayer_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Doomsayer_01.prefab:b6c2a25e18f840e439234c300e9a15c6");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_History_Buff_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_History_Buff_01.prefab:3ddffbfd01e602c4d8325d8da126d37b");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Pharaohs_Blessing_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Pharaohs_Blessing_01.prefab:0bf3da4fccdb2af4084a8621a7141878");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Puzzle_Box_of_Yogg_Saron_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Puzzle_Box_of_Yogg_Saron_01.prefab:f5801d2dc607d5149b12c95ef0d7c2da");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_ShadowWordPain_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_ShadowWordPain_01.prefab:852ff26535557cc4986fe220f77b13d7");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerCThun_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerCThun_01.prefab:ae6ea80bb8f3a094fa76cc5896beb20d");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerCThunDefeatBoss_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerCThunDefeatBoss_01.prefab:47f5a78f2981a7d43b3bddb129d23d1f");
  private static readonly AssetReference VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerFatigue_01 = new AssetReference("VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerFatigue_01.prefab:37b4ca4b53526784994ef4c6f6170e0d");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_02,
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_03,
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_04,
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Idle_01,
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Idle_02,
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Idle_03
  };
  private List<string> m_PlayerTriggerCThun = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Cthun_01,
    (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerCThun_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerCthun_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerFaceless_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerLineLieutenant_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Death_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_02,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_03,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_04,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_HeroPower_05,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Idle_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Idle_02,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Idle_03,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Intro_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_IntroResponseReno_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Cthun_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Doomsayer_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_History_Buff_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Pharaohs_Blessing_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Puzzle_Box_of_Yogg_Saron_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_ShadowWordPain_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerCThun_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerCThunDefeatBoss_01,
      (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerFatigue_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Reno")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_IntroResponseReno_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
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
    ULDA_Dungeon_Boss_60h uldaDungeonBoss60h = this;
    while (uldaDungeonBoss60h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerCThunDefeatBoss_01);
        break;
      case 102:
        yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTriggerFatigue_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss60h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_60h uldaDungeonBoss60h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss60h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss60h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss60h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss60h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss60h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_143"))
      {
        if (!(cardId == "ULD_290"))
        {
          if (!(cardId == "OG_280"))
          {
            if (!(cardId == "NEW1_021"))
            {
              if (!(cardId == "CS2_234"))
              {
                if (cardId == "ULD_216")
                  yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Puzzle_Box_of_Yogg_Saron_01);
              }
              else
                yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_ShadowWordPain_01);
            }
            else
              yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Doomsayer_01);
          }
          else
            yield return (object) uldaDungeonBoss60h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss60h.m_PlayerTriggerCThun);
        }
        else
          yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_History_Buff_01);
      }
      else
        yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_PlayerTrigger_Pharaohs_Blessing_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_60h uldaDungeonBoss60h = this;
    while (uldaDungeonBoss60h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss60h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss60h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss60h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss60h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "DAL_613":
        case "DAL_744":
        case "EX1_564":
        case "OG_024":
        case "OG_141":
        case "OG_174":
        case "OG_207":
          yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerFaceless_01);
          break;
        case "OG_280":
          yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerCthun_01);
          break;
        case "ULD_189":
          yield return (object) uldaDungeonBoss60h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_60h.VO_ULDA_BOSS_60h_Male_Ethereal_BossTriggerLineLieutenant_01);
          break;
      }
    }
  }
}
