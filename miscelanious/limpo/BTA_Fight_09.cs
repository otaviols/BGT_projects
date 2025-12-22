using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_09 : BTA_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BTA_Fight_09.InitBooleanOptions();
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_09_Hero_HeroPower_02 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_09_Hero_HeroPower_02.prefab:c82d3b7dbe014c645be61ffcd2c1d3a1");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_09_PlayerStart_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_09_PlayerStart_01.prefab:1c96ac5f4a472254d89c5f5458c156d2");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_09_VictoryB_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_09_VictoryB_01.prefab:256ee2d4247107c49ac0ad8ea19433ec");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_09_VictoryC_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_09_VictoryC_01.prefab:5cafe1b87d62a0347911a17d8f95630b");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_Attack_01.prefab:dc3d7c77c3b005841ad0df25740b93e3");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_ConchguardWarlord_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_ConchguardWarlord_01.prefab:3267260319c4caf4d9cb241acf9a1de9");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_SerpentShrinePortal_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_SerpentShrinePortal_01.prefab:7dd9fae21b7c68146843a3ec35063bf6");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_Torrent_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_Torrent_01.prefab:c7a9c52a826a3534093d2d9503e88580");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_WrathscaleNaga_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_WrathscaleNaga_01.prefab:4d5ed87abba2dde41a7e0dec2cae7861");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_BossDeath_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_BossDeath_01.prefab:3f5731086b9af044797663a86ff45608");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_BossStart_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_BossStart_01.prefab:2bd3c4cbead065444ba9bee77bc4897a");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Emote_Response_01.prefab:0a1297a92926faf4b8be06fdb1548cfc");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_Blur_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_Blur_01.prefab:8e6a29c38451174448e3813ca771c499");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_CoordinatedStrike_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_CoordinatedStrike_01.prefab:b59d0cf2e7fda1a499f5e0022acb6823");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_TwinSlice_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_TwinSlice_01.prefab:6c11f1ac9f43e544eb9935ec9ce46de3");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_01.prefab:ba5088602273fc645813ee38a14e0640");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_02 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_02.prefab:3a8618191e6cae14b89343d8ea437cd3");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_03 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_03.prefab:11757f385d542bb4a85a38a9d42b3f83");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_04 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_04.prefab:acc821a837c199848b732cc6c5a436d5");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleA_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleA_01.prefab:d70407fa52d318b458c1b8c5ba323b31");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleB_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleB_01.prefab:d256b1b5035d76848a3ecee818b7a3db");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleC_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleC_01.prefab:c4ac31fbad6745848a793343717d030b");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_VictoryA_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_VictoryA_01.prefab:9d9d95cee3244d0489683f2c567eba95");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_A_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_A_01.prefab:da13367489516a84f975bcf2a83ec610");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_B_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_B_01.prefab:0618987063f9b19498f892ea34e6431c");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_C_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_C_01.prefab:2a533cc710278b94895fb3ebec0d619c");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_10_Bonding_01B_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_10_Bonding_01B_01.prefab:92fbd307bb421b242b08ea8bf9f282f2");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_10_Bonding_01A_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_10_Bonding_01A_01.prefab:a79ed8c6862640c49ab65db8455e3dbb");
  private List<string> m_VO_BTA_BOSS_09h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleA_01,
    (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleB_01,
    (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleC_01
  };
  private List<string> m_missionEventTrigger101_Lines = new List<string>()
  {
    (string) BTA_Fight_09.VO_BTA_01_Female_NightElf_Mission_Fight_09_Hero_HeroPower_02
  };
  private List<string> m_VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_Lines = new List<string>()
  {
    (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_01,
    (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_02,
    (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_03,
    (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_04
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BTA_Fight_09() => this.m_gameOptions.AddBooleanOptions(BTA_Fight_09.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_09.VO_BTA_01_Female_NightElf_Mission_Fight_09_Hero_HeroPower_02,
      (string) BTA_Fight_09.VO_BTA_01_Female_NightElf_Mission_Fight_09_PlayerStart_01,
      (string) BTA_Fight_09.VO_BTA_01_Female_NightElf_Mission_Fight_09_VictoryB_01,
      (string) BTA_Fight_09.VO_BTA_01_Female_NightElf_Mission_Fight_09_VictoryC_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_Attack_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_ConchguardWarlord_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_SerpentShrinePortal_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_Torrent_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_WrathscaleNaga_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_BossDeath_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_BossStart_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Emote_Response_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_Blur_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_CoordinatedStrike_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_TwinSlice_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_02,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_03,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_04,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleA_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleB_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_IdleC_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_VictoryA_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_A_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_B_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_C_01,
      (string) BTA_Fight_09.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_10_Bonding_01A_01,
      (string) BTA_Fight_09.VO_BTA_01_Female_NightElf_Mission_Fight_10_Bonding_01B_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_09h_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_Lines;

  public override void OnCreateGame() => base.OnCreateGame();

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BTA_Fight_09 btaFight09 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) btaFight09.PlayLineAlways(actor, (string) BTA_Fight_09.VO_BTA_01_Female_NightElf_Mission_Fight_09_PlayerStart_01);
    yield return (object) btaFight09.PlayLineAlways(enemyActor, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_BossStart_01);
    GameState.Get().SetBusy(false);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Emote_Response_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Fight_09 btaFight09 = this;
    while (btaFight09.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 100:
        GameState.Get().SetBusy(true);
        yield return (object) btaFight09.PlayLineAlways(actor1, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_VictoryA_01);
        GameState.Get().SetBusy(false);
        break;
      case 101:
        yield return (object) btaFight09.PlayLineAlways(actor1, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_ConchguardWarlord_01);
        break;
      case 500:
        btaFight09.PlaySound((string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_Attack_01);
        break;
      case 501:
        btaFight09.m_DisableIdle = true;
        break;
      case 505:
        yield return (object) btaFight09.PlayRandomLineAlways(actor2, btaFight09.m_missionEventTrigger101_Lines);
        break;
      case 507:
        yield return (object) btaFight09.PlayRandomLineAlways(actor1, btaFight09.m_VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_HeroPower_Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight09.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_09 btaFight09 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight09.\u003C\u003En__1(entity);
    while (btaFight09.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight09.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight09.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight09.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BT_036"))
      {
        if (!(cardId == "BT_175"))
        {
          if (cardId == "BT_752")
            yield return (object) btaFight09.PlayLineAlways(actor, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_Blur_01);
        }
        else
          yield return (object) btaFight09.PlayLineAlways(actor, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_TwinSlice_01);
      }
      else
        yield return (object) btaFight09.PlayLineAlways(actor, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Hero_CoordinatedStrike_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_09 btaFight09 = this;
    while (btaFight09.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight09.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight09.\u003C\u003En__2(entity);
      yield return (object) btaFight09.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight09.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BT_100"))
      {
        if (!(cardId == "BT_110"))
        {
          if (!(cardId == "BT_355"))
          {
            if (cardId == "BT_761t")
              yield return (object) btaFight09.PlayLineAlways(actor, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_ConchguardWarlord_01);
          }
          else
            yield return (object) btaFight09.PlayLineAlways(actor, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_WrathscaleNaga_01);
        }
        else
          yield return (object) btaFight09.PlayLineAlways(actor, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_Torrent_01);
      }
      else
        yield return (object) btaFight09.PlayLineAlways(actor, (string) BTA_Fight_09.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_09_Boss_SerpentShrinePortal_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_09 btaFight09 = this;
    while (btaFight09.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) btaFight09.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_09.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_10_Bonding_01A_01);
        yield return (object) btaFight09.PlayLineAlways(friendlyActor, (string) BTA_Fight_09.VO_BTA_01_Female_NightElf_Mission_Fight_10_Bonding_01B_01);
        break;
      case 5:
        yield return (object) btaFight09.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_09.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_A_01);
        break;
      case 9:
        yield return (object) btaFight09.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_09.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_B_01);
        break;
      case 13:
        yield return (object) btaFight09.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_09.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_09_Illidan_Coaching_C_01);
        break;
    }
  }
}
