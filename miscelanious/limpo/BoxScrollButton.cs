using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class BoxScrollButton : BoxMenuButton
{
  private const string ANIMATION_POPUP = "TavernBrawl_ButtonPopup";
  private const string ANIMATION_POPDOWN = "TavernBrawl_ButtonPopdown";
  private const string ANIMATION_DEACTIVATE = "TavernBrawl_ButtonDeactivate";
  public NetCache.NetCacheFeatures.CacheGames.FeatureFlags m_feature;
  public float m_hoverDelay = 0.5f;
  public Animator m_animator;
  public WeakAssetReference m_popupSound;
  public WeakAssetReference m_popdownSound;
  private bool m_isPoppedUp;
  private Coroutine m_coroutine;

  public override void TriggerOver()
  {
    if (this.IsFeatureActive())
      base.TriggerOver();
    else
      this.m_coroutine = this.StartCoroutine(this.DoPopup());
  }

  private IEnumerator DoPopup()
  {
    if (!UniversalInputManager.Get().IsTouchMode())
      yield return (object) new WaitForSeconds(this.m_hoverDelay);
    this.m_isPoppedUp = true;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_popupSound.AssetString);
    this.m_animator.Play("TavernBrawl_ButtonPopup");
  }

  public override void TriggerOut()
  {
    if (this.IsFeatureActive())
    {
      base.TriggerOut();
    }
    else
    {
      if (this.m_coroutine != null)
      {
        this.StopCoroutine(this.m_coroutine);
        this.m_coroutine = (Coroutine) null;
      }
      if (!this.m_isPoppedUp)
        return;
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_popdownSound.AssetString);
      this.m_animator.Play("TavernBrawl_ButtonPopdown");
      this.m_isPoppedUp = false;
    }
  }

  public override void TriggerPress()
  {
    if (!this.IsFeatureActive())
      return;
    base.TriggerPress();
  }

  public override void TriggerRelease()
  {
    if (!this.IsFeatureActive())
      return;
    base.TriggerRelease();
  }

  public void SetDisabledVisuals() => this.m_animator.Play("TavernBrawl_ButtonDeactivate", -1, 1f);

  public bool IsFeatureActive() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.GetFeatureFlag(this.m_feature);
}
