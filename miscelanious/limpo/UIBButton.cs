using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
[RequireComponent(typeof (Collider))]
public class UIBButton : PegUIElement
{
  [CustomEditField(Sections = "Button Objects")]
  public GameObject m_RootObject;
  [CustomEditField(Sections = "Text Object")]
  public UberText m_ButtonText;
  [CustomEditField(Sections = "Click Depress Behavior")]
  public Vector3 m_ClickDownOffset = new Vector3(0.0f, -0.05f, 0.0f);
  [CustomEditField(Sections = "Click Depress Behavior")]
  public float m_RaiseTime = 0.1f;
  [CustomEditField(Sections = "Click Depress Behavior")]
  public float m_DepressTime = 0.1f;
  [CustomEditField(Sections = "Click Depress Behavior")]
  public iTween.EaseType m_DepressEaseType = iTween.EaseType.linear;
  [CustomEditField(Sections = "Click Depress Behavior")]
  public bool m_HoldDepressionOnRelease;
  [CustomEditField(Sections = "Click Depress Behavior")]
  public bool m_DepressOnPhone;
  [CustomEditField(Sections = "Roll Over Depress Behavior")]
  public bool m_DepressOnOver;
  [CustomEditField(Sections = "Wiggle Behavior")]
  public Vector3 m_WiggleAmount = new Vector3(90f, 0.0f, 0.0f);
  [CustomEditField(Sections = "Wiggle Behavior")]
  public float m_WiggleTime = 0.5f;
  [CustomEditField(Sections = "Flip Enable Behavior")]
  public Vector3 m_DisabledRotation = Vector3.zero;
  [CustomEditField(Sections = "Flip Enable Behavior")]
  public bool m_AnimateFlip;
  [CustomEditField(Sections = "Flip Enable Behavior")]
  public float m_AnimateFlipTime = 0.25f;
  [CustomEditField(Sections = "Flip Enable Behavior")]
  public bool m_WigglePostFlip;
  [CustomEditField(Sections = "Flip Enable Behavior")]
  public Vector3 m_PostFlipWiggleAmount = new Vector3(90f, 0.0f, 0.0f);
  [CustomEditField(Sections = "Flip Enable Behavior")]
  public float m_PostFlipWiggleTime = 0.5f;
  [SerializeField]
  [CustomEditField(Sections = "Events")]
  private string m_bubbleUpEvent;
  [CustomEditField(Sections = "Events")]
  public bool m_UseCustomDragTolerance;
  [CustomEditField(Sections = "Events")]
  public float m_CustomDragTolerance = 40f;
  private Vector3? m_RootObjectOriginalPosition;
  private Vector3? m_RootObjectOriginalRotation;
  private bool m_Depressed;
  private bool m_HoldingDepression;
  private Vector3 m_targetRotation;

  [Overridable]
  [CustomEditField(Sections = "Events")]
  public string BubbleUpEvent
  {
    get => this.m_bubbleUpEvent;
    set => this.m_bubbleUpEvent = value;
  }

  protected override void OnPress()
  {
    if (this.m_DepressOnOver)
      return;
    this.Depress();
  }

  protected override void OnRelease()
  {
    if (!this.m_DepressOnOver && !this.m_HoldDepressionOnRelease || this.m_HoldingDepression && this.m_HoldDepressionOnRelease)
    {
      this.Raise();
    }
    else
    {
      if (!this.m_HoldDepressionOnRelease)
        return;
      this.m_HoldingDepression = true;
    }
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    if (!this.m_Depressed || this.m_HoldingDepression)
      return;
    this.Raise();
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if (this.m_DepressOnOver)
      this.Depress();
    this.Wiggle();
  }

  public void Select() => this.Depress();

  public void Deselect() => this.Raise();

