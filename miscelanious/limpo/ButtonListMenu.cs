using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonListMenu : MonoBehaviour
{
  protected ButtonListMenuDef m_menu;
  protected GameLayer m_targetLayer = GameLayer.UI;
  private bool m_isShown;
  private List<UIBButton> m_allButtons = new List<UIBButton>();
  private List<GameObject> m_horizontalDividers = new List<GameObject>();
  protected PegUIElement m_blocker;
  protected Transform m_menuParent;
  protected float PUNCH_SCALE = 1.08f;
  protected Vector3 NORMAL_SCALE = Vector3.one;
  protected static readonly Vector3 HIDDEN_SCALE = 0.01f * Vector3.one;
  protected string m_menuDefPrefab = "ButtonListMenuDef.prefab:1ab57b5c429373a4b8b4e0c0c706ca3e";
  protected bool m_showAnimation = true;

  protected virtual void Awake()
  {
    GameObject gameObject = (GameObject) GameUtils.InstantiateGameObject(this.m_menuDefPrefab);
    this.m_menu = gameObject.GetComponent<ButtonListMenuDef>();
    OverlayUI.Get().AddGameObject(this.gameObject);
    this.SetTransform();
    this.m_blocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(gameObject.layer), "GameMenuInputBlocker", (Component) this, (Component) gameObject.transform, 10f).AddComponent<PegUIElement>();
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    this.m_blocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBlockerRelease));
  }

  protected virtual void OnDestroy() => FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));

  public virtual void Show(bool playSound = true)
  {
    UniversalInputManager.Get().CancelTextInput(this.gameObject, true);
    this.SetTransform();
    if (playSound)
    {
      Log.Sound.PrintDebug("ButtonListMenu Playing sound Small_Click");
      SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    }
    this.gameObject.SetActive(true);
    UniversalInputManager.Get().SetGameDialogActive(true);
    this.HideAllButtons();
    this.LayoutMenu();
    this.m_isShown = true;
    TransformUtil.SetLocalScaleToWorldDimension(this.m_menu.m_headerMiddle, new WorldDimensionIndex(this.m_menu.m_headerText.GetTextBounds().size.x, 0));
    this.m_menu.m_header.UpdateSlices();
    if (!this.m_showAnimation)
      return;
    AnimationUtil.ShowWithPunch(this.m_menu.gameObject, ButtonListMenu.HIDDEN_SCALE, this.PUNCH_SCALE * this.NORMAL_SCALE, this.NORMAL_SCALE, (string) null, true);
  }

  public virtual void Hide()
  {
    if ((Object) this.gameObject != (Object) null)
      this.gameObject.SetActive(false);
    UniversalInputManager.Get().SetGameDialogActive(false);
    this.m_isShown = false;
  }

  public bool IsShown() => this.m_isShown;

  public UIBButton CreateMenuButton(
    string name,
    string buttonTextString,
    UIEvent.Handler releaseHandler)
  {
    return this.CreateMenuButton(name, buttonTextString, releaseHandler, this.m_menu.m_templateButton);
  }

  public UIBButton CreateMenuButton(
    string name,
    string buttonTextString,
    UIEvent.Handler releaseHandler,
    UIBButton buttonTemplate)
  {
    UIBButton menuButton = (UIBButton) GameUtils.Instantiate((Component) buttonTemplate, this.m_menu.m_buttonContainer.gameObject);
    menuButton.SetText(GameStrings.Get(buttonTextString));
    if (name != null)
      menuButton.gameObject.name = name;
    menuButton.AddEventListener(UIEventType.RELEASE, releaseHandler);
    menuButton.transform.localRotation = buttonTemplate.transform.localRotation;
    this.m_allButtons.Add(menuButton);
    return menuButton;
  }

  public void DisableInputBlocker() => this.m_blocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBlockerRelease));

  protected abstract List<UIBButton> GetButtons();

  protected void SetTransform()
  {
    if ((Object) this.m_menuParent == (Object) null)
      this.m_menuParent = this.transform;
    TransformUtil.AttachAndPreserveLocalTransform(this.m_menu.transform, this.m_menuParent);
    if ((Object) this.m_blocker != (Object) null)
    {
      this.m_blocker.transform.localPosition = new Vector3(0.0f, -5f, 0.0f);
      this.m_blocker.transform.eulerAngles = new Vector3(90f, 0.0f, 0.0f);
    }
    LayerUtils.SetLayer((Component) this, this.m_targetLayer);
    this.m_menu.gameObject.transform.localScale = this.NORMAL_SCALE;
  }

  protected virtual void LayoutMenu()
  {
    this.LayoutMenuButtons();
    this.m_menu.m_buttonContainer.UpdateSlices();
    this.LayoutMenuBackground();
  }

  protected void LayoutMenuButtons()
  {
    List<UIBButton> buttons = this.GetButtons();
    this.m_menu.m_buttonContainer.ClearSlices();
    int index1 = 0;
    int index2 = 0;
    for (; index1 < buttons.Count; ++index1)
    {
      UIBButton uibButton = buttons[index1];
      Vector3 minLocalPadding = Vector3.zero;
      bool reverse = false;
      GameObject gameObject1;
      if ((Object) uibButton == (Object) null)
      {
        GameObject gameObject2;
        if (index2 >= this.m_horizontalDividers.Count)
        {
          gameObject2 = (GameObject) GameUtils.Instantiate(this.m_menu.m_templateHorizontalDivider, this.m_menu.m_buttonContainer.gameObject);
          gameObject2.transform.localRotation = this.m_menu.m_templateHorizontalDivider.transform.localRotation;
          this.m_horizontalDividers.Add(gameObject2);
        }
        else
          gameObject2 = this.m_horizontalDividers[index2];
        ++index2;
        gameObject1 = gameObject2;
        minLocalPadding = this.m_menu.m_horizontalDividerMinPadding;
      }
      else
        gameObject1 = uibButton.gameObject;
      this.m_menu.m_buttonContainer.AddSlice(gameObject1, minLocalPadding, Vector3.zero, reverse);
      gameObject1.SetActive(true);
    }
  }

  protected void LayoutMenuBackground()
  {
    OrientedBounds orientedWorldBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_menu.m_buttonContainer.gameObject);
    float width = orientedWorldBounds.Extents[0].magnitude * 2f;
    float height = orientedWorldBounds.Extents[2].magnitude * 2f;
    this.m_menu.m_background.SetSize(width, height);
    this.m_menu.m_border.SetSize(width, height);
  }

  protected static void MakeButtonRed(UIBButton button, Material materialOverride)
  {
    MultiSliceElement component = button.m_RootObject.GetComponent<MultiSliceElement>();
    if ((Object) component == (Object) null)
    {
      Error.AddDevFatal("ButtonListMenu.MakeButtonRed() - Attempting to make button red, but the button does not have a multi slice element component!");
    }
    else
    {
      foreach (MultiSliceElement.Slice slice1 in component.m_slices)
      {
        GameObject slice2 = slice1.m_slice;
        if ((Object) slice2 != (Object) null)
          RendererExtension.SetMaterial(slice2.GetComponent<Renderer>(), materialOverride);
      }
      if (!((Object) button.m_ButtonText != (Object) null))
        return;
      button.m_ButtonText.TextColor = Color.white;
    }
  }

  private void OnFatalError(FatalErrorMessage message, object userData)
  {
    if (SceneMgr.Get().GetNextMode() != SceneMgr.Mode.FATAL_ERROR)
      return;
    this.Hide();
  }

  private void HideAllButtons()
  {
    for (int index = 0; index < this.m_allButtons.Count; ++index)
      this.m_allButtons[index].gameObject.SetActive(false);
    for (int index = 0; index < this.m_horizontalDividers.Count; ++index)
      this.m_horizontalDividers[index].SetActive(false);
  }

  private void OnBlockerRelease(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    this.Hide();
  }
}
