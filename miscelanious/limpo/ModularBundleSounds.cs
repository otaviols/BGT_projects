using UnityEngine;

public class ModularBundleSounds : MonoBehaviour
{
  private string m_entrySound;
  private string m_landingSound;
  private string m_exitSound;

  public void Initialize(string entrySound, string landingSound, string exitSound)
  {
    this.m_entrySound = entrySound;
    this.m_landingSound = landingSound;
    this.m_exitSound = exitSound;
  }

  private void PlayEntrySound()
  {
    if (string.IsNullOrEmpty(this.m_entrySound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_entrySound);
  }

  private void PlayLandingSound()
  {
    if (string.IsNullOrEmpty(this.m_landingSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_landingSound);
  }

  private void PlayExitSound()
  {
    if (string.IsNullOrEmpty(this.m_exitSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_exitSound);
  }
}
