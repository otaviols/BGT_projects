using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_63h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_BossBigWeapon_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_BossBigWeapon_01.prefab:9784661d7e311d849bf1835ef8179ad7");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_BossBrawl_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_BossBrawl_01.prefab:8dff18608f17a894b8cb74d0c039e9e4");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_Death_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_Death_01.prefab:1a4a883ab3a4fe04d95aabd6d04bf895");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_DefeatPlayer_01.prefab:bbd0d481210cce443bd5039f7d45678d");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_EmoteResponse_01.prefab:9de198d6fd5a14f4caa17ca7a28e87ea");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPower_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPower_01.prefab:cdbef254005d58e4f90ddc35bfb49d51");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPower_02 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPower_02.prefab:056b67e688cc12e4ca622361ab47b20f");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPower_03 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPower_03.prefab:05bf56009309d504a977a4ad167583f1");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPower_04 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPower_04.prefab:488c9e1d211e6844ba412e8534832820");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPower_06 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPower_06.prefab:00d17bdb4f9ea7948af3055891fba90b");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPower_07 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPower_07.prefab:f8a993b2f7342074a8437ae9a20b53fa");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPower_08 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPower_08.prefab:440cdcfdab0e5a3449c10a29ad26361b");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_01.prefab:8e1bdf0f90b1cce45a05f3cc8a997d14");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_02 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_02.prefab:98b3e8862b840154d809e0ae7a33b855");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_03 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_03.prefab:8cff32844530f6042ad345db5dc2ccec");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_Idle_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_Idle_01.prefab:0ef683245d33d974cacc58065fcb6e0b");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_Idle_02 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_Idle_02.prefab:14a8caa11d7b0c640a74fe8307987266");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_Idle_03 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_Idle_03.prefab:391b87c4109fbf74699041f93c5b1107");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_Intro_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_Intro_01.prefab:656ea1eb30a094f448ef27cb5f474710");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_PlayerBurglyBully_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_PlayerBurglyBully_01.prefab:e6dc51ef6513a0545a7fcb58e286d892");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_PlayerDevestate_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_PlayerDevestate_01.prefab:17171b8a17cf96641ae127666f5a6d64");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_PlayerDragon_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_PlayerDragon_01.prefab:19d54d88746302f4091eece73b5092b3");
  private static readonly AssetReference VO_DALA_BOSS_63h_Male_Orc_PlayerInnerRage_01 = new AssetReference("VO_DALA_BOSS_63h_Male_Orc_PlayerInnerRage_01.prefab:4aafbb8a6461e3f41b1e55d5dfb993d4");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Idle_01,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Idle_02,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Idle_03
  };
  private static List<string> m_HeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_01,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_02,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_03,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_04,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_06,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_07,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_08
  };
  private static List<string> m_HeroPowerRare = new List<string>()
  {
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_01,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_02,
    (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_BossBigWeapon_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_BossBrawl_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Death_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_02,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_03,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_04,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_06,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_07,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPower_08,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_02,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_HeroPowerRare_03,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Idle_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Idle_02,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Idle_03,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Intro_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_PlayerBurglyBully_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_PlayerDevestate_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_PlayerDragon_01,
      (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_PlayerInnerRage_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_63h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Eudora") || !(cardId != "DALA_Chu") || !(cardId != "DALA_Squeamlish"))
        return;
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
    DALA_Dungeon_Boss_63h dalaDungeonBoss63h = this;
    while (dalaDungeonBoss63h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss63h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_BossBigWeapon_01);
        break;
      case 102:
        yield return (object) dalaDungeonBoss63h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_PlayerDragon_01);
        break;
      case 103:
        yield return (object) dalaDungeonBoss63h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_63h.m_HeroPower);
        break;
      case 104:
        yield return (object) dalaDungeonBoss63h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_63h.m_HeroPowerRare);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss63h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_63h dalaDungeonBoss63h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss63h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss63h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss63h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss63h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss63h.m_playedLines.Add(cardId);
      if (!(cardId == "CFM_669"))
      {
        if (!(cardId == "TRL_321"))
        {
          if (cardId == "EX1_607")
            yield return (object) dalaDungeonBoss63h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_PlayerInnerRage_01);
        }
        else
          yield return (object) dalaDungeonBoss63h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_PlayerDevestate_01);
      }
      else
        yield return (object) dalaDungeonBoss63h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_PlayerBurglyBully_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_63h dalaDungeonBoss63h = this;
    while (dalaDungeonBoss63h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss63h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss63h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss63h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "EX1_407")
        yield return (object) dalaDungeonBoss63h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_63h.VO_DALA_BOSS_63h_Male_Orc_BossBrawl_01);
    }
  }
}
