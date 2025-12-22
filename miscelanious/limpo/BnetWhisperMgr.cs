using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Hearthstone;
using System;
using System.Collections.Generic;

public class BnetWhisperMgr
{
  private static BnetWhisperMgr s_instance;
  private List<BnetWhisper> m_whispers = new List<BnetWhisper>();
  private Map<BnetAccountId, List<BnetWhisper>> m_whisperMap = new Map<BnetAccountId, List<BnetWhisper>>();
  private int m_firstPendingWhisperIndex = -1;
  private List<BnetWhisperMgr.WhisperListener> m_whisperListeners = new List<BnetWhisperMgr.WhisperListener>();

  public static BnetWhisperMgr Get()
  {
    if (BnetWhisperMgr.s_instance == null)
    {
      BnetWhisperMgr.s_instance = new BnetWhisperMgr();
      HearthstoneApplication.Get().WillReset += (System.Action) (() =>
      {
        BnetWhisperMgr.s_instance.m_whispers.Clear();
        BnetWhisperMgr.s_instance.m_whisperMap.Clear();
        BnetWhisperMgr.s_instance.m_firstPendingWhisperIndex = -1;
        BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(BnetWhisperMgr.Get().OnPlayersChanged));
      });
    }
    return BnetWhisperMgr.s_instance;
  }

  public void Initialize()
  {
    Network.Get().SetWhisperHandler(new Network.WhisperHandler(this.OnWhispers));
    Network.Get().AddBnetErrorListener(BnetFeature.Whisper, new Network.BnetErrorCallback(this.OnBnetError));
  }

  public List<BnetWhisper> GetWhispersWithPlayer(BnetPlayer player)
  {
    if (player == null)
      return (List<BnetWhisper>) null;
    List<BnetWhisper> whispersWithPlayer = new List<BnetWhisper>();
    List<BnetWhisper> collection;
    if (this.m_whisperMap.TryGetValue(player.GetAccountId(), out collection))
      whispersWithPlayer.AddRange((IEnumerable<BnetWhisper>) collection);
    if (whispersWithPlayer.Count == 0)
      return (List<BnetWhisper>) null;
    whispersWithPlayer.Sort((Comparison<BnetWhisper>) ((a, b) =>
    {
      ulong timestampMicrosec1 = a.GetTimestampMicrosec();
      ulong timestampMicrosec2 = b.GetTimestampMicrosec();
      if (timestampMicrosec1 < timestampMicrosec2)
        return -1;
      return timestampMicrosec1 > timestampMicrosec2 ? 1 : 0;
    }));
    return whispersWithPlayer;
  }

  public bool SendWhisper(BnetPlayer player, string message)
  {
    if (player == null)
      return false;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (myPlayer == null || !myPlayer.IsOnline() || myPlayer.IsAppearingOffline())
      return false;
    BnetAccountId accountId = player.GetAccountId();
    if ((BnetEntityId) accountId == (BnetEntityId) null)
      return false;
    Network.SendWhisper(accountId, message);
    return true;
  }

  public bool HavePendingWhispers() => this.m_firstPendingWhisperIndex >= 0;

  public bool AddWhisperListener(BnetWhisperMgr.WhisperCallback callback) => this.AddWhisperListener(callback, (object) null);

  public bool AddWhisperListener(BnetWhisperMgr.WhisperCallback callback, object userData)
  {
    BnetWhisperMgr.WhisperListener whisperListener = new BnetWhisperMgr.WhisperListener();
    whisperListener.SetCallback(callback);
    whisperListener.SetUserData(userData);
    if (this.m_whisperListeners.Contains(whisperListener))
      return false;
    this.m_whisperListeners.Add(whisperListener);
    return true;
  }

  public bool RemoveWhisperListener(BnetWhisperMgr.WhisperCallback callback) => this.RemoveWhisperListener(callback, (object) null);

  public bool RemoveWhisperListener(BnetWhisperMgr.WhisperCallback callback, object userData)
  {
    BnetWhisperMgr.WhisperListener whisperListener = new BnetWhisperMgr.WhisperListener();
    whisperListener.SetCallback(callback);
    whisperListener.SetUserData(userData);
    return this.m_whisperListeners.Remove(whisperListener);
  }

  private void OnWhispers(BnetWhisper[] whispers)
  {
    for (int index = 0; index < whispers.Length; ++index)
    {
      BnetWhisper whisper = whispers[index];
      this.m_whispers.Add(whisper);
      if (!this.HavePendingWhispers())
      {
        if (WhisperUtil.IsDisplayable(whisper))
        {
          this.ProcessWhisper(this.m_whispers.Count - 1);
        }
        else
        {
          this.m_firstPendingWhisperIndex = index;
          BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
        }
      }
    }
  }

  private bool OnBnetError(BnetErrorInfo info, object userData) => true;

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (!this.CanProcessPendingWhispers())
      return;
    BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    this.ProcessPendingWhispers();
  }

  private void FireWhisperEvent(BnetWhisper whisper)
  {
    foreach (BnetWhisperMgr.WhisperListener whisperListener in this.m_whisperListeners.ToArray())
      whisperListener.Fire(whisper);
  }

  private bool CanProcessPendingWhispers()
  {
    if (this.m_firstPendingWhisperIndex < 0)
      return true;
    for (int pendingWhisperIndex = this.m_firstPendingWhisperIndex; pendingWhisperIndex < this.m_whispers.Count; ++pendingWhisperIndex)
    {
      if (!WhisperUtil.IsDisplayable(this.m_whispers[pendingWhisperIndex]))
        return false;
    }
    return true;
  }

  private void ProcessPendingWhispers()
  {
    if (this.m_firstPendingWhisperIndex < 0)
      return;
    for (int pendingWhisperIndex = this.m_firstPendingWhisperIndex; pendingWhisperIndex < this.m_whispers.Count; ++pendingWhisperIndex)
      this.ProcessWhisper(pendingWhisperIndex);
    this.m_firstPendingWhisperIndex = -1;
  }

  private void ProcessWhisper(int index)
  {
    BnetWhisper whisper = this.m_whispers[index];
    BnetAccountId theirAccountId = WhisperUtil.GetTheirAccountId(whisper);
    if ((BnetEntityId) theirAccountId == (BnetEntityId) null || !BnetUtils.CanReceiveWhisperFrom(theirAccountId))
    {
      this.m_whispers.RemoveAt(index);
    }
    else
    {
      List<BnetWhisper> whispers;
      if (!this.m_whisperMap.TryGetValue(theirAccountId, out whispers))
      {
        whispers = new List<BnetWhisper>();
        this.m_whisperMap.Add(theirAccountId, whispers);
      }
      else if (whispers.Count == 100)
        this.RemoveOldestWhisper(whispers);
      whispers.Add(whisper);
      this.FireWhisperEvent(whisper);
    }
  }

  private void RemoveOldestWhisper(List<BnetWhisper> whispers)
  {
    BnetWhisper whisper = whispers[0];
    whispers.RemoveAt(0);
    this.m_whispers.Remove(whisper);
  }

  public delegate void WhisperCallback(BnetWhisper whisper, object userData);

  private class WhisperListener : EventListener<BnetWhisperMgr.WhisperCallback>
  {
    public void Fire(BnetWhisper whisper) => this.m_callback(whisper, this.m_userData);
  }
}
