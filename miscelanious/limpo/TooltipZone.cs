using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TooltipZone : MonoBehaviour, IPopupRendering
{
  public GameObject tooltipPrefab;
  public Transform tooltipDisplayLocation;
  public Transform touchTooltipLocation;
  public GameObject targetObject;
  public TooltipZone.TooltipLayoutDirection m_tooltipDirection;
  private List<GameObject> m_tooltips = new List<GameObject>();
  private TooltipZone.TooltipChangeCallback m_changeCallback;
  private List<Action> m_onTooltipHiddenCallbacks = new List<Action>();
  private string m_defaultHeadlineText = string.Empty;
  private string m_defaultBodyText = string.Empty;
  private float m_defaultScale = 1f;
  private GameLayer? m_layerOverride;
  private GameLayer? m_screenConstraintLayerOverride;
  private IPopupRoot m_popupRoot;
  private HashSet<IPopupRendering> m_popupRenderingComponents = new HashSet<IPopupRendering>();

  [Overridable]
  public GameLayer LayerOverride
  {
    get => !this.m_layerOverride.HasValue ? GameLayer.Default : this.m_layerOverride.Value;
    set
    {
      this.m_layerOverride = new GameLayer?(value);
      GameObject tooltipObject = this.GetTooltipObject();
      if (!((UnityEngine.Object) tooltipObject != (UnityEngine.Object) null))
        return;
      LayerUtils.SetLayer(tooltipObject, value);
    }
  }

  [Overridable]
  public GameLayer ScreenConstraintLayerOverride
  {
    get => !this.m_screenConstraintLayerOverride.HasValue ? GameLayer.Default : this.m_screenConstraintLayerOverride.Value;
    set
    {
      this.m_screenConstraintLayerOverride = new GameLayer?(value);
      GameObject tooltipObject = this.GetTooltipObject();
      if (!((UnityEngine.Object) tooltipObject != (UnityEngine.Object) null))
        return;
      LayerUtils.SetLayer(tooltipObject, value);
    }
  }

  [Overridable]
  public string HeadlineText
  {
    get => this.m_defaultHeadlineText;
    set
    {
      this.m_defaultHeadlineText = GameStrings.Get(value);
      TooltipPanel tooltipPanel = this.GetTooltipPanel();
      if (!((UnityEngine.Object) tooltipPanel != (UnityEngine.Object) null))
        return;
      tooltipPanel.SetName(this.m_defaultHeadlineText);
      tooltipPanel.m_name.UpdateNow();
    }
  }

  [Overridable]
  public string BodyText
  {
    get => this.m_defaultBodyText;
    set
    {
      this.m_defaultBodyText = GameStrings.Get(value);
      TooltipPanel tooltipPanel = this.GetTooltipPanel();
      if (!((UnityEngine.Object) tooltipPanel != (UnityEngine.Object) null))
        return;
      tooltipPanel.SetBodyText(this.m_defaultBodyText);
      tooltipPanel.m_body.UpdateNow();
    }
  }

  [Overridable]
  public float Scale
  {
    get => this.m_defaultScale;
    set
    {
      this.m_defaultScale = value;
      TooltipPanel tooltipPanel = this.GetTooltipPanel();
      if (!((UnityEngine.Object) tooltipPanel != (UnityEngine.Object) null))
        return;
      tooltipPanel.SetScale(value);
    }
  }

  [Overridable]
  public bool Shown
  {
    get => this.IsShowingTooltip();
    set
    {
      if (value)
        this.ShowTooltip();
      else
        this.HideTooltip();
    }
  }

  private void Awake()
  {
    WidgetTransform component = this.GetComponent<WidgetTransform>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || !((UnityEngine.Object) this.GetComponent<Clickable>() == (UnityEngine.Object) null))
      return;
    component.CreateBoxCollider(this.gameObject);
  }

  public GameObject GetTooltipObject(int index = 0) => index < 0 || index >= this.m_tooltips.Count ? (GameObject) null : this.m_tooltips[index];

  public TooltipPanel GetTooltipPanel(int index = 0)
  {
    GameObject tooltipObject = this.GetTooltipObject(index);
    return (UnityEngine.Object) tooltipObject == (UnityEngine.Object) null ? (TooltipPanel) null : tooltipObject.GetComponent<TooltipPanel>();
  }

  public bool IsShowingTooltip(int index = 0) => (UnityEngine.Object) this.GetTooltipObject(index) != (UnityEngine.Object) null;

  public TooltipPanel ShowTooltip(int index = 0)
  {
    TooltipPanel c = this.ShowTooltip(this.m_defaultHeadlineText, this.m_defaultBodyText, this.m_defaultScale, Vector3.zero, index);
    if ((UnityEngine.Object) c != (UnityEngine.Object) null)
    {
      c.SetScale(this.m_defaultScale);
      if (this.m_layerOverride.HasValue)
        LayerUtils.SetLayer((Component) c, this.m_layerOverride.Value);
    }
    return c;
  }

  public TooltipPanel ShowTooltip(
    string headline,
    string bodytext,
    float scale,
    int index = 0)
  {
    return this.ShowTooltip(headline, bodytext, scale, Vector3.zero, index);
  }

  public TooltipPanel ShowTooltip(
    string headline,
    string bodytext,
    float scale,
    Vector3 localOffset,
    int index = 0)
  {
    if (this.IsShowingTooltip(index))
      return this.m_tooltips[index].GetComponent<TooltipPanel>();
    if (index < 0)
      return (TooltipPanel) null;
    while (this.m_tooltips.Count <= index)
      this.m_tooltips.Add((GameObject) null);
    if ((bool) UniversalInputManager.UsePhoneUI)
      scale *= 2f;
    this.m_tooltips[index] = UnityEngine.Object.Instantiate<GameObject>(this.tooltipPrefab);
    TooltipPanel component = this.m_tooltips[index].GetComponent<TooltipPanel>();
    component.Reset();
    component.Initialize(headline, bodytext);
    component.SetScale(scale);
    if (UniversalInputManager.Get().IsTouchMode() && (UnityEngine.Object) this.touchTooltipLocation != (UnityEngine.Object) null)
    {
      component.transform.position = this.touchTooltipLocation.position;
      component.transform.rotation = this.touchTooltipLocation.rotation;
    }
    else if ((UnityEngine.Object) this.tooltipDisplayLocation != (UnityEngine.Object) null)
    {
      component.transform.position = this.tooltipDisplayLocation.position;
      component.transform.rotation = this.tooltipDisplayLocation.rotation;
    }
    component.transform.parent = this.transform;
    component.transform.localPosition += localOffset;
    Vector3 previousTooltips = this.GetHeightOfPreviousTooltips(index);
    component.transform.localPosition += previousTooltips;
    int layer = this.gameObject.layer;
    if (this.m_popupRoot != null && !this.m_popupRoot.IsPerspectivePopup)
      layer = 5;
    else if (this.m_screenConstraintLayerOverride.HasValue)
      layer = (int) this.m_screenConstraintLayerOverride.Value;
    TransformUtil.ConstrainToScreen(this.m_tooltips[index], layer);
    if (this.m_changeCallback != null)
      this.m_changeCallback(true);
    component.ShiftBodyText();
    if (this.m_popupRoot != null)
      this.m_popupRoot.ApplyPopupRendering(this.m_tooltips[index].transform, this.m_popupRenderingComponents, true, this.m_layerOverride.HasValue ? (int) this.m_layerOverride.Value : this.gameObject.layer);
    return component;
  }

  public void ShowGameplayTooltip(string headline, string bodytext, int index = 0) => this.ShowGameplayTooltip(headline, bodytext, Vector3.zero, index);

  public void ShowGameplayTooltip(
    string headline,
    string bodytext,
    Vector3 localOffset,
    int index = 0)
  {
    this.ShowTooltip(headline, bodytext, 0.75f, index);
  }

  public void ShowGameplayTooltipLarge(string headline, string bodytext, int index = 0) => this.ShowGameplayTooltipLarge(headline, bodytext, Vector3.zero, index);

  public void ShowGameplayTooltipLarge(
    string headline,
    string bodytext,
    Vector3 localOffset,
    int index = 0)
  {
    this.ShowTooltip(headline, bodytext, (float) TooltipPanel.GAMEPLAY_SCALE_LARGE, localOffset, index);
  }

  public void ShowBoxTooltip(string headline, string bodytext, int index = 0) => this.ShowTooltip(headline, bodytext, (float) TooltipPanel.BOX_SCALE, index);

  public void ShowCollectionManagerTooltip(string headline, string bodytext, int index = 0) => this.ShowTooltip(headline, bodytext, (float) TooltipPanel.COLLECTION_MANAGER_SCALE, index);

  public TooltipPanel ShowLayerTooltip(string headline, string bodytext, int index = 0) => this.ShowLayerTooltip(headline, bodytext, 1f, index);

  public TooltipPanel ShowLayerTooltip(
    string headline,
    string bodytext,
    float scale,
    int index = 0)
  {
    TooltipPanel tooltipPanel = this.ShowTooltip(headline, bodytext, scale, index);
    if ((UnityEngine.Object) this.tooltipDisplayLocation == (UnityEngine.Object) null || (UnityEngine.Object) tooltipPanel == (UnityEngine.Object) null)
      return tooltipPanel;
    tooltipPanel.transform.parent = this.tooltipDisplayLocation.transform;
    Vector3 vector3 = new Vector3(scale, scale, scale);
    tooltipPanel.transform.localScale = vector3;
    LayerUtils.SetLayer(this.m_tooltips[index], this.tooltipDisplayLocation.gameObject.layer);
    return tooltipPanel;
  }

  public void ShowSocialTooltip(
    Component target,
    string headline,
    string bodytext,
    float scale,
    GameLayer layer,
    int index = 0)
  {
    this.ShowSocialTooltip(target.gameObject, headline, bodytext, scale, layer, index);
  }

  public void ShowSocialTooltip(
    GameObject tooltipTargetObject,
    string headline,
    string bodytext,
    float scale,
    GameLayer layer,
    int index = 0)
  {
    this.ShowTooltip(headline, bodytext, scale, index);
    LayerUtils.SetLayer(this.m_tooltips[index], layer);
    Camera firstByLayer1 = CameraUtils.FindFirstByLayer(tooltipTargetObject.layer);
    Camera firstByLayer2 = CameraUtils.FindFirstByLayer(this.m_tooltips[index].layer);
    if (!((UnityEngine.Object) firstByLayer1 != (UnityEngine.Object) firstByLayer2))
      return;
    Vector3 screenPoint = firstByLayer1.WorldToScreenPoint(this.m_tooltips[index].transform.position);
    Vector3 worldPoint = firstByLayer2.ScreenToWorldPoint(screenPoint);
    this.m_tooltips[index].transform.position = worldPoint;
  }

  public void ShowMultiColumnTooltip(
    string headline,
    string bodytext,
    string[] columnsText,
    float scale,
    int index = 0)
  {
    TooltipPanel tooltipPanel = this.ShowTooltip(headline, bodytext, scale, index);
    if (!(tooltipPanel is MultiColumnTooltipPanel))
      return;
    MultiColumnTooltipPanel columnTooltipPanel = (MultiColumnTooltipPanel) tooltipPanel;
    if (columnsText.Length > columnTooltipPanel.m_textColumns.Count)
      Log.All.PrintWarning("ShowMultiColumnTooltip - Attempting to display {0} columns of text, when the prefab only supports {1} columns!", (object) columnsText.Length, (object) columnTooltipPanel.m_textColumns.Count);
    for (int index1 = 0; index1 < columnsText.Length && index1 < columnTooltipPanel.m_textColumns.Count; ++index1)
      columnTooltipPanel.m_textColumns[index1].Text = columnsText[index1];
  }

  private Vector3 GetHeightOfPreviousTooltips(int currentIndex)
  {
    float z = 0.0f;
    if (this.m_tooltipDirection == TooltipZone.TooltipLayoutDirection.DOWN)
    {
      for (int index = 0; index < currentIndex; ++index)
      {
        if (this.IsShowingTooltip(index))
        {
          TooltipPanel component = this.GetTooltipObject(index).GetComponent<TooltipPanel>();
          z -= component.GetHeight() / 2f;
        }
      }
    }
    else if (this.m_tooltipDirection == TooltipZone.TooltipLayoutDirection.UP)
    {
      for (int index = 1; index <= currentIndex; ++index)
      {
        if (this.IsShowingTooltip(index))
        {
          TooltipPanel component = this.GetTooltipObject(index).GetComponent<TooltipPanel>();
          z += component.GetHeight() * 1.5f;
        }
      }
    }
    return new Vector3(0.0f, 0.0f, z);
  }

  public void AnchorTooltipTo(
    GameObject target,
    Anchor targetAnchorPoint,
    Anchor tooltipAnchorPoint,
    int index = 0)
  {
    if (!this.IsShowingTooltip(index))
      return;
    TransformUtil.SetPoint(this.m_tooltips[index], tooltipAnchorPoint, target, targetAnchorPoint);
  }

  public void HideTooltip()
  {
    TooltipZone.TooltipChangeCallback changeCallback = this.m_changeCallback;
    if (changeCallback != null)
      changeCallback(false);
    foreach (Action tooltipHiddenCallback in this.m_onTooltipHiddenCallbacks)
      tooltipHiddenCallback();
    this.m_onTooltipHiddenCallbacks.Clear();
    foreach (GameObject tooltip in this.m_tooltips)
    {
      if ((UnityEngine.Object) tooltip != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) tooltip);
    }
  }

  public void SetTooltipChangeCallback(TooltipZone.TooltipChangeCallback callback) => this.m_changeCallback = callback;

  public void RegisterOnTooltipHiddenCallback(Action callback) => this.m_onTooltipHiddenCallbacks.Add(callback);

  public void EnablePopupRendering(IPopupRoot popupRoot) => this.m_popupRoot = popupRoot;

  public void DisablePopupRendering()
  {
    if (this.m_popupRoot == null)
      return;
    this.m_popupRoot.CleanupPopupRendering(this.m_popupRenderingComponents);
    this.m_popupRoot = (IPopupRoot) null;
  }

  public bool HandlesChildPropagation() => false;

  public delegate void TooltipChangeCallback(bool shown);

  public enum TooltipLayoutDirection
  {
    DOWN,
    UP,
  }
}
