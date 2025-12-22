using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_RagRaidScript : MissionEntity
{
  private Notification StartPopup;
  private Vector3 popUpPos;
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      14,
      new string[1]{ "FB_RAGRAID_01" }
    },
    {
      15,
      new string[1]{ "FB_LK_DEAD_02" }
    }
  };

  public override void PreloadAssets()
  {
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    if (missionEvent == 14 && this.m_popUpInfo.ContainsKey(missionEvent))
    {
      yield return (object) this.ShowPopup(this.m_popUpInfo[missionEvent][0]);
      GameState.Get().SetBusy(true);
      yield return (object) new WaitForSeconds(6f);
      GameState.Get().SetBusy(false);
      yield return (object) this.ShowPopup(this.m_popUpInfo[15][0]);
    }
  }

  private IEnumerator ShowPopup(string displayString)
  {
    this.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(displayString), false);
    NotificationManager.Get().DestroyNotification(this.StartPopup, 7f);
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(2f);
    GameState.Get().SetBusy(false);
  }

  public FB_RagRaidScript()
    : base()
  {
  }
}
