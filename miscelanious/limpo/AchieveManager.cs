using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Progression;
using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AchieveManager : IService, IHasUpdate
{
  private static readonly long TIMED_ACHIEVE_VALIDATION_DELAY_TICKS = 600000000;
  private static readonly long CHECK_LICENSE_ADDED_ACHIEVE_DELAY_TICKS = 3000000000;
  private static readonly long TIMED_AND_LICENSE_ACHIEVE_CHECK_DELAY_TICKS = Math.Min(AchieveManager.TIMED_ACHIEVE_VALIDATION_DELAY_TICKS, AchieveManager.CHECK_LICENSE_ADDED_ACHIEVE_DELAY_TICKS);
  private Map<int, Achievement> m_achievements = new Map<int, Achievement>();
  private bool m_allNetAchievesReceived;
  private int m_numEventResponsesNeeded;
  private HashSet<int> m_achieveValidationsToRequest = new HashSet<int>();
  private HashSet<int> m_achieveValidationsRequested = new HashSet<int>();
  private HashSet<int> m_achievesSeenByPlayerThisSession = new HashSet<int>();
  private bool m_disableCancelButtonUntilServerReturns;
  private Map<int, long> m_lastEventTimingValidationByAchieve = new Map<int, long>();
  private Map<int, long> m_lastCheckLicenseAddedByAchieve = new Map<int, long>();
  private long m_lastEventTimingAndLicenseAchieveCheck;
  private bool m_queueNotifications;
  private List<int> m_achieveNotificationsToQueue = new List<int>();
  private List<AchievementNotification> m_blockedAchievementNotifications = new List<AchievementNotification>();
  private List<AchieveManager.AchieveCanceledListener> m_achieveCanceledListeners = new List<AchieveManager.AchieveCanceledListener>();
  private List<AchieveManager.AchievesUpdatedListener> m_achievesUpdatedListeners = new List<AchieveManager.AchievesUpdatedListener>();
  private List<AchieveManager.LicenseAddedAchievesUpdatedListener> m_licenseAddedAchievesUpdatedListeners = new List<AchieveManager.LicenseAddedAchievesUpdatedListener>();

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AchieveManager achieveManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new System.Action(achieveManager.WillReset);
    HearthstoneApplication.Get().Resetting += new System.Action(achieveManager.OnReset);
    achieveManager.LoadAchievesFromDBF();
    Network network = serviceLocator.Get<Network>();
    network.RegisterNetHandler((object) CancelQuestResponse.PacketID.ID, new Network.NetHandler(achieveManager.OnQuestCanceled));
    network.RegisterNetHandler((object) ValidateAchieveResponse.PacketID.ID, new Network.NetHandler(achieveManager.OnAchieveValidated));
    network.RegisterNetHandler((object) TriggerEventResponse.PacketID.ID, new Network.NetHandler(achieveManager.OnEventTriggered));
    network.RegisterNetHandler((object) PegasusUtil.AccountLicenseAchieveResponse.PacketID.ID, new Network.NetHandler(achieveManager.OnAccountLicenseAchieveResponse));
    serviceLocator.Get<NetCache>().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(achieveManager.OnNewNotices));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[4]
  {
    typeof (Network),
    typeof (NetCache),
    typeof (GameDbf),
    typeof (SpecialEventManager)
  };

  public void Shutdown()
  {
  }

  private void WillReset()
  {
    this.m_allNetAchievesReceived = false;
    this.m_achieveValidationsToRequest.Clear();
    this.m_achieveValidationsRequested.Clear();
    this.m_achievesUpdatedListeners.Clear();
    this.m_lastEventTimingValidationByAchieve.Clear();
    this.m_lastCheckLicenseAddedByAchieve.Clear();
    this.m_licenseAddedAchievesUpdatedListeners.Clear();
    this.m_achievements.Clear();
  }

  private void OnReset() => this.LoadAchievesFromDBF();

  public static AchieveManager Get() => ServiceManager.Get<AchieveManager>();

  public static bool IsPredicateTrue(Assets.Achieve.AltTextPredicate predicate) => predicate == Assets.Achieve.AltTextPredicate.CAN_SEE_WILD && CollectionManager.Get() != null && CollectionManager.Get().ShouldAccountSeeStandardWild();

  public void InitAchieveManager()
  {
    this.WillReset();
    this.LoadAchievesFromDBF();
  }

  public bool IsReady() => this.m_allNetAchievesReceived && this.m_numEventResponsesNeeded <= 0 && this.m_achieveValidationsToRequest.Count <= 0 && this.m_achieveValidationsRequested.Count <= 0 && NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() != null;

  public bool RegisterAchievesUpdatedListener(
    AchieveManager.AchievesUpdatedCallback callback,
    object userData = null)
  {
    if (callback == null)
      return false;
    AchieveManager.AchievesUpdatedListener achievesUpdatedListener = new AchieveManager.AchievesUpdatedListener();
    achievesUpdatedListener.SetCallback(callback);
    achievesUpdatedListener.SetUserData(userData);
    if (this.m_achievesUpdatedListeners.Contains(achievesUpdatedListener))
      return false;
    this.m_achievesUpdatedListeners.Add(achievesUpdatedListener);
    return true;
  }

  public bool RemoveAchievesUpdatedListener(AchieveManager.AchievesUpdatedCallback callback) => this.RemoveAchievesUpdatedListener(callback, (object) null);

  public bool RemoveAchievesUpdatedListener(
    AchieveManager.AchievesUpdatedCallback callback,
    object userData)
  {
    if (callback == null)
      return false;
    AchieveManager.AchievesUpdatedListener achievesUpdatedListener = new AchieveManager.AchievesUpdatedListener();
    achievesUpdatedListener.SetCallback(callback);
    achievesUpdatedListener.SetUserData(userData);
    if (!this.m_achievesUpdatedListeners.Contains(achievesUpdatedListener))
      return false;
    this.m_achievesUpdatedListeners.Remove(achievesUpdatedListener);
    return true;
  }

  public List<Achievement> GetNewCompletedAchievesToShow()
  {
    List<Achievement> completedAchievesToShow = new List<Achievement>();
    QuestManager questManager = QuestManager.Get();
    foreach (KeyValuePair<int, Achievement> achievement1 in this.m_achievements)
    {
      Achievement achievement2 = achievement1.Value;
      if (achievement2.IsNewlyCompleted() && !achievement2.IsInternal() && achievement2.RewardTiming != Assets.Achieve.RewardTiming.NEVER)
      {
        switch (achievement2.AchieveType)
        {
          case Assets.Achieve.Type.HERO:
          case Assets.Achieve.Type.GOLDHERO:
          case Assets.Achieve.Type.DAILY_REPEATABLE:
            continue;
          default:
            if (!achievement2.IsGenericRewardChest && (questManager == null || !questManager.IsProxyLegacyAchieve(achievement2.ID)))
            {
              completedAchievesToShow.Add(achievement2);
              continue;
            }
            continue;
        }
      }
    }
    return completedAchievesToShow;
  }

  private static bool IsActiveQuest(Achievement obj, bool onlyNewlyActive)
  {
    if (!obj.Active || !obj.CanShowInQuestLog)
      return false;
    return !onlyNewlyActive || obj.IsNewlyActive();
  }

  private static bool IsAutoDestroyQuest(Achievement obj) => obj.CanShowInQuestLog && obj.AutoDestroy;

  private static bool IsDialogQuest(Achievement obj) => obj.CanShowInQuestLog && obj.QuestDialogId != 0;

  public List<Achievement> GetActiveQuests(bool onlyNewlyActive = false)
  {
    List<Achievement> activeQuests = new List<Achievement>();
    foreach (KeyValuePair<int, Achievement> achievement1 in this.m_achievements)
    {
      Achievement achievement2 = achievement1.Value;
      if (AchieveManager.IsActiveQuest(achievement2, onlyNewlyActive))
        activeQuests.Add(achievement2);
    }
    return activeQuests;
  }

  public bool HasQuestsToShow(bool onlyNewlyActive = false)
  {
    bool show = false;
    foreach (KeyValuePair<int, Achievement> achievement in this.m_achievements)
    {
      if (AchieveManager.IsActiveQuest(achievement.Value, false) && (achievement.Value.IsNewlyActive() || achievement.Value.AutoDestroy))
      {
        show = true;
        break;
      }
    }
    return show;
  }

  public bool MarkQuestAsSeenByPlayerThisSession(Achievement obj) => this.m_achievesSeenByPlayerThisSession.Add(obj.ID);

  public bool ResetQuestSeenByPlayerThisSession(Achievement obj) => this.m_achievesSeenByPlayerThisSession.Remove(obj.ID);

  public bool HasActiveAutoDestroyQuests()
  {
    foreach (KeyValuePair<int, Achievement> achievement in this.m_achievements)
    {
      if (AchieveManager.IsActiveQuest(achievement.Value, false) && AchieveManager.IsAutoDestroyQuest(achievement.Value))
        return true;
    }
    return false;
  }

  public bool HasActiveUnseenWelcomeQuestDialog()
  {
    int num = Options.Get().GetInt(Option.LATEST_SEEN_WELCOME_QUEST_DIALOG);
    foreach (KeyValuePair<int, Achievement> achievement1 in this.m_achievements)
    {
      Achievement achievement2 = achievement1.Value;
      if (AchieveManager.IsActiveQuest(achievement2, false) && AchieveManager.IsDialogQuest(achievement2) && num != achievement2.ID)
        return true;
    }
    return false;
  }

  public List<Achievement> GetNewlyProgressedQuests() => AchieveManager.Get().GetActiveQuests().FindAll((Predicate<Achievement>) (obj => obj.AcknowledgedProgress < obj.Progress && obj.Progress > 0 && obj.Progress < obj.MaxProgress));

  public bool HasUnlockedFeature(Assets.Achieve.Unlocks feature)
  {
    if (DemoMgr.Get().ArenaIs1WinMode() && feature == Assets.Achieve.Unlocks.FORGE)
      return true;
    if (feature == Assets.Achieve.Unlocks.VANILLA_HEROES && AchievementManager.Get() != null)
      return this.HasUnlockedDefaultHeroes();
    Achievement achievement1 = (Achievement) null;
    foreach (KeyValuePair<int, Achievement> achievement2 in this.m_achievements)
    {
      if (achievement2.Value.UnlockedFeature == feature)
      {
        achievement1 = achievement2.Value;
        break;
      }
    }
    if (achievement1 != null)
      return achievement1.IsCompleted();
    Debug.LogWarning((object) string.Format("AchieveManager.HasUnlockedFeature(): could not find achieve that unlocks feature {0}", (object) feature));
    return false;
  }

  public bool HasUnlockedDefaultHeroes()
  {
    int index = 0;
    for (int length = GameUtils.DEFAULT_HERO_CLASSES.Length; index < length; ++index)
    {
      NetCache.HeroLevel heroLevel = GameUtils.GetHeroLevel(GameUtils.DEFAULT_HERO_CLASSES[index]);
      if (heroLevel == null || heroLevel.CurrentLevel.Level == 0)
        return false;
    }
    return true;
  }

  public bool HasUnlockedArena() => this.HasUnlockedDefaultHeroes();

  public Achievement GetAchievement(int achieveID) => !this.m_achievements.ContainsKey(achieveID) ? (Achievement) null : this.m_achievements[achieveID];

  public IEnumerable<Achievement> GetCompletedAchieves() => this.GetAchieves((Func<Achievement, bool>) (a => a.IsCompleted()));

  public List<Achievement> GetAchievesInGroup(Assets.Achieve.Type achieveGroup) => new List<Achievement>((IEnumerable<Achievement>) this.m_achievements.Values).FindAll((Predicate<Achievement>) (obj => obj.AchieveType == achieveGroup));

  public List<Achievement> GetAchievesInGroup(
    Assets.Achieve.Type achieveGroup,
    bool isComplete)
  {
    return this.GetAchievesInGroup(achieveGroup).FindAll((Predicate<Achievement>) (obj => obj.IsCompleted() == isComplete));
  }

  public List<Achievement> GetAchievesForAdventureWing(int wingID) => new List<Achievement>((IEnumerable<Achievement>) this.m_achievements.Values).FindAll((Predicate<Achievement>) (obj => obj.Enabled && obj.WingID == wingID));

  public List<Achievement> GetAchievesForAdventureAndMode(
    int adventureId,
    int modeId)
  {
    return new List<Achievement>((IEnumerable<Achievement>) this.m_achievements.Values).FindAll((Predicate<Achievement>) (obj => obj.AdventureID == adventureId && obj.AdventureModeID == modeId));
  }

  public bool HasActiveAchievesForEvent(SpecialEventType eventTrigger)
  {
    if (eventTrigger == SpecialEventType.IGNORE)
      return false;
    foreach (KeyValuePair<int, Achievement> achievement1 in this.m_achievements)
    {
      Achievement achievement2 = achievement1.Value;
      if (achievement2.EventTrigger == eventTrigger && achievement2.Enabled && achievement2.Active)
        return true;
    }
    return false;
  }

  public bool CanCancelQuest(int achieveID)
  {
    if (this.m_disableCancelButtonUntilServerReturns || !this.CanCancelQuestNow() || !AchieveManager.HasAccessToDailies())
      return false;
    Achievement achievement = this.GetAchievement(achieveID);
    return achievement != null && achievement.CanBeCancelled && achievement.Active;
  }

  public static bool HasAccessToDailies() => AchieveManager.Get().HasUnlockedFeature(Assets.Achieve.Unlocks.DAILY);

  public bool RegisterQuestCanceledListener(AchieveManager.AchieveCanceledCallback callback) => this.RegisterQuestCanceledListener(callback, (object) null);

  public bool RegisterQuestCanceledListener(
    AchieveManager.AchieveCanceledCallback callback,
    object userData)
  {
    AchieveManager.AchieveCanceledListener canceledListener = new AchieveManager.AchieveCanceledListener();
    canceledListener.SetCallback(callback);
    canceledListener.SetUserData(userData);
    if (this.m_achieveCanceledListeners.Contains(canceledListener))
      return false;
    this.m_achieveCanceledListeners.Add(canceledListener);
    return true;
  }

  public bool RemoveQuestCanceledListener(AchieveManager.AchieveCanceledCallback callback) => this.RemoveQuestCanceledListener(callback, (object) null);

  public bool RemoveQuestCanceledListener(
    AchieveManager.AchieveCanceledCallback callback,
    object userData)
  {
    AchieveManager.AchieveCanceledListener canceledListener = new AchieveManager.AchieveCanceledListener();
    canceledListener.SetCallback(callback);
    canceledListener.SetUserData(userData);
    return this.m_achieveCanceledListeners.Remove(canceledListener);
  }

  public void CancelQuest(int achieveID)
  {
    if (!this.CanCancelQuest(achieveID))
    {
      this.FireAchieveCanceledEvent(achieveID, false);
    }
    else
    {
      this.BlockAllNotifications();
      this.m_disableCancelButtonUntilServerReturns = true;
      Network.Get().RequestCancelQuest(achieveID);
    }
  }

  public bool RegisterLicenseAddedAchievesUpdatedListener(
    AchieveManager.LicenseAddedAchievesUpdatedCallback callback)
  {
    return this.RegisterLicenseAddedAchievesUpdatedListener(callback, (object) null);
  }

  public bool RegisterLicenseAddedAchievesUpdatedListener(
    AchieveManager.LicenseAddedAchievesUpdatedCallback callback,
    object userData)
  {
    AchieveManager.LicenseAddedAchievesUpdatedListener achievesUpdatedListener = new AchieveManager.LicenseAddedAchievesUpdatedListener();
    achievesUpdatedListener.SetCallback(callback);
    achievesUpdatedListener.SetUserData(userData);
    if (this.m_licenseAddedAchievesUpdatedListeners.Contains(achievesUpdatedListener))
      return false;
    this.m_licenseAddedAchievesUpdatedListeners.Add(achievesUpdatedListener);
    return true;
  }

  public bool RemoveLicenseAddedAchievesUpdatedListener(
    AchieveManager.LicenseAddedAchievesUpdatedCallback callback)
  {
    return this.RemoveLicenseAddedAchievesUpdatedListener(callback, (object) null);
  }

  public bool RemoveLicenseAddedAchievesUpdatedListener(
    AchieveManager.LicenseAddedAchievesUpdatedCallback callback,
    object userData)
  {
    AchieveManager.LicenseAddedAchievesUpdatedListener achievesUpdatedListener = new AchieveManager.LicenseAddedAchievesUpdatedListener();
    achievesUpdatedListener.SetCallback(callback);
    achievesUpdatedListener.SetUserData(userData);
    return this.m_licenseAddedAchievesUpdatedListeners.Remove(achievesUpdatedListener);
  }

  public bool HasActiveLicenseAddedAchieves() => this.GetActiveLicenseAddedAchieves().Count > 0;

  public bool HasActiveLicenseForAdventure(AdventureDbId adventureId)
  {
    List<Achievement> licenseAddedAchieves = this.GetActiveLicenseAddedAchieves();
    for (int index = 0; index < licenseAddedAchieves.Count; ++index)
    {
      if ((AdventureDbId) licenseAddedAchieves[index].AdventureID == adventureId)
        return true;
    }
    return false;
  }

  public void NotifyOfClick(Achievement.ClickTriggerType clickType)
  {
    Log.Achievements.Print("AchieveManager.NotifyOfClick(): clickType {0}", (object) clickType);
    bool hasAllVanillaHeroes = this.HasUnlockedFeature(Assets.Achieve.Unlocks.VANILLA_HEROES);
    foreach (Achievement achieve in this.GetAchieves((Func<Achievement, bool>) (obj =>
    {
      if (obj.AchieveTrigger != Assets.Achieve.Trigger.CLICK)
        return false;
      if (!obj.Enabled)
      {
        Log.Achievements.Print("AchieveManager.NotifyOfClick(): skip disabled achieve {0}", (object) obj.ID);
        return false;
      }
      if (obj.IsCompleted())
      {
        Log.Achievements.Print("AchieveManager.NotifyOfClick(): skip already completed achieve {0}", (object) obj.ID);
        return false;
      }
      Achievement.ClickTriggerType? clickType1 = obj.ClickType;
      if (!clickType1.HasValue)
      {
        Log.Achievements.Print("AchieveManager.NotifyOfClick(): skip missing ClickType achieve {0}", (object) obj.ID);
        return false;
      }
      clickType1 = obj.ClickType;
      if (clickType1.Value != clickType)
      {
        Logger achievements = Log.Achievements;
        object[] objArray = new object[2]
        {
          (object) obj.ID,
          null
        };
        clickType1 = obj.ClickType;
        objArray[1] = (object) clickType1.Value;
        achievements.Print("AchieveManager.NotifyOfClick(): skip achieve {0} with non-matching ClickType {1}", objArray);
        return false;
      }
      if (clickType != Achievement.ClickTriggerType.BUTTON_ADVENTURE || hasAllVanillaHeroes || !AdventureUtils.DoesAdventureRequireAllHeroesUnlocked((AdventureDbId) obj.AdventureID))
        return true;
      Log.Achievements.Print("AchieveManager.NotifyOfClick(): skip achieve {0} for BUTTON_ADVENTURE requiring all heroes unlocked", (object) obj.ID);
      return false;
    })))
    {
      Log.Achievements.Print("AchieveManager.NotifyOfClick(): add achieve {0}", (object) achieve.ID);
      this.m_achieveValidationsToRequest.Add(achieve.ID);
    }
    this.ValidateAchievesNow();
  }

  public void CompleteAutoDestroyAchieve(int achieveId)
  {
    foreach (Achievement achieve in this.GetAchieves((Func<Achievement, bool>) (obj => !obj.IsCompleted() && obj.Enabled && obj.Active && obj.AchieveTrigger == Assets.Achieve.Trigger.DESTROYED)))
    {
      if (achieve.ID == achieveId)
        this.m_achieveValidationsToRequest.Add(achieve.ID);
    }
    this.ValidateAchievesNow();
  }

  public void NotifyOfAccountCreation()
  {
    foreach (Achievement achieve in this.GetAchieves((Func<Achievement, bool>) (obj => !obj.IsCompleted() && obj.Enabled && obj.AchieveTrigger == Assets.Achieve.Trigger.ACCOUNT_CREATED)))
      this.m_achieveValidationsToRequest.Add(achieve.ID);
    this.ValidateAchievesNow();
  }

  public void NotifyOfPacksReadyToOpen(UnopenedPack unopenedPack)
  {
    IEnumerable<Achievement> achieves = this.GetAchieves((Func<Achievement, bool>) (obj => obj.Enabled && !obj.IsCompleted() && obj.AchieveTrigger == Assets.Achieve.Trigger.PACK_READY_TO_OPEN && obj.BoosterRequirement == unopenedPack.GetBoosterId() && unopenedPack.GetCount() != 0 && unopenedPack.CanOpenPack()));
    bool flag = false;
    foreach (Achievement achievement in achieves)
    {
      this.m_achieveValidationsToRequest.Add(achievement.ID);
      flag = true;
    }
    if (!flag)
      return;
    this.ValidateAchievesNow();
  }

  public void Update()
  {
    if (!Network.IsRunning())
      return;
    this.CheckTimedEventsAndLicenses(DateTime.UtcNow);
  }

  public void ValidateAchievesNow()
  {
    if (this.m_achieveValidationsToRequest.Count == 0)
      return;
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    List<AchieveRegionDataDbfRecord> records = GameDbf.AchieveRegionData.GetRecords();
    foreach (int achieveID in this.m_achieveValidationsToRequest)
    {
      AchieveRegionDataDbfRecord regionDataDbfRecord1 = (AchieveRegionDataDbfRecord) null;
      foreach (AchieveRegionDataDbfRecord regionDataDbfRecord2 in records)
      {
        if (regionDataDbfRecord2.AchieveId == achieveID && !specialEventManager.IsEventActive(regionDataDbfRecord2.ProgressableEvent, false))
        {
          regionDataDbfRecord1 = regionDataDbfRecord2;
          break;
        }
      }
      if (regionDataDbfRecord1 != null && !specialEventManager.IsEventActive(regionDataDbfRecord1.ProgressableEvent, false))
      {
        Log.Achievements.Print("AchieveManager.ValidateAchievesNow(): skip non-progressable achieve {0} event {1}", (object) achieveID, (object) regionDataDbfRecord1.ProgressableEvent);
      }
      else
      {
        Log.Achievements.Print("AchieveManager.ValidateAchievesNow(): ValidateAchieve {0}", (object) achieveID);
        this.m_achieveValidationsRequested.Add(achieveID);
        Network.Get().ValidateAchieve(achieveID);
      }
    }
    this.m_achieveValidationsToRequest.Clear();
  }

  public void CheckPlayedNearbyPlayerOnSubnet()
  {
    if (!this.HasActiveAchievesForEvent(SpecialEventType.FIRESIDE_GATHERINGS_CARDBACK))
      return;
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    if (opposingSidePlayer == null)
      return;
    BnetPlayer nearbyPlayer = BnetNearbyPlayerMgr.Get().FindNearbyPlayer(opposingSidePlayer.GetGameAccountId());
    if (nearbyPlayer == null)
      return;
    BnetAccountId accountId1 = nearbyPlayer.GetAccountId();
    if ((BnetEntityId) accountId1 == (BnetEntityId) null)
      return;
    List<BnetPlayer> nearbyPlayers = BnetNearbyPlayerMgr.Get().GetNearbyPlayers();
    BnetPlayer bnetPlayer1 = (BnetPlayer) null;
    foreach (BnetPlayer bnetPlayer2 in nearbyPlayers)
    {
      BnetAccountId accountId2 = bnetPlayer2.GetAccountId();
      if (!((BnetEntityId) accountId2 == (BnetEntityId) null) && !accountId2.Equals((BnetEntityId) accountId1))
      {
        bnetPlayer1 = bnetPlayer2;
        break;
      }
    }
    ulong sessionStartTime1;
    ulong sessionStartTime2;
    if (bnetPlayer1 == null || !BnetNearbyPlayerMgr.Get().GetNearbySessionStartTime(nearbyPlayer, out sessionStartTime1) || !BnetNearbyPlayerMgr.Get().GetNearbySessionStartTime(bnetPlayer1, out sessionStartTime2))
      return;
    BnetGameAccountId hearthstoneGameAccountId1 = nearbyPlayer.GetHearthstoneGameAccountId();
    if ((BnetEntityId) hearthstoneGameAccountId1 == (BnetEntityId) null)
      return;
    BnetGameAccountId hearthstoneGameAccountId2 = bnetPlayer1.GetHearthstoneGameAccountId();
    if ((BnetEntityId) hearthstoneGameAccountId2 == (BnetEntityId) null)
      return;
    ++this.m_numEventResponsesNeeded;
    Network.Get().TriggerPlayedNearbyPlayerOnSubnet(hearthstoneGameAccountId1, sessionStartTime1, hearthstoneGameAccountId2, sessionStartTime2);
  }

  public void LoadAchievesFromDBF()
  {
    this.m_achievements.Clear();
    List<AchieveDbfRecord> records1 = GameDbf.Achieve.GetRecords();
    List<CharacterDialogDbfRecord> records2 = GameDbf.CharacterDialog.GetRecords();
    Map<int, int> map = new Map<int, int>();
    foreach (AchieveDbfRecord dbfRecord in records1)
    {
      int id1 = dbfRecord.ID;
      int race = dbfRecord.Race;
      TAG_RACE? raceReq = new TAG_RACE?();
      if (race != 0)
        raceReq = new TAG_RACE?((TAG_RACE) race);
      int cardSet = dbfRecord.CardSet;
      TAG_CARD_SET? cardSetReq = new TAG_CARD_SET?();
      if (cardSet != 0)
        cardSetReq = new TAG_CARD_SET?((TAG_CARD_SET) cardSet);
      int myHeroClassId = dbfRecord.MyHeroClassId;
      TAG_CLASS? myHeroClassReq = new TAG_CLASS?();
      if (myHeroClassId != 0)
        myHeroClassReq = new TAG_CLASS?((TAG_CLASS) myHeroClassId);
      long rewardData1 = dbfRecord.RewardData1;
      long rewardData2 = dbfRecord.RewardData2;
      bool isGenericRewardChest = false;
      string chestVisualPrefabPath = "";
      List<RewardData> rewards = new List<RewardData>();
      TAG_CLASS? classReward = new TAG_CLASS?();
      switch (dbfRecord.Reward)
      {
        case "arcane_orbs":
          rewards.Add((RewardData) RewardUtils.CreateArcaneOrbRewardData((int) rewardData1));
          break;
        case "basic":
          Debug.LogWarning((object) string.Format("AchieveManager.LoadAchievesFromFile(): unable to define reward {0} for achieve {1}", (object) dbfRecord.Reward, (object) id1));
          break;
        case "card":
          string cardId1 = GameUtils.TranslateDbIdToCardId((int) rewardData1);
          TAG_PREMIUM premium1 = (TAG_PREMIUM) rewardData2;
          rewards.Add((RewardData) new CardRewardData(cardId1, premium1, 1));
          break;
        case "card2x":
          string cardId2 = GameUtils.TranslateDbIdToCardId((int) rewardData1);
          TAG_PREMIUM premium2 = (TAG_PREMIUM) rewardData2;
          rewards.Add((RewardData) new CardRewardData(cardId2, premium2, 2));
          break;
        case "cardback":
          rewards.Add((RewardData) new CardBackRewardData((int) rewardData1));
          break;
        case "deck":
          rewards.Add((RewardData) RewardUtils.CreateDeckRewardData((int) rewardData1, (int) rewardData2, (string) null));
          break;
        case "dust":
          rewards.Add((RewardData) new ArcaneDustRewardData((int) rewardData1));
          break;
        case "event_notice":
          int eventType1 = rewardData1 > 0L ? (int) rewardData1 : 0;
          rewards.Add((RewardData) new EventRewardData(eventType1));
          break;
        case "forge":
          rewards.Add((RewardData) new ForgeTicketRewardData((int) rewardData1));
          break;
        case "generic_reward_chest":
          isGenericRewardChest = true;
          rewards.AddRange((IEnumerable<RewardData>) RewardUtils.GetRewardDataFromRewardChestAsset((int) rewardData1, (int) rewardData2));
          chestVisualPrefabPath = GameDbf.RewardChest.GetRecord((int) rewardData1).ChestPrefab;
          break;
        case "gold":
          rewards.Add((RewardData) new GoldRewardData((long) (int) rewardData1));
          break;
        case "goldhero":
          string cardId3 = GameUtils.TranslateDbIdToCardId((int) rewardData1);
          TAG_PREMIUM premium3 = (TAG_PREMIUM) rewardData2;
          rewards.Add((RewardData) new CardRewardData(cardId3, premium3, 1));
          break;
        case "hero":
          classReward = new TAG_CLASS?((TAG_CLASS) rewardData2);
          string vanillaHero = CollectionManager.GetVanillaHero(classReward.Value);
          if (!string.IsNullOrEmpty(vanillaHero))
          {
            rewards.Add((RewardData) new CardRewardData(vanillaHero, TAG_PREMIUM.NORMAL, 1));
            break;
          }
          break;
        case "mercenary":
          rewards.Add((RewardData) RewardUtils.CreateMercenaryRewardData((int) rewardData1, 0, TAG_PREMIUM.NORMAL));
          break;
        case "mercenary_coins":
          rewards.Add((RewardData) RewardUtils.CreateMercenaryCoinsRewardData((int) rewardData1, (int) rewardData2, true, false));
          break;
        case "mount":
          rewards.Add((RewardData) new MountRewardData((MountRewardData.MountType) rewardData1));
          break;
        case "pack":
          int id2 = rewardData2 > 0L ? (int) rewardData2 : 1;
          rewards.Add((RewardData) new BoosterPackRewardData(id2, (int) rewardData1));
          break;
      }
      Assets.Achieve.RewardTiming rewardTiming = dbfRecord.RewardTiming;
      int num1 = 0;
      int linkToId = 0;
      string parentAch = dbfRecord.ParentAch;
      string linkTo = dbfRecord.LinkTo;
      int index1 = 0;
      for (int count = records1.Count; index1 < count; ++index1)
      {
        string noteDesc = records1[index1].NoteDesc;
        if (num1 == 0 && noteDesc == parentAch)
          num1 = records1[index1].ID;
        if (linkToId == 0 && noteDesc == linkTo)
          linkToId = records1[index1].ID;
        if (num1 != 0 && linkToId != 0)
          break;
      }
      map[id1] = num1;
      Achievement.ClickTriggerType? clickType = new Achievement.ClickTriggerType?();
      if (dbfRecord.Triggered == Assets.Achieve.Trigger.CLICK)
        clickType = new Achievement.ClickTriggerType?((Achievement.ClickTriggerType) rewardData1);
      if (id1 == 94)
        clickType = new Achievement.ClickTriggerType?(Achievement.ClickTriggerType.BUTTON_ARENA);
      List<int> scenarios = new List<int>();
      List<AchieveConditionDbfRecord> records3 = GameDbf.AchieveCondition.GetRecords();
      int index2 = 0;
      for (int count = records3.Count; index2 < count; ++index2)
      {
        AchieveConditionDbfRecord conditionDbfRecord = records3[index2];
        if (conditionDbfRecord.AchieveId == id1)
          scenarios.Add(conditionDbfRecord.ScenarioId);
      }
      CharacterDialogDbfRecord characterDialogDbfRecord = (CharacterDialogDbfRecord) null;
      int questDialogId = dbfRecord.QuestDialogId;
      int index3 = 0;
      for (int count = records2.Count; index3 < count; ++index3)
      {
        if (records2[index3].ID == questDialogId)
        {
          characterDialogDbfRecord = records2[index3];
          break;
        }
      }
      int num2 = characterDialogDbfRecord == null ? 0 : characterDialogDbfRecord.ID;
      CharacterDialogSequence onReceivedDialogSequence = (CharacterDialogSequence) null;
      CharacterDialogSequence onCompleteDialogSequence = (CharacterDialogSequence) null;
      CharacterDialogSequence onProgress1DialogSequence = (CharacterDialogSequence) null;
      CharacterDialogSequence onProgress2DialogSequence = (CharacterDialogSequence) null;
      CharacterDialogSequence onDismissDialogSequence = (CharacterDialogSequence) null;
      if (characterDialogDbfRecord != null)
      {
        onReceivedDialogSequence = new CharacterDialogSequence(num2, CharacterDialogEventType.RECEIVE);
        onCompleteDialogSequence = new CharacterDialogSequence(num2, CharacterDialogEventType.COMPLETE);
        onProgress1DialogSequence = new CharacterDialogSequence(num2, CharacterDialogEventType.PROGRESS1);
        onProgress2DialogSequence = new CharacterDialogSequence(num2, CharacterDialogEventType.PROGRESS2);
        onDismissDialogSequence = new CharacterDialogSequence(num2, CharacterDialogEventType.DISMISS);
      }
      int onCompleteQuestDialogBannerId = characterDialogDbfRecord == null ? 0 : characterDialogDbfRecord.OnCompleteBannerId;
      Achievement achievement = new Achievement(dbfRecord, id1, dbfRecord.AchType, dbfRecord.AchQuota, linkToId, dbfRecord.Triggered, dbfRecord.GameMode, raceReq, classReward, cardSetReq, myHeroClassReq, clickType, dbfRecord.Unlocks, rewards, scenarios, dbfRecord.AdventureWingId, dbfRecord.AdventureId, dbfRecord.AdventureModeId, rewardTiming, dbfRecord.Booster, dbfRecord.UseGenericRewardVisual, dbfRecord.ShowToReturningPlayer, num2, dbfRecord.AutoDestroy, dbfRecord.QuestTilePrefab, onCompleteQuestDialogBannerId, onReceivedDialogSequence, onCompleteDialogSequence, onProgress1DialogSequence, onProgress2DialogSequence, onDismissDialogSequence, isGenericRewardChest, chestVisualPrefabPath, dbfRecord.CustomVisualWidget, dbfRecord.EnemyHeroClassId);
      SpecialEventType eventType2 = SpecialEventType.IGNORE;
      switch (dbfRecord.Triggered)
      {
        case Assets.Achieve.Trigger.FINISH:
        case Assets.Achieve.Trigger.EVENT_TIMING_ONLY:
          AchieveRegionDataDbfRecord currentRegionData = achievement.GetCurrentRegionData();
          if (currentRegionData != null)
          {
            eventType2 = currentRegionData.ProgressableEvent;
            break;
          }
          break;
      }
      achievement.SetEventTrigger(eventType2);
      achievement.SetClientFlags(dbfRecord.ClientFlags);
      achievement.SetAltTextPredicate(dbfRecord.AltTextPredicate);
      achievement.SetName((string) dbfRecord.Name, (string) dbfRecord.AltName);
      achievement.SetDescription((string) dbfRecord.Description, (string) dbfRecord.AltDescription);
      this.InitAchievement(achievement);
    }
  }

  private void InitAchievement(Achievement achievement)
  {
    if (this.m_achievements.ContainsKey(achievement.ID))
      Debug.LogWarning((object) string.Format("AchieveManager.InitAchievement() - already registered achievement with ID {0}", (object) achievement.ID));
    else
      this.m_achievements.Add(achievement.ID, achievement);
  }

  private IEnumerable<Achievement> GetAchieves(Func<Achievement, bool> filter = null)
  {
    List<Achievement> achieves = new List<Achievement>();
    foreach (KeyValuePair<int, Achievement> achievement in this.m_achievements)
    {
      if (filter == null || filter(achievement.Value))
        achieves.Add(achievement.Value);
    }
    return (IEnumerable<Achievement>) achieves;
  }

  public void OnInitialAchievements(Achieves achievements)
  {
    if (achievements == null)
      return;
    this.OnAllAchieves(achievements);
  }

  private void OnAllAchieves(Achieves allAchievesList)
  {
    foreach (PegasusUtil.Achieve achieveData in allAchievesList.List)
      this.GetAchievement(achieveData.Id)?.OnAchieveData(achieveData);
    this.CheckAllCardGainAchieves();
    this.m_allNetAchievesReceived = true;
    this.UnblockAllNotifications();
  }

  public void OnAchievementNotifications(
    List<AchievementNotification> achievementNotifications)
  {
    List<Achievement> completedAchieves = new List<Achievement>();
    List<Achievement> updatedAchieves = new List<Achievement>();
    bool flag = false;
    foreach (AchievementNotification achievementNotification in achievementNotifications)
    {
      if (this.m_queueNotifications || !this.m_allNetAchievesReceived || this.m_achieveNotificationsToQueue.Contains((int) achievementNotification.AchievementId))
      {
        Log.Achievements.Print("Blocking AchievementNotification: ID={0}", (object) achievementNotification.AchievementId);
        this.m_blockedAchievementNotifications.Add(achievementNotification);
      }
      else
      {
        Achievement achievement = this.GetAchievement((int) achievementNotification.AchievementId);
        if (achievement != null)
        {
          if (achievement.AchieveTrigger == Assets.Achieve.Trigger.LICENSEADDED || achievement.AchieveTrigger == Assets.Achieve.Trigger.EVENT_TIMING_ONLY)
            flag = true;
          achievement.OnAchieveNotification(achievementNotification);
          if (!achievement.Active && achievementNotification.Complete)
            completedAchieves.Add(achievement);
          else
            updatedAchieves.Add(achievement);
          Log.Achievements.Print("OnAchievementNotification: Achievement={0}", (object) achievement);
        }
      }
    }
    if (flag)
      this.m_lastEventTimingAndLicenseAchieveCheck = 0L;
    foreach (AchieveManager.AchievesUpdatedListener achievesUpdatedListener in this.m_achievesUpdatedListeners.ToArray())
      achievesUpdatedListener.Fire(updatedAchieves, completedAchieves);
  }

  public void BlockAllNotifications() => this.m_queueNotifications = true;

  public void UnblockAllNotifications()
  {
    this.m_queueNotifications = false;
    if (this.m_blockedAchievementNotifications.Count <= 0)
      return;
    this.OnAchievementNotifications(this.m_blockedAchievementNotifications);
    this.m_blockedAchievementNotifications.Clear();
  }

  private void OnQuestCanceled()
  {
    Network.CanceledQuest canceledQuest = Network.Get().GetCanceledQuest();
    Log.Achievements.Print("OnQuestCanceled: CanceledQuest={0}", (object) canceledQuest);
    this.m_disableCancelButtonUntilServerReturns = false;
    if (canceledQuest.Canceled)
    {
      this.GetAchievement(canceledQuest.AchieveID).OnCancelSuccess();
      NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
      if (netObject != null)
        netObject.NextQuestCancelDate = canceledQuest.NextQuestCancelDate;
    }
    this.FireAchieveCanceledEvent(canceledQuest.AchieveID, canceledQuest.Canceled);
    this.UnblockAllNotifications();
  }

  private void OnAchieveValidated()
  {
    ValidateAchieveResponse validatedAchieve = Network.Get().GetValidatedAchieve();
    this.m_achieveValidationsRequested.Remove(validatedAchieve.Achieve);
    Log.Achievements.Print("AchieveManager.OnAchieveValidated(): achieve={0} success={1}", (object) validatedAchieve.Achieve, (object) validatedAchieve.Success);
  }

  private void OnEventTriggered()
  {
    Network.Get().GetTriggerEventResponse();
    --this.m_numEventResponsesNeeded;
  }

  private void OnAccountLicenseAchieveResponse()
  {
    Network.AccountLicenseAchieveResponse licenseAchieveResponse = Network.Get().GetAccountLicenseAchieveResponse();
    if (licenseAchieveResponse.Result != Network.AccountLicenseAchieveResponse.AchieveResult.COMPLETE)
    {
      this.FireLicenseAddedAchievesUpdatedEvent();
    }
    else
    {
      Log.Achievements.Print("AchieveManager.OnAccountLicenseAchieveResponse(): achieve {0} is now complete, refreshing achieves", (object) licenseAchieveResponse.Achieve);
      this.OnAccountLicenseAchievesUpdated((object) licenseAchieveResponse.Achieve);
    }
  }

  private void OnAccountLicenseAchievesUpdated(object userData)
  {
    Log.Achievements.Print("AchieveManager.OnAccountLicenseAchievesUpdated(): refreshing achieves complete, triggered by achieve {0}", (object) (int) userData);
    this.FireLicenseAddedAchievesUpdatedEvent();
  }

  private void FireLicenseAddedAchievesUpdatedEvent()
  {
    List<Achievement> licenseAddedAchieves = this.GetActiveLicenseAddedAchieves();
    foreach (AchieveManager.LicenseAddedAchievesUpdatedListener achievesUpdatedListener in this.m_licenseAddedAchievesUpdatedListeners.ToArray())
      achievesUpdatedListener.Fire(licenseAddedAchieves);
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    foreach (NetCache.ProfileNotice newNotice in newNotices)
    {
      if (NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT == newNotice.Origin)
        this.GetAchievement((int) newNotice.OriginData)?.AddRewardNoticeID(newNotice.NoticeID);
    }
  }

  private bool CanCancelQuestNow()
  {
    if (Vars.Key("Quests.CanCancelManyTimes").GetBool(false))
      return true;
    NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
    if (netObject == null)
      return false;
    long fileTimeUtc = DateTime.Now.ToFileTimeUtc();
    return netObject.NextQuestCancelDate <= fileTimeUtc;
  }

  private void FireAchieveCanceledEvent(int achieveID, bool success)
  {
    foreach (AchieveManager.AchieveCanceledListener canceledListener in this.m_achieveCanceledListeners.ToArray())
      canceledListener.Fire(achieveID, success);
  }

  private void CheckAllCardGainAchieves()
  {
    this.GetAchieves((Func<Achievement, bool>) (obj =>
    {
      if (!obj.Enabled || obj.IsCompleted())
        return false;
      switch (obj.AchieveTrigger)
      {
        case Assets.Achieve.Trigger.RACE:
        case Assets.Achieve.Trigger.GOLDRACE:
          return obj.RaceRequirement.HasValue;
        default:
          return false;
      }
    }));
    this.GetAchieves((Func<Achievement, bool>) (obj => obj.Enabled && !obj.IsCompleted() && obj.AchieveTrigger == Assets.Achieve.Trigger.CARDSET && obj.CardSetRequirement.HasValue));
    this.ValidateAchievesNow();
  }

  private void CheckTimedEventsAndLicenses(DateTime utcNow)
  {
    if (!this.m_allNetAchievesReceived)
      return;
    DateTime localTime = utcNow.ToLocalTime();
    if (localTime.Ticks - this.m_lastEventTimingAndLicenseAchieveCheck < AchieveManager.TIMED_AND_LICENSE_ACHIEVE_CHECK_DELAY_TICKS)
      return;
    this.m_lastEventTimingAndLicenseAchieveCheck = localTime.Ticks;
    int num = 0;
    foreach (Achievement achievement in this.m_achievements.Values)
    {
      if (achievement.Enabled && !achievement.IsCompleted() && achievement.Active && Assets.Achieve.Trigger.EVENT_TIMING_ONLY == achievement.AchieveTrigger && SpecialEventManager.Get().IsEventActive(achievement.EventTrigger, false) && (!this.m_lastEventTimingValidationByAchieve.ContainsKey(achievement.ID) || localTime.Ticks - this.m_lastEventTimingValidationByAchieve[achievement.ID] >= AchieveManager.TIMED_ACHIEVE_VALIDATION_DELAY_TICKS))
      {
        Log.Achievements.Print("AchieveManager.CheckTimedEventsAndLicenses(): checking on timed event achieve {0} time {1}", (object) achievement.ID, (object) localTime);
        this.m_lastEventTimingValidationByAchieve[achievement.ID] = localTime.Ticks;
        this.m_achieveValidationsToRequest.Add(achievement.ID);
        ++num;
      }
      if (achievement.IsActiveLicenseAddedAchieve() && (!this.m_lastCheckLicenseAddedByAchieve.ContainsKey(achievement.ID) || utcNow.Ticks - this.m_lastCheckLicenseAddedByAchieve[achievement.ID] >= AchieveManager.CHECK_LICENSE_ADDED_ACHIEVE_DELAY_TICKS))
      {
        Log.Achievements.Print("AchieveManager.CheckTimedEventsAndLicenses(): checking on license added achieve {0} time {1}", (object) achievement.ID, (object) localTime);
        this.m_lastCheckLicenseAddedByAchieve[achievement.ID] = utcNow.Ticks;
        Network.Get().CheckAccountLicenseAchieve(achievement.ID);
      }
    }
    if (num == 0)
      return;
    this.ValidateAchievesNow();
  }

  private List<Achievement> GetActiveLicenseAddedAchieves()
  {
    List<Achievement> licenseAddedAchieves = new List<Achievement>();
    foreach (KeyValuePair<int, Achievement> achievement1 in this.m_achievements)
    {
      Achievement achievement2 = achievement1.Value;
      if (achievement2.IsActiveLicenseAddedAchieve())
        licenseAddedAchieves.Add(achievement2);
    }
    return licenseAddedAchieves;
  }

  public List<RewardData> GetRewardsForAdventureAndMode(
    int adventureId,
    int modeId,
    HashSet<Assets.Achieve.RewardTiming> rewardTimings)
  {
    List<RewardData> adventureAndMode = new List<RewardData>();
    foreach (Achievement achievement in this.GetAchievesForAdventureAndMode(adventureId, modeId))
      adventureAndMode.AddRange((IEnumerable<RewardData>) this.GetRewardsForAchieve(achievement.ID, rewardTimings));
    return adventureAndMode;
  }

  public List<RewardData> GetRewardsForAdventureWing(
    int wingID,
    HashSet<Assets.Achieve.RewardTiming> rewardTimings)
  {
    List<RewardData> forAdventureWing = new List<RewardData>();
    foreach (Achievement achievement in this.GetAchievesForAdventureWing(wingID))
      forAdventureWing.AddRange((IEnumerable<RewardData>) this.GetRewardsForAchieve(achievement.ID, rewardTimings));
    return forAdventureWing;
  }

  public List<RewardData> GetRewardsForAdventureScenario(
    int wingID,
    int scenarioID,
    HashSet<Assets.Achieve.RewardTiming> rewardTimings)
  {
    List<RewardData> adventureScenario = new List<RewardData>();
    foreach (Achievement achievement in this.GetAchievesForAdventureWing(wingID))
    {
      if (achievement.Scenarios.Contains(scenarioID))
        adventureScenario.AddRange((IEnumerable<RewardData>) this.GetRewardsForAchieve(achievement.ID, rewardTimings));
    }
    return adventureScenario;
  }

  public List<RewardData> GetRewardsForAchieve(
    int achieveID,
    HashSet<Assets.Achieve.RewardTiming> rewardTimings)
  {
    List<RewardData> rewardsForAchieve = new List<RewardData>();
    Achievement achievement = this.GetAchievement(achieveID);
    List<RewardData> rewards = achievement.Rewards;
    if (rewardTimings.Contains(achievement.RewardTiming))
    {
      foreach (RewardData rewardData in rewards)
        rewardsForAchieve.Add(rewardData);
    }
    return rewardsForAchieve;
  }

  public delegate void AchieveCanceledCallback(int achieveID, bool success, object userData);

  private class AchieveCanceledListener : EventListener<AchieveManager.AchieveCanceledCallback>
  {
    public void Fire(int achieveID, bool success) => this.m_callback(achieveID, success, this.m_userData);
  }

  public delegate void AchievesUpdatedCallback(
    List<Achievement> updatedAchieves,
    List<Achievement> completedAchieves,
    object userData);

  private class AchievesUpdatedListener : EventListener<AchieveManager.AchievesUpdatedCallback>
  {
    public void Fire(List<Achievement> updatedAchieves, List<Achievement> completedAchieves) => this.m_callback(updatedAchieves, completedAchieves, this.m_userData);
  }

  public delegate void LicenseAddedAchievesUpdatedCallback(
    List<Achievement> activeLicenseAddedAchieves,
    object userData);

  private class LicenseAddedAchievesUpdatedListener : 
    EventListener<AchieveManager.LicenseAddedAchievesUpdatedCallback>
  {
    public void Fire(List<Achievement> activeLicenseAddedAchieves) => this.m_callback(activeLicenseAddedAchieves, this.m_userData);
  }
}
