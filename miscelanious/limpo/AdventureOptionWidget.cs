using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

[CustomEditClass]
public abstract class AdventureOptionWidget : MonoBehaviour
{
  private const string OPTION_INTRO_COMPLETE = "CODE_OPTION_INTRO_COMPLETE";
  private const string OPTION_OUTRO_COMPLETE = "CODE_OPTION_OUTRO_COMPLETE";
  [CustomEditField(Sections = "Widget References")]
  public AsyncReference m_widgetReference;
  [CustomEditField(Sections = "Widget References")]
  public AsyncReference m_clickableReference;
  [CustomEditField(Sections = "Visual Controller State Names")]
  public string m_ControllerIntroStateName = "PLAY_MOTE_IN";
  [CustomEditField(Sections = "Visual Controller State Names")]
  public string m_ControllerOutroStateName = "PLAY_MOTE_OUT";
  protected long m_databaseId;
  protected bool m_isEnabled = true;
  protected bool m_isVisible = true;
  protected bool m_isIntroPlaying;
  protected bool m_isOutroPlaying;
  protected bool m_isClickableInitialized;
  protected AdventureLoadoutOptionDataModel m_dataModel = new AdventureLoadoutOptionDataModel();
  protected WidgetInstance m_widgetInstance;
  protected Clickable m_clickable;
  protected AdventureOptionWidget.OptionAcknowledgedCallback m_acknowledgedCallback;
  protected Delegate m_selectedCallback;
  protected Delegate m_rolloverCallback;
  protected Delegate m_rolloutCallback;

  [CustomEditField(Sections = "Properties (Instance-Only)")]
  public bool IsNewlyUnlocked
  {
    get => this.m_dataModel.NewlyUnlocked;
    set => this.m_dataModel.NewlyUnlocked = value;
  }

  [CustomEditField(Sections = "Properties (Instance-Only)")]
  public virtual bool IsReady => (UnityEngine.Object) this.m_widgetInstance != (UnityEngine.Object) null && this.m_widgetInstance.IsReady && !this.m_widgetInstance.IsChangingStates && (UnityEngine.Object) this.m_clickable != (UnityEngine.Object) null;

  [CustomEditField(Sections = "Properties (Read-Only)")]
  public bool IsIntroPlaying => this.m_isIntroPlaying;

  [CustomEditField(Sections = "Properties (Read-Only)")]
  public bool IsOutroPlaying => this.m_isOutroPlaying;

  private void Awake() => this.m_widgetReference.RegisterReadyListener<WidgetInstance>(new Action<WidgetInstance>(this.OnWidgetInstanceReady));

  protected virtual void OnWidgetInstanceReady(WidgetInstance widgetInstance)
  {
    if ((UnityEngine.Object) widgetInstance == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Issue!", "m_widgetReference is not set in the AdventureOptionWidget.cs! Cannot initialize its properties.");
    }
    else
    {
      this.m_widgetInstance = widgetInstance;
      this.m_widgetInstance.BindDataModel((IDataModel) this.m_dataModel, false);
      this.m_widgetInstance.TriggerEvent("SetUpState", new Widget.TriggerEventParameters());
      this.m_widgetInstance.RegisterEventListener(new Widget.EventListenerDelegate(this.OnIntroSequenceComplete));
      this.m_widgetInstance.RegisterEventListener(new Widget.EventListenerDelegate(this.OnOutroSequenceComplete));
      this.SetVisible(false);
    }
  }

