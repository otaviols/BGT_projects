using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_22 : BTA_Dungeon_Heroic
{
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_Attack_01.prefab:5e2ee0089e6d2da49b73f3207c236af9");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_DeathandDecay_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_DeathandDecay_01.prefab:ff2f65b86ac9ba7409a577c212efd9c5");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_DeathGrip_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_DeathGrip_01.prefab:fc70bc1102656c74b8003626be6cf1bb");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_SiphonSoul_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_SiphonSoul_01.prefab:eae6b8b3e2aab6041a96172296d697c0");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossDeathAlt_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossDeathAlt_01.prefab:f0d128f0675fe1b47b6460e96a4d682e");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossStart_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossStart_01.prefab:ce8940cb3601ab848a4e57ee44c23185");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossStartIllidan_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossStartIllidan_01.prefab:6d92097ccf49b8841b802599db8cf03a");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Emote_Response_01.prefab:24f7e4daae17ce349ac74edf6857220d");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Hero_DeathKnightHero_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Hero_DeathKnightHero_01.prefab:6a5f4f9cbae1f084089954a22ebbbaec");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Hero_LichKing_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Hero_LichKing_01.prefab:8977f253905fd80479ddca63cd047ff4");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_01.prefab:a9395cd9495bf9444ba1875c33ba1d9f");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_02 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_02.prefab:0d8d0d883a997ca4bba2b3eb6e7672c0");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_03 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_03.prefab:531f90851e82b15449e6b3a94fdcdcb2");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_04 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_04.prefab:3931c237faf965f458570832c5a93c14");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleA_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleA_01.prefab:69a93c8681b5a0044ae3cdfeb65322c3");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleB_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleB_01.prefab:b7834b0c5235adf4f9b97e4259dc400c");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleC_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleC_01.prefab:0f46df05c85bf7a43a346b3e85a5fd50");
  private static readonly AssetReference VO_BTA_BOSS_22h_Male_Human_UI_Mission_Fight_22_CoinSelect_01 = new AssetReference("VO_BTA_BOSS_22h_Male_Human_UI_Mission_Fight_22_CoinSelect_01.prefab:d67906a7a549b754e99c79a963f502e1");
  private List<string> m_missionEventTrigger507Lines = new List<string>()
  {
    (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_01,
    (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_02,
    (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_03,
    (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_04
  };
  private List<string> m_VO_BTA_BOSS_22h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleA_01,
    (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleB_01,
    (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleC_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_Attack_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_DeathandDecay_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_DeathGrip_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_SiphonSoul_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossDeathAlt_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossStart_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossStartIllidan_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Emote_Response_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Hero_DeathKnightHero_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Hero_LichKing_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_02,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_03,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_HeroPower_04,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleA_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleB_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_IdleC_01,
      (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_UI_Mission_Fight_22_CoinSelect_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_22h_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossDeathAlt_01;
    this.m_standardEmoteResponseLine = (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Emote_Response_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "HERO_10" || cardId == "HERO_10a")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossStartIllidan_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_BossStart_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    BTA_Fight_22 btaFight22 = this;
    while (btaFight22.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 500:
        btaFight22.PlaySound((string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_Attack_01);
        break;
      case 507:
        yield return (object) btaFight22.PlayAndRemoveRandomLineOnlyOnce(actor, btaFight22.m_missionEventTrigger507Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight22.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_22 btaFight22 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight22.\u003C\u003En__1(entity);
    while (btaFight22.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight22.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight22.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight22.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "ICC_314":
          yield return (object) btaFight22.PlayLineOnlyOnce(actor, (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Hero_LichKing_01);
          break;
        case "ICC_481":
        case "ICC_827":
        case "ICC_828":
        case "ICC_829":
        case "ICC_830":
        case "ICC_831":
        case "ICC_832":
        case "ICC_833":
        case "ICC_834":
          yield return (object) btaFight22.PlayLineOnlyOnce(actor, (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Hero_DeathKnightHero_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_22 btaFight22 = this;
    while (btaFight22.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight22.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight22.\u003C\u003En__2(entity);
      yield return (object) btaFight22.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight22.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ICC_314t4"))
      {
        if (!(cardId == "ICC_314t5"))
        {
          if (cardId == "ICC_314t8")
            yield return (object) btaFight22.PlayLineOnlyOnce(actor, (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_DeathandDecay_01);
        }
        else
          yield return (object) btaFight22.PlayLineOnlyOnce(actor, (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_SiphonSoul_01);
      }
      else
        yield return (object) btaFight22.PlayLineOnlyOnce(actor, (string) BTA_Fight_22.VO_BTA_BOSS_22h_Male_Human_Mission_Fight_22_Boss_DeathGrip_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_22 btaFight22 = this;
    while (btaFight22.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }
}
