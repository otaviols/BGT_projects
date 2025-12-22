using System.Collections.Generic;

[CustomEditClass]
public class AdventurePurchaseScreen : Store
{
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_BuyDungeonButton;
  private List<AdventurePurchaseScreen.PurchaseListener> m_PurchaseListeners = new List<AdventurePurchaseScreen.PurchaseListener>();

  protected override void Awake()
  {
    base.Awake();
    this.m_buyWithMoneyButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.BuyWithMoney()));
    this.m_buyWithGoldButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.BuyWithGold()));
    this.m_BuyDungeonButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.SendToStore()));
  }

  public void AddPurchaseListener(AdventurePurchaseScreen.Purchase dlg, object userdata)
  {
    AdventurePurchaseScreen.PurchaseListener purchaseListener = new AdventurePurchaseScreen.PurchaseListener();
    purchaseListener.SetCallback(dlg);
    purchaseListener.SetUserData(userdata);
    this.m_PurchaseListeners.Add(purchaseListener);
  }

  public void RemovePurchaseListener(AdventurePurchaseScreen.Purchase dlg)
  {
    foreach (AdventurePurchaseScreen.PurchaseListener purchaseListener in this.m_PurchaseListeners)
    {
      if (purchaseListener.GetCallback() == dlg)
      {
        this.m_PurchaseListeners.Remove(purchaseListener);
        break;
      }
    }
  }

  private void BuyWithMoney() => this.FirePurchaseEvent(true);

  private void BuyWithGold() => this.FirePurchaseEvent(true);

  private void SendToStore() => this.FirePurchaseEvent(false);

  private void FirePurchaseEvent(bool success)
  {
    foreach (AdventurePurchaseScreen.PurchaseListener purchaseListener in this.m_PurchaseListeners.ToArray())
      purchaseListener.Fire(success);
  }

  protected override void ShowImpl(bool isTotallyFake) => this.FireOpenedEvent();

  public delegate void Purchase(bool success, object userdata);

  public class PurchaseListener : EventListener<AdventurePurchaseScreen.Purchase>
  {
    public void Fire(bool success) => this.m_callback(success, this.m_userData);
  }
}
