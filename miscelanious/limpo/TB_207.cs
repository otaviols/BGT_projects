using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_207 : MissionEntity
{
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[2]{ "TB_207_INTRO_02", "TB_207_INTRO_01" }
    },
    {
      11,
      new string[1]{ "TB_207_INTRO_01" }
    },
    {
      1,
      new string[10]
      {
        "",
        "TB_207_01",
        "TB_207_02",
        "TB_207_03",
        "TB_207_04",
        "TB_207_05",
        "TB_207_06",
        "TB_207_07",
        "TB_207_08",
        "TB_207_09"
      }
    }
  };
  private Player friendlySidePlayer;
  private Player opposingSidePlayer;
  private float popUpScale = 1.25f;
  private Vector3 popUpPos;
  private int brawl;
  private int yourBrawl;

  public override void PreloadAssets()
  {
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_207 tb207 = this;
    tb207.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    tb207.opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    tb207.brawl = tb207.opposingSidePlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
    tb207.yourBrawl = tb207.friendlySidePlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
    Debug.Log((object) ("Brawl # " + (object) tb207.brawl));
    while (tb207.m_enemySpeaking)
      yield return (object) null;
    tb207.popUpPos = new Vector3(0.0f, 0.0f, -40f);
    if (tb207.m_popUpInfo.ContainsKey(missionEvent))
    {
      Notification popup;
      if (missionEvent == 10)
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb207.popUpPos, TutorialEntity.GetTextScale() * tb207.popUpScale, GameStrings.Get(tb207.m_popUpInfo[missionEvent][1]) + "\n" + GameStrings.Get(tb207.m_popUpInfo[1][tb207.yourBrawl]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(3.5f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        yield return (object) new WaitForSeconds(0.5f);
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb207.popUpPos, TutorialEntity.GetTextScale() * tb207.popUpScale, GameStrings.Get(tb207.m_popUpInfo[missionEvent][0]) + "\n" + GameStrings.Get(tb207.m_popUpInfo[1][tb207.brawl]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(3.5f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        popup = (Notification) null;
      }
      if (missionEvent == 11)
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb207.popUpPos, TutorialEntity.GetTextScale() * tb207.popUpScale, GameStrings.Get(tb207.m_popUpInfo[missionEvent][0]) + "\n" + GameStrings.Get(tb207.m_popUpInfo[1][tb207.yourBrawl]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        popup = (Notification) null;
      }
    }
  }

  public TB_207()
    : base()
  {
  }
}
