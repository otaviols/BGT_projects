using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_26 : BTA_Dungeon_Heroic
{
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_Attack_01.prefab:ee76810f02a0ba748babf88b8d13ed4d");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_ChaosNova_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_ChaosNova_01.prefab:65614c4fdbebe8f4c99d9135a1e2b4b2");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_InnerDemon_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_InnerDemon_01.prefab:ba9911180c4c0324ebac3644c628b18a");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_Metamorphosis_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_Metamorphosis_01.prefab:8936b149faf735046b09a1f29db80a7e");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_SkullofGuldan_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_SkullofGuldan_01.prefab:82e1af5cdc70ad947b6f58d42ccace02");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_SpectralSight_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_SpectralSight_01.prefab:002f5db33f9866a4ab967f1a098c0036");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_YouAreNotPrepared_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_YouAreNotPrepared_01.prefab:c9dbf36fb40b1b64ab7780969fa65c26");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossDeath_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossDeath_01.prefab:d4ecd250d23994945b4ad186f38c8ce3");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStart_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStart_01.prefab:2b0459206e474d546b90d55bf9b3c317");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStartArthas_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStartArthas_01.prefab:d024bda905fefd64da7315a7a17f1f35");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStartMaiev_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStartMaiev_01.prefab:0a16efcd2ae2d814ca96be22ef01e5ed");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Emote_Response_01.prefab:4e5b5dd8953d3914ea93b895e5d40635");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Hero_Akama_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Hero_Akama_01.prefab:50819e3f29e238249a66796fc88e902b");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_01.prefab:b8f3cf6c5f066eb49990304505e4f72c");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_02 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_02.prefab:c7308a14d565c834fb1aadf41fe530eb");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_03 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_03.prefab:3f4a7a416d8da494f9cfc59fe244c397");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_04 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_04.prefab:bbdf279a156c844499370d4ae01cfa71");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleA_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleA_01.prefab:be61259e181b6dc41930db91d10c652d");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleB_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleB_01.prefab:f1236a66eda34ca4c801823ffaa08dfd");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleC_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleC_01.prefab:d9754730b5485f54c96e772d1e472982");
  private static readonly AssetReference VO_BTA_BOSS_17hx_Male_NightElf_UI_Mission_Fight_26_CoinSelect_01 = new AssetReference("VO_BTA_BOSS_17hx_Male_NightElf_UI_Mission_Fight_26_CoinSelect_01.prefab:780367df262656b468fa8ae84e6e6ecb");
  private List<string> m_VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_InnerDemon = new List<string>()
  {
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_ChaosNova_01,
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_InnerDemon_01
  };
  private List<string> m_missionEventTrigger507Lines = new List<string>()
  {
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_01,
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_02,
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_03,
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_04
  };
  private List<string> m_VO_BTA_BOSS_26h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleA_01,
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleB_01,
    (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleC_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_Attack_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_ChaosNova_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_InnerDemon_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_Metamorphosis_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_SkullofGuldan_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_SpectralSight_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_YouAreNotPrepared_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossDeath_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStart_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStartArthas_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStartMaiev_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Emote_Response_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Hero_Akama_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_02,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_03,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_HeroPower_04,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleA_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleB_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_IdleC_01,
      (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_UI_Mission_Fight_26_CoinSelect_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_26h_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossDeath_01;
    this.m_standardEmoteResponseLine = (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Emote_Response_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "HERO_04b")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStartArthas_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "HERO_03a")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStartMaiev_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_BossStart_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    BTA_Fight_26 btaFight26 = this;
    while (btaFight26.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 161:
        yield return (object) btaFight26.PlayAndRemoveRandomLineOnlyOnce(actor, btaFight26.m_VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_InnerDemon);
        break;
      case 500:
        btaFight26.PlaySound((string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_Attack_01);
        break;
      case 507:
        yield return (object) btaFight26.PlayAndRemoveRandomLineOnlyOnce(actor, btaFight26.m_missionEventTrigger507Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight26.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_26 btaFight26 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight26.\u003C\u003En__1(entity);
    while (btaFight26.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight26.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight26.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight26.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "BT_713")
        yield return (object) btaFight26.PlayLineAlways(actor, (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Hero_Akama_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_26 btaFight26 = this;
    while (btaFight26.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight26.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight26.\u003C\u003En__2(entity);
      yield return (object) btaFight26.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight26.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BTA_BOSS_26s"))
      {
        if (!(cardId == "BT_429"))
        {
          if (!(cardId == "BT_491"))
          {
            if (cardId == "BT_601")
              yield return (object) btaFight26.PlayLineAlways(actor, (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_SkullofGuldan_01);
          }
          else
            yield return (object) btaFight26.PlayLineAlways(actor, (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_SpectralSight_01);
        }
        else
          yield return (object) btaFight26.PlayLineAlways(actor, (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_Metamorphosis_01);
      }
      else
        yield return (object) btaFight26.PlayLineOnlyOnce(actor, (string) BTA_Fight_26.VO_BTA_BOSS_17hx_Male_NightElf_Mission_Fight_26_Boss_YouAreNotPrepared_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_26 btaFight26 = this;
    while (btaFight26.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }
}
