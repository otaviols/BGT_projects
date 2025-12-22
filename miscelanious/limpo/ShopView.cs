using System;
using System.Collections.Generic;
using System.Linq;

public class ShopView
{
  private readonly List<ShopView.IComponent> m_components;

  public PurchaseAuthView PurchaseAuth => this.FindComponent<PurchaseAuthView>();

  public SummaryView Summary => this.FindComponent<SummaryView>();

  public SendToBamView SendToBam => this.FindComponent<SendToBamView>();

  public LegalBamView LegalBam => this.FindComponent<LegalBamView>();

  public DoneWithBamView DoneWithBam => this.FindComponent<DoneWithBamView>();

  public ChallengePromptView ChallengePrompt => this.FindComponent<ChallengePromptView>();

  public bool HasStartedLoading { get; private set; }

  public ShopView() => this.m_components = new List<ShopView.IComponent>()
  {
    (ShopView.IComponent) this.InitializeComponent<PurchaseAuthView>(),
    (ShopView.IComponent) this.InitializeComponent<SummaryView>(),
    (ShopView.IComponent) this.InitializeComponent<SendToBamView>(),
    (ShopView.IComponent) this.InitializeComponent<LegalBamView>(),
    (ShopView.IComponent) this.InitializeComponent<DoneWithBamView>(),
    (ShopView.IComponent) this.InitializeComponent<ChallengePromptView>()
  };

  public event Action OnComponentReady = () => { };

  public bool IsLoaded()
  {
    foreach (ShopView.IComponent component in this.m_components)
    {
      if (!component.IsLoaded)
        return false;
    }
    return true;
  }

  public bool IsPromptShowing()
  {
    int index = 0;
    for (int count = this.m_components.Count; index < count; ++index)
    {
      if (this.m_components[index].IsShown)
        return true;
    }
    return false;
  }

  public void LoadAssets()
  {
    if (this.HasStartedLoading)
      return;
    IAssetLoader assetLoader = AssetLoader.Get();
    this.m_components.ForEach((Action<ShopView.IComponent>) (component => component.Load(assetLoader)));
    this.HasStartedLoading = true;
  }

  public void UnloadAssets()
  {
    this.m_components.ForEach((Action<ShopView.IComponent>) (component => component.Unload()));
    this.HasStartedLoading = false;
  }

  public void Hide() => this.m_components.ForEach((Action<ShopView.IComponent>) (component => component.Hide()));

  private T InitializeComponent<T>() where T : ShopView.IComponent, new()
  {
    T obj = new T();
    obj.OnComponentReady += new Action(this.HandleComponentReady);
    return obj;
  }

  private void HandleComponentReady() => this.OnComponentReady();

  private T FindComponent<T>() where T : class, ShopView.IComponent => this.m_components.FirstOrDefault<ShopView.IComponent>((Func<ShopView.IComponent, bool>) (component => component is T)) as T;

  public interface IComponent
  {
    bool IsLoaded { get; }

    bool IsShown { get; }

    event Action OnComponentReady;

    void Load(IAssetLoader assetLoader);

    void Unload();

    void Hide();
  }
}
