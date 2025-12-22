using Blizzard.GameService.SDK.Client.Integration;
using UnityEngine;

public class PlayerIcon : PegUIElement
{
  public GameObject m_OfflineIcon;
  public GameObject m_OnlineIcon;
  public PlayerPortrait m_OnlinePortrait;
  private bool m_hidden;
  private BnetPlayer m_player;

  public void Hide()
  {
    this.m_hidden = true;
    this.gameObject.SetActive(false);
  }

  public void Show()
  {
    this.m_hidden = false;
    this.gameObject.SetActive(true);
  }

  public BnetPlayer GetPlayer() => this.m_player;

  public bool SetPlayer(BnetPlayer player)
  {
    if (this.m_player == player)
      return false;
    this.m_player = player;
    this.UpdateIcon();
    return true;
  }

  public void UpdateIcon()
  {
    if (this.m_player == null)
      return;
    BnetProgramId bestProgramId = this.m_player.GetBestProgramId();
    bool flag = false;
    if ((Blizzard.GameService.SDK.Client.Integration.FourCC) bestProgramId != (Blizzard.GameService.SDK.Client.Integration.FourCC) null)
      flag = bestProgramId.IsGame();
    if (this.m_player.IsOnline() & flag && PlayerPortrait.GetTextureName(bestProgramId) != null)
    {
      if (!this.m_hidden)
        this.gameObject.SetActive(true);
      this.m_OnlinePortrait.SetProgramId(bestProgramId);
    }
    else
      this.gameObject.SetActive(false);
  }
}
