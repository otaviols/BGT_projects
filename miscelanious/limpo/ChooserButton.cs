using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public abstract class ChooserButton : AdventureGenericButton
{
  private const string s_EventButtonExpand = "Expand";
  private const string s_EventButtonContract = "Contract";
  [SerializeField]
  [CustomEditField(Sections = "Button State Table")]
  public StateEventTable m_ButtonStateTable;
  [SerializeField]
  private float m_ButtonBottomPadding;
  [SerializeField]
  [CustomEditField(Sections = "Sub Button Settings")]
  public GameObject m_SubButtonContainer;
  [SerializeField]
  private float m_SubButtonHeight = 3.75f;
  [SerializeField]
  private float m_SubButtonContainerBtmPadding = 0.1f;
  [CustomEditField(Sections = "Sub Button Settings")]
  [SerializeField]
  public iTween.EaseType m_ActivateEaseType = iTween.EaseType.easeOutBounce;
  [CustomEditField(Sections = "Sub Button Settings")]
  [SerializeField]
  public iTween.EaseType m_DeactivateEaseType = iTween.EaseType.easeOutSine;
  [CustomEditField(Sections = "Sub Button Settings")]
  [SerializeField]
  public float m_SubButtonVisibilityPadding = 5f;
  [CustomEditField(Sections = "Sub Button Settings")]
  [SerializeField]
  public float m_SubButtonAnimationTime = 0.25f;
  [SerializeField]
  [CustomEditField(Sections = "Sub Button Settings")]
  public float m_SubButtonShowPosZ;
  private bool m_Toggled;
  private bool m_SelectSubButtonOnToggle;
  private Vector3 m_MainButtonExtents = Vector3.zero;
  protected List<ChooserSubButton> m_SubButtons = new List<ChooserSubButton>();
  protected List<ChooserButton.VisualUpdated> m_VisualUpdatedEventList = new List<ChooserButton.VisualUpdated>();
  protected List<ChooserButton.Toggled> m_ToggleEventList = new List<ChooserButton.Toggled>();
  protected List<ChooserButton.ModeSelection> m_ModeSelectionEventList = new List<ChooserButton.ModeSelection>();
  protected List<ChooserButton.Expanded> m_ExpandedEventList = new List<ChooserButton.Expanded>();
  protected ChooserSubButton m_LastSelectedSubButton;

  [CustomEditField(Sections = "Button Settings")]
  public float ButtonBottomPadding
  {
    get => this.m_ButtonBottomPadding;
    set
    {
      this.m_ButtonBottomPadding = value;
      this.UpdateButtonPositions();
    }
  }

  [CustomEditField(Sections = "Sub Button Settings")]
  public float SubButtonHeight
  {
    get => this.m_SubButtonHeight;
    set
    {
      this.m_SubButtonHeight = value;
      this.UpdateButtonPositions();
    }
  }

  [CustomEditField(Sections = "Sub Button Settings")]
  public float SubButtonContainerBtmPadding
  {
    get => this.m_SubButtonContainerBtmPadding;
    set
    {
      this.m_SubButtonContainerBtmPadding = value;
      this.UpdateButtonPositions();
    }
  }

  [CustomEditField(Sections = "Button Settings")]
  public bool Toggle
  {
    get => this.m_Toggled;
    set => this.ToggleButton(value);
  }

  protected override void Awake()
  {
    base.Awake();
    this.m_SubButtonContainer.SetActive(this.Toggle);
    this.m_SubButtonContainer.transform.localPosition = this.GetHiddenPosition();
    if ((UnityEngine.Object) this.m_PortraitRenderer != (UnityEngine.Object) null)
      this.m_MainButtonExtents = this.m_PortraitRenderer.bounds.extents;
    this.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ReleaseListener));
  }

  public void RemoveReleaseHandler() => this.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ReleaseListener));

  public ChooserSubButton[] GetSubButtons() => this.m_SubButtons.ToArray();

  public void SetSelectSubButtonOnToggle(bool flag) => this.m_SelectSubButtonOnToggle = flag;

  public void UpdateButtonPositions()
  {
    float subButtonHeight = this.m_SubButtonHeight;
    for (int index = 1; index < this.m_SubButtons.Count; ++index)
      TransformUtil.SetLocalPosZ((Component) this.m_SubButtons[index], subButtonHeight * (float) index);
  }

  public void AddVisualUpdatedListener(ChooserButton.VisualUpdated dlg) => this.m_VisualUpdatedEventList.Add(dlg);

  public void AddToggleListener(ChooserButton.Toggled dlg) => this.m_ToggleEventList.Add(dlg);

  public void AddModeSelectionListener(ChooserButton.ModeSelection dlg) => this.m_ModeSelectionEventList.Add(dlg);

  public void AddExpandedListener(ChooserButton.Expanded dlg) => this.m_ExpandedEventList.Add(dlg);

  public float GetFullButtonHeight()
  {
    if ((UnityEngine.Object) this.m_PortraitRenderer == (UnityEngine.Object) null || (UnityEngine.Object) this.m_SubButtonContainer == (UnityEngine.Object) null)
      return TransformUtil.GetBoundsOfChildren(this.gameObject).size.z;
    float val2 = this.m_SubButtonContainer.transform.localPosition.z + this.m_SubButtonHeight * (float) this.m_SubButtons.Count + this.m_SubButtonContainerBtmPadding;
    float num = this.m_PortraitRenderer.transform.localPosition.z - this.m_MainButtonExtents.z;
    return Math.Max(this.m_PortraitRenderer.transform.localPosition.z + this.m_MainButtonExtents.z, val2) - num - this.m_ButtonBottomPadding;
  }

  public void DisableSubButtonHighlights()
  {
    foreach (ChooserSubButton subButton in this.m_SubButtons)
      subButton.SetHighlight(false);
  }

  public bool ContainsSubButton(ChooserSubButton btn) => this.m_SubButtons.Exists((Predicate<ChooserSubButton>) (x => (UnityEngine.Object) x == (UnityEngine.Object) btn));

  public void ToggleButton(bool toggle)
  {
    if (toggle == this.m_Toggled)
      return;
    this.m_Toggled = toggle;
    this.m_ButtonStateTable.CancelQueuedStates();
    this.m_ButtonStateTable.TriggerState(this.Toggle ? "Expand" : "Contract");
    if (this.Toggle)
      this.m_SubButtonContainer.SetActive(true);
    Vector3 hiddenPosition = this.GetHiddenPosition();
    Vector3 showPosition = this.GetShowPosition();
    Vector3 curr = this.Toggle ? hiddenPosition : showPosition;
    Vector3 vector3 = this.Toggle ? showPosition : hiddenPosition;
    this.m_SubButtonContainer.transform.localPosition = curr;
    this.UpdateSubButtonsVisibility(curr, this.m_SubButtonShowPosZ);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "islocal", (object) true, (object) "from", (object) curr, (object) "to", (object) vector3, (object) "time", (object) this.m_SubButtonAnimationTime, (object) "easeType", (object) (iTween.EaseType) (this.Toggle ? (int) this.m_ActivateEaseType : (int) this.m_DeactivateEaseType), (object) "oncomplete", (object) "OnExpandAnimationComplete", (object) "oncompletetarget", (object) this.gameObject, (object) "onupdate", (object) (Action<object>) (newVal => this.OnButtonAnimating((Vector3) newVal, this.m_SubButtonShowPosZ)), (object) "onupdatetarget", (object) this.gameObject));
    this.FireToggleEvent();
    if (!this.Toggle || !this.m_SelectSubButtonOnToggle || !((UnityEngine.Object) this.m_LastSelectedSubButton != (UnityEngine.Object) null))
      return;
    this.OnSubButtonClicked(this.m_LastSelectedSubButton);
  }

  protected ChooserSubButton CreateSubButton(
    string subButtonPrefab,
    bool useAsLastSelected)
  {
    if ((UnityEngine.Object) this.m_SubButtonContainer == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "m_SubButtonContainer cannot be null. Unable to create subbutton.", (UnityEngine.Object) this);
      return (ChooserSubButton) null;
    }
    ChooserSubButton newSubButton = GameUtils.LoadGameObjectWithComponent<ChooserSubButton>(subButtonPrefab);
    if ((UnityEngine.Object) newSubButton == (UnityEngine.Object) null)
      return (ChooserSubButton) null;
    GameUtils.SetParent((Component) newSubButton, this.m_SubButtonContainer);
    if (useAsLastSelected || (UnityEngine.Object) this.m_LastSelectedSubButton == (UnityEngine.Object) null)
      this.m_LastSelectedSubButton = newSubButton;
    newSubButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnSubButtonClicked(newSubButton)));
    this.m_SubButtons.Add(newSubButton);
    this.UpdateButtonPositions();
    this.m_SubButtonContainer.transform.localPosition = this.GetHiddenPosition();
    return newSubButton;
  }

  protected Vector3 GetHiddenPosition()
  {
    Vector3 localPosition = this.m_SubButtonContainer.transform.localPosition;
    return new Vector3(localPosition.x, localPosition.y, this.m_SubButtonShowPosZ - this.m_SubButtonHeight * (float) this.m_SubButtons.Count - this.m_SubButtonContainerBtmPadding);
  }

  private Vector3 GetShowPosition()
  {
    Vector3 localPosition = this.m_SubButtonContainer.transform.localPosition;
    return new Vector3(localPosition.x, localPosition.y, this.m_SubButtonShowPosZ);
  }

  private void OnButtonAnimating(Vector3 curr, float zposshowlimit)
  {
    this.m_SubButtonContainer.transform.localPosition = curr;
    this.UpdateSubButtonsVisibility(curr, zposshowlimit);
    this.FireVisualUpdatedEvent();
  }

  private void UpdateSubButtonsVisibility(Vector3 curr, float zposshowlimit)
  {
    float subButtonHeight = this.m_SubButtonHeight;
    for (int index = 0; index < this.m_SubButtons.Count; ++index)
    {
      float num = subButtonHeight * (float) (index + 1) + curr.z;
      bool flag = (double) zposshowlimit - (double) num <= (double) this.m_SubButtonVisibilityPadding;
      GameObject gameObject = this.m_SubButtons[index].gameObject;
      if (gameObject.activeSelf != flag)
        gameObject.SetActive(flag);
    }
  }

  private void OnExpandAnimationComplete()
  {
    if (this.m_SubButtonContainer.activeSelf != this.m_Toggled)
      this.m_SubButtonContainer.SetActive(this.Toggle);
    this.FireExpandedEvent(this.Toggle);
  }

  public void FireVisualUpdatedEvent()
  {
    foreach (ChooserButton.VisualUpdated visualUpdated in this.m_VisualUpdatedEventList.ToArray())
      visualUpdated();
  }

  private void FireToggleEvent()
  {
    foreach (ChooserButton.Toggled toggled in this.m_ToggleEventList.ToArray())
      toggled(this.Toggle);
  }

  private void FireModeSelectedEvent(ChooserSubButton btn)
  {
    foreach (ChooserButton.ModeSelection modeSelection in this.m_ModeSelectionEventList.ToArray())
      modeSelection(btn);
  }

  private void FireExpandedEvent(bool expand)
  {
    foreach (ChooserButton.Expanded expanded in this.m_ExpandedEventList.ToArray())
      expanded(this, expand);
  }

  private void ReleaseListener(UIEvent e) => this.ToggleButton(!this.Toggle);

  protected void OnSubButtonClicked(ChooserSubButton btn)
  {
    this.m_LastSelectedSubButton = btn;
    this.FireModeSelectedEvent(btn);
    foreach (ChooserSubButton subButton in this.m_SubButtons)
      subButton.SetHighlight((UnityEngine.Object) subButton == (UnityEngine.Object) btn);
  }

  public delegate void VisualUpdated();

  public delegate void Toggled(bool toggle);

  public delegate void ModeSelection(ChooserSubButton btn);

  public delegate void Expanded(ChooserButton button, bool expand);
}
