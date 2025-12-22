using System.Collections.Generic;
using UnityEngine;

public class GameToast : MonoBehaviour
{
  public List<Material> m_intensityMaterials = new List<Material>();

  private void Start()
  {
    this.UpdateIntensity(16f);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "time", (object) 0.5f, (object) "from", (object) 16f, (object) "to", (object) 1f, (object) "delay", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "onupdate", (object) "UpdateIntensity"));
  }

  private void UpdateIntensity(float intensity)
  {
    foreach (Material intensityMaterial in this.m_intensityMaterials)
      intensityMaterial.SetFloat("_Intensity", intensity);
  }
}
