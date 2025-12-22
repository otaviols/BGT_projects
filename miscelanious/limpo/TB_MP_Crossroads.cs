using System.Collections;
using UnityEngine;

public class TB_MP_Crossroads : MissionEntity
{
  private Notification CrasherPopup;
  private string crasherText = "TB_MP_CROSSROADS";
  private Vector3 popUpPos;
  private float popupScale = 1.25f;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_MP_Crossroads tbMpCrossroads = this;
    while (tbMpCrossroads.m_enemySpeaking)
      yield return (object) null;
    tbMpCrossroads.popUpPos = new Vector3(-55f, 0.0f, -10f);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      tbMpCrossroads.popUpPos.x = -74f;
      tbMpCrossroads.popUpPos.z = -21f;
      tbMpCrossroads.popupScale = 1.75f;
    }
    if (missionEvent == 99)
    {
      yield return (object) new WaitForSeconds(2f);
      if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
      {
        tbMpCrossroads.popUpPos.x = 55f;
        tbMpCrossroads.popUpPos.z = 19f;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          tbMpCrossroads.popUpPos.x = 75f;
          tbMpCrossroads.popUpPos.z = 17f;
        }
      }
      GameState.Get().SetBusy(true);
      tbMpCrossroads.CrasherPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMpCrossroads.popUpPos, TutorialEntity.GetTextScale() * tbMpCrossroads.popupScale, GameStrings.Get(tbMpCrossroads.crasherText), false);
      NotificationManager.Get().DestroyNotification(tbMpCrossroads.CrasherPopup, 3f);
      yield return (object) new WaitForSeconds(1f);
      GameState.Get().SetBusy(false);
    }
  }

  public TB_MP_Crossroads()
    : base()
  {
  }
}
