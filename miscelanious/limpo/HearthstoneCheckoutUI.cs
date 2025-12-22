using Blizzard.Commerce;
using Hearthstone;
using UnityEngine;

public class HearthstoneCheckoutUI : UIBPopup
{
  [CustomEditField(Sections = "Checkout Configuration")]
  public int m_MeshWidth = 64;
  [CustomEditField(Sections = "Checkout Configuration")]
  public int m_MeshHeight = 48;
  [CustomEditField(Sections = "Checkout Configuration")]
  public float m_MeshScaleMinBound = 0.6f;
  [CustomEditField(Sections = "Checkout Configuration")]
  public Vector3 m_BrowserMeshPosition = Vector3.zero;
  [CustomEditField(Sections = "Checkout Configuration")]
  public Vector3 m_BrowserMeshRotation = Vector3.zero;
  [CustomEditField(Sections = "Checkout Configuration")]
  public float m_BrowserMeshScale = 1f;
  [CustomEditField(Sections = "Checkout Configuration")]
  public float m_BrowserResolutionScale = 1f;
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_OffClickCatcher;
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_ConsoleButton;
  private CheckoutMesh m_checkoutMesh;
  private CheckoutInputManager m_checkoutInput;

  private event HearthstoneCheckoutUI.OutsideClickListener m_outsideClickEvent;

  public int BrowserWidth { get; private set; }

  public int BrowserHeight { get; private set; }

  public IScreenSpace ScreenSpace => (IScreenSpace) this.m_checkoutMesh;

  public bool HasCheckoutMesh => (Object) this.m_checkoutMesh != (Object) null;

