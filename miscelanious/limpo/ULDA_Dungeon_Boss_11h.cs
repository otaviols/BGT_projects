using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_11h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_BossFireball_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_BossFireball_01.prefab:a9bc1a89d1342b54ba636a4af3a9054c");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_BossFlamestrike_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_BossFlamestrike_01.prefab:2a1233559d309f54aad7a8898dde6fbc");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_BossTombWarden_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_BossTombWarden_01.prefab:543cfb02873b4ba45969a03ed8ba3c3d");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_DeathALT_01.prefab:bde450fd9b831a34ca4fd72a06065cce");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_DefeatPlayer_01.prefab:1acbd439713bab34fb277f38a5801177");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_EmoteResponse_01.prefab:a8fe00c1179535449b9bacd63b7b719b");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_HeroPower_01.prefab:3cd1c82fe46697148b129bd79ac6495d");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_HeroPower_03.prefab:65f6fe0474443b745b1294f3523f1d61");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_HeroPower_04.prefab:1ad76cd877be8d946872948a18629beb");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_HeroPower_05.prefab:f25e4acdd8969e0428b0f53eefa1879f");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_Idle1_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_Idle1_01.prefab:fa133439491238c4fb4c5570d04fa478");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_Idle2_02 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_Idle2_02.prefab:c530a76ea1469364ba6d6328043e0a5d");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_Idle3_03 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_Idle3_03.prefab:9e8f2f16970ef204392072d59cb25106");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_Intro_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_Intro_01.prefab:0d2a41dde5a7b294a81b0fc7bc414262");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_IntroBrannResponse_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_IntroBrannResponse_01.prefab:ee8c6a117004c4442b094d077f935ebb");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_PlayerDrBoom_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_PlayerDrBoom_01.prefab:aedf5be25ff0400479fe964900e019a1");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_PlayerKobold_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_PlayerKobold_01.prefab:554a9d5bae3df3d498a35f4bf55484af");
  private static readonly AssetReference VO_ULDA_BOSS_11h_Male_Mech_PlayerLargeMech_01 = new AssetReference("VO_ULDA_BOSS_11h_Male_Mech_PlayerLargeMech_01.prefab:894d5af10a9b49843ac4483642f69462");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_HeroPower_01,
    (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_HeroPower_03,
    (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_HeroPower_04,
    (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_HeroPower_05
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_BossFireball_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_BossFlamestrike_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_BossTombWarden_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_DeathALT_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_HeroPower_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_HeroPower_03,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_HeroPower_04,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_HeroPower_05,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_Idle1_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_Idle2_02,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_Idle3_03,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_Intro_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_IntroBrannResponse_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_PlayerDrBoom_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_PlayerKobold_01,
      (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_PlayerLargeMech_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Brann")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_IntroBrannResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_11h uldaDungeonBoss11h = this;
    while (uldaDungeonBoss11h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss11h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss11h.m_HeroPowerLines);
        break;
      case 102:
        yield return (object) uldaDungeonBoss11h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_PlayerLargeMech_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss11h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_11h uldaDungeonBoss11h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss11h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss11h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss11h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss11h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss11h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "CS2_142":
        case "DAL_417":
        case "DAL_614":
        case "LOOT_014":
        case "LOOT_041":
        case "LOOT_062":
        case "LOOT_347":
        case "LOOT_367":
        case "LOOT_382":
        case "LOOT_389":
        case "LOOT_412":
        case "LOOT_541":
        case "OG_082":
        case "ULD_184":
          yield return (object) uldaDungeonBoss11h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_PlayerKobold_01);
          break;
        case "DAL_064":
        case "GVG_110":
          yield return (object) uldaDungeonBoss11h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_PlayerDrBoom_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_11h uldaDungeonBoss11h = this;
    while (uldaDungeonBoss11h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss11h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss11h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss11h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss11h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_029"))
      {
        if (!(cardId == "CS2_032"))
        {
          if (cardId == "ULD_253")
            yield return (object) uldaDungeonBoss11h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_BossTombWarden_01);
        }
        else
          yield return (object) uldaDungeonBoss11h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_BossFlamestrike_01);
      }
      else
        yield return (object) uldaDungeonBoss11h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_11h.VO_ULDA_BOSS_11h_Male_Mech_BossFireball_01);
    }
  }
}
