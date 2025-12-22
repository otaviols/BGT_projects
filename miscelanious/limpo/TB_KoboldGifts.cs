using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_KoboldGifts : MissionEntity
{
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[1]{ "TB_KOBOLDGIFTS_01" }
    },
    {
      11,
      new string[1]{ "TB_KOBOLDGIFTS_02" }
    }
  };
  private Player friendlySidePlayer;
  private Entity playerEntity;
  private int isPlayerHorseman;
  private float popUpScale = 1.25f;
  private Vector3 popUpPos;
  private Notification StartPopup;

  private void Start() => this.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();

  public override void PreloadAssets() => this.PreloadSound("VO_LOOT_384_Male_Kobold_Event_01.prefab:5caf2a56bda8b96418925f1f08c99f53");

  private void SetPopupPosition()
  {
    if (this.friendlySidePlayer.IsCurrentPlayer())
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.popUpPos.z = -66f;
      else
        this.popUpPos.z = -44f;
    }
    else if ((bool) UniversalInputManager.UsePhoneUI)
      this.popUpPos.z = 66f;
    else
      this.popUpPos.z = 44f;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_KoboldGifts tbKoboldGifts = this;
    tbKoboldGifts.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    while (tbKoboldGifts.m_enemySpeaking)
      yield return (object) null;
    tbKoboldGifts.popUpPos = new Vector3(0.0f, 0.0f, -40f);
    if (tbKoboldGifts.m_popUpInfo.ContainsKey(missionEvent))
    {
      Notification popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbKoboldGifts.popUpPos, TutorialEntity.GetTextScale() * tbKoboldGifts.popUpScale, GameStrings.Get(tbKoboldGifts.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
      yield return (object) new WaitForSeconds(3.5f);
      NotificationManager.Get().DestroyNotification(popup, 0.0f);
      if (missionEvent == 10)
      {
        yield return (object) new WaitForSeconds(0.15f);
        tbKoboldGifts.PlaySound("VO_LOOT_384_Male_Kobold_Event_01.prefab:5caf2a56bda8b96418925f1f08c99f53");
      }
      popup = (Notification) null;
    }
  }

  private IEnumerator ShowPopup(string displayString)
  {
    this.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.GetTextScale(), GameStrings.Get(displayString), false);
    NotificationManager.Get().DestroyNotification(this.StartPopup, 7f);
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(2f);
    GameState.Get().SetBusy(false);
  }

  public TB_KoboldGifts()
    : base()
  {
  }
}
