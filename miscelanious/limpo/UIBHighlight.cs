using UnityEngine;

[RequireComponent(typeof (PegUIElement))]
[CustomEditClass]
public class UIBHighlight : MonoBehaviour
{
  [CustomEditField(Sections = "Highlight Objects")]
  public GameObject m_MouseOverHighlight;
  [CustomEditField(Sections = "Highlight Objects")]
  public GameObject m_MouseDownHighlight;
  [CustomEditField(Sections = "Highlight Objects")]
  public GameObject m_MouseUpHighlight;
  [CustomEditField(Sections = "Highlight Sounds", T = EditType.SOUND_PREFAB)]
  public string m_MouseOverSound = "Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9";
  [CustomEditField(Sections = "Highlight Sounds", T = EditType.SOUND_PREFAB)]
  public string m_MouseOutSound;
  [CustomEditField(Sections = "Highlight Sounds", T = EditType.SOUND_PREFAB)]
  public string m_MouseDownSound = "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681";
  [CustomEditField(Sections = "Highlight Sounds", T = EditType.SOUND_PREFAB)]
  public string m_MouseUpSound;
  [CustomEditField(Sections = "Behavior Settings")]
  public bool m_SelectOnRelease;
  [CustomEditField(Sections = "Behavior Settings")]
  public bool m_HideMouseOverOnPress;
  [SerializeField]
  private bool m_AlwaysOver;
  [SerializeField]
  private bool m_EnableResponse = true;
  [Tooltip("Note: Allowing selection and allowing dragging are mutually exclusive.")]
  [CustomEditField(Label = "Enable", Sections = "Allow Selection")]
  public bool m_AllowSelection;
  [CustomEditField(Parent = "m_AllowSelection")]
  public GameObject m_SelectedHighlight;
  [CustomEditField(Parent = "m_AllowSelection")]
  public GameObject m_MouseOverSelectedHighlight;
  [Tooltip("Note: Allowing selection and allowing dragging are mutually exclusive.")]
  [CustomEditField(Label = "Enable", Sections = "Allow Dragging")]
  public bool m_AllowDragging;
  [CustomEditField(Parent = "m_AllowDragging")]
  public GameObject m_DraggingHighlight;
  [CustomEditField(Parent = "m_AllowDragging")]
  public bool m_SoundOnReleaseDrag;
  [CustomEditField(Parent = "m_AllowDragging", T = EditType.SOUND_PREFAB)]
  public string m_ReleaseDragSound;
  private bool m_Dragging;
  [CustomEditField(Sections = "Hold")]
  public bool m_HighlightOnHold;
  [CustomEditField(Parent = "m_HighlightOnHold")]
  public GameObject m_HoldHighlight;
  private bool m_Holding;

  [CustomEditField(Sections = "Behavior Settings")]
  public bool AlwaysOver
  {
    get => this.m_AlwaysOver;
    set
    {
      this.m_AlwaysOver = value;
      this.ResetState();
    }
  }

  [CustomEditField(Sections = "Behavior Settings")]
  public bool EnableResponse
  {
    get => this.m_EnableResponse;
    set
    {
      this.m_EnableResponse = value;
      this.ResetState();
    }
  }

