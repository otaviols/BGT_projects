using Blizzard.GameService.SDK.Client.Integration;
using System;
using System.Collections.Generic;

public class BnetPlayerChangelist
{
  private List<BnetPlayerChange> m_changes = new List<BnetPlayerChange>();

  public List<BnetPlayerChange> GetChanges() => this.m_changes;

  public void AddChange(BnetPlayerChange change) => this.m_changes.Add(change);

  public bool HasChange(BnetPlayer player) => this.FindChange(player) != null;

  public BnetPlayerChange FindChange(BnetGameAccountId id) => this.FindChange(BnetPresenceMgr.Get().GetPlayer(id));

  public BnetPlayerChange FindChange(BnetPlayer player) => player == null ? (BnetPlayerChange) null : this.m_changes.Find((Predicate<BnetPlayerChange>) (change => change.GetPlayer() == player));
}
