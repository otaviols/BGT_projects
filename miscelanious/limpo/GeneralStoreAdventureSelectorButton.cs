using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStoreAdventureSelectorButton : PegUIElement
{
  public UberText m_adventureTitle;
  public HighlightState m_highlight;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_selectSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_unselectSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_mouseOverSound;
  public TooltipZone m_unavailableTooltip;
  public GameLayer m_unavailableTooltipLayer = GameLayer.PerspectiveUI;
  public float m_unavailableTooltipScale = 20f;
  public GameObject m_preorderRibbon;
  private bool m_selected;
  private AdventureDbId m_adventureId;

  public void SetAdventureId(AdventureDbId adventureId)
  {
    if ((Object) this.m_adventureTitle != (Object) null)
    {
      AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) adventureId);
      if (record != null)
        this.m_adventureTitle.Text = (string) record.StoreBuyButtonLabel;
    }
    this.m_adventureId = adventureId;
    this.UpdateState();
  }

  public AdventureDbId GetAdventureId() => this.m_adventureId;

  public void Select()
  {
    if (this.m_selected)
      return;
    this.m_selected = true;
    this.m_highlight.ChangeState(this.GetInteractionState() == PegUIElement.InteractionState.Up ? ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    if (string.IsNullOrEmpty(this.m_selectSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_selectSound);
  }

  public void Unselect()
  {
    if (!this.m_selected)
      return;
    this.m_selected = false;
    this.m_highlight.ChangeState(ActorStateType.NONE);
    if (string.IsNullOrEmpty(this.m_unselectSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_unselectSound);
  }

  public bool IsPrePurchase()
  {
    Network.Bundle bundle = (Network.Bundle) null;
    StoreManager.Get().GetAvailableAdventureBundle(this.m_adventureId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
    return (Record) bundle != (Record) null && StoreManager.Get().IsProductPrePurchase(bundle);
  }

  public void UpdateState()
  {
    if (!((Object) this.m_preorderRibbon != (Object) null))
      return;
    this.m_preorderRibbon.SetActive(this.IsPrePurchase());
  }

  public bool IsPurchasable()
  {
    ProductType adventureProductType = StoreManager.GetAdventureProductType(this.m_adventureId);
    if (adventureProductType == ProductType.PRODUCT_TYPE_UNKNOWN)
      return false;
    List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAvailableBundlesForProduct(adventureProductType, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION);
    return bundlesForProduct != null && bundlesForProduct.Count > 0;
  }

  public bool IsAvailable()
  {
    Network.Bundle bundle;
    StoreManager.Get().GetAvailableAdventureBundle(this.m_adventureId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
    return (Record) bundle != (Record) null;
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    base.OnOver(oldState);
    if (this.IsAvailable())
    {
      this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
      if (string.IsNullOrEmpty(this.m_mouseOverSound))
        return;
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_mouseOverSound);
    }
    else
    {
      if (!((Object) this.m_unavailableTooltip != (Object) null))
        return;
      LayerUtils.SetLayer((Component) this.m_unavailableTooltip.ShowTooltip(GameStrings.Get("GLUE_STORE_ADVENTURE_BUTTON_UNAVAILABLE_HEADLINE"), GameStrings.Get("GLUE_STORE_ADVENTURE_BUTTON_UNAVAILABLE_DESCRIPTION"), this.m_unavailableTooltipScale), this.m_unavailableTooltipLayer);
    }
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    base.OnOut(oldState);
    if (this.IsAvailable())
    {
      this.m_highlight.ChangeState(this.m_selected ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.NONE);
    }
    else
    {
      if (!((Object) this.m_unavailableTooltip != (Object) null))
        return;
      this.m_unavailableTooltip.HideTooltip();
    }
  }

  protected override void OnRelease()
  {
    base.OnRelease();
    if (!this.IsAvailable())
      return;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
  }

  protected override void OnPress()
  {
    base.OnPress();
    if (!this.IsAvailable())
      return;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }
}
