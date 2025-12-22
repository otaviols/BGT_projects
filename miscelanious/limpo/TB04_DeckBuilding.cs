using Blizzard.T5.Core;
using System.Collections;
using UnityEngine;

public class TB04_DeckBuilding : MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = TB04_DeckBuilding.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = TB04_DeckBuilding.InitStringOptions();
  private Notification PickThreePopup;
  private Notification EndOfTurnPopup;
  private Notification StartOfTurnPopup;
  private Notification CardPlayedPopup;
  private Vector3 popUpPos;
  private string textID;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public TB04_DeckBuilding()
    : base()
  {
    this.m_gameOptions.AddOptions(TB04_DeckBuilding.s_booleanOptions, TB04_DeckBuilding.s_stringOptions);
  }

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  public override bool ShouldDoAlternateMulliganIntro() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB04_DeckBuilding tb04DeckBuilding = this;
    while (tb04DeckBuilding.m_enemySpeaking)
      yield return (object) null;
    tb04DeckBuilding.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
    switch (missionEvent)
    {
      case 1:
        if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
          break;
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        tb04DeckBuilding.textID = "TB_DECKBUILDING_FIRSTPICKTHREE";
        tb04DeckBuilding.popUpPos.z = !(bool) UniversalInputManager.UsePhoneUI ? -44f : -66f;
        tb04DeckBuilding.PickThreePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb04DeckBuilding.popUpPos, TutorialEntity.GetTextScale() * 1.25f, GameStrings.Get(tb04DeckBuilding.textID), false);
        tb04DeckBuilding.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.PickThreePopup, 12f);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(1f);
        GameState.Get().SetBusy(false);
        break;
      case 2:
        if (!(bool) (Object) tb04DeckBuilding.PickThreePopup)
          break;
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.PickThreePopup, 0.25f);
        break;
      case 3:
        if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
          break;
        tb04DeckBuilding.textID = "TB_DECKBUILDING_FIRSTENDTURN";
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          tb04DeckBuilding.popUpPos.x = 82f;
          tb04DeckBuilding.popUpPos.z = -28f;
        }
        else
          tb04DeckBuilding.popUpPos.z = -36f;
        GameState.Get().SetBusy(true);
        tb04DeckBuilding.EndOfTurnPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb04DeckBuilding.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(tb04DeckBuilding.textID), false);
        tb04DeckBuilding.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.EndOfTurnPopup, 5f);
        tb04DeckBuilding.EndOfTurnPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.CardPlayedPopup, 0.0f);
        yield return (object) new WaitForSeconds(3.5f);
        GameState.Get().SetBusy(false);
        break;
      case 4:
        if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
          break;
        tb04DeckBuilding.textID = "TB_DECKBUILDING_FIRSTCARDPLAYED";
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          tb04DeckBuilding.popUpPos.x = 82f;
          tb04DeckBuilding.popUpPos.y = 0.0f;
          tb04DeckBuilding.popUpPos.z = -28f;
        }
        else
        {
          tb04DeckBuilding.popUpPos.x = 51f;
          tb04DeckBuilding.popUpPos.y = 0.0f;
          tb04DeckBuilding.popUpPos.z = -15.5f;
        }
        GameState.Get().SetBusy(true);
        tb04DeckBuilding.CardPlayedPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb04DeckBuilding.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(tb04DeckBuilding.textID), false);
        tb04DeckBuilding.CardPlayedPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
        tb04DeckBuilding.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.CardPlayedPopup, 10f);
        yield return (object) new WaitForSeconds(3f);
        GameState.Get().SetBusy(false);
        if (!((Object) tb04DeckBuilding.CardPlayedPopup != (Object) null))
          break;
        iTween.PunchScale(tb04DeckBuilding.CardPlayedPopup.gameObject, iTween.Hash((object) "amount", (object) new Vector3(2f, 2f, 2f), (object) "time", (object) 1f));
        break;
      case 10:
        if (GameState.Get().IsFriendlySidePlayerTurn())
          TurnStartManager.Get().BeginListeningForTurnEvents();
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        tb04DeckBuilding.textID = "TB_DECKBUILDING_STARTOFGAME";
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          tb04DeckBuilding.popUpPos.x = 0.0f;
          tb04DeckBuilding.popUpPos.y = 0.0f;
          tb04DeckBuilding.popUpPos.z = 0.0f;
        }
        else
        {
          tb04DeckBuilding.popUpPos.x = 0.0f;
          tb04DeckBuilding.popUpPos.y = 0.0f;
          tb04DeckBuilding.popUpPos.z = 0.0f;
        }
        GameState.Get().SetBusy(true);
        tb04DeckBuilding.StartOfTurnPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb04DeckBuilding.popUpPos, TutorialEntity.GetTextScale() * 2f, GameStrings.Get(tb04DeckBuilding.textID), false);
        tb04DeckBuilding.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.StartOfTurnPopup, 3f);
        yield return (object) new WaitForSeconds(3f);
        GameState.Get().SetBusy(false);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(3f);
        GameState.Get().SetBusy(false);
        break;
      case 11:
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.StartOfTurnPopup, 0.0f);
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.EndOfTurnPopup, 0.0f);
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.CardPlayedPopup, 0.0f);
        NotificationManager.Get().DestroyNotification(tb04DeckBuilding.PickThreePopup, 0.0f);
        break;
    }
  }
}
