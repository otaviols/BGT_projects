using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOT_Dungeon : LOOT_MissionEntity
{
  private static readonly string m_KingTogwaggle_BigQuote = "KingTogwaggle_BigQuote.prefab:9416c71ab37ae184b8c4bfaaf3233882";
  private static readonly string m_KingTogwaggle_Quote = "KingTogwaggle_Quote.prefab:b20f7b1314c0a2d46a9de0e48e7ae6f5";
  private static readonly AssetReference VO_LOOT_541_Male_Kobold_TUT_Game1Begin1_01 = new AssetReference("VO_LOOT_541_Male_Kobold_TUT_Game1Begin1_01.prefab:4eb8b422c051369409d10b42a777ee1d");
  private static readonly AssetReference VO_LOOT_541_Male_Kobold_TUT_Game1Defeat_01 = new AssetReference("VO_LOOT_541_Male_Kobold_TUT_Game1Defeat_01.prefab:c16731bb4687d154a9a22c8e01eeabb2");
  private static readonly AssetReference VO_LOOT_541_Male_Kobold_TUT_Game1Victory_01 = new AssetReference("VO_LOOT_541_Male_Kobold_TUT_Game1Victory_01.prefab:bf40d91ec399f7e4c88b3a4c8c71bda5");
  private static readonly AssetReference VO_LOOT_541_Male_Kobold_TUT_Game2Begin1_01 = new AssetReference("VO_LOOT_541_Male_Kobold_TUT_Game2Begin1_01.prefab:7fcfd8f32efb5374aae312892eac84ff");
  private static readonly AssetReference VO_LOOT_541_Male_Kobold_TUT_Game2Begin2_01 = new AssetReference("VO_LOOT_541_Male_Kobold_TUT_Game2Begin2_01.prefab:90b80890ad0f972478294383cc02e233");
  private static readonly AssetReference VO_LOOT_541_Male_Kobold_TUT_GeneralDefeat1_01 = new AssetReference("VO_LOOT_541_Male_Kobold_TUT_GeneralDefeat1_01.prefab:9bb4ec22f68d90342a84fba2f3d7a100");
  private static readonly AssetReference VO_LOOT_541_Male_Kobold_TUT_GeneralDefeat2_01 = new AssetReference("VO_LOOT_541_Male_Kobold_TUT_GeneralDefeat2_01.prefab:6dfdf8edb59d4c14380b38e874769662");
  private static readonly AssetReference VO_LOOTA_829_Male_Human_Event_01 = new AssetReference("VO_LOOTA_829_Male_Human_Event_01.prefab:beb8a7cd19bc24f46a617b0c1774da48");
  private GameSaveKeyId m_gameSaveDataClientKey;
  private long m_hasSeenInGameWinVO;
  private long m_hasSeenInGameLoseVO;
  private long m_hasSeenInGameLose2VO;
  private long m_hasSeenInGameMulliganVO;
  private long m_hasSeenInGameMulligan2VO;
  private List<GameSaveKeySubkeyId> m_inGameSubkeysToSave = new List<GameSaveKeySubkeyId>();

  public LOOT_Dungeon()
  {
    this.m_gameSaveDataClientKey = (GameSaveKeyId) GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => r.AdventureId == 414)).GameSaveDataClientKey;
    GameSaveDataManager.Get().GetSubkeyValue(this.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_WIN_VO, out this.m_hasSeenInGameWinVO);
    GameSaveDataManager.Get().GetSubkeyValue(this.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_VO, out this.m_hasSeenInGameLoseVO);
    GameSaveDataManager.Get().GetSubkeyValue(this.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_2_VO, out this.m_hasSeenInGameLose2VO);
    GameSaveDataManager.Get().GetSubkeyValue(this.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_1_VO, out this.m_hasSeenInGameMulliganVO);
    GameSaveDataManager.Get().GetSubkeyValue(this.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_2_VO, out this.m_hasSeenInGameMulligan2VO);
  }

  public override void PreloadAssets()
  {
    this.PreloadSound((string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game1Victory_01);
    this.PreloadSound((string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game1Defeat_01);
    this.PreloadSound((string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_GeneralDefeat1_01);
    this.PreloadSound((string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_GeneralDefeat2_01);
    this.PreloadSound((string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game1Begin1_01);
    this.PreloadSound((string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game2Begin1_01);
    this.PreloadSound((string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game2Begin2_01);
    this.PreloadSound((string) LOOT_Dungeon.VO_LOOTA_829_Male_Human_Event_01);
  }

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_LOOTMulligan);
  }

  public static LOOT_Dungeon InstantiateLootDungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    string opposingHeroCardId = GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame);
    switch (opposingHeroCardId)
    {
      case "LOOTA_BOSS_04h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_04h();
      case "LOOTA_BOSS_05h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_05h();
      case "LOOTA_BOSS_06h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_06h();
      case "LOOTA_BOSS_09h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_09h();
      case "LOOTA_BOSS_10h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_10h();
      case "LOOTA_BOSS_11h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_11h();
      case "LOOTA_BOSS_12h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_12h();
      case "LOOTA_BOSS_13h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_13h();
      case "LOOTA_BOSS_15h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_15h();
      case "LOOTA_BOSS_16h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_16h();
      case "LOOTA_BOSS_17h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_17h();
      case "LOOTA_BOSS_18h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_18h();
      case "LOOTA_BOSS_19h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_19h();
      case "LOOTA_BOSS_20h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_20h();
      case "LOOTA_BOSS_21h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_21h();
      case "LOOTA_BOSS_22h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_22h();
      case "LOOTA_BOSS_23h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_23h();
      case "LOOTA_BOSS_24h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_24h();
      case "LOOTA_BOSS_25h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_25h();
      case "LOOTA_BOSS_26h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_26h();
      case "LOOTA_BOSS_27h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_04h();
      case "LOOTA_BOSS_28h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_05h();
      case "LOOTA_BOSS_29h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_06h();
      case "LOOTA_BOSS_30h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_11h();
      case "LOOTA_BOSS_31h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_12h();
      case "LOOTA_BOSS_32h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_15h();
      case "LOOTA_BOSS_33h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_33h();
      case "LOOTA_BOSS_34h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_34h();
      case "LOOTA_BOSS_35h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_35h();
      case "LOOTA_BOSS_36h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_36h();
      case "LOOTA_BOSS_37h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_37h();
      case "LOOTA_BOSS_38h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_38h();
      case "LOOTA_BOSS_39h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_39h();
      case "LOOTA_BOSS_40h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_40h();
      case "LOOTA_BOSS_41h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_41h();
      case "LOOTA_BOSS_42h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_42h();
      case "LOOTA_BOSS_43h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_43h();
      case "LOOTA_BOSS_44h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_44h();
      case "LOOTA_BOSS_45h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_45h();
      case "LOOTA_BOSS_46h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_46h();
      case "LOOTA_BOSS_47h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_47h();
      case "LOOTA_BOSS_48h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_48h();
      case "LOOTA_BOSS_49h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_49h();
      case "LOOTA_BOSS_50h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_50h();
      case "LOOTA_BOSS_51h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_51h();
      case "LOOTA_BOSS_52h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_52h();
      case "LOOTA_BOSS_53h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_53h();
      case "LOOTA_BOSS_53h2":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_53h();
      case "LOOTA_BOSS_54h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_54h();
      case "LOOTA_BOSS_99h":
        return (LOOT_Dungeon) new LOOT_Dungeon_BOSS_99h();
      default:
        Log.All.PrintError("LOOT_Dungeon.InstantiateLootDungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) opposingHeroCardId);
        return new LOOT_Dungeon();
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

  protected virtual void OnBossHeroPowerPlayed(Entity entity)
  {
    if ((double) this.ChanceToPlayBossHeroPowerVOLine() < (double) UnityEngine.Random.Range(0.0f, 1f))
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
    LOOT_Dungeon lootDungeon = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (lootDungeon.m_enemySpeaking || entity.GetCardType() == TAG_CARDTYPE.INVALID || entity.GetCardType() != TAG_CARDTYPE.HERO_POWER || entity.GetControllerSide() != Player.Side.OPPOSING)
      return false;
    lootDungeon.OnBossHeroPowerPlayed(entity);
    return false;
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LOOT_Dungeon lootDungeon = this;
    if (turn == 1)
    {
      if (lootDungeon.m_hasSeenInGameMulliganVO == 0L && lootDungeon.m_hasSeenInGameMulligan2VO == 0L)
      {
        lootDungeon.m_inGameSubkeysToSave.Add(GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_1_VO);
        yield return (object) lootDungeon.PlayBigCharacterQuoteAndWait(LOOT_Dungeon.m_KingTogwaggle_BigQuote, (string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game1Begin1_01);
      }
      else if (lootDungeon.m_hasSeenInGameMulliganVO > 0L && lootDungeon.m_hasSeenInGameMulligan2VO == 0L)
      {
        lootDungeon.m_inGameSubkeysToSave.Add(GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_2_VO);
        yield return (object) lootDungeon.PlayBigCharacterQuoteAndWait(LOOT_Dungeon.m_KingTogwaggle_BigQuote, (string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game2Begin1_01);
        yield return (object) lootDungeon.PlayBigCharacterQuoteAndWait(LOOT_Dungeon.m_KingTogwaggle_BigQuote, (string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game2Begin2_01);
      }
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    LOOT_Dungeon lootDungeon = this;
    List<GameSaveDataManager.SubkeySaveRequest> requests = new List<GameSaveDataManager.SubkeySaveRequest>();
    foreach (GameSaveKeySubkeyId subkey in lootDungeon.m_inGameSubkeysToSave)
      requests.Add(new GameSaveDataManager.SubkeySaveRequest(lootDungeon.m_gameSaveDataClientKey, subkey, new long[1]
      {
        1L
      }));
    if (requests.Count > 0)
      GameSaveDataManager.Get().SaveSubkeys(requests);
    if (gameResult == TAG_PLAYSTATE.WON && lootDungeon.m_hasSeenInGameWinVO == 0L)
    {
      yield return (object) new WaitForSeconds(5f);
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(lootDungeon.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_WIN_VO, new long[1]
      {
        1L
      }));
      yield return (object) Gameplay.Get().StartCoroutine(lootDungeon.PlayCharacterQuoteAndWait(LOOT_Dungeon.m_KingTogwaggle_Quote, (string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game1Victory_01));
    }
    if (gameResult == TAG_PLAYSTATE.LOST)
    {
      if (lootDungeon.m_hasSeenInGameLoseVO == 0L && lootDungeon.m_hasSeenInGameLose2VO == 0L)
      {
        yield return (object) new WaitForSeconds(5f);
        GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(lootDungeon.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_VO, new long[1]
        {
          1L
        }));
        yield return (object) Gameplay.Get().StartCoroutine(lootDungeon.PlayCharacterQuoteAndWait(LOOT_Dungeon.m_KingTogwaggle_Quote, (string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_Game1Defeat_01));
      }
      else if (lootDungeon.m_hasSeenInGameLoseVO > 0L && lootDungeon.m_hasSeenInGameLose2VO == 0L)
      {
        yield return (object) new WaitForSeconds(5f);
        GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(lootDungeon.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_2_VO, new long[1]
        {
          1L
        }));
        yield return (object) Gameplay.Get().StartCoroutine(lootDungeon.PlayCharacterQuoteAndWait(LOOT_Dungeon.m_KingTogwaggle_Quote, (string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_GeneralDefeat1_01));
        yield return (object) Gameplay.Get().StartCoroutine(lootDungeon.PlayCharacterQuoteAndWait(LOOT_Dungeon.m_KingTogwaggle_Quote, (string) LOOT_Dungeon.VO_LOOT_541_Male_Kobold_TUT_GeneralDefeat2_01));
      }
    }
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string bossDeathLine = this.GetBossDeathLine();
    if (this.m_enemySpeaking && !string.IsNullOrEmpty(bossDeathLine) || gameResult != TAG_PLAYSTATE.WON)
      return;
    if (bossDeathLine == "VO_LOOTA_BOSS_51h_Male_Dwarf_Death_01.prefab:e5c8b619095374542bac028ed3654007")
      this.PlaySound("RussellTheBard_Death_Underlay.prefab:8d76a143441379e40a36cb5b7c84b9b9");
    if (this.GetShouldSupressDeathTextBubble())
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(bossDeathLine, Notification.SpeechBubbleDirection.None, actor));
    else
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(bossDeathLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  private Actor GetEnemyLoyalSidekickActor()
  {
    foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2) == 1000 && entity.GetCardId() == "LOOTA_829")
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

  public IEnumerator PlayLoyalSideKickBetrayal(int missionEvent)
  {
    LOOT_Dungeon lootDungeon = this;
    if (missionEvent == 1000)
    {
      Actor loyalSideKick = lootDungeon.GetEnemyLoyalSidekickActor();
      yield return (object) lootDungeon.WaitForEntitySoundToFinish(loyalSideKick.GetEntity());
      yield return (object) lootDungeon.PlayLineOnlyOnce(loyalSideKick, (string) LOOT_Dungeon.VO_LOOTA_829_Male_Human_Event_01);
      loyalSideKick = (Actor) null;
    }
  }
}
