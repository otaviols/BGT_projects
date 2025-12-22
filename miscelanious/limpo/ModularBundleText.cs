using Blizzard.T5.Core.Utils;
using UnityEngine;

public class ModularBundleText : MonoBehaviour
{
  public GameObject LargeGlow;
  public GameObject MediumGlow;
  public GameObject SmallGlow;
  public UberText Text;

  public void SetGlowSize(ModularBundleText.GlowSize activeGlowSize)
  {
    this.SetGlowActive(this.LargeGlow, false);
    this.SetGlowActive(this.MediumGlow, false);
    this.SetGlowActive(this.SmallGlow, false);
    switch (activeGlowSize)
    {
      case ModularBundleText.GlowSize.LARGE:
        this.SetGlowActive(this.LargeGlow, true);
        break;
      case ModularBundleText.GlowSize.MEDIUM:
        this.SetGlowActive(this.MediumGlow, true);
        break;
      case ModularBundleText.GlowSize.SMALL:
        this.SetGlowActive(this.SmallGlow, true);
        break;
    }
  }

  public void SetGlowSize(string glowSizeString) => this.SetGlowSize(EnumUtils.SafeParse<ModularBundleText.GlowSize>(glowSizeString, ignoreCase: true));

  private void SetGlowActive(GameObject glow, bool active)
  {
    if ((Object) glow != (Object) null)
    {
      glow.SetActive(active);
    }
    else
    {
      if (!active)
        return;
      Debug.LogWarning((object) string.Format("Unable to activate glow for Text={0} in Node={1}", (object) this.name, (object) this.transform.parent.name));
    }
  }

  public enum GlowSize
  {
    NONE,
    LARGE,
    MEDIUM,
    SMALL,
  }
}
