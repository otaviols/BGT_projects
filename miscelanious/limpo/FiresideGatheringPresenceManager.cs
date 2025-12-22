using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Hearthstone;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FiresideGatheringPresenceManager
{
  private static FiresideGatheringPresenceManager s_instance;
  private Map<BnetGameAccountId, long> m_subscribedPatronList = new Map<BnetGameAccountId, long>();
  private Map<BnetGameAccountId, FiresideGatheringPresenceManager.PreviouslySubscribedPatron> m_previouslySubscribedPatrons = new Map<BnetGameAccountId, FiresideGatheringPresenceManager.PreviouslySubscribedPatron>();
  private Map<BnetGameAccountId, string> m_previouslySubscribedPatronBattleTags;
  private long m_lastCheckPruneOlderPatronsUnixTimestamp = -1;
  private const int MAX_PATRONS_TO_LOG = 10;

  private int CurrentSubscribedPlayerCount => this.m_subscribedPatronList.Count;

  public static FiresideGatheringPresenceManager Get()
  {
    if (FiresideGatheringPresenceManager.s_instance == null)
    {
      FiresideGatheringPresenceManager.s_instance = new FiresideGatheringPresenceManager();
      if (FiresideGatheringPresenceManager.IsRequestBattleTagEnabled)
        BnetPresenceMgr.Get().OnGameAccountPresenceChange += new System.Action<PresenceUpdate[]>(FiresideGatheringPresenceManager.s_instance.BnetPresenceMgr_OnGameAccountPresenceChange);
    }
    return FiresideGatheringPresenceManager.s_instance;
  }

  public static int MAX_SUBSCRIBED_PATRONS
  {
    get
    {
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      return netObject == null || netObject.FsgMaxPresencePubscribedPatronCount < 0 ? 100 : netObject.FsgMaxPresencePubscribedPatronCount;
    }
  }

  public static int PERIODIC_SUBSCRIBE_CHECK_SECONDS => Vars.Key("FSG.PeriodicPrunePatronOldSubscriptionsSeconds").GetInt(5);

  public static long PATRON_OLD_SUBSCRIPTION_THRESHOLD_SECONDS => Vars.Key("FSG.PatronOldSubscriptionThresholdSeconds").GetLong(15L);

  public static bool IsRequestBattleTagEnabled => FiresideGatheringPresenceManager.IsVerboseLogging && HearthstoneApplication.IsInternal();

  public static bool IsVerboseLogging => Vars.Key("FSG.PresenceSubscriptionsVerboseLog").GetBool(false);

  public static bool IsVerboseLoggingToScreen => Vars.Key("FSG.PresenceSubscriptionsVerboseLogToScreen").GetBool(false);

  public static bool IsDisplayable(BnetPlayer player) => player != null && player.IsDisplayable() && player.IsOnline() && !((Blizzard.GameService.SDK.Client.Integration.FourCC) player.GetBestProgramId() != (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE);

  private FiresideGatheringPresenceManager.PreviouslySubscribedPatron UpdatePreviouslySubscribedPatron(
    BnetGameAccountId gameAccountId,
    long timestamp)
  {
    FiresideGatheringPresenceManager.PreviouslySubscribedPatron subscribedPatron;
    if (!this.m_previouslySubscribedPatrons.TryGetValue(gameAccountId, out subscribedPatron))
    {
      subscribedPatron = new FiresideGatheringPresenceManager.PreviouslySubscribedPatron();
      this.m_previouslySubscribedPatrons.Add(gameAccountId, subscribedPatron);
    }
    subscribedPatron.m_lastSubscribeUnixTimestamp = timestamp;
    return subscribedPatron;
  }

  private static void PrintLog(string format, params object[] args)
  {
    string str = string.Format(format, args);
    Log.FiresideGatherings.PrintInfo(str);
    if (!HearthstoneApplication.IsInternal() || !FiresideGatheringPresenceManager.IsVerboseLoggingToScreen)
      return;
    float delay = 5f * Time.timeScale;
    if (InputCollection.GetKey(KeyCode.LeftShift) || InputCollection.GetKey(KeyCode.LeftControl) || Input.touchCount >= 2)
    {
      delay *= 2f;
      if (InputCollection.GetKey(KeyCode.LeftShift) && InputCollection.GetKey(KeyCode.LeftControl) || Input.touchCount >= 3)
        delay *= 2f;
    }
    UIStatus.Get().AddInfo(str, delay);
  }

  public void AddRemovePatronSubscriptions(
    List<FSGPatron> addedPatrons,
    List<FSGPatron> removedPatrons)
  {
    BnetGameAccountId bnetGameAccountId = BnetPresenceMgr.Get() == null ? (BnetGameAccountId) null : BnetPresenceMgr.Get().GetMyGameAccountId();
    ulong num = (BnetEntityId) bnetGameAccountId == (BnetEntityId) null ? 0UL : bnetGameAccountId.Low;
    List<FSGPatron> patronsSubscribed = (List<FSGPatron>) null;
    List<FSGPatron> patronsUnsubscribed = (List<FSGPatron>) null;
    System.Action<FSGPatron> action1 = (System.Action<FSGPatron>) null;
    System.Action<FSGPatron> action2 = (System.Action<FSGPatron>) null;
    if (FiresideGatheringPresenceManager.IsVerboseLogging)
    {
      patronsSubscribed = new List<FSGPatron>();
      patronsUnsubscribed = new List<FSGPatron>();
      action1 = (System.Action<FSGPatron>) (p =>
      {
        if (patronsSubscribed.Count >= 10)
          return;
        patronsSubscribed.Add(p);
      });
      action2 = (System.Action<FSGPatron>) (p =>
      {
        if (patronsUnsubscribed.Count >= 10)
          return;
        patronsUnsubscribed.Add(p);
      });
    }
    if (removedPatrons != null)
    {
      foreach (FSGPatron removedPatron in removedPatrons)
      {
        if (removedPatron != null)
        {
          BnetGameAccountId fromNet = BnetGameAccountId.CreateFromNet(removedPatron.GameAccount);
          if (this.m_subscribedPatronList.Remove(fromNet))
          {
            BattleNet.PresenceUnsubscribe((BnetEntityId) fromNet);
            BnetPresenceMgr.Get().CheckSubscriptionsAndClearTransientStatus(fromNet);
            if (action2 != null)
              action2(removedPatron);
          }
          this.m_previouslySubscribedPatrons.Remove(fromNet);
        }
      }
    }
    if (addedPatrons != null)
    {
      BnetFriendMgr bnetFriendMgr = BnetFriendMgr.Get();
      foreach (FSGPatron addedPatron in addedPatrons)
      {
        if (addedPatron != null)
        {
          if (this.CurrentSubscribedPlayerCount < FiresideGatheringPresenceManager.MAX_SUBSCRIBED_PATRONS)
          {
            BnetGameAccountId fromNet = BnetGameAccountId.CreateFromNet(addedPatron.GameAccount);
            if ((long) fromNet.Low != (long) num && (bnetFriendMgr == null || !bnetFriendMgr.IsFriend(fromNet)) && !this.m_subscribedPatronList.ContainsKey(fromNet))
            {
              BattleNet.PresenceSubscribe((BnetEntityId) fromNet);
              long timestampSeconds = TimeUtils.UnixTimestampSeconds;
              this.m_subscribedPatronList.Add(fromNet, timestampSeconds);
              this.UpdatePreviouslySubscribedPatron(fromNet, timestampSeconds);
              if (action1 != null)
                action1(addedPatron);
              if (FiresideGatheringPresenceManager.IsRequestBattleTagEnabled)
                this.RequestGameAccountBattleTag(fromNet);
            }
          }
          else
            break;
        }
      }
    }
    if (!FiresideGatheringPresenceManager.IsVerboseLogging || patronsSubscribed == null || patronsUnsubscribed == null || patronsSubscribed.Count <= 0 && patronsUnsubscribed.Count <= 0)
      return;
    FiresideGatheringPresenceManager.PrintLog("FSGPresence patron delta added={0} removed={1}\nadded=({2})\nremoved=({3})", (object) patronsSubscribed.Count, (object) patronsUnsubscribed.Count, (object) string.Join(", ", patronsSubscribed.Select<FSGPatron, string>((Func<FSGPatron, string>) (p => this.GetKnownPatronName(BnetGameAccountId.CreateFromNet(p.GameAccount)))).OrderBy<string, string>((Func<string, string>) (n => n)).ToArray<string>()), (object) string.Join(", ", patronsUnsubscribed.Select<FSGPatron, string>((Func<FSGPatron, string>) (p => this.GetKnownPatronName(BnetGameAccountId.CreateFromNet(p.GameAccount)))).OrderBy<string, string>((Func<string, string>) (n => n)).ToArray<string>()));
  }

  private void UpdateLastOnlineValuesForPreviouslySubscribedPatrons()
  {
    BnetPresenceMgr bnetPresenceMgr = BnetPresenceMgr.Get();
    long timestampSeconds = TimeUtils.UnixTimestampSeconds;
    foreach (KeyValuePair<BnetGameAccountId, FiresideGatheringPresenceManager.PreviouslySubscribedPatron> subscribedPatron in this.m_previouslySubscribedPatrons)
    {
      BnetPlayer player = bnetPresenceMgr.GetPlayer(subscribedPatron.Key);
      if (player != null)
      {
        BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
        if (!(hearthstoneGameAccount == (BnetGameAccount) null))
        {
          if (hearthstoneGameAccount.IsOnline())
            subscribedPatron.Value.m_lastOnlineUnixTimestamp = timestampSeconds;
          else if (subscribedPatron.Value.m_lastOnlineUnixTimestamp != 0L && timestampSeconds - subscribedPatron.Value.m_lastOnlineUnixTimestamp >= FiresideGatheringPresenceManager.PATRON_OLD_SUBSCRIPTION_THRESHOLD_SECONDS * 2L)
            subscribedPatron.Value.m_lastOnlineUnixTimestamp = 0L;
        }
      }
    }
  }

  private IEnumerable<BnetPlayer> GetOlderSubscribedPatronsThatAreNotDisplayable()
  {
    long now = TimeUtils.UnixTimestampSeconds;
    if (now - this.m_lastCheckPruneOlderPatronsUnixTimestamp < (long) FiresideGatheringPresenceManager.PERIODIC_SUBSCRIBE_CHECK_SECONDS)
      return (IEnumerable<BnetPlayer>) null;
    this.m_lastCheckPruneOlderPatronsUnixTimestamp = now;
    this.UpdateLastOnlineValuesForPreviouslySubscribedPatrons();
    BnetPresenceMgr presenceMgr = BnetPresenceMgr.Get();
    return this.m_subscribedPatronList.Where<KeyValuePair<BnetGameAccountId, long>>((Func<KeyValuePair<BnetGameAccountId, long>, bool>) (kv => now - kv.Value >= FiresideGatheringPresenceManager.PATRON_OLD_SUBSCRIPTION_THRESHOLD_SECONDS)).Select<KeyValuePair<BnetGameAccountId, long>, BnetPlayer>((Func<KeyValuePair<BnetGameAccountId, long>, BnetPlayer>) (kv => presenceMgr.GetPlayer(kv.Key))).Where<BnetPlayer>((Func<BnetPlayer, bool>) (p => p != null && !FiresideGatheringPresenceManager.IsDisplayable(p)));
  }

  public void CheckForMoreSubscribeOpportunities(
    List<BnetPlayer> patronsNoLongerDisplayable,
    IEnumerable<BnetPlayer> pendingPatrons)
  {
    bool flag1 = patronsNoLongerDisplayable == null;
    List<BnetGameAccountId> patronsSubscribed = (List<BnetGameAccountId>) null;
    List<BnetGameAccountId> patronsUnsubscribed = (List<BnetGameAccountId>) null;
    List<BnetPlayer> patronsOldPruned = (List<BnetPlayer>) null;
    System.Action<BnetGameAccountId> action1 = (System.Action<BnetGameAccountId>) null;
    System.Action<BnetGameAccountId> action2 = (System.Action<BnetGameAccountId>) null;
    System.Action<BnetPlayer> func = (System.Action<BnetPlayer>) null;
    if (FiresideGatheringPresenceManager.IsVerboseLogging)
    {
      patronsSubscribed = new List<BnetGameAccountId>();
      patronsUnsubscribed = new List<BnetGameAccountId>();
      patronsOldPruned = new List<BnetPlayer>();
      action1 = (System.Action<BnetGameAccountId>) (p =>
      {
        if (patronsSubscribed.Count >= 10)
          return;
        patronsSubscribed.Add(p);
      });
      action2 = (System.Action<BnetGameAccountId>) (p =>
      {
        if (patronsUnsubscribed.Count >= 10)
          return;
        patronsUnsubscribed.Add(p);
      });
      func = (System.Action<BnetPlayer>) (p =>
      {
        if (patronsOldPruned.Count >= 10)
          return;
        patronsOldPruned.Add(p);
      });
    }
    List<BnetPlayer> bnetPlayerList = patronsNoLongerDisplayable == null ? new List<BnetPlayer>() : new List<BnetPlayer>((IEnumerable<BnetPlayer>) patronsNoLongerDisplayable);
    IEnumerable<BnetPlayer> bnetPlayers = flag1 ? this.GetOlderSubscribedPatronsThatAreNotDisplayable() : (IEnumerable<BnetPlayer>) null;
    if (bnetPlayers != null)
    {
      int count = bnetPlayerList.Count;
      bnetPlayerList.AddRange(bnetPlayers);
      if (func != null)
        bnetPlayers.Take<BnetPlayer>(10).ForEach<BnetPlayer>(func);
    }
    if (bnetPlayerList.Count == 0 && this.CurrentSubscribedPlayerCount >= FiresideGatheringPresenceManager.MAX_SUBSCRIBED_PATRONS)
      return;
    BnetGameAccountId bnetGameAccountId1 = BnetPresenceMgr.Get() == null ? (BnetGameAccountId) null : BnetPresenceMgr.Get().GetMyGameAccountId();
    ulong myselfGameAccountIdLo = (BnetEntityId) bnetGameAccountId1 == (BnetEntityId) null ? 0UL : bnetGameAccountId1.Low;
    HashSet<BnetGameAccountId> bnetGameAccountIdSet = (HashSet<BnetGameAccountId>) null;
    foreach (BnetPlayer bnetPlayer in bnetPlayerList)
    {
      if (bnetPlayer != null)
      {
        BnetGameAccountId hearthstoneGameAccountId = bnetPlayer.GetHearthstoneGameAccountId();
        if (!((BnetEntityId) hearthstoneGameAccountId == (BnetEntityId) null) && this.m_subscribedPatronList.ContainsKey(hearthstoneGameAccountId))
        {
          if (bnetGameAccountIdSet == null)
            bnetGameAccountIdSet = new HashSet<BnetGameAccountId>();
          bnetGameAccountIdSet.Add(hearthstoneGameAccountId);
        }
      }
    }
    if (this.CurrentSubscribedPlayerCount - (bnetGameAccountIdSet == null ? 0 : bnetGameAccountIdSet.Count) < FiresideGatheringPresenceManager.MAX_SUBSCRIBED_PATRONS)
    {
      BnetFriendMgr friendMgr = BnetFriendMgr.Get();
      List<BnetPlayer> potentialPatrons = pendingPatrons == null ? new List<BnetPlayer>() : pendingPatrons.Where<BnetPlayer>((Func<BnetPlayer, bool>) (p =>
      {
        BnetGameAccountId bnetGameAccountId2 = p == null ? (BnetGameAccountId) null : p.GetHearthstoneGameAccountId();
        return p != null && !((BnetEntityId) bnetGameAccountId2 == (BnetEntityId) null) && (long) bnetGameAccountId2.Low != (long) myselfGameAccountIdLo && (friendMgr == null || !friendMgr.IsFriend(bnetGameAccountId2)) && !this.m_subscribedPatronList.ContainsKey(bnetGameAccountId2);
      })).ToList<BnetPlayer>();
      this.SortPotentialPatronForSubscription(potentialPatrons);
      foreach (BnetPlayer bnetPlayer in potentialPatrons)
      {
        if (bnetPlayer != null)
        {
          if (this.CurrentSubscribedPlayerCount - (bnetGameAccountIdSet == null ? 0 : bnetGameAccountIdSet.Count) < FiresideGatheringPresenceManager.MAX_SUBSCRIBED_PATRONS)
          {
            BnetGameAccountId hearthstoneGameAccountId = bnetPlayer.GetHearthstoneGameAccountId();
            if (!((BnetEntityId) hearthstoneGameAccountId == (BnetEntityId) null))
            {
              bool flag2 = this.m_subscribedPatronList.ContainsKey(hearthstoneGameAccountId);
              bool flag3 = bnetGameAccountIdSet != null && bnetGameAccountIdSet.Contains(hearthstoneGameAccountId);
              if (!flag2 || flag3)
              {
                if (flag3)
                  bnetGameAccountIdSet.Remove(hearthstoneGameAccountId);
                if (!flag2)
                {
                  BattleNet.PresenceSubscribe((BnetEntityId) hearthstoneGameAccountId);
                  long timestampSeconds = TimeUtils.UnixTimestampSeconds;
                  this.m_subscribedPatronList.Add(hearthstoneGameAccountId, timestampSeconds);
                  this.UpdatePreviouslySubscribedPatron(hearthstoneGameAccountId, timestampSeconds);
                  if (action1 != null)
                    action1(hearthstoneGameAccountId);
                  if (FiresideGatheringPresenceManager.IsRequestBattleTagEnabled)
                    this.RequestGameAccountBattleTag(hearthstoneGameAccountId);
                }
              }
            }
          }
          else
            break;
        }
      }
    }
    if (bnetGameAccountIdSet != null)
    {
      foreach (BnetGameAccountId bnetGameAccountId3 in bnetGameAccountIdSet)
      {
        this.m_subscribedPatronList.Remove(bnetGameAccountId3);
        BattleNet.PresenceUnsubscribe((BnetEntityId) bnetGameAccountId3);
        BnetPresenceMgr.Get().CheckSubscriptionsAndClearTransientStatus(bnetGameAccountId3);
        if (action2 != null)
          action2(bnetGameAccountId3);
      }
    }
    if (!FiresideGatheringPresenceManager.IsVerboseLogging || patronsSubscribed == null || patronsUnsubscribed == null || patronsSubscribed.Count == 0 && patronsUnsubscribed.Count == 0)
      return;
    int num = pendingPatrons == null ? 0 : pendingPatrons.Count<BnetPlayer>();
    FiresideGatheringPresenceManager.PrintLog("FSGPresence {0} newSubscribe={1} old={2} unsubscribed={3} total={4}\nnew=({5})\nold=({6})\nunsubscribed=({7})", flag1 ? (object) "periodic" : (object) "update", (object) patronsSubscribed.Count, (object) (patronsOldPruned == null ? 0 : patronsOldPruned.Count), (object) patronsUnsubscribed.Count, (object) num, (object) string.Join(", ", patronsSubscribed.Select<BnetGameAccountId, string>((Func<BnetGameAccountId, string>) (id => this.GetKnownPatronName(id))).OrderBy<string, string>((Func<string, string>) (n => n)).ToArray<string>()), patronsOldPruned == null ? (object) "" : (object) string.Join(", ", patronsOldPruned.Select<BnetPlayer, string>((Func<BnetPlayer, string>) (p => !((BnetEntityId) p.GetHearthstoneGameAccountId() == (BnetEntityId) null) ? this.GetKnownPatronName(p.GetHearthstoneGameAccountId()) : string.Empty)).OrderBy<string, string>((Func<string, string>) (n => n)).ToArray<string>()), (object) string.Join(", ", patronsUnsubscribed.Select<BnetGameAccountId, string>((Func<BnetGameAccountId, string>) (id => this.GetKnownPatronName(id))).OrderBy<string, string>((Func<string, string>) (n => n)).ToArray<string>()));
  }

  private void SortPotentialPatronForSubscription(List<BnetPlayer> potentialPatrons)
  {
    GeneralUtils.Shuffle<BnetPlayer>((IList<BnetPlayer>) potentialPatrons);
    potentialPatrons.Sort((Comparison<BnetPlayer>) ((a, b) =>
    {
      BnetGameAccountId hearthstoneGameAccountId1 = a.GetHearthstoneGameAccountId();
      BnetGameAccountId hearthstoneGameAccountId2 = b.GetHearthstoneGameAccountId();
      long num1 = 0;
      long num2 = 0;
      long num3 = 0;
      long num4 = 0;
      FiresideGatheringPresenceManager.PreviouslySubscribedPatron subscribedPatron1;
      if (this.m_previouslySubscribedPatrons.TryGetValue(hearthstoneGameAccountId1, out subscribedPatron1))
      {
        num1 = subscribedPatron1.m_lastSubscribeUnixTimestamp;
        num3 = subscribedPatron1.m_lastOnlineUnixTimestamp;
      }
      FiresideGatheringPresenceManager.PreviouslySubscribedPatron subscribedPatron2;
      if (this.m_previouslySubscribedPatrons.TryGetValue(hearthstoneGameAccountId2, out subscribedPatron2))
      {
        num2 = subscribedPatron2.m_lastSubscribeUnixTimestamp;
        num4 = subscribedPatron2.m_lastOnlineUnixTimestamp;
      }
      if (num1 != num2)
      {
        if (num1 == 0L)
          return -1;
        if (num2 == 0L)
          return 1;
      }
      if (num3 != num4)
      {
        if (num3 == 0L)
          return 1;
        return num4 == 0L ? -1 : num4.CompareTo(num3);
      }
      return num1 != num2 ? num1.CompareTo(num2) : 0;
    }));
  }

  public void RequestGameAccountBattleTag(BnetGameAccountId patronEntity) => BattleNet.RequestPresenceFields(true, (BnetEntityId) patronEntity, new PresenceFieldKey[1]
  {
    new PresenceFieldKey()
    {
      programId = BnetProgramId.BNET.GetValue(),
      groupId = 2U,
      fieldId = 5U,
      uniqueId = 0UL
    }
  });

  private void BnetPresenceMgr_OnGameAccountPresenceChange(PresenceUpdate[] updates)
  {
    long timestampSeconds = TimeUtils.UnixTimestampSeconds;
    foreach (PresenceUpdate update in updates)
    {
      if ((int) update.programId == (int) BnetProgramId.BNET.GetValue() && update.groupId == 2U && (update.fieldId == 5U || update.fieldId == 1U))
      {
        BnetGameAccountId key = new BnetGameAccountId(update.entityId?.EntityId);
        if (this.m_previouslySubscribedPatrons.ContainsKey(key))
        {
          if (update.fieldId == 5U)
          {
            if (this.m_previouslySubscribedPatronBattleTags == null)
              this.m_previouslySubscribedPatronBattleTags = new Map<BnetGameAccountId, string>();
            this.m_previouslySubscribedPatronBattleTags[key] = update.stringVal;
          }
          else if (update.fieldId == 1U && update.boolVal)
            this.m_previouslySubscribedPatrons[key].m_lastOnlineUnixTimestamp = timestampSeconds;
        }
      }
    }
  }

  private string GetKnownPatronName(BnetGameAccountId gameAccountId)
  {
    string str;
    return this.m_previouslySubscribedPatronBattleTags != null && this.m_previouslySubscribedPatronBattleTags.TryGetValue(gameAccountId, out str) ? str : gameAccountId.Low.ToString();
  }

  public void ClearSubscribedPatrons()
  {
    foreach (KeyValuePair<BnetGameAccountId, long> subscribedPatron in this.m_subscribedPatronList)
    {
      BattleNet.PresenceUnsubscribe((BnetEntityId) subscribedPatron.Key);
      BnetPresenceMgr.Get().CheckSubscriptionsAndClearTransientStatus(subscribedPatron.Key);
    }
    this.m_subscribedPatronList.Clear();
    this.m_previouslySubscribedPatrons.Clear();
    if (this.m_previouslySubscribedPatronBattleTags == null)
      return;
    this.m_previouslySubscribedPatronBattleTags.Clear();
  }

  private class PreviouslySubscribedPatron
  {
    public long m_lastSubscribeUnixTimestamp;
    public long m_lastOnlineUnixTimestamp;
  }
}
