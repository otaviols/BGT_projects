using Blizzard.GameService.SDK.Client.Integration;
using System.Collections.Generic;

public class FriendUtils
{
  public static string GetUniqueName(BnetPlayer friend)
  {
    BnetBattleTag battleTag;
    string name;
    return FriendUtils.GetUniqueName(friend, out battleTag, out name) ? battleTag.ToString() : name;
  }

  public static string GetUniqueNameWithColor(BnetPlayer friend)
  {
    string nameColorStr = friend == null || !friend.IsOnline() ? "999999ff" : "5ecaf0ff";
    BnetBattleTag battleTag;
    string name;
    return FriendUtils.GetUniqueName(friend, out battleTag, out name) ? FriendUtils.GetBattleTagWithColor(battleTag, nameColorStr) : string.Format("<color=#{0}>{1}</color>", (object) nameColorStr, (object) name);
  }

  public static string GetBattleTagWithColor(BnetBattleTag battleTag, string nameColorStr) => string.Format("<color=#{0}>{1}</color><color=#{2}>#{3}</color>", (object) nameColorStr, (object) battleTag.GetName(), (object) "a1a1a1ff", (object) battleTag.GetNumber());

  public static string GetFriendListName(BnetPlayer friend, bool addColorTags)
  {
    string str = (string) null;
    BnetAccount account = friend.GetAccount();
    if (account != (BnetAccount) null)
    {
      str = account.GetFullName();
      if (str == null && account.GetBattleTag() != (BnetBattleTag) null)
        str = account.GetBattleTag().ToString();
    }
    if (str == null)
    {
      foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> gameAccount in friend.GetGameAccounts())
      {
        if (gameAccount.Value.GetBattleTag() != (BnetBattleTag) null)
        {
          str = gameAccount.Value.GetBattleTag().ToString();
          break;
        }
      }
    }
    return addColorTags ? string.Format("<color=#{0}>{1}</color>", friend.IsOnline() ? (object) "5ecaf0ff" : (object) "999999ff", (object) str) : str;
  }

  public static string GetRequestElapsedTimeString(long epochMicrosec)
  {
    TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
    {
      m_seconds = "GLOBAL_DATETIME_FRIENDREQUEST_SECONDS",
      m_minutes = "GLOBAL_DATETIME_FRIENDREQUEST_MINUTES",
      m_hours = "GLOBAL_DATETIME_FRIENDREQUEST_HOURS",
      m_yesterday = "GLOBAL_DATETIME_FRIENDREQUEST_DAY",
      m_days = "GLOBAL_DATETIME_FRIENDREQUEST_DAYS",
      m_weeks = "GLOBAL_DATETIME_FRIENDREQUEST_WEEKS",
      m_monthAgo = "GLOBAL_DATETIME_FRIENDREQUEST_MONTH"
    };
    return TimeUtils.GetElapsedTimeStringFromEpochMicrosec(epochMicrosec, stringSet);
  }

  public static string GetLastOnlineElapsedTimeString(long epochMicrosec)
  {
    if (epochMicrosec == 0L)
      return GameStrings.Get("GLOBAL_OFFLINE");
    TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
    {
      m_seconds = "GLOBAL_DATETIME_LASTONLINE_SECONDS",
      m_minutes = "GLOBAL_DATETIME_LASTONLINE_MINUTES",
      m_hours = "GLOBAL_DATETIME_LASTONLINE_HOURS",
      m_yesterday = "GLOBAL_DATETIME_LASTONLINE_DAY",
      m_days = "GLOBAL_DATETIME_LASTONLINE_DAYS",
      m_weeks = "GLOBAL_DATETIME_LASTONLINE_WEEKS",
      m_monthAgo = "GLOBAL_DATETIME_LASTONLINE_MONTH"
    };
    return TimeUtils.GetElapsedTimeStringFromEpochMicrosec(epochMicrosec, stringSet);
  }

  public static string GetAwayTimeString(long epochMicrosec)
  {
    TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
    {
      m_seconds = "GLOBAL_DATETIME_AFK_SECONDS",
      m_minutes = "GLOBAL_DATETIME_AFK_MINUTES",
      m_hours = "GLOBAL_DATETIME_AFK_HOURS",
      m_yesterday = "GLOBAL_DATETIME_AFK_DAY",
      m_days = "GLOBAL_DATETIME_AFK_DAYS",
      m_weeks = "GLOBAL_DATETIME_AFK_WEEKS",
      m_monthAgo = "GLOBAL_DATETIME_AFK_MONTH"
    };
    return TimeUtils.GetElapsedTimeStringFromEpochMicrosec(epochMicrosec, stringSet);
  }

  public static int FriendSortCompare(BnetPlayer friend1, BnetPlayer friend2)
  {
    int result = 0;
    if (friend1 == null || friend2 == null)
    {
      if (friend1 == friend2)
        return 0;
      return friend1 != null ? -1 : 1;
    }
    if (!friend1.IsOnline() && !friend2.IsOnline())
      return FriendUtils.FriendNameSortCompare(friend1, friend2);
    if (friend1.IsOnline() && !friend2.IsOnline())
      return -1;
    if (!friend1.IsOnline() && friend2.IsOnline())
      return 1;
    BnetProgramId bestProgramId1 = friend1.GetBestProgramId();
    BnetProgramId bestProgramId2 = friend2.GetBestProgramId();
    if (FriendUtils.FriendSortFlagCompare(friend1, friend2, (Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId1 == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE, (Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId2 == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE, out result))
      return result;
    bool lhsflag1 = !((Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId1 == (Blizzard.GameService.SDK.Client.Integration.FourCC) null) && bestProgramId1.IsGame();
    bool rhsflag1 = !((Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId2 == (Blizzard.GameService.SDK.Client.Integration.FourCC) null) && bestProgramId2.IsGame();
    if (FriendUtils.FriendSortFlagCompare(friend1, friend2, lhsflag1, rhsflag1, out result))
      return result;
    bool lhsflag2 = !((Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId1 == (Blizzard.GameService.SDK.Client.Integration.FourCC) null) && bestProgramId1.IsPhoenix();
    bool rhsflag2 = !((Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId2 == (Blizzard.GameService.SDK.Client.Integration.FourCC) null) && bestProgramId2.IsPhoenix();
    if (FriendUtils.FriendSortFlagCompare(friend1, friend2, lhsflag2, rhsflag2, out result))
      return result;
    bool flag1 = BnetFriendMgr.Get().IsFriend(friend1);
    bool flag2 = BnetFriendMgr.Get().IsFriend(friend2);
    if (flag1 == flag2)
      return FriendUtils.FriendNameSortCompare(friend1, friend2);
    return !flag1 ? 1 : -1;
  }

  public static int RecentFriendSortCompare(BnetPlayer friend1, BnetPlayer friend2) => friend2.TimeLastAddedToRecentPlayers.CompareTo(friend1.TimeLastAddedToRecentPlayers);

  public static int FriendNameSort(BnetPlayer friend1, BnetPlayer friend2) => FriendUtils.FriendNameSortCompare(friend1, friend2);

  public static bool FriendFlagSort(
    BnetPlayer lhs,
    BnetPlayer rhs,
    bool lhsflag,
    bool rhsflag,
    out int result)
  {
    return FriendUtils.FriendSortFlagCompare(lhs, rhs, lhsflag, rhsflag, out result);
  }

  private static bool GetUniqueName(
    BnetPlayer friend,
    out BnetBattleTag battleTag,
    out string name)
  {
    if (friend != null)
    {
      battleTag = friend.GetBattleTag();
      name = friend.GetBestName();
    }
    else
    {
      battleTag = (BnetBattleTag) null;
      name = string.Empty;
    }
    if (battleTag == (BnetBattleTag) null)
      return false;
    if (BnetNearbyPlayerMgr.Get().IsNearbyStranger(friend))
      return true;
    foreach (BnetPlayer friend1 in BnetFriendMgr.Get().GetFriends())
    {
      if (friend1 != friend)
      {
        string bestName = friend1.GetBestName();
        if (string.Compare(name, bestName, true) == 0)
          return true;
      }
    }
    return false;
  }

  private static bool FriendSortFlagCompare(
    BnetPlayer lhs,
    BnetPlayer rhs,
    bool lhsflag,
    bool rhsflag,
    out int result)
  {
    if (lhsflag && !rhsflag)
    {
      result = -1;
      return true;
    }
    if (!lhsflag & rhsflag)
    {
      result = 1;
      return true;
    }
    result = 0;
    return false;
  }

  private static int FriendNameSortCompare(BnetPlayer friend1, BnetPlayer friend2)
  {
    int num = string.Compare(FriendUtils.GetFriendListName(friend1, false), FriendUtils.GetFriendListName(friend2, false), true);
    return num != 0 ? num : (int) ((long) friend1.GetAccountId().Low - (long) friend2.GetAccountId().Low);
  }
}
