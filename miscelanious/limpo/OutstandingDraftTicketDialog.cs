using System;
using UnityEngine;

public class OutstandingDraftTicketDialog : DialogBase
{
  [CustomEditField(Sections = "Object Links")]
  public UIBButton m_enterButton;
  [CustomEditField(Sections = "Object Links")]
  public UIBButton m_cancelButton;
  public UberText m_ticketCount;
  public UberText m_description;
  public GameObject m_plusSign;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_showAnimationSound = "Expand_Up.prefab:775d97ea42498c044897f396362b9db3";
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_hideAnimationSound = "Shrink_Down_Quicker.prefab:2fe963b171811ca4b8d544fa53e3330c";
  private OutstandingDraftTicketDialog.Info m_info;
  private bool m_isConfirmed;

  protected override void Awake()
  {
    base.Awake();
    this.m_enterButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.HandleDraftTicketResponse(true)));
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.HandleDraftTicketResponse(false)));
    this.m_plusSign.SetActive(false);
    this.AddHideListener(new DialogBase.HideCallback(this.OnHideComplete));
  }

  public void SetInfo(OutstandingDraftTicketDialog.Info info) => this.m_info = info;

  public override void Show()
  {
    Vector3 localScale = this.transform.localScale;
    this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    this.EnableFullScreenEffects(true);
    base.Show();
    int outstandingTicketCount = this.m_info.m_outstandingTicketCount;
    bool flag;
    if (outstandingTicketCount > 9)
    {
      this.m_ticketCount.SetText(GameStrings.Get("9"));
      flag = true;
    }
    else
    {
      this.m_ticketCount.SetText(GameStrings.Get(outstandingTicketCount.ToString()));
      flag = false;
    }
    this.m_description.Text = GameStrings.FormatPlurals("GLUE_OUTSTANDING_DRAFT_TICKET_DIALOG_DESC", new GameStrings.PluralNumber[1]
    {
      new GameStrings.PluralNumber()
      {
        m_index = 0,
        m_number = outstandingTicketCount
      }
    });
    if ((UnityEngine.Object) this.m_plusSign != (UnityEngine.Object) null)
      this.m_plusSign.SetActive(flag);
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

  private void HandleDraftTicketResponse(bool isConfirmed)
  {
    this.m_isConfirmed = isConfirmed;
    this.EnableFullScreenEffects(false);
    this.Hide();
  }

  private void OnHideComplete(DialogBase dialog, object userdata)
  {
    if (this.m_isConfirmed)
    {
      OutstandingDraftTicketDialog.Info info = this.m_info;
      if (info == null)
        return;
      Action callbackOnEnter = info.m_callbackOnEnter;
      if (callbackOnEnter == null)
        return;
      callbackOnEnter();
    }
    else
    {
      OutstandingDraftTicketDialog.Info info = this.m_info;
      if (info == null)
        return;
      Action callbackOnCancel = info.m_callbackOnCancel;
      if (callbackOnCancel == null)
        return;
      callbackOnCancel();
    }
  }

  public class Info
  {
    public Action m_callbackOnEnter;
    public Action m_callbackOnCancel;
    public int m_outstandingTicketCount;
  }
}
