using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB16_MP_Stormwind : MissionEntity
{
  private Notification MyPopup;
  private Vector3 popUpPos;
  private string textID;
  private bool doPopup;
  private bool doLeftArrow;
  private bool doUpArrow;
  private bool doDownArrow;
  private float delayTime;
  private float popupDuration = 2.5f;
  private float popupScale = 2.5f;
  private HashSet<int> seen = new HashSet<int>();
  private static readonly Dictionary<int, string> minionMsgs = new Dictionary<int, string>()
  {
    {
      10,
      "TB_MP_STORMWIND_SUCCESS"
    },
    {
      11,
      "TB_MP_BOSS2"
    },
    {
      12,
      "TB_MP_BOSS3"
    },
    {
      13,
      "TB_MP_BOSS"
    }
  };

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB16_MP_Stormwind tb16MpStormwind = this;
    while (tb16MpStormwind.m_enemySpeaking)
      yield return (object) null;
    if (!tb16MpStormwind.seen.Contains(missionEvent))
    {
      tb16MpStormwind.seen.Add(missionEvent);
      tb16MpStormwind.doPopup = false;
      tb16MpStormwind.doLeftArrow = false;
      tb16MpStormwind.doUpArrow = false;
      tb16MpStormwind.doDownArrow = false;
      tb16MpStormwind.delayTime = 0.0f;
      tb16MpStormwind.popupDuration = 2.5f;
      tb16MpStormwind.textID = GameStrings.Get(TB16_MP_Stormwind.minionMsgs[missionEvent]);
      if (missionEvent == 10)
      {
        tb16MpStormwind.doPopup = true;
        tb16MpStormwind.popUpPos.x = 0.0f;
        tb16MpStormwind.popUpPos.z = 4f;
        if (!(bool) UniversalInputManager.UsePhoneUI)
          ;
      }
      else
        Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName(tb16MpStormwind.textID);
      if (tb16MpStormwind.doPopup)
      {
        if (missionEvent == 1)
        {
          tb16MpStormwind.delayTime = 5f;
          tb16MpStormwind.popupDuration = 5f;
        }
        yield return (object) new WaitForSeconds(tb16MpStormwind.delayTime);
        tb16MpStormwind.MyPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb16MpStormwind.popUpPos, TutorialEntity.GetTextScale() * tb16MpStormwind.popupScale, GameStrings.Get(tb16MpStormwind.textID), false);
        if (tb16MpStormwind.doLeftArrow)
          tb16MpStormwind.MyPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
        if (tb16MpStormwind.doUpArrow)
          tb16MpStormwind.MyPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
        if (tb16MpStormwind.doDownArrow)
          tb16MpStormwind.MyPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
        tb16MpStormwind.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(tb16MpStormwind.MyPopup, tb16MpStormwind.popupDuration);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public TB16_MP_Stormwind()
    : base()
  {
  }
}
