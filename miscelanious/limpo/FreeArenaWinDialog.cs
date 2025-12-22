using UnityEngine;

public class FreeArenaWinDialog : DialogBase
{
  [CustomEditField(Sections = "Object Links")]
  public UIBButton m_okayButton;
  public UberText m_okayButtonText;
  public UberText m_winCount;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_showAnimationSound = "Expand_Up.prefab:775d97ea42498c044897f396362b9db3";
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_hideAnimationSound = "Shrink_Down_Quicker.prefab:2fe963b171811ca4b8d544fa53e3330c";
  private FreeArenaWinDialog.Info m_info;

  protected override void Awake()
  {
    base.Awake();
    this.m_okayButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.PressOk()));
  }

  public void SetInfo(FreeArenaWinDialog.Info info)
  {
    this.m_info = info;
    if (this.m_info.m_callbackOnHide == null)
      return;
    this.AddHideListener(this.m_info.m_callbackOnHide);
  }

  public override void Show()
  {
    Vector3 localScale = this.transform.localScale;
    this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    this.EnableFullScreenEffects(true);
    base.Show();
    this.m_winCount.Text = this.m_info.m_winCount.ToString();
    if (!string.IsNullOrEmpty(this.m_showAnimationSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_showAnimationSound);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.3f, (object) "easetype", (object) iTween.EaseType.easeOutBack));
    UniversalInputManager.Get().SetSystemDialogActive(true);
  }

  protected void EnableFullScreenEffects(bool enable)
  {
    if (enable)
    {
      ScreenEffectParameters desaturatePerspective = ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 1f
      };
      DialogBase.m_screenEffectsHandle.StartEffect(desaturatePerspective);
    }
    else
      DialogBase.m_screenEffectsHandle.StopEffect();
  }

  protected override void DoHideAnimation()
  {
    if (!string.IsNullOrEmpty(this.m_hideAnimationSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_hideAnimationSound);
    base.DoHideAnimation();
  }

  private void PressOk()
  {
    this.EnableFullScreenEffects(false);
    this.Hide();
  }

  public class Info
  {
    public DialogBase.HideCallback m_callbackOnHide;
    public int m_winCount;
  }
}
