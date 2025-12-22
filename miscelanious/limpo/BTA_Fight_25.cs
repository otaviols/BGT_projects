using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_25 : BTA_Dungeon_Heroic
{
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Alar_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Alar_01.prefab:4aef60dfaf30b284da28ca670afa55b0");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Attack_01.prefab:787a7dfe8c47dfe44a12934f2e8f0a40");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Flamestrike_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Flamestrike_01.prefab:2939a0f0b0f981b4e9c38402a754d70d");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapse_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapse_01.prefab:a379c6013128a8e4e85deff9774a316b");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapse_02 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapse_02.prefab:df8037736618d744b8de92b99fc23f21");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_VaporizeTrigger_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_VaporizeTrigger_01.prefab:83dbf0df0f3cc984aa2a347e9bcdeca0");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossDeath_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossDeath_01.prefab:5ec0485117847b14f8a6c27615ea7dae");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStart_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStart_01.prefab:8c365e844e8fbd942ae834384c6a9306");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartArthas_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartArthas_01.prefab:428703020b5b38944b94a3c33734c3c3");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartIllidan_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartIllidan_01.prefab:d03f1948124a1b64d9bf23c3398d8669");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartJaina_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartJaina_01.prefab:2c1802c42b8c882449a15adf4c402698");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartLiadrin_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartLiadrin_01.prefab:4a0147b3cc19e5947a7e6667fe8e682b");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Emote_Response_01.prefab:a9da02ca8273f2f4083bd7278ce10fe2");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_CounterSpell_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_CounterSpell_01.prefab:f0ef1375170df6944b07429733b3c37e");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_FrostSpell_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_FrostSpell_01.prefab:25750ef66cb48ca46a213e8e2665a0d8");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_LichKing_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_LichKing_01.prefab:877527192df02f64a934c620c2f52c87");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_Treant_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_Treant_01.prefab:2620c6ede6afc634d8767180bd30374a");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_01.prefab:b5f544133cfa14e46ab09f2dd879e0ae");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_02 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_02.prefab:9c325f9ef72a5394bbc0b3be1ce70204");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_03 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_03.prefab:bd8f21c60ae861f43b743e190cf66347");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_04 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_04.prefab:b4237b7c38f022647938e331d248d6aa");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleA_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleA_01.prefab:585b909af2b19cc4db4e57a491c7e90a");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleB_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleB_01.prefab:f5641b662f8b72747ad2ec9dfb1d7d6a");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleC_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleC_01.prefab:b7495f55053f07547b51f30dc4d37e87");
  private static readonly AssetReference VO_BTA_BOSS_25h_Male_BloodElf_UI_Mission_Fight_25_CoinSelect_01 = new AssetReference("VO_BTA_BOSS_25h_Male_BloodElf_UI_Mission_Fight_25_CoinSelect_01.prefab:9c22bb165b089ac40a2c4e750f597cb6");
  private List<string> m_VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapseLines = new List<string>()
  {
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapse_01,
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapse_02
  };
  private List<string> m_VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPowerLines = new List<string>()
  {
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_01,
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_02,
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_03,
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_04
  };
  private List<string> m_VO_BTA_BOSS_25h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleA_01,
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleB_01,
    (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleC_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Alar_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Attack_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Flamestrike_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapse_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapse_02,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_VaporizeTrigger_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossDeath_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStart_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartArthas_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartIllidan_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartJaina_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartLiadrin_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Emote_Response_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_CounterSpell_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_FrostSpell_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_LichKing_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_Treant_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_02,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_03,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPower_04,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleA_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleB_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_IdleC_01,
      (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_UI_Mission_Fight_25_CoinSelect_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_25h_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossDeath_01;
    this.m_standardEmoteResponseLine = (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Emote_Response_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "HERO_04a")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartLiadrin_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "HERO_04b")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartArthas_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "HERO_10" || cardId == "HERO_10a")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartIllidan_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "HERO_08" || cardId == "HERO_08c")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStartJaina_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_BossStart_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    BTA_Fight_25 btaFight25 = this;
    while (btaFight25.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 110:
        yield return (object) btaFight25.PlayLineOnlyOnce(actor, (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_VaporizeTrigger_01);
        break;
      case 115:
        yield return (object) btaFight25.PlayLineAlways(actor, (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_CounterSpell_01);
        break;
      case 120:
        yield return (object) btaFight25.PlayLineOnlyOnce(actor, (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_Treant_01);
        break;
      case 500:
        btaFight25.PlaySound((string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Attack_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight25.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_25 btaFight25 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight25.\u003C\u003En__1(entity);
    while (btaFight25.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight25.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight25.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight25.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "BT_072":
        case "CS2_024":
        case "CS2_026":
        case "CS2_028":
        case "CS2_037":
        case "DAL_577":
        case "DRG_248":
        case "EX1_275":
          yield return (object) btaFight25.PlayLineOnlyOnce(actor, (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_FrostSpell_01);
          break;
        case "ICC_314":
          yield return (object) btaFight25.PlayLineOnlyOnce(actor, (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Hero_LichKing_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_25 btaFight25 = this;
    while (btaFight25.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight25.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight25.\u003C\u003En__2(entity);
      yield return (object) btaFight25.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight25.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BTA_BOSS_25s"))
      {
        if (!(cardId == "BT_735"))
        {
          if (!(cardId == "CS2_032"))
          {
            if (cardId == "EX1_279")
              yield return (object) btaFight25.PlayAndRemoveRandomLineOnlyOnce(actor, btaFight25.m_VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_HeroPowerLines);
          }
          else
            yield return (object) btaFight25.PlayLineOnlyOnce(actor, (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Flamestrike_01);
        }
        else
          yield return (object) btaFight25.PlayLineOnlyOnce(actor, (string) BTA_Fight_25.VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_Alar_01);
      }
      else
        yield return (object) btaFight25.PlayAndRemoveRandomLineOnlyOnce(actor, btaFight25.m_VO_BTA_BOSS_25h_Male_BloodElf_Mission_Fight_25_Boss_GravityLapseLines);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_25 btaFight25 = this;
    while (btaFight25.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }
}
