using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Faelin_08 : BoH_Faelin_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Faelin_08.InitBooleanOptions();
  private static readonly AssetReference VO_Story_11_Caye_012hp_Female_Nightborne_Story_Faelin_PreMission8_01 = new AssetReference("VO_Story_11_Caye_012hp_Female_Nightborne_Story_Faelin_PreMission8_01.prefab:7d8d790efe69a984ab1ba100fcc3b8ba");
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8ExchangeA_01 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8ExchangeA_01.prefab:d5b0eaf00b54de6429b561a836800fca");
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8ExchangeF_01 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8ExchangeF_01.prefab:46369b303279e28439b60ce90e9f2dfb");
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Start_01 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Start_01.prefab:98e13ee59e7c21c42b3f880cff0f632b");
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Victory_01 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Victory_01.prefab:5b28472ff57c86644b829dd15567a261");
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Victory_02 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Victory_02.prefab:9f4ab1ffc6509d24ca8fbecfadeeddd6");
  private static readonly AssetReference VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_PreMission8_01 = new AssetReference("VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_PreMission8_01.prefab:09becc0bc2c6a3a4ebdab286f6b380c5");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Loss_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Loss_01.prefab:ec4afe4d893bf38438d61c7f38c4ef9d");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Loss_02 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Loss_02.prefab:5602b344563ad2149bda928a72ab1766");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_01.prefab:3a5e940ad8c7aed4a9788c7336e3c57e");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_02 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_02.prefab:07a4ee4bfb204fd44a4c03656d53ed57");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_03 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_03.prefab:4f95caa9522cbe14ea4e9f5c0c7a9f9c");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_04 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_04.prefab:67293738c15302947a9c0c493d573ac7");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_05 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_05.prefab:a1868e7f968aa00469414f8edb9728fa");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeA_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeA_01.prefab:0e076cf0fbd990848a0affd995f48182");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeB_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeB_01.prefab:80c63c7cb4fd77d418a8bfccf57e20a7");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeC_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeC_01.prefab:2ec1ca0ee393a5c4b8d53a96fd795660");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeD_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeD_01.prefab:b49cbe405195b6148bfaaede3f8ae688");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeE_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeE_01.prefab:1c73daba59a173d4f8572b5a1d52dbe6");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeF_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeF_01.prefab:df4f25624a5ccdd4fb66a0e57fe50c1e");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Start_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Start_01.prefab:9e383c3e57577d5478db79f9bd56044d");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Start_02 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Start_02.prefab:cb1488da7676d584eb25b18bf48c9e37");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Victory_01 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Victory_01.prefab:61fa161f66762304e98aed938672e75b");
  private static readonly AssetReference VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Victory_02 = new AssetReference("VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Victory_02.prefab:3b5c8b1fa064a0949b5b74c3acd10dc6");
  private static readonly AssetReference Story_11_Leviathan_001hb_Death_Sound = new AssetReference("Story_11_Leviathan_001hb_Death_Sound.prefab:d5051de62f48f4b49aa855220792f2f4");
  private static readonly AssetReference Story_11_Leviathan_001hb_EmoteResponse_Sound = new AssetReference("Story_11_Leviathan_001hb_EmoteResponse_Sound.prefab:90b2aae30ca06f648a8033e394c1fa22");
  private static readonly AssetReference Story_11_Leviathan_001hb_Loss_Sound = new AssetReference("Story_11_Leviathan_001hb_Loss_Sound.prefab:c8702d2dff96aa1468e091e19a0f99cb");
  private static readonly AssetReference Story_11_Leviathan_001hb_Rev_Sound = new AssetReference("Story_11_Leviathan_001hb_Rev_Sound.prefab:63e2a91d4276f1a429b25b7ddcddd53d");
  public static readonly AssetReference FaelinBrassRing = new AssetReference("SKN23-002_H_FAELIN_BrassRing_Quote.prefab:9984152d900140547aa7d721f3e49428");
  private List<string> m_NegativeReactionLines = new List<string>()
  {
    (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_01,
    (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_02,
    (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_03
  };
  private List<string> m_PositiveReactionLines = new List<string>()
  {
    (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_04,
    (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_05
  };
  private List<string> m_LossLines = new List<string>()
  {
    (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Loss_01,
    (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Loss_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Faelin_08() => this.m_gameOptions.AddBooleanOptions(BoH_Faelin_08.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Start_01,
      (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Start_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Start_02,
      (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8ExchangeA_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeA_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeB_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeC_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_02,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_03,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_04,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Reaction_05,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeD_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeE_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeF_01,
      (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8ExchangeF_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Victory_01,
      (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Victory_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Victory_02,
      (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Victory_02,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Loss_01,
      (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8_Loss_02,
      (string) BoH_Faelin_08.Story_11_Leviathan_001hb_Death_Sound,
      (string) BoH_Faelin_08.Story_11_Leviathan_001hb_EmoteResponse_Sound,
      (string) BoH_Faelin_08.Story_11_Leviathan_001hb_Loss_Sound,
      (string) BoH_Faelin_08.Story_11_Leviathan_001hb_Rev_Sound
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
    BoH_Faelin_08 boHFaelin08 = this;
    while (boHFaelin08.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeB_01);
        break;
      case 102:
        yield return (object) boHFaelin08.PlayLineInOrderOnce(friendlyActor, boHFaelin08.m_NegativeReactionLines);
        break;
      case 103:
        yield return (object) boHFaelin08.PlayLineInOrderOnce(friendlyActor, boHFaelin08.m_PositiveReactionLines);
        break;
      case 104:
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeD_01);
        break;
      case 105:
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeE_01);
        break;
      case 106:
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeF_01);
        yield return (object) boHFaelin08.MissionPlayVO(BoH_Faelin_08.FaelinBrassRing, (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8ExchangeF_01);
        break;
      case 107:
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Start_01);
        yield return (object) boHFaelin08.MissionPlayVO(BoH_Faelin_08.FaelinBrassRing, (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Start_01);
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Start_02);
        break;
      case 108:
        yield return (object) boHFaelin08.MissionPlayVO(BoH_Faelin_08.FaelinBrassRing, (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8ExchangeA_01);
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8ExchangeA_01);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Victory_01);
        yield return (object) boHFaelin08.MissionPlayVO(BoH_Faelin_08.FaelinBrassRing, (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Victory_01);
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, (string) BoH_Faelin_08.VO_Story_11_Ini_004hp_Female_MechaGnome_Story_Faelin_Mission8Victory_02);
        yield return (object) boHFaelin08.MissionPlayVO(BoH_Faelin_08.FaelinBrassRing, (string) BoH_Faelin_08.VO_Story_11_Faelin_000hp_Male_Nightborne_Story_Faelin_Mission8Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 515:
        yield return (object) boHFaelin08.MissionPlaySound(actor, (string) BoH_Faelin_08.Story_11_Leviathan_001hb_EmoteResponse_Sound);
        break;
      case 520:
        GameState.Get().SetBusy(true);
        yield return (object) boHFaelin08.MissionPlaySound(actor, (string) BoH_Faelin_08.Story_11_Leviathan_001hb_Loss_Sound);
        yield return (object) boHFaelin08.MissionPlayVO(friendlyActor, boHFaelin08.m_LossLines);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHFaelin08.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Faelin_08 boHFaelin08 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHFaelin08.\u003C\u003En__1(entity);
    while (boHFaelin08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHFaelin08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHFaelin08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHFaelin08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Faelin_08 boHFaelin08 = this;
    while (boHFaelin08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHFaelin08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHFaelin08.\u003C\u003En__2(entity);
      yield return (object) boHFaelin08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHFaelin08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Faelin_08 boHFaelin08 = this;
    while (boHFaelin08.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }
}
