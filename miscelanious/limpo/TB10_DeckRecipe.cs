using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB10_DeckRecipe : MissionEntity
{
  private Notification DeckRecipePopup;
  private Vector3 popUpPos;
  private string textID;
  private bool doPopup;
  private bool doLeftArrow;
  private bool doUpArrow;
  private bool doDownArrow;
  private float delayTime = 2.5f;
  private float popupDuration = 7f;
  private float popupScale = 2.5f;
  private HashSet<int> seen = new HashSet<int>();
  private static readonly Dictionary<int, TB10_DeckRecipe.RecipeMessage> popupMsgs = new Dictionary<int, TB10_DeckRecipe.RecipeMessage>()
  {
    {
      939,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_DRUID",
        Delay = 7f
      }
    },
    {
      946,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_HUNTER",
        Delay = 7f
      }
    },
    {
      947,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_MAGE",
        Delay = 7f
      }
    },
    {
      938,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_PALADIN",
        Delay = 7f
      }
    },
    {
      945,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_PRIEST",
        Delay = 7f
      }
    },
    {
      944,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_ROGUE",
        Delay = 7f
      }
    },
    {
      937,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_SHAMAN",
        Delay = 7f
      }
    },
    {
      940,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_WARLOCK",
        Delay = 7f
      }
    },
    {
      936,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_OG_WARRIOR",
        Delay = 7f
      }
    },
    {
      1125,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_DRUID",
        Delay = 2.5f
      }
    },
    {
      1130,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_HUNTER",
        Delay = 2.5f
      }
    },
    {
      1131,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_MAGE",
        Delay = 2.5f
      }
    },
    {
      1124,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_PALADIN",
        Delay = 2.5f
      }
    },
    {
      1129,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_PRIEST",
        Delay = 2.5f
      }
    },
    {
      1128,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_ROGUE",
        Delay = 2.5f
      }
    },
    {
      1123,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_SHAMAN",
        Delay = 2.5f
      }
    },
    {
      1126,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_WARLOCK",
        Delay = 2.5f
      }
    },
    {
      1122,
      new TB10_DeckRecipe.RecipeMessage()
      {
        Message = "TB_DECKRECIPE_MSG_WARRIOR",
        Delay = 2.5f
      }
    }
  };

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB10_DeckRecipe tb10DeckRecipe = this;
    while (tb10DeckRecipe.m_enemySpeaking)
      yield return (object) null;
    if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
      yield return (object) null;
    if (!tb10DeckRecipe.seen.Contains(missionEvent))
    {
      tb10DeckRecipe.seen.Add(missionEvent);
      tb10DeckRecipe.doPopup = false;
      tb10DeckRecipe.doLeftArrow = false;
      tb10DeckRecipe.doUpArrow = false;
      tb10DeckRecipe.doDownArrow = false;
      if (missionEvent == 11)
      {
        NotificationManager.Get().DestroyNotification(tb10DeckRecipe.DeckRecipePopup, 0.0f);
        tb10DeckRecipe.doPopup = false;
      }
      else if (missionEvent > 900)
      {
        tb10DeckRecipe.doPopup = true;
        tb10DeckRecipe.textID = TB10_DeckRecipe.popupMsgs[missionEvent].Message;
        tb10DeckRecipe.popupDuration = TB10_DeckRecipe.popupMsgs[missionEvent].Delay;
        tb10DeckRecipe.popUpPos.x = 0.0f;
        tb10DeckRecipe.popUpPos.z = 10f;
        int num = (bool) UniversalInputManager.UsePhoneUI ? 1 : 0;
      }
      if (tb10DeckRecipe.doPopup)
      {
        yield return (object) new WaitForSeconds(tb10DeckRecipe.delayTime);
        tb10DeckRecipe.DeckRecipePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb10DeckRecipe.popUpPos, TutorialEntity.GetTextScale() * tb10DeckRecipe.popupScale, GameStrings.Get(tb10DeckRecipe.textID), false);
        if (tb10DeckRecipe.doLeftArrow)
          tb10DeckRecipe.DeckRecipePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
        if (tb10DeckRecipe.doUpArrow)
          tb10DeckRecipe.DeckRecipePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
        if (tb10DeckRecipe.doDownArrow)
          tb10DeckRecipe.DeckRecipePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
        tb10DeckRecipe.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(tb10DeckRecipe.DeckRecipePopup, tb10DeckRecipe.popupDuration);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(tb10DeckRecipe.popupDuration);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public TB10_DeckRecipe()
    : base()
  {
  }

  public struct RecipeMessage
  {
    public string Message;
    public float Delay;
  }
}
