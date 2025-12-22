using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using System.Text;

public class BnetPlayer
{
  private BnetPlayerSource m_source;
  private BnetAccountId m_accountId;
  private BnetAccount m_account;
  private Map<BnetGameAccountId, BnetGameAccount> m_gameAccounts = new Map<BnetGameAccountId, BnetGameAccount>();
  private BnetGameAccount m_hsGameAccount;
  private BnetGameAccount m_bestGameAccount;

  public BnetPlayer(BnetPlayerSource source) => this.m_source = source;

  public BnetPlayer Clone()
  {
    BnetPlayer bnetPlayer = (BnetPlayer) this.MemberwiseClone();
    if ((BnetEntityId) this.m_accountId != (BnetEntityId) null)
      bnetPlayer.m_accountId = this.m_accountId.Clone();
    if (this.m_account != (BnetAccount) null)
      bnetPlayer.m_account = this.m_account.Clone();
    if (this.m_hsGameAccount != (BnetGameAccount) null)
      bnetPlayer.m_hsGameAccount = this.m_hsGameAccount.Clone();
    if (this.m_bestGameAccount != (BnetGameAccount) null)
      bnetPlayer.m_bestGameAccount = this.m_bestGameAccount.Clone();
    bnetPlayer.m_gameAccounts = new Map<BnetGameAccountId, BnetGameAccount>();
    foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> gameAccount in this.m_gameAccounts)
      bnetPlayer.m_gameAccounts.Add(gameAccount.Key.Clone(), gameAccount.Value.Clone());
    return bnetPlayer;
  }

  public BnetPlayerSource Source => this.m_source;

  public BnetAccountId GetAccountId()
  {
    if ((BnetEntityId) this.m_accountId != (BnetEntityId) null)
      return this.m_accountId;
    BnetGameAccount firstGameAccount = this.GetFirstGameAccount();
    return firstGameAccount != (BnetGameAccount) null ? firstGameAccount.GetOwnerId() : (BnetAccountId) null;
  }

  public void SetAccountId(BnetAccountId accountId) => this.m_accountId = accountId;

  public BnetAccount GetAccount() => this.m_account;

  public void SetAccount(BnetAccount account)
  {
    this.m_account = account;
    this.m_accountId = account.GetId();
  }

  public string GetFullName() => !(this.m_account == (BnetAccount) null) ? this.m_account.GetFullName() : (string) null;

  public BnetBattleTag GetBattleTag()
  {
    if (this.m_account != (BnetAccount) null && this.m_account.GetBattleTag() != (BnetBattleTag) null)
      return this.m_account.GetBattleTag();
    BnetGameAccount firstGameAccount = this.GetFirstGameAccount();
    return firstGameAccount != (BnetGameAccount) null ? firstGameAccount.GetBattleTag() : (BnetBattleTag) null;
  }

  public Map<BnetGameAccountId, BnetGameAccount> GetGameAccounts() => this.m_gameAccounts;

  public bool HasGameAccount(BnetGameAccountId id) => this.m_gameAccounts.ContainsKey(id);

  public void AddGameAccount(BnetGameAccount gameAccount)
  {
    BnetGameAccountId id = gameAccount.GetId();
    if (this.m_gameAccounts.ContainsKey(id))
      return;
    this.m_gameAccounts.Add(id, gameAccount);
    this.CacheSpecialGameAccounts();
  }

  public bool RemoveGameAccount(BnetGameAccountId id)
  {
    if (!this.m_gameAccounts.Remove(id))
      return false;
    this.CacheSpecialGameAccounts();
    return true;
  }

  public BnetGameAccount GetHearthstoneGameAccount() => this.m_hsGameAccount;

  public BnetGameAccountId GetHearthstoneGameAccountId() => this.m_hsGameAccount == (BnetGameAccount) null ? (BnetGameAccountId) null : this.m_hsGameAccount.GetId();

  public BnetGameAccount GetBestGameAccount() => this.m_bestGameAccount;

  public BnetGameAccountId GetBestGameAccountId() => this.m_bestGameAccount == (BnetGameAccount) null ? (BnetGameAccountId) null : this.m_bestGameAccount.GetId();

  public bool IsCheatPlayer { get; set; }

  public float TimeLastAddedToRecentPlayers { get; set; }

  public bool IsDisplayable() => this.GetBestName() != null;

  public BnetGameAccount GetFirstGameAccount()
  {
    using (Map<BnetGameAccountId, BnetGameAccount>.ValueCollection.Enumerator enumerator = this.m_gameAccounts.Values.GetEnumerator())
    {
      if (enumerator.MoveNext())
        return enumerator.Current;
    }
    return (BnetGameAccount) null;
  }

  public long GetPersistentGameId() => 0;

  public string GetBestName()
  {
    if (this == BnetPresenceMgr.Get().GetMyPlayer())
    {
      if (this.m_hsGameAccount == (BnetGameAccount) null)
        return (string) null;
      return !(this.m_hsGameAccount.GetBattleTag() == (BnetBattleTag) null) ? this.m_hsGameAccount.GetBattleTag().GetName() : (string) null;
    }
    if (this.m_account != (BnetAccount) null)
    {
      string fullName = this.m_account.GetFullName();
      if (fullName != null)
        return fullName;
      if (this.m_account.GetBattleTag() != (BnetBattleTag) null)
        return this.m_account.GetBattleTag().GetName();
    }
    foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> gameAccount in this.m_gameAccounts)
    {
      if (gameAccount.Value.GetBattleTag() != (BnetBattleTag) null)
        return gameAccount.Value.GetBattleTag().GetName();
    }
    return (string) null;
  }

  public BnetProgramId GetBestProgramId() => this.m_bestGameAccount == (BnetGameAccount) null ? (BnetProgramId) null : this.m_bestGameAccount.GetProgramId();

  public bool IsOnline()
  {
    foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> gameAccount in this.m_gameAccounts)
    {
      if (gameAccount.Value.IsOnline())
        return true;
    }
    return false;
  }

  public bool IsAway() => this.m_account != (BnetAccount) null && this.m_account.IsAway() || this.m_bestGameAccount != (BnetGameAccount) null && this.m_bestGameAccount.IsAway();

  public bool IsBusy() => this.m_account != (BnetAccount) null && this.m_account.IsBusy() || this.m_bestGameAccount != (BnetGameAccount) null && this.m_bestGameAccount.IsBusy();

  public bool IsAppearingOffline() => this.m_account.IsAppearingOffline();

  public long GetBestAwayTimeMicrosec()
  {
    long awayTimeMicrosec = 0;
    if (this.m_account != (BnetAccount) null && this.m_account.IsAway())
    {
      awayTimeMicrosec = Math.Max(this.m_account.GetAwayTimeMicrosec(), this.m_account.GetLastOnlineMicrosec());
      if (awayTimeMicrosec != 0L)
        return awayTimeMicrosec;
    }
    if (!(this.m_bestGameAccount != (BnetGameAccount) null) || !this.m_bestGameAccount.IsAway())
      return awayTimeMicrosec;
    return Math.Max(this.m_bestGameAccount.GetAwayTimeMicrosec(), this.m_bestGameAccount.GetLastOnlineMicrosec());
  }

  public long GetBestLastOnlineMicrosec()
  {
    long lastOnlineMicrosec = 0;
    if (this.m_account != (BnetAccount) null)
    {
      lastOnlineMicrosec = this.m_account.GetLastOnlineMicrosec();
      if (lastOnlineMicrosec != 0L)
        return lastOnlineMicrosec;
    }
    if (!(this.m_bestGameAccount != (BnetGameAccount) null))
      return lastOnlineMicrosec;
    return this.m_bestGameAccount.GetLastOnlineMicrosec();
  }

  public bool HasAccount(BnetEntityId id)
  {
    if (id == (BnetEntityId) null)
      return false;
    if ((BnetEntityId) this.m_accountId == id)
      return true;
    foreach (BnetEntityId key in this.m_gameAccounts.Keys)
    {
      if (key == id)
        return true;
    }
    return false;
  }

  public void OnGameAccountChanged(uint fieldId)
  {
    if (fieldId != 3U && fieldId != 1U && fieldId != 4U)
      return;
    this.CacheSpecialGameAccounts();
  }

  public override string ToString()
  {
    BnetAccountId accountId = this.GetAccountId();
    BnetBattleTag battleTag = this.GetBattleTag();
    return (BnetEntityId) accountId == (BnetEntityId) null && battleTag == (BnetBattleTag) null ? "UNKNOWN PLAYER" : string.Format("[account={0} battleTag={1} numGameAccounts={2}]", (object) accountId, (object) battleTag, (object) this.m_gameAccounts.Count);
  }

  public string ShortSummary
  {
    get
    {
      string fullName = this.GetFullName();
      BnetBattleTag battleTag = this.GetBattleTag();
      string str1 = battleTag == (BnetBattleTag) null ? "null" : battleTag.ToString();
      if (!string.IsNullOrEmpty(fullName) && battleTag != (BnetBattleTag) null)
        str1 = " " + str1;
      string str2 = this.IsOnline() ? "online" : "offline";
      return string.Format("{0}{1} {2}", (object) fullName, (object) str1, (object) str2);
    }
  }

  public string FullPresenceSummary
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.m_account != (BnetAccount) null)
        stringBuilder.Append(this.m_account.FullPresenceSummary);
      else
        stringBuilder.Append("null bnet account");
      foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> gameAccount in this.m_gameAccounts)
      {
        BnetGameAccount bnetGameAccount = gameAccount.Value;
        if (!(bnetGameAccount == (BnetGameAccount) null))
          stringBuilder.Append("\n").Append(bnetGameAccount.FullPresenceSummary);
      }
      return stringBuilder.ToString();
    }
  }

  private void CacheSpecialGameAccounts()
  {
    this.m_hsGameAccount = (BnetGameAccount) null;
    this.m_bestGameAccount = (BnetGameAccount) null;
    long num = 0;
    foreach (BnetGameAccount bnetGameAccount in this.m_gameAccounts.Values)
    {
      BnetProgramId programId1 = bnetGameAccount.GetProgramId();
      if (!((Blizzard.GameService.SDK.Client.Integration.FourCC) programId1 == (Blizzard.GameService.SDK.Client.Integration.FourCC) null))
      {
        if ((Blizzard.GameService.SDK.Client.Integration.FourCC) programId1 == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE)
        {
          this.m_hsGameAccount = bnetGameAccount;
          if (!bnetGameAccount.IsOnline() && BnetFriendMgr.Get().IsFriend(bnetGameAccount.GetId()))
            break;
          this.m_bestGameAccount = bnetGameAccount;
          break;
        }
        if (this.m_bestGameAccount == (BnetGameAccount) null)
        {
          this.m_bestGameAccount = bnetGameAccount;
          num = this.m_bestGameAccount.GetLastOnlineMicrosec();
        }
        else if (!this.m_bestGameAccount.IsOnline() && bnetGameAccount.IsOnline())
        {
          this.m_bestGameAccount = bnetGameAccount;
          num = this.m_bestGameAccount.GetLastOnlineMicrosec();
        }
        else
        {
          BnetProgramId programId2 = this.m_bestGameAccount.GetProgramId();
          if (bnetGameAccount.IsOnline())
          {
            if (programId1.IsGame() && !programId2.IsGame())
            {
              this.m_bestGameAccount = bnetGameAccount;
              num = this.m_bestGameAccount.GetLastOnlineMicrosec();
            }
            else if (programId1.IsGame() && programId2.IsGame())
            {
              long lastOnlineMicrosec = bnetGameAccount.GetLastOnlineMicrosec();
              if (lastOnlineMicrosec > num)
              {
                this.m_bestGameAccount = bnetGameAccount;
                num = lastOnlineMicrosec;
              }
            }
          }
          else if (!this.m_bestGameAccount.IsOnline())
          {
            long lastOnlineMicrosec = bnetGameAccount.GetLastOnlineMicrosec();
            if (lastOnlineMicrosec > num)
            {
              this.m_bestGameAccount = bnetGameAccount;
              num = lastOnlineMicrosec;
            }
          }
        }
      }
    }
  }
}
