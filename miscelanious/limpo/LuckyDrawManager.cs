using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Cysharp.Threading.Tasks;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLuckyDraw;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class LuckyDrawManager : IService
{
  public const int NUM_HOURS_REMAINING_TO_SHOW_TIMER = 48;
  private int m_battlegroundsLuckyDrawID = -1;
  private LuckyDrawDataModel m_battlegroundsLuckyDrawDataModel;
  private LuckyDrawWidget m_luckyDrawWidget;
  private LuckyDrawButtonDataModel m_luckyDrawButtonDataModel;
  private CancellationTokenSource m_requestLuckyDrawBoxCallelationTokenSource;
  private int m_previousHammerCount = -1;
  private bool m_hasNewHammer;
  private UniTask m_RequestDataTask = UniTask.CompletedTask;
  private bool m_isInitialized;

  public static LuckyDrawManager Get() => ServiceManager.Get<LuckyDrawManager>();

  private event System.Action m_onDataUpdatedCallbacks;

  private event System.Action m_onEventExpiredListeners;

  public int NumUnacknowledgedBonusHammers() => this.m_battlegroundsLuckyDrawDataModel != null ? this.m_battlegroundsLuckyDrawDataModel.NumUnacknowledgedBonusHammers : 0;

  public int NumUnacknowledgedEarnedHammers() => this.m_battlegroundsLuckyDrawDataModel != null ? this.m_battlegroundsLuckyDrawDataModel.NumUnacknowledgedEarnedHammers : 0;

  public void SetShowHighlight(bool show)
  {
    this.GetLuckyDrawButtonDataModel().ShowHighlight = show;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.BACON, GameSaveKeySubkeyId.BACON_SHOW_LUCKY_DRAW_BUTTON_HIGHLIGHT, new long[1]
    {
      show ? 1L : 0L
    }));
  }

  public bool HasActiveLuckyDrawBox() => this.m_battlegroundsLuckyDrawID != -1;

  private async UniTask CountDownEventTimer()
  {
    await UniTask.Yield();
    while (LuckyDrawUtils.GetLuckyDrawTimeRemaining(this.m_battlegroundsLuckyDrawID) > TimeSpan.Zero)
      await UniTask.Delay(1000);
    await UniTask.SwitchToMainThread();
    if (this.m_luckyDrawButtonDataModel != null)
      this.m_luckyDrawButtonDataModel.IsEventExpired = true;
    if (this.m_onEventExpiredListeners == null)
      return;
    this.m_onEventExpiredListeners();
  }

  public void AcknowledgeAllHammers()
  {
    if (this.m_luckyDrawButtonDataModel != null)
      this.m_battlegroundsLuckyDrawDataModel.NumUnacknowledgedHammers = 0;
    Network.Get().AcknowledgeLuckyDrawHammers();
  }

  public void AcknowledgeAllRewards() => Network.Get().AcknowledgeLuckyDrawRewards();

  public void RegisterOnEventEndsListeners(System.Action action)
  {
    this.m_onEventExpiredListeners -= action;
    this.m_onEventExpiredListeners += action;
  }

  public void RemoveOnEventEndsListenders(System.Action action) => this.m_onEventExpiredListeners -= action;

  public void BindLuckyDrawDataModelToWidget(Widget widget) => widget.BindDataModel((IDataModel) this.m_battlegroundsLuckyDrawDataModel);

  public void BindAllLuckyDrawDataModelToWidget(Widget widget)
  {
    widget.BindDataModel((IDataModel) this.m_luckyDrawButtonDataModel);
    if (this.m_battlegroundsLuckyDrawDataModel == null)
      return;
    widget.BindDataModel((IDataModel) this.m_battlegroundsLuckyDrawDataModel);
  }

  public void UsedFreeHammer(LuckyDrawUseHammerResponse rewardResponse)
  {
    if (rewardResponse == null)
    {
      Log.All.PrintError("Error [LuckyDrawManager] UsedFreeHammer() rewardResponse was null!");
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else if (rewardResponse.HasErrorCode && rewardResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.All.PrintError("Error [LuckyDrawManager] UsedFreeHammer() rewardResponse had error {0}", (object) rewardResponse.ErrorCode);
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else
    {
      if (rewardResponse.NumUnusedFreeHammersRemaining >= 1)
        return;
      this.m_battlegroundsLuckyDrawDataModel.HasUnclamedFree = false;
      this.m_luckyDrawButtonDataModel.ClaimedFirstHammer = true;
    }
  }

  public void DetermineBattlegroundsLuckyDrawBox() => this.m_battlegroundsLuckyDrawID = LuckyDrawUtils.GetCurrentLuckyDrawID();

  public void UnregisterOnInitOrUpdateFinishedCallback(System.Action onFinishedCallback) => this.m_onDataUpdatedCallbacks -= onFinishedCallback;

  public void InitializeOrUpdateData(System.Action onFinishedCallback = null)
  {
    if (onFinishedCallback != null)
    {
      this.m_onDataUpdatedCallbacks -= onFinishedCallback;
      this.m_onDataUpdatedCallbacks += onFinishedCallback;
    }
    if (this.m_RequestDataTask.Status != UniTaskStatus.Succeeded)
      return;
    this.m_RequestDataTask = this.RequestUpdateDataModel();
  }

  public bool IsIntialized() => this.m_isInitialized;

  public bool IsDataDirty() => !this.m_isInitialized || this.m_RequestDataTask.Status != UniTaskStatus.Succeeded;

  private void SortReponse(ref LuckyDrawBoxStateResponse response)
  {
    if (response == null)
      return;
    response.Rewards.Sort((Comparison<LuckyDrawReward>) ((Reward1, Reward2) => Reward1.Id - Reward2.Id));
  }

  private async UniTask RequestUpdateDataModel()
  {
    await UniTask.SwitchToMainThread();
    if (this.m_requestLuckyDrawBoxCallelationTokenSource != null)
      this.m_requestLuckyDrawBoxCallelationTokenSource.Cancel();
    this.DetermineBattlegroundsLuckyDrawBox();
    if (this.m_battlegroundsLuckyDrawID == -1)
    {
      this.m_isInitialized = true;
      this.m_RequestDataTask = UniTask.CompletedTask;
    }
    else
    {
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      Network network = Network.Get();
      LuckyDrawBoxStateResponse luckyDrawResponse = (LuckyDrawBoxStateResponse) null;
      bool finishedGettingResponse = false;
      Network.NetHandler luckyDrawResponseHandler = (Network.NetHandler) (() =>
      {
        finishedGettingResponse = true;
        try
        {
          luckyDrawResponse = network.GetLuckyDrawBoxStateResponse();
        }
        catch (Exception ex)
        {
          Log.All.PrintError("Error [LuckyDrawManager] RequestBattlegroundsLuckyDraw() - {0}", (object) ex.Message);
        }
      });
      network.RegisterNetHandler((object) LuckyDrawBoxStateResponse.PacketID.ID, luckyDrawResponseHandler);
      network.RequestLuckyDrawBoxState(this.m_battlegroundsLuckyDrawID);
      this.m_requestLuckyDrawBoxCallelationTokenSource = cancellationTokenSource;
      while (!finishedGettingResponse)
        await UniTask.Yield();
      await UniTask.SwitchToMainThread();
      network.RemoveNetHandler((object) LuckyDrawBoxStateResponse.PacketID.ID, luckyDrawResponseHandler);
      if (cancellationTokenSource.Token.IsCancellationRequested)
      {
        if (this.m_requestLuckyDrawBoxCallelationTokenSource == cancellationTokenSource)
          this.m_requestLuckyDrawBoxCallelationTokenSource = (CancellationTokenSource) null;
        cancellationTokenSource.Dispose();
        this.m_RequestDataTask = UniTask.CompletedTask;
      }
      else
      {
        if (luckyDrawResponse != null && luckyDrawResponse.ErrorCode == PegasusShared.ErrorCode.ERROR_OK)
        {
          this.SortReponse(ref luckyDrawResponse);
          this.ConvertResponseToDataModel(luckyDrawResponse);
          this.m_hasNewHammer = false;
          if (this.m_previousHammerCount > 0)
            this.m_hasNewHammer = this.m_previousHammerCount < this.m_battlegroundsLuckyDrawDataModel.Hammers;
          this.m_previousHammerCount = this.m_battlegroundsLuckyDrawDataModel.Hammers;
        }
        cancellationTokenSource.Dispose();
        this.m_requestLuckyDrawBoxCallelationTokenSource = (CancellationTokenSource) null;
        this.UpdateLuckyDrawButtonDataModel();
        this.m_RequestDataTask = UniTask.CompletedTask;
        if (!this.m_isInitialized)
        {
          this.m_isInitialized = true;
          if (!this.m_luckyDrawButtonDataModel.IsEventExpired)
          {
            this.RegisterOnEventEndsListeners((System.Action) (() => this.InitializeOrUpdateData()));
            this.CountDownEventTimer().Forget();
          }
        }
        if (this.m_onDataUpdatedCallbacks == null)
          return;
        this.m_onDataUpdatedCallbacks();
        this.m_onDataUpdatedCallbacks = (System.Action) null;
      }
    }
  }

  public LuckyDrawDataModel GetBattlegroundsLuckyDrawDataModel() => this.m_battlegroundsLuckyDrawDataModel;

  public LuckyDrawButtonDataModel GetLuckyDrawButtonDataModel()
  {
    if (this.m_luckyDrawButtonDataModel == null)
    {
      this.m_luckyDrawButtonDataModel = new LuckyDrawButtonDataModel();
      this.UpdateLuckyDrawButtonDataModel();
    }
    return this.m_luckyDrawButtonDataModel;
  }

  public void UpdateLuckyDrawButtonDataModel()
  {
    LuckyDrawButtonDataModel drawButtonDataModel = this.GetLuckyDrawButtonDataModel();
    LuckyDrawBoxDbfRecord record1 = GameDbf.LuckyDrawBox.GetRecord(this.m_battlegroundsLuckyDrawID);
    if (record1 == null)
    {
      drawButtonDataModel.LuckyDrawEnabled = false;
    }
    else
    {
      AccountLicenseMgr accountLicenseMgr = AccountLicenseMgr.Get();
      GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
      AccountLicenseDbfRecord record2 = GameDbf.AccountLicense.GetRecord(record1.AccountLicenseId);
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      if (accountLicenseMgr == null || gameSaveDataManager == null || record2 == null || this.m_battlegroundsLuckyDrawDataModel == null || netObject == null)
      {
        drawButtonDataModel.LuckyDrawEnabled = false;
      }
      else
      {
        bool countryIsDisabled = this.GetCountryIsDisabled(netObject);
        bool flag = accountLicenseMgr.OwnsAccountLicense(record2.LicenseId);
        drawButtonDataModel.Hammers = this.m_battlegroundsLuckyDrawDataModel.Hammers;
        drawButtonDataModel.LuckyDrawEnabled = netObject.LuckyDrawEnabled && !countryIsDisabled;
        drawButtonDataModel.BattlePassPurchased = flag;
        drawButtonDataModel.HasNewHammers = this.m_hasNewHammer;
        drawButtonDataModel.ClaimedFirstHammer = !this.m_battlegroundsLuckyDrawDataModel.HasUnclamedFree;
        TimeSpan drawTimeRemaining = LuckyDrawUtils.GetLuckyDrawTimeRemaining(this.m_battlegroundsLuckyDrawID);
        drawButtonDataModel.IsEventExpired = drawTimeRemaining <= TimeSpan.Zero;
        drawButtonDataModel.HoursRemaining = 24 * drawTimeRemaining.Days + drawTimeRemaining.Hours;
        drawButtonDataModel.NumHoursRemainingToShowTimer = 48;
        long num;
        GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, GameSaveKeySubkeyId.BACON_SHOW_LUCKY_DRAW_BUTTON_HIGHLIGHT, out num);
        drawButtonDataModel.ShowHighlight = num > 0L;
      }
    }
  }

  private bool GetCountryIsDisabled(NetCache.NetCacheFeatures features)
  {
    string str1 = BattleNet.GetAccountCountry().Trim();
    string disabledCountryCode = features.BattlegroundsLuckyDrawDisabledCountryCode;
    char[] chArray = new char[1]{ ',' };
    foreach (string str2 in disabledCountryCode.Split(chArray))
    {
      if (str2.Trim().Equals(str1))
        return true;
    }
    return false;
  }

  public bool HasUnclamedFree() => this.m_battlegroundsLuckyDrawDataModel != null && this.m_battlegroundsLuckyDrawDataModel.HasUnclamedFree;

  private void ConvertResponseToDataModel(LuckyDrawBoxStateResponse response)
  {
    if (this.m_battlegroundsLuckyDrawDataModel == null)
      this.m_battlegroundsLuckyDrawDataModel = new LuckyDrawDataModel();
    if (response.HasErrorCode && response.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.All.PrintError("Error: [LuckyDrawManager] ConvertResponseToDataModel() Response had error. Error: {0}", (object) response.ErrorCode);
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else
    {
      LuckyDrawBoxDbfRecord record = GameDbf.LuckyDrawBox.GetRecord(this.m_battlegroundsLuckyDrawID);
      TimeSpan drawTimeRemaining = LuckyDrawUtils.GetLuckyDrawTimeRemaining(this.m_battlegroundsLuckyDrawID);
      this.m_battlegroundsLuckyDrawDataModel.Name = (string) record.Name;
      this.m_battlegroundsLuckyDrawDataModel.Theme = record.Theme;
      if (string.IsNullOrEmpty(this.m_battlegroundsLuckyDrawDataModel.Theme))
        this.m_battlegroundsLuckyDrawDataModel.Theme = "default";
      this.m_battlegroundsLuckyDrawDataModel.Layout = record.Layout;
      this.m_battlegroundsLuckyDrawDataModel.Hammers = response.Hammers.Count<LuckyDrawHammer>((Func<LuckyDrawHammer, bool>) (Hammer => !Hammer.IsUsed));
      this.m_battlegroundsLuckyDrawDataModel.HasUnclamedFree = response.Hammers.Count<LuckyDrawHammer>((Func<LuckyDrawHammer, bool>) (Hammer => Hammer.GrantMethod_ == LuckyDrawHammer.GrantMethod.GRANT_METHOD_FREE && !Hammer.IsUsed)) > 0;
      this.m_battlegroundsLuckyDrawDataModel.Rewards = this.GetLuckyDrawRewardDataModels(response.Rewards);
      this.m_battlegroundsLuckyDrawDataModel.Event = record.Event;
      this.m_battlegroundsLuckyDrawDataModel.NumUnacknowledgedHammers = response.Hammers.Count<LuckyDrawHammer>((Func<LuckyDrawHammer, bool>) (Hammer => !Hammer.IsAcknowledged));
      this.m_battlegroundsLuckyDrawDataModel.TimeLeft = this.BuildLuckyDrawTimeLeftString(drawTimeRemaining);
      this.m_battlegroundsLuckyDrawDataModel.TimeLeftStrPopup = GameStrings.Format("GLUE_BATTLEBASH_EVENT_POPUP_TIME_REM_DAYS", (object) drawTimeRemaining.Days);
      this.m_battlegroundsLuckyDrawDataModel.IsAllRewardsOwned = response.Rewards.Count<LuckyDrawReward>((Func<LuckyDrawReward, bool>) (Reward => !Reward.IsOwned)) == 0;
      this.m_battlegroundsLuckyDrawDataModel.NumUnacknowledgedBonusHammers = response.Hammers.Count<LuckyDrawHammer>((Func<LuckyDrawHammer, bool>) (Hammer => !Hammer.IsAcknowledged && Hammer.GrantMethod_ == LuckyDrawHammer.GrantMethod.GRANT_METHOD_BONUS));
      this.m_battlegroundsLuckyDrawDataModel.NumUnacknowledgedEarnedHammers = response.Hammers.Count<LuckyDrawHammer>((Func<LuckyDrawHammer, bool>) (Hammer => !Hammer.IsAcknowledged && Hammer.GrantMethod_ == LuckyDrawHammer.GrantMethod.GRANT_METHOD_EARNED));
      this.m_battlegroundsLuckyDrawDataModel.NumUnacknowledgedFreeHammers = response.Hammers.Count<LuckyDrawHammer>((Func<LuckyDrawHammer, bool>) (Hammer => !Hammer.IsAcknowledged && Hammer.GrantMethod_ == LuckyDrawHammer.GrantMethod.GRANT_METHOD_FREE));
    }
  }

  private string BuildLuckyDrawTimeLeftString(TimeSpan timeLeft)
  {
    if (timeLeft.Days >= 1)
      return GameStrings.Format("GLUE_BATTLEBASH_EVENT_TIME_REM_DAYS", (object) timeLeft.Days);
    if (timeLeft.Hours < 1)
      return GameStrings.Format("GLUE_BATTLEBASH_EVENT_TIME_REM_HOUR_OR_LESS");
    return GameStrings.Format("GLUE_BATTLEBASH_EVENT_TIME_REM_HOURS", (object) timeLeft.Hours);
  }

  private DataModelList<LuckyDrawRewardDataModel> GetLuckyDrawRewardDataModels(
    List<LuckyDrawReward> luckyDrawRewards)
  {
    DataModelList<LuckyDrawRewardDataModel> rewardDataModels = new DataModelList<LuckyDrawRewardDataModel>();
    foreach (LuckyDrawReward luckyDrawReward in luckyDrawRewards)
    {
      LuckyDrawRewardsDbfRecord record = GameDbf.LuckyDrawRewards.GetRecord(luckyDrawReward.Id);
      rewardDataModels.Add(new LuckyDrawRewardDataModel()
      {
        Style = this.GetLuckyDrawStyle(record.Style),
        RewardList = RewardUtils.CreateRewardListDataModelFromRewardListId(record.RewardListId),
        IsOwned = luckyDrawReward.IsOwned,
        RewardID = record.ID
      });
    }
    return rewardDataModels;
  }

  private LuckyDrawStyle GetLuckyDrawStyle(LuckyDrawRewards.LuckyDrawStyle style) => style == LuckyDrawRewards.LuckyDrawStyle.COMMON || style != LuckyDrawRewards.LuckyDrawStyle.LEGENDARY ? LuckyDrawStyle.COMMON : LuckyDrawStyle.LEGENDARY;

  public void UseLuckyDrawHammer(LuckyDrawWidget requestingWidget)
  {
    Network network = Network.Get();
    this.m_luckyDrawWidget = (LuckyDrawWidget) null;
    this.m_luckyDrawWidget = requestingWidget;
    network.RegisterNetHandler((object) LuckyDrawUseHammerResponse.PacketID.ID, new Network.NetHandler(this.OnLuckyDrawRewardReceived));
    network.UseLuckyDrawHammer();
  }

  private void OnLuckyDrawRewardReceived()
  {
    Network network = Network.Get();
    LuckyDrawUseHammerResponse drawHammerResponse = network.GetUseLuckyDrawHammerResponse();
    network.RemoveNetHandler((object) LuckyDrawUseHammerResponse.PacketID.ID, new Network.NetHandler(this.OnLuckyDrawRewardReceived));
    if (drawHammerResponse == null)
    {
      Log.All.PrintError("Error [LuckyDrawManager] OnLuckyDrawRewardReceived() rewardResponse was null!");
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else if (drawHammerResponse.HasErrorCode && drawHammerResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.All.PrintError("Error [LuckyDrawManager] OnLuckyDrawRewardReceived() had error {0}", (object) drawHammerResponse.ErrorCode);
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else if (drawHammerResponse.LuckyDrawBoxId != this.m_battlegroundsLuckyDrawID)
      Log.All.PrintWarning("Error [LuckyDrawManager] OnLuckyDrawRewardReceived() reward box ID {0} does not match current box ID {1}", (object) drawHammerResponse.LuckyDrawBoxId, (object) this.m_battlegroundsLuckyDrawID);
    else if (this.m_battlegroundsLuckyDrawDataModel == null)
    {
      Log.All.PrintError("Error [LuckyDrawManager] OnLuckyDrawRewardReceived() lucky draw data model is null");
    }
    else
    {
      this.m_battlegroundsLuckyDrawDataModel.Hammers = drawHammerResponse.NumUnusedHammersRemaining;
      this.UpdateLuckyDrawButtonDataModel();
      this.m_luckyDrawWidget.OnRewardResponseReceived(drawHammerResponse);
    }
  }

  public void OnLuckyDrawHammerAnimationFinished() => this.AcknowledgeAllRewards();

  public void UpdateAllRewardsOwnedStatus()
  {
    bool flag = true;
    foreach (LuckyDrawRewardDataModel reward in this.m_battlegroundsLuckyDrawDataModel.Rewards)
    {
      if (!reward.IsOwned)
      {
        flag = false;
        break;
      }
    }
    this.m_battlegroundsLuckyDrawDataModel.IsAllRewardsOwned = flag;
  }

  public Network.Bundle GetProduct()
  {
    StoreManager storeManager = StoreManager.Get();
    LuckyDrawBoxDbfRecord record = GameDbf.LuckyDrawBox.GetRecord(this.m_battlegroundsLuckyDrawID);
    if (record == null)
      return (Network.Bundle) null;
    if (record.AccountLicenseRecord == null)
      return (Network.Bundle) null;
    int id = record.AccountLicenseRecord.ID;
    return storeManager.EnumerateBundlesForProductType(ProductType.PRODUCT_TYPE_LUCKY_DRAW, false, id).FirstOrDefault<Network.Bundle>();
  }

  public int GetActiveLuckyDrawBoxID() => this.m_battlegroundsLuckyDrawID;

  private void OnAccountLicenseUpdate(List<AccountLicenseInfo> changedLicensesInfo, object userData) => this.UpdateLuckyDrawButtonDataModel();

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (Network),
    typeof (AccountLicenseMgr)
  };

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LuckyDrawManager luckyDrawManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    AccountLicenseMgr.Get().RegisterAccountLicensesChangedListener(new AccountLicenseMgr.AccountLicensesChangedCallback(luckyDrawManager.OnAccountLicenseUpdate));
    return false;
  }

  public void Shutdown() => AccountLicenseMgr.Get().RemoveAccountLicensesChangedListener(new AccountLicenseMgr.AccountLicensesChangedCallback(this.OnAccountLicenseUpdate));
}
