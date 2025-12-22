using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using PegasusClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialToastMgr : MonoBehaviour
{
  private const float FADE_IN_TIME = 0.25f;
  private const float FADE_OUT_TIME = 0.5f;
  private const float HOLD_TIME = 2f;
  private const float SHUTDOWN_MESSAGE_TIME = 3.5f;
  private const float OFFLINE_TOAST_DELAY = 5f;
  private const int MAX_QUEUE_CAPACITY = 5;
  private const string BNET_TOAST_SOUND = "UI_BnetToast.prefab:b869739323d1fc241984f9f480fff8ef";
  public SocialToast m_defaultSocialToastPrefab;
  public SocialToast m_firesideGatheringSocialToastPrefab;
  private static SocialToastMgr s_instance;
  private SocialToast m_defaultToast;
  private SocialToast m_firesideGatheringToast;
  private SocialToast m_currentToast;
  private Queue<SocialToastMgr.ToastArgs> m_toastQueue = new Queue<SocialToastMgr.ToastArgs>();
  private bool m_toastIsShown;
  private bool m_toastsEnabled;
  private PlatformDependentValue<Vector3> TOAST_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(235f, 1f, 235f),
    Phone = new Vector3(470f, 1f, 470f)
  };
  private Map<BnetGameAccountId, MedalInfoTranslator> m_lastKnownMedals = new Map<BnetGameAccountId, MedalInfoTranslator>();
  private Map<BnetGameAccountId, string> m_lastOpenedLegendary = new Map<BnetGameAccountId, string>();
  private Map<BnetGameAccountId, int> m_lastAchievementId = new Map<BnetGameAccountId, int>();
  private Map<int, SocialToastMgr.LastOnlineTracker> m_lastOnlineTracker = new Map<int, SocialToastMgr.LastOnlineTracker>();

  private void Awake()
  {
    SocialToastMgr.s_instance = this;
    this.CreateSocialToastObjects();
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    BnetPresenceMgr.Get().OnGameAccountPresenceChange += new System.Action<PresenceUpdate[]>(this.OnPresenceChanged);
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    Network.Get().SetShutdownHandler(new Network.ShutdownHandler(this.ShutdownHandler));
    SoundManager.Get().Load((AssetReference) "UI_BnetToast.prefab:b869739323d1fc241984f9f480fff8ef");
    LoginManager.Get().OnFullLoginFlowComplete += new System.Action(this.OnLoginCompleted);
  }

  private void OnDestroy()
  {
    if (BnetPresenceMgr.Get() != null)
    {
      BnetPresenceMgr.Get().OnGameAccountPresenceChange -= new System.Action<PresenceUpdate[]>(this.OnPresenceChanged);
      BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    }
    if (BnetFriendMgr.Get() != null)
      BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    if (LoginManager.Get() != null)
      LoginManager.Get().OnFullLoginFlowComplete -= new System.Action(this.OnLoginCompleted);
    this.m_lastKnownMedals.Clear();
    SocialToastMgr.s_instance = (SocialToastMgr) null;
  }

  public static SocialToastMgr Get() => SocialToastMgr.s_instance;

  public void Reset()
  {
    if ((UnityEngine.Object) this.m_currentToast == (UnityEngine.Object) null)
      return;
    iTween.Stop(this.m_currentToast.gameObject, true);
    iTween.Stop(this.gameObject, true);
    RenderUtils.SetAlpha(this.m_currentToast.gameObject, 0.0f);
    this.m_toastQueue.Clear();
    this.DeactivateToast();
  }

  public void AddToast(UserAttentionBlocker blocker, string textArg) => this.AddToast(blocker, textArg, SocialToastMgr.TOAST_TYPE.DEFAULT, 2f, true);

  public void AddToast(
    UserAttentionBlocker blocker,
    string textArg,
    SocialToastMgr.TOAST_TYPE toastType)
  {
    this.AddToast(blocker, textArg, toastType, 2f, true);
  }

  public void AddToast(
    UserAttentionBlocker blocker,
    string textArg,
    SocialToastMgr.TOAST_TYPE toastType,
    bool playSound)
  {
    this.AddToast(blocker, textArg, toastType, 2f, playSound);
  }

  public void AddToast(
    UserAttentionBlocker blocker,
    string textArg,
    SocialToastMgr.TOAST_TYPE toastType,
    float displayTime)
  {
    this.AddToast(blocker, textArg, toastType, displayTime, true);
  }

  public void AddToast(
    UserAttentionBlocker blocker,
    string textArg,
    SocialToastMgr.TOAST_TYPE toastType,
    float displayTime,
    bool playSound)
  {
    if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "SocialToastMgr.AddToast:" + (object) toastType))
      return;
    SocialToastDesign design = SocialToastDesign.Default;
    string message;
    switch (toastType)
    {
      case SocialToastMgr.TOAST_TYPE.DEFAULT:
        message = textArg;
        break;
      case SocialToastMgr.TOAST_TYPE.FRIEND_ONLINE:
        message = GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ONLINE", (object) "5ecaf0ff", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.FRIEND_OFFLINE:
        message = GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_OFFLINE", (object) "999999ff", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.FRIEND_INVITE:
        message = GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_REQUEST", (object) "5ecaf0ff", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.HEALTHY_GAMING:
        message = GameStrings.Format("GLOBAL_HEALTHY_GAMING_TOAST", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.HEALTHY_GAMING_OVER_THRESHOLD:
        message = GameStrings.Format("GLOBAL_HEALTHY_GAMING_TOAST_OVER_THRESHOLD", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_SENT:
        message = GameStrings.Format("GLOBAL_SOCIAL_TOAST_SPECTATOR_INVITE_SENT", (object) "5ecaf0ff", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_RECEIVED:
        message = GameStrings.Format("GLOBAL_SOCIAL_TOAST_SPECTATOR_INVITE_RECEIVED", (object) "5ecaf0ff", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.SPECTATOR_ADDED:
        message = GameStrings.Format("GLOBAL_SOCIAL_TOAST_SPECTATOR_ADDED", (object) "5ecaf0ff", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.SPECTATOR_REMOVED:
        message = GameStrings.Format("GLOBAL_SOCIAL_TOAST_SPECTATOR_REMOVED", (object) "5ecaf0ff", (object) textArg);
        break;
      case SocialToastMgr.TOAST_TYPE.FIRESIDE_GATHERING_IS_HERE_REMINDER:
        design = SocialToastDesign.FiresideGathering;
        message = string.Format("<color=#{0}>{1}</color>", (object) "ffd200", (object) GameStrings.Get("GLOBAL_FIRESIDE_GATHERING"));
        break;
      default:
        message = "";
        break;
    }
    this.m_currentToast = this.GetSocialToastFromDesign(design);
    if ((UnityEngine.Object) this.m_currentToast == (UnityEngine.Object) null)
    {
      Log.All.PrintWarning("Toast design is not created yet");
    }
    else
    {
      if (this.m_toastQueue.Count > 5)
        return;
      this.m_toastQueue.Enqueue(new SocialToastMgr.ToastArgs(message, displayTime, playSound));
      this.CheckToastQueue();
    }
  }

  public SocialToast CreateDefaultSocialToast(Transform parent)
  {
    SocialToast newToastReference = (SocialToast) null;
    this.CreateSocialToastObject(this.m_defaultSocialToastPrefab, ref newToastReference, parent);
    return newToastReference;
  }

  private void OnLoginCompleted()
  {
    this.m_toastsEnabled = true;
    this.CheckToastQueue();
  }

  private void CreateSocialToastObjects()
  {
    if ((UnityEngine.Object) BnetBar.Get() == (UnityEngine.Object) null || (UnityEngine.Object) BnetBar.Get().m_socialToastBone == (UnityEngine.Object) null || !((UnityEngine.Object) this.m_defaultToast == (UnityEngine.Object) null) && !((UnityEngine.Object) this.m_firesideGatheringToast == (UnityEngine.Object) null))
      return;
    this.CreateSocialToastObject(this.m_defaultSocialToastPrefab, ref this.m_defaultToast);
    this.CreateSocialToastObject(this.m_firesideGatheringSocialToastPrefab, ref this.m_firesideGatheringToast);
    this.m_currentToast = this.m_defaultToast;
  }

  private void CreateSocialToastObject(
    SocialToast prefab,
    ref SocialToast newToastReference,
    Transform parent = null)
  {
    if ((UnityEngine.Object) parent == (UnityEngine.Object) null && ((UnityEngine.Object) BnetBar.Get() == (UnityEngine.Object) null || (UnityEngine.Object) BnetBar.Get().m_socialToastBone == (UnityEngine.Object) null))
    {
      Debug.LogError((object) "FAILED to create Social Toast Object, no parent transform found!");
    }
    else
    {
      newToastReference = UnityEngine.Object.Instantiate<SocialToast>(prefab);
      RenderUtils.SetAlpha(newToastReference.gameObject, 0.0f);
      newToastReference.gameObject.SetActive(false);
      newToastReference.transform.parent = (UnityEngine.Object) parent != (UnityEngine.Object) null ? parent : BnetBar.Get().m_socialToastBone.transform;
      newToastReference.transform.localRotation = Quaternion.Euler(new Vector3(90f, 180f, 0.0f));
      newToastReference.transform.localScale = (Vector3) this.TOAST_SCALE;
      newToastReference.transform.position = BnetBar.Get().m_socialToastBone.transform.position;
    }
  }

  private void AssignParentAndPosition(ref SocialToast newToastReference, Transform parent = null)
  {
    if ((UnityEngine.Object) BnetBar.Get() != (UnityEngine.Object) null && (UnityEngine.Object) BnetBar.Get().m_socialToastBone != (UnityEngine.Object) null)
    {
      newToastReference.transform.position = BnetBar.Get().m_socialToastBone.transform.position;
      if ((UnityEngine.Object) parent == (UnityEngine.Object) null)
        newToastReference.transform.parent = BnetBar.Get().m_socialToastBone.transform;
    }
    if (!((UnityEngine.Object) parent != (UnityEngine.Object) null))
      return;
    newToastReference.transform.parent = parent;
  }

  private SocialToast GetSocialToastFromDesign(SocialToastDesign design)
  {
    this.CreateSocialToastObjects();
    return design == SocialToastDesign.FiresideGathering ? this.m_firesideGatheringToast : this.m_defaultToast;
  }

  private void FadeInToast()
  {
    this.m_toastIsShown = true;
    SocialToastMgr.ToastArgs toastArgs = this.m_toastQueue.Dequeue();
    this.m_currentToast.gameObject.SetActive(true);
    this.m_currentToast.SetText(toastArgs.m_message);
    Hashtable args = iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "FadeOutToast", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) toastArgs.m_displayTime, (object) "name", (object) "fade");
    iTween.StopByName(this.gameObject, "fade");
    iTween.FadeTo(this.m_currentToast.gameObject, args);
    RenderUtils.SetAlpha(this.m_currentToast.gameObject, 1f);
    if (!toastArgs.m_playSound)
      return;
    this.PlayToastSound();
  }

  public void PlayToastSound() => SoundManager.Get().LoadAndPlay((AssetReference) "UI_BnetToast.prefab:b869739323d1fc241984f9f480fff8ef");

  private void FadeOutToast(float displayTime) => iTween.FadeTo(this.m_currentToast.gameObject, iTween.Hash((object) "amount", (object) 0.0f, (object) "delay", (object) displayTime, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "DeactivateToast", (object) "oncompletetarget", (object) this.gameObject, (object) "name", (object) "fade"));

  private void DeactivateToast()
  {
    this.m_currentToast.gameObject.SetActive(false);
    this.m_toastIsShown = false;
    this.CheckToastQueue();
  }

  public void CheckToastQueue()
  {
    if (!this.m_toastsEnabled || this.m_toastIsShown)
      return;
    AchievementManager achievementManager = AchievementManager.Get();
    if (achievementManager != null)
    {
      achievementManager.CheckToastQueue();
      if (achievementManager.IsShowingToast())
        return;
    }
    if (this.m_toastQueue.Count == 0)
      return;
    this.FadeInToast();
  }

  public bool IsShowingToast() => this.m_toastIsShown;

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (!DemoMgr.Get().IsSocialEnabled())
      return;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    foreach (BnetPlayerChange change in changelist.GetChanges())
    {
      if (change.GetPlayer() != null && change.GetNewPlayer() != null && change != null && change.GetPlayer().IsDisplayable() && change.GetPlayer() != myPlayer && BnetFriendMgr.Get().IsFriend(change.GetPlayer()))
      {
        BnetPlayer oldPlayer = change.GetOldPlayer();
        BnetPlayer newPlayer = change.GetNewPlayer();
        this.CheckForOnlineStatusChanged(oldPlayer, newPlayer);
        if (oldPlayer != null)
        {
          BnetGameAccount hearthstoneGameAccount1 = newPlayer.GetHearthstoneGameAccount();
          BnetGameAccount hearthstoneGameAccount2 = oldPlayer.GetHearthstoneGameAccount();
          if (!(hearthstoneGameAccount2 == (BnetGameAccount) null) && !(hearthstoneGameAccount1 == (BnetGameAccount) null))
          {
            this.CheckForCardOpened(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForDruidLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForHunterLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForMageLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForPaladinLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForPriestLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForRogueLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForShamanLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForWarlockLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForWarriorLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForMissionComplete(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
            this.CheckForAchievementCompleted(hearthstoneGameAccount2, hearthstoneGameAccount1, newPlayer);
          }
        }
      }
    }
  }

  private void OnPresenceChanged(PresenceUpdate[] updates)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    foreach (PresenceUpdate update in updates)
    {
      if (!(update.programId != (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE))
      {
        BnetPlayer player = BnetUtils.GetPlayer(new BnetGameAccountId(update.entityId?.EntityId));
        if (player != null && player != myPlayer && player.IsDisplayable() && BnetFriendMgr.Get().IsFriend(player))
        {
          switch (update.fieldId)
          {
            case 17:
              this.CheckSessionGameStarted(player);
              continue;
            case 18:
              this.CheckForNewRank(player);
              continue;
            case 22:
              this.CheckSessionRecordChanged(player);
              continue;
            default:
              continue;
          }
        }
      }
    }
  }

  private void CheckForOnlineStatusChanged(BnetPlayer oldPlayer, BnetPlayer newPlayer)
  {
    if (oldPlayer == null || newPlayer == null || oldPlayer.IsOnline() == newPlayer.IsOnline())
      return;
    long lastOnlineMicrosec = newPlayer.GetBestLastOnlineMicrosec();
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    long num = 0;
    if (myPlayer != null)
      num = myPlayer.GetBestLastOnlineMicrosec();
    if (lastOnlineMicrosec == 0L || num == 0L || num > lastOnlineMicrosec)
      return;
    SocialToastMgr.LastOnlineTracker lastOnlineTracker = (SocialToastMgr.LastOnlineTracker) null;
    float fixedTime = Time.fixedTime;
    int hashCode = newPlayer.GetAccountId().GetHashCode();
    if (!this.m_lastOnlineTracker.TryGetValue(hashCode, out lastOnlineTracker))
    {
      lastOnlineTracker = new SocialToastMgr.LastOnlineTracker();
      this.m_lastOnlineTracker[hashCode] = lastOnlineTracker;
    }
    if (newPlayer.IsOnline())
    {
      if (lastOnlineTracker.m_callback != null)
        Processor.CancelScheduledCallback(lastOnlineTracker.m_callback);
      lastOnlineTracker.m_callback = (Processor.ScheduledCallback) null;
      if ((double) fixedTime - (double) lastOnlineTracker.m_localLastOnlineTime < 5.0)
        return;
      this.AddToast(UserAttentionBlocker.NONE, newPlayer.GetBestName(), SocialToastMgr.TOAST_TYPE.FRIEND_ONLINE);
    }
    else
    {
      lastOnlineTracker.m_localLastOnlineTime = fixedTime;
      lastOnlineTracker.m_callback = (Processor.ScheduledCallback) (data =>
      {
        if (newPlayer.IsOnline())
          return;
        this.AddToast(UserAttentionBlocker.NONE, newPlayer.GetBestName(), SocialToastMgr.TOAST_TYPE.FRIEND_OFFLINE, false);
      });
      Processor.ScheduleCallback(5f, false, lastOnlineTracker.m_callback);
    }
  }

  private void CheckSessionGameStarted(BnetPlayer player)
  {
    if (PresenceMgr.Get().GetStatus(player) == Global.PresenceStatus.TAVERN_BRAWL_GAME)
    {
      if (!TavernBrawlManager.Get().IsCurrentSeasonSessionBased)
        return;
    }
    else if (PresenceMgr.Get().GetStatus(player) != Global.PresenceStatus.ARENA_GAME)
      return;
    BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
    if (hearthstoneGameAccount == (BnetGameAccount) null)
      return;
    SessionRecord sessionRecord = hearthstoneGameAccount.GetSessionRecord();
    if (sessionRecord == null || sessionRecord.Wins < 8U || sessionRecord.RunFinished)
      return;
    string key = string.Empty;
    switch (sessionRecord.SessionRecordType)
    {
      case SessionRecordType.ARENA:
        key = "GLOBAL_SOCIAL_TOAST_FRIEND_ARENA_START_WITH_MANY_WINS";
        break;
      case SessionRecordType.HEROIC_BRAWL:
        key = "GLOBAL_SOCIAL_TOAST_FRIEND_HEROIC_BRAWL_START_WITH_MANY_WINS";
        break;
      case SessionRecordType.TAVERN_BRAWL:
        key = "GLOBAL_SOCIAL_TOAST_FRIEND_BRAWLISEUM_START_WITH_MANY_WINS";
        break;
    }
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format(key, (object) "5ecaf0ff", (object) player.GetBestName(), (object) sessionRecord.Wins));
  }

  private void CheckSessionRecordChanged(BnetPlayer player)
  {
    BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
    if (hearthstoneGameAccount == (BnetGameAccount) null)
      return;
    SessionRecord sessionRecord = hearthstoneGameAccount.GetSessionRecord();
    if (sessionRecord == null)
      return;
    string key = string.Empty;
    if (sessionRecord.RunFinished)
    {
      if (sessionRecord.Wins < 3U)
        return;
      switch (sessionRecord.SessionRecordType)
      {
        case SessionRecordType.ARENA:
          key = "GLOBAL_SOCIAL_TOAST_FRIEND_ARENA_COMPLETE";
          break;
        case SessionRecordType.HEROIC_BRAWL:
          key = "GLOBAL_SOCIAL_TOAST_FRIEND_HEROIC_BRAWL_COMPLETE";
          break;
        case SessionRecordType.TAVERN_BRAWL:
          key = "GLOBAL_SOCIAL_TOAST_FRIEND_BRAWLISEUM_COMPLETE";
          break;
      }
      this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format(key, (object) "5ecaf0ff", (object) player.GetBestName(), (object) sessionRecord.Wins, (object) sessionRecord.Losses));
    }
    else
    {
      if (sessionRecord.Wins != 0U || sessionRecord.Losses != 0U)
        return;
      switch (sessionRecord.SessionRecordType)
      {
        case SessionRecordType.ARENA:
          key = "GLOBAL_SOCIAL_TOAST_FRIEND_ARENA_START";
          break;
        case SessionRecordType.HEROIC_BRAWL:
          key = "GLOBAL_SOCIAL_TOAST_FRIEND_HEROIC_BRAWL_START";
          break;
        case SessionRecordType.TAVERN_BRAWL:
          key = "GLOBAL_SOCIAL_TOAST_FRIEND_BRAWLISEUM_START";
          break;
        case SessionRecordType.DUELS:
          key = "GLOBAL_SOCIAL_TOAST_FRIEND_DUEL_START";
          break;
      }
      this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format(key, (object) "5ecaf0ff", (object) player.GetBestName()));
    }
  }

  private void CheckForCardOpened(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    string cardsOpened = newPlayerAccount.GetCardsOpened();
    if (string.IsNullOrEmpty(cardsOpened) || cardsOpened == oldPlayerAccount.GetCardsOpened())
      return;
    BnetGameAccountId id = oldPlayerAccount.GetId();
    string str;
    if (!this.m_lastOpenedLegendary.TryGetValue(id, out str))
    {
      this.m_lastOpenedLegendary[id] = cardsOpened;
    }
    else
    {
      this.m_lastOpenedLegendary[id] = cardsOpened;
      if (str == cardsOpened)
        return;
      string[] strArray = cardsOpened.Split(',');
      if (strArray.Length != 2)
        return;
      EntityDef entityDef = DefLoader.Get().GetEntityDef(strArray[0]);
      TAG_PREMIUM result;
      if (entityDef == null || !Enum.TryParse<TAG_PREMIUM>(strArray[1], out result))
        return;
      if (result == TAG_PREMIUM.GOLDEN)
        this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_GOLDEN_LEGENDARY", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) entityDef.GetName(), (object) "ffd200"));
      else
        this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_LEGENDARY", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) entityDef.GetName(), (object) "ff9c00"));
    }
  }

  private bool CheckForHigherRankForFormat(
    PegasusShared.FormatType format,
    MedalInfoTranslator currentMedalInfo,
    MedalInfoTranslator lastKnownMedalInfo,
    out TranslatedMedalInfo rankToShowToastFor)
  {
    rankToShowToastFor = (TranslatedMedalInfo) null;
    TranslatedMedalInfo currentMedal1 = currentMedalInfo.GetCurrentMedal(format);
    TranslatedMedalInfo currentMedal2 = lastKnownMedalInfo.GetCurrentMedal(format);
    if (!currentMedal1.IsValid() || !currentMedal2.IsValid() || currentMedal1.LeagueConfig.LeagueLevel < currentMedal2.LeagueConfig.LeagueLevel)
      return false;
    int num = 1;
    if (currentMedal1.LeagueConfig.LeagueLevel == currentMedal2.LeagueConfig.LeagueLevel)
    {
      if (currentMedal1.starLevel <= currentMedal2.starLevel)
        return false;
      num = currentMedal2.starLevel + 1;
    }
    for (int starLevel = currentMedal1.starLevel; starLevel >= num; --starLevel)
    {
      LeagueRankDbfRecord leagueRankRecord = RankMgr.Get().GetLeagueRankRecord(currentMedal1.leagueId, starLevel);
      if (leagueRankRecord == null)
        return false;
      if (leagueRankRecord.ShowToastOnAttained)
      {
        rankToShowToastFor = MedalInfoTranslator.CreateTranslatedMedalInfo(format, leagueRankRecord.LeagueId, leagueRankRecord.StarLevel, 0);
        break;
      }
    }
    return true;
  }

  private void CheckForNewRank(BnetPlayer player)
  {
    MedalInfoTranslator rankPresenceField = RankMgr.Get().GetRankedMedalFromRankPresenceField(player);
    if (rankPresenceField == null || !rankPresenceField.IsDisplayable())
      return;
    BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
    if (!this.m_lastKnownMedals.ContainsKey(hearthstoneGameAccountId))
    {
      this.m_lastKnownMedals[hearthstoneGameAccountId] = rankPresenceField;
    }
    else
    {
      MedalInfoTranslator lastKnownMedal = this.m_lastKnownMedals[hearthstoneGameAccountId];
      TranslatedMedalInfo rankToShowToastFor1;
      int num1 = this.CheckForHigherRankForFormat(PegasusShared.FormatType.FT_STANDARD, rankPresenceField, lastKnownMedal, out rankToShowToastFor1) ? 1 : 0;
      TranslatedMedalInfo rankToShowToastFor2;
      bool flag1 = this.CheckForHigherRankForFormat(PegasusShared.FormatType.FT_WILD, rankPresenceField, lastKnownMedal, out rankToShowToastFor2);
      TranslatedMedalInfo rankToShowToastFor3;
      bool flag2 = this.CheckForHigherRankForFormat(PegasusShared.FormatType.FT_CLASSIC, rankPresenceField, lastKnownMedal, out rankToShowToastFor3);
      int num2 = flag1 ? 1 : 0;
      if ((num1 | num2 | (flag2 ? 1 : 0)) != 0)
        this.m_lastKnownMedals[hearthstoneGameAccountId] = rankPresenceField;
      if (rankToShowToastFor3 != null)
      {
        if (rankToShowToastFor3.IsLegendRank())
          this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_RANK_LEGEND_CLASSIC", (object) "5ecaf0ff", (object) player.GetBestName()));
        else
          this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_RANK_EARNED_CLASSIC", (object) "5ecaf0ff", (object) player.GetBestName(), (object) rankToShowToastFor3.GetRankName()));
      }
      else if (rankToShowToastFor1 != null)
      {
        if (rankToShowToastFor1.IsLegendRank())
          this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_RANK_LEGEND", (object) "5ecaf0ff", (object) player.GetBestName()));
        else
          this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_RANK_EARNED", (object) "5ecaf0ff", (object) player.GetBestName(), (object) rankToShowToastFor1.GetRankName()));
      }
      else
      {
        if (rankToShowToastFor2 == null)
          return;
        if (rankToShowToastFor2.IsLegendRank())
          this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_RANK_LEGEND_WILD", (object) "5ecaf0ff", (object) player.GetBestName()));
        else
          this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_RANK_EARNED_WILD", (object) "5ecaf0ff", (object) player.GetBestName(), (object) rankToShowToastFor2.GetRankName()));
      }
    }
  }

  private void CheckForMissionComplete(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (newPlayerAccount.GetTutorialBeaten() == oldPlayerAccount.GetTutorialBeaten() || newPlayerAccount.GetTutorialBeaten() != 1)
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ILLIDAN_COMPLETE", (object) "5ecaf0ff", (object) newPlayer.GetBestName()));
  }

  private void CheckForMageLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetMageLevel(), newPlayerAccount.GetMageLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_MAGE_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetMageLevel()));
  }

  private void CheckForPaladinLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetPaladinLevel(), newPlayerAccount.GetPaladinLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_PALADIN_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetPaladinLevel()));
  }

  private void CheckForDruidLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetDruidLevel(), newPlayerAccount.GetDruidLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_DRUID_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetDruidLevel()));
  }

  private void CheckForRogueLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetRogueLevel(), newPlayerAccount.GetRogueLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ROGUE_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetRogueLevel()));
  }

  private void CheckForHunterLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetHunterLevel(), newPlayerAccount.GetHunterLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_HUNTER_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetHunterLevel()));
  }

  private void CheckForShamanLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetShamanLevel(), newPlayerAccount.GetShamanLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_SHAMAN_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetShamanLevel()));
  }

  private void CheckForWarriorLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetWarriorLevel(), newPlayerAccount.GetWarriorLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_WARRIOR_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetWarriorLevel()));
  }

  private void CheckForWarlockLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetWarlockLevel(), newPlayerAccount.GetWarlockLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_WARLOCK_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetWarlockLevel()));
  }

  private void CheckForPriestLevelChanged(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    if (!this.ShouldToastThisLevel(oldPlayerAccount.GetPriestLevel(), newPlayerAccount.GetPriestLevel()))
      return;
    this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_PRIEST_LEVEL", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) newPlayerAccount.GetPriestLevel()));
  }

  private void CheckForAchievementCompleted(
    BnetGameAccount oldPlayerAccount,
    BnetGameAccount newPlayerAccount,
    BnetPlayer newPlayer)
  {
    BnetGameAccountId id = oldPlayerAccount.GetId();
    int lastAchievement = newPlayerAccount.GetLastAchievement();
    if (lastAchievement == 0)
      return;
    int num;
    if (!this.m_lastAchievementId.TryGetValue(id, out num))
    {
      this.m_lastAchievementId[id] = lastAchievement;
    }
    else
    {
      if (num == lastAchievement)
        return;
      this.m_lastAchievementId[id] = lastAchievement;
      AchievementDataModel modelFromSection = AchievementManager.Get().GetAchievementDataModelFromSection(lastAchievement);
      if (modelFromSection == null)
        return;
      string textArg;
      if (modelFromSection.Tier == 1 && modelFromSection.NextTierID == 0)
        textArg = GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ACHIEVEMENT", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) modelFromSection.Name);
      else
        textArg = GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ACHIEVEMENT_TIER", (object) "5ecaf0ff", (object) newPlayer.GetBestName(), (object) modelFromSection.Name, (object) modelFromSection.Tier);
      this.AddToast(UserAttentionBlocker.NONE, textArg);
    }
  }

  private bool ShouldToastThisLevel(int oldLevel, int newLevel) => oldLevel != newLevel && (newLevel == 20 || newLevel == 30 || newLevel == 40 || newLevel == 50 || newLevel == 60);

  private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
  {
    if (!DemoMgr.Get().IsSocialEnabled())
      return;
    List<BnetInvitation> addedReceivedInvites = changelist.GetAddedReceivedInvites();
    if (addedReceivedInvites == null)
      return;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (myPlayer != null && myPlayer.IsBusy())
      return;
    foreach (BnetInvitation bnetInvitation in addedReceivedInvites)
    {
      BnetPlayer recentOpponent = FriendMgr.Get().GetRecentOpponent();
      if (recentOpponent != null && recentOpponent.HasAccount(bnetInvitation.GetInviterId()))
        this.AddToast(UserAttentionBlocker.NONE, GameStrings.Get("GLOBAL_SOCIAL_TOAST_RECENT_OPPONENT_FRIEND_REQUEST"));
      else
        this.AddToast(UserAttentionBlocker.NONE, bnetInvitation.GetInviterName(), SocialToastMgr.TOAST_TYPE.FRIEND_INVITE);
    }
  }

  private void ShutdownHandler(int minutes) => this.AddToast(UserAttentionBlocker.ALL, GameStrings.Format("GLOBAL_SHUTDOWN_TOAST", (object) "f61f1fff", (object) minutes), SocialToastMgr.TOAST_TYPE.DEFAULT, 3.5f);

  public enum TOAST_TYPE
  {
    DEFAULT,
    FRIEND_ONLINE,
    FRIEND_OFFLINE,
    FRIEND_INVITE,
    HEALTHY_GAMING,
    HEALTHY_GAMING_OVER_THRESHOLD,
    FRIEND_ARENA_COMPLETE,
    SPECTATOR_INVITE_SENT,
    SPECTATOR_INVITE_RECEIVED,
    SPECTATOR_ADDED,
    SPECTATOR_REMOVED,
    FIRESIDE_GATHERING_IS_HERE_REMINDER,
  }

  private class ToastArgs
  {
    public string m_message;
    public float m_displayTime;
    public bool m_playSound;

    public ToastArgs(string message, float displayTime, bool playSound)
    {
      this.m_message = message;
      this.m_displayTime = displayTime;
      this.m_playSound = playSound;
    }
  }

  private class LastOnlineTracker
  {
    public float m_localLastOnlineTime;
    public Processor.ScheduledCallback m_callback;
  }
}
