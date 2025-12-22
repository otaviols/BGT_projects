using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_ChooseYourFateRandom : MissionEntity
{
  private Notification ChooseYourFatePopup;
  private Vector3 popUpPos;
  private string textID;
  private string newFate = "TB_PICKYOURFATE_RANDOM_NEWFATE";
  private string opponentFate = "TB_PICKYOURFATE_RANDOM_OPPONENTFATE";
  private string firstFate = "TB_PICKYOURFATE_RANDOM_FIRSTFATE";
  private string firstOpponenentFate = "TB_PICKYOURFATE_BUILDAROUND_OPPONENT_FIRSTFATE";
  private HashSet<int> seen = new HashSet<int>();

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_ChooseYourFateRandom chooseYourFateRandom = this;
    while (chooseYourFateRandom.m_enemySpeaking)
      yield return (object) null;
    if (!chooseYourFateRandom.seen.Contains(missionEvent))
    {
      chooseYourFateRandom.seen.Add(missionEvent);
      chooseYourFateRandom.popUpPos = new Vector3(-46f, 0.0f, 0.0f);
      int entityId1 = GameState.Get().GetFriendlySidePlayer().GetEntityId();
      int entityId2 = GameState.Get().GetOpposingSidePlayer().GetEntityId();
      if (missionEvent > 1000)
      {
        int num = missionEvent - 1000;
        missionEvent -= num;
        if (num == entityId1)
        {
          chooseYourFateRandom.popUpPos.z = -44f;
          chooseYourFateRandom.textID = chooseYourFateRandom.newFate;
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            chooseYourFateRandom.popUpPos.x = -51f;
            chooseYourFateRandom.popUpPos.z = -62f;
          }
        }
        if (num == entityId2)
        {
          chooseYourFateRandom.popUpPos.z = 44f;
          chooseYourFateRandom.textID = chooseYourFateRandom.opponentFate;
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            chooseYourFateRandom.popUpPos.x = -51f;
            chooseYourFateRandom.popUpPos.z = 53f;
          }
        }
      }
      int num1 = missionEvent;
      if ((uint) (num1 - 1) > 19U)
      {
        if (num1 == 1000)
        {
          chooseYourFateRandom.ChooseYourFatePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, chooseYourFateRandom.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(chooseYourFateRandom.textID), false);
          chooseYourFateRandom.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
          NotificationManager.Get().DestroyNotification(chooseYourFateRandom.ChooseYourFatePopup, 5f);
          chooseYourFateRandom.ChooseYourFatePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
        }
      }
      else
      {
        if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
        {
          chooseYourFateRandom.textID = chooseYourFateRandom.firstFate;
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            chooseYourFateRandom.popUpPos.x = -77f;
            chooseYourFateRandom.popUpPos.z = 30.5f;
          }
          else
          {
            chooseYourFateRandom.popUpPos.x = -50.5f;
            chooseYourFateRandom.popUpPos.z = 29f;
          }
        }
        else
        {
          chooseYourFateRandom.textID = chooseYourFateRandom.firstOpponenentFate;
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            chooseYourFateRandom.popUpPos.x = -34f;
            chooseYourFateRandom.popUpPos.z = 12f;
          }
          else
          {
            chooseYourFateRandom.popUpPos.x = -7f;
            chooseYourFateRandom.popUpPos.z = 9f;
          }
        }
        yield return (object) new WaitForSeconds(1f);
        chooseYourFateRandom.ChooseYourFatePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, chooseYourFateRandom.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(chooseYourFateRandom.textID), false);
        chooseYourFateRandom.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(chooseYourFateRandom.ChooseYourFatePopup, 3f);
        chooseYourFateRandom.ChooseYourFatePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
      }
    }
  }

  public TB_ChooseYourFateRandom()
    : base()
  {
  }
}
