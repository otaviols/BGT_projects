using Blizzard.T5.Core;
using System.Collections;
using UnityEngine;

public class TB_BaconHand : MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = TB_BaconHand.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = TB_BaconHand.InitStringOptions();
  private Notification m_popup;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public TB_BaconHand()
    : base()
  {
    this.m_gameOptions.AddOptions(TB_BaconHand.s_booleanOptions, TB_BaconHand.s_stringOptions);
    HistoryManager.Get().DisableHistory();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  public override bool ShouldDoAlternateMulliganIntro() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    if (missionEvent == 1)
      yield return (object) this.ShowPopup("Shop");
    else if (missionEvent == 2)
      yield return (object) this.ShowPopup("Combat");
  }

  private IEnumerator ShowPopup(string text)
  {
    TB_BaconHand tbBaconHand = this;
    float seconds = 0.0f;
    float popupDuration = 1.5f;
    float popupScale = 2f;
    Vector3 popUpPos = new Vector3(0.0f, 0.0f, 4f);
    yield return (object) new WaitForSeconds(seconds);
    tbBaconHand.m_popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPos, TutorialEntity.GetTextScale() * popupScale, text, false);
    tbBaconHand.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
    NotificationManager.Get().DestroyNotification(tbBaconHand.m_popup, popupDuration);
    tbBaconHand.DoBlur();
    yield return (object) new WaitForSeconds(popupDuration);
    tbBaconHand.EndBlur();
  }

  private void DoBlur() => this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective with
  {
    Blur = new BlurParameters(brightness: 1f)
  });

  public void EndBlur()
  {
    FullScreenFXMgr.Get();
    this.m_screenEffectsHandle.StopEffect();
  }
}
