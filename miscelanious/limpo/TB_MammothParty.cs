using System.Collections;
using UnityEngine;

public class TB_MammothParty : MissionEntity
{
  private Notification StartPopup;
  private bool hasCrasherBeenDiscarded;
  private string textID10 = "TB_MP_COOP_TEXT_START";
  private string textID11 = "TB_MP_COOP_FIRST_CRASHER";
  private string textID12 = "TB_MP_COOP_PINATA";
  private string textID13 = "TB_MP_COOP_CRASHER_DISCARD";
  private string textID14 = "TB_MP_COOP_ENDING";
  private string textID15 = "TB_MP_COOP_1STSPELL";
  private string textID16 = "TB_MP_COOP_2NDSPELL";
  private Vector3 popUpPos;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_MammothParty tbMammothParty = this;
    while (tbMammothParty.m_enemySpeaking)
      yield return (object) null;
    tbMammothParty.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
    switch (missionEvent)
    {
      case 10:
        tbMammothParty.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMammothParty.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(tbMammothParty.textID10), false);
        NotificationManager.Get().DestroyNotification(tbMammothParty.StartPopup, 7f);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        break;
      case 11:
        tbMammothParty.popUpPos.z = !(bool) UniversalInputManager.UsePhoneUI ? -44f : -66f;
        tbMammothParty.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMammothParty.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(tbMammothParty.textID11), false);
        NotificationManager.Get().DestroyNotification(tbMammothParty.StartPopup, 7f);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        break;
      case 12:
        tbMammothParty.popUpPos.z = !(bool) UniversalInputManager.UsePhoneUI ? -44f : -66f;
        tbMammothParty.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMammothParty.popUpPos, TutorialEntity.GetTextScale() * 2f, GameStrings.Get(tbMammothParty.textID12), false);
        NotificationManager.Get().DestroyNotification(tbMammothParty.StartPopup, 5f);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(6.5f);
        GameState.Get().SetBusy(false);
        break;
      case 13:
        if (tbMammothParty.hasCrasherBeenDiscarded)
          break;
        tbMammothParty.hasCrasherBeenDiscarded = true;
        tbMammothParty.popUpPos.z = !(bool) UniversalInputManager.UsePhoneUI ? -44f : -66f;
        tbMammothParty.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMammothParty.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(tbMammothParty.textID13), false);
        NotificationManager.Get().DestroyNotification(tbMammothParty.StartPopup, 7f);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        break;
      case 14:
        tbMammothParty.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMammothParty.popUpPos, TutorialEntity.GetTextScale() * 2f, GameStrings.Get(tbMammothParty.textID14), false);
        NotificationManager.Get().DestroyNotification(tbMammothParty.StartPopup, 7f);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(7f);
        GameState.Get().SetBusy(false);
        break;
      case 15:
        tbMammothParty.popUpPos.z = !(bool) UniversalInputManager.UsePhoneUI ? -44f : -66f;
        tbMammothParty.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMammothParty.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(tbMammothParty.textID15), false);
        NotificationManager.Get().DestroyNotification(tbMammothParty.StartPopup, 7f);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        break;
      case 16:
        tbMammothParty.popUpPos.z = !(bool) UniversalInputManager.UsePhoneUI ? -44f : -66f;
        tbMammothParty.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tbMammothParty.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(tbMammothParty.textID16), false);
        NotificationManager.Get().DestroyNotification(tbMammothParty.StartPopup, 7f);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        break;
    }
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public TB_MammothParty()
    : base()
  {
  }
}
