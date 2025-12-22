using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_44h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_BossArakoaPlayed_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_BossArakoaPlayed_01.prefab:2b8a991f05085af49939c9b32477bae8");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_BossShamanDamageSpell_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_BossShamanDamageSpell_01.prefab:bc022a1bc6a6b0946b9e93cdec79ef40");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_Death_02 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_Death_02.prefab:f5bf9ffbaf7275e498664c5cff721bb5");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_DefeatPlayer_01.prefab:5fdb529ce1bc8da4f9c974cc87012514");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_EmoteResponse_01.prefab:95b9a2a8c81bcc6499926001d31b5939");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_01.prefab:3e9b2a4b9c944fe4ebcbc4022a6b2be8");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_02 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_02.prefab:beb97e4e5a22cfe418b259ef1310383f");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_03 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_03.prefab:339a3bfab079bc947a2ca8500f9330e8");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_01.prefab:2b2253ffb72c06248a2205b113a5d865");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_02 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_02.prefab:38a4df2dcfffa634480f81d82bae3dff");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_03 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_03.prefab:ae2a338cc382bb1428ba11f2a726f84f");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_04 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_04.prefab:69a8371277b25a74499783548d407298");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_05 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_05.prefab:821a0aafb777f2143851e65b2836af23");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_Idle_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_Idle_01.prefab:b0e87c0523eaed745a705994ab101c3a");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_Idle_02 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_Idle_02.prefab:40783cb4a8355b847ba7ab57f1b7e7ad");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_Idle_03 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_Idle_03.prefab:f4e01ba448bdb5d46b50f636375b5d13");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_Intro_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_Intro_01.prefab:9c67a65d3460aef42bf1096904403de9");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_IntroKriziki_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_IntroKriziki_01.prefab:befc9255c7514a648b3cbf6f72cd78a0");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_PlayerGnomeferatu_01 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_PlayerGnomeferatu_01.prefab:1578e4a57d072234f977b69ff9b35ca6");
  private static readonly AssetReference VO_DALA_BOSS_44h_Male_Arakkoa_PlayerGnomeferatu_02 = new AssetReference("VO_DALA_BOSS_44h_Male_Arakkoa_PlayerGnomeferatu_02.prefab:7b823ebad582c4346972e779258d1854");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Idle_01,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Idle_02,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Idle_03
  };
  private static List<string> m_Gnomeferatu = new List<string>()
  {
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_PlayerGnomeferatu_01,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_PlayerGnomeferatu_02
  };
  private static List<string> m_BossBurnsCardOnDraw = new List<string>()
  {
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_01,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_02,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_BossArakoaPlayed_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_BossShamanDamageSpell_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Death_02,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_02,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HandFull_03,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_02,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_03,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_04,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_05,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Idle_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Idle_02,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Idle_03,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Intro_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_IntroKriziki_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_PlayerGnomeferatu_01,
      (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_PlayerGnomeferatu_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_Death_02;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_44h.m_IdleLines;

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_01,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_02,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_03,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_04,
    (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_HeroPower_05
  };

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Kriziki"))
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

  protected override bool GetShouldSuppressDeathTextBubble() => false;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_44h dalaDungeonBoss44h = this;
    while (dalaDungeonBoss44h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) dalaDungeonBoss44h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_44h.m_BossBurnsCardOnDraw);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss44h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_44h dalaDungeonBoss44h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss44h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss44h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss44h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss44h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss44h.m_playedLines.Add(cardId);
      if (cardId == "ICC_407")
        yield return (object) dalaDungeonBoss44h.PlayAndRemoveRandomLineOnlyOnce(enemyActor, DALA_Dungeon_Boss_44h.m_Gnomeferatu);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_44h dalaDungeonBoss44h = this;
    while (dalaDungeonBoss44h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss44h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss44h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss44h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss44h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "GIL_600") && !(cardId == "EX1_238") && !(cardId == "EX1_245"))
      {
        if (cardId == "OG_293" || cardId == "CFM_626")
          yield return (object) dalaDungeonBoss44h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_BossArakoaPlayed_01);
      }
      else
        yield return (object) dalaDungeonBoss44h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_44h.VO_DALA_BOSS_44h_Male_Arakkoa_BossShamanDamageSpell_01);
    }
  }
}
