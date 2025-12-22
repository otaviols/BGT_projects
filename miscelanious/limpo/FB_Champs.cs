using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_Champs : MissionEntity
{
  private Notification m_popup;
  public bool doPopup;
  private static readonly Dictionary<int, FB_Champs.PopupMessage> popupMsgs = new Dictionary<int, FB_Champs.PopupMessage>()
  {
    {
      1235,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_PAVEL_DRUID",
        Delay = 6f,
        Champion = "Pavel"
      }
    },
    {
      1236,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_PAVEL_MAGE",
        Delay = 6f,
        Champion = "Pavel"
      }
    },
    {
      1237,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_PAVEL_SHAMAN",
        Delay = 6f,
        Champion = "Pavel"
      }
    },
    {
      1238,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_PAVEL_ROGUE",
        Delay = 6f,
        Champion = "Pavel"
      }
    },
    {
      1239,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_PAVEL_WARRIOR",
        Delay = 6f,
        Champion = "Pavel"
      }
    },
    {
      1671,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_OSTKAKA_ROGUE",
        Delay = 6f,
        Champion = "Ostkaka"
      }
    },
    {
      1672,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_OSTKAKA_WARRIOR",
        Delay = 6f,
        Champion = "Ostkaka"
      }
    },
    {
      1673,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_OSTKAKA_MAGE",
        Delay = 6f,
        Champion = "Ostkaka"
      }
    },
    {
      1675,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_FIREBAT_ROGUE",
        Delay = 6f,
        Champion = "Firebat"
      }
    },
    {
      1676,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_FIREBAT_HUNTER",
        Delay = 6f,
        Champion = "Firebat"
      }
    },
    {
      1678,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_FIREBAT_DRUID",
        Delay = 6f,
        Champion = "Firebat"
      }
    },
    {
      1679,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_FIREBAT_WARLOCK",
        Delay = 6f,
        Champion = "Firebat"
      }
    },
    {
      2173,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_TOM60229_WARLOCK",
        Delay = 6f,
        Champion = "tom60229"
      }
    },
    {
      2174,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_TOM60229_DRUID",
        Delay = 6f,
        Champion = "tom60229"
      }
    },
    {
      2175,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_TOM60229_ROGUE",
        Delay = 6f,
        Champion = "tom60229"
      }
    },
    {
      2176,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_TOM60229_PRIEST",
        Delay = 6f,
        Champion = "tom60229"
      }
    },
    {
      2838,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_VKLIOOON_SHAMAN",
        Delay = 6f,
        Champion = "VKLiooon"
      }
    },
    {
      2839,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_VKLIOOON_HUNTER",
        Delay = 6f,
        Champion = "VKLiooon"
      }
    },
    {
      2840,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_VKLIOOON_PRIEST",
        Delay = 6f,
        Champion = "VKLiooon"
      }
    },
    {
      2841,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_VKLIOOON_DRUID",
        Delay = 6f,
        Champion = "VKLiooon"
      }
    },
    {
      2842,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_HUNTERACE_SHAMAN",
        Delay = 6f,
        Champion = "Hunterace"
      }
    },
    {
      2843,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_HUNTERACE_ROGUE",
        Delay = 6f,
        Champion = "Hunterace"
      }
    },
    {
      2844,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_HUNTERACE_MAGE",
        Delay = 6f,
        Champion = "Hunterace"
      }
    },
    {
      2845,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_HUNTERACE_WARRIOR",
        Delay = 6f,
        Champion = "Hunterace"
      }
    },
    {
      2847,
      new FB_Champs.PopupMessage()
      {
        Message = "FB_CHAMPS_MERC14",
        Delay = 6f,
        Champion = "Mercenaries 14"
      }
    }
  };

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  public override string GetNameBannerOverride(Player.Side playerSide)
  {
    int tag = GameState.Get().GetPlayerBySide(playerSide).GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
    return FB_Champs.popupMsgs[tag].Champion;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    FB_Champs fbChamps = this;
    while (fbChamps.m_enemySpeaking)
      yield return (object) null;
    fbChamps.doPopup = true;
    if (missionEvent == 10000)
      fbChamps.doPopup = false;
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
    if (fbChamps.doPopup)
      yield return (object) fbChamps.ShowPopup(GameStrings.Get(FB_Champs.popupMsgs[missionEvent].Message), FB_Champs.popupMsgs[missionEvent].Delay, popUpPos);
  }

  private IEnumerator ShowPopup(string stringID, float popupDuration, Vector3 popUpPos)
  {
    this.m_popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPos, TutorialEntity.GetTextScale() * 1.4f, GameStrings.Get(stringID), false, NotificationManager.PopupTextType.FANCY);
    NotificationManager.Get().DestroyNotification(this.m_popup, popupDuration);
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(5f);
    GameState.Get().SetBusy(false);
  }

  public FB_Champs()
    : base()
  {
  }

  public struct PopupMessage
  {
    public string Message;
    public float Delay;
    public string Champion;
  }
}
