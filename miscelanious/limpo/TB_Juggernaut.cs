using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_Juggernaut : MissionEntity
{
  private Notification m_popup;
  private Vector3 popUpPos;
  private string text;
  private bool doPopup;
  private int HumanHeroClass;
  private int AIHeroClass;
  private float popupDuration = 7f;
  private float popupScale = 2.5f;
  private float popupDelay;
  private static readonly Dictionary<int, string> minionMsgs = new Dictionary<int, string>()
  {
    {
      0,
      "FB_JUGGERNAUT_UNKNOWN"
    },
    {
      1,
      "FB_JUGGERNAUT_UNKNOWN"
    },
    {
      2,
      "FB_JUGGERNAUT_DRUID"
    },
    {
      3,
      "FB_JUGGERNAUT_HUNTER"
    },
    {
      4,
      "FB_JUGGERNAUT_MAGE"
    },
    {
      5,
      "FB_JUGGERNAUT_PALADIN"
    },
    {
      6,
      "FB_JUGGERNAUT_PRIEST"
    },
    {
      7,
      "FB_JUGGERNAUT_ROGUE"
    },
    {
      8,
      "FB_JUGGERNAUT_SHAMAN"
    },
    {
      9,
      "FB_JUGGERNAUT_WARLOCK"
    },
    {
      10,
      "FB_JUGGERNAUT_WARRIOR"
    }
  };

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_Juggernaut tbJuggernaut = this;
    while (tbJuggernaut.m_enemySpeaking)
      yield return (object) null;
    tbJuggernaut.doPopup = false;
    if (missionEvent == 1)
    {
      tbJuggernaut.doPopup = true;
      tbJuggernaut.popupScale = 1.85f;
      tbJuggernaut.text = GameStrings.Get("FB_JUGGERNAUT_CHOOSE_OPPONENT");
      tbJuggernaut.popupDuration = 3f;
      tbJuggernaut.popupDelay = 3f;
      tbJuggernaut.popUpPos.x = 0.0f;
      tbJuggernaut.popUpPos.z = 51f;
    }
    else
    {
      tbJuggernaut.doPopup = true;
      tbJuggernaut.AIHeroClass = missionEvent / 100;
      tbJuggernaut.HumanHeroClass = missionEvent - 100 * tbJuggernaut.AIHeroClass;
      tbJuggernaut.text = GameStrings.Get("FB_JUGGERNAUT_FIRSTLINE") + "\n" + GameStrings.Get(TB_Juggernaut.minionMsgs[tbJuggernaut.HumanHeroClass]) + " beats " + GameStrings.Get(TB_Juggernaut.minionMsgs[tbJuggernaut.AIHeroClass]);
      tbJuggernaut.popUpPos.x = 0.0f;
      tbJuggernaut.popUpPos.z = 10f;
    }
    if (tbJuggernaut.doPopup)
    {
      yield return (object) new WaitForSeconds(tbJuggernaut.popupDelay);
      tbJuggernaut.m_popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbJuggernaut.popUpPos, TutorialEntity.GetTextScale() * tbJuggernaut.popupScale, tbJuggernaut.text, false);
      NotificationManager.Get().DestroyNotification(tbJuggernaut.m_popup, tbJuggernaut.popupDuration);
      GameState.Get().SetBusy(true);
      yield return (object) new WaitForSeconds(4f);
      GameState.Get().SetBusy(false);
    }
  }

  public TB_Juggernaut()
    : base()
  {
  }
}
