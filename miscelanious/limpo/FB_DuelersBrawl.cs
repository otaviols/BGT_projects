using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_DuelersBrawl : MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = FB_DuelersBrawl.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = FB_DuelersBrawl.InitStringOptions();
  private Notification m_popup;
  private static readonly Dictionary<int, FB_DuelersBrawl.PopupMessage> popupMsgs = new Dictionary<int, FB_DuelersBrawl.PopupMessage>()
  {
    {
      1,
      new FB_DuelersBrawl.PopupMessage()
      {
        Message = "TB_DUELERSBRAWL_TAKE_PACES",
        Delay = 6f
      }
    },
    {
      2,
      new FB_DuelersBrawl.PopupMessage()
      {
        Message = "TB_DUELERSBRAWL_SUDDEN_DEATH",
        Delay = 6f
      }
    }
  };

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public FB_DuelersBrawl()
    : base()
  {
    this.m_gameOptions.AddOptions(FB_DuelersBrawl.s_booleanOptions, FB_DuelersBrawl.s_stringOptions);
  }

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  public override bool ShouldDoAlternateMulliganIntro() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    FB_DuelersBrawl fbDuelersBrawl = this;
    while (fbDuelersBrawl.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 10)
    {
      if (GameState.Get().IsFriendlySidePlayerTurn())
        TurnStartManager.Get().BeginListeningForTurnEvents();
      GameState.Get().SetBusy(true);
      yield return (object) new WaitForSeconds(2f);
      GameState.Get().SetBusy(false);
    }
    else
    {
      Vector3 popUpPos = new Vector3();
      if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
      {
        popUpPos.z = (bool) UniversalInputManager.UsePhoneUI ? 27f : 18f;
      }
      else
      {
        popUpPos.z = (bool) UniversalInputManager.UsePhoneUI ? -18f : -12f;
        yield return (object) new WaitForSeconds(3f);
      }
      yield return (object) fbDuelersBrawl.ShowPopup(GameStrings.Get(FB_DuelersBrawl.popupMsgs[missionEvent].Message), FB_DuelersBrawl.popupMsgs[missionEvent].Delay, popUpPos);
      popUpPos = new Vector3();
    }
  }

  private IEnumerator ShowPopup(string stringID, float popupDuration, Vector3 popUpPos)
  {
    this.m_popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPos, TutorialEntity.GetTextScale() * 1.4f, GameStrings.Get(stringID), false, NotificationManager.PopupTextType.FANCY);
    NotificationManager.Get().DestroyNotification(this.m_popup, popupDuration);
    yield return (object) new WaitForSeconds(1f);
  }

  public struct PopupMessage
  {
    public string Message;
    public float Delay;
  }
}
