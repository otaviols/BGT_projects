using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLicenseMgr : IService
{
  private Map<long, long> m_seenLicenseNotices;
  private AccountLicenseMgr.LicenseUpdateState m_consumableLicensesUpdateState;
  private AccountLicenseMgr.LicenseUpdateState m_fixedLicensesUpdateState;
  private List<AccountLicenseMgr.AccountLicensesChangedListener> m_accountLicensesChangedListeners = new List<AccountLicenseMgr.AccountLicensesChangedListener>();

  public AccountLicenseMgr.LicenseUpdateState FixedLicensesState => this.m_fixedLicensesUpdateState;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AccountLicenseMgr accountLicenseMgr = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new Action(accountLicenseMgr.WillReset);
    serviceLocator.Get<Network>().RegisterNetHandler((object) UpdateAccountLicensesResponse.PacketID.ID, new Network.NetHandler(accountLicenseMgr.OnAccountLicensesUpdatedResponse));
    serviceLocator.Get<NetCache>().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(accountLicenseMgr.OnNewNotices));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (Network),
    typeof (NetCache)
  };

  public void Shutdown()
  {
  }

  private void WillReset()
  {
    if (this.m_seenLicenseNotices != null)
      this.m_seenLicenseNotices.Clear();
    this.m_consumableLicensesUpdateState = AccountLicenseMgr.LicenseUpdateState.UNKNOWN;
    this.m_fixedLicensesUpdateState = AccountLicenseMgr.LicenseUpdateState.UNKNOWN;
  }

  public static AccountLicenseMgr Get() => ServiceManager.Get<AccountLicenseMgr>();

  public void InitRequests() => Network.Get().RequestAccountLicensesUpdate();

  public bool OwnsAccountLicense(long license)
  {
    NetCache.NetCacheAccountLicenses netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
    return netObject != null && netObject.AccountLicenses.ContainsKey(license) && this.OwnsAccountLicense(netObject.AccountLicenses[license]);
  }

  public bool OwnsAccountLicense(AccountLicenseInfo accountLicenseInfo) => accountLicenseInfo != null && ((long) accountLicenseInfo.Flags_ & 1L) == 1L;

  public List<AccountLicenseInfo> GetAllOwnedAccountLicenseInfo()
  {
    List<AccountLicenseInfo> accountLicenseInfo1 = new List<AccountLicenseInfo>();
    NetCache.NetCacheAccountLicenses netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
    if (netObject != null)
    {
      foreach (AccountLicenseInfo accountLicenseInfo2 in netObject.AccountLicenses.Values)
      {
        if (this.OwnsAccountLicense(accountLicenseInfo2))
          accountLicenseInfo1.Add(accountLicenseInfo2);
      }
    }
    return accountLicenseInfo1;
  }

  public bool RegisterAccountLicensesChangedListener(
    AccountLicenseMgr.AccountLicensesChangedCallback callback)
  {
    return this.RegisterAccountLicensesChangedListener(callback, (object) null);
  }

  public bool RegisterAccountLicensesChangedListener(
    AccountLicenseMgr.AccountLicensesChangedCallback callback,
    object userData)
  {
    AccountLicenseMgr.AccountLicensesChangedListener licensesChangedListener = new AccountLicenseMgr.AccountLicensesChangedListener();
    licensesChangedListener.SetCallback(callback);
    licensesChangedListener.SetUserData(userData);
    if (this.m_accountLicensesChangedListeners.Contains(licensesChangedListener))
      return false;
    this.m_accountLicensesChangedListeners.Add(licensesChangedListener);
    return true;
  }

  public bool RemoveAccountLicensesChangedListener(
    AccountLicenseMgr.AccountLicensesChangedCallback callback)
  {
    return this.RemoveAccountLicensesChangedListener(callback, (object) null);
  }

  public bool RemoveAccountLicensesChangedListener(
    AccountLicenseMgr.AccountLicensesChangedCallback callback,
    object userData)
  {
    AccountLicenseMgr.AccountLicensesChangedListener licensesChangedListener = new AccountLicenseMgr.AccountLicensesChangedListener();
    licensesChangedListener.SetCallback(callback);
    licensesChangedListener.SetUserData(userData);
    return this.m_accountLicensesChangedListeners.Remove(licensesChangedListener);
  }

  private void OnAccountLicensesUpdatedResponse()
  {
    UpdateAccountLicensesResponse licensesResponse = Network.Get().GetUpdateAccountLicensesResponse();
    this.m_consumableLicensesUpdateState = licensesResponse.ConsumableLicenseSuccess ? AccountLicenseMgr.LicenseUpdateState.SUCCESS : AccountLicenseMgr.LicenseUpdateState.FAIL;
    this.m_fixedLicensesUpdateState = licensesResponse.FixedLicenseSuccess ? AccountLicenseMgr.LicenseUpdateState.SUCCESS : AccountLicenseMgr.LicenseUpdateState.FAIL;
    Log.All.Print("OnAccountLicensesUpdatedResponse consumableLicensesUpdateState={0} fixedLicensesUpdateState={1}", (object) this.m_consumableLicensesUpdateState, (object) this.m_fixedLicensesUpdateState);
    if (AccountLicenseMgr.LicenseUpdateState.SUCCESS == this.m_consumableLicensesUpdateState && AccountLicenseMgr.LicenseUpdateState.SUCCESS == this.m_fixedLicensesUpdateState)
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER"),
      m_text = GameStrings.Get("GLOBAL_ERROR_ACCOUNT_LICENSES"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    NetCache.NetCacheAccountLicenses netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
    if (netObject == null)
      Processor.RunCoroutine(this.OnNewNotices_WaitForNetCacheAccountLicenses(newNotices));
    else
      this.OnNewNotices_Internal(newNotices, netObject);
  }

  private IEnumerator OnNewNotices_WaitForNetCacheAccountLicenses(
    List<NetCache.ProfileNotice> newNotices)
  {
    float startTime = Time.realtimeSinceStartup;
    NetCache.NetCacheAccountLicenses netObject;
    for (netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>(); netObject == null && (double) Time.realtimeSinceStartup - (double) startTime < 30.0; netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>())
      yield return (object) null;
    this.OnNewNotices_Internal(newNotices, netObject);
  }

  private void OnNewNotices_Internal(
    List<NetCache.ProfileNotice> newNotices,
    NetCache.NetCacheAccountLicenses netCacheAccountLicenses)
  {
    if (netCacheAccountLicenses == null)
      Debug.LogWarning((object) "AccountLicenses.OnNewNotices netCacheAccountLicenses is null -- going to ack all ACCOUNT_LICENSE notices assuming NetCache is not yet loaded");
    HashSet<long> longSet = new HashSet<long>();
    foreach (NetCache.ProfileNotice newNotice in newNotices)
    {
      if (NetCache.ProfileNotice.NoticeType.ACCOUNT_LICENSE == newNotice.Type)
      {
        NetCache.ProfileNoticeAcccountLicense noticeAcccountLicense = newNotice as NetCache.ProfileNoticeAcccountLicense;
        if (netCacheAccountLicenses != null)
        {
          if (!netCacheAccountLicenses.AccountLicenses.ContainsKey(noticeAcccountLicense.License))
            netCacheAccountLicenses.AccountLicenses[noticeAcccountLicense.License] = new AccountLicenseInfo()
            {
              License = noticeAcccountLicense.License,
              Flags_ = 0UL,
              CasId = 0L
            };
          if (noticeAcccountLicense.CasID >= netCacheAccountLicenses.AccountLicenses[noticeAcccountLicense.License].CasId)
          {
            netCacheAccountLicenses.AccountLicenses[noticeAcccountLicense.License].CasId = noticeAcccountLicense.CasID;
            if (newNotice.Origin == NetCache.ProfileNotice.NoticeOrigin.ACCOUNT_LICENSE_FLAGS)
              netCacheAccountLicenses.AccountLicenses[noticeAcccountLicense.License].Flags_ = (ulong) newNotice.OriginData;
            else
              Debug.LogWarning((object) string.Format("AccountLicenses.OnNewNotices unexpected notice origin {0} (data={1}) for license {2} casID {3}", (object) newNotice.Origin, (object) newNotice.OriginData, (object) noticeAcccountLicense.License, (object) noticeAcccountLicense.CasID));
            long num = noticeAcccountLicense.CasID - 1L;
            if (this.m_seenLicenseNotices != null)
              this.m_seenLicenseNotices.TryGetValue(noticeAcccountLicense.License, out num);
            if (num < noticeAcccountLicense.CasID)
              longSet.Add(noticeAcccountLicense.License);
            if (this.m_seenLicenseNotices == null)
              this.m_seenLicenseNotices = new Map<long, long>();
            this.m_seenLicenseNotices[noticeAcccountLicense.License] = noticeAcccountLicense.CasID;
          }
        }
        Network.Get().AckNotice(newNotice.NoticeID);
      }
    }
    if (netCacheAccountLicenses == null)
      return;
    List<AccountLicenseInfo> changedLicensesInfo = new List<AccountLicenseInfo>();
    foreach (long key in longSet)
    {
      if (netCacheAccountLicenses.AccountLicenses.ContainsKey(key))
        changedLicensesInfo.Add(netCacheAccountLicenses.AccountLicenses[key]);
    }
    if (changedLicensesInfo.Count == 0)
      return;
    foreach (AccountLicenseMgr.AccountLicensesChangedListener licensesChangedListener in this.m_accountLicensesChangedListeners.ToArray())
      licensesChangedListener.Fire(changedLicensesInfo);
  }

  public enum LicenseUpdateState
  {
    UNKNOWN,
    SUCCESS,
    FAIL,
  }

  public delegate void AccountLicensesChangedCallback(
    List<AccountLicenseInfo> changedLicensesInfo,
    object userData);

  private class AccountLicensesChangedListener : 
    EventListener<AccountLicenseMgr.AccountLicensesChangedCallback>
  {
    public void Fire(List<AccountLicenseInfo> changedLicensesInfo) => this.m_callback(changedLicensesInfo, this.m_userData);
  }
}
