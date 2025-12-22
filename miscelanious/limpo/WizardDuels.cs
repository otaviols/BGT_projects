using Hearthstone.DataModels;
using PegasusUtil;

public class WizardDuels : StandardGameEntity
{
  private PVPDRLobbyDataModel m_pvpdrDataModel;

  public override void OnCreate()
  {
    Network.Get().RegisterNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRSessionInfoResponse));
    this.GetPVPDRDataModel();
  }

  public override void StartGameplaySoundtracks()
  {
    if (this.m_pvpdrDataModel != null && this.m_pvpdrDataModel.Wins >= 9)
      MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_SCH_FinalLevels);
    else
      base.StartGameplaySoundtracks();
  }

  public override void StartMulliganSoundtracks(bool soft) => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_SCH_Mulligan);

  private PVPDRLobbyDataModel GetPVPDRDataModel()
  {
    if (this.m_pvpdrDataModel != null)
      return this.m_pvpdrDataModel;
    Network.Get().SendPVPDRSessionInfoRequest();
    return (PVPDRLobbyDataModel) null;
  }

  private void OnPVPDRSessionInfoResponse()
  {
    PVPDRSessionInfoResponse sessionInfoResponse = Network.Get().GetPVPDRSessionInfoResponse();
    if (sessionInfoResponse.HasSession)
    {
      this.m_pvpdrDataModel = new PVPDRLobbyDataModel();
      this.m_pvpdrDataModel.Wins = (int) sessionInfoResponse.Session.Wins;
      this.m_pvpdrDataModel.Losses = (int) sessionInfoResponse.Session.Losses;
      this.m_pvpdrDataModel.HasSession = sessionInfoResponse.Session.HasSession;
      this.m_pvpdrDataModel.IsSessionActive = sessionInfoResponse.Session.IsActive;
      this.m_pvpdrDataModel.IsPaidEntry = sessionInfoResponse.Session.IsPaidEntry;
      this.m_pvpdrDataModel.IsSessionRolledOver = sessionInfoResponse.Session.DidSeasonRollover;
    }
    Network.Get().RemoveNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRSessionInfoResponse));
  }
}
