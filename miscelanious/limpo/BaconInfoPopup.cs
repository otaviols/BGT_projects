using Hearthstone.UI;
using PegasusShared;
using System;
using UnityEngine;

public class BaconInfoPopup : MonoBehaviour
{
  public AsyncReference m_PlayTutorialButtonReference;

  private void Start() => this.m_PlayTutorialButtonReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPlayTutorialButtonReady));

  public void OnPlayTutorialButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "PlayTutorialButton could not be found! You will not be able to click 'Play Tutorial'!");
    else
      buttonVisualController.gameObject.GetComponent<UIBButton>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayTutorialButtonRelease));
  }

  public void PlayTutorialButtonRelease(UIEvent e)
  {
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.BattlegroundsTutorial)
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_TOOLTIP_BUTTON_BACON_HEADLINE"),
        m_text = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
    }
    else if (PartyManager.Get().IsInParty())
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_TOOLTIP_BUTTON_BACON_HEADLINE"),
        m_text = GameStrings.Get("GLUE_BACON_PARTY_TUTORIAL_DISABLED"),
        m_showAlertIcon = false,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
    }
    else
      GameMgr.Get().FindGame(GameType.GT_VS_AI, FormatType.FT_WILD, 3539);
  }
}
