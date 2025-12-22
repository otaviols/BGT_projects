using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_Marin : MissionEntity
{
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[1]{ "TB_MARIN_QUEST" }
    }
  };
  private float popUpScale = 1f;
  private Vector3 popUpPos;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_Marin tbMarin = this;
    while (tbMarin.m_enemySpeaking)
      yield return (object) null;
    tbMarin.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
    if (tbMarin.m_popUpInfo.ContainsKey(missionEvent))
    {
      Notification popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMarin.popUpPos, TutorialEntity.GetTextScale() * tbMarin.popUpScale, GameStrings.Get(tbMarin.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
      yield return (object) new WaitForSeconds(4f);
      NotificationManager.Get().DestroyNotification(popup, 0.0f);
      popup = (Notification) null;
    }
  }

  public TB_Marin()
    : base()
  {
  }
}
