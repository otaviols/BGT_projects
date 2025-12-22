using System.Collections;
using UnityEngine;

public class GameOpenPack : MonoBehaviour
{
  public PlayMakerFSM m_playMakerFSM;
  private bool clickedOnPack;
  private bool fullyLoaded;

  public void Finish()
  {
    if (GameState.Get() == null)
      return;
    GameState.Get().GetGameEntity().NotifyOfCustomIntroFinished();
  }

  public void PlayJainaLine() => GameState.Get().GetGameEntity().SendCustomEvent(66);

  public void PlayHoggerLine()
  {
    int num = (Object) MulliganManager.Get() == (Object) null ? 1 : 0;
  }

  private IEnumerator PlayHoggerAfterVersus()
  {
    yield return (object) new WaitForSeconds(1f);
    Card heroCard = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    SoundManager.Get().Play(heroCard.GetAnnouncerLine(Card.AnnouncerLineType.DEFAULT));
  }

  public void RaiseBoardLights() => Board.Get().RaiseTheLights();

  public void Begin()
  {
    if (GameState.Get() == null)
      return;
    GameState.Get().GetGameEntity().NotifyOfGamePackOpened();
  }

  public void NotifyOfFullyLoaded() => this.fullyLoaded = true;

  public void NotifyOfMouseOver()
  {
    if (!this.fullyLoaded || this.clickedOnPack)
      return;
    this.m_playMakerFSM.SendEvent("Birth");
  }

  public void NotifyOfMouseOff()
  {
    if (!this.fullyLoaded || this.clickedOnPack)
      return;
    this.m_playMakerFSM.SendEvent("Cancel");
  }

  public void HandleClick()
  {
    if (!this.fullyLoaded || this.clickedOnPack || !SceneMgr.Get().IsSceneLoaded() || (Object) LoadingScreen.Get() != (Object) null && LoadingScreen.Get().IsTransitioning())
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.Misc_Tutorial01PackOpen);
    this.clickedOnPack = true;
    this.m_playMakerFSM.SendEvent("Action");
  }
}
