using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB09_ShadowTowers : MissionEntity
{
  private Notification ShadowTowerPopup;
  private Vector3 popUpPos;
  private string textID;
  private bool doPopup;
  private bool doLeftArrow;
  private bool doUpArrow;
  private bool doDownArrow;
  private float delayTime;
  private float popupDuration;
  private HashSet<int> seen = new HashSet<int>();
  private static readonly Dictionary<int, string> minionMsgs = new Dictionary<int, string>()
  {
    {
      1,
      "TB_SHADOWTOWERS_SHADOWSPAWNED"
    },
    {
      2,
      "TB_SHADOWTOWERS_SHADOWSPAWNED"
    },
    {
      3,
      "TB_SHADOWTOWERS_ADJACENTMINIONS"
    },
    {
      4,
      "TB_SHADOWTOWERS_SHADOWSPAWNEDNEXT"
    }
  };

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB09_ShadowTowers tb09ShadowTowers = this;
    while (tb09ShadowTowers.m_enemySpeaking)
      yield return (object) null;
    if (!tb09ShadowTowers.seen.Contains(missionEvent))
    {
      tb09ShadowTowers.seen.Add(missionEvent);
      tb09ShadowTowers.doPopup = false;
      tb09ShadowTowers.doLeftArrow = false;
      tb09ShadowTowers.doUpArrow = false;
      tb09ShadowTowers.doDownArrow = false;
      switch (missionEvent)
      {
        case 1:
        case 2:
          if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
          {
            yield break;
          }
          else
          {
            tb09ShadowTowers.doPopup = true;
            tb09ShadowTowers.textID = TB09_ShadowTowers.minionMsgs[missionEvent];
            tb09ShadowTowers.doLeftArrow = true;
            tb09ShadowTowers.delayTime = 3f;
            tb09ShadowTowers.popUpPos.x = 46f;
            tb09ShadowTowers.popUpPos.z = -9f;
            tb09ShadowTowers.popupDuration = 4f;
            if (!(bool) UniversalInputManager.UsePhoneUI)
              break;
            break;
          }
        case 3:
        case 4:
          tb09ShadowTowers.doPopup = true;
          tb09ShadowTowers.textID = TB09_ShadowTowers.minionMsgs[missionEvent];
          tb09ShadowTowers.delayTime = 0.0f;
          tb09ShadowTowers.popUpPos.x = 0.0f;
          tb09ShadowTowers.popUpPos.z = 20f;
          tb09ShadowTowers.popupDuration = 3f;
          if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
          {
            tb09ShadowTowers.popUpPos.z = -11f;
            if (missionEvent == 3)
              tb09ShadowTowers.doDownArrow = true;
          }
          else if (missionEvent == 3)
            tb09ShadowTowers.doUpArrow = true;
          if (!(bool) UniversalInputManager.UsePhoneUI)
            break;
          break;
        case 11:
          NotificationManager.Get().DestroyNotification(tb09ShadowTowers.ShadowTowerPopup, 0.0f);
          tb09ShadowTowers.doPopup = false;
          break;
      }
      if (tb09ShadowTowers.doPopup)
      {
        yield return (object) new WaitForSeconds(tb09ShadowTowers.delayTime);
        float num = 1.5f;
        tb09ShadowTowers.ShadowTowerPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb09ShadowTowers.popUpPos, TutorialEntity.GetTextScale() * num, GameStrings.Get(tb09ShadowTowers.textID), false);
        if (tb09ShadowTowers.doLeftArrow)
          tb09ShadowTowers.ShadowTowerPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
        if (tb09ShadowTowers.doUpArrow)
          tb09ShadowTowers.ShadowTowerPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
        if (tb09ShadowTowers.doDownArrow)
          tb09ShadowTowers.ShadowTowerPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
        tb09ShadowTowers.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(tb09ShadowTowers.ShadowTowerPopup, tb09ShadowTowers.popupDuration);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(5f);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public TB09_ShadowTowers()
    : base()
  {
  }
}
