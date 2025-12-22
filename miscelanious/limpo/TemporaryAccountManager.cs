using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Login;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TemporaryAccountManager
{
  private static Map<int, int> HEAL_UP_FREQUENCY = new Map<int, int>()
  {
    {
      20,
      48
    },
    {
      10,
      3
    },
    {
      0,
      1
    }
  };
  private static TemporaryAccountManager s_Instance;
  private TemporaryAccountManager.TemporaryAccountData m_temporaryAccountData;
  private bool m_isTemporaryAccountDataLoaded;
  private string m_createdTemporaryAccountId;
  private TemporaryAccountManager.TemporaryAccountData.TemporaryAccount m_createdTemporaryAccount;
  private TemporaryAccountSignUpPopUp m_signUpPopUp;
  private bool m_isSignUpPopUpLoading;
  private TemporaryAccountSignUpPopUp.PopupTextParameters m_popupArgs;
  private TemporaryAccountManager.OnHealUpDialogDismissed m_onSignUpDismissedHandler;
  private TemporaryAccountManager.HealUpReason m_signUpReason;
  private SwitchAccountMenu m_switchAccountMenu;
  private bool m_isSwitchAccountMenuLoading;
  private bool m_disableSwitchAccountMenuInputBlocker;
  private SwitchAccountMenu.OnSwitchAccountLogInPressed m_onSwithAccountLogInPressedHandler;
  private int m_lastLoginSelectedTemporaryAccountIndex = -1;

  public static bool IsTemporaryAccount() => !HearthstoneApplication.IsInternal() || !Options.Get().HasOption(Option.IS_TEMPORARY_ACCOUNT_CHEAT) ? BattleNet.IsHeadlessAccount() : Options.Get().GetBool(Option.IS_TEMPORARY_ACCOUNT_CHEAT);

  public static TemporaryAccountManager Get()
  {
    if (TemporaryAccountManager.s_Instance == null)
      TemporaryAccountManager.s_Instance = new TemporaryAccountManager();
    return TemporaryAccountManager.s_Instance;
  }

  public void Initialize()
  {
    HearthstoneApplication.Get().WillReset += new System.Action(this.WillReset);
    if (!TemporaryAccountManager.IsTemporaryAccount())
      return;
    Processor.QueueJob("TemporaryAccountManager.AddFakeBooster", this.Job_AddFakeBooster(), JobFlags.StartImmediately);
  }

  private IEnumerator<IAsyncJobResult> Job_AddFakeBooster()
  {
    yield return (IAsyncJobResult) new WaitForNetCacheObject<NetCache.NetCacheBoosters>();
    NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
    int num = 18;
    NetCache.BoosterStack boosterStack = new NetCache.BoosterStack()
    {
      Id = num,
      Count = 1
    };
    netObject.BoosterStacks.Add(boosterStack);
  }

  public void WillReset()
  {
    HearthstoneApplication.Get().WillReset -= new System.Action(this.WillReset);
    BnetPresenceMgr.Get().OnGameAccountPresenceChange -= new System.Action<PresenceUpdate[]>(this.OnPresenceChanged);
  }

  public void LoadTemporaryAccountData()
  {
    if (this.m_isTemporaryAccountDataLoaded)
      return;
    this.m_temporaryAccountData = (TemporaryAccountManager.TemporaryAccountData) null;
    this.m_isTemporaryAccountDataLoaded = true;
  }

  public string GetSelectedTemporaryAccountId()
  {
    Log.TemporaryAccount.Print("Get selected Temporary Account Id");
    if (!this.m_isTemporaryAccountDataLoaded)
      this.LoadTemporaryAccountData();
    if (this.m_temporaryAccountData == null)
    {
      Log.TemporaryAccount.PrintWarning("Unable to load temporary account data!");
      return (string) null;
    }
    if (this.m_temporaryAccountData.m_selectedTemporaryAccountIndex == -1)
    {
      Log.TemporaryAccount.PrintWarning("No selected temporary account!");
      return (string) null;
    }
    string temporaryAccountId = this.m_temporaryAccountData.m_temporaryAccounts[this.m_temporaryAccountData.m_selectedTemporaryAccountIndex].m_temporaryAccountId;
    return !string.IsNullOrEmpty(temporaryAccountId) ? temporaryAccountId : (string) null;
  }

  public void UpdateTemporaryAccountData()
  {
    if (!TemporaryAccountManager.IsTemporaryAccount())
      return;
    CloudStorageManager.Get().StartInitialize(new CloudStorageManager.OnInitializedFinished(this.OnCloudStorageInitializedUpdateTemporaryAccountData), GameStrings.Get("GLUE_CLOUD_STORAGE_CONTEXT_BODY_01"));
  }

  public void DeleteTemporaryAccountData()
  {
    Log.TemporaryAccount.PrintWarning("Deleting Temporary Account Data!");
    this.m_temporaryAccountData = new TemporaryAccountManager.TemporaryAccountData();
    Options.Get().DeleteOption(Option.TEMPORARY_ACCOUNT_DATA);
    CloudStorageManager cloudStorageManager = CloudStorageManager.Get();
    if (!((UnityEngine.Object) cloudStorageManager != (UnityEngine.Object) null))
      return;
    cloudStorageManager.RemoveObject("TEMPORARY_ACCOUNT_DATA");
  }

  public void UnselectTemporaryAccount()
  {
    Log.TemporaryAccount.Print("Unselect Selected Temporary Account (if any)");
    if (!this.m_isTemporaryAccountDataLoaded)
      this.LoadTemporaryAccountData();
    if (this.m_temporaryAccountData == null)
    {
      Log.TemporaryAccount.PrintWarning("Unable to load temporary account data!");
    }
    else
    {
      this.m_lastLoginSelectedTemporaryAccountIndex = this.m_temporaryAccountData.m_selectedTemporaryAccountIndex;
      this.m_temporaryAccountData.m_selectedTemporaryAccountIndex = -1;
      this.SaveTemporaryAccountData();
    }
  }

  public bool IsSelectedTemporaryAccountMinor()
  {
    Log.TemporaryAccount.Print("Is Selected Temporary Account a Minor");
    if (!this.m_isTemporaryAccountDataLoaded)
      this.LoadTemporaryAccountData();
    if (this.m_temporaryAccountData == null)
    {
      Log.TemporaryAccount.PrintWarning("Unable to load temporary account data!");
      return false;
    }
    if (this.m_temporaryAccountData.m_selectedTemporaryAccountIndex != -1)
      return this.m_temporaryAccountData.m_temporaryAccounts[this.m_temporaryAccountData.m_selectedTemporaryAccountIndex].m_isMinor;
    Log.TemporaryAccount.PrintWarning("No selected temporary account!");
    return false;
  }

  public bool ShowHealUpDialog(
    string header,
    string body,
    TemporaryAccountManager.HealUpReason reason,
    bool userTriggered,
    TemporaryAccountManager.OnHealUpDialogDismissed onSignUpHandler)
  {
    return this.ShowHealUpDialog(new TemporaryAccountSignUpPopUp.PopupTextParameters()
    {
      Header = header,
      Body = body
    }, reason, userTriggered, onSignUpHandler);
  }

  public bool ShowHealUpDialog(
    TemporaryAccountSignUpPopUp.PopupTextParameters popupArgs,
    TemporaryAccountManager.HealUpReason reason,
    bool userTriggered,
    TemporaryAccountManager.OnHealUpDialogDismissed onSignUpHandler)
  {
    if (!TemporaryAccountManager.IsTemporaryAccount() || !GameUtils.IsAnyTutorialComplete())
      return false;
    if (!userTriggered)
    {
      long ticks = Options.Get().GetLong(Option.LAST_HEAL_UP_EVENT_DATE);
      DateTime now = DateTime.Now;
      if (ticks != 0L)
      {
        int totalWins = this.GetTotalWins();
        DateTime dateTime = new DateTime(ticks);
        TimeSpan timeSpan = now - dateTime;
        int num = 1;
        foreach (int key in TemporaryAccountManager.HEAL_UP_FREQUENCY.Keys)
        {
          if (totalWins >= key)
            num = TemporaryAccountManager.HEAL_UP_FREQUENCY[key];
        }
        if (timeSpan.TotalHours < (double) num)
          return false;
      }
      Options.Get().SetLong(Option.LAST_HEAL_UP_EVENT_DATE, now.Ticks);
    }
    this.m_signUpReason = reason;
    this.m_onSignUpDismissedHandler = onSignUpHandler;
    if ((UnityEngine.Object) this.m_signUpPopUp == (UnityEngine.Object) null)
    {
      if (!this.m_isSignUpPopUpLoading)
      {
        this.m_isSignUpPopUpLoading = true;
        AssetLoader.Get().InstantiatePrefab((AssetReference) "TemporaryAccountSignUp.prefab:14791f0c7af5c6f4480fc78ab36c81bc", new PrefabCallback<GameObject>(this.ShowSignUpPopUp));
      }
      this.m_popupArgs = popupArgs;
      return true;
    }
    this.m_signUpPopUp.Show(popupArgs, new TemporaryAccountSignUpPopUp.OnSignUpPopUpBack(this.OnHealUpProcessCancelled));
    return true;
  }

  public bool ShowEarnCardEventHealUpDialog(TemporaryAccountManager.HealUpReason reason) => this.ShowHealUpDialog(GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_HEADER_03"), GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_BODY_01"), reason, false, (TemporaryAccountManager.OnHealUpDialogDismissed) null);

  public void ShowHealUpPage(TemporaryAccountManager.HealUpReason reason, System.Action<bool> onDismissed = null)
  {
    this.m_signUpReason = reason;
    this.ShowHealUpPage(onDismissed);
  }

  public void ShowHealUpPage(System.Action<bool> onDismissed = null)
  {
    ILoginService loginService = ServiceManager.Get<ILoginService>();
    if (loginService != null)
    {
      Log.TemporaryAccount.PrintDebug("Using Login Service for account heal up");
      loginService.HealupCurrentTemporaryAccount(onDismissed);
    }
    else
      Log.TemporaryAccount.PrintError("Login Service null when trying to heal up temporary account");
  }

  public void ShowSwitchAccountMenu(
    SwitchAccountMenu.OnSwitchAccountLogInPressed handler,
    bool disableInputBlocker)
  {
    this.m_onSwithAccountLogInPressedHandler = handler;
    this.m_disableSwitchAccountMenuInputBlocker = disableInputBlocker;
    this.ShowSwitchAccountMenu();
  }

  public void ShowSwitchAccountMenu()
  {
    if ((bool) (UnityEngine.Object) this.m_switchAccountMenu)
    {
      if (!this.m_switchAccountMenu.IsShown())
      {
        this.m_switchAccountMenu.Show(this.m_onSwithAccountLogInPressedHandler);
        if (this.m_disableSwitchAccountMenuInputBlocker)
          this.m_switchAccountMenu.DisableInputBlocker();
      }
      this.m_onSwithAccountLogInPressedHandler = (SwitchAccountMenu.OnSwitchAccountLogInPressed) null;
      this.m_disableSwitchAccountMenuInputBlocker = false;
    }
    else
    {
      if (this.m_isSwitchAccountMenuLoading)
        return;
      this.m_isSwitchAccountMenuLoading = true;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "SwitchAccountMenu.prefab:bca3c7466980f484fbf25690f6cef4bf", new PrefabCallback<GameObject>(this.OnSwitchAccountMenuLoaded));
    }
  }

  public void PrintTemporaryAccountData()
  {
    if (!this.m_isTemporaryAccountDataLoaded)
      this.LoadTemporaryAccountData();
    if (this.m_temporaryAccountData == null)
    {
      Log.TemporaryAccount.Print("m_temporaryAccountData == null");
    }
    else
    {
      Log.TemporaryAccount.Print("Selected Account = " + (object) this.m_temporaryAccountData.m_selectedTemporaryAccountIndex + ", m_lastUpdate = " + (object) Convert.ToDateTime(this.m_temporaryAccountData.m_lastUpdated));
      foreach (TemporaryAccountManager.TemporaryAccountData.TemporaryAccount temporaryAccount in this.m_temporaryAccountData.m_temporaryAccounts)
        Log.TemporaryAccount.Print("[m_temporaryAccountId = " + temporaryAccount.m_temporaryAccountId + ", m_battleTag = " + temporaryAccount.m_battleTag + ", m_regionId = " + (object) temporaryAccount.m_regionId + ", m_lastLogin = " + (object) Convert.ToDateTime(temporaryAccount.m_lastLogin) + ", m_isHealedUp = " + temporaryAccount.m_isHealedUp.ToString() + ", m_isMinor = " + temporaryAccount.m_isMinor.ToString() + "]");
      this.SortPrint();
    }
  }

  public void Test()
  {
    if (!this.m_isTemporaryAccountDataLoaded)
      this.LoadTemporaryAccountData();
    if (this.m_temporaryAccountData == null)
    {
      Log.TemporaryAccount.Print("m_temporaryAccountData == null");
    }
    else
    {
      this.m_temporaryAccountData = new TemporaryAccountManager.TemporaryAccountData();
      this.m_temporaryAccountData.m_temporaryAccounts.Add(new TemporaryAccountManager.TemporaryAccountData.TemporaryAccount()
      {
        m_temporaryAccountId = "BLAH BLAH BLAH",
        m_battleTag = "BATTLETAG",
        m_lastLogin = DateTime.UtcNow.ToString()
      });
      this.m_temporaryAccountData.m_temporaryAccounts.Add(new TemporaryAccountManager.TemporaryAccountData.TemporaryAccount()
      {
        m_temporaryAccountId = "HEHEHEHEHEHE",
        m_battleTag = "ARGAREOJ",
        m_lastLogin = DateTime.UtcNow.ToString()
      });
      this.m_temporaryAccountData.m_temporaryAccounts.Add(new TemporaryAccountManager.TemporaryAccountData.TemporaryAccount()
      {
        m_temporaryAccountId = "Wha?",
        m_battleTag = "YE",
        m_lastLogin = DateTime.UtcNow.ToString()
      });
      this.m_temporaryAccountData.m_temporaryAccounts.Add(new TemporaryAccountManager.TemporaryAccountData.TemporaryAccount()
      {
        m_temporaryAccountId = "SUPER_SECRET_ACCOUNT_ID",
        m_battleTag = "GoodKnight",
        m_lastLogin = DateTime.UtcNow.ToString()
      });
      this.m_temporaryAccountData.m_temporaryAccounts.Add(new TemporaryAccountManager.TemporaryAccountData.TemporaryAccount()
      {
        m_temporaryAccountId = "SUPER_SECRET_ACCOUNT_ID",
        m_battleTag = "GoodKnight",
        m_lastLogin = DateTime.UtcNow.ToString()
      });
      this.SaveTemporaryAccountData();
    }
  }

  public void SortPrint()
  {
    IEnumerable<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount> temporaryAccounts = this.GetSortedTemporaryAccounts();
    Log.TemporaryAccount.Print("Sorted!");
    Log.TemporaryAccount.Print("Selected Account = " + (object) this.m_temporaryAccountData.m_selectedTemporaryAccountIndex + ", m_lastUpdate = " + (object) Convert.ToDateTime(this.m_temporaryAccountData.m_lastUpdated));
    foreach (TemporaryAccountManager.TemporaryAccountData.TemporaryAccount temporaryAccount in temporaryAccounts)
      Log.TemporaryAccount.Print("[m_temporaryAccountId = " + temporaryAccount.m_temporaryAccountId + ", m_battleTag = " + temporaryAccount.m_battleTag + ", m_regionId = " + (object) temporaryAccount.m_regionId + ", m_lastLogin = " + (object) Convert.ToDateTime(temporaryAccount.m_lastLogin) + ", m_isHealedUp = " + temporaryAccount.m_isHealedUp.ToString() + ", m_isMinor = " + temporaryAccount.m_isMinor.ToString() + "]");
  }

  public string NagTimeDebugLog()
  {
    long ticks = Options.Get().GetLong(Option.LAST_HEAL_UP_EVENT_DATE);
    DateTime now = DateTime.Now;
    string str1;
    if (ticks != 0L)
    {
      int totalWins = this.GetTotalWins();
      DateTime dateTime = new DateTime(ticks);
      TimeSpan timeSpan = now - dateTime;
      int num = 1;
      foreach (int key in TemporaryAccountManager.HEAL_UP_FREQUENCY.Keys)
      {
        if (totalWins >= key)
          num = TemporaryAccountManager.HEAL_UP_FREQUENCY[key];
      }
      string str2 = "Last frequency time: " + (object) dateTime;
      if (timeSpan.TotalHours > (double) num)
        str1 = str2 + " Next event will trigger nag";
      else
        str1 = str2 + " Next nag in " + (object) ((double) num - timeSpan.TotalHours) + " hours";
    }
    else
      str1 = "No frequency time saved!";
    return str1;
  }

  private void ShowSignUpPopUp(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_signUpPopUp = go.GetComponent<TemporaryAccountSignUpPopUp>();
    this.m_signUpPopUp.Show(this.m_popupArgs, new TemporaryAccountSignUpPopUp.OnSignUpPopUpBack(this.OnHealUpProcessCancelled));
    this.m_isSignUpPopUpLoading = false;
  }

  private void OnHealUpProcessCancelled()
  {
    if (this.m_onSignUpDismissedHandler == null)
      return;
    this.m_onSignUpDismissedHandler();
    this.m_onSignUpDismissedHandler = (TemporaryAccountManager.OnHealUpDialogDismissed) null;
  }

  private void SaveTemporaryAccountData()
  {
  }

  private void AddCreatedTemporaryAccount(string battleTag)
  {
    if (this.m_createdTemporaryAccountId == null)
    {
      Log.TemporaryAccount.PrintError("Attempting to add new temporary account without ID!");
    }
    else
    {
      AdTrackingManager.Get().TrackHeadlessAccountCreated();
      Log.TemporaryAccount.Print("Adding Created Temporary Account, updating data...");
      this.m_createdTemporaryAccount = new TemporaryAccountManager.TemporaryAccountData.TemporaryAccount();
      this.m_createdTemporaryAccount.m_temporaryAccountId = this.m_createdTemporaryAccountId;
      this.m_createdTemporaryAccount.m_battleTag = battleTag;
      this.m_createdTemporaryAccount.m_regionId = (int) MobileDeviceLocale.GetCurrentRegionId();
      this.m_createdTemporaryAccount.m_lastLogin = DateTime.UtcNow.ToString();
      this.m_createdTemporaryAccountId = (string) null;
      CloudStorageManager.Get().StartInitialize(new CloudStorageManager.OnInitializedFinished(this.OnCloudStorageInitializedAddCreatedTemporaryAccount), GameStrings.Get("GLUE_CLOUD_STORAGE_CONTEXT_BODY_01"));
    }
  }

  private void OnCloudStorageInitializedAddCreatedTemporaryAccount()
  {
    if (!this.m_isTemporaryAccountDataLoaded)
      this.LoadTemporaryAccountData();
    if (this.m_temporaryAccountData == null)
    {
      Log.TemporaryAccount.PrintWarning("Unable to load temporary account data!");
    }
    else
    {
      if (!this.m_temporaryAccountData.m_temporaryAccounts.Exists((Predicate<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount>) (account => account.m_temporaryAccountId == this.m_createdTemporaryAccount.m_temporaryAccountId)))
      {
        this.m_temporaryAccountData.m_temporaryAccounts.Add(this.m_createdTemporaryAccount);
        this.m_temporaryAccountData.m_selectedTemporaryAccountIndex = this.m_temporaryAccountData.m_temporaryAccounts.Count - 1;
        this.SaveTemporaryAccountData();
      }
      else
        Log.TemporaryAccount.PrintInfo("Did not add temporary account to cloud storage as it was already saved");
      this.m_createdTemporaryAccount = (TemporaryAccountManager.TemporaryAccountData.TemporaryAccount) null;
    }
  }

  private void OnCloudStorageInitializedUpdateTemporaryAccountData()
  {
    if (!this.m_isTemporaryAccountDataLoaded)
      this.LoadTemporaryAccountData();
    if (this.m_temporaryAccountData == null)
    {
      Log.TemporaryAccount.PrintWarning("Unable to load temporary account data!");
    }
    else
    {
      if (!string.IsNullOrEmpty(this.m_createdTemporaryAccountId) || this.m_temporaryAccountData.m_selectedTemporaryAccountIndex == -1)
        return;
      this.m_temporaryAccountData.m_temporaryAccounts[this.m_temporaryAccountData.m_selectedTemporaryAccountIndex].m_lastLogin = DateTime.UtcNow.ToString();
      this.SaveTemporaryAccountData();
    }
  }

  private IEnumerable<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount> GetSortedTemporaryAccounts() => (IEnumerable<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount>) this.m_temporaryAccountData.m_temporaryAccounts.OrderByDescending<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount, DateTime>((Func<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount, DateTime>) (temporaryAccount =>
  {
    DateTime result;
    DateTime.TryParse(temporaryAccount.m_lastLogin, out result);
    return result;
  }));

  private void OnPresenceChanged(PresenceUpdate[] updates)
  {
    if (string.IsNullOrEmpty(this.m_createdTemporaryAccountId))
    {
      BnetPresenceMgr.Get().OnGameAccountPresenceChange -= new System.Action<PresenceUpdate[]>(this.OnPresenceChanged);
    }
    else
    {
      BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
      if (myPlayer == null)
        return;
      this.AddCreatedTemporaryAccount(myPlayer.GetBattleTag().GetName());
      BnetPresenceMgr.Get().OnGameAccountPresenceChange -= new System.Action<PresenceUpdate[]>(this.OnPresenceChanged);
    }
  }

  private void OnSwitchAccountMenuLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_switchAccountMenu = go.GetComponent<SwitchAccountMenu>();
    this.m_switchAccountMenu.AddTemporaryAccountButtons(this.GetSortedTemporaryAccounts(), this.GetSelectedTemporaryAccountId());
    this.m_switchAccountMenu.Show(this.m_onSwithAccountLogInPressedHandler);
    if (this.m_disableSwitchAccountMenuInputBlocker)
    {
      this.m_switchAccountMenu.DisableInputBlocker();
      this.m_disableSwitchAccountMenuInputBlocker = false;
    }
    this.m_onSwithAccountLogInPressedHandler = (SwitchAccountMenu.OnSwitchAccountLogInPressed) null;
    this.m_isSwitchAccountMenuLoading = false;
  }

  private int GetTotalWins()
  {
    int totalWins = 0;
    if (NetCache.Get() == null || NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>() == null || NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>().Records == null)
      return totalWins;
    foreach (NetCache.PlayerRecord record in NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>().Records)
    {
      if (record.Data == 0)
      {
        switch (record.RecordType)
        {
          case GameType.GT_VS_AI:
          case GameType.GT_ARENA:
          case GameType.GT_RANKED:
          case GameType.GT_CASUAL:
          case GameType.GT_TAVERNBRAWL:
          case GameType.GT_FSG_BRAWL:
          case GameType.GT_FSG_BRAWL_2P_COOP:
            totalWins += record.Wins;
            continue;
          default:
            continue;
        }
      }
    }
    return totalWins;
  }

  [Serializable]
  public class TemporaryAccountData
  {
    public int m_selectedTemporaryAccountIndex = -1;
    public List<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount> m_temporaryAccounts = new List<TemporaryAccountManager.TemporaryAccountData.TemporaryAccount>();
    public string m_lastUpdated;

    [Serializable]
    public class TemporaryAccount
    {
      public string m_temporaryAccountId;
      public string m_battleTag;
      public int m_regionId = -1;
      public string m_lastLogin;
      public bool m_isHealedUp;
      public bool m_isMinor;
    }
  }

  public delegate void OnHealUpDialogDismissed();

  public enum HealUpReason
  {
    UNKNOWN,
    FRIENDS_LIST,
    GAME_MENU,
    REAL_MONEY,
    LOCKED_PACK,
    WIN_GAME,
    CRAFT_CARD,
    OPEN_PACK,
  }
}