  protected virtual void OnClickableReady(Clickable clickable)
  {
    if ((UnityEngine.Object) clickable == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Issue!", "m_clickableReference is not set in the AdventureOptionWidget.cs! Cannot initialize its properties.");
    }
    else
    {
      this.m_clickable = clickable;
      this.SetInteractionEnabled(false);
      this.m_clickable.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.Rollover()));
      this.m_clickable.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.Rollout()));
    }
  }

  protected virtual void OnIntroFinished()
  {
    this.SetInteractionEnabled(true);
    this.m_isIntroPlaying = false;
  }

  protected virtual void OnOutroFinished()
  {
    this.SetEnabled(false);
    this.gameObject.SetActive(false);
    this.m_isOutroPlaying = false;
  }

  protected virtual void Rollover()
  {
    if (this.m_acknowledgedCallback == null)
      Log.Adventures.PrintError("Attempting to invoke callback for the OptionAcknowledgedCallback, but no callback was provided!");
    else
      this.m_acknowledgedCallback();
  }

  protected virtual void Rollout()
  {
  }

  private void OnIntroSequenceComplete(string eventName)
  {
    if (!this.m_isIntroPlaying || eventName != "CODE_OPTION_INTRO_COMPLETE")
      return;
    this.OnIntroFinished();
  }

  private void OnOutroSequenceComplete(string eventName)
  {
    if (!this.m_isOutroPlaying || eventName != "CODE_OPTION_OUTRO_COMPLETE")
      return;
    this.OnOutroFinished();
  }

  protected void InitWidget(
    string name,
    bool locked,
    string lockedText,
    bool upgraded,
    bool completed,
    bool newlyUnlocked,
    AdventureOptionWidget.OptionAcknowledgedCallback acknowledgedCallback)
  {
    this.m_dataModel.Name = name;
    this.m_dataModel.Locked = locked;
    this.m_dataModel.LockedText = lockedText;
    this.m_dataModel.Completed = completed;
    this.m_dataModel.NewlyUnlocked = newlyUnlocked;
    this.m_dataModel.IsUpgraded = upgraded;
    if ((UnityEngine.Object) this.m_widgetInstance != (UnityEngine.Object) null)
      this.m_widgetInstance.TriggerEvent("SetUpState", new Widget.TriggerEventParameters());
    this.m_acknowledgedCallback = acknowledgedCallback;
    this.Deselect();
    this.gameObject.SetActive(true);
  }

  protected void InitClickable()
  {
    if (this.m_isClickableInitialized)
      return;
    this.m_isClickableInitialized = true;
    this.m_clickableReference.RegisterReadyListener<Clickable>(new Action<Clickable>(this.OnClickableReady));
  }

  public AdventureLoadoutOptionDataModel GetDataModel() => this.m_dataModel;

  public void SetOptionCallbacks(
    Delegate selectedCallback,
    Delegate rolloverCallback = null,
    Delegate rolloutCallback = null)
  {
    this.m_selectedCallback = selectedCallback;
    this.m_rolloverCallback = rolloverCallback;
    this.m_rolloutCallback = rolloutCallback;
    this.InitClickable();
  }

  public virtual void Select()
  {
    if (this.m_dataModel == null || this.m_dataModel.Locked)
      return;
    this.m_dataModel.IsSelectedOption = true;
  }

  public void Deselect()
  {
    if (this.m_dataModel == null)
      return;
    this.m_dataModel.IsSelectedOption = false;
  }

  public virtual void PlayIntro()
  {
    this.m_isIntroPlaying = true;
    this.m_widgetInstance.TriggerEvent(this.m_ControllerIntroStateName, new Widget.TriggerEventParameters());
    this.SetVisible(true);
  }

  public virtual void PlayOutro()
  {
    this.SetInteractionEnabled(false);
    this.m_widgetInstance.TriggerEvent(this.m_ControllerOutroStateName, new Widget.TriggerEventParameters());
    this.m_isOutroPlaying = true;
  }

  public void SetEnabled(bool isEnable)
  {
    this.m_isEnabled = isEnable;
    this.SetInteractionEnabled(isEnable);
    this.SetVisible(isEnable);
  }

  public void SetInteractionEnabled(bool bEnable)
  {
    if ((UnityEngine.Object) this.m_clickable == (UnityEngine.Object) null)
      return;
    this.m_clickable.Active = bEnable;
  }

  public virtual void SetVisible(bool isVisible)
  {
    if (isVisible == this.m_isVisible)
      return;
    this.m_isVisible = isVisible;
    if (isVisible)
      this.m_widgetInstance.Show();
    else
      this.m_widgetInstance.Hide();
  }

  public delegate void OptionAcknowledgedCallback();
}
