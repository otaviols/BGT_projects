using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_11 : BTA_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BTA_Fight_11.InitBooleanOptions();
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_04B_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_04B_01.prefab:571fe638b871c094a84d087df3b0616f");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_05A_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_05A_01.prefab:f05bc37056f694744bbfd2a6e9ed0107");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_06A_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_06A_01.prefab:9057d1ffe2dcffa4590cded80a1a8196");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_11_PlayerStart_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_11_PlayerStart_01.prefab:97096cf61f5264e42a214b2d6f64959e");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_11_VictoryA_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_11_VictoryA_01.prefab:d3b69ef6c17b61743866e563e62596c2");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_AmbushTrigger_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_AmbushTrigger_01.prefab:5b7de994202af1448aa9decedeb90799");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_Attack_01.prefab:c24f1a153b2184540a57d92ab853af5b");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_Penance_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_Penance_01.prefab:0f5ecc3ebd6e68442b87ebd1ff697ba9");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_RustswornCultist_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_RustswornCultist_01.prefab:e36b7ad491a62c645adef3aafd3433a3");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_BossDeath_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_BossDeath_01.prefab:39b22c8b10b36d44f976a827d8a47b59");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_BossStart_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_BossStart_01.prefab:c62508be2248a834aaf8c57fafc68623");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Emote_Response_01.prefab:8df04a710d3a8f349b6ffe348b7a4606");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Hero_CommandtheIllidari_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Hero_CommandtheIllidari_01.prefab:8b948e5abf2b2fa47b25f452a851ee19");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Hero_UrzulHorror_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Hero_UrzulHorror_01.prefab:5acb38a9e6e22b541a11678f40d8a09a");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_01.prefab:bf9f5a59be91f0642b76116021045f5b");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_02 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_02.prefab:668490af1baa90e4eaa65be304c4e17e");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_03 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_03.prefab:bb582781ee6b72142ad06a6cb08784a7");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_04 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_04.prefab:3b5daacb133e2a74d8b5258b60c9bf08");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleA_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleA_01.prefab:111d31e0c9f912e469388935d361ed84");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleB_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleB_01.prefab:5bf6c40e21857d147939033f6a467bf3");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleC_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleC_01.prefab:58a585790e6c16844baa37ecd986ade8");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Misc_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Misc_01.prefab:b89ba069b9017364c880ee8df315d254");
  private static readonly AssetReference VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_RustedLegionGanarg_01 = new AssetReference("VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_RustedLegionGanarg_01.prefab:49d6fd66d60aabb4fa5fa50a8c0c7ffc");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_04A_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_04A_01.prefab:e53b8d31a63d53e49a4f8010817cc667");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_04C_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_04C_01.prefab:36a44b8818a5d164b982ab1e349ded91");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_05B_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_05B_01.prefab:7b232f545b1d4ea46a615f3a4ef539da");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_05C_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_05C_01.prefab:ad777afec9f9c4d41b935f92cd7fb197");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_06B_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_06B_01.prefab:2c051bd8f3e3854488d0e9ce5e560c9c");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_VictoryB_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_VictoryB_01.prefab:b1a421d657ab60b438e76b459a08d19e");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_UI_Mission_Fight_11_Turn_1_Flavor_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_UI_Mission_Fight_11_Turn_1_Flavor_01.prefab:12e7a0d0526302c4a9fd1a2dca6760dd");
  private List<string> m_VO_BTA_BOSS_11h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleA_01,
    (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleB_01,
    (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleC_01
  };
  private List<string> m_missionEventTrigger105_Lines = new List<string>()
  {
    (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_01,
    (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_02,
    (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_03,
    (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_04
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BTA_Fight_11() => this.m_gameOptions.AddBooleanOptions(BTA_Fight_11.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_04B_01,
      (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_05A_01,
      (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_06A_01,
      (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_PlayerStart_01,
      (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_VictoryA_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_AmbushTrigger_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_Attack_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_Penance_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_RustswornCultist_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_BossDeath_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_BossStart_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Emote_Response_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Hero_CommandtheIllidari_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Hero_UrzulHorror_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_02,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_03,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_HeroPowerTrigger_04,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleA_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleB_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_IdleC_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Misc_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_RustedLegionGanarg_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_04A_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_04C_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_05B_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_05C_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_06B_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_VictoryB_01,
      (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_UI_Mission_Fight_11_Turn_1_Flavor_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_11h_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_BossDeath_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BTA_Fight_11 btaFight11 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) btaFight11.PlayLineAlways(actor, (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_PlayerStart_01);
    yield return (object) btaFight11.PlayLineAlways(enemyActor, (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_BossStart_01);
    GameState.Get().SetBusy(false);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Emote_Response_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Fight_11 btaFight11 = this;
    while (btaFight11.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 100:
        yield return (object) btaFight11.PlayLineAlways(actor1, (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_AmbushTrigger_01);
        break;
      case 500:
        btaFight11.PlaySound((string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_Attack_01);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) btaFight11.PlayLineAlways(actor2, (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_VictoryA_01);
        yield return (object) btaFight11.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_VictoryB_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        yield return (object) btaFight11.PlayRandomLineAlways(actor1, btaFight11.m_missionEventTrigger105_Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight11.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_11 btaFight11 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight11.\u003C\u003En__1(entity);
    while (btaFight11.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight11.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight11.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight11.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BT_173"))
      {
        if (!(cardId == "BT_407"))
        {
          if (cardId == "BTA_06")
            yield return (object) btaFight11.PlayLineAlways(actor, (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Misc_01);
        }
        else
          yield return (object) btaFight11.PlayLineAlways(actor, (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Hero_UrzulHorror_01);
      }
      else
        yield return (object) btaFight11.PlayLineAlways(actor, (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Hero_CommandtheIllidari_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_11 btaFight11 = this;
    while (btaFight11.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight11.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight11.\u003C\u003En__2(entity);
      yield return (object) btaFight11.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight11.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BT_160"))
      {
        if (!(cardId == "BTA_12"))
        {
          if (cardId == "CFM_781")
            yield return (object) btaFight11.PlayLineAlways(actor, (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_Penance_01);
        }
        else
          yield return (object) btaFight11.PlayLineAlways(actor, (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_RustedLegionGanarg_01);
      }
      else
        yield return (object) btaFight11.PlayLineAlways(actor, (string) BTA_Fight_11.VO_BTA_BOSS_11h_Female_Arakkoa_Mission_Fight_11_Boss_RustswornCultist_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_11 btaFight11 = this;
    while (btaFight11.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) btaFight11.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_UI_Mission_Fight_11_Turn_1_Flavor_01);
        break;
      case 3:
        yield return (object) btaFight11.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_04A_01);
        yield return (object) btaFight11.PlayLineAlways(friendlyActor, (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_04B_01);
        yield return (object) btaFight11.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_04C_01);
        break;
      case 7:
        yield return (object) btaFight11.PlayLineAlways(friendlyActor, (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_05A_01);
        yield return (object) btaFight11.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_05B_01);
        yield return (object) btaFight11.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_05C_01);
        break;
      case 11:
        yield return (object) btaFight11.PlayLineAlways(friendlyActor, (string) BTA_Fight_11.VO_BTA_01_Female_NightElf_Mission_Fight_11_Bonding_06A_01);
        yield return (object) btaFight11.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_11.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_11_Bonding_06B_01);
        break;
    }
  }
}
