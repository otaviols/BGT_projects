using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOM_03_Guff_Dungeon : BOM_03_Guff_MissionEntity
{
  public readonly AssetReference Brukan_BrassRing = new AssetReference("Brukan_BrassRing_Quote.prefab:16aa2801dfe06db489bd2259944af32b");
  public readonly AssetReference Rokara_B_BrassRing = new AssetReference("Rokara_B_BrassRing_Quote.prefab:301c3d7a32636944884d6fa120099950");
  public readonly AssetReference Tamsin_BrassRing = new AssetReference("Tamsin_BrassRing_Quote.prefab:62964357f9958d64f9346685fc1f87f5");
  public readonly AssetReference Dawngrasp_BrassRing = new AssetReference("Dawngrasp_BrassRing_Quote.prefab:45d9ad7c018bcf7429f8ff3d10e2aaf0");
  public readonly AssetReference Hamuul_20_4_BrassRing_Quote = new AssetReference("Hamuul_20_4_BrassRing_Quote.prefab:54c037c90dc48994b8db6374e72f32ab");
  public readonly AssetReference Naralex_BrassRing = new AssetReference("Naralex_BrassRing_Quote.prefab:6bbc6ac031d7ccf48a6e7edd7933d248");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_01.prefab:e68a254459535874c93976f6f44c2612");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_02.prefab:86eb7ab46c12a0f45b99589387128a14");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_03.prefab:94d244024e2844648b14650966ef2b6f");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_04 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_04.prefab:c675321f29a32224e816755609b3d64e");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_05 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_05.prefab:27b889a954e6d444f8bd91fe7b5fb7f9");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_06 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_06.prefab:3dbe08c856d9df94b8612e68e6438357");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_01.prefab:25487958973e1c44b8420788fb3ef1dd");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_02.prefab:3d9fa43f0094f3744bcd268400aa1158");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_03.prefab:7728604172e2de14194b7ce46cbf27c4");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_04 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_04.prefab:36aa36379df63984a8ce0679d1ad4d33");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_05 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_05.prefab:59a194a3be06a5c4eba09d6caad103fd");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_06 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_06.prefab:243169e212bd7464493d04a1772f4894");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_01.prefab:a9e2702de8692584b8122089162dfaca");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_02.prefab:e61c4b0290c531e4089794f1ee41bb37");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_03.prefab:1b193b2362a255c41965142cb1aa3e32");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_04 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_04.prefab:4778ed0a3df0e444a90a136bf75637d0");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_05 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_05.prefab:e8345dd582c4d8a4d8fdb0b29f5594e6");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_06 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_06.prefab:868f935cd5262d7478b304db1fc9097c");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_02.prefab:5c59fe1390006ac47893c626407cfeb4");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_04 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_04.prefab:14232e12bd04f984094b465344d47a4f");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_05 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_05.prefab:d54d0e82d394b754ab824a1abf586137");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_06 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_06.prefab:f5da77d66fd1f594c8bbb082353258ce");
  private List<string> m_Tamsin_HeroPowerLines = new List<string>()
  {
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_02,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_04,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_05,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_06
  };
  private List<string> m_Rokara_HeroPowerLines = new List<string>()
  {
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_01,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_02,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_03,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_04,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_05,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_06
  };
  private List<string> m_Dawngrasp_HeroPowerLines = new List<string>()
  {
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_01,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_02,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_03,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_04,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_05,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_06
  };
  private List<string> m_Brukan_HeroPowerLines = new List<string>()
  {
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_01,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_02,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_03,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_04,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_05,
    (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_06
  };
  public bool HeroPowerBrukan;
  public bool HeroPowerTamsin;
  public bool HeroPowerDawngrasp;
  public bool HeroPowerRokara;
  public const int Tavish_TriggerLine = 58032;
  public const int Tavish_DeathLines = 58033;
  public const int Tavish_HealLines = 58034;
  public const int Tavish_IsDeadLines = 58035;
  public const int Tavish_RezLines = 58036;
  public const int Tavish_Attack = 58042;
  public const int Scabbs_RezLines = 58037;
  public const int Scabbs_DeathLines = 58038;
  public const int Scabbs_TriggerLines = 58039;
  public const int Scabbs_HealLines = 58040;
  public const int Scabbs_isDeadLines = 58041;
  public const int Scabbs_Attack = 58043;
  public bool m_Scabbs_isDead;
  public bool m_Tavish_isDead;
  public const int XyrellaCustomIdle = 58042;
  public const int SetHeroPowerBrukan = 58024;
  public const int SetHeroPowerRokara = 58025;
  public const int SetHeroPowerTamsin = 58026;
  public const int SetHeroPowerDawngrasp = 58027;
  public const float m_Xyrella_HP_Speaking_Chance = 0.5f;
  public const float m_Xyrella_HP_Speaking_Delay = 20f;
  public float m_Xyrella_HP_Seconds_Since_Action;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) null;
    this.m_deathLine = (string) null;
    this.m_standardEmoteResponseLine = (string) null;
    this.m_BossIdleLines = new List<string>((IEnumerable<string>) this.GetBossIdleLines());
    this.m_BossIdleLinesCopy = new List<string>((IEnumerable<string>) this.GetBossIdleLines());
    this.m_OverrideMusicTrack = MusicPlaylistType.Invalid;
    this.m_OverrideMulliganMusicTrack = MusicPlaylistType.Invalid;
    this.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
    this.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
    this.m_OverrideBossSubtext = (string) null;
    this.m_OverridePlayerSubtext = (string) null;
    this.m_SupressEnemyDeathTextBubble = false;
  }

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_02,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_04,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_05,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2HeroPower_06,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_01,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_02,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_03,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_04,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_05,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2HeroPower_06,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_01,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_02,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_03,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_04,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_05,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2HeroPower_06,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_01,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_02,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_03,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_04,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_05,
      (string) BOM_03_Guff_Dungeon.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2HeroPower_06
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.BOM;

  public static BOM_03_Guff_Dungeon InstantiateTemplate_SoloDungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    Log.All.PrintError("BOM_03_Guff_Dungeon.InstantiateTemplate_SoloDungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame));
    return new BOM_03_Guff_Dungeon();
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_03_Guff_Dungeon bom03GuffDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom03GuffDungeon.\u003C\u003En__0(entity);
    yield return (object) bom03GuffDungeon.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_03_Guff_Dungeon bom03GuffDungeon = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      bom03GuffDungeon.MissionPause(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    bom03GuffDungeon.MissionPause(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) bom03GuffDungeon.HandleMissionEventWithTiming(514);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_03_Guff_Dungeon bom03GuffDungeon = this;
    while (bom03GuffDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (bom03GuffDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num1 = (double) Random.Range(0.0f, 1f);
      bom03GuffDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      double num2 = (double) Random.Range(0.0f, 1f);
      switch (missionEvent)
      {
        case 508:
          if (bom03GuffDungeon.HeroPowerBrukan)
            yield return (object) bom03GuffDungeon.MissionPlaySound(friendlyHeroPowerActor, bom03GuffDungeon.m_Brukan_HeroPowerLines);
          if (bom03GuffDungeon.HeroPowerRokara)
            yield return (object) bom03GuffDungeon.MissionPlaySound(friendlyHeroPowerActor, bom03GuffDungeon.m_Rokara_HeroPowerLines);
          if (bom03GuffDungeon.HeroPowerTamsin)
            yield return (object) bom03GuffDungeon.MissionPlaySound(friendlyHeroPowerActor, bom03GuffDungeon.m_Tamsin_HeroPowerLines);
          if (!bom03GuffDungeon.HeroPowerDawngrasp)
            break;
          yield return (object) bom03GuffDungeon.MissionPlaySound(friendlyHeroPowerActor, bom03GuffDungeon.m_Dawngrasp_HeroPowerLines);
          break;
        case 516:
          if (bom03GuffDungeon.m_SupressEnemyDeathTextBubble)
          {
            yield return (object) bom03GuffDungeon.MissionPlaySound(enemyActor, bom03GuffDungeon.m_deathLine);
            break;
          }
          yield return (object) bom03GuffDungeon.MissionPlayVO(enemyActor, bom03GuffDungeon.m_deathLine);
          break;
        case 600:
          bom03GuffDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = false;
          break;
        case 601:
          bom03GuffDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = false;
          break;
        case 602:
          bom03GuffDungeon.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          bom03GuffDungeon.m_MissionDisableAutomaticVO = false;
          break;
        case 610:
          bom03GuffDungeon.m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
          break;
        case 611:
          bom03GuffDungeon.m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
          break;
        case 612:
          bom03GuffDungeon.m_DoEmoteDrivenStart = true;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom03GuffDungeon.m_PlayPlayerVOLineIndex + 1 >= bom03GuffDungeon.m_PlayerVOLines.Count)
            bom03GuffDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++bom03GuffDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(bom03GuffDungeon.m_PlayerVOLines[bom03GuffDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom03GuffDungeon.PlayBossLine(actor, bom03GuffDungeon.m_PlayerVOLines[bom03GuffDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom03GuffDungeon.m_PlayerVOLines[bom03GuffDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) bom03GuffDungeon.PlayBossLine(actor, bom03GuffDungeon.m_PlayerVOLines[bom03GuffDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (bom03GuffDungeon.m_PlayBossVOLineIndex + 1 >= bom03GuffDungeon.m_BossVOLines.Count)
            bom03GuffDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++bom03GuffDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(bom03GuffDungeon.m_BossVOLines[bom03GuffDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom03GuffDungeon.PlayBossLine(enemyActor, bom03GuffDungeon.m_BossVOLines[bom03GuffDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(bom03GuffDungeon.m_BossVOLines[bom03GuffDungeon.m_PlayBossVOLineIndex]);
          yield return (object) bom03GuffDungeon.PlayBossLine(enemyActor, bom03GuffDungeon.m_BossVOLines[bom03GuffDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (bom03GuffDungeon.m_forceAlwaysPlayLine)
          {
            bom03GuffDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          bom03GuffDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom03GuffDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom03GuffDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in bom03GuffDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom03GuffDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in bom03GuffDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) bom03GuffDungeon.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in bom03GuffDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) bom03GuffDungeon.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        case 58024:
          bom03GuffDungeon.HeroPowerBrukan = true;
          bom03GuffDungeon.HeroPowerRokara = false;
          bom03GuffDungeon.HeroPowerTamsin = false;
          bom03GuffDungeon.HeroPowerDawngrasp = false;
          break;
        case 58025:
          bom03GuffDungeon.HeroPowerBrukan = false;
          bom03GuffDungeon.HeroPowerRokara = true;
          bom03GuffDungeon.HeroPowerTamsin = false;
          bom03GuffDungeon.HeroPowerDawngrasp = false;
          break;
        case 58026:
          bom03GuffDungeon.HeroPowerBrukan = false;
          bom03GuffDungeon.HeroPowerRokara = false;
          bom03GuffDungeon.HeroPowerTamsin = true;
          bom03GuffDungeon.HeroPowerDawngrasp = false;
          break;
        case 58027:
          bom03GuffDungeon.HeroPowerBrukan = false;
          bom03GuffDungeon.HeroPowerRokara = false;
          bom03GuffDungeon.HeroPowerTamsin = false;
          bom03GuffDungeon.HeroPowerDawngrasp = true;
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) bom03GuffDungeon.\u003C\u003En__1(missionEvent);
          break;
      }
    }
  }
}
