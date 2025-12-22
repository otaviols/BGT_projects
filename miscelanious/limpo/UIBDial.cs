using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBDial : PegUIElement
{
  [CustomEditField(Sections = "Arrows")]
  public UIBButton m_TopArrow;
  [CustomEditField(Sections = "Arrows")]
  public UIBButton m_BottomArrow;
  [CustomEditField(Sections = "Arrows")]
  public MeshRenderer m_TopArrowMesh;
  [CustomEditField(Sections = "Arrows")]
  public MeshRenderer m_BottomArrowMesh;
  [CustomEditField(Sections = "Arrows")]
  public Material m_EnabledArrowButtonMaterial;
  [CustomEditField(Sections = "Arrows")]
  public Material m_DisabledArrowButtonMaterial;
  [CustomEditField(Sections = "Dial")]
  public GameObject m_Dial;
  [CustomEditField(Sections = "Dial")]
  public List<DialItem> m_DialItems = new List<DialItem>();
  [CustomEditField(Sections = "Dial")]
  public float m_AnimateSpinTime = 0.25f;
  [CustomEditField(Sections = "Dial")]
  public int m_StartingSelectedIndex;
  [CustomEditField(Sections = "Dial")]
  public int m_NumItemsShowingAbove = 5;
  [CustomEditField(Sections = "Dial")]
  public int m_NumItemsShowingBelow = 5;
  [CustomEditField(Sections = "Dial")]
  public float m_StartingHoldRollFrequency = 0.15f;
  [CustomEditField(Sections = "Dial")]
  public float m_MaxHoldRollFrequency = 0.075f;
  [CustomEditField(Sections = "Dial")]
  public float m_MaxFrequencyThreshold = 2f;
  [CustomEditField(Sections = "Dial")]
  public bool m_EnableMaxFrequency;
  [CustomEditField(Sections = "Tooltip")]
  public bool m_EnableTooltip = true;
  [CustomEditField(Sections = "Tooltip")]
  public GameObject m_TooltipPrefab;
  [CustomEditField(Sections = "Tooltip")]
  public Transform m_TooltipBone;
  private UIBDial.ItemChosenCallback m_itemChosenCallback = (UIBDial.ItemChosenCallback) ((_param1, _param2) => { });
  private UIBDial.ItemTextCallback m_itemTextCallback = new UIBDial.ItemTextCallback(UIBDial.DefaultItemTextCallback);
  private DialItem m_selectedItem;
  private List<object> m_items = new List<object>();
  private int m_itemsSelectedIndex;
  private int m_itemsFirstVisibleIndex;
  private int m_itemsLastVisibleIndex;
  private int m_dialItemsSelectedIndex;
  private int m_dialItemsFirstIndex;
  private int m_dialItemsLastIndex;
  private TooltipPanel m_tooltip;
  private string m_tooltipHeaderText;
  private string m_tooltipDescText;
  private bool m_topArrowHeld;
  private bool m_bottomArrowHeld;
  private bool m_mouseOver;
  private float m_rollRotationAmount;
  private Vector3 m_upRotationEuler;
  private Vector3 m_downRotationEuler;
  private Quaternion m_upRotationQuat;
  private Quaternion m_downRotationQuat;
  private Quaternion m_desiredOrientation = Quaternion.identity;
  private float m_currentHoldRollFrequency;
  private float m_holdPeriod;
  private float m_totalHoldDuration;
  private const string ITWEEN_SPIN_NAME = "spin";

  protected override void Awake()
  {
    this.m_dialItemsSelectedIndex = this.m_StartingSelectedIndex;
    this.m_dialItemsFirstIndex = this.m_dialItemsSelectedIndex - this.m_NumItemsShowingAbove;
    this.m_dialItemsLastIndex = this.m_dialItemsSelectedIndex + this.m_NumItemsShowingBelow;
    base.Awake();
  }

  public void Start()
  {
    if ((UnityEngine.Object) this.m_TopArrow != (UnityEngine.Object) null)
    {
      this.m_TopArrow.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTopArrowReleased));
      this.m_TopArrow.AddEventListener(UIEventType.HOLD, new UIEvent.Handler(this.OnTopArrowHeld));
      this.m_TopArrow.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnArrowOver));
      this.m_TopArrow.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnArrowOut));
      this.m_TopArrow.gameObject.SetActive(false);
    }
    if ((UnityEngine.Object) this.m_BottomArrow != (UnityEngine.Object) null)
    {
      this.m_BottomArrow.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBottomArrowReleased));
      this.m_BottomArrow.AddEventListener(UIEventType.HOLD, new UIEvent.Handler(this.OnBottomArrowHeld));
      this.m_BottomArrow.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnArrowOver));
      this.m_BottomArrow.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnArrowOut));
      this.m_BottomArrow.gameObject.SetActive(false);
    }
    this.m_rollRotationAmount = 360f / (float) this.m_DialItems.Count;
    this.m_upRotationEuler = this.m_rollRotationAmount * Vector3.right;
    this.m_downRotationEuler = this.m_rollRotationAmount * Vector3.left;
    this.m_desiredOrientation = this.m_Dial.transform.localRotation;
    this.m_upRotationQuat = Quaternion.AngleAxis(this.m_rollRotationAmount, Vector3.right);
    this.m_downRotationQuat = Quaternion.AngleAxis(this.m_rollRotationAmount, Vector3.left);
    this.m_currentHoldRollFrequency = this.m_StartingHoldRollFrequency;
  }

  private void Update()
  {
    if (!UniversalInputManager.Get().IsTouchMode() && !this.m_topArrowHeld && !this.m_bottomArrowHeld)
      this.UpdateMouseInput();
    if ((double) this.m_totalHoldDuration >= (double) this.m_MaxFrequencyThreshold && this.m_EnableMaxFrequency)
      this.m_currentHoldRollFrequency = this.m_MaxHoldRollFrequency;
    if ((double) this.m_holdPeriod > (double) this.m_currentHoldRollFrequency)
    {
      if (this.m_topArrowHeld)
        this.RollUpOne();
      else if (this.m_bottomArrowHeld)
        this.RollDownOne();
      this.m_holdPeriod = 0.0f;
    }
    if (!this.m_topArrowHeld && !this.m_bottomArrowHeld)
      return;
    this.m_holdPeriod += Time.deltaTime;
    this.m_totalHoldDuration += Time.deltaTime;
  }

  private void UpdateMouseInput()
  {
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null)
      return;
    Ray ray = firstByLayer.ScreenPointToRay(InputCollection.GetMousePosition());
    if (!this.GetComponent<Collider>().Raycast(ray, out RaycastHit _, firstByLayer.farClipPlane))
      return;
    float axis = Input.GetAxis("Mouse ScrollWheel");
    if ((double) axis < 0.0)
    {
      this.RollDownOne();
    }
    else
    {
      if ((double) axis <= 0.0)
        return;
      this.RollUpOne();
    }
  }

  protected override void OnDestroy()
  {
    this.m_EnabledArrowButtonMaterial = (Material) null;
    this.m_DisabledArrowButtonMaterial = (Material) null;
    base.OnDestroy();
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    base.OnOver(oldState);
    this.m_mouseOver = true;
    this.m_TopArrow.gameObject.SetActive(true);
    this.m_BottomArrow.gameObject.SetActive(true);
    this.ShowTooltip();
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    base.OnOut(oldState);
    this.m_mouseOver = false;
    this.m_TopArrow.gameObject.SetActive(false);
    this.m_BottomArrow.gameObject.SetActive(false);
    this.HideTooltip();
  }

  private void ShowTooltip()
  {
    if (!this.m_EnableTooltip || !((UnityEngine.Object) this.m_TooltipPrefab != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_TooltipBone != (UnityEngine.Object) null))
      return;
    if ((UnityEngine.Object) this.m_tooltip != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_tooltip.gameObject);
    GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.m_TooltipPrefab);
    LayerUtils.SetLayer(go, GameLayer.UI);
    this.m_tooltip = go.GetComponent<TooltipPanel>();
    this.m_tooltip.Reset();
    this.m_tooltip.Initialize(this.m_tooltipHeaderText, this.m_tooltipDescText);
    GameUtils.SetParent((Component) this.m_tooltip, (Component) this.m_TooltipBone);
  }

  private void HideTooltip()
  {
    if (!this.m_EnableTooltip || !((UnityEngine.Object) this.m_tooltip != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_tooltip.gameObject);
  }

  public void UpdateTooltip(string headerText, string descText)
  {
    if (!this.m_EnableTooltip)
      return;
    this.m_tooltipHeaderText = headerText;
    this.m_tooltipDescText = descText;
    if (!this.m_mouseOver)
      return;
    this.ShowTooltip();
  }

  public void AddItem(object value)
  {
    this.m_items.Add(value);
    this.LayoutDial();
  }

  public bool RemoveItem(object value)
  {
    int itemIndex = this.FindItemIndex(value);
    if (itemIndex < 0)
      return false;
    this.m_items.RemoveAt(itemIndex);
    this.LayoutDial();
    return true;
  }

  public void ClearItems()
  {
    this.m_items.Clear();
    this.LayoutDial();
  }

  public object GetSelection() => (UnityEngine.Object) this.m_selectedItem == (UnityEngine.Object) null ? (object) null : this.m_selectedItem.GetValue();

  private void OnTopArrowReleased(UIEvent e)
  {
    if (this.m_topArrowHeld)
      this.StopHoldingArrow();
    else
      this.RollUpOne();
  }

  private void OnTopArrowHeld(UIEvent e)
  {
    this.m_topArrowHeld = true;
    this.m_currentHoldRollFrequency = this.m_StartingHoldRollFrequency;
  }

  private void OnBottomArrowReleased(UIEvent e)
  {
    if (this.m_bottomArrowHeld)
      this.StopHoldingArrow();
    else
      this.RollDownOne();
  }

  private void OnBottomArrowHeld(UIEvent e)
  {
    this.m_bottomArrowHeld = true;
    this.m_currentHoldRollFrequency = this.m_StartingHoldRollFrequency;
  }

  private void OnArrowOver(UIEvent e) => this.TriggerOver();

  private void OnArrowOut(UIEvent e) => this.StopHoldingArrow();

  private void LayoutDial()
  {
    if (!this.gameObject.activeSelf)
      return;
    foreach (DialItem dialItem in this.m_DialItems)
      dialItem.SetValue((object) null, string.Empty);
    this.m_itemsSelectedIndex = 0;
    this.m_itemsFirstVisibleIndex = this.m_itemsSelectedIndex - this.m_NumItemsShowingAbove;
    this.m_itemsLastVisibleIndex = this.m_itemsSelectedIndex + this.m_NumItemsShowingBelow;
    int num1 = this.m_NumItemsShowingBelow + 1;
    int num2 = 0;
    int index = this.m_dialItemsSelectedIndex;
    foreach (object val in this.m_items)
    {
      if (index >= this.m_DialItems.Count)
        index = 0;
      this.m_DialItems[index].SetValue(val, this.m_itemTextCallback(val));
      ++index;
      if (++num2 >= num1)
        break;
    }
    this.m_selectedItem = this.m_DialItems[this.m_dialItemsSelectedIndex];
    this.UpdateArrowButtonsState();
  }

  private void RollUpOne()
  {
    if (this.m_items.Count == 0 || this.m_itemsSelectedIndex == 0)
    {
      this.StopHoldingArrow();
    }
    else
    {
      if (iTween.CountByName(this.m_Dial, "spin") > 0)
      {
        iTween.StopByName(this.m_Dial, "spin");
        this.m_Dial.transform.localRotation = this.m_desiredOrientation;
      }
      Quaternion targetOrientation = this.m_Dial.transform.localRotation * this.m_upRotationQuat;
      this.m_desiredOrientation = targetOrientation;
      iTween.RotateAdd(this.m_Dial, iTween.Hash((object) "amount", (object) this.m_upRotationEuler, (object) "time", (object) this.m_AnimateSpinTime, (object) "easeType", (object) iTween.EaseType.easeOutBounce, (object) "isLocal", (object) true, (object) "name", (object) "spin", (object) "oncomplete", (object) (Action<object>) (o => this.m_Dial.transform.localRotation = targetOrientation)));
      --this.m_itemsSelectedIndex;
      if (--this.m_dialItemsSelectedIndex < 0)
        this.m_dialItemsSelectedIndex = this.m_DialItems.Count - 1;
      --this.m_itemsFirstVisibleIndex;
      if (--this.m_dialItemsFirstIndex < 0)
        this.m_dialItemsFirstIndex = this.m_DialItems.Count - 1;
      if (this.m_itemsFirstVisibleIndex >= 0)
      {
        object val = this.m_items[this.m_itemsFirstVisibleIndex];
        this.m_DialItems[this.m_dialItemsFirstIndex].SetValue(val, this.m_itemTextCallback(val));
      }
      else
        this.m_DialItems[this.m_dialItemsFirstIndex].SetValue((object) null, string.Empty);
      this.m_DialItems[this.m_dialItemsLastIndex].SetValue((object) null, string.Empty);
      --this.m_itemsLastVisibleIndex;
      if (--this.m_dialItemsLastIndex < 0)
        this.m_dialItemsLastIndex = this.m_DialItems.Count - 1;
      this.m_selectedItem = this.m_DialItems[this.m_dialItemsSelectedIndex];
      this.m_itemChosenCallback(this.m_items[this.m_itemsSelectedIndex], this.m_items[this.m_itemsSelectedIndex + 1]);
      this.UpdateArrowButtonsState();
    }
  }

  private void RollDownOne()
  {
    if (this.m_items.Count == 0 || this.m_itemsSelectedIndex == this.m_items.Count - 1)
    {
      this.StopHoldingArrow();
    }
    else
    {
      if (iTween.CountByName(this.m_Dial, "spin") > 0)
      {
        iTween.StopByName(this.m_Dial, "spin");
        this.m_Dial.transform.localRotation = this.m_desiredOrientation;
      }
      Quaternion targetOrientation = this.m_Dial.transform.localRotation * this.m_downRotationQuat;
      this.m_desiredOrientation = targetOrientation;
      iTween.RotateAdd(this.m_Dial, iTween.Hash((object) "amount", (object) this.m_downRotationEuler, (object) "time", (object) this.m_AnimateSpinTime, (object) "easeType", (object) iTween.EaseType.easeOutBounce, (object) "isLocal", (object) true, (object) "name", (object) "spin", (object) "oncomplete", (object) (Action<object>) (o => this.m_Dial.transform.localRotation = targetOrientation)));
      ++this.m_itemsSelectedIndex;
      if (++this.m_dialItemsSelectedIndex >= this.m_DialItems.Count)
        this.m_dialItemsSelectedIndex = 0;
      ++this.m_itemsLastVisibleIndex;
      if (++this.m_dialItemsLastIndex >= this.m_DialItems.Count)
        this.m_dialItemsLastIndex = 0;
      if (this.m_itemsLastVisibleIndex < this.m_items.Count)
      {
        object val = this.m_items[this.m_itemsLastVisibleIndex];
        this.m_DialItems[this.m_dialItemsLastIndex].SetValue(val, this.m_itemTextCallback(val));
      }
      else
        this.m_DialItems[this.m_dialItemsLastIndex].SetValue((object) null, string.Empty);
      this.m_DialItems[this.m_dialItemsFirstIndex].SetValue((object) null, string.Empty);
      ++this.m_itemsFirstVisibleIndex;
      if (++this.m_dialItemsFirstIndex >= this.m_DialItems.Count)
        this.m_dialItemsFirstIndex = 0;
      this.m_selectedItem = this.m_DialItems[this.m_dialItemsSelectedIndex];
      this.m_itemChosenCallback(this.m_items[this.m_itemsSelectedIndex], this.m_items[this.m_itemsSelectedIndex - 1]);
      this.UpdateArrowButtonsState();
    }
  }

  public UIBDial.ItemChosenCallback GetItemChosenCallback() => this.m_itemChosenCallback;

  public void SetItemChosenCallback(UIBDial.ItemChosenCallback callback) => this.m_itemChosenCallback = callback ?? (UIBDial.ItemChosenCallback) ((_param1, _param2) => { });

  public UIBDial.ItemTextCallback GetItemTextCallback() => this.m_itemTextCallback;

  public void SetItemTextCallback(UIBDial.ItemTextCallback callback) => this.m_itemTextCallback = callback ?? new UIBDial.ItemTextCallback(UIBDial.DefaultItemTextCallback);

  public static string DefaultItemTextCallback(object val) => val != null ? val.ToString() : string.Empty;

  private int FindItemIndex(object value)
  {
    for (int index = 0; index < this.m_items.Count; ++index)
    {
      if (this.m_items[index] == value)
        return index;
    }
    return -1;
  }

  private int FindVisibleItemIndex(object value)
  {
    for (int index = 0; index < this.m_DialItems.Count; ++index)
    {
      if (this.m_DialItems[index].GetValue() == value)
        return index;
    }
    return -1;
  }

  private void UpdateArrowButtonsState()
  {
    if ((UnityEngine.Object) this.m_TopArrowMesh != (UnityEngine.Object) null)
    {
      if (this.m_itemsSelectedIndex == 0)
      {
        this.m_TopArrowMesh.SetMaterial(this.m_DisabledArrowButtonMaterial);
        this.m_TopArrow.GetComponent<Collider>().enabled = false;
      }
      else
      {
        this.m_TopArrowMesh.SetMaterial(this.m_EnabledArrowButtonMaterial);
        this.m_TopArrow.GetComponent<Collider>().enabled = true;
      }
    }
    if (!((UnityEngine.Object) this.m_BottomArrowMesh != (UnityEngine.Object) null))
      return;
    if (this.m_itemsSelectedIndex == this.m_items.Count - 1)
    {
      this.m_BottomArrowMesh.SetMaterial(this.m_DisabledArrowButtonMaterial);
      this.m_BottomArrow.GetComponent<Collider>().enabled = false;
    }
    else
    {
      this.m_BottomArrowMesh.SetMaterial(this.m_EnabledArrowButtonMaterial);
      this.m_BottomArrow.GetComponent<Collider>().enabled = true;
    }
  }

  private void StopHoldingArrow()
  {
    this.m_bottomArrowHeld = false;
    this.m_topArrowHeld = false;
    this.m_totalHoldDuration = 0.0f;
  }

  public delegate void ItemChosenCallback(object selection, object prevSelection);

  public delegate string ItemTextCallback(object val);
}
