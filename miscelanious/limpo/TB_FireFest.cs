using System.Collections;
using UnityEngine;

public class TB_FireFest : MissionEntity
{
  private Notification m_popup;
  private int m_deaths;

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_FireFest tbFireFest = this;
    while (tbFireFest.m_enemySpeaking)
      yield return (object) null;
    if (tbFireFest.m_deaths <= 7)
    {
      Vector3 popUpPos = new Vector3();
      popUpPos.z = (bool) UniversalInputManager.UsePhoneUI ? -66f : -44f;
      if (missionEvent == 99)
      {
        ++tbFireFest.m_deaths;
        if (tbFireFest.m_deaths == 1)
          yield return (object) tbFireFest.ShowPopup("TB_FIREFEST_FIRST", 7f, popUpPos);
        else if (tbFireFest.m_deaths == 7)
          yield return (object) tbFireFest.ShowPopup("TB_FIREFEST_SECOND", 2.5f, popUpPos);
      }
    }
  }

  private IEnumerator ShowPopup(string stringID, float popupDuration, Vector3 popUpPos)
  {
    this.m_popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPos, TutorialEntity.GetTextScale() * 2.5f, GameStrings.Get(stringID), false);
    NotificationManager.Get().DestroyNotification(this.m_popup, popupDuration);
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(4f);
    GameState.Get().SetBusy(false);
  }

  public TB_FireFest()
    : base()
  {
  }
}
