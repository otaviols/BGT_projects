using Blizzard.GameService.SDK.Client.Integration;

public class SharedPlayerInfo : Entity
{
  private BnetGameAccountId m_gameAccountId;
  private string m_name;
  private Entity m_playerHero;

  public void InitPlayerInfo(
    Network.HistCreateGame.SharedPlayerInfo netPlayerInfo)
  {
    this.SetPlayerId(netPlayerInfo.ID);
    this.SetGameAccountId(netPlayerInfo.GameAccountId);
  }

  public int GetPlayerId() => this.GetTag(GAME_TAG.PLAYER_ID);

  public void SetPlayerId(int playerId) => this.SetTag(GAME_TAG.PLAYER_ID, playerId);

  public Entity GetPlayerHero() => this.m_playerHero;

  public void SetPlayerHero(Entity playerHero) => this.m_playerHero = playerHero;

  public void SetGameAccountId(BnetGameAccountId id)
  {
    this.m_gameAccountId = id;
    if (this.IsDisplayable())
    {
      this.UpdateDisplayInfo();
    }
    else
    {
      if (!GameUtils.IsBnetPlayer(this.m_gameAccountId))
        return;
      BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnBnetPlayersChanged));
      if (BnetFriendMgr.Get().IsFriend(this.m_gameAccountId))
        return;
      GameUtils.RequestPlayerPresence(this.m_gameAccountId);
    }
  }

  public override string GetName() => this.m_name;

  private void OnBnetPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (changelist.FindChange(this.m_gameAccountId) == null || !this.IsDisplayable())
      return;
    BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnBnetPlayersChanged));
    this.UpdateDisplayInfo();
  }

  public bool IsDisplayable()
  {
    if ((BnetEntityId) this.m_gameAccountId == (BnetEntityId) null)
      return false;
    BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
    if (player == null || !player.IsDisplayable())
      return false;
    if (GameUtils.IsGameTypeRanked())
    {
      BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
      if (hearthstoneGameAccount == (BnetGameAccount) null || !hearthstoneGameAccount.HasGameField(18U))
        return false;
    }
    return true;
  }

  private void UpdateDisplayInfo() => this.UpdateName();

  private void UpdateName()
  {
    if (GameUtils.IsBnetPlayer(this.m_gameAccountId))
    {
      BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
      if (player != null)
        this.m_name = player.GetBestName();
      if (string.IsNullOrEmpty(this.m_name))
        return;
      GameMgr.Get().SetLastDisplayedPlayerName(this.GetPlayerId(), this.m_name);
    }
    else
      this.m_name = "Player " + (object) this.GetPlayerId();
  }
}
