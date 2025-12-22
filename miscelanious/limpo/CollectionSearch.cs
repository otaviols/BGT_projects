using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectionSearch : MonoBehaviour
{
  public UberText m_searchText;
  public PegUIElement m_background;
  public PegUIElement m_clearButton;
  public GameObject m_xMesh;
  public GameObject m_backgroundWhenAtBottom;
  public GameObject m_backgroundWhenAtBottomTavernBrawl;
  public GameObject m_backgroundWhenAtTopNormal;
  public GameObject m_backgroundWhenAtTopWild;
  public Color m_altSearchColor;
  private const float ANIM_TIME = 0.1f;
  private const int MAX_SEARCH_LENGTH = 31;
  private Material m_origSearchMaterial;
  private Vector3 m_origSearchPos;
  private bool m_isActive;
  private string m_prevText;
  private string m_text;
  private bool m_wildModeActive;
  private List<CollectionSearch.ActivatedListener> m_activatedListeners = new List<CollectionSearch.ActivatedListener>();
  private List<CollectionSearch.DeactivatedListener> m_deactivatedListeners = new List<CollectionSearch.DeactivatedListener>();
  private List<CollectionSearch.ClearedListener> m_clearedListeners = new List<CollectionSearch.ClearedListener>();
  private GameLayer m_originalLayer;
  private GameLayer m_activeLayer;
  private bool m_isTouchKeyboardDisplayMode;

  private void Start()
  {
    this.m_background.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackgroundReleased));
    this.m_clearButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClearReleased));
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    touchScreenService.AddOnVirtualKeyboardShowListener(new Action(this.OnKeyboardShown));
    touchScreenService.AddOnVirtualKeyboardHideListener(new Action(this.OnKeyboardHidden));
    this.m_origSearchPos = this.transform.localPosition;
    this.UpdateBackground();
    this.UpdateSearchText();
  }

  private void OnDestroy()
  {
    ITouchScreenService service;
    if (ServiceManager.TryGet<ITouchScreenService>(out service))
    {
      service.RemoveOnVirtualKeyboardShowListener(new Action(this.OnKeyboardShown));
      service.RemoveOnVirtualKeyboardHideListener(new Action(this.OnKeyboardHidden));
    }
    if (UniversalInputManager.Get() == null)
      return;
    UniversalInputManager.Get().CancelTextInput(this.gameObject);
  }

  public bool IsActive() => this.m_isActive;

  public void SetActiveLayer(GameLayer activeLayer)
  {
    if (activeLayer == this.m_activeLayer)
      return;
    this.m_activeLayer = activeLayer;
    if (!this.IsActive())
      return;
    this.MoveToActiveLayer(false);
  }

  public void SetWildModeActive(bool active) => this.m_wildModeActive = active;

  public void Activate(bool ignoreTouchMode = false)
  {
    if (this.m_isActive)
      return;
    this.m_background.SetEnabled(false);
    this.MoveToActiveLayer(true);
    this.m_isActive = true;
    this.m_prevText = this.m_text;
    foreach (CollectionSearch.ActivatedListener activatedListener in this.m_activatedListeners.ToArray())
      activatedListener();
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    if (!ignoreTouchMode && UniversalInputManager.Get().UseWindowsTouch() && touchScreenService.IsTouchSupported() && touchScreenService.IsVirtualKeyboardVisible())
      this.TouchKeyboardSearchDisplay(true);
    else
      this.ShowInput();
  }

  public void Deactivate()
  {
    if (!this.m_isActive)
      return;
    this.m_background.SetEnabled(true);
    this.MoveToOriginalLayer();
    this.m_isActive = false;
    this.HideInput();
    this.ResetSearchDisplay();
    foreach (CollectionSearch.DeactivatedListener deactivatedListener in this.m_deactivatedListeners.ToArray())
      deactivatedListener(this.m_prevText, this.m_text);
  }

  public void Cancel()
  {
    if (!this.m_isActive)
      return;
    this.m_text = this.m_prevText;
    this.UpdateSearchText();
    this.Deactivate();
  }

  public string GetText() => this.m_text;

  public void SetText(string text)
  {
    this.m_text = text;
    this.UpdateSearchText();
  }

  public void ClearFilter(bool updateVisuals = true)
  {
    this.m_text = "";
    this.UpdateSearchText();
    this.ClearInput();
    foreach (CollectionSearch.ClearedListener clearedListener in this.m_clearedListeners.ToArray())
      clearedListener(updateVisuals);
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    if ((!UniversalInputManager.Get().IsTouchMode() || !touchScreenService.IsTouchSupported()) && !touchScreenService.IsVirtualKeyboardVisible())
      return;
    this.Deactivate();
  }

  public void RegisterActivatedListener(CollectionSearch.ActivatedListener listener)
  {
    if (this.m_activatedListeners.Contains(listener))
      return;
    this.m_activatedListeners.Add(listener);
  }

  public void RemoveActivatedListener(CollectionSearch.ActivatedListener listener) => this.m_activatedListeners.Remove(listener);

  public void RegisterDeactivatedListener(CollectionSearch.DeactivatedListener listener)
  {
    if (this.m_deactivatedListeners.Contains(listener))
      return;
    this.m_deactivatedListeners.Add(listener);
  }

  public void RemoveDeactivatedListener(CollectionSearch.DeactivatedListener listener) => this.m_deactivatedListeners.Remove(listener);

  public void RegisterClearedListener(CollectionSearch.ClearedListener listener)
  {
    if (this.m_clearedListeners.Contains(listener))
      return;
    this.m_clearedListeners.Add(listener);
  }

  public void RemoveClearedListener(CollectionSearch.ClearedListener listener) => this.m_clearedListeners.Remove(listener);

  public void SetEnabled(bool enabled)
  {
    this.m_background.SetEnabled(enabled);
    this.m_clearButton.SetEnabled(enabled);
  }

  private void OnBackgroundReleased(UIEvent e) => this.Activate();

  private void OnClearReleased(UIEvent e) => this.ClearFilter();

  private void OnActivateAnimComplete() => this.ShowInput();

  private void OnDeactivateAnimComplete()
  {
    foreach (CollectionSearch.DeactivatedListener deactivatedListener in this.m_deactivatedListeners.ToArray())
      deactivatedListener(this.m_prevText, this.m_text);
  }

  private void ShowInput(bool fromActivate = true)
  {
    Bounds bounds = this.m_searchText.GetBounds();
    this.m_searchText.gameObject.SetActive(false);
    Rect guiViewportRect = CameraUtils.CreateGUIViewportRect(Box.Get().GetCamera(), bounds.min, bounds.max);
    Color? nullable = new Color?();
    if (ServiceManager.Get<ITouchScreenService>().IsVirtualKeyboardVisible())
      nullable = new Color?(this.m_altSearchColor);
    UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams()
    {
      m_owner = this.gameObject,
      m_rect = guiViewportRect,
      m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(this.OnInputUpdated),
      m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
      m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(this.OnInputCanceled),
      m_unfocusedCallback = new UniversalInputManager.TextInputUnfocusedCallback(this.OnInputUnfocus),
      m_font = this.m_searchText.GetLocalizedFont(),
      m_text = this.m_text,
      m_color = nullable,
      m_touchScreenKeyboardHideInput = false
    };
    parms.m_showVirtualKeyboard = fromActivate;
    UniversalInputManager.Get().UseTextInput(parms);
  }

  private void HideInput()
  {
    UniversalInputManager.Get().CancelTextInput(this.gameObject);
    this.m_searchText.gameObject.SetActive(true);
  }

  private void ClearInput()
  {
    if (!this.m_isActive)
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "text_box_delete_text.prefab:b4209934f760cc745b3dba5add912398");
    UniversalInputManager.Get().SetInputText("");
  }

  private void OnInputUpdated(string input)
  {
    this.m_text = input;
    this.UpdateSearchText();
  }

  private void OnInputComplete(string input)
  {
    this.m_text = input;
    this.UpdateSearchText();
    SoundManager.Get().LoadAndPlay((AssetReference) "text_commit.prefab:05a794ae046d3e842b87893629a826f1");
    this.Deactivate();
  }

  private void OnInputCanceled(bool userRequested, GameObject requester) => this.Cancel();

  private void OnInputUnfocus()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    this.Deactivate();
  }

  private void UpdateSearchText()
  {
    if (string.IsNullOrEmpty(this.m_text))
    {
      this.m_searchText.Text = GameStrings.Get("GLUE_COLLECTION_SEARCH");
      this.m_clearButton.gameObject.SetActive(false);
    }
    else
    {
      this.m_searchText.Text = this.m_text;
      this.m_clearButton.gameObject.SetActive(true);
    }
  }

  private void MoveToActiveLayer(bool saveOriginalLayer)
  {
    if (saveOriginalLayer)
      this.m_originalLayer = (GameLayer) this.gameObject.layer;
    LayerUtils.SetLayer(this.gameObject, this.m_activeLayer);
  }

  private void MoveToOriginalLayer() => LayerUtils.SetLayer(this.gameObject, this.m_originalLayer);

  private void TouchKeyboardSearchDisplay(bool fromActivate = false)
  {
    if (this.m_isTouchKeyboardDisplayMode)
      return;
    this.m_isTouchKeyboardDisplayMode = true;
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      this.transform.localPosition = collectibleDisplay.m_activeSearchBone_Win8.transform.localPosition;
    this.HideInput();
    this.ShowInput(fromActivate || ServiceManager.Get<ITouchScreenService>().IsVirtualKeyboardVisible());
    RendererExtension.GetMaterial(this.m_xMesh.GetComponent<Renderer>()).SetColor("_Color", this.m_altSearchColor);
    this.UpdateBackground();
  }

  private void ResetSearchDisplay()
  {
    if (!this.m_isTouchKeyboardDisplayMode)
      return;
    this.m_isTouchKeyboardDisplayMode = false;
    this.transform.localPosition = this.m_origSearchPos;
    this.HideInput();
    this.ShowInput(false);
    RendererExtension.GetMaterial(this.m_xMesh.GetComponent<Renderer>()).SetColor("_Color", Color.white);
    this.UpdateBackground();
  }

  private void OnKeyboardShown()
  {
    if (!this.m_isActive || this.m_isTouchKeyboardDisplayMode)
      return;
    this.TouchKeyboardSearchDisplay();
  }

  private void OnKeyboardHidden()
  {
    if (!this.m_isActive || !this.m_isTouchKeyboardDisplayMode)
      return;
    this.ResetSearchDisplay();
  }

  private void UpdateBackground()
  {
    bool flag = (UnityEngine.Object) this.m_backgroundWhenAtTopNormal != (UnityEngine.Object) null || (UnityEngine.Object) this.m_backgroundWhenAtTopWild != (UnityEngine.Object) null;
    GameObject gameObject = !SceneMgr.Get().IsInTavernBrawlMode() || !((UnityEngine.Object) this.m_backgroundWhenAtBottomTavernBrawl != (UnityEngine.Object) null) ? this.m_backgroundWhenAtBottom : this.m_backgroundWhenAtBottomTavernBrawl;
    if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
      gameObject.gameObject.SetActive(!this.m_isTouchKeyboardDisplayMode || !flag);
    if ((UnityEngine.Object) this.m_backgroundWhenAtTopNormal != (UnityEngine.Object) null)
      this.m_backgroundWhenAtTopNormal.gameObject.SetActive(this.m_isTouchKeyboardDisplayMode && ((UnityEngine.Object) this.m_backgroundWhenAtTopWild == (UnityEngine.Object) null || !this.m_wildModeActive));
    if (!((UnityEngine.Object) this.m_backgroundWhenAtTopWild != (UnityEngine.Object) null))
      return;
    this.m_backgroundWhenAtTopWild.gameObject.SetActive(this.m_isTouchKeyboardDisplayMode && this.m_wildModeActive);
  }

  public delegate void ActivatedListener();

  public delegate void DeactivatedListener(string oldSearchText, string newSearchText);

  public delegate void ClearedListener(bool updateVisuals);
}
