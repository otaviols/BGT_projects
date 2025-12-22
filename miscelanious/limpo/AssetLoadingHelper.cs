using System;
using UnityEngine;

public class AssetLoadingHelper
{
  private int m_AssetsLoading;

  public event EventHandler AssetLoadingComplete;

  public int AssetsLoading => this.m_AssetsLoading;

  public bool AddAssetToLoad(int assetCount = 1)
  {
    this.m_AssetsLoading += assetCount;
    return true;
  }

  public void AssetLoadCompleted()
  {
    if (this.m_AssetsLoading > 0)
    {
      --this.m_AssetsLoading;
      if (this.m_AssetsLoading != 0 || this.AssetLoadingComplete == null)
        return;
      this.AssetLoadingComplete((object) this, EventArgs.Empty);
    }
    else
      Debug.LogError((object) "AssetLoadCompleted() called when no assets left.");
  }
}
