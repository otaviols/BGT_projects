using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_12 : BTA_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BTA_Fight_12.InitBooleanOptions();
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_12_PlayerStart_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_12_PlayerStart_01.prefab:6207b28dfe2d2dd489bddb216c9598bc");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_12_VictoryA_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_12_VictoryA_01.prefab:d5c8917a4cb3401499ecd86c42801133");
  private static readonly AssetReference VO_BTA_08_Male_Orc_Mission_Fight_12_MiscA_01 = new AssetReference("VO_BTA_08_Male_Orc_Mission_Fight_12_MiscA_01.prefab:9ee0d92e24cef7840b5bf44e3dd19b09");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Attack_01.prefab:dbd37b82d84594e499da15674822603e");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_BloodWarriors_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_BloodWarriors_01.prefab:7c97c474478831c479dafff631800ca3");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Crush_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Crush_01.prefab:8679ea5398ff9bd4099f5a620571ea31");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_EndlessLegion_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_EndlessLegion_01.prefab:2961d0b9b354e684a9f522f100b12494");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Soulfire_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Soulfire_01.prefab:aa316ccb5f534e546b70dd1124c96ed4");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossDeathA_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossDeathA_01.prefab:2de2f95d4acf4f94d91022f3c956bd40");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossDeathB_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossDeathB_01.prefab:567321497cafb6b4ba8cbba14d7ca737");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossStart_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossStart_01.prefab:bb154eb15195c304db7ce2c69f43c2ac");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Emote_Response_01.prefab:0b372717b0e66d64f9015a3e6ddd4cab");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_ChaosNova_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_ChaosNova_01.prefab:59c3aa5627b9e804ca17b67f4c4c5cba");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_EyeBeam_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_EyeBeam_01.prefab:98d6cdd652a551b4f9d2a854c790630d");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_Magtheridon_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_Magtheridon_01.prefab:f62ffa26019811a4eaa176e993130198");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_01.prefab:4e0d3012425089f49b88e6fce4eaa82d");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_02 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_02.prefab:2003ba4e664e63549983cfc1d8bb7936");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_03 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_03.prefab:bf3ecd2fbc8e26840a45a12a378e4d9c");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_04 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_04.prefab:346f319a2dd82e54a88897feee4ad529");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleA_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleA_01.prefab:8012dbfe8fce27947a8cfe559c91f909");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleB_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleB_01.prefab:b969da525c961e943be2fd9e0b2f7ca9");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleC_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleC_01.prefab:4be27b6c5038b534f955d6c048843293");
  private static readonly AssetReference VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_MiscB_01 = new AssetReference("VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_MiscB_01.prefab:58583e48a54a77346bb7115ba47638b0");
  private List<string> m_VO_BTA_BOSS_12h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleA_01,
    (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleB_01,
    (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleC_01
  };
  private List<string> m_VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_Lines = new List<string>()
  {
    (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_01,
    (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_02,
    (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_03,
    (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_04
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BTA_Fight_12() => this.m_gameOptions.AddBooleanOptions(BTA_Fight_12.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_12.VO_BTA_01_Female_NightElf_Mission_Fight_12_PlayerStart_01,
      (string) BTA_Fight_12.VO_BTA_01_Female_NightElf_Mission_Fight_12_VictoryA_01,
      (string) BTA_Fight_12.VO_BTA_08_Male_Orc_Mission_Fight_12_MiscA_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Attack_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_BloodWarriors_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Crush_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_EndlessLegion_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Soulfire_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossDeathA_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossDeathB_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossStart_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Emote_Response_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_ChaosNova_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_EyeBeam_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_Magtheridon_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_02,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_03,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_04,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleA_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleB_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_IdleC_01,
      (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_MiscB_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_12h_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossDeathB_01;
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BTA_Fight_12 btaFight12 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) btaFight12.PlayLineAlways(actor, (string) BTA_Fight_12.VO_BTA_01_Female_NightElf_Mission_Fight_12_PlayerStart_01);
    yield return (object) btaFight12.PlayLineAlways(enemyActor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_BossStart_01);
    GameState.Get().SetBusy(false);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Emote_Response_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Fight_12 btaFight12 = this;
    while (btaFight12.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 500:
        btaFight12.PlaySound((string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Attack_01);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) btaFight12.PlayLineAlways(actor2, (string) BTA_Fight_12.VO_BTA_01_Female_NightElf_Mission_Fight_12_VictoryA_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        yield return (object) btaFight12.PlayAndRemoveRandomLineOnlyOnce(actor1, btaFight12.m_VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_HeroPowerTrigger_Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight12.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_12 btaFight12 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight12.\u003C\u003En__1(entity);
    while (btaFight12.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight12.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight12.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight12.m_playedLines.Add(cardId);
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BT_235"))
      {
        if (!(cardId == "BT_801"))
        {
          if (!(cardId == "BT_850"))
          {
            if (cardId == "BTA_08")
            {
              yield return (object) btaFight12.PlayLineAlwaysWithBrassRing(btaFight12.GetFriendlyActorByCardId("BTA_08"), BTA_Dungeon.KarnukBrassRingDemonHunter, (string) BTA_Fight_12.VO_BTA_08_Male_Orc_Mission_Fight_12_MiscA_01);
              yield return (object) btaFight12.PlayLineAlways(enemyActor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_MiscB_01);
            }
          }
          else
            yield return (object) btaFight12.PlayLineAlways(enemyActor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_Magtheridon_01);
        }
        else
          yield return (object) btaFight12.PlayLineAlways(enemyActor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_EyeBeam_01);
      }
      else
        yield return (object) btaFight12.PlayLineAlways(enemyActor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Hero_ChaosNova_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_12 btaFight12 = this;
    while (btaFight12.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight12.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight12.\u003C\u003En__2(entity);
      yield return (object) btaFight12.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight12.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BTA_15"))
      {
        if (!(cardId == "EX1_308"))
        {
          if (!(cardId == "GVG_052"))
          {
            if (cardId == "OG_276")
              yield return (object) btaFight12.PlayLineAlways(actor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_BloodWarriors_01);
          }
          else
            yield return (object) btaFight12.PlayLineAlways(actor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Crush_01);
        }
        else
          yield return (object) btaFight12.PlayLineAlways(actor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_Soulfire_01);
      }
      else
        yield return (object) btaFight12.PlayLineAlways(actor, (string) BTA_Fight_12.VO_BTA_BOSS_12h_Male_Demon_Mission_Fight_12_Boss_EndlessLegion_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_12 btaFight12 = this;
    while (btaFight12.m_enemySpeaking)
      yield return (object) null;
  }
}
