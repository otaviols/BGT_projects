using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_BuildABrawl : MissionEntity
{
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[1]{ "FB_BUILDABRAWL_INTRO" }
    },
    {
      11,
      new string[1]{ "FB_BUILDABRAWL_NOTSTARTED" }
    },
    {
      1,
      new string[17]
      {
        "",
        "FB_BUILDABRAWL_001",
        "FB_BUILDABRAWL_002",
        "FB_BUILDABRAWL_003",
        "FB_BUILDABRAWL_004",
        "FB_BUILDABRAWL_005",
        "FB_BUILDABRAWL_006",
        "FB_BUILDABRAWL_007",
        "FB_BUILDABRAWL_008",
        "FB_BUILDABRAWL_009",
        "FB_BUILDABRAWL_010",
        "FB_BUILDABRAWL_011",
        "FB_BUILDABRAWL_012",
        "FB_BUILDABRAWL_013",
        "FB_BUILDABRAWL_014",
        "FB_BUILDABRAWL_015",
        "FB_BUILDABRAWL_016"
      }
    }
  };
  private Player friendlySidePlayer;
  private float popUpScale = 1.25f;
  private Vector3 popUpPos;
  private int brawl;

  public override void PreloadAssets()
  {
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    FB_BuildABrawl fbBuildAbrawl = this;
    fbBuildAbrawl.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    fbBuildAbrawl.brawl = fbBuildAbrawl.friendlySidePlayer.GetTag(GAME_TAG.SCORE_VALUE_3);
    Debug.Log((object) ("Brawl # " + (object) fbBuildAbrawl.brawl));
    while (fbBuildAbrawl.m_enemySpeaking)
      yield return (object) null;
    fbBuildAbrawl.popUpPos = new Vector3(0.0f, 0.0f, -40f);
    if (fbBuildAbrawl.m_popUpInfo.ContainsKey(missionEvent))
    {
      Notification popup;
      if (missionEvent == 10)
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, fbBuildAbrawl.popUpPos, TutorialEntity.GetTextScale() * fbBuildAbrawl.popUpScale, GameStrings.Get(fbBuildAbrawl.m_popUpInfo[missionEvent][0]) + "\n" + GameStrings.Get(fbBuildAbrawl.m_popUpInfo[1][fbBuildAbrawl.brawl]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        popup = (Notification) null;
      }
      if (missionEvent == 11)
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, fbBuildAbrawl.popUpPos, TutorialEntity.GetTextScale() * fbBuildAbrawl.popUpScale, GameStrings.Get(fbBuildAbrawl.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        popup = (Notification) null;
      }
    }
  }

  public FB_BuildABrawl()
    : base()
  {
  }
}
