using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class StoreItemDisplayDef : MonoBehaviour
{
  [CustomEditField(Sections = "Video")]
  public List<StoreItemDisplayDef.StoreVideoDisplay> m_StoreVideoDisplay;
  [CustomEditField(Sections = "Collection Manager", T = EditType.GAME_OBJECT)]
  public string m_CustomCMPortraitScene;
  private bool m_HasInitializedLookup;
  private readonly Dictionary<string, StoreItemDisplayDef.StoreVideoDisplay> m_VideoDisplayLookup = new Dictionary<string, StoreItemDisplayDef.StoreVideoDisplay>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

  private void TryInitLookup()
  {
    if (this.m_HasInitializedLookup)
      return;
    if (this.m_StoreVideoDisplay != null)
    {
      foreach (StoreItemDisplayDef.StoreVideoDisplay storeVideoDisplay in this.m_StoreVideoDisplay)
      {
        if (storeVideoDisplay != null && !string.IsNullOrEmpty(storeVideoDisplay.Id))
          this.m_VideoDisplayLookup[storeVideoDisplay.Id] = storeVideoDisplay;
      }
    }
    this.m_HasInitializedLookup = true;
  }

  public StoreItemDisplayDef.StoreVideoDisplay GetStoreVideoDisplay(string id)
  {
    this.TryInitLookup();
    return string.IsNullOrEmpty(id) || !this.m_VideoDisplayLookup.ContainsKey(id) ? (StoreItemDisplayDef.StoreVideoDisplay) null : this.m_VideoDisplayLookup[id];
  }

  [Serializable]
  public class StoreVideoDisplay
  {
    [CustomEditField(T = EditType.TEXT_AREA)]
    public string Id;
    [CustomEditField(T = EditType.VIDEO)]
    public string VideoPath;
    [CustomEditField(T = EditType.TEXTURE)]
    public string FallbackTexturePath;
    [CustomEditField(ListTable = true)]
    public List<VideoCaptionKey> VideoCaptions;
  }
}
