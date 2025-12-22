using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Faelin_04 : BoH_Faelin_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Faelin_04.InitBooleanOptions();
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4ExchangeD_01 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4ExchangeD_01.prefab:1ccd2dd9851216743956131049085968");
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4ExchangeE_01 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4ExchangeE_01.prefab:832457df1e8266243abe4fc517069d14");
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4Victory_01 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4Victory_01.prefab:396764033087a9a43bba5d66f7f8fc3c");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeA_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeA_01.prefab:41a1f7bfde1a9474c9cdb5d0358f7b21");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_01.prefab:964c62eb038f88c4ab9d3c0c127d56fd");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_02 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_02.prefab:3e0c68c25f55d82468009e4c727394dd");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_03 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_03.prefab:bdca3515e8760504884cfac9b3d30c4c");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeC_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeC_01.prefab:618ca42488f54f8489e094d58216c90e");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeD_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeD_01.prefab:a4f1f205b544051429d15f9bddf7cceb");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeE_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeE_01.prefab:27bd5070b56ef604fb05380cb1868e1e");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Loss_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Loss_01.prefab:45304247ce4cf5846a79f093ce453151");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Loss_02 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Loss_02.prefab:14cdb6ef6a8c76d4aaf289500c5dd852");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Start_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Start_01.prefab:01c33679a7cd9ee409a718a6f84bb851");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Victory_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Victory_01.prefab:d8684de97a21d7f4984fc39e24a8060c");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeC_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeC_01.prefab:2ec1ca0ee393a5c4b8d53a96fd795660");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeD_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeD_01.prefab:b49cbe405195b6148bfaaede3f8ae688");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeE_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeE_01.prefab:1c73daba59a173d4f8572b5a1d52dbe6");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_01.prefab:3a5e940ad8c7aed4a9788c7336e3c57e");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_02 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_02.prefab:07a4ee4bfb204fd44a4c03656d53ed57");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_03 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_03.prefab:4f95caa9522cbe14ea4e9f5c0c7a9f9c");
  private static readonly AssetReference Story_11_Leviathan_001hb_Death_Sound = new AssetReference("Story_11_Leviathan_001hb_Death_Sound.prefab:d5051de62f48f4b49aa855220792f2f4");
  private static readonly AssetReference Story_11_Leviathan_001hb_EmoteResponse_Sound = new AssetReference("Story_11_Leviathan_001hb_EmoteResponse_Sound.prefab:90b2aae30ca06f648a8033e394c1fa22");
  private static readonly AssetReference Story_11_Leviathan_001hb_Loss_Sound = new AssetReference("Story_11_Leviathan_001hb_Loss_Sound.prefab:c8702d2dff96aa1468e091e19a0f99cb");
  private static readonly AssetReference Story_11_Leviathan_001hb_Rev_Sound = new AssetReference("Story_11_Leviathan_001hb_Rev_Sound.prefab:63e2a91d4276f1a429b25b7ddcddd53d");
  public static readonly AssetReference IniBrassRing = new AssetReference("HS25-189_H_Ini_BrassRing_Quote.prefab:9f20435bdae99d24bae357f002abcf27");
  public static readonly AssetReference CayeBrassRing = new AssetReference("LC23-001_H_Caye_BrassRing_Quote.prefab:2364e8ce64f9c404fb569a86b17f3d01");
  public static readonly AssetReference BookBrassRing = new AssetReference("Wisdomball_Pop-up_BrassRing_Quote.prefab:896ee20514caff74db639aa7055838f6");
  public static readonly AssetReference FinleyBrassRing = new AssetReference("HS25-190_M_Finley_BrassRing_Quote.prefab:7123c129afe769e4aa9fb262a8f7c919");
  public static readonly AssetReference HalusBrassRing = new AssetReference("LC23-003_H_Halus_BrassRing_Quote.prefab:36e0aabb3ed4ce84599d9dd4e9ebdcf5");
  public static readonly AssetReference GraceBrassRing = new AssetReference("LC23-002_H_Grace_BrassRing_Quote.prefab:1f849ba2e303ff3449d34b96f960c96a");
  public static readonly AssetReference FaelinBrassRing = new AssetReference("SKN23-002_H_FAELIN_BrassRing_Quote.prefab:9984152d900140547aa7d721f3e49428");
  private List<string> m_LossLines = new List<string>()
  {
    (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Loss_01,
    (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Loss_02
  };
  private List<string> m_NegativeReactionLines = new List<string>()
  {
    (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_01,
    (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_02,
    (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Faelin_04() => this.m_gameOptions.AddBooleanOptions(BoH_Faelin_04.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Start_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeA_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_02,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_03,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeC_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeE_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeC_01,
      (string) BoH_Faelin_04.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4ExchangeD_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeD_01,
      (string) BoH_Faelin_04.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4ExchangeE_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeE_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Victory_01,
      (string) BoH_Faelin_04.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4Victory_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_02,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_03,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Loss_01,
      (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Loss_02,
      (string) BoH_Faelin_04.Story_11_Leviathan_001hb_Death_Sound,
      (string) BoH_Faelin_04.Story_11_Leviathan_001hb_EmoteResponse_Sound,
      (string) BoH_Faelin_04.Story_11_Leviathan_001hb_Loss_Sound,
      (string) BoH_Faelin_04.Story_11_Leviathan_001hb_Rev_Sound
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_Mission_EnemyPlayIdleLines = false;
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_TSC_Leviathan;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Faelin_04 boHFaelin04 = this;
    while (boHFaelin04.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 102:
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_01);
        break;
      case 103:
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeC_01);
        break;
      case 104:
        yield return (object) boHFaelin04.MissionPlayVO(BoH_Faelin_04.FaelinBrassRing, (string) BoH_Faelin_04.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4ExchangeD_01);
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeD_01);
        break;
      case 105:
        yield return (object) boHFaelin04.MissionPlayVO(BoH_Faelin_04.FaelinBrassRing, (string) BoH_Faelin_04.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4ExchangeE_01);
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeE_01);
        break;
      case 106:
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeA_01);
        break;
      case 107:
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_02);
        break;
      case 108:
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4ExchangeB_03);
        break;
      case 109:
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeC_01);
        break;
      case 110:
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeE_01);
        break;
      case 111:
        yield return (object) boHFaelin04.PlayLineInOrderOnce(friendlyActor, boHFaelin04.m_NegativeReactionLines);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHFaelin04.MissionPlaySound(actor, (string) BoH_Faelin_04.Story_11_Leviathan_001hb_Rev_Sound);
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, (string) BoH_Faelin_04.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission4Victory_01);
        yield return (object) boHFaelin04.MissionPlayVO(BoH_Faelin_04.FaelinBrassRing, (string) BoH_Faelin_04.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission4Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 515:
        yield return (object) boHFaelin04.MissionPlaySound(actor, (string) BoH_Faelin_04.Story_11_Leviathan_001hb_EmoteResponse_Sound);
        break;
      case 520:
        GameState.Get().SetBusy(true);
        yield return (object) boHFaelin04.MissionPlaySound(actor, (string) BoH_Faelin_04.Story_11_Leviathan_001hb_Loss_Sound);
        yield return (object) boHFaelin04.MissionPlayVO(friendlyActor, boHFaelin04.m_LossLines);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHFaelin04.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Faelin_04 boHFaelin04 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHFaelin04.\u003C\u003En__1(entity);
    while (boHFaelin04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHFaelin04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHFaelin04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHFaelin04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Faelin_04 boHFaelin04 = this;
    while (boHFaelin04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHFaelin04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHFaelin04.\u003C\u003En__2(entity);
      yield return (object) boHFaelin04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHFaelin04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Faelin_04 boHFaelin04 = this;
    while (boHFaelin04.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }
}
