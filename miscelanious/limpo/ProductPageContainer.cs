using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductPageContainer : MonoBehaviour
{
  [SerializeField]
  private GameObject m_pageRoot;
  private Widget m_widget;
  private ProductDataModel m_product = ProductFactory.CreateEmptyProductDataModel();
  private MusicPlaylistBookmark m_musicPlaylistBookmark;
  private MusicPlaylistType m_musicOverride;
  private readonly List<ProductPage> m_pages = new List<ProductPage>();
  private List<WidgetInstance> m_tempInstances = new List<WidgetInstance>();
  private ProductPage m_currentProductPage;
  private bool m_tempInstancesHaveBeenInitialized;
  private const string OPEN = "OPEN";
  private const string CLOSED = "CLOSED";
  private const string EVENT_DISMISS = "CODE_DISMISS";
  private const string EVENT_NO_MUSIC = "NO_MUSIC";
  private readonly PlatformDependentValue<bool> UnloadUnusedAssetsOnClose = new PlatformDependentValue<bool>(PlatformCategory.Memory)
  {
    LowMemory = true,
    MediumMemory = true,
    HighMemory = false
  };

  protected virtual void Awake()
  {
    if ((UnityEngine.Object) this.m_pageRoot != (UnityEngine.Object) null)
      this.m_pageRoot.SetActive(false);
    else
      Log.Store.PrintError("ProductPageContainer missing reference to product page root object. This may prevent pages from opening!");
  }

  protected virtual void Start()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (evt =>
    {
      if (!(evt == "CODE_DISMISS"))
        return;
      DynamicVideoLoader componentInChildren = this.GetComponentInChildren<DynamicVideoLoader>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
        componentInChildren.OnClosed();
      UIContext.GetRoot().DismissPopup(this.m_widget.gameObject);
      if (!((UnityEngine.Object) this.m_pageRoot != (UnityEngine.Object) null))
        return;
      this.m_pageRoot.SetActive(false);
    }));
    this.GetComponentsInChildren<WidgetInstance>(true, this.m_tempInstances);
    this.m_tempInstances.RemoveAll((Predicate<WidgetInstance>) (w => !w.name.Contains("[temp]")));
    Shop shop = Shop.Get();
    if (!((UnityEngine.Object) shop != (UnityEngine.Object) null))
      return;
    shop.OnOpened += new Action(this.HandleShopOpened);
    shop.OnCloseCompleted += new Action(this.HandleShopClosed);
  }

  protected virtual void OnDestroy()
  {
    Shop shop = Shop.Get();
    if ((UnityEngine.Object) shop != (UnityEngine.Object) null)
    {
      shop.OnOpened -= new Action(this.HandleShopOpened);
      shop.OnCloseCompleted -= new Action(this.HandleShopClosed);
    }
    DynamicVideoLoader componentInChildren = this.GetComponentInChildren<DynamicVideoLoader>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.OnClosed();
    foreach (ProductPage page in this.m_pages)
    {
      if (page != null)
      {
        page.OnOpened -= new EventHandler(this.HandleProductPageOpened);
        page.OnClosed -= new EventHandler(this.HandleProductPageClosed);
      }
    }
    this.m_pages.Clear();
    this.m_tempInstances = (List<WidgetInstance>) null;
  }

  public event EventHandler OnOpened;

  public event EventHandler OnClosed;

  public event Action OnProductSet;

  public bool IsOpen { get; private set; }

  public void Open() => this.Open(this.m_product, this.Variant);

  public void Open(ProductDataModel product, ProductDataModel variant = null)
  {
    if (product == null || product == ProductFactory.CreateEmptyProductDataModel())
    {
      Log.Store.PrintError("ProductPageContainer cannot open null or empty product");
    }
    else
    {
      this.gameObject.SetActive(true);
      if (this.IsOpen)
        return;
      if ((UnityEngine.Object) this.m_pageRoot == (UnityEngine.Object) null)
      {
        Log.Store.PrintError("ProductPageContainer missing reference to the product page root object");
      }
      else
      {
        this.m_pageRoot.SetActive(true);
        this.IsOpen = true;
        this.SetProduct(product, variant);
        if (this.m_musicOverride != MusicPlaylistType.Invalid)
          this.OverrideMusic(this.m_musicOverride);
        this.StartCoroutine(this.OpenProductPageCoroutine());
      }
    }
  }

  public void Close()
  {
    if (!this.IsOpen)
      return;
    if ((UnityEngine.Object) this.m_currentProductPage != (UnityEngine.Object) null)
    {
      if (this.m_currentProductPage.IsOpen)
      {
        this.m_currentProductPage.Close();
        return;
      }
      this.m_currentProductPage = (ProductPage) null;
    }
    this.IsOpen = false;
    this.StopMusicOverride();
    this.SetProduct((ProductDataModel) null);
    this.m_widget.TriggerEvent("CLOSED");
    if (this.OnClosed != null)
      this.OnClosed((object) this, new EventArgs());
    if (!(bool) this.UnloadUnusedAssetsOnClose || !((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null))
      return;
    HearthstoneApplication.Get().UnloadUnusedAssets();
  }

  public ProductPage GetCurrentProductPage() => this.m_currentProductPage;

  public void RegisterProductPage(ProductPage page)
  {
    this.m_pages.Add(page);
    page.OnOpened += new EventHandler(this.HandleProductPageOpened);
    page.OnClosed += new EventHandler(this.HandleProductPageClosed);
  }

  public void UnregisterProductPage(ProductPage page)
  {
    this.m_pages.Remove(page);
    page.OnOpened -= new EventHandler(this.HandleProductPageOpened);
    page.OnClosed -= new EventHandler(this.HandleProductPageClosed);
  }

  public void SetProduct(ProductDataModel product, ProductDataModel variant = null)
  {
    product = product ?? ProductFactory.CreateEmptyProductDataModel();
    this.Variant = variant ?? product;
    if (product == this.m_product)
      return;
    this.m_product = product;
    this.BindCurrentProduct();
    if (this.OnProductSet == null)
      return;
    this.OnProductSet();
  }

  public ProductDataModel Product => this.m_product;

  public ProductDataModel Variant { get; set; }

  [Overridable]
  public string MusicOverride
  {
    get => this.m_musicOverride.ToString();
    set
    {
      MusicPlaylistType playlist = MusicPlaylistType.Invalid;
      if (!string.IsNullOrEmpty(value))
      {
        try
        {
          object obj = Enum.Parse(typeof (MusicPlaylistType), value, true);
          if (obj != null)
            playlist = (MusicPlaylistType) obj;
        }
        catch (Exception ex)
        {
          Debug.LogErrorFormat("Invalid playlist name '{0}'", (object) value);
        }
      }
      this.OverrideMusic(playlist);
    }
  }

  public void OverrideMusic(MusicPlaylistType playlist)
  {
    if (this.m_musicOverride == playlist)
      return;
    this.m_musicOverride = playlist;
    if (!this.IsOpen)
      return;
    if (this.m_musicOverride == MusicPlaylistType.Invalid)
    {
      this.StopMusicOverride();
    }
    else
    {
      if (this.m_musicPlaylistBookmark == null)
        this.m_musicPlaylistBookmark = MusicManager.Get().CreateBookmarkOfCurrentPlaylist();
      MusicManager.Get().StartPlaylist(this.m_musicOverride);
    }
  }

  public void StopMusicOverride()
  {
    if (this.m_musicPlaylistBookmark != null)
    {
      MusicManager musicManager = MusicManager.Get();
      if (musicManager != null)
      {
        musicManager.StopPlaylist();
        musicManager.PlayFromBookmark(this.m_musicPlaylistBookmark);
      }
      this.m_musicPlaylistBookmark = (MusicPlaylistBookmark) null;
    }
    this.m_musicOverride = MusicPlaylistType.Invalid;
  }

  public void InitializeTempInstances()
  {
    if (this.m_tempInstancesHaveBeenInitialized)
      return;
    this.m_tempInstancesHaveBeenInitialized = true;
    foreach (WidgetInstance tempInstance in this.m_tempInstances)
      this.ForceInitializeTempInstance(tempInstance);
  }

  protected void BindCurrentProduct() => this.m_widget.BindDataModel((IDataModel) this.m_product);

  protected void HandleProductPageOpened(object sender, EventArgs e)
  {
    ProductPage productPage = sender as ProductPage;
    if ((UnityEngine.Object) this.m_currentProductPage != (UnityEngine.Object) null)
      Log.Store.PrintError("Previous product page did not close properly: {0}", (object) this.m_currentProductPage.gameObject.name);
    PopupDisplayManager.Get().RedundantNDERerollPopups.SuppressNDEPopups = true;
    this.m_currentProductPage = productPage;
  }

  protected void HandleProductPageClosed(object sender, EventArgs e)
  {
    ProductPage productPage = sender as ProductPage;
    if ((UnityEngine.Object) this.m_currentProductPage == (UnityEngine.Object) productPage)
    {
      this.m_currentProductPage = (ProductPage) null;
      this.Close();
    }
    else
      Log.Store.PrintError("Product page closed but it is not the currently open page: {0}", (object) productPage.gameObject.name);
    PopupDisplayManager.Get().RedundantNDERerollPopups.SuppressNDEPopups = false;
  }

  protected void HandleShopOpened()
  {
    foreach (WidgetInstance tempInstance in this.m_tempInstances)
      this.StartCoroutine(this.PreloadPageInstanceCoroutine(tempInstance));
  }

  protected IEnumerator PreloadPageInstanceCoroutine(WidgetInstance instance)
  {
    yield return (object) new WaitForSeconds(0.1f);
    Shop shop = Shop.Get();
    while ((UnityEngine.Object) shop != (UnityEngine.Object) null && !shop.Browser.IsReady() && shop.IsOpen())
      yield return (object) null;
    if (!((UnityEngine.Object) shop == (UnityEngine.Object) null) && shop.IsOpen())
    {
      instance.Initialize();
      bool wasActive = instance.gameObject.activeSelf;
      instance.gameObject.SetActive(true);
      yield return (object) null;
      instance.gameObject.SetActive(wasActive);
    }
  }

  protected void ForceInitializeTempInstance(WidgetInstance instance)
  {
    instance.Initialize();
    instance.gameObject.SetActive(true);
  }

  protected void HandleShopClosed()
  {
    this.m_tempInstancesHaveBeenInitialized = false;
    this.SetProduct((ProductDataModel) null);
    this.m_tempInstances.ForEach((Action<WidgetInstance>) (i => i.Unload()));
    this.m_pages.Clear();
  }

  protected IEnumerator OpenProductPageCoroutine()
  {
    ProductPageContainer sender = this;
    while (sender.m_widget.IsChangingStates && sender.IsOpen)
      yield return (object) null;
    if (sender.IsOpen)
    {
      UIContext.GetRoot().ShowPopup(sender.m_widget.gameObject, projection: UIContext.ProjectionType.Perspective);
      WidgetInstance activeInstance = sender.m_tempInstances.FirstOrDefault<WidgetInstance>((Func<WidgetInstance, bool>) (i => i.gameObject.activeInHierarchy));
      if ((UnityEngine.Object) activeInstance == (UnityEngine.Object) null)
      {
        Log.Store.PrintError("Failed to activate any product page for data model.");
        sender.Close();
      }
      else
      {
        activeInstance.Initialize();
        while (sender.IsOpen && (!activeInstance.IsReady || activeInstance.IsChangingStates))
          yield return (object) null;
        ProductPage activePage = sender.m_pages.FirstOrDefault<ProductPage>((Func<ProductPage, bool>) (p => p.gameObject.activeInHierarchy));
        if ((UnityEngine.Object) activePage == (UnityEngine.Object) null)
        {
          Log.Store.PrintError("Failed to instantiate any product page for data model.");
          sender.Close();
        }
        else
        {
          activePage.Open();
          while (activePage.WidgetComponent.IsChangingStates && sender.IsOpen)
            yield return (object) null;
          if (sender.IsOpen)
          {
            sender.m_widget.TriggerEvent("OPEN");
            EventHandler onOpened = sender.OnOpened;
            if (onOpened != null)
              onOpened((object) sender, EventArgs.Empty);
          }
        }
      }
    }
  }
}
