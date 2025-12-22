using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_16h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_BossLivingMonument_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_BossLivingMonument_01.prefab:e19979b124813df4ea60a858301aa8dc");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_BossPharoahCat_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_BossPharoahCat_01.prefab:d490c54d7b5dcb7449219d7a3f4b81bf");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_BossWastelandScorpid_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_BossWastelandScorpid_01.prefab:6e2287671c626494bae598ac1c8391e3");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_Death_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_Death_01.prefab:8c42b4718167aeb41b94340afc8e5a99");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_DefeatPlayer_01.prefab:3f94a44833135534bb266edb6fba29a0");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_EmoteResponse_01.prefab:18f01fdb6e83ad14bba08f1d0201dd8c");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_01.prefab:1035da81b396ba548a0cee2c9ac8f1be");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_02 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_02.prefab:385556a0452255040afaee4ca69eb19a");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_03 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_03.prefab:55415163087181643bc5c60ac1427953");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_04 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_04.prefab:59bdf3254f14bce4893c46c94d167dfe");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_Idle_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_Idle_01.prefab:e5574b0cc2ab90c4ea8ee06c39f4c183");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_Idle_02 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_Idle_02.prefab:029e97e2ddc74b64c8144839a2d912e4");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_Idle_03 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_Idle_03.prefab:19cc36bc9abbf564f9369dac62e60c4c");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_Intro_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_Intro_01.prefab:a87391e6aca01904c9e964a5f2ae2be2");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_IntroEliseResponse_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_IntroEliseResponse_01.prefab:2847787990484a9468f42f0148c80fb9");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_IntroFinleyResponse_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_IntroFinleyResponse_01.prefab:fc6f5988aa3e16d41b73e5eccab16693");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_PlayerTrigger_Restless_Mummy_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_PlayerTrigger_Restless_Mummy_01.prefab:24c85f783c3610745b90b398cad1f00a");
  private static readonly AssetReference VO_ULDA_BOSS_16h_Male_Ogre_PlayerTrigger_Sunstruck_Henchman_01 = new AssetReference("VO_ULDA_BOSS_16h_Male_Ogre_PlayerTrigger_Sunstruck_Henchman_01.prefab:553b91fa30e96a943b3871d0e4cef235");
  private List<string> m_HeroPowerTriggerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_01,
    (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_02,
    (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_03,
    (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_04
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Idle_01,
    (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Idle_02,
    (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_BossLivingMonument_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_BossPharoahCat_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_BossWastelandScorpid_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Death_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_02,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_03,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_HeroPowerTrigger_04,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Idle_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Idle_02,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Idle_03,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Intro_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_IntroEliseResponse_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_IntroFinleyResponse_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_PlayerTrigger_Restless_Mummy_01,
      (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_PlayerTrigger_Sunstruck_Henchman_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Elise")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_IntroEliseResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "ULDA_Finley")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_IntroFinleyResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_16h uldaDungeonBoss16h = this;
    while (uldaDungeonBoss16h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss16h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss16h.m_HeroPowerTriggerLines);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss16h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_16h uldaDungeonBoss16h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss16h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss16h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss16h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss16h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss16h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_180"))
      {
        if (cardId == "ULD_206")
          yield return (object) uldaDungeonBoss16h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_PlayerTrigger_Restless_Mummy_01);
      }
      else
        yield return (object) uldaDungeonBoss16h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_PlayerTrigger_Sunstruck_Henchman_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_16h uldaDungeonBoss16h = this;
    while (uldaDungeonBoss16h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss16h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss16h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss16h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss16h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_193"))
      {
        if (!(cardId == "ULD_186"))
        {
          if (cardId == "ULD_194")
            yield return (object) uldaDungeonBoss16h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_BossWastelandScorpid_01);
        }
        else
          yield return (object) uldaDungeonBoss16h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_BossPharoahCat_01);
      }
      else
        yield return (object) uldaDungeonBoss16h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_16h.VO_ULDA_BOSS_16h_Male_Ogre_BossLivingMonument_01);
    }
  }
}
