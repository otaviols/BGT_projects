using Hearthstone.Commerce;
using UnityEngine;

public class StoreMiniSummary : MonoBehaviour
{
  public UberText m_headlineText;
  public UberText m_itemsHeadlineText;
  public UberText m_itemsText;

  private void Awake()
  {
    this.m_headlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_HEADLINE");
    this.m_itemsHeadlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_ITEMS_ORDERED_HEADLINE");
  }

  public void SetDetails(ProductId productId, int quantity) => this.m_itemsText.Text = this.GetItemsText(productId, quantity);

  private string GetItemsText(ProductId productId, int quantity) => GameStrings.Format("GLUE_STORE_SUMMARY_ITEM_ORDERED", (object) quantity, (object) this.GetProductName(productId));

  private string GetProductName(ProductId productId) => StoreManager.Get().GetProductName(StoreManager.Get().GetBundleFromPmtProductId(new long?(productId.Value))) ?? GameStrings.Get("GLUE_STORE_PRODUCT_NAME_MOBILE_UNKNOWN");
}
