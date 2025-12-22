using Hearthstone.UI.Core;
using UnityEngine;

public class StoreCollectionManagerCardInfoLoader : MonoBehaviour
{
  [Tooltip("Optional: When provided will set parent to loaded card info, when empty will load to world position as set in loaded asset.")]
  [SerializeField]
  private BindLegendaryHeroToMaterial m_bindLegendaryToMaterial;
  private StoreItemDisplayDef m_loadedStoreDef;
  private string m_cardId;

  [Overridable]
  public string CardId
  {
    get => this.m_cardId;
    set
    {
      if (this.m_cardId == value)
        return;
      if (string.IsNullOrEmpty(value))
      {
        this.Cleanup();
      }
      else
      {
        this.m_cardId = value;
        if (this.TryLoadObjectFromCardInfo())
          return;
        this.Cleanup();
      }
    }
  }

  private void Cleanup()
  {
    this.m_cardId = string.Empty;
    if ((Object) this.m_bindLegendaryToMaterial != (Object) null)
      this.m_bindLegendaryToMaterial.Cleanup();
    if (!((Object) this.m_loadedStoreDef != (Object) null))
      return;
    Object.Destroy((Object) this.m_loadedStoreDef.gameObject);
    this.m_loadedStoreDef = (StoreItemDisplayDef) null;
  }

  private bool TryLoadObjectFromCardInfo()
  {
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(this.m_cardId))
    {
      if ((Object) cardDef?.CardDef == (Object) null)
      {
        Log.Store.PrintError("StoreCollectionManagerCardInfoLoader: Failed to get card def for card id: " + this.m_cardId);
        return false;
      }
      if (string.IsNullOrEmpty(cardDef.CardDef.m_StoreItemDisplayPath))
        return false;
      if ((Object) this.m_bindLegendaryToMaterial == (Object) null)
      {
        Log.Store.PrintError("StoreCollectionManagerCardInfoLoader: Failed to use StoreItemDisplayDef data due to missing BindLegendaryHeroToMaterial component!");
        return false;
      }
      if ((Object) this.m_loadedStoreDef != (Object) null)
        Object.Destroy((Object) this.m_loadedStoreDef.gameObject);
      this.m_loadedStoreDef = GameUtils.LoadGameObjectWithComponent<StoreItemDisplayDef>(cardDef.CardDef.m_StoreItemDisplayPath);
      if ((Object) this.m_loadedStoreDef == (Object) null)
      {
        Log.Store.PrintError("StoreCollectionManagerCardInfoLoader: Failed to pull StoreItemDisplayDef for card " + this.m_cardId + "!");
        return false;
      }
      if (string.IsNullOrEmpty(this.m_loadedStoreDef.m_CustomCMPortraitScene))
      {
        Log.Store.PrintWarning("StoreCollectionManagerCardInfoLoader: Failed to loaded CM scene to store as card:" + this.m_cardId + " has no info prefab set in StoreItemDisplayDef!");
        return false;
      }
      this.m_bindLegendaryToMaterial.LegendaryHeroPrefab = this.m_loadedStoreDef.m_CustomCMPortraitScene;
      this.m_bindLegendaryToMaterial.BindMaterial();
      Object.Destroy((Object) this.m_loadedStoreDef.gameObject);
      this.m_loadedStoreDef = (StoreItemDisplayDef) null;
      return true;
    }
  }
}
