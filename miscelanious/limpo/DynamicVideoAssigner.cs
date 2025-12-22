using Hearthstone.UI.Core;
using System.Collections.Generic;
using UnityEngine;

public class DynamicVideoAssigner : MonoBehaviour
{
  [SerializeField]
  private string m_VideoIdToLoad;
  [SerializeField]
  private DynamicVideoLoader m_DynamicVideo;
  [SerializeField]
  private DynamicVideoCaptionDriver m_dynamicCaptionDriver;
  private StoreItemDisplayDef m_LoadedStoreDef;
  private string m_CardId;

  [Overridable]
  public string CardId
  {
    get => this.m_CardId;
    set
    {
      if (string.IsNullOrEmpty(value) || (Object) this.m_DynamicVideo == (Object) null)
      {
        this.Cleanup();
      }
      else
      {
        this.m_CardId = value;
        if (this.TryLoadVideosOnCardSet())
          return;
        this.Cleanup();
      }
    }
  }

  private void Cleanup()
  {
    this.m_CardId = string.Empty;
    if ((Object) this.m_LoadedStoreDef != (Object) null)
    {
      Object.Destroy((Object) this.m_LoadedStoreDef.gameObject);
      this.m_LoadedStoreDef = (StoreItemDisplayDef) null;
    }
    if ((Object) this.m_DynamicVideo != (Object) null)
    {
      this.m_DynamicVideo.OnClosed();
      this.m_DynamicVideo.VideoLocation = string.Empty;
      this.m_DynamicVideo.FallbackTextureLocation = string.Empty;
    }
    if (!((Object) this.m_dynamicCaptionDriver != (Object) null))
      return;
    this.m_dynamicCaptionDriver.VideoCaptionKeys = (List<VideoCaptionKey>) null;
  }

  private bool TryLoadVideosOnCardSet()
  {
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(this.m_CardId))
    {
      if ((Object) cardDef?.CardDef == (Object) null)
      {
        Debug.LogError((object) ("Failed to assign dynamic video(s) for card id: " + this.m_CardId));
        return false;
      }
      if (string.IsNullOrEmpty(cardDef.CardDef.m_StoreItemDisplayPath))
        return false;
      if ((Object) this.m_LoadedStoreDef != (Object) null)
        Object.Destroy((Object) this.m_LoadedStoreDef.gameObject);
      this.m_LoadedStoreDef = GameUtils.LoadGameObjectWithComponent<StoreItemDisplayDef>(cardDef.CardDef.m_StoreItemDisplayPath);
      if ((Object) this.m_LoadedStoreDef == (Object) null)
      {
        Debug.LogError((object) ("Failed to pull StoreItemDisplayDef for card " + this.m_CardId + "!"));
        return false;
      }
      int num = this.TrySetVideoToPlayer(this.m_LoadedStoreDef.GetStoreVideoDisplay(this.m_VideoIdToLoad)) ? 1 : 0;
      Object.Destroy((Object) this.m_LoadedStoreDef.gameObject);
      this.m_LoadedStoreDef = (StoreItemDisplayDef) null;
      return num != 0;
    }
  }

  private bool TrySetVideoToPlayer(StoreItemDisplayDef.StoreVideoDisplay displayVideo)
  {
    if (displayVideo == null)
    {
      Debug.LogError((object) "StoreItemDisplayDef returned null videos!");
      return false;
    }
    if ((Object) this.m_DynamicVideo == (Object) null)
    {
      Debug.LogError((object) "StoreItemDisplayDef has no DynamicVideoLoader to assign videos to!");
      return false;
    }
    this.m_DynamicVideo.VideoLocation = displayVideo.VideoPath;
    this.m_DynamicVideo.FallbackTextureLocation = displayVideo.FallbackTexturePath;
    if ((bool) (Object) this.m_dynamicCaptionDriver && displayVideo.VideoCaptions != null && displayVideo.VideoCaptions.Count > 0)
      this.m_dynamicCaptionDriver.VideoCaptionKeys = displayVideo.VideoCaptions;
    return true;
  }
}
