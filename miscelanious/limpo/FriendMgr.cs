using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone;
using System.Collections.Generic;

public class FriendMgr
{
  private static FriendMgr s_instance;
  private BnetPlayer m_selectedFriend;
  private BnetPlayer m_recentOpponent;
  private List<FriendMgr.RecentOpponentListener> m_recentOpponentListeners = new List<FriendMgr.RecentOpponentListener>();

  public static FriendMgr Get()
  {
    if (FriendMgr.s_instance == null)
    {
      FriendMgr.s_instance = new FriendMgr();
      HearthstoneApplication.Get().WillReset += new System.Action(FriendMgr.s_instance.WillReset);
    }
    return FriendMgr.s_instance;
  }

  public BnetPlayer GetSelectedFriend() => this.m_selectedFriend;

  public void SetSelectedFriend(BnetPlayer friend) => this.m_selectedFriend = friend;

  public BnetPlayer GetRecentOpponent() => this.m_recentOpponent;

  private void UpdateRecentOpponent()
  {
    if (SpectatorManager.Get().IsSpectatingOrWatching || GameState.Get() == null)
      return;
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    if (opposingSidePlayer == null)
      return;
    BnetRecentPlayerMgr bnetRecentPlayerMgr = BnetRecentPlayerMgr.Get();
    BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(opposingSidePlayer.GetGameAccountId());
    if (player == null)
    {
      player = bnetRecentPlayerMgr.GetCurrentOpponent();
      if (player == null)
        return;
    }
    this.m_recentOpponent = player;
    bnetRecentPlayerMgr.AddRecentPlayer(player, BnetRecentPlayerMgr.RecentReason.LAST_OPPONENT);
    this.FireRecentOpponentEvent(this.m_recentOpponent);
  }

  public void FireRecentOpponentEvent(BnetPlayer recentOpponent)
  {
    foreach (FriendMgr.RecentOpponentListener opponentListener in this.m_recentOpponentListeners.ToArray())
      opponentListener.Fire(recentOpponent);
  }

  public void Initialize()
  {
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    Network.Get().AddBnetErrorListener(BnetFeature.Friends, new Network.BnetErrorCallback(this.OnBnetError));
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
  }

  private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
  {
    List<BnetPlayer> removedFriends = changelist.GetRemovedFriends();
    if (removedFriends == null || !removedFriends.Contains(this.m_selectedFriend))
      return;
    this.m_selectedFriend = (BnetPlayer) null;
  }

  private bool OnBnetError(BnetErrorInfo info, object userData)
  {
    int feature = (int) info.GetFeature();
    BnetFeatureEvent featureEvent = info.GetFeatureEvent();
    if (feature == 1 && featureEvent == BnetFeatureEvent.Friends_OnSendInvitation)
    {
      switch (info.GetError())
      {
        case BattleNetErrors.ERROR_OK:
          string message1 = GameStrings.Get("GLOBAL_ADDFRIEND_SENT_CONFIRMATION");
          UIStatus.Get().AddInfo(message1);
          return true;
        case BattleNetErrors.ERROR_FRIENDS_FRIENDSHIP_ALREADY_EXISTS:
          string message2 = GameStrings.Get("GLOBAL_ADDFRIEND_ERROR_ALREADY_FRIEND");
          UIStatus.Get().AddError(message2);
          return true;
      }
    }
    return false;
  }

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    BnetPlayerChange change = changelist.FindChange(this.m_selectedFriend);
    if (change == null)
      return;
    BnetPlayer oldPlayer = change.GetOldPlayer();
    BnetPlayer newPlayer = change.GetNewPlayer();
    if (oldPlayer != null && oldPlayer.IsOnline() == newPlayer.IsOnline())
      return;
    this.m_selectedFriend = (BnetPlayer) null;
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.GAMEPLAY)
      return;
    GameState gameState = GameState.Get();
    if (gameState == null)
    {
      Log.All.PrintWarning("FriendMgr.OnSceneLoaded event was fired when GameState was null!");
      gameState = GameState.Initialize();
    }
    gameState?.RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
  }

  private void OnGameOver(TAG_PLAYSTATE playState, object userData)
  {
    GameState.Get().UnregisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
    this.UpdateRecentOpponent();
  }

  private void WillReset()
  {
    BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    Network.Get().RemoveBnetErrorListener(BnetFeature.Friends, new Network.BnetErrorCallback(this.OnBnetError));
    BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
  }

  public delegate void RecentOpponentCallback(BnetPlayer recentOpponent, object userData);

  private class RecentOpponentListener : EventListener<FriendMgr.RecentOpponentCallback>
  {
    public void Fire(BnetPlayer recentOpponent) => this.m_callback(recentOpponent, this.m_userData);
  }
}
