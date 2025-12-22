using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_23 : BTA_Dungeon_Heroic
{
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Boss_Attack_01.prefab:0ab275c1a9c09324c90c9f473801367d");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossDeath_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossDeath_01.prefab:bb35237313dfd7d488cb61ca0aff4640");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossStart_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossStart_01.prefab:e7c86907f2c517a4b92c1b4dddcaff05");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossStartDemonHunter_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossStartDemonHunter_01.prefab:ce4f2428ca4b0ad45b52cd05492403bf");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Emote_Response_01.prefab:2d0d469f9e1076a41a9618ffaafd542d");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_Bladestorm_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_Bladestorm_01.prefab:7259cd5165ef06840b83cf49afa4b7b2");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_LightsChampion_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_LightsChampion_01.prefab:c14def875a76cc14faaead0534261c29");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_StolenSteel_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_StolenSteel_01.prefab:b72cd9ac8ca920745aecb99ab99ea920");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_01.prefab:757a239fd530b664f8a30160ebe84160");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_02 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_02.prefab:65e625c6021c3b040b04891fb5ad1254");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_03 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_03.prefab:63c3f3a9c20770847a240f7d0da8e467");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_04 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_04.prefab:6e7f6f9d93f52804295821f8db9c7943");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleA_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleA_01.prefab:df0639a438208ce4990b599d1c5a24de");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleB_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleB_01.prefab:caa9d5ffa48b0ff47aeb88df0b875996");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleC_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleC_01.prefab:b9c83c40dfdc537468ad758526fa05d7");
  private static readonly AssetReference VO_BTA_BOSS_08hx_Female_Demon_UI_Mission_Fight_23_CoinSelect_01 = new AssetReference("VO_BTA_BOSS_08hx_Female_Demon_UI_Mission_Fight_23_CoinSelect_01.prefab:e4e2166f87a9bd04e9eb417285950635");
  private List<string> m_VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_WeaponDestroy = new List<string>()
  {
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_LightsChampion_01,
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_StolenSteel_01
  };
  private List<string> m_missionEventTrigger507Lines = new List<string>()
  {
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_01,
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_02,
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_03,
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_04
  };
  private List<string> m_VO_BTA_BOSS_23h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleA_01,
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleB_01,
    (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleC_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Boss_Attack_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossDeath_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossStart_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossStartDemonHunter_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Emote_Response_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_Bladestorm_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_LightsChampion_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_StolenSteel_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_02,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_03,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_HeroPower_04,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleA_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleB_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_IdleC_01,
      (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_UI_Mission_Fight_23_CoinSelect_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_23h_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossDeath_01;
    this.m_standardEmoteResponseLine = (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Emote_Response_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "HERO_10" || cardId == "HERO_10a" || cardId == "HERO_10b")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossStartDemonHunter_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_BossStart_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    BTA_Fight_23 btaFight23 = this;
    while (btaFight23.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 110:
        yield return (object) btaFight23.PlayLineInOrderOnce(actor, btaFight23.m_VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_WeaponDestroy);
        break;
      case 500:
        btaFight23.PlaySound((string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Boss_Attack_01);
        break;
      case 507:
        yield return (object) btaFight23.PlayAndRemoveRandomLineOnlyOnce(actor, btaFight23.m_missionEventTrigger507Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight23.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_23 btaFight23 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight23.\u003C\u003En__1(entity);
    while (btaFight23.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight23.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight23.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight23.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "BT_117")
        yield return (object) btaFight23.PlayLineOnlyOnce(actor, (string) BTA_Fight_23.VO_BTA_BOSS_08hx_Female_Demon_Mission_Fight_23_Hero_Bladestorm_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_23 btaFight23 = this;
    while (btaFight23.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight23.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight23.\u003C\u003En__2(entity);
      yield return (object) btaFight23.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight23.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_23 btaFight23 = this;
    while (btaFight23.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }
}
