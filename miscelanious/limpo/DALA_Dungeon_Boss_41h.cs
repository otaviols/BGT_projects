using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_41h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_BossKillCommand_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_BossKillCommand_01.prefab:f4bdc2ab157fd73458d08057e8bfb920");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_BossUnleashtheHounds_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_BossUnleashtheHounds_01.prefab:2ebb5814bf5f1ff4c9b0bf1567367f44");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_Death_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_Death_01.prefab:e5e7424969182cb45818ff64e4c328b3");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_DefeatPlayer_01.prefab:953fdc9700382ac439ad4dbb5b2e6c34");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_EmoteResponse_01.prefab:5877e5a049f4d664786defe3694ae21a");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_01.prefab:ed45eae2d303bd14f857b02a5ff232cf");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_02 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_02.prefab:6a8badd8657aeb14a9c4aa6e6364f9fb");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_04 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_04.prefab:bba06439cb597654aa0aaa36258ef7b9");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_05 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_05.prefab:b65ce7482ef35c64b80790840910dd8f");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_Idle_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_Idle_01.prefab:4c2744462214cec419ab6a48fde7923e");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_Idle_02 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_Idle_02.prefab:af4832b2d4ac2cf4aab5f6bca8645075");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_Idle_03 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_Idle_03.prefab:2a4b262db6a8a4c4e893c11866acbd87");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_Intro_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_Intro_01.prefab:d819c9261cf3869408d3a7e278d7579e");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_IntroChu_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_IntroChu_01.prefab:1714e092db43fd2489d8b062af1f8558");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_IntroGeorge_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_IntroGeorge_01.prefab:f240a34e273e4674aaa1f8552db887db");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_IntroKriziki_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_IntroKriziki_01.prefab:a48abc5898681454780fa2a3dcf1b0cd");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_IntroMongrel_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_IntroMongrel_01.prefab:8245fc02da8852945bc65bed142b3bad");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_IntroRakanishu_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_IntroRakanishu_01.prefab:839de3fdc28ffec448f64173a0348cbc");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_IntroTekahn_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_IntroTekahn_01.prefab:849f53e65a54d40499e26d92ae40435e");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_IntroVessina_02 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_IntroVessina_02.prefab:378a6799d75468e48b3800debfb66bf3");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_Misc_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_Misc_01.prefab:6498676d3c093b34c93c2e603fda30fe");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_PlayerBeast_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_PlayerBeast_01.prefab:80a3a5dc7cad2ea438c9bded691f4f03");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_PlayerBigGameHunter_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_PlayerBigGameHunter_01.prefab:02f33ec39a1ee494b871d26e2af30e1c");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_PlayerHitsFirst_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_PlayerHitsFirst_01.prefab:72c8e6c081de5ff449d742fa7cfe474a");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_PlayerHunterWeapon_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_PlayerHunterWeapon_01.prefab:cbb07827eacd19840b279ed47b1f6580");
  private static readonly AssetReference VO_DALA_BOSS_41h_Male_Dwarf_PlayerHyperBlaster_01 = new AssetReference("VO_DALA_BOSS_41h_Male_Dwarf_PlayerHyperBlaster_01.prefab:7c15a2a15fbe25d4eb184dd39e18319b");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Idle_01,
    (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Idle_02,
    (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_BossKillCommand_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_BossUnleashtheHounds_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Death_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_02,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_04,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_05,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Idle_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Idle_02,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Idle_03,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Intro_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroChu_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroGeorge_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroKriziki_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroMongrel_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroRakanishu_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroTekahn_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroVessina_02,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Misc_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerBeast_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerBigGameHunter_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerHitsFirst_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerHunterWeapon_01,
      (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerHyperBlaster_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_41h.m_IdleLines;

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_01,
    (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_02,
    (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_04,
    (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_HeroPower_05
  };

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_Chu")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroChu_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "DALA_George")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroGeorge_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "DALA_Barkeye")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroMongrel_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "DALA_Vessina")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_IntroVessina_02, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "DALA_Kriziki") || !(cardId != "DALA_Rakanishu") || !(cardId != "DALA_Tekahn") || !(cardId != "DALA_Eudora"))
          return;
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_41h dalaDungeonBoss41h = this;
    while (dalaDungeonBoss41h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerBeast_01);
        break;
      case 102:
        yield return (object) dalaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerHitsFirst_01);
        break;
      case 103:
        yield return (object) dalaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerHunterWeapon_01);
        break;
      case 104:
        yield return (object) dalaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_Misc_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss41h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_41h dalaDungeonBoss41h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss41h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss41h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss41h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss41h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss41h.m_playedLines.Add(cardId);
      if (!(cardId == "EX1_005"))
      {
        if (cardId == "DALA_723")
          yield return (object) dalaDungeonBoss41h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerHyperBlaster_01);
      }
      else
        yield return (object) dalaDungeonBoss41h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_PlayerBigGameHunter_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_41h dalaDungeonBoss41h = this;
    while (dalaDungeonBoss41h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss41h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss41h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss41h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss41h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_539"))
      {
        if (cardId == "EX1_538")
          yield return (object) dalaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_BossUnleashtheHounds_01);
      }
      else
        yield return (object) dalaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_41h.VO_DALA_BOSS_41h_Male_Dwarf_BossKillCommand_01);
    }
  }
}
