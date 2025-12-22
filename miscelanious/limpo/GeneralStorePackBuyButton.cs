using Blizzard.T5.MaterialService.Extensions;
using Shared.Scripts.Util.ValueTypes;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStorePackBuyButton : PegUIElement
{
  public UberText m_quantityText;
  public UberText m_costText;
  public UberText m_fullText;
  public Color m_goldQuantityTextColor;
  public Color m_moneyQuantityTextColor;
  public Color m_moneyQuantityBonusPacksTextColor;
  public int m_moneyQuantityBonusPacksTextOutlineSize;
  public Color m_goldCostTextColor;
  public Color m_moneyCostTextColor;
  public GameObject m_goldIcon;
  public GameObject m_selectGlow;
  public List<Renderer> m_buttonRenderers = new List<Renderer>();
  public int m_materialIndex;
  public string m_materialPropName = "_MainTex";
  public Vector2 m_goldBtnMatOffset;
  public Vector2 m_goldBtnDownMatOffset;
  public Vector2 m_moneyBtnMatOffset;
  public Vector2 m_moneyBtnDownMatOffset;
  private bool m_selected;
  private bool m_isGold;

  public bool IsSelected() => this.m_selected;

  public void Select()
  {
    if (this.m_selected)
      return;
    this.m_selected = true;
    this.UpdateButtonState();
  }

  public void Unselect()
  {
    if (!this.m_selected)
      return;
    this.m_selected = false;
    this.UpdateButtonState();
  }

  public void UpdateFromGTAPP(NoGTAPPTransactionData noGTAPPGoldPrice)
  {
    string quantityText = string.Empty;
    long cost;
    if (StoreManager.Get().GetGoldCostNoGTAPP(noGTAPPGoldPrice, out cost))
      quantityText = StoreManager.Get().GetProductQuantityText(noGTAPPGoldPrice.Product, noGTAPPGoldPrice.ProductData, noGTAPPGoldPrice.Quantity, 0);
    this.SetGoldValue(cost, quantityText);
  }

  public void SetGoldValue(long goldCost, string quantityText)
  {
    if ((Object) this.m_fullText != (Object) null)
    {
      this.m_quantityText.gameObject.SetActive(true);
      this.m_costText.gameObject.SetActive(true);
      this.m_fullText.gameObject.SetActive(false);
    }
    this.m_costText.Text = goldCost.ToString();
    this.m_costText.TextColor = this.m_goldCostTextColor;
    this.m_quantityText.Text = quantityText;
    this.m_quantityText.TextColor = this.m_goldQuantityTextColor;
    this.m_isGold = true;
    this.UpdateButtonState();
  }

  public void SetMoneyValue(
    Network.Bundle bundle,
    Network.BundleItem packsBundleItem,
    string quantityText)
  {
    if ((Record) bundle != (Record) null && !StoreManager.Get().IsProductAlreadyOwned(bundle))
    {
      if ((Object) this.m_fullText != (Object) null)
      {
        this.m_quantityText.gameObject.SetActive(true);
        this.m_costText.gameObject.SetActive(true);
        this.m_fullText.gameObject.SetActive(false);
      }
      this.m_costText.Text = StoreManager.Get().FormatCostBundle(bundle);
      this.m_costText.TextColor = this.m_moneyCostTextColor;
      this.m_costText.Outline = false;
      this.m_quantityText.Text = quantityText;
      this.m_quantityText.TextColor = this.m_moneyQuantityTextColor;
      this.m_quantityText.Outline = false;
      if ((Record) packsBundleItem != (Record) null && packsBundleItem.BaseQuantity > 0)
      {
        this.m_quantityText.TextColor = this.m_moneyQuantityBonusPacksTextColor;
        this.m_quantityText.Outline = true;
        this.m_quantityText.OutlineSize = (float) this.m_moneyQuantityBonusPacksTextOutlineSize;
      }
    }
    else
    {
      this.m_costText.Text = string.Empty;
      UberText uberText = this.m_quantityText;
      if ((Object) this.m_fullText != (Object) null)
      {
        this.m_quantityText.gameObject.SetActive(false);
        this.m_costText.gameObject.SetActive(false);
        this.m_fullText.gameObject.SetActive(true);
        uberText = this.m_fullText;
      }
      uberText.Text = GameStrings.Get("GLUE_STORE_PACK_BUTTON_TEXT_PURCHASED");
    }
    this.m_isGold = false;
    this.UpdateButtonState();
  }

  private void UpdateButtonState()
  {
    if ((Object) this.m_goldIcon != (Object) null)
      this.m_goldIcon.SetActive(this.m_isGold);
    Vector2 zero = Vector2.zero;
    Vector2 vector2 = !this.m_isGold ? (this.m_selected ? this.m_moneyBtnDownMatOffset : this.m_moneyBtnMatOffset) : (this.m_selected ? this.m_goldBtnDownMatOffset : this.m_goldBtnMatOffset);
    foreach (Renderer buttonRenderer in this.m_buttonRenderers)
      buttonRenderer.GetMaterial(this.m_materialIndex).SetTextureOffset(this.m_materialPropName, vector2);
    if (!((Object) this.m_selectGlow != (Object) null))
      return;
    this.m_selectGlow.SetActive(this.m_selected);
  }

  protected override void OnDoubleClick()
  {
  }
}
