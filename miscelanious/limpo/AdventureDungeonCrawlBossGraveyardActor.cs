using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DungeonCrawl;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdventureDungeonCrawlBossGraveyardActor : Actor
{
  public MeshRenderer m_BossBackerRenderer;
  public List<AdventureDungeonCrawlBossGraveyardActor.BossGraveyardActorVisualStyle> m_BossGraveyardActorStyle;

  public void SetStyle(IDungeonCrawlData data)
  {
    DungeonRunVisualStyle visualStyle = data.VisualStyle;
    foreach (AdventureDungeonCrawlBossGraveyardActor.BossGraveyardActorVisualStyle actorVisualStyle in this.m_BossGraveyardActorStyle)
    {
      if (visualStyle == actorVisualStyle.VisualStyle)
      {
        this.m_BossBackerRenderer.SetMaterial(actorVisualStyle.BossBackerMaterial);
        break;
      }
    }
  }

  [Serializable]
  public class BossGraveyardActorVisualStyle
  {
    public DungeonRunVisualStyle VisualStyle;
    public Material BossBackerMaterial;
  }
}
