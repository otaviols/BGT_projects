using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIL_Dungeon : GIL_MissionEntity
{
  private static readonly string m_GennGreymane_BigQuote = "Greymane_BrassRing_Quote.prefab:3e16b31a3b009ad468fa76462c5eda3b";
  private static readonly string m_GennGreymane_Quote = "Greymane_Banner_Quote.prefab:cee4fc7a3f6bdd1439db34d534f85d5c";
  private static readonly AssetReference VO_GIL_692_Male_Worgen_TUT_FightBegin1Crowley_01 = new AssetReference("VO_GIL_692_Male_Worgen_TUT_FightBegin1Crowley_01.prefab:3352fbe060719c646a4b143495a2d04e");
  private static readonly AssetReference VO_GIL_692_Male_Worgen_TUT_FightBegin1Shaw_01 = new AssetReference("VO_GIL_692_Male_Worgen_TUT_FightBegin1Shaw_01.prefab:7f2afa4db1da44549b15a22e58b3d1c0");
  private static readonly AssetReference VO_GIL_692_Male_Worgen_TUT_FightBegin1Tess_01 = new AssetReference("VO_GIL_692_Male_Worgen_TUT_FightBegin1Tess_01.prefab:ce366f4649081ed42aec86a6291f14b4");
  private static readonly AssetReference VO_GIL_692_Male_Worgen_TUT_FightBegin1Tess_02 = new AssetReference("VO_GIL_692_Male_Worgen_TUT_FightBegin1Tess_02.prefab:81cc5d4027d42c04aa6e95ebca7d858a");
  private static readonly AssetReference VO_GIL_692_Male_Worgen_TUT_FightBegin1Toki_01 = new AssetReference("VO_GIL_692_Male_Worgen_TUT_FightBegin1Toki_01.prefab:7514dbda27d99a0418a8f764d0c07d26");
  private static readonly AssetReference VO_GIL_692_Male_Worgen_TUT_FightBegin2_01 = new AssetReference("VO_GIL_692_Male_Worgen_TUT_FightBegin2_01.prefab:1f5d9fa8502dfdc46ad78744f2f9ea57");
  private static readonly AssetReference VO_GIL_692_Male_Worgen_TUT_Defeat1_01 = new AssetReference("VO_GIL_692_Male_Worgen_TUT_Defeat1_01.prefab:6e4828354338f134fa115aff3c02fb85");
  private const AdventureDbId AdventureId = AdventureDbId.GIL;
  private GameSaveKeyId m_gameSaveDataClientKey;
  private long m_hasSeenInGameLoseVO;
  private long m_hasSeenInGameMulliganVO;
  private List<GameSaveKeySubkeyId> m_inGameSubkeysToSave = new List<GameSaveKeySubkeyId>();

  public GIL_Dungeon()
  {
    this.m_gameSaveDataClientKey = (GameSaveKeyId) GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => r.AdventureId == 423)).GameSaveDataClientKey;
    GameSaveDataManager.Get().GetSubkeyValue(this.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_VO, out this.m_hasSeenInGameLoseVO);
    GameSaveDataManager.Get().GetSubkeyValue(this.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_1_VO, out this.m_hasSeenInGameMulliganVO);
  }

  public override void PreloadAssets()
  {
    if (!Options.Get().GetBool(Option.HAS_SEEN_PLAYED_DARIUS))
      this.PreloadSound((string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Crowley_01);
    if (!Options.Get().GetBool(Option.HAS_SEEN_PLAYED_TESS))
    {
      this.PreloadSound((string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Tess_01);
      this.PreloadSound((string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Tess_02);
    }
    if (!Options.Get().GetBool(Option.HAS_SEEN_PLAYED_SHAW))
      this.PreloadSound((string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Shaw_01);
    if (!Options.Get().GetBool(Option.HAS_SEEN_PLAYED_TOKI))
      this.PreloadSound((string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Toki_01);
    if (this.m_hasSeenInGameMulliganVO == 0L)
      this.PreloadSound((string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin2_01);
    if (Options.Get().GetBool(Option.HAS_SEEN_LOOT_IN_GAME_LOSE_VO))
      return;
    this.PreloadSound((string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_Defeat1_01);
  }

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_GILMulligan);
  }

  public static GIL_Dungeon InstantiateGilDungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    string opposingHeroCardId = GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame);
    switch (opposingHeroCardId)
    {
      case "GILA_BOSS_20h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_20h();
      case "GILA_BOSS_21h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_21h();
      case "GILA_BOSS_22h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_22h();
      case "GILA_BOSS_23h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_23h();
      case "GILA_BOSS_24h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_24h();
      case "GILA_BOSS_25h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_25h();
      case "GILA_BOSS_26h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_26h();
      case "GILA_BOSS_27h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_27h();
      case "GILA_BOSS_29h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_29h();
      case "GILA_BOSS_30h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_30h();
      case "GILA_BOSS_31h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_31h();
      case "GILA_BOSS_32h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_32h();
      case "GILA_BOSS_33h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_33h();
      case "GILA_BOSS_34h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_34h();
      case "GILA_BOSS_35h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_35h();
      case "GILA_BOSS_36h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_36h();
      case "GILA_BOSS_37h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_37h();
      case "GILA_BOSS_38h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_38h();
      case "GILA_BOSS_39h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_39h();
      case "GILA_BOSS_40h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_40h();
      case "GILA_BOSS_41h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_41h();
      case "GILA_BOSS_42h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_42h();
      case "GILA_BOSS_43h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_43h();
      case "GILA_BOSS_44h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_44h();
      case "GILA_BOSS_45h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_45h();
      case "GILA_BOSS_46h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_46h();
      case "GILA_BOSS_47h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_47h();
      case "GILA_BOSS_48h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_48h();
      case "GILA_BOSS_49h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_49h();
      case "GILA_BOSS_50h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_50h();
      case "GILA_BOSS_51h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_51h();
      case "GILA_BOSS_52h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_52h();
      case "GILA_BOSS_52h2":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_52h();
      case "GILA_BOSS_54h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_54h();
      case "GILA_BOSS_55h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_55h();
      case "GILA_BOSS_56h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_56h();
      case "GILA_BOSS_57h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_57h();
      case "GILA_BOSS_58h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_58h();
      case "GILA_BOSS_59h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_59h();
      case "GILA_BOSS_60h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_60h();
      case "GILA_BOSS_61h":
        return (GIL_Dungeon) new GIL_Dungeon_Bonus_Boss_61h();
      case "GILA_BOSS_62h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_62h();
      case "GILA_BOSS_63h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_63h();
      case "GILA_BOSS_64h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_64h();
      case "GILA_BOSS_65h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_65h();
      case "GILA_BOSS_66h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_66h();
      case "GILA_BOSS_67h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_67h();
      case "GILA_BOSS_68h":
        return (GIL_Dungeon) new GIL_Dungeon_Boss_68h();
      default:
        Log.All.PrintError("GIL_Dungeon.InstantiateGILDungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) opposingHeroCardId);
        return new GIL_Dungeon();
    }
  }

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide())
      return;
    currentPlayer.GetHeroCard().HasActiveEmoteSound();
  }

  protected virtual List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected virtual string GetBossDeathLine() => (string) null;

  protected virtual bool GetShouldSupressDeathTextBubble() => false;

  protected virtual float ChanceToPlayBossHeroPowerVOLine() => 0.5f;

  protected override float ChanceToPlayRandomVOLine() => 0.5f;

  protected virtual void OnBossHeroPowerPlayed(Entity entity)
  {
    float bossHeroPowerVoLine = this.ChanceToPlayBossHeroPowerVOLine();
    float num = UnityEngine.Random.Range(0.0f, 1f);
    if (this.m_enemySpeaking || (double) bossHeroPowerVoLine < (double) num)
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    List<string> powerRandomLines = this.GetBossHeroPowerRandomLines();
    string soundPath = "";
    while (powerRandomLines.Count > 0)
    {
      int index = UnityEngine.Random.Range(0, powerRandomLines.Count);
      soundPath = powerRandomLines[index];
      powerRandomLines.RemoveAt(index);
      if (!NotificationManager.Get().HasSoundPlayedThisSession(soundPath))
        break;
    }
    if (soundPath == "")
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeechOnce(soundPath, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    GIL_Dungeon gilDungeon = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (gilDungeon.m_enemySpeaking || entity.GetCardType() == TAG_CARDTYPE.INVALID || entity.GetCardType() != TAG_CARDTYPE.HERO_POWER || entity.GetControllerSide() != Player.Side.OPPOSING)
      return false;
    gilDungeon.OnBossHeroPowerPlayed(entity);
    return false;
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    GIL_Dungeon gilDungeon = this;
    if (turn == 1 && GameState.Get() != null && GameState.Get().GetFriendlySidePlayer() != null && GameState.Get().GetFriendlySidePlayer().GetHero() != null)
    {
      bool hasPlayedLineThisGame = false;
      string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
      if (!(cardId == "GILA_500h3"))
      {
        if (!(cardId == "GILA_600h"))
        {
          if (!(cardId == "GILA_400h"))
          {
            if (cardId == "GILA_900h" && !Options.Get().GetBool(Option.HAS_SEEN_PLAYED_TOKI))
            {
              Options.Get().SetBool(Option.HAS_SEEN_PLAYED_TOKI, true);
              hasPlayedLineThisGame = true;
              yield return (object) gilDungeon.PlayBossLine(GIL_Dungeon.m_GennGreymane_BigQuote, (string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Toki_01);
            }
          }
          else if (!Options.Get().GetBool(Option.HAS_SEEN_PLAYED_SHAW))
          {
            Options.Get().SetBool(Option.HAS_SEEN_PLAYED_SHAW, true);
            hasPlayedLineThisGame = true;
            yield return (object) gilDungeon.PlayBossLine(GIL_Dungeon.m_GennGreymane_BigQuote, (string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Shaw_01);
          }
        }
        else if (!Options.Get().GetBool(Option.HAS_SEEN_PLAYED_DARIUS))
        {
          Options.Get().SetBool(Option.HAS_SEEN_PLAYED_DARIUS, true);
          hasPlayedLineThisGame = true;
          yield return (object) gilDungeon.PlayBossLine(GIL_Dungeon.m_GennGreymane_BigQuote, (string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Crowley_01);
        }
      }
      else if (!Options.Get().GetBool(Option.HAS_SEEN_PLAYED_TESS))
      {
        Options.Get().SetBool(Option.HAS_SEEN_PLAYED_TESS, true);
        hasPlayedLineThisGame = true;
        yield return (object) gilDungeon.PlayBossLine(GIL_Dungeon.m_GennGreymane_BigQuote, (string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Tess_01);
        yield return (object) gilDungeon.PlayBossLine(GIL_Dungeon.m_GennGreymane_BigQuote, (string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin1Tess_02);
      }
      if (gilDungeon.m_hasSeenInGameMulliganVO == 0L && !hasPlayedLineThisGame)
      {
        gilDungeon.m_inGameSubkeysToSave.Add(GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_1_VO);
        yield return (object) gilDungeon.PlayBossLine(GIL_Dungeon.m_GennGreymane_BigQuote, (string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_FightBegin2_01);
      }
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    GIL_Dungeon gilDungeon = this;
    List<GameSaveDataManager.SubkeySaveRequest> requests = new List<GameSaveDataManager.SubkeySaveRequest>();
    foreach (GameSaveKeySubkeyId subkey in gilDungeon.m_inGameSubkeysToSave)
      requests.Add(new GameSaveDataManager.SubkeySaveRequest(gilDungeon.m_gameSaveDataClientKey, subkey, new long[1]
      {
        1L
      }));
    if (requests.Count > 0)
      GameSaveDataManager.Get().SaveSubkeys(requests);
    if (gameResult == TAG_PLAYSTATE.LOST && gilDungeon.m_hasSeenInGameLoseVO == 0L)
    {
      yield return (object) new WaitForSeconds(5f);
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(gilDungeon.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_VO, new long[1]
      {
        1L
      }));
      yield return (object) Gameplay.Get().StartCoroutine(gilDungeon.PlayCharacterQuoteAndWait(GIL_Dungeon.m_GennGreymane_Quote, (string) GIL_Dungeon.VO_GIL_692_Male_Worgen_TUT_Defeat1_01));
    }
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string bossDeathLine = this.GetBossDeathLine();
    if (this.m_enemySpeaking && !string.IsNullOrEmpty(bossDeathLine) || gameResult != TAG_PLAYSTATE.WON)
      return;
    if (this.GetShouldSupressDeathTextBubble())
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(bossDeathLine, Notification.SpeechBubbleDirection.None, actor));
    else
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(bossDeathLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  public override void NotifyOfResetGameFinished(Entity source, Entity oldGameEntity)
  {
    base.NotifyOfResetGameFinished(source, oldGameEntity);
    if (!(oldGameEntity is GIL_Dungeon gilDungeon))
      return;
    this.m_inGameSubkeysToSave = new List<GameSaveKeySubkeyId>((IEnumerable<GameSaveKeySubkeyId>) gilDungeon.m_inGameSubkeysToSave);
  }
}
