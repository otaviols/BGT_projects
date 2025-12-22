using Blizzard.T5.Core;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class BaconGuideConfigManager : MonoBehaviour
{
  [CustomEditField(Sections = "Guide Config")]
  public List<BaconGuideConfig> m_GuideConfigs;
  private Map<string, BaconGuideConfig> m_GuideConfigLookup;

  public BaconGuideConfig GetGuideConfigForSkinCardId(string skinCardId)
  {
    this.InitGuideLookup();
    if (this.m_GuideConfigLookup != null && this.m_GuideConfigLookup.ContainsKey(skinCardId))
      return this.m_GuideConfigLookup[skinCardId];
    Log.All.PrintError("BaconGuideConfigManager: no matching config for skin ID: {0}", (object) skinCardId);
    return new GameObject().AddComponent<BaconGuideConfig>();
  }

  private void InitGuideLookup()
  {
    if (this.m_GuideConfigs == null)
    {
      Log.All.PrintError("BaconGuideConfigManager: no GuideConfigs set");
    }
    else
    {
      if (this.m_GuideConfigLookup != null)
        return;
      this.m_GuideConfigLookup = new Map<string, BaconGuideConfig>();
      foreach (BaconGuideConfig guideConfig in this.m_GuideConfigs)
      {
        if ((Object) guideConfig == (Object) null || guideConfig.m_GuideCardId == null)
          Log.All.PrintError("BaconGuideConfigManager: invalid config in list");
        else
          this.m_GuideConfigLookup[guideConfig.m_GuideCardId] = guideConfig;
      }
    }
  }
}
