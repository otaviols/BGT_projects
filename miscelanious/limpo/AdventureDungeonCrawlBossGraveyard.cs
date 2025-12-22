using Hearthstone.DungeonCrawl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class AdventureDungeonCrawlBossGraveyard : MonoBehaviour
{
  private const int MAX_GRAVEYARD_BOSSES_TO_SHOW = 8;
  private const float CHANCE_TO_PLAY_RARE_DEFEAT_LINE = 0.2f;
  [CustomEditField(Sections = "UI")]
  public NestedPrefab m_bossArchNestedPrefab;
  [CustomEditField(Sections = "UI")]
  public float m_bossArchSpacingHorizontal;
  [CustomEditField(Sections = "UI")]
  public float m_bossArchSpacingVertical;
  [CustomEditField(Sections = "UI")]
  public int m_bossesPerRow = 4;
  [CustomEditField(Sections = "UI")]
  public UberText m_defeatedCount;
  [CustomEditField(Sections = "Animations")]
  public string m_bossFlipSmallAnimName;
  [CustomEditField(Sections = "Animations")]
  public string m_bossFlipLargeAnimName;
  [CustomEditField(Sections = "Animations")]
  public string m_bossFlipNoDesaturateAnimName;
  [CustomEditField(Sections = "Animations")]
  public float m_delayPerBossFlip = 0.63f;
  [CustomEditField(Sections = "Animations")]
  public float m_delayAfterBossFlips = 1.5f;
  [CustomEditField(Sections = "Animations")]
  public float m_delayBeforeRunWinVO;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_bossFlipSmallSFX;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_bossFlipLargeSFX;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_victorySequenceStartSFX;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_defeatSequenceStartSFX;
  [CustomEditField(Sections = "Rewards")]
  public GameObject m_rewardPopupContainer;
  private List<AdventureDungeonCrawlBossGraveyardActor> m_bossArches = new List<AdventureDungeonCrawlBossGraveyardActor>();
  private Actor m_bossLostToActor;
  private bool m_subsceneTransitionComplete;
  private bool m_emoteLoadingComplete;
  private EmoteEntryDef m_bossLostToEmoteDef;
  private CardSoundSpell m_bossLostToEmoteSoundSpell;
  private bool m_runCompleteSequenceSeen;
  private List<AdventureHeroPowerDbfRecord> m_justUnlockedHeroPowers;
  private List<AdventureDeckDbfRecord> m_justUnlockedDecks;
  private List<AdventureLoadoutTreasuresDbfRecord> m_justUnlockedLoadoutTreasures;
  private List<AdventureLoadoutTreasuresDbfRecord> m_justUpgradedLoadoutTreasures;
  private IDungeonCrawlData m_dungeonCrawlData;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Start()
  {
    this.m_bossArches.Add(this.m_bossArchNestedPrefab.PrefabGameObject().GetComponent<AdventureDungeonCrawlBossGraveyardActor>());
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Update()
  {
    if (!Application.isEditor)
      return;
    this.UpdateLayout();
  }

  private void OnDestroy()
  {
    if (FullScreenFXMgr.Get() == null)
      return;
    this.m_screenEffectsHandle.StopEffect();
  }

  private void OnBonusChallengeUnlockObjectLoaded(Reward reward, object callbackData)
  {
    GameUtils.SetParent((Component) reward, this.m_rewardPopupContainer);
    reward.Show(false);
  }

  private IEnumerator PlayBossFlippingSequence(
    int numBossesToShow,
    bool defeatedLastBoss)
  {
    float flipDelayTime = this.m_delayPerBossFlip;
    for (int i = 0; i < numBossesToShow; ++i)
    {
      bool flag = i == numBossesToShow - 1;
      Animator component = this.m_bossArches[i].GetComponent<Animator>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        string stateName = !flag ? this.m_bossFlipSmallAnimName : (defeatedLastBoss ? this.m_bossFlipLargeAnimName : this.m_bossFlipNoDesaturateAnimName);
        component.Play(stateName);
        SoundManager.Get().LoadAndPlay((AssetReference) (flag ? this.m_bossFlipLargeSFX : this.m_bossFlipSmallSFX));
        yield return (object) new WaitForSeconds(flipDelayTime);
        flipDelayTime *= 0.9f;
        if ((double) flipDelayTime < 0.100000001490116)
          flipDelayTime = 0.1f;
      }
    }
    yield return (object) new WaitForSeconds(this.m_delayAfterBossFlips);
  }

  private IEnumerator PlayDefeatSequence(
    int numBossesToShow,
    int numDefeatedBosses,
    int numTotalBosses,
    int bossWhoDefeatedMeId,
    int heroDbId,
    GameSaveKeyId adventureServerKeyId,
    AdventureDungeonCrawlBossGraveyard.RunEndSequenceCompletedCallback completedCallback)
  {
    AdventureDungeonCrawlBossGraveyard crawlBossGraveyard = this;
    if ((UnityEngine.Object) crawlBossGraveyard.m_bossLostToActor == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlBossGraveyard.PlayDefeatSequence() - Can't PlayDefeatSequence() without a m_bossLostToActor!");
    }
    else
    {
      bool bossHasEmote = crawlBossGraveyard.LoadBossLostToEmote();
      if (!bossHasEmote)
        Log.Adventures.Print("No EmoteDef set for DUNGEON_CRAWL_DEFEAT_TAUNT for boss {0}.", (object) crawlBossGraveyard.m_bossLostToActor.CardDefName);
      while (!crawlBossGraveyard.m_subsceneTransitionComplete || GameUtils.IsAnyTransitionActive())
        yield return (object) null;
      ScreenEffectParameters parameters = new ScreenEffectParameters(ScreenEffectType.VIGNETTE, vignette: new VignetteParameters?(VignetteParameters.Default));
      crawlBossGraveyard.m_screenEffectsHandle.StartEffect(parameters);
      if (!string.IsNullOrEmpty(crawlBossGraveyard.m_defeatSequenceStartSFX))
        SoundManager.Get().LoadAndPlay((AssetReference) crawlBossGraveyard.m_defeatSequenceStartSFX);
      yield return (object) crawlBossGraveyard.StartCoroutine(crawlBossGraveyard.PlayBossFlippingSequence(numBossesToShow, false));
      if (bossHasEmote)
      {
        while (!crawlBossGraveyard.m_emoteLoadingComplete)
          yield return (object) null;
        Notification notification = NotificationManager.Get().CreateSpeechBubble(GameStrings.Get(crawlBossGraveyard.m_bossLostToEmoteDef.m_emoteGameStringKey), crawlBossGraveyard.m_bossLostToActor);
        if ((UnityEngine.Object) crawlBossGraveyard.m_bossLostToEmoteSoundSpell == (UnityEngine.Object) null)
        {
          NotificationManager.Get().DestroyNotification(notification, 5f);
        }
        else
        {
          crawlBossGraveyard.m_bossLostToEmoteSoundSpell.AddFinishedCallback((Spell.FinishedCallback) ((spell, data) =>
          {
            NotificationManager.Get().DestroyNotification(notification, 0.0f);
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bossLostToEmoteSoundSpell.gameObject);
          }));
          crawlBossGraveyard.m_bossLostToEmoteSoundSpell.Reactivate();
        }
        while ((UnityEngine.Object) notification != (UnityEngine.Object) null)
          yield return (object) null;
      }
      bool flag = false;
      WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(crawlBossGraveyard.m_dungeonCrawlData.GetMission());
      if (numDefeatedBosses >= numTotalBosses - 1)
        flag = DungeonCrawlSubDef_VOLines.PlayVOLine(crawlBossGraveyard.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, heroDbId, DungeonCrawlSubDef_VOLines.FINAL_BOSS_LOSS_EVENTS, bossWhoDefeatedMeId);
      if (!flag)
        DungeonCrawlSubDef_VOLines.PlayVOLine(crawlBossGraveyard.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, heroDbId, DungeonCrawlSubDef_VOLines.VOEventType.BOSS_LOSS_1, bossWhoDefeatedMeId);
      crawlBossGraveyard.m_screenEffectsHandle.StopEffect();
      if (crawlBossGraveyard.HasNewlyUnlockedGSDRewardsToShow())
        PopupDisplayManager.Get().RewardPopups.ShowRewardsForAdventureUnlocks(crawlBossGraveyard.m_justUnlockedHeroPowers, crawlBossGraveyard.m_justUnlockedDecks, crawlBossGraveyard.m_justUnlockedLoadoutTreasures, crawlBossGraveyard.m_justUpgradedLoadoutTreasures, (Action) (() =>
        {
          if (this.m_runCompleteSequenceSeen)
            return;
          this.MarkRunCompleteSequenceAsSeen(adventureServerKeyId, completedCallback);
        }));
    }
  }

  private static void PlayVictoryVO(
    IDungeonCrawlData dungeonCrawlData,
    bool hasCompletedAdventureWithAllClasses,
    bool hasSeenCompleteWithAllClassesFirstTime,
    bool firstTimeCompletedAsClass,
    int numClassesCompleted,
    int heroDbId)
  {
    AdventureDbId selectedAdventure = dungeonCrawlData.GetSelectedAdventure();
    GameSaveKeyId gameSaveClientKey = dungeonCrawlData.GetGameSaveClientKey();
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(dungeonCrawlData.GetMission());
    bool flag = false;
    if (hasCompletedAdventureWithAllClasses)
    {
      if (!hasSeenCompleteWithAllClassesFirstTime)
      {
        flag = flag || DungeonCrawlSubDef_VOLines.PlayVOLine(selectedAdventure, wingIdFromMissionId, heroDbId, DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_ALL_CLASSES_FIRST_TIME);
        GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(gameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_CLASSES_VO, new long[1]
        {
          1L
        }));
      }
      else
        flag = flag || DungeonCrawlSubDef_VOLines.PlayVOLine(selectedAdventure, wingIdFromMissionId, heroDbId, DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_ALL_CLASSES, allowRepeatDuringSession: false);
    }
    if (!flag & firstTimeCompletedAsClass && numClassesCompleted > 0)
    {
      int index = numClassesCompleted - 1;
      if (index < DungeonCrawlSubDef_VOLines.CLASS_COMPLETE_EVENTS.Length)
        flag = flag || DungeonCrawlSubDef_VOLines.PlayVOLine(selectedAdventure, wingIdFromMissionId, heroDbId, DungeonCrawlSubDef_VOLines.CLASS_COMPLETE_EVENTS[index], allowRepeatDuringSession: false);
    }
    if (!flag)
    {
      List<long> values;
      if (!GameSaveDataManager.Get().GetSubkeyValue(gameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_WING_COMPLETE_VO, out values))
        values = new List<long>();
      int num = values.Contains((long) wingIdFromMissionId) ? 1 : 0;
      int count = values.Count;
      if (num == 0 && count < DungeonCrawlSubDef_VOLines.WING_COMPLETE_EVENTS.Length)
      {
        flag = flag || DungeonCrawlSubDef_VOLines.PlayVOLine(selectedAdventure, wingIdFromMissionId, heroDbId, DungeonCrawlSubDef_VOLines.WING_COMPLETE_EVENTS[count], allowRepeatDuringSession: false);
        if (flag)
        {
          values.Add((long) wingIdFromMissionId);
          GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(gameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_WING_COMPLETE_VO, values.ToArray()));
        }
      }
    }
    if (!flag)
      flag = flag || DungeonCrawlSubDef_VOLines.PlayVOLine(selectedAdventure, wingIdFromMissionId, heroDbId, DungeonCrawlSubDef_VOLines.VOEventType.WING_COMPLETE_GENERAL, allowRepeatDuringSession: false);
    if (flag)
      return;
    AdventureWingDef wingDef = dungeonCrawlData.GetWingDef(wingIdFromMissionId);
    if (!AdventureUtils.CanPlayWingCompleteQuote(wingDef))
      return;
    string legacyAssetName = new AssetReference(wingDef.m_CompleteQuoteVOLine).GetLegacyAssetName();
    NotificationManager.Get().CreateCharacterQuote(wingDef.m_CompleteQuotePrefab, GameStrings.Get(legacyAssetName), wingDef.m_CompleteQuoteVOLine, false);
  }

  private IEnumerator PlayVictorySequence(
    int numBossesToShow,
    bool hasCompletedAdventureWithAllClasses,
    bool firstTimeCompletedAsClass,
    int numClassesCompleted,
    int heroDbId,
    AdventureDungeonCrawlBossGraveyard.RunEndSequenceCompletedCallback completedCallback)
  {
    AdventureDungeonCrawlBossGraveyard crawlBossGraveyard = this;
    while (!crawlBossGraveyard.m_subsceneTransitionComplete || GameUtils.IsAnyTransitionActive())
      yield return (object) null;
    ScreenEffectParameters parameters = new ScreenEffectParameters(ScreenEffectType.VIGNETTE, vignette: new VignetteParameters?(VignetteParameters.Default));
    crawlBossGraveyard.m_screenEffectsHandle.StartEffect(parameters);
    if (!string.IsNullOrEmpty(crawlBossGraveyard.m_victorySequenceStartSFX))
      SoundManager.Get().LoadAndPlay((AssetReference) crawlBossGraveyard.m_victorySequenceStartSFX);
    yield return (object) crawlBossGraveyard.StartCoroutine(crawlBossGraveyard.PlayBossFlippingSequence(numBossesToShow, true));
    AdventureDbId adventureDbId = crawlBossGraveyard.m_dungeonCrawlData.GetSelectedAdventure();
    AdventureModeDbId adventureModeDbId = crawlBossGraveyard.m_dungeonCrawlData.GetSelectedMode();
    AdventureDef adventureDef = crawlBossGraveyard.m_dungeonCrawlData.GetAdventureDef();
    AdventureSubDef adventureSubDef = (UnityEngine.Object) adventureDef == (UnityEngine.Object) null ? (AdventureSubDef) null : adventureDef.GetSubDef(adventureModeDbId);
    if ((UnityEngine.Object) adventureDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(adventureDef.m_BannerRewardPrefab))
    {
      TAG_CLASS classFromCardDbId = GameUtils.GetTagClassFromCardDbId(heroDbId);
      string text = GameStrings.FormatLocalizedString(adventureSubDef.GetCompleteBannerText(), (object) crawlBossGraveyard.GetClassNameFromTagClass(classFromCardDbId));
      BannerManager.Get().ShowBanner(adventureDef.m_BannerRewardPrefab, (string) null, text);
    }
    yield return (object) new WaitForSeconds(crawlBossGraveyard.m_delayBeforeRunWinVO);
    GameSaveKeyId gameSaveClientKey = crawlBossGraveyard.m_dungeonCrawlData.GetGameSaveClientKey();
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(gameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_CLASSES_VO, out num);
    bool hasSeenCompleteWithAllClassesFirstTime = num == 1L;
    AdventureDungeonCrawlBossGraveyard.PlayVictoryVO(crawlBossGraveyard.m_dungeonCrawlData, hasCompletedAdventureWithAllClasses, hasSeenCompleteWithAllClassesFirstTime, firstTimeCompletedAsClass, numClassesCompleted, heroDbId);
    crawlBossGraveyard.m_screenEffectsHandle.StopEffect();
    PopupDisplayManager.Get().RewardPopups.ShowRewardsForAdventureUnlocks(crawlBossGraveyard.m_justUnlockedHeroPowers, crawlBossGraveyard.m_justUnlockedDecks, crawlBossGraveyard.m_justUnlockedLoadoutTreasures, crawlBossGraveyard.m_justUpgradedLoadoutTreasures, (Action) (() => this.ShowAdditionalPopupsAfterOutstandingPopups(hasCompletedAdventureWithAllClasses, hasSeenCompleteWithAllClassesFirstTime, GameUtils.GetAdventureDataRecord((int) adventureDbId, (int) adventureModeDbId), completedCallback)));
  }

  private void MarkRunCompleteSequenceAsSeen(
    GameSaveKeyId adventureServerKeyId,
    AdventureDungeonCrawlBossGraveyard.RunEndSequenceCompletedCallback completedCallback)
  {
    this.m_runCompleteSequenceSeen = true;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(adventureServerKeyId, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_LATEST_DUNGEON_RUN_COMPLETE, new long[1]
    {
      1L
    }));
    completedCallback();
  }

  private void ShowAdditionalPopupsAfterOutstandingPopups(
    bool hasCompletedAdventureWithAllClasses,
    bool hasSeenCompleteWithAllClassesFirstTime,
    AdventureDataDbfRecord adventureDataRecord,
    AdventureDungeonCrawlBossGraveyard.RunEndSequenceCompletedCallback completedCallback)
  {
    if (hasCompletedAdventureWithAllClasses && !hasSeenCompleteWithAllClassesFirstTime)
    {
      string prefabShownOnComplete = adventureDataRecord.PrefabShownOnComplete;
      if (!string.IsNullOrEmpty(prefabShownOnComplete))
      {
        new BonusChallengeUnlockData(prefabShownOnComplete, adventureDataRecord.DungeonCrawlBossCardPrefab).LoadRewardObject((Reward.DelOnRewardLoaded) ((reward, data) =>
        {
          reward.RegisterHideListener((Reward.OnHideCallback) (userData =>
          {
            this.MarkRunCompleteSequenceAsSeen((GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey, completedCallback);
            Navigation.GoBack();
            this.m_dungeonCrawlData.SetSelectedAdventureMode((AdventureDbId) adventureDataRecord.AdventureId, AdventureModeDbId.BONUS_CHALLENGE);
          }));
          this.OnBonusChallengeUnlockObjectLoaded(reward, data);
        }));
        return;
      }
    }
    this.MarkRunCompleteSequenceAsSeen((GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey, completedCallback);
  }

  private string GetClassNameFromTagClass(TAG_CLASS deckClass)
  {
    List<GuestHero> currentAdventure = this.m_dungeonCrawlData.GetGuestHeroesForCurrentAdventure();
    if (currentAdventure.Count > 0)
    {
      foreach (GuestHero guestHero in currentAdventure)
      {
        GuestHero hero = guestHero;
        if (GameUtils.GetTagClassFromCardDbId(hero.cardDbId) == deckClass)
        {
          GuestHeroDbfRecord record = GameDbf.GuestHero.GetRecord((Predicate<GuestHeroDbfRecord>) (r => r.CardId == hero.cardDbId));
          if (record != null)
            return (string) record.Name;
        }
      }
    }
    return GameStrings.GetClassName(deckClass);
  }

  private void DisableBoss(Actor boss) => boss.transform.Rotate(new Vector3(0.0f, 0.0f, 180f));

  private bool LoadBossLostToEmote()
  {
    if (!this.m_bossLostToActor.HasCardDef || this.m_bossLostToActor.EmoteDefs == null)
    {
      Log.Adventures.PrintWarning("AdventureDungeonCrawlBossGraveyard.PlayDefeatSequence() - No cardDef found for the boss you lost to!");
      return false;
    }
    this.m_bossLostToEmoteDef = this.m_bossLostToActor.EmoteDefs.Find((Predicate<EmoteEntryDef>) (e => e.m_emoteType == EmoteType.DUNGEON_CRAWL_DEFEAT_TAUNT));
    if (this.m_bossLostToEmoteDef == null)
      return false;
    EmoteEntryDef emoteEntryDef = this.m_bossLostToActor.EmoteDefs.Find((Predicate<EmoteEntryDef>) (e => e.m_emoteType == EmoteType.DUNGEON_CRAWL_DEFEAT_TAUNT_SUPER_RARE));
    if (emoteEntryDef != null && (double) UnityEngine.Random.Range(0.0f, 1f) < 0.200000002980232)
      this.m_bossLostToEmoteDef = emoteEntryDef;
    AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_bossLostToEmoteDef.m_emoteSoundSpellPath, (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      this.m_emoteLoadingComplete = true;
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
        Log.Adventures.PrintError("AdventureDungeonCrawlBossGraveyard.PlayDefeatSequence() - Failed to load CardSoundSpell at path {0}!", (object) this.m_bossLostToEmoteDef.m_emoteSoundSpellPath);
      else
        this.m_bossLostToEmoteSoundSpell = go.GetComponent<CardSoundSpell>();
    }));
    return true;
  }

  private void UpdateLayout()
  {
    Vector3 localPosition = this.m_bossArches[0].transform.localPosition;
    for (int index = 1; index < this.m_bossArches.Count; ++index)
      this.m_bossArches[index].transform.localPosition = new Vector3(localPosition.x + this.m_bossArchSpacingHorizontal * (float) (index % this.m_bossesPerRow), localPosition.y, localPosition.z + this.m_bossArchSpacingVertical * (float) (index / this.m_bossesPerRow));
  }

  private void CheckForNewlyUnlockedAdventureRewards(
    GameSaveKeyId gameSaveServerKey,
    GameSaveKeyId gameSaveClientKey,
    int heroCardDbId)
  {
    List<GameSaveDataManager.SubkeySaveRequest> subkeySaveRequestList = new List<GameSaveDataManager.SubkeySaveRequest>();
    List<AdventureHeroPowerDbfRecord> dungeonCrawlHero1 = AdventureUtils.GetHeroPowersForDungeonCrawlHero(this.m_dungeonCrawlData, heroCardDbId);
    List<AdventureLoadoutTreasuresDbfRecord> dungeonCrawlHero2 = AdventureUtils.GetTreasuresForDungeonCrawlHero(this.m_dungeonCrawlData, heroCardDbId);
    this.m_justUnlockedHeroPowers = DungeonCrawlUtil.CheckForNewlyUnlockedGSDRewardsOfSpecificType(dungeonCrawlHero1.Cast<DbfRecord>(), this.m_dungeonCrawlData, gameSaveServerKey, gameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_AWARDED_HERO_POWERS, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_HERO_POWERS, subkeySaveRequestList).Cast<AdventureHeroPowerDbfRecord>().ToList<AdventureHeroPowerDbfRecord>();
    this.m_justUnlockedDecks = DungeonCrawlUtil.CheckForNewlyUnlockedGSDRewardsOfSpecificType(this.m_dungeonCrawlData.GetDecksForClass(AdventureUtils.GetHeroClassFromHeroId(heroCardDbId)).Cast<DbfRecord>(), this.m_dungeonCrawlData, gameSaveServerKey, gameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_AWARDED_DECKS, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_DECKS, subkeySaveRequestList).Cast<AdventureDeckDbfRecord>().ToList<AdventureDeckDbfRecord>();
    this.m_justUnlockedLoadoutTreasures = DungeonCrawlUtil.CheckForNewlyUnlockedGSDRewardsOfSpecificType(dungeonCrawlHero2.Cast<DbfRecord>(), this.m_dungeonCrawlData, gameSaveServerKey, gameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_AWARDED_LOADOUT_TREASURES, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_LOADOUT_TREASURES, subkeySaveRequestList).Cast<AdventureLoadoutTreasuresDbfRecord>().ToList<AdventureLoadoutTreasuresDbfRecord>();
    this.m_justUpgradedLoadoutTreasures = DungeonCrawlUtil.CheckForNewlyUnlockedGSDRewardsOfSpecificType(dungeonCrawlHero2.Cast<DbfRecord>(), this.m_dungeonCrawlData, gameSaveServerKey, gameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_AWARDED_LOADOUT_TREASURES, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_LOADOUT_TREASURES, subkeySaveRequestList, true).Cast<AdventureLoadoutTreasuresDbfRecord>().ToList<AdventureLoadoutTreasuresDbfRecord>();
    if (subkeySaveRequestList.Count <= 0)
      return;
    GameSaveDataManager.Get().SaveSubkeys(subkeySaveRequestList);
  }

  private bool HasNewlyUnlockedGSDRewardsToShow()
  {
    if (this.m_justUnlockedHeroPowers != null && this.m_justUnlockedHeroPowers.Count > 0 || this.m_justUnlockedDecks != null && this.m_justUnlockedDecks.Count > 0 || this.m_justUnlockedLoadoutTreasures != null && this.m_justUnlockedLoadoutTreasures.Count > 0)
      return true;
    return this.m_justUpgradedLoadoutTreasures != null && this.m_justUpgradedLoadoutTreasures.Count > 0;
  }

  public void OnSubSceneTransitionComplete() => this.m_subsceneTransitionComplete = true;

  public void ShowRunEnd(
    IDungeonCrawlData dungeonCrawlData,
    List<long> defeatedBossIds,
    long bossWhoDefeatedMeId,
    int numTotalBosses,
    bool hasCompletedAdventureWithAllClasses,
    bool firstTimeCompletedAsClass,
    int numClassesCompleted,
    int heroDbId,
    GameSaveKeyId adventureGameSaveServerKey,
    GameSaveKeyId adventureGameSaveClientKey,
    AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback loadCompletedCallback,
    AdventureDungeonCrawlBossGraveyard.RunEndSequenceCompletedCallback completedCallback)
  {
    if (dungeonCrawlData == null)
    {
      Log.Adventures.PrintError("Error!  AdventureDungeonCrawlBossGraveyard.ShowRunEnd() called with null dungeonCrawlData!)");
    }
    else
    {
      this.m_dungeonCrawlData = dungeonCrawlData;
      if (this.m_bossArches.Count < 1)
      {
        Log.Adventures.PrintError("Error!  AdventureDungeonCrawlBossGraveyard.ShowRunEnd() called when m_bossArches is empty! (Probably because Start() has not yet executed.)");
      }
      else
      {
        this.CheckForNewlyUnlockedAdventureRewards(adventureGameSaveServerKey, adventureGameSaveClientKey, heroDbId);
        int numDefeatedBosses = defeatedBossIds == null ? 0 : defeatedBossIds.Count;
        bool flag1 = numDefeatedBosses < numTotalBosses;
        int numBossesToShow = Mathf.Min(8, numDefeatedBosses + (flag1 ? 1 : 0));
        int num = Mathf.Max(0, numDefeatedBosses - numBossesToShow + (flag1 ? 1 : 0));
        this.m_defeatedCount.Text = GameStrings.Format("GLUE_ADVENTURE_DUNGEON_CRAWL_DEFEATED_COUNT", (object) numDefeatedBosses, (object) numTotalBosses);
        Actor bossArch1 = (Actor) this.m_bossArches[0];
        while (this.m_bossArches.Count < 8)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(bossArch1.gameObject);
          gameObject.transform.parent = bossArch1.transform.parent;
          gameObject.transform.localScale = bossArch1.transform.localScale;
          this.m_bossArches.Add(gameObject.GetComponent<AdventureDungeonCrawlBossGraveyardActor>());
        }
        for (int index = 0; index < this.m_bossArches.Count; ++index)
        {
          AdventureDungeonCrawlBossGraveyardActor bossArch2 = this.m_bossArches[index];
          bossArch2.SetStyle(this.m_dungeonCrawlData);
          this.DisableBoss((Actor) bossArch2);
        }
        for (int index1 = 0; index1 < numBossesToShow; ++index1)
        {
          int index2 = index1 + num;
          bool flag2 = index2 == numDefeatedBosses;
          long dbId = flag2 ? bossWhoDefeatedMeId : defeatedBossIds[index2];
          string cardId = GameUtils.TranslateDbIdToCardId((int) dbId);
          if (cardId == null)
          {
            Log.Adventures.PrintWarning("AdventureDungeonCrawlBossGraveyard.SetBossDbIds() - No cardId for boss dbId {0}, in arch number {1}!", (object) dbId, (object) index1);
          }
          else
          {
            using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardId))
            {
              this.m_bossArches[index1].SetFullDef(fullDef);
              this.m_bossArches[index1].SetPremium(TAG_PREMIUM.NORMAL);
              this.m_bossArches[index1].UpdateAllComponents();
              this.m_bossArches[index1].Show();
            }
            if (flag2)
            {
              this.m_bossLostToActor = (Actor) this.m_bossArches[index1];
            }
            else
            {
              Flipbook componentInChildren = this.m_bossArches[index1].GetComponentInChildren<Flipbook>(true);
              if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
                componentInChildren.gameObject.SetActive(true);
            }
          }
        }
        this.UpdateLayout();
        loadCompletedCallback();
        this.StopAllCoroutines();
        if (flag1)
        {
          if (!this.HasNewlyUnlockedGSDRewardsToShow())
            this.MarkRunCompleteSequenceAsSeen(adventureGameSaveServerKey, completedCallback);
          this.StartCoroutine(this.PlayDefeatSequence(numBossesToShow, numDefeatedBosses, numTotalBosses, (int) bossWhoDefeatedMeId, heroDbId, adventureGameSaveServerKey, completedCallback));
        }
        else
          this.StartCoroutine(this.PlayVictorySequence(numBossesToShow, hasCompletedAdventureWithAllClasses, firstTimeCompletedAsClass, numClassesCompleted, heroDbId, completedCallback));
      }
    }
  }

  public delegate void RunEndSequenceCompletedCallback();
}
