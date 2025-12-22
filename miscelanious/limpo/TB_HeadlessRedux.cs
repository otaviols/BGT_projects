using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_HeadlessRedux : MissionEntity
{
  private static readonly AssetReference VO_CS2_222_Attack_02 = new AssetReference("VO_CS2_222_Attack_02.prefab:c3191e3764f78654899b70a311936b93");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_13 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_13.prefab:a015bfc61fca6a0489f276e3e2fbb0a3");
  private float popUpScale = 1f;
  private Vector3 popUpPos;
  private int _announcerLinesPlayed;
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[2]
      {
        "TB_HEADLESSREDUX_01",
        "TB_HEADLESSREDUX_03"
      }
    },
    {
      11,
      new string[2]
      {
        "TB_HEADLESSREDUX_02",
        "TB_HEADLESSREDUX_04"
      }
    }
  };
  private Player friendlySidePlayer;
  private int isPlayerHorseman;

  public override void PreloadAssets()
  {
    this.PreloadSound(TB_HeadlessRedux.VO_HeadlessHorseman_Male_Human_HallowsEve_13.ToString());
    this.PreloadSound(TB_HeadlessRedux.VO_CS2_222_Attack_02.ToString());
  }

  public override AudioSource GetAnnouncerLine(
    Card heroCard,
    Card.AnnouncerLineType type)
  {
    ++this._announcerLinesPlayed;
    switch (this._announcerLinesPlayed)
    {
      case 1:
        return this.GetPreloadedSound(TB_HeadlessRedux.VO_CS2_222_Attack_02.ToString());
      case 2:
        return this.GetPreloadedSound(TB_HeadlessRedux.VO_HeadlessHorseman_Male_Human_HallowsEve_13.ToString());
      default:
        return base.GetAnnouncerLine(heroCard, type);
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_HeadlessRedux tbHeadlessRedux = this;
    tbHeadlessRedux.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    tbHeadlessRedux.isPlayerHorseman = tbHeadlessRedux.friendlySidePlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
    while (tbHeadlessRedux.m_enemySpeaking)
      yield return (object) null;
    tbHeadlessRedux.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
    Notification popup;
    if (tbHeadlessRedux.m_popUpInfo.ContainsKey(missionEvent))
    {
      if (tbHeadlessRedux.isPlayerHorseman == 1)
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbHeadlessRedux.popUpPos, TutorialEntity.GetTextScale() * tbHeadlessRedux.popUpScale, GameStrings.Get(tbHeadlessRedux.m_popUpInfo[missionEvent][1]), false, NotificationManager.PopupTextType.FANCY);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        yield return (object) new WaitForSeconds(1f);
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbHeadlessRedux.popUpPos, TutorialEntity.GetTextScale() * tbHeadlessRedux.popUpScale, GameStrings.Get(tbHeadlessRedux.m_popUpInfo[11][1]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        GameState.Get().SetBusy(false);
        popup = (Notification) null;
      }
      else
      {
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbHeadlessRedux.popUpPos, TutorialEntity.GetTextScale() * tbHeadlessRedux.popUpScale, GameStrings.Get(tbHeadlessRedux.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        yield return (object) new WaitForSeconds(1f);
        popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbHeadlessRedux.popUpPos, TutorialEntity.GetTextScale() * tbHeadlessRedux.popUpScale, GameStrings.Get(tbHeadlessRedux.m_popUpInfo[11][0]), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) new WaitForSeconds(4f);
        NotificationManager.Get().DestroyNotification(popup, 0.0f);
        GameState.Get().SetBusy(false);
        popup = (Notification) null;
      }
    }
  }

  public TB_HeadlessRedux()
    : base()
  {
  }
}
