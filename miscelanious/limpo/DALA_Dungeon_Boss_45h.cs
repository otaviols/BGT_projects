using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_45h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_BossBigMinion_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_BossBigMinion_01.prefab:c9b5c007adc1d47479cb63fb745440bd");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_BossDamageSpell_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_BossDamageSpell_01.prefab:103109527d0edec4d831219d9f912c62");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_BossDemonBolt_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_BossDemonBolt_01.prefab:fb60f2c2d816b1c4da1d0aab3efe871b");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_Death_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_Death_01.prefab:391eccc27cc158f4891c0137404b57b7");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_DefeatPlayer_01.prefab:4c90adfa1ac2a914883b037e84c53291");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_EmoteResponse_01.prefab:270ba001adc69d346b11e0da01575706");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerHuge_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerHuge_01.prefab:1273a7741e390404bbae704ecf0af063");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerHuge_02 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerHuge_02.prefab:e7d00fe290f2b2940828884cd589c5c2");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerLarge_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerLarge_01.prefab:6f85f31d6f01dad42ba7081167fee6fb");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerLarge_03 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerLarge_03.prefab:1f465cbe004558f44b721730368109cb");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_02 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_02.prefab:03f11ba113733f347970bfc6d66e9b07");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_03 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_03.prefab:9f2a8ed468c1a7b409ae4474a8445fa5");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_04 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_04.prefab:4c7091808ee85d54291ab51adef09946");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_01.prefab:acff87aa9f3b49540ab389e22db4ab00");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_02 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_02.prefab:3143292133cb7264da7ae438cb182bd7");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_03 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_03.prefab:2e01bacaf6f390a4fb6d037b45b1d405");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_Idle_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_Idle_01.prefab:b1e880467bb31424bbb4bfdcb399cde9");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_Idle_02 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_Idle_02.prefab:716973a1e23e5e347a8e4468a6b80fcf");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_Idle_03 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_Idle_03.prefab:23cdc76d6145d8a4b8c96d51a7677c6e");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_Intro_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_Intro_01.prefab:b648d91b63b48f245a62f9be0e5ddd0c");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_IntroSqueamlish_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_IntroSqueamlish_01.prefab:806ab394e027f4647bae43c9088712c6");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_PlayerEntomb_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_PlayerEntomb_01.prefab:431fe0b8b5e767240a1d59775c8b192c");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_PlayerEyeforanEyeTrigger_01 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_PlayerEyeforanEyeTrigger_01.prefab:b979c5b6460858942849bf99336fef9c");
  private static readonly AssetReference VO_DALA_BOSS_45h_Male_Observer_PlayerPatches_02 = new AssetReference("VO_DALA_BOSS_45h_Male_Observer_PlayerPatches_02.prefab:528d096fa0569d845861f108881b39b2");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Idle_01,
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Idle_02,
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Idle_03
  };
  private static List<string> m_HeroPowerHuge = new List<string>()
  {
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerHuge_01,
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerHuge_02
  };
  private static List<string> m_HeroPowerLarge = new List<string>()
  {
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerLarge_01,
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerLarge_03
  };
  private static List<string> m_HeroPowerMedium = new List<string>()
  {
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_02,
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_03,
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_04
  };
  private static List<string> m_HeroPowerSmall = new List<string>()
  {
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_01,
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_02,
    (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_BossBigMinion_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_BossDamageSpell_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_BossDemonBolt_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Death_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerHuge_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerHuge_02,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerLarge_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerLarge_03,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_02,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_03,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerMedium_04,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_02,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_HeroPowerSmall_03,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Idle_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Idle_02,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Idle_03,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Intro_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_IntroSqueamlish_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_PlayerEntomb_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_PlayerEyeforanEyeTrigger_01,
      (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_PlayerPatches_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_45h.m_IdleLines;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_Squeamlish")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_IntroSqueamlish_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "DALA_Tekahn"))
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
    DALA_Dungeon_Boss_45h dalaDungeonBoss45h = this;
    while (dalaDungeonBoss45h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss45h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_45h.m_HeroPowerHuge);
        break;
      case 102:
        yield return (object) dalaDungeonBoss45h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_45h.m_HeroPowerLarge);
        break;
      case 103:
        yield return (object) dalaDungeonBoss45h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_45h.m_HeroPowerMedium);
        break;
      case 104:
        yield return (object) dalaDungeonBoss45h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_45h.m_HeroPowerSmall);
        break;
      case 105:
        yield return (object) dalaDungeonBoss45h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_BossBigMinion_01);
        break;
      case 106:
        yield return (object) dalaDungeonBoss45h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_PlayerEyeforanEyeTrigger_01);
        break;
      case 107:
        yield return (object) dalaDungeonBoss45h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_PlayerPatches_02);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss45h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_45h dalaDungeonBoss45h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss45h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss45h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss45h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss45h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss45h.m_playedLines.Add(cardId);
      if (cardId == "LOE_104")
        yield return (object) dalaDungeonBoss45h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_PlayerEntomb_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_45h dalaDungeonBoss45h = this;
    while (dalaDungeonBoss45h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss45h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss45h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss45h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "TRL_555"))
      {
        if (cardId == "CS2_062" || cardId == "EX1_312" || cardId == "OG_239")
          yield return (object) dalaDungeonBoss45h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_BossDamageSpell_01);
      }
      else
        yield return (object) dalaDungeonBoss45h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_45h.VO_DALA_BOSS_45h_Male_Observer_BossDemonBolt_01);
    }
  }
}
