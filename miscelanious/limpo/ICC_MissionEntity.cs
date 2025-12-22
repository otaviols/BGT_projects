using System.Collections;
using UnityEngine;

public class ICC_MissionEntity : GenericDungeonMissionEntity
{
  public Vector3 ragLinePosition = new Vector3(95f, NotificationManager.DEPTH, 36.8f);

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ICCMulligan);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ICC);

  protected Actor GetActorByCardId(string cardId)
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    foreach (Card card in friendlySidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == friendlySidePlayer.GetPlayerId() && entity.GetCardId() == cardId)
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

  protected Actor GetLichKingFriendlyMinion() => this.GetActorByCardId("ICC_314");

  protected IEnumerator IfPlayerPlaysDKHeroVO(
    Entity entity,
    Actor actor,
    string voString)
  {
    ICC_MissionEntity iccMissionEntity = this;
    if (entity.GetCardType() == TAG_CARDTYPE.HERO && entity.GetCardSet() == TAG_CARD_SET.ICECROWN)
    {
      yield return (object) new WaitForSeconds(0.3f);
      yield return (object) iccMissionEntity.PlayEasterEggLine(actor, voString);
    }
  }
}
