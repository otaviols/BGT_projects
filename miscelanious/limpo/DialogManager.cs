using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using SpectatorProto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class DialogManager : MonoBehaviour
{
  private static DialogManager s_instance;
  private Queue<DialogManager.DialogRequest> m_dialogRequests = new Queue<DialogManager.DialogRequest>();
  private DialogBase m_currentDialog;
  private bool m_loadingDialog;
  private bool m_isReadyForSeasonEndPopup;
  private bool m_waitingToShowSeasonEndDialog;
  private List<long> m_handledMedalNoticeIDs = new List<long>();
  public List<DialogManager.DialogTypeMapping> m_typeMapping = new List<DialogManager.DialogTypeMapping>();

  public static event System.Action OnStarted;

  public event System.Action OnDialogShown;

  public event System.Action OnDialogHidden;

  private void Awake() => DialogManager.s_instance = this;

  private void Start()
  {
    LoginManager.Get().OnInitialClientStateReceived += new System.Action(this.HandleSeasonEnd);
    if (DialogManager.OnStarted == null)
      return;
    DialogManager.OnStarted();
  }

  private void OnDestroy()
  {
    NetCache service;
    if (ServiceManager.TryGet<NetCache>(out service))
      service.RemoveNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
    if (LoginManager.Get() != null)
      LoginManager.Get().OnInitialClientStateReceived -= new System.Action(this.HandleSeasonEnd);
    DialogManager.s_instance = (DialogManager) null;
  }

  public void HandleSeasonEnd()
  {
    NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
    if (netObject != null)
      this.MaybeShowSeasonEndDialog(netObject.Notices, false);
    NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
  }

  public static DialogManager Get() => DialogManager.s_instance;

  public void GoBack()
  {
    if (!(bool) (UnityEngine.Object) this.m_currentDialog)
      return;
    this.m_currentDialog.GoBack();
  }

  public void ReadyForSeasonEndPopup(bool ready) => this.m_isReadyForSeasonEndPopup = ready;

  public void ClearHandledMedalNotices() => this.m_handledMedalNoticeIDs.Clear();

  public bool HandleKeyboardInput() => InputCollection.GetKeyUp(KeyCode.Escape) && (bool) (UnityEngine.Object) this.m_currentDialog && this.m_currentDialog.HandleKeyboardInput();

  public bool AddToQueue(DialogManager.DialogRequest request)
  {
    UserAttentionBlocker attentionCategory = request == null ? UserAttentionBlocker.NONE : request.m_attentionCategory;
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(attentionCategory, "DialogManager.AddToQueue:" + (request == null ? "null" : request.m_type.ToString())))
      return false;
    this.m_dialogRequests.Enqueue(request);
    this.UpdateQueue();
    return true;
  }

  private void UpdateQueue()
  {
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || (UnityEngine.Object) this.m_currentDialog != (UnityEngine.Object) null || this.m_loadingDialog || this.m_dialogRequests.Count == 0)
      return;
    DialogManager.DialogRequest request = this.m_dialogRequests.Peek();
    if (!UserAttentionManager.CanShowAttentionGrabber(request.m_attentionCategory, "DialogManager.UpdateQueue:" + (object) request.m_attentionCategory))
      Processor.ScheduleCallback(0.5f, false, (Processor.ScheduledCallback) (userData => this.UpdateQueue()));
    else
      this.LoadPopup(request);
  }

  public void ShowPopup(
    AlertPopup.PopupInfo info,
    DialogManager.DialogProcessCallback callback,
    object userData)
  {
    UserAttentionBlocker attentionCategory = info == null ? UserAttentionBlocker.NONE : info.m_attentionCategory;
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(attentionCategory, "DialogManager.ShowPopup:" + (info == null ? "null" : info.m_id + ":" + info.m_attentionCategory.ToString())))
      return;
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.ALERT,
      m_attentionCategory = attentionCategory,
      m_info = (object) info,
      m_callback = callback,
      m_userData = userData
    });
  }

  public void ShowPopup(AlertPopup.PopupInfo info, DialogManager.DialogProcessCallback callback) => this.ShowPopup(info, callback, (object) null);

  public void ShowPopup(AlertPopup.PopupInfo info) => this.ShowPopup(info, (DialogManager.DialogProcessCallback) null, (object) null);

  public bool ShowUniquePopup(
    AlertPopup.PopupInfo info,
    DialogManager.DialogProcessCallback callback,
    object userData)
  {
    UserAttentionBlocker attentionCategory = info == null ? UserAttentionBlocker.NONE : info.m_attentionCategory;
    if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(attentionCategory, "DialogManager.ShowUniquePopup:" + (info == null ? "null" : info.m_id + ":" + info.m_attentionCategory.ToString())))
      return false;
    if (info != null && !string.IsNullOrEmpty(info.m_id))
    {
      foreach (DialogManager.DialogRequest dialogRequest in this.m_dialogRequests)
      {
        if (dialogRequest.m_type == DialogManager.DialogType.ALERT && ((AlertPopup.PopupInfo) dialogRequest.m_info).m_id == info.m_id)
          return false;
      }
    }
    this.ShowPopup(info, callback, userData);
    return true;
  }

  public bool ShowUniquePopup(
    AlertPopup.PopupInfo info,
    DialogManager.DialogProcessCallback callback)
  {
    return this.ShowUniquePopup(info, callback, (object) null);
  }

  public bool ShowUniquePopup(AlertPopup.PopupInfo info) => this.ShowUniquePopup(info, (DialogManager.DialogProcessCallback) null, (object) null);

  public void ShowMessageOfTheDay(string message) => this.ShowPopup(new AlertPopup.PopupInfo()
  {
    m_text = message
  });

  public void RemoveUniquePopupRequestFromQueue(string id)
  {
    if (string.IsNullOrEmpty(id))
      return;
    foreach (DialogManager.DialogRequest dialogRequest in this.m_dialogRequests)
    {
      if (dialogRequest.m_info is AlertPopup.PopupInfo && ((AlertPopup.PopupInfo) dialogRequest.m_info).m_id == id)
      {
        this.m_dialogRequests = new Queue<DialogManager.DialogRequest>(this.m_dialogRequests.Where<DialogManager.DialogRequest>((Func<DialogManager.DialogRequest, bool>) (r => r.m_info != null && r.m_info.GetType() == typeof (AlertPopup.PopupInfo) && ((AlertPopup.PopupInfo) r.m_info).m_id != id)));
        break;
      }
    }
  }

  public bool WaitingToShowSeasonEndDialog() => this.m_waitingToShowSeasonEndDialog || (UnityEngine.Object) this.m_currentDialog != (UnityEngine.Object) null && this.m_currentDialog is SeasonEndDialog || this.m_dialogRequests.FirstOrDefault<DialogManager.DialogRequest>((Func<DialogManager.DialogRequest, bool>) (obj => obj.m_type == DialogManager.DialogType.SEASON_END)) != null;

  public IEnumerator<IAsyncJobResult> Job_WaitForSeasonEndPopup()
  {
    this.ReadyForSeasonEndPopup(true);
    while (this.WaitingToShowSeasonEndDialog())
      yield return (IAsyncJobResult) null;
  }

  public void ShowFriendlyChallenge(
    FormatType formatType,
    BnetPlayer challenger,
    bool challengeIsTavernBrawl,
    PartyType partyType,
    PartyQuestInfo questInfo,
    FriendlyChallengeDialog.ResponseCallback responseCallback,
    DialogManager.DialogProcessCallback callback)
  {
    DialogManager.DialogRequest request = new DialogManager.DialogRequest();
    if (challengeIsTavernBrawl)
    {
      request.m_type = DialogManager.DialogType.TAVERN_BRAWL_CHALLENGE;
    }
    else
    {
      switch (partyType)
      {
        case PartyType.BATTLEGROUNDS_PARTY:
          request.m_type = DialogManager.DialogType.BACON_CHALLENGE;
          break;
        case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
          request.m_type = DialogManager.DialogType.MERCENARIES_FRIENDLY_CHALLENGE;
          break;
        case PartyType.MERCENARIES_COOP_PARTY:
          request.m_type = DialogManager.DialogType.MERCENARIES_COOP_CHALLENGE;
          break;
        default:
          request.m_type = DialogManager.DialogType.FRIENDLY_CHALLENGE;
          break;
      }
    }
    request.m_info = (object) new FriendlyChallengeDialog.Info()
    {
      m_formatType = formatType,
      m_challenger = challenger,
      m_partyType = partyType,
      m_questInfo = questInfo,
      m_callback = responseCallback
    };
    request.m_callback = callback;
    this.AddToQueue(request);
  }

  public void ShowBattlegroundsSuggestion(
    BnetGameAccountId playerToInviteGameAccountId,
    string playerToInviteName,
    BnetGameAccountId suggesterGameAccountId,
    string suggesterName,
    BattlegroundsSuggestDialog.ResponseCallback responseCallback)
  {
    DialogManager.DialogRequest request = new DialogManager.DialogRequest();
    request.m_type = DialogManager.DialogType.BATTLEGROUNDS_SUGGESTION;
    BattlegroundsSuggestDialog.Info info = new BattlegroundsSuggestDialog.Info();
    info.PlayerToInviteGameAccountId = playerToInviteGameAccountId;
    info.PlayerToInviteName = playerToInviteName;
    info.SuggesterGameAccountId = suggesterGameAccountId;
    info.SuggesterName = suggesterName;
    info.Callback = responseCallback;
    info.m_id = string.Format("partysuggestion_{0}", (object) playerToInviteGameAccountId.Low);
    request.m_info = (object) info;
    this.AddToQueue(request);
  }

  public void ShowBattlegroundsLuckyDrawEndSoonPopup(
    LuckyDrawDataModel dataModel,
    DialogManager.DialogProcessCallback cb)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.BATTLEGROUNDS_LUCKYDRAW_END_SOON,
      m_isWidget = true,
      m_dataModel = (IDataModel) dataModel,
      m_callback = cb
    });
  }

  public void ShowPrivacyPolicyPopup(
    PrivacyPolicyPopup.ResponseCallback responseCallback,
    DialogManager.DialogProcessCallback callback)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.PRIVACY_POLICY,
      m_info = (object) new PrivacyPolicyPopup.Info()
      {
        m_callback = responseCallback
      },
      m_callback = callback
    });
  }

  public void ShowExistingAccountPopup(
    ExistingAccountPopup.ResponseCallback responseCallback,
    DialogManager.DialogProcessCallback callback,
    bool useCNStyle)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = useCNStyle ? DialogManager.DialogType.EXISTING_ACCOUNT_CN : DialogManager.DialogType.EXISTING_ACCOUNT,
      m_info = (object) new ExistingAccountPopup.Info()
      {
        m_callback = responseCallback
      },
      m_callback = callback
    });
  }

  public void ShowTavernBrawlChoiceDialog(
    FiresideBrawlChoiceDialog.ResponseCallback callback)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.TAVERN_BRAWL_CHOICE,
      m_info = (object) new FiresideBrawlChoiceDialog.Info()
      {
        m_callback = callback
      },
      m_callback = (DialogManager.DialogProcessCallback) null
    });
  }

  public void ShowFiresideOKDialog() => this.AddToQueue(new DialogManager.DialogRequest()
  {
    m_type = DialogManager.DialogType.FIRESIDE_BRAWL_OK
  });

  public void ShowFiresideGatheringNearbyDialog(
    FiresideGatheringJoinDialog.ResponseCallback callback)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.FIRESIDE_GATHERING_JOIN,
      m_info = (object) new FiresideGatheringJoinDialog.Info()
      {
        m_callback = callback
      },
      m_callback = (DialogManager.DialogProcessCallback) null
    });
  }

  private FiresideGatheringLocationHelperDialog.Info CreateFiresideGatheringLocationHelperDialogInfo(
    System.Action callback)
  {
    return new FiresideGatheringLocationHelperDialog.Info()
    {
      m_callback = callback,
      m_isInnkeeperSetup = false,
      m_gpsOffIntroText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_TURN_ON_GPS_BODY"),
      m_wifiOffIntroText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_TURN_ON_WIFI_BODY"),
      m_waitingForWifiText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_CONNECT_TO_WIFI_BODY"),
      m_wifiConfirmText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_CONNECT_TO_WIFI_SSID_CONFIRM_TITLE")
    };
  }

  public void ShowFiresideGatheringLocationHelperDialog(System.Action callback)
  {
    FiresideGatheringLocationHelperDialog.Info helperDialogInfo = this.CreateFiresideGatheringLocationHelperDialogInfo(callback);
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.FIRESIDE_LOCATION_HELPER,
      m_info = (object) helperDialogInfo,
      m_callback = (DialogManager.DialogProcessCallback) null
    });
  }

  public void ShowFiresideGatheringCheckInFailedDialog()
  {
    FiresideGatheringLocationHelperDialog.Info helperDialogInfo = this.CreateFiresideGatheringLocationHelperDialogInfo((System.Action) null);
    helperDialogInfo.m_isCheckInFailure = true;
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.FIRESIDE_LOCATION_HELPER,
      m_info = (object) helperDialogInfo,
      m_callback = (DialogManager.DialogProcessCallback) null
    });
  }

  public void ShowFiresideGatheringInnkeeperSetupHelperDialog(System.Action callback) => this.AddToQueue(new DialogManager.DialogRequest()
  {
    m_type = DialogManager.DialogType.FIRESIDE_LOCATION_HELPER,
    m_info = (object) new FiresideGatheringLocationHelperDialog.Info()
    {
      m_callback = callback,
      m_isInnkeeperSetup = true,
      m_gpsOffIntroText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_INNKEEPER_TURN_ON_GPS_BODY"),
      m_wifiOffIntroText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_INNKEEPER_TURN_ON_WIFI_BODY"),
      m_waitingForWifiText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_INKEEPER_CONNECT_TO_WIFI_BODY"),
      m_wifiConfirmText = GameStrings.Get("GLUE_FIRESIDE_GATHERING_INNKEEPER_CONNECT_TO_WIFI_SSID_CONFIRM_TITLE")
    },
    m_callback = (DialogManager.DialogProcessCallback) null
  });

  public void ShowFiresideGatheringFindEventDialog(
    FiresideGatheringFindEventDialog.ResponseCallback callback)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.FIRESIDE_FIND_EVENT,
      m_info = (object) new FiresideGatheringFindEventDialog.Info()
      {
        m_callback = callback
      },
      m_callback = (DialogManager.DialogProcessCallback) null
    });
  }

  public void ShowFiresideGatheringInnkeeperSetupDialog(
    FiresideGatheringInnkeeperSetupDialog.ResponseCallback callback,
    string tavernName)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.FIRESIDE_INNKEEPER_SETUP,
      m_info = (object) new FiresideGatheringInnkeeperSetupDialog.Info()
      {
        m_callback = callback,
        m_tavernName = tavernName
      },
      m_callback = (DialogManager.DialogProcessCallback) null
    });
  }

  public void ShowLeaguePromoteSelfManuallyDialog(
    LeaguePromoteSelfManuallyDialog.ResponseCallback callback)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.LEAGUE_PROMOTE_SELF_MANUALLY,
      m_info = (object) new LeaguePromoteSelfManuallyDialog.Info()
      {
        m_callback = callback
      },
      m_callback = (DialogManager.DialogProcessCallback) null
    });
  }

  public void ShowCardListPopup(UserAttentionBlocker attentionCategory, CardListPopup.Info info) => this.AddToQueue(new DialogManager.DialogRequest()
  {
    m_type = DialogManager.DialogType.CARD_LIST,
    m_attentionCategory = attentionCategory,
    m_info = (object) info
  });

  public void ShowSetRotationTutorialPopup(
    UserAttentionBlocker attentionCategory,
    SetRotationRotatedBoostersPopup.SetRotationRotatedBoostersPopupInfo info)
  {
    info.m_prefabAssetRefs.Add((string) new AssetReference("SetRotationRotatedBoostersPopup.prefab:2a1c1ce78c98c1e418039a479c8ddce4"));
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.GENERIC_BASIC_POPUP,
      m_attentionCategory = attentionCategory,
      m_info = (object) info,
      m_isWidget = true
    });
  }

  public void ShowOutstandingDraftTicketPopup(
    UserAttentionBlocker attentionCategory,
    OutstandingDraftTicketDialog.Info info)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.OUTSTANDING_DRAFT_TICKETS,
      m_attentionCategory = attentionCategory,
      m_info = (object) info
    });
  }

  public void ShowFreeArenaWinPopup(
    UserAttentionBlocker attentionCategory,
    FreeArenaWinDialog.Info info)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.FREE_ARENA_WIN,
      m_attentionCategory = attentionCategory,
      m_info = (object) info
    });
  }

  public bool ShowArenaSeasonPopup(
    UserAttentionBlocker attentionCategory,
    BasicPopup.PopupInfo info)
  {
    return this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.ARENA_SEASON,
      m_attentionCategory = attentionCategory,
      m_info = (object) info
    });
  }

  public void ShowLoginPopupSequenceBasicPopup(
    UserAttentionBlocker attentionCategory,
    LoginPopupSequencePopup.Info info)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.LOGIN_POPUP_SEQUENCE_BASIC,
      m_attentionCategory = attentionCategory,
      m_info = (object) info,
      m_prefabAssetReferenceOverride = info.m_prefabAssetReference
    });
  }

  public void ShowMultiPagePopup(UserAttentionBlocker attentionCategory, MultiPagePopup.Info info) => this.AddToQueue(new DialogManager.DialogRequest()
  {
    m_type = DialogManager.DialogType.MULTI_PAGE_POPUP,
    m_attentionCategory = attentionCategory,
    m_info = (object) info,
    m_prefabAssetReferenceOverride = "MultiPagePopup.prefab:a9b6df0282662ed449031d34aa2ecfa7"
  });

  public bool ShowBasicPopup(UserAttentionBlocker attentionCategory, BasicPopup.PopupInfo info) => this.AddToQueue(new DialogManager.DialogRequest()
  {
    m_type = DialogManager.DialogType.GENERIC_BASIC_POPUP,
    m_attentionCategory = attentionCategory,
    m_info = (object) info
  });

  public bool ShowAssetDownloadPopup(AssetDownloadDialog.Info info) => this.AddToQueue(new DialogManager.DialogRequest()
  {
    m_type = DialogManager.DialogType.ASSET_DOWNLOAD,
    m_attentionCategory = UserAttentionBlocker.NONE,
    m_info = (object) info
  });

  public void ShowReconnectHelperDialog(System.Action reconnectSuccessCallback = null, System.Action goBackCallback = null) => this.AddToQueue(new DialogManager.DialogRequest()
  {
    m_type = DialogManager.DialogType.RECONNECT_HELPER,
    m_info = (object) new ReconnectHelperDialog.Info()
    {
      m_reconnectSuccessCallback = reconnectSuccessCallback,
      m_goBackCallback = goBackCallback
    },
    m_callback = (DialogManager.DialogProcessCallback) null
  });

  public void ShowClassUpcomingPopup() => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_showAlertIcon = false,
    m_alertTextAlignment = UberText.AlignmentOptions.Center,
    m_responseDisplay = AlertPopup.ResponseDisplay.OK,
    m_headerText = GameStrings.Get("GLUE_CLASS_UPCOMING_HEADER"),
    m_text = GameStrings.Get("GLUE_CLASS_UPCOMING_DESC")
  });

  public void ShowBonusStarsPopup(RankedPlayDataModel dataModel, System.Action onHiddenCallback)
  {
    RankedBonusStarsPopup.BonusStarsPopupInfo bonusStarsPopupInfo = new RankedBonusStarsPopup.BonusStarsPopupInfo()
    {
      m_onHiddenCallback = onHiddenCallback
    };
    DialogManager.DialogRequest request = new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.GENERIC_BASIC_POPUP,
      m_dataModel = (IDataModel) dataModel,
      m_info = (object) bonusStarsPopupInfo,
      m_isWidget = true
    };
    bonusStarsPopupInfo.m_prefabAssetRefs.Add((string) RankMgr.BONUS_STAR_POPUP_PREFAB);
    this.AddToQueue(request);
  }

  public void ShowRankedIntroPopUp(System.Action onHiddenCallback)
  {
    RankedIntroPopup.RankedIntroPopupInfo rankedIntroPopupInfo = new RankedIntroPopup.RankedIntroPopupInfo()
    {
      m_onHiddenCallback = onHiddenCallback
    };
    DialogManager.DialogRequest request = new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.GENERIC_BASIC_POPUP,
      m_info = (object) rankedIntroPopupInfo,
      m_isWidget = true
    };
    rankedIntroPopupInfo.m_prefabAssetRefs.Add((string) RankMgr.RANKED_INTRO_POPUP_PREFAB);
    this.AddToQueue(request);
  }

  public void ClearAllImmediately()
  {
    if ((UnityEngine.Object) this.m_currentDialog != (UnityEngine.Object) null)
    {
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_currentDialog.gameObject);
      this.m_currentDialog = (DialogBase) null;
    }
    this.m_dialogRequests.Clear();
  }

  public bool ShowingDialog() => (UnityEngine.Object) this.m_currentDialog != (UnityEngine.Object) null || this.m_dialogRequests.Count > 0;

  public bool ShowingHighPriorityDialog() => (UnityEngine.Object) this.m_currentDialog != (UnityEngine.Object) null && this.m_currentDialog.gameObject.layer == 27;

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList) => this.MaybeShowSeasonEndDialog(newNotices, !isInitialNoticeList);

  private void MaybeShowSeasonEndDialog(
    List<NetCache.ProfileNotice> newNotices,
    bool fromOutOfBandNotice)
  {
    newNotices.Sort((Comparison<NetCache.ProfileNotice>) ((a, b) =>
    {
      if (a.Type != b.Type)
        return a.Type - b.Type;
      if (a.Origin != b.Origin)
        return a.Origin - b.Origin;
      return a.OriginData != b.OriginData ? (int) (a.OriginData - b.OriginData) : (int) (a.NoticeID - b.NoticeID);
    }));
    NetCache.ProfileNotice latestMedalNotice = this.MaybeShowSeasonEndDialog_GetLatestMedalNotice(newNotices);
    if (latestMedalNotice == null || !(latestMedalNotice is NetCache.ProfileNoticeMedal profileNoticeMedal) || this.m_handledMedalNoticeIDs.Contains(profileNoticeMedal.NoticeID) || UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber("DialogManager.MaybeShowSeasonEndDialog"))
      return;
    this.m_handledMedalNoticeIDs.Add(profileNoticeMedal.NoticeID);
    if (ReturningPlayerMgr.Get().SuppressOldPopups)
    {
      Log.ReturningPlayer.Print("Suppressing popup for Season End Dialogue {0} due to being a Returning Player!");
      Network.Get().AckNotice(profileNoticeMedal.NoticeID);
    }
    else
    {
      if (fromOutOfBandNotice)
      {
        NetCache.Get().RefreshNetObject<NetCache.NetCacheMedalInfo>();
        NetCache.Get().ReloadNetObject<NetCache.NetCacheRewardProgress>();
      }
      this.StartCoroutine(this.ShowSeasonEndDialogWhenReady(new DialogManager.DialogRequest()
      {
        m_type = DialogManager.DialogType.SEASON_END,
        m_info = (object) new DialogManager.SeasonEndDialogRequestInfo()
        {
          m_noticeMedal = profileNoticeMedal
        }
      }));
    }
  }

  private NetCache.ProfileNotice MaybeShowSeasonEndDialog_GetLatestMedalNotice(
    List<NetCache.ProfileNotice> newNotices)
  {
    List<NetCache.ProfileNotice> source1 = new List<NetCache.ProfileNotice>((IEnumerable<NetCache.ProfileNotice>) newNotices);
    IEnumerable<NetCache.ProfileNotice> source2 = source1.Where<NetCache.ProfileNotice>((Func<NetCache.ProfileNotice, bool>) (notice => notice.Type == NetCache.ProfileNotice.NoticeType.GAINED_MEDAL));
    IEnumerable<NetCache.ProfileNotice> enumerable = source1.Where<NetCache.ProfileNotice>((Func<NetCache.ProfileNotice, bool>) (notice => notice.Type == NetCache.ProfileNotice.NoticeType.BONUS_STARS));
    if (source2.Any<NetCache.ProfileNotice>())
    {
      long maxSeason = Math.Max(52L, source2.Max<NetCache.ProfileNotice>((Func<NetCache.ProfileNotice, long>) (n => n.OriginData)));
      source2.Where<NetCache.ProfileNotice>((Func<NetCache.ProfileNotice, bool>) (notice => notice.OriginData != maxSeason)).ForEach<NetCache.ProfileNotice>((System.Action<NetCache.ProfileNotice>) (notice => Network.Get().AckNotice(notice.NoticeID)));
      source2 = source2.Where<NetCache.ProfileNotice>((Func<NetCache.ProfileNotice, bool>) (notice => notice.OriginData == maxSeason));
      source2.Skip<NetCache.ProfileNotice>(1).ForEach<NetCache.ProfileNotice>((System.Action<NetCache.ProfileNotice>) (notice => Network.Get().AckNotice(notice.NoticeID)));
    }
    enumerable.ForEach<NetCache.ProfileNotice>((System.Action<NetCache.ProfileNotice>) (notice => Network.Get().AckNotice(notice.NoticeID)));
    return source2.FirstOrDefault<NetCache.ProfileNotice>();
  }

  private void LoadPopup(DialogManager.DialogRequest request)
  {
    List<string> stringList;
    if (request.m_info is BasicPopup.PopupInfo)
    {
      stringList = ((BasicPopup.PopupInfo) request.m_info).m_prefabAssetRefs;
    }
    else
    {
      stringList = new List<string>();
      stringList.Add(this.GetPrefabNameFromDialogRequest(request));
    }
    if (stringList == null || stringList.Count == 0 || string.IsNullOrEmpty(stringList[0]))
    {
      Error.AddDevFatal("DialogManager.LoadPopup() - no prefab to load for type={0} info={1} attnCategory={2} prefabName={3}", (object) request.m_type, request.m_info, (object) request.m_attentionCategory, stringList == null ? (object) "<null>" : (stringList.Count == 0 ? (object) "<empty>" : (object) (stringList[0] ?? "null")));
    }
    else
    {
      stringList.RemoveAll((Predicate<string>) (assetRef => string.IsNullOrEmpty(assetRef)));
      this.m_loadingDialog = true;
      DialogManager.PopupCallbackSharedData sharedData = new DialogManager.PopupCallbackSharedData(stringList.Count);
      for (int index = 0; index < stringList.Count; ++index)
        sharedData.m_loadedPrefabs.Add((GameObject) null);
      for (int index = 0; index < stringList.Count; ++index)
      {
        DialogManager.PopupCallbackData callbackData = new DialogManager.PopupCallbackData(sharedData, index);
        if (request.m_isWidget)
        {
          WidgetInstance widgetInstance = WidgetInstance.Create(stringList[index]);
          if (request.m_dataModel != null)
            widgetInstance.BindDataModel(request.m_dataModel, false);
          this.StartCoroutine(this.WaitForWidgetPopupReady((AssetReference) stringList[index], widgetInstance, (object) callbackData));
        }
        else
          AssetLoader.Get().InstantiatePrefab((AssetReference) stringList[index], new PrefabCallback<GameObject>(this.OnPopupLoaded), (object) callbackData);
      }
    }
  }

  private string GetPrefabNameFromDialogRequest(DialogManager.DialogRequest request)
  {
    if (!string.IsNullOrEmpty(request.m_prefabAssetReferenceOverride))
      return request.m_prefabAssetReferenceOverride;
    DialogManager.DialogTypeMapping dialogTypeMapping = this.m_typeMapping.Find((Predicate<DialogManager.DialogTypeMapping>) (x => x.m_type == request.m_type));
    if (dialogTypeMapping != null && dialogTypeMapping.m_prefabName != null)
      return dialogTypeMapping.m_prefabName;
    Error.AddDevFatal("DialogManager.GetPrefabNameFromDialogRequest() - unhandled dialog type {0}", (object) request.m_type);
    return (string) null;
  }

  private IEnumerator WaitForWidgetPopupReady(
    AssetReference assetRef,
    WidgetInstance widgetInstance,
    object callbackData)
  {
    if (!((UnityEngine.Object) widgetInstance == (UnityEngine.Object) null))
    {
      widgetInstance.Hide();
      while (!widgetInstance.IsReady || widgetInstance.IsChangingStates)
        yield return (object) null;
      this.OnPopupLoaded(assetRef, widgetInstance.gameObject, callbackData);
      widgetInstance.Show();
    }
  }

  private void OnPopupLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    DialogManager.DialogRequest dialogRequest = this.m_dialogRequests.Count == 0 ? (DialogManager.DialogRequest) null : this.m_dialogRequests.Peek();
    UserAttentionBlocker attentionCategory = dialogRequest == null ? UserAttentionBlocker.NONE : dialogRequest.m_attentionCategory;
    if (this.m_dialogRequests.Count == 0 || UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(attentionCategory, "DialogManager.OnPopupLoaded:" + (dialogRequest == null ? "null" : dialogRequest.m_type.ToString())))
    {
      this.m_loadingDialog = false;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
    }
    else
    {
      DialogManager.PopupCallbackData popupCallbackData = (DialogManager.PopupCallbackData) callbackData;
      popupCallbackData.m_sharedData.m_loadedPrefabs[popupCallbackData.m_index] = go;
      if (--popupCallbackData.m_sharedData.m_remainingToLoad > 0)
        return;
      this.m_loadingDialog = false;
      DialogManager.DialogRequest request = this.m_dialogRequests.Dequeue();
      GameObject gameObject = popupCallbackData.m_sharedData.m_loadedPrefabs.Count == 0 ? (GameObject) null : popupCallbackData.m_sharedData.m_loadedPrefabs[0];
      DialogBase dialog = (UnityEngine.Object) gameObject == (UnityEngine.Object) null ? (DialogBase) null : gameObject.GetComponentInChildren<DialogBase>();
      if ((UnityEngine.Object) dialog == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("DialogManager.OnPopupLoaded() - game object {0} has no DialogBase component (request_type={1} count prefabs loaded={2})", (object) go, (object) request.m_type, (object) popupCallbackData.m_sharedData.m_loadedPrefabs.Count));
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
        this.UpdateQueue();
      }
      else
      {
        for (int index = 1; index < popupCallbackData.m_sharedData.m_loadedPrefabs.Count; ++index)
        {
          GameObject loadedPrefab = popupCallbackData.m_sharedData.m_loadedPrefabs[index];
          if (!((UnityEngine.Object) loadedPrefab == (UnityEngine.Object) null))
            loadedPrefab.transform.SetParent(gameObject.transform, false);
        }
        this.ProcessRequest(request, dialog);
      }
    }
  }

  private void ProcessRequest(DialogManager.DialogRequest request, DialogBase dialog)
  {
    if (request.m_callback != null && !request.m_callback(dialog, request.m_userData))
    {
      this.UpdateQueue();
      UnityEngine.Object.Destroy((UnityEngine.Object) dialog.gameObject);
    }
    else
    {
      this.m_currentDialog = dialog;
      this.m_currentDialog.SetReadyToDestroyCallback(new DialogBase.ReadyToDestroyCallback(this.OnCurrentDialogHidden));
      if (request.m_type == DialogManager.DialogType.ALERT)
        this.ProcessAlertRequest(request, (AlertPopup) dialog);
      else if (request.m_type == DialogManager.DialogType.SEASON_END)
        this.ProcessMedalRequest(request, (SeasonEndDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.FRIENDLY_CHALLENGE || request.m_type == DialogManager.DialogType.TAVERN_BRAWL_CHALLENGE || request.m_type == DialogManager.DialogType.MERCENARIES_COOP_CHALLENGE || request.m_type == DialogManager.DialogType.MERCENARIES_FRIENDLY_CHALLENGE)
        this.ProcessFriendlyChallengeRequest(request, (FriendlyChallengeDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.BACON_CHALLENGE)
        this.ProcessBattlegroundsInviteRequest(request, (BattlegroundsInviteDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.BATTLEGROUNDS_SUGGESTION)
        this.ProcessBattlegroundsSuggestionRequest(request, (BattlegroundsSuggestDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.EXISTING_ACCOUNT || request.m_type == DialogManager.DialogType.EXISTING_ACCOUNT_CN)
        this.ProcessExistingAccountRequest(request, (ExistingAccountPopup) dialog);
      else if (request.m_type == DialogManager.DialogType.CARD_LIST)
        this.ProcessCardListRequest(request, (CardListPopup) dialog);
      else if (request.m_type == DialogManager.DialogType.TAVERN_BRAWL_CHOICE)
        this.ProcessFiresideBrawlChoiceRequest(request, (FiresideBrawlChoiceDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.FIRESIDE_BRAWL_OK)
        this.ProcessFiresideBrawlOkRequest(request, (FiresideBrawlOkDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.FIRESIDE_GATHERING_JOIN)
        this.ProcessFiresideGatheringNearbyRequest(request, (FiresideGatheringJoinDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.FIRESIDE_FIND_EVENT)
        this.ProcessFiresideGatheringFindEventRequest(request, (FiresideGatheringFindEventDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.FIRESIDE_LOCATION_HELPER)
        this.ProcessFiresideGatheringLocationHelperRequest(request, (FiresideGatheringLocationHelperDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.FIRESIDE_INNKEEPER_SETUP)
        this.ProcessFiresideGatheringInnkeeperSetupRequest(request, (FiresideGatheringInnkeeperSetupDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.LEAGUE_PROMOTE_SELF_MANUALLY)
        this.ProcessLeaguePromoteSelfManuallyRequest(request, (LeaguePromoteSelfManuallyDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.OUTSTANDING_DRAFT_TICKETS)
        this.ProcessOutstandingDraftTicketDialog(request, (OutstandingDraftTicketDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.FREE_ARENA_WIN)
        this.ProcessFreeArenaWinDialog(request, (FreeArenaWinDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.GENERIC_BASIC_POPUP || request.m_type == DialogManager.DialogType.ARENA_SEASON)
        this.ProcessBasicPopupRequest(request, (BasicPopup) dialog);
      else if (request.m_type == DialogManager.DialogType.ASSET_DOWNLOAD)
        this.ProcessAssetDownloadRequest(request, (AssetDownloadDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.RECONNECT_HELPER)
        this.ProcessReconnectRequest(request, (ReconnectHelperDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.LOGIN_POPUP_SEQUENCE_BASIC)
        this.ProcessLoginPopupSequenceBasicPopupRequest(request, (LoginPopupSequencePopup) dialog);
      else if (request.m_type == DialogManager.DialogType.MULTI_PAGE_POPUP)
        this.ProcessMultiPagePopupRequest(request, (MultiPagePopup) dialog);
      else if (request.m_type == DialogManager.DialogType.PRIVACY_POLICY)
        this.ProcessPrivacyPolicyRequest(request, (PrivacyPolicyPopup) dialog);
      else if (request.m_type == DialogManager.DialogType.MERCENARIES_SEASON_REWARDS)
        this.ProcessMercenariesSeasonRewardsDialog(request, (MercenariesSeasonRewardsDialog) dialog);
      else if (request.m_type == DialogManager.DialogType.MERCENARIES_ZONE_UNLOCK)
        this.ProcessMercenariesZoneUnlockDialog(request, (MercenariesZoneUnlockDialog) dialog);
      if (this.OnDialogShown == null)
        return;
      this.OnDialogShown();
    }
  }

  private void ProcessExistingAccountRequest(
    DialogManager.DialogRequest request,
    ExistingAccountPopup exAcctPopup)
  {
    exAcctPopup.SetInfo((ExistingAccountPopup.Info) request.m_info);
    exAcctPopup.Show();
  }

  private void ProcessAlertRequest(DialogManager.DialogRequest request, AlertPopup alertPopup)
  {
    AlertPopup.PopupInfo info = (AlertPopup.PopupInfo) request.m_info;
    alertPopup.SetInfo(info);
    alertPopup.Show();
  }

  private void ProcessFiresideBrawlChoiceRequest(
    DialogManager.DialogRequest request,
    FiresideBrawlChoiceDialog choicePopup)
  {
    FiresideBrawlChoiceDialog.Info info = (FiresideBrawlChoiceDialog.Info) request.m_info;
    choicePopup.SetInfo(info);
    choicePopup.Show();
  }

  private void ProcessFiresideBrawlOkRequest(
    DialogManager.DialogRequest request,
    FiresideBrawlOkDialog okPopup)
  {
    okPopup.Show();
  }

  private void ProcessFiresideGatheringNearbyRequest(
    DialogManager.DialogRequest request,
    FiresideGatheringJoinDialog choicePopup)
  {
    FiresideGatheringJoinDialog.Info info = (FiresideGatheringJoinDialog.Info) request.m_info;
    choicePopup.SetInfo(info);
    choicePopup.Show();
  }

  private void ProcessFiresideGatheringFindEventRequest(
    DialogManager.DialogRequest request,
    FiresideGatheringFindEventDialog choicePopup)
  {
    FiresideGatheringFindEventDialog.Info info = (FiresideGatheringFindEventDialog.Info) request.m_info;
    choicePopup.SetInfo(info);
    choicePopup.Show();
  }

  private void ProcessFiresideGatheringInnkeeperSetupRequest(
    DialogManager.DialogRequest request,
    FiresideGatheringInnkeeperSetupDialog choicePopup)
  {
    FiresideGatheringInnkeeperSetupDialog.Info info = (FiresideGatheringInnkeeperSetupDialog.Info) request.m_info;
    choicePopup.SetInfo(info);
    choicePopup.Show();
  }

  private void ProcessFiresideGatheringLocationHelperRequest(
    DialogManager.DialogRequest request,
    FiresideGatheringLocationHelperDialog fsgLocationHelperPopup)
  {
    FiresideGatheringLocationHelperDialog.Info info = (FiresideGatheringLocationHelperDialog.Info) request.m_info;
    fsgLocationHelperPopup.SetInfo(info);
    fsgLocationHelperPopup.Show();
  }

  private void ProcessBasicPopupRequest(DialogManager.DialogRequest request, BasicPopup basicPopup)
  {
    BasicPopup.PopupInfo info = (BasicPopup.PopupInfo) request.m_info;
    basicPopup.SetInfo(info);
    basicPopup.Show();
  }

  private void ProcessAssetDownloadRequest(
    DialogManager.DialogRequest request,
    AssetDownloadDialog dialog)
  {
    dialog.Show();
  }

  private void ProcessReconnectRequest(
    DialogManager.DialogRequest request,
    ReconnectHelperDialog dialog)
  {
    dialog.SetInfo((ReconnectHelperDialog.Info) request.m_info);
    dialog.Show();
  }

  private void ProcessMedalRequest(
    DialogManager.DialogRequest request,
    SeasonEndDialog seasonEndDialog)
  {
    if (request.m_isFake)
    {
      if (!(request.m_info is SeasonEndDialog.SeasonEndInfo info1))
        return;
    }
    else
    {
      DialogManager.SeasonEndDialogRequestInfo info2 = request.m_info as DialogManager.SeasonEndDialogRequestInfo;
      if (PopupDisplayManager.ShouldDisableNotificationOnLogin())
      {
        Network.Get().AckNotice(info2.m_noticeMedal.NoticeID);
        UIStatus.Get().AddInfo("Season Roll skipped due to disableLoginPopups", 5f);
        return;
      }
      info1 = new SeasonEndDialog.SeasonEndInfo();
      info1.m_noticesToAck.Add(info2.m_noticeMedal.NoticeID);
      info1.m_seasonID = (int) info2.m_noticeMedal.OriginData;
      info1.m_leagueId = info2.m_noticeMedal.LeagueId;
      info1.m_starLevelAtEndOfSeason = info2.m_noticeMedal.StarLevel;
      info1.m_bestStarLevelAtEndOfSeason = info2.m_noticeMedal.BestStarLevel;
      info1.m_legendIndex = info2.m_noticeMedal.LegendRank;
      info1.m_rankedRewards = info2.m_noticeMedal.Chest.Rewards;
      info1.m_formatType = info2.m_noticeMedal.FormatType;
      info1.m_wasLimitedByBestEverStarLevel = info2.m_noticeMedal.WasLimitedByBestEverStarLevel;
    }
    seasonEndDialog.Init(info1);
    seasonEndDialog.Show();
  }

  private void ProcessFriendlyChallengeRequest(
    DialogManager.DialogRequest request,
    FriendlyChallengeDialog friendlyChallengeDialog)
  {
    friendlyChallengeDialog.SetInfo((FriendlyChallengeDialog.Info) request.m_info);
    friendlyChallengeDialog.Show();
  }

  private void ProcessBattlegroundsInviteRequest(
    DialogManager.DialogRequest request,
    BattlegroundsInviteDialog battlegroundsInviteDialog)
  {
    battlegroundsInviteDialog.SetInfo((FriendlyChallengeDialog.Info) request.m_info);
    battlegroundsInviteDialog.Show();
  }

  private void ProcessBattlegroundsSuggestionRequest(
    DialogManager.DialogRequest request,
    BattlegroundsSuggestDialog battlegroundsSuggestDialog)
  {
    battlegroundsSuggestDialog.SetInfo((BattlegroundsSuggestDialog.Info) request.m_info);
    battlegroundsSuggestDialog.Show();
  }

  private void ProcessCardListRequest(
    DialogManager.DialogRequest request,
    CardListPopup cardListPopup)
  {
    CardListPopup.Info info = (CardListPopup.Info) request.m_info;
    cardListPopup.SetInfo(info);
    cardListPopup.Show();
  }

  private void ProcessSetRotationRotatedBoostersPopupRequest(
    DialogManager.DialogRequest request,
    SetRotationRotatedBoostersPopup setRotationTutorialDialog)
  {
    SetRotationRotatedBoostersPopup.SetRotationRotatedBoostersPopupInfo info = (SetRotationRotatedBoostersPopup.SetRotationRotatedBoostersPopupInfo) request.m_info;
    setRotationTutorialDialog.SetInfo((BasicPopup.PopupInfo) info);
    setRotationTutorialDialog.Show();
  }

  private void ProcessLeaguePromoteSelfManuallyRequest(
    DialogManager.DialogRequest request,
    LeaguePromoteSelfManuallyDialog leaguePromoteSelfManuallyDialog)
  {
    LeaguePromoteSelfManuallyDialog.Info info = (LeaguePromoteSelfManuallyDialog.Info) request.m_info;
    leaguePromoteSelfManuallyDialog.SetInfo(info);
    leaguePromoteSelfManuallyDialog.Show();
  }

  private void ProcessOutstandingDraftTicketDialog(
    DialogManager.DialogRequest request,
    OutstandingDraftTicketDialog outstandingDraftTicketDialog)
  {
    OutstandingDraftTicketDialog.Info info = (OutstandingDraftTicketDialog.Info) request.m_info;
    outstandingDraftTicketDialog.SetInfo(info);
    outstandingDraftTicketDialog.Show();
  }

  private void ProcessFreeArenaWinDialog(
    DialogManager.DialogRequest request,
    FreeArenaWinDialog freeArenaWinDialog)
  {
    FreeArenaWinDialog.Info info = (FreeArenaWinDialog.Info) request.m_info;
    freeArenaWinDialog.SetInfo(info);
    freeArenaWinDialog.Show();
  }

  private void ProcessLoginPopupSequenceBasicPopupRequest(
    DialogManager.DialogRequest request,
    LoginPopupSequencePopup loginPopupSequencePopup)
  {
    LoginPopupSequencePopup.Info info = (LoginPopupSequencePopup.Info) request.m_info;
    loginPopupSequencePopup.SetInfo(info);
    loginPopupSequencePopup.LoadAssetsAndShowWhenReady();
  }

  private void ProcessPrivacyPolicyRequest(
    DialogManager.DialogRequest request,
    PrivacyPolicyPopup privacyPolicyPopup)
  {
    privacyPolicyPopup.SetInfo((PrivacyPolicyPopup.Info) request.m_info);
    privacyPolicyPopup.Show();
  }

  private void ProcessMultiPagePopupRequest(
    DialogManager.DialogRequest request,
    MultiPagePopup multiPagePopup)
  {
    MultiPagePopup.Info info = (MultiPagePopup.Info) request.m_info;
    multiPagePopup.SetInfo(info);
    multiPagePopup.Show();
  }

  private void ProcessMercenariesSeasonRewardsDialog(
    DialogManager.DialogRequest request,
    MercenariesSeasonRewardsDialog dialog)
  {
    dialog.SetInfo((MercenariesSeasonRewardsDialog.Info) request.m_info);
    dialog.Show();
  }

  private void ProcessMercenariesZoneUnlockDialog(
    DialogManager.DialogRequest request,
    MercenariesZoneUnlockDialog dialog)
  {
    dialog.SetInfo((MercenariesZoneUnlockDialog.Info) request.m_info);
    dialog.Show();
  }

  private void OnCurrentDialogHidden(DialogBase dialog)
  {
    if ((UnityEngine.Object) dialog != (UnityEngine.Object) this.m_currentDialog)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentDialog.gameObject);
    this.m_currentDialog = (DialogBase) null;
    this.UpdateQueue();
    if (this.OnDialogHidden == null)
      return;
    this.OnDialogHidden();
  }

  private IEnumerator ShowSeasonEndDialogWhenReady(DialogManager.DialogRequest request)
  {
    this.m_waitingToShowSeasonEndDialog = true;
    while (!NetCache.Get().IsNetObjectAvailable<NetCache.NetCacheRewardProgress>() || !this.m_isReadyForSeasonEndPopup)
      yield return (object) null;
    while (SceneMgr.Get().IsTransitioning())
      yield return (object) null;
    while (SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB)
    {
      if ((SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT || SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN) && !SceneMgr.Get().IsTransitioning())
      {
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        break;
      }
      yield return (object) null;
    }
    while (SceneMgr.Get().IsTransitioning())
      yield return (object) null;
    this.AddToQueue(request);
    this.m_waitingToShowSeasonEndDialog = false;
  }

  public void ShowMercenariesSeasonRewardsDialog(
    NetCache.ProfileNoticeMercenariesSeasonRewards rewardNotice,
    System.Action doneCallback = null)
  {
    this.AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.MERCENARIES_SEASON_REWARDS,
      m_info = (object) new MercenariesSeasonRewardsDialog.Info()
      {
        m_noticeId = rewardNotice.NoticeID,
        m_rewards = Network.ConvertRewardChest(rewardNotice.Chest).Rewards,
        m_rewardAssetId = rewardNotice.RewardAssetId,
        m_doneCallback = doneCallback
      }
    });
  }

  public void ShowMercenariesZoneUnlockDialog(int zoneId, System.Action onCompleteCallback) => this.AddToQueue(new DialogManager.DialogRequest()
  {
    m_type = DialogManager.DialogType.MERCENARIES_ZONE_UNLOCK,
    m_info = (object) new MercenariesZoneUnlockDialog.Info()
    {
      m_zoneId = zoneId,
      m_onCompleteCallback = onCompleteCallback
    }
  });

  public delegate bool DialogProcessCallback(DialogBase dialog, object userData);

  public enum DialogType
  {
    ALERT,
    SEASON_END,
    FRIENDLY_CHALLENGE,
    TAVERN_BRAWL_CHALLENGE,
    EXISTING_ACCOUNT,
    CARD_LIST,
    STANDARD_COMING_SOON,
    ROTATION_TUTORIAL,
    HALL_OF_FAME,
    TAVERN_BRAWL_CHOICE,
    FIRESIDE_BRAWL_OK,
    FIRESIDE_GATHERING_JOIN,
    FIRESIDE_FIND_EVENT,
    FIRESIDE_LOCATION_HELPER,
    FIRESIDE_INNKEEPER_SETUP,
    RETURNING_PLAYER_OPT_OUT,
    OUTSTANDING_DRAFT_TICKETS,
    FREE_ARENA_WIN,
    ARENA_SEASON,
    ASSET_DOWNLOAD,
    LEAGUE_PROMOTE_SELF_MANUALLY,
    RECONNECT_HELPER,
    LOGIN_POPUP_SEQUENCE_BASIC,
    MULTI_PAGE_POPUP,
    GAME_MODES,
    BACON_CHALLENGE,
    PRIVACY_POLICY,
    MERCENARIES_COOP_CHALLENGE,
    MERCENARIES_FRIENDLY_CHALLENGE,
    MERCENARIES_SEASON_REWARDS,
    EXISTING_ACCOUNT_CN,
    MERCENARIES_ZONE_UNLOCK,
    BATTLEGROUNDS_SUGGESTION,
    BATTLEGROUNDS_LUCKYDRAW_END_SOON,
    GENERIC_BASIC_POPUP,
  }

  public class DialogRequest
  {
    public DialogManager.DialogType m_type;
    public UserAttentionBlocker m_attentionCategory;
    public object m_info;
    public DialogManager.DialogProcessCallback m_callback;
    public object m_userData;
    public string m_prefabAssetReferenceOverride;
    public bool m_isWidget;
    public IDataModel m_dataModel;
    public bool m_isFake;
  }

  [Serializable]
  public class DialogTypeMapping
  {
    public DialogManager.DialogType m_type;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string m_prefabName;
  }

  private class SeasonEndDialogRequestInfo
  {
    public NetCache.ProfileNoticeMedal m_noticeMedal;
  }

  private class PopupCallbackSharedData
  {
    public readonly List<GameObject> m_loadedPrefabs = new List<GameObject>();
    public int m_remainingToLoad;

    public PopupCallbackSharedData(int count) => this.m_remainingToLoad = count;
  }

  private struct PopupCallbackData
  {
    public DialogManager.PopupCallbackSharedData m_sharedData;
    public int m_index;

    public PopupCallbackData(DialogManager.PopupCallbackSharedData sharedData, int index)
    {
      this.m_sharedData = sharedData;
      this.m_index = index;
    }
  }
}
