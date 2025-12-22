using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTA_Prologue_Fight_03 : BTA_Prologue_Dungeon
{
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Death_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Death_01.prefab:b6a55707d36999646b64f240eedc8024");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_EmoteResponse_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_EmoteResponse_01.prefab:378dc07a23179714c8fe918124abc111");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_01_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_01_01.prefab:02dcb71f3b441f14ca1d5b9fd3ea548c");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_02_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_02_01.prefab:63a8af3bbd8c4184eacfe337af95f915");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_03_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_03_01.prefab:2c97029f2379e0942988e405391a3a94");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_01_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_01_01.prefab:7d53b1c9b32145c40857178a8e7557b9");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_02_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_02_01.prefab:cce53bb4c3100d847a8ee6f60aa7c2a9");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_03_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_03_01.prefab:dd95db906aa9ab64485eea53cec6c25f");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeA_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeA_01.prefab:14572c61115e4f6468f78106f4fecdf9");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeB_01_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeB_01_01.prefab:390deb6b13c6333458f7a76cfc88d859");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeC_01_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeC_01_01.prefab:8964e50a67f91bb4696c9789507728a1");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_Intro01_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_Intro01_01.prefab:5a3d6d17ae3583146be8ed89bf239eb2");
  private static readonly AssetReference VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_Loss_01 = new AssetReference("VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_Loss_01.prefab:97e8b9d8a1d2e044fa79f5fca38cc05e");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeB_02_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeB_02_01.prefab:8ad0c499e218b394abd8da1ae28b8088");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeC_02_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeC_02_01.prefab:ca3b4823ab2ef0847ac1566dc5c33aa0");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeD_02_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeD_02_01.prefab:5bbd35b5b51bf604c84daf23774d29f7");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Intro03_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Intro03_01.prefab:ef5a638c1cd33c44686f8a5feb8afc0f");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Turn1_02_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Turn1_02_01.prefab:2e986a1010f324142b518a5ed272c477");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Turn2_01_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Turn2_01_01.prefab:9be734993b596304abe45ac010e36dc3");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Victory_01_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Victory_01_01.prefab:050b81b9d5ac7d54882021ac63d8d3c5");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission3_Intro01_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission3_Intro01_01.prefab:f041d10dbcffa9c41bc6fbc17273f43b");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission3_Intro02_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission3_Intro02_01.prefab:ad099ea97d81efd43b82ac57eafcb4b2");
  private static readonly AssetReference VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_ExchangeD_01_01 = new AssetReference("VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_ExchangeD_01_01.prefab:4fa0a3331b13c5a4896a15672f54aee5");
  private static readonly AssetReference VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Turn1_01_01 = new AssetReference("VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Turn1_01_01.prefab:bdb4321f03e9d444b9a97f2c31a4e47e");
  private static readonly AssetReference VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Turn2_02_01 = new AssetReference("VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Turn2_02_01.prefab:75c105dbc65af55428b800aaf3a4db9b");
  private static readonly AssetReference VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Victory_02_01 = new AssetReference("VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Victory_02_01.prefab:4641eccade5c2074fb147680a96912d0");
  private static readonly AssetReference VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission4_Intro_01_01 = new AssetReference("VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission4_Intro_01_01.prefab:edf38deebdf0f7242b98f20339352507");
  private static readonly AssetReference VO_TB_Hero_Malfurion_Male_NightElf_Prologue_PreMission4_Intro_02_01 = new AssetReference("VO_TB_Hero_Malfurion_Male_NightElf_Prologue_PreMission4_Intro_02_01.prefab:31c20bb2df92f2c4aba657f6b5cccf6c");
  public static readonly AssetReference MalfurionBrassRing = new AssetReference("YoungMalfurion_Popup_BrassRing.prefab:5544ac85196277542a7fa0b1a9b578df");
  private List<string> m_VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_Lines = new List<string>()
  {
    (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_01_01,
    (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_02_01,
    (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_03_01,
    (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_Loss_01
  };
  private List<string> m_VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_Lines = new List<string>()
  {
    (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_01_01,
    (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_02_01,
    (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_03_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Death_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_EmoteResponse_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_01_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_02_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_03_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_01_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_02_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_03_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeA_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeB_01_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeC_01_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_Intro01_01,
      (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_Loss_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeB_02_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeC_02_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeD_02_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Intro03_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Turn1_02_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Turn2_01_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Victory_01_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission3_Intro01_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission3_Intro02_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_ExchangeD_01_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Turn1_01_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Turn2_02_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Victory_02_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission4_Intro_01_01,
      (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_PreMission4_Intro_02_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override List<string> GetIdleLines() => this.m_VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Idle_Lines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_HeroPower_Lines;

  protected override void OnBossHeroPowerPlayed(Entity entity)
  {
    float bossHeroPowerVoLine = this.ChanceToPlayBossHeroPowerVOLine();
    float num = Random.Range(0.0f, 1f);
    if (this.m_enemySpeaking || (double) bossHeroPowerVoLine < (double) num)
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if ((Object) actor == (Object) null)
      return;
    List<string> powerRandomLines = this.GetBossHeroPowerRandomLines();
    string soundPath = "";
    while (powerRandomLines.Count > 0)
    {
      int index = Random.Range(0, powerRandomLines.Count);
      soundPath = powerRandomLines[index];
      powerRandomLines.RemoveAt(index);
      if (!NotificationManager.Get().HasSoundPlayedThisSession(soundPath))
        break;
    }
    if (soundPath == "")
      return;
    if (soundPath == (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_Loss_01)
      this.PlaySound(soundPath);
    else
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeechOnce(soundPath, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_Death_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mannoroth_EmoteResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Prologue_Fight_03 btaPrologueFight03 = this;
    while (btaPrologueFight03.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) btaPrologueFight03.PlayLineAlways(friendlyActor, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Victory_01_01);
        yield return (object) btaPrologueFight03.PlayLineAlways((string) BTA_Prologue_Fight_03.MalfurionBrassRing, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Victory_02_01);
        yield return (object) btaPrologueFight03.PlayLineAlways(friendlyActor, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_PreMission4_Intro_01_01);
        yield return (object) btaPrologueFight03.PlayLineAlways((string) BTA_Prologue_Fight_03.MalfurionBrassRing, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_PreMission4_Intro_02_01);
        GameState.Get().SetBusy(false);
        break;
      case 601:
        GameState.Get().SetBusy(true);
        yield return (object) btaPrologueFight03.PlayLineAlways((string) BTA_Prologue_Fight_03.MalfurionBrassRing, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Turn1_01_01);
        yield return (object) btaPrologueFight03.PlayLineAlways(friendlyActor, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Turn1_02_01);
        GameState.Get().SetBusy(false);
        break;
      case 602:
        GameState.Get().SetBusy(true);
        yield return (object) btaPrologueFight03.PlayLineAlways(friendlyActor, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_Turn2_01_01);
        yield return (object) btaPrologueFight03.PlayLineAlways((string) BTA_Prologue_Fight_03.MalfurionBrassRing, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_Turn2_02_01);
        GameState.Get().SetBusy(false);
        break;
      case 603:
        GameState.Get().SetBusy(true);
        yield return (object) btaPrologueFight03.PlayLineAlways((string) BTA_Prologue_Fight_03.MalfurionBrassRing, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Malfurion_Male_NightElf_Prologue_Mission3_ExchangeD_01_01);
        yield return (object) btaPrologueFight03.PlayLineAlways(friendlyActor, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeD_02_01);
        GameState.Get().SetBusy(false);
        break;
      case 605:
        GameState.Get().SetBusy(true);
        yield return (object) btaPrologueFight03.PlayLineAlways(actor, (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeC_01_01);
        yield return (object) btaPrologueFight03.PlayLineAlways(friendlyActor, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeC_02_01);
        GameState.Get().SetBusy(false);
        break;
      case 606:
        GameState.Get().SetBusy(true);
        yield return (object) btaPrologueFight03.PlayLineAlways(actor, (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeB_01_01);
        yield return (object) btaPrologueFight03.PlayLineAlways(friendlyActor, (string) BTA_Prologue_Fight_03.VO_TB_Hero_Illidan2_Male_NightElf_Prologue_Mission3_ExchangeB_02_01);
        GameState.Get().SetBusy(false);
        break;
      case 607:
        GameState.Get().SetBusy(true);
        yield return (object) btaPrologueFight03.PlayLineAlways(actor, (string) BTA_Prologue_Fight_03.VO_Prologue_Mannoroth_Male_Demon_Prologue_Mission3_ExchangeA_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaPrologueFight03.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Prologue_Fight_03 btaPrologueFight03 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaPrologueFight03.\u003C\u003En__1(entity);
    while (btaPrologueFight03.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaPrologueFight03.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaPrologueFight03.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaPrologueFight03.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Prologue_Fight_03 btaPrologueFight03 = this;
    while (btaPrologueFight03.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaPrologueFight03.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaPrologueFight03.\u003C\u003En__2(entity);
      yield return (object) btaPrologueFight03.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaPrologueFight03.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Prologue_Fight_03 btaPrologueFight03 = this;
    while (btaPrologueFight03.m_enemySpeaking)
      yield return (object) null;
  }
}
