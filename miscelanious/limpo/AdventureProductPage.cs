using Blizzard.T5.AssetManager;
using Hearthstone.DataModels;
using System;
using System.Linq;
using UnityEngine;

public class AdventureProductPage : ProductPage
{
  public override void Open()
  {
    if ((UnityEngine.Object) this.m_container != (UnityEngine.Object) null)
      this.m_container.OverrideMusic(MusicPlaylistType.Invalid);
    base.Open();
  }

  protected override void OnProductSet()
  {
    base.OnProductSet();
    RewardItemDataModel rewardItemDataModel = this.Product.Items.FirstOrDefault<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (item => item.ItemType == RewardItemType.ADVENTURE));
    if (rewardItemDataModel == null)
    {
      Log.Store.PrintError("No Adventures in Product \"{0}\"", (object) this.Product.Name);
    }
    else
    {
      using (AssetHandle<GameObject> assetHandle = ShopUtils.LoadStoreAdventurePrefab((AdventureDbId) rewardItemDataModel.ItemId))
      {
        StoreAdventureDef storeAdventureDef = (bool) assetHandle ? assetHandle.Asset.GetComponent<StoreAdventureDef>() : (StoreAdventureDef) null;
        if (!((UnityEngine.Object) this.m_container != (UnityEngine.Object) null))
          return;
        this.m_container.OverrideMusic((bool) (UnityEngine.Object) storeAdventureDef ? storeAdventureDef.GetPlaylist() : MusicPlaylistType.Invalid);
      }
    }
  }
}
