using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_ChooseYourFateBuildaround : MissionEntity
{
  private Notification ChooseYourFatePopup;
  private Vector3 popUpPos;
  private string textID;
  private string friendlyFate = "TB_PICKYOURFATE_BUILDAROUND_NEWFATE";
  private string opposingFate = "TB_PICKYOURFATE_BUILDAROUND_OPPONENTFATE";
  private HashSet<int> seen = new HashSet<int>();

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_ChooseYourFateBuildaround yourFateBuildaround = this;
    while (yourFateBuildaround.m_enemySpeaking)
      yield return (object) null;
    if (!yourFateBuildaround.seen.Contains(missionEvent))
    {
      yourFateBuildaround.seen.Add(missionEvent);
      yourFateBuildaround.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
      switch (missionEvent)
      {
        case 1:
        case 2:
        case 3:
          if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
          {
            yourFateBuildaround.textID = yourFateBuildaround.friendlyFate;
            if ((bool) UniversalInputManager.UsePhoneUI)
            {
              yourFateBuildaround.popUpPos.x = -75f;
              yourFateBuildaround.popUpPos.z = 30.5f;
            }
            else
            {
              yourFateBuildaround.popUpPos.x = -50.5f;
              yourFateBuildaround.popUpPos.z = 29f;
            }
          }
          else
          {
            yourFateBuildaround.textID = yourFateBuildaround.opposingFate;
            if ((bool) UniversalInputManager.UsePhoneUI)
            {
              yourFateBuildaround.popUpPos.x = -34f;
              yourFateBuildaround.popUpPos.z = 12f;
            }
            else
            {
              yourFateBuildaround.popUpPos.x = -7f;
              yourFateBuildaround.popUpPos.z = 9f;
            }
          }
          yield return (object) new WaitForSeconds(1f);
          yourFateBuildaround.ChooseYourFatePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, yourFateBuildaround.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(yourFateBuildaround.textID), false);
          yourFateBuildaround.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
          NotificationManager.Get().DestroyNotification(yourFateBuildaround.ChooseYourFatePopup, 3f);
          yourFateBuildaround.ChooseYourFatePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
          break;
      }
    }
  }

  public TB_ChooseYourFateBuildaround()
    : base()
  {
  }
}
