using Hearthstone.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_Carousel : MissionEntity
{
  private Vector3 popUpPos;
  private Player friendlySidePlayer;
  private float popUpScale = 1.5f;
  private int playerNum;
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[2]{ "TB_CAROUSEL_A", "TB_CAROUSEL_B" }
    }
  };

  private IEnumerator ShowArrow()
  {
    this.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    this.playerNum = this.friendlySidePlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
    WidgetInstance arrow = new WidgetInstance();
    arrow = this.playerNum != 1 ? WidgetInstance.Create("CarouselBArrows.prefab:5718b27c261c6654d887b62a406da354") : WidgetInstance.Create("CarouselAArrows.prefab:1eb2af643b42e904bb83957e95320ba6");
    while (!arrow.IsReady)
      yield return (object) null;
    arrow.transform.position = new Vector3(-12.78f, 0.67f, -10.9f);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_Carousel tbCarousel = this;
    tbCarousel.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    tbCarousel.playerNum = tbCarousel.friendlySidePlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
    while (tbCarousel.m_enemySpeaking)
      yield return (object) null;
    tbCarousel.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
    if (missionEvent == 11)
    {
      Debug.Log((object) "Reached Mission Event 11");
      yield return (object) tbCarousel.ShowArrow();
    }
    Notification popup;
    if (tbCarousel.m_popUpInfo.ContainsKey(missionEvent) && missionEvent == 10)
    {
      if (tbCarousel.playerNum == 1)
      {
        GameState.Get().SetBusy(true);
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbCarousel.popUpPos, TutorialEntity.GetTextScale() * tbCarousel.popUpScale, GameStrings.Get(tbCarousel.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(5f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        GameState.Get().SetBusy(false);
        popup = (Notification) null;
      }
      else
      {
        GameState.Get().SetBusy(true);
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbCarousel.popUpPos, TutorialEntity.GetTextScale() * tbCarousel.popUpScale, GameStrings.Get(tbCarousel.m_popUpInfo[missionEvent][1]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        GameState.Get().SetBusy(false);
        popup = (Notification) null;
      }
    }
  }

  public TB_Carousel()
    : base()
  {
  }
}