  private void Awake()
  {
    PegUIElement component = this.gameObject.GetComponent<PegUIElement>();
    if (!((Object) component != (Object) null))
      return;
    component.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.OnRollOver()));
    component.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.OnPress(true)));
    component.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnRelease()));
    component.AddEventListener(UIEventType.RELEASEALL, (UIEvent.Handler) (e => this.OnReleaseAll()));
    component.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.OnRollOut()));
    component.AddEventListener(UIEventType.DRAG, (UIEvent.Handler) (e => this.OnDrag()));
    component.AddEventListener(UIEventType.HOLD, (UIEvent.Handler) (e => this.OnHold()));
    component.AddEventListener(UIEventType.DISABLE, (UIEvent.Handler) (e => this.OnDisable()));
    component.AddEventListener(UIEventType.ENABLE, (UIEvent.Handler) (e => this.OnEnable()));
    this.ResetState();
  }

  public void HighlightOnce() => this.OnRollOver(true);

  public void Select()
  {
    if (this.m_SelectOnRelease)
      this.OnRelease(true);
    else
      this.OnPress(true);
  }

  public void SelectNoSound()
  {
    if (this.m_SelectOnRelease)
      this.OnRelease(false);
    else
      this.OnPress(false);
  }

  public void Reset()
  {
    this.ResetState();
    this.ShowHighlightObject(this.m_SelectedHighlight, false);
    this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, false);
    this.ShowHighlightObject(this.m_DraggingHighlight, false);
    this.ShowHighlightObject(this.m_MouseOverHighlight, false);
  }

  private void ResetState()
  {
    if (this.m_AlwaysOver)
      this.OnRollOver(true);
    else
      this.OnRollOut(true);
  }

  private void OnRollOver(bool force = false)
  {
    if (!this.m_EnableResponse && !force || this.m_Dragging || this.m_Holding)
      return;
    if (!this.m_AlwaysOver)
      this.PlaySound(this.m_MouseOverSound);
    if (this.m_AllowSelection && ((Object) this.m_SelectedHighlight == (Object) null || this.m_SelectedHighlight.activeSelf))
    {
      this.ShowHighlightObject(this.m_SelectedHighlight, false);
      this.ShowHighlightObject(this.m_MouseOverHighlight, false);
      this.ShowHighlightObject(this.m_MouseUpHighlight, false);
      this.ShowHighlightObject(this.m_MouseDownHighlight, false);
      this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, true);
    }
    else
    {
      this.ShowHighlightObject(this.m_MouseDownHighlight, false);
      this.ShowHighlightObject(this.m_MouseUpHighlight, false);
      this.ShowHighlightObject(this.m_MouseOverHighlight, true);
    }
  }

  private void OnRollOut(bool force = false)
  {
    if (!this.m_EnableResponse && !force || this.m_Dragging || this.m_Holding)
      return;
    this.PlaySound(this.m_MouseOutSound);
    if (this.m_AllowSelection && ((Object) this.m_MouseOverSelectedHighlight == (Object) null || this.m_MouseOverSelectedHighlight.activeSelf))
    {
      this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, false);
      this.ShowHighlightObject(this.m_MouseOverHighlight, false);
      this.ShowHighlightObject(this.m_MouseUpHighlight, false);
      this.ShowHighlightObject(this.m_MouseDownHighlight, false);
      this.ShowHighlightObject(this.m_SelectedHighlight, true);
    }
    else
    {
      this.ShowHighlightObject(this.m_MouseDownHighlight, false);
      this.ShowHighlightObject(this.m_MouseOverHighlight, this.m_AlwaysOver);
      this.ShowHighlightObject(this.m_MouseUpHighlight, !this.m_AlwaysOver);
    }
  }

  private void OnPress() => this.OnPress(true);

  private void OnPress(bool playSound)
  {
    if (!this.m_EnableResponse)
      return;
    if (playSound)
      this.PlaySound(this.m_MouseDownSound);
    if (this.m_AllowSelection && !this.m_SelectOnRelease)
    {
      this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, false);
      this.ShowHighlightObject(this.m_MouseOverHighlight, false);
      this.ShowHighlightObject(this.m_MouseUpHighlight, false);
      this.ShowHighlightObject(this.m_MouseDownHighlight, false);
      this.ShowHighlightObject(this.m_SelectedHighlight, true);
    }
    else
    {
      this.ShowHighlightObject(this.m_MouseOverHighlight, this.m_AlwaysOver || !this.m_HideMouseOverOnPress);
      this.ShowHighlightObject(this.m_MouseUpHighlight, !this.m_AlwaysOver);
      this.ShowHighlightObject(this.m_MouseDownHighlight, true);
    }
  }

  private void OnRelease() => this.OnRelease(true);

  private void OnRelease(bool playSound)
  {
    if (!this.m_EnableResponse)
      return;
    if (this.m_AllowDragging && this.m_Dragging)
      this.ReleaseDrag();
    else if (this.m_HighlightOnHold && this.m_Holding)
    {
      this.ReleaseHold();
    }
    else
    {
      if (this.m_AllowSelection && this.m_SelectOnRelease)
      {
        this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, false);
        this.ShowHighlightObject(this.m_MouseOverHighlight, false);
        this.ShowHighlightObject(this.m_MouseUpHighlight, false);
        this.ShowHighlightObject(this.m_MouseDownHighlight, false);
        this.ShowHighlightObject(this.m_HoldHighlight, false);
        this.ShowHighlightObject(this.m_SelectedHighlight, true);
      }
      else
      {
        this.ShowHighlightObject(this.m_MouseDownHighlight, false);
        this.ShowHighlightObject(this.m_MouseUpHighlight, false);
        this.ShowHighlightObject(this.m_HoldHighlight, false);
        this.ShowHighlightObject(this.m_MouseOverHighlight, true);
      }
      if (!playSound)
        return;
      this.PlaySound(this.m_MouseUpSound);
    }
  }

  private void OnReleaseAll()
  {
    if (this.m_AllowDragging && this.m_Dragging)
    {
      this.ReleaseDrag();
    }
    else
    {
      if (!this.m_HighlightOnHold || !this.m_Holding)
        return;
      this.ReleaseHold();
    }
  }

  private void OnDrag()
  {
    if (!this.m_EnableResponse || !this.m_AllowDragging)
      return;
    if (this.m_Holding)
      this.ReleaseHold();
    this.m_Dragging = true;
    if (this.m_AlwaysOver)
      return;
    this.ShowHighlightObject(this.m_MouseDownHighlight, false);
    this.ShowHighlightObject(this.m_MouseOverHighlight, false);
    this.ShowHighlightObject(this.m_HoldHighlight, false);
    this.ShowHighlightObject(this.m_DraggingHighlight, true);
  }

  private void ReleaseDrag()
  {
    this.m_Dragging = false;
    if (this.m_SoundOnReleaseDrag)
      this.PlaySound(this.m_ReleaseDragSound);
    if (this.m_AlwaysOver)
      return;
    this.ShowHighlightObject(this.m_MouseOverHighlight, false);
    this.ShowHighlightObject(this.m_MouseUpHighlight, false);
    this.ShowHighlightObject(this.m_MouseDownHighlight, false);
    this.ShowHighlightObject(this.m_DraggingHighlight, false);
    this.ShowHighlightObject(this.m_HoldHighlight, false);
  }

  private void OnHold()
  {
    if (!this.m_EnableResponse || this.m_AlwaysOver || !this.m_HighlightOnHold)
      return;
    this.m_Holding = true;
    this.ShowHighlightObject(this.m_MouseDownHighlight, false);
    this.ShowHighlightObject(this.m_MouseOverHighlight, false);
    this.ShowHighlightObject(this.m_DraggingHighlight, false);
    this.ShowHighlightObject(this.m_HoldHighlight, true);
  }

  private void ReleaseHold()
  {
    this.m_Holding = false;
    if (this.m_AlwaysOver)
      return;
    this.ShowHighlightObject(this.m_MouseUpHighlight, false);
    this.ShowHighlightObject(this.m_MouseDownHighlight, false);
    this.ShowHighlightObject(this.m_DraggingHighlight, false);
    this.ShowHighlightObject(this.m_HoldHighlight, false);
    this.ShowHighlightObject(this.m_MouseOverHighlight, true);
  }

  private void OnDisable() => this.ResetState();

  private void OnEnable() => this.ResetState();

  private void ShowHighlightObject(GameObject obj, bool show)
  {
    if (!((Object) obj != (Object) null) || obj.activeSelf == show)
      return;
    obj.SetActive(show);
  }

  private void PlaySound(string soundFilePath)
  {
    if (SoundManager.Get() == null || string.IsNullOrEmpty(soundFilePath))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) soundFilePath);
  }
}
