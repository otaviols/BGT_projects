using UnityEngine;

[CustomEditClass]
public class FiresideGatheringChooserButton : ChooserButton
{
  public GameObject LanternMesh;
  public GameObject SwordMesh;

  public FiresideGatheringChooserSubButton CreateSubButton(
    string subButtonPrefab,
    bool useAsLastSelected)
  {
    return (FiresideGatheringChooserSubButton) base.CreateSubButton(subButtonPrefab, useAsLastSelected);
  }

  public void ShowLantern() => this.LanternMesh.SetActive(true);

  public void ShowSwords() => this.SwordMesh.SetActive(true);
}
