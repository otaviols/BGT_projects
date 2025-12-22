using Assets;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class LettuceLobbyChooserButton : ChooserButton
{
  public MeshRenderer m_glowRenderer;
  public VisualController m_visualController;
  public GameObject m_newIndicator;
  public string m_heroicTextureAsset;
  public Vector2 m_heroicTextureTiling;
  public Vector2 m_heroicTextureOffset;

  public LettuceLobbyChooserSubButton CreateLettuceLobbySubButton(
    string buttonText,
    SceneMgr.Mode nextModeWhenChosen,
    LettuceBountySetDbfRecord bountySetRecord,
    LettuceBounty.MercenariesBountyDifficulty difficulty,
    string subButtonPrefab,
    bool useAsLastSelected,
    int numNew = 0)
  {
    LettuceLobbyChooserSubButton subButton = (LettuceLobbyChooserSubButton) this.CreateSubButton(subButtonPrefab, useAsLastSelected);
    subButton.SetButtonText(buttonText);
    subButton.SetUnlocks(numNew);
    subButton.SetMode(nextModeWhenChosen);
    subButton.SetBountySetRecord(bountySetRecord.ID);
    subButton.SetDifficulty(difficulty);
    if (difficulty == LettuceBounty.MercenariesBountyDifficulty.HEROIC && !string.IsNullOrEmpty(this.m_heroicTextureAsset))
    {
      subButton.SetPortraitTexture(this.m_heroicTextureAsset);
      subButton.SetPortraitTiling(this.m_heroicTextureTiling);
      subButton.SetPortraitOffset(this.m_heroicTextureOffset);
    }
    this.GetComponentInParent<IPopupRoot>()?.ApplyPopupRendering(this.transform, new HashSet<IPopupRendering>(), true, 31);
    return subButton;
  }

  public void SetNewCount(int newCount)
  {
    if ((Object) this.m_newIndicator == (Object) null)
      return;
    this.m_newIndicator.SetActive(newCount > 0);
  }
}
