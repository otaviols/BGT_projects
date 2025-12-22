using Hearthstone;
using UnityEngine;

public class CardSound
{
  private string m_path;
  private AudioSource m_source;
  private Card m_owner;
  private bool m_alwaysValid;

  public CardSound(string path, Card owner, bool alwaysValid)
  {
    this.m_path = path;
    this.m_owner = owner;
    this.m_alwaysValid = alwaysValid;
  }

  public AudioSource GetSound(bool loadIfNeeded = true)
  {
    if ((Object) this.m_source == (Object) null & loadIfNeeded)
      this.LoadSound();
    return this.m_source;
  }

  public void Clear()
  {
    if ((Object) this.m_source == (Object) null)
      return;
    Object.Destroy((Object) this.m_source.gameObject);
  }

  private void LoadSound()
  {
    if (string.IsNullOrEmpty(this.m_path) || !AssetLoader.Get().IsAssetAvailable((AssetReference) this.m_path))
      return;
    GameObject gameObject = SoundLoader.LoadSound((AssetReference) this.m_path);
    if ((Object) gameObject == (Object) null)
    {
      if (!this.m_alwaysValid)
        return;
      string message = string.Format("CardSound.LoadSound() - Failed to load \"{0}\"", (object) this.m_path);
      if (HearthstoneApplication.UseDevWorkarounds())
        Debug.LogError((object) message);
      else
        Error.AddDevFatal(message);
    }
    else
    {
      this.m_source = gameObject.GetComponent<AudioSource>();
      if ((Object) this.m_source == (Object) null)
      {
        Object.Destroy((Object) gameObject);
        if (!this.m_alwaysValid)
          return;
        string message = string.Format("CardSound.LoadSound() - \"{0}\" does not have an AudioSource component.", (object) this.m_path);
        if (HearthstoneApplication.UseDevWorkarounds())
          Debug.LogError((object) message);
        else
          Error.AddDevFatal(message);
      }
      else
        this.SetupSound();
    }
  }

  private void SetupSound()
  {
    if ((Object) this.m_source == (Object) null || (Object) this.m_owner == (Object) null)
      return;
    this.m_source.transform.parent = this.m_owner.transform;
    TransformUtil.Identity((Component) this.m_source.transform);
  }
}
