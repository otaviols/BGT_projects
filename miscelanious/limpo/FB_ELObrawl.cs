using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_ELObrawl : MissionEntity
{
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[3]
      {
        "FB_ELO_FAVORED",
        "FB_ELO_UNDERDOG",
        "FB_ELO_EVEN"
      }
    }
  };
  private Player friendlySidePlayer;
  private float popUpScale = 1f;
  private Vector3 popUpPos;

  public override void PreloadAssets()
  {
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    FB_ELObrawl fbElObrawl = this;
    fbElObrawl.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    int isPlayerUnderdog = fbElObrawl.friendlySidePlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
    while (fbElObrawl.m_enemySpeaking)
      yield return (object) null;
    fbElObrawl.popUpPos = new Vector3(0.0f, 0.0f, -40f);
    Notification popup;
    if (fbElObrawl.m_popUpInfo.ContainsKey(missionEvent))
    {
      if (isPlayerUnderdog == 3)
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, fbElObrawl.popUpPos, TutorialEntity.GetTextScale() * fbElObrawl.popUpScale, GameStrings.Get(fbElObrawl.m_popUpInfo[missionEvent][2]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        popup = (Notification) null;
      }
      else if (isPlayerUnderdog == 1)
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, fbElObrawl.popUpPos, TutorialEntity.GetTextScale() * fbElObrawl.popUpScale, GameStrings.Get(fbElObrawl.m_popUpInfo[missionEvent][1]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        popup = (Notification) null;
      }
      else
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, fbElObrawl.popUpPos, TutorialEntity.GetTextScale() * fbElObrawl.popUpScale, GameStrings.Get(fbElObrawl.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        popup = (Notification) null;
      }
    }
  }

  public FB_ELObrawl()
    : base()
  {
  }
}
