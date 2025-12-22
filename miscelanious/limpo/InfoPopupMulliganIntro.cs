using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class InfoPopupMulliganIntro
{
  private WidgetInstance m_introPopup;
  private Spell m_introSpell;

  protected IEnumerator ShowPopup(
    string introPopupWidgetName,
    string boardBoneName,
    bool skipPopup = false)
  {
    InfoPopupMulliganIntro popupMulliganIntro = this;
    SceneMgr.Get().NotifySceneLoaded();
    while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
      yield return (object) null;
    GameMgr.Get().UpdatePresence();
    if (!skipPopup)
    {
      popupMulliganIntro.m_introPopup = WidgetInstance.Create(introPopupWidgetName);
      if (!(bool) (Object) popupMulliganIntro.m_introPopup)
      {
        yield break;
      }
      else
      {
        while (!popupMulliganIntro.m_introPopup.IsReady)
          yield return (object) null;
        Vector3 position = Board.Get().FindBone(boardBoneName).position;
        popupMulliganIntro.m_introPopup.transform.localPosition = position;
        popupMulliganIntro.m_introSpell = popupMulliganIntro.m_introPopup.GetComponentInChildren<Spell>();
        if (!(bool) (Object) popupMulliganIntro.m_introSpell)
        {
          yield break;
        }
        else
        {
          popupMulliganIntro.m_introSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(popupMulliganIntro.OnIntroSpellStateFinished));
          popupMulliganIntro.m_introSpell.ActivateState(SpellStateType.BIRTH);
          while (!popupMulliganIntro.m_introSpell.IsFinished())
            yield return (object) null;
        }
      }
    }
    Board.Get().RaiseTheLights();
    EndTurnButton.Get().RemoveInputBlocker();
    TurnStartManager.Get().BeginListeningForTurnEvents();
    MulliganManager.Get().SkipMulligan();
  }

  private void OnIntroSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (this.m_introSpell.GetActiveState() != SpellStateType.NONE)
      return;
    Object.Destroy((Object) this.m_introSpell);
    this.m_introSpell = (Spell) null;
    Object.Destroy((Object) this.m_introPopup);
    this.m_introPopup = (WidgetInstance) null;
  }
}