  protected override void Awake()
  {
    base.Awake();
    this.m_destroyOnSceneLoad = false;
    if ((Object) this.m_OffClickCatcher != (Object) null)
      this.m_OffClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOutsideClick));
    if (!((Object) this.m_ConsoleButton != (Object) null))
      return;
    if (!HearthstoneCheckoutUI.ShouldStreamBrowserTexture() && HearthstoneApplication.IsInternal())
    {
      this.m_ConsoleButton.gameObject.SetActive(true);
      this.m_ConsoleButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBugReporterClick));
    }
    else
      this.m_ConsoleButton.gameObject.SetActive(false);
  }

  private void Update()
  {
    if (!Application.isEditor)
      return;
    this.UpdateMeshTransform();
  }

  public override void Show()
  {
    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("ShopVideoPreview"))
    {
      DynamicVideoLoader component = gameObject.GetComponent<DynamicVideoLoader>();
      if ((Object) component != (Object) null)
        component.VideoPlayer.Pause();
    }
    base.Show();
    if (!((Object) this.m_checkoutInput != (Object) null))
      return;
    this.m_checkoutInput.IsActive = true;
  }

  public void GenerateMeshes()
  {
    this.DetermineBrowserSize();
    if (!HearthstoneCheckoutUI.ShouldStreamBrowserTexture())
      return;
    if ((Object) this.m_checkoutMesh == (Object) null)
    {
      this.m_checkoutMesh = CheckoutMesh.GenerateCheckoutMesh(this.BrowserWidth, this.BrowserHeight, (float) this.m_MeshWidth, (float) this.m_MeshHeight);
      this.m_checkoutMesh.gameObject.AddComponent<PegUIElement>();
      this.m_checkoutMesh.transform.SetParent(this.transform, false);
      this.m_checkoutMesh.gameObject.layer = this.gameObject.layer;
      this.m_checkoutInput = this.m_checkoutMesh.gameObject.AddComponent<CheckoutInputManager>();
      this.m_checkoutInput.AddKeyboardEventListener(KeyCode.Escape, (CheckoutInputManager.KeyboardEventListener) (onKeyDown =>
      {
        if (onKeyDown)
          return;
        this.OnOutsideClick((UIEvent) null);
      }));
      this.UpdateMeshTransform();
    }
    else
      this.m_checkoutMesh.ResizeTexture(this.BrowserWidth, this.BrowserHeight);
    this.UpdateTextureHandle();
  }

  public void InitiateCheckout(HearthstoneCheckout checkoutClient)
  {
    if (checkoutClient == null || !this.HasCheckoutMesh)
      return;
    if (!CommerceWrapper.Instance.SendResizeEvent(this.BrowserWidth, this.BrowserHeight))
      Log.Store.PrintWarning("[HearthstoneCheckoutUI.InitiateCheckout] SendResizeEvent failed.");
    if (!((Object) this.m_checkoutInput != (Object) null))
      return;
    this.m_checkoutInput.Setup(checkoutClient, (IScreenSpace) this.m_checkoutMesh);
  }

  public void ResizeTexture(int width, int height)
  {
    if (!((Object) this.m_checkoutMesh != (Object) null))
      return;
    this.m_checkoutMesh.ResizeTexture(width, height);
    this.UpdateTextureHandle();
  }

  public void UpdateTexture(byte[] buffer)
  {
    if (!((Object) this.m_checkoutMesh != (Object) null))
      return;
    this.m_checkoutMesh.UpdateTexture(buffer);
  }

  public void DetermineBrowserSize()
  {
    if (!HearthstoneCheckoutUI.ShouldStreamBrowserTexture())
    {
      this.BrowserWidth = (int) ((double) Screen.height * (double) this.m_BrowserResolutionScale);
      this.BrowserHeight = (int) ((double) Screen.width * (double) this.m_BrowserResolutionScale);
    }
    else
    {
      if ((double) Screen.height > 1080.0)
      {
        this.m_BrowserMeshScale = Mathf.Max(864f / (float) Screen.height, this.m_MeshScaleMinBound);
        float num = 864f / this.m_BrowserMeshScale;
        this.BrowserWidth = (int) ((double) num * 1.5 * (double) this.m_BrowserResolutionScale * (double) this.m_BrowserMeshScale);
        this.BrowserHeight = (int) ((double) num * (double) this.m_BrowserResolutionScale * (double) this.m_BrowserMeshScale);
      }
      else
      {
        this.m_BrowserMeshScale = 0.8f;
        this.BrowserWidth = (int) ((double) Screen.height * 1.5 * (double) this.m_BrowserResolutionScale * (double) this.m_BrowserMeshScale);
        this.BrowserHeight = (int) ((double) Screen.height * (double) this.m_BrowserResolutionScale * (double) this.m_BrowserMeshScale);
      }
      this.UpdateMeshTransform();
    }
    Log.Store.PrintDebug("[DetermineBrowserSize] Height: " + (object) this.BrowserHeight + " Width: " + (object) this.BrowserWidth);
  }

  public void AddOutsideClickListener(
    HearthstoneCheckoutUI.OutsideClickListener listener)
  {
    this.m_outsideClickEvent -= listener;
    this.m_outsideClickEvent += listener;
  }

  public void RemoveOutsideClickListener(
    HearthstoneCheckoutUI.OutsideClickListener listener)
  {
    this.m_outsideClickEvent -= listener;
  }

  public void HandleCommerceReadyEvent()
  {
    if (!((Object) this.m_checkoutInput != (Object) null))
      return;
    this.m_checkoutInput.IsActive = this.IsShown();
  }

  private void UpdateMeshTransform()
  {
    if (!((Object) this.m_checkoutMesh != (Object) null))
      return;
    Transform transform = this.m_checkoutMesh.transform;
    transform.localPosition = this.m_BrowserMeshPosition * this.m_BrowserMeshScale;
    transform.localRotation = Quaternion.Euler(this.m_BrowserMeshRotation);
    transform.localScale = new Vector3(this.m_BrowserMeshScale, this.m_BrowserMeshScale, this.m_BrowserMeshScale);
  }

  private void OnOutsideClick(UIEvent e)
  {
    if (this.m_outsideClickEvent == null)
      return;
    this.m_outsideClickEvent();
  }

  private void OnBugReporterClick(UIEvent e) => CheatMgr.Get()?.ShowConsole();

  private void UpdateTextureHandle()
  {
  }

  private static bool ShouldStreamBrowserTexture() => Application.isEditor || !PlatformSettings.IsMobileRuntimeOS;

  protected override void Hide(bool animate)
  {
    if ((Object) this.m_checkoutInput != (Object) null)
      this.m_checkoutInput.IsActive = false;
    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("ShopVideoPreview"))
    {
      DynamicVideoLoader component = gameObject.GetComponent<DynamicVideoLoader>();
      if ((Object) component != (Object) null)
        component.VideoPlayer.Play();
    }
    base.Hide(animate);
  }

  public delegate void OutsideClickListener();
}