  public void Flip(bool faceUp, bool forceImmediate = false)
  {
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null)
      return;
    this.InitOriginalRotation();
    this.m_targetRotation = faceUp ? this.m_RootObjectOriginalRotation.Value : this.m_RootObjectOriginalRotation.Value + this.m_DisabledRotation;
    iTween.StopByName(this.m_RootObject, "flip");
    if (this.m_AnimateFlip && !forceImmediate)
    {
      iTween.RotateAdd(this.m_RootObject, iTween.Hash((object) "amount", (object) (faceUp ? -this.m_DisabledRotation : this.m_DisabledRotation), (object) "time", (object) this.m_AnimateFlipTime, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true, (object) "name", (object) "flip", (object) "oncomplete", (object) (Action<object>) (o => this.m_RootObject.transform.localEulerAngles = this.m_targetRotation)));
      if (!this.m_WigglePostFlip)
        return;
      this.Wiggle(this.m_PostFlipWiggleAmount, this.m_targetRotation, this.m_PostFlipWiggleTime, this.m_AnimateFlipTime);
    }
    else
      this.m_RootObject.transform.localEulerAngles = this.m_targetRotation;
  }

  public void SetText(string text)
  {
    if (!((UnityEngine.Object) this.m_ButtonText != (UnityEngine.Object) null))
      return;
    this.m_ButtonText.Text = text;
  }

  public string GetText() => !this.m_ButtonText.GameStringLookup ? this.m_ButtonText.Text : GameStrings.Get(this.m_ButtonText.Text);

  public bool IsHoldingDepression() => this.m_HoldingDepression;

  private void Raise()
  {
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null || !this.m_Depressed)
      return;
    this.m_Depressed = false;
    this.m_HoldingDepression = false;
    iTween.StopByName(this.m_RootObject, "depress");
    if ((double) this.m_RaiseTime > 0.0)
      iTween.MoveTo(this.m_RootObject, iTween.Hash((object) "position", (object) this.m_RootObjectOriginalPosition, (object) "time", (object) this.m_RaiseTime, (object) "easeType", (object) this.m_DepressEaseType, (object) "isLocal", (object) true, (object) "name", (object) "depress"));
    else
      this.m_RootObject.transform.localPosition = this.m_RootObjectOriginalPosition.Value;
  }

  private void Depress()
  {
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null || this.m_Depressed || (bool) UniversalInputManager.UsePhoneUI && !this.m_DepressOnPhone)
      return;
    this.InitOriginalPosition();
    this.m_Depressed = true;
    iTween.StopByName(this.m_RootObject, "depress");
    Vector3 vector3 = this.m_RootObjectOriginalPosition.Value + this.m_ClickDownOffset;
    if ((double) this.m_DepressTime > 0.0)
      iTween.MoveTo(this.m_RootObject, iTween.Hash((object) "position", (object) vector3, (object) "time", (object) this.m_DepressTime, (object) "easeType", (object) this.m_DepressEaseType, (object) "isLocal", (object) true, (object) "name", (object) "depress"));
    else
      this.m_RootObject.transform.localPosition = vector3;
  }

  private void Wiggle()
  {
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null || (bool) UniversalInputManager.UsePhoneUI)
      return;
    this.InitOriginalRotation();
    this.Wiggle(this.m_WiggleAmount, this.m_RootObjectOriginalRotation.Value, this.m_WiggleTime, 0.0f);
  }

  private void Wiggle(Vector3 amount, Vector3 originalRotation, float time, float delay)
  {
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null || (double) amount.sqrMagnitude == 0.0 || (double) time <= 0.0)
      return;
    this.InitOriginalRotation();
    if (iTween.CountByName(this.m_RootObject, "wiggle") > 0)
    {
      iTween.StopByName(this.m_RootObject, "wiggle");
      this.m_RootObject.transform.localEulerAngles = this.m_targetRotation;
    }
    Hashtable tweenHashTable = iTweenManager.Get().GetTweenHashTable();
    tweenHashTable.Add((object) nameof (amount), (object) amount);
    tweenHashTable.Add((object) nameof (time), (object) time);
    tweenHashTable.Add((object) nameof (delay), (object) delay);
    tweenHashTable.Add((object) "name", (object) "wiggle");
    tweenHashTable.Add((object) "onstart", (object) (Action<object>) (o => this.m_RootObject.transform.localEulerAngles = this.m_targetRotation));
    tweenHashTable.Add((object) "oncomplete", (object) (Action<object>) (o => this.m_RootObject.transform.localEulerAngles = this.m_targetRotation));
    iTween.PunchRotation(this.m_RootObject, tweenHashTable, false);
  }

  private void InitOriginalRotation()
  {
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null || this.m_RootObjectOriginalRotation.HasValue)
      return;
    this.m_RootObjectOriginalRotation = new Vector3?(this.m_RootObject.transform.localEulerAngles);
  }

  private void InitOriginalPosition()
  {
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null || this.m_RootObjectOriginalPosition.HasValue)
      return;
    this.m_RootObjectOriginalPosition = new Vector3?(this.m_RootObject.transform.localPosition);
  }

  private void OnDisable()
  {
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null)
      return;
    iTween.StopByName(this.m_RootObject, "wiggle");
    this.m_RootObject.transform.localEulerAngles = this.m_targetRotation;
  }

  protected override void Awake()
  {
    base.Awake();
    if ((UnityEngine.Object) this.m_RootObject == (UnityEngine.Object) null)
      return;
    if (this.m_UseCustomDragTolerance)
      this.SetDragTolerance(this.m_CustomDragTolerance);
    this.m_targetRotation = this.m_RootObject.transform.localEulerAngles;
  }

  protected override void OnTap()
  {
    base.OnTap();
    if (string.IsNullOrEmpty(this.m_bubbleUpEvent))
      return;
    SendEventUpwardStateAction.SendEventUpward(this.gameObject, this.m_bubbleUpEvent);
  }
}
