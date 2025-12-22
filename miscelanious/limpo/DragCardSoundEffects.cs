using System.Collections;
using UnityEngine;

public class DragCardSoundEffects : MonoBehaviour
{
  private const string CARD_MOTION_LOOP_AIR_SOUND = "card_motion_loop_air.prefab:d0cf08f88e847ba40ba92d3e4cc45ce5";
  private const string CARD_MOTION_LOOP_MAGICAL_SOUND = "card_motion_loop_magical.prefab:b954a7ca45bf25a49970664b7dd90c55";
  private const float MAGICAL_SOUND_VOLUME = 0.15f;
  private const float MAGICAL_SOUND_FADE_IN_TIME = 0.5f;
  private const float AIR_SOUND_MAX_VOLUME = 0.5f;
  private const float AIR_SOUND_MOVEMENT_THRESHOLD = 0.92f;
  private const float AIR_SOUND_VOLUME_SPEED = 0.4f;
  private const float AIR_SOUND_VOLUME_VELOCITY_SCALE = 0.5f;
  private const float DISABLE_VOLUME_FADE_OUT_TIME = 0.2f;
  private bool m_Disabled;
  private bool m_FadingOut;
  private Vector3 m_PreviousPosition;
  private AudioSource m_AirSoundLoop;
  private bool m_AirSoundLoading;
  private AudioSource m_MagicalSoundLoop;
  private bool m_MagicalSoundLoading;
  private float m_MagicalVolume;
  private float m_MagicalVelocity;
  private float m_AirVolume;
  private float m_AirVelocity;
  private Actor m_Actor;
  private Card m_Card;

  private void Awake() => this.m_PreviousPosition = this.transform.position;

  private void Update()
  {
    if (this.m_Disabled || !this.enabled || this.m_AirSoundLoading || this.m_MagicalSoundLoading)
      return;
    if ((Object) this.m_AirSoundLoop == (Object) null)
      this.LoadAirSound();
    else if ((Object) this.m_MagicalSoundLoop == (Object) null)
    {
      this.LoadMagicalSound();
    }
    else
    {
      if ((Object) this.m_AirSoundLoop == (Object) null || (Object) this.m_MagicalSoundLoop == (Object) null)
        return;
      if ((Object) this.m_Card == (Object) null)
        this.m_Card = this.GetComponent<Card>();
      if ((Object) this.m_Card == (Object) null)
      {
        this.Disable();
      }
      else
      {
        if ((Object) this.m_Actor == (Object) null)
          this.m_Actor = this.m_Card.GetActor();
        if ((Object) this.m_Actor == (Object) null)
          this.Disable();
        else if (!this.m_Actor.IsShown())
        {
          this.Disable();
        }
        else
        {
          this.m_MagicalSoundLoop.transform.position = this.transform.position;
          this.m_AirSoundLoop.transform.position = this.transform.position;
          if ((double) this.m_MagicalVolume < 0.150000005960464)
          {
            this.m_MagicalVolume = Mathf.SmoothDamp(this.m_MagicalVolume, 0.15f, ref this.m_MagicalVelocity, 0.5f);
            SoundManager.Get().SetVolume(this.m_MagicalSoundLoop, this.m_MagicalVolume);
          }
          else if ((double) this.m_MagicalVolume > 0.150000005960464)
          {
            this.m_MagicalVolume = 0.15f;
            SoundManager.Get().SetVolume(this.m_MagicalSoundLoop, this.m_MagicalVolume);
          }
          Vector3 position = this.transform.position;
          this.m_AirVolume = Mathf.SmoothDamp(this.m_AirVolume, Mathf.Log((float) ((double) (position - this.m_PreviousPosition).magnitude * 0.5 + 0.920000016689301)), ref this.m_AirVelocity, 0.04f, 1f);
          SoundManager.Get().SetVolume(this.m_AirSoundLoop, Mathf.Clamp(this.m_AirVolume, 0.0f, 0.5f));
          SoundManager.Get().SetVolume(this.m_MagicalSoundLoop, this.m_MagicalVolume);
          this.m_PreviousPosition = position;
        }
      }
    }
  }

  public void Restart()
  {
    this.enabled = true;
    this.m_Disabled = false;
  }

  public void Disable()
  {
    this.m_Disabled = true;
    if (!this.enabled || this.m_FadingOut)
      return;
    this.StartCoroutine("FadeOutSound");
  }

  private void OnDisable() => this.StopSound();

  private void OnDestroy()
  {
    this.StopCoroutine("FadeOutSound");
    this.StopSound();
  }

  private void StopSound()
  {
    this.m_FadingOut = false;
    this.m_MagicalVolume = 0.0f;
    this.m_AirVolume = 0.0f;
    this.m_AirVelocity = 0.0f;
    if ((Object) this.m_AirSoundLoop != (Object) null)
      SoundManager.Get()?.Stop(this.m_AirSoundLoop);
    if (!((Object) this.m_MagicalSoundLoop != (Object) null))
      return;
    SoundManager.Get()?.Stop(this.m_MagicalSoundLoop);
  }

  private IEnumerator FadeOutSound()
  {
    if ((Object) this.m_AirSoundLoop == (Object) null || (Object) this.m_MagicalSoundLoop == (Object) null)
    {
      this.StopSound();
    }
    else
    {
      this.m_FadingOut = true;
      while ((double) this.m_AirVolume > 0.0 && (double) this.m_MagicalVolume > 0.0)
      {
        this.m_AirVolume = Mathf.SmoothDamp(this.m_AirVolume, 0.0f, ref this.m_AirVelocity, 0.2f);
        this.m_MagicalVolume = Mathf.SmoothDamp(this.m_MagicalVolume, 0.0f, ref this.m_MagicalVelocity, 0.2f);
        SoundManager.Get().SetVolume(this.m_AirSoundLoop, Mathf.Clamp(this.m_AirVolume, 0.0f, 1f));
        SoundManager.Get().SetVolume(this.m_MagicalSoundLoop, this.m_MagicalVolume);
        yield return (object) null;
      }
      this.m_FadingOut = false;
      this.StopSound();
    }
  }

  private void LoadAirSound()
  {
    SoundManager.LoadedCallback callback = (SoundManager.LoadedCallback) ((source, userData) =>
    {
      if ((Object) source == (Object) null)
        return;
      this.m_AirSoundLoading = false;
      this.m_AirSoundLoop = source;
      if (!this.m_Disabled && this.enabled)
        return;
      SoundManager.Get().Stop(this.m_AirSoundLoop);
    });
    SoundManager.Get().LoadAndPlay((AssetReference) "card_motion_loop_air.prefab:d0cf08f88e847ba40ba92d3e4cc45ce5", this.gameObject, 0.0f, callback);
  }

  private void LoadMagicalSound()
  {
    SoundManager.LoadedCallback callback = (SoundManager.LoadedCallback) ((source, userData) =>
    {
      if ((Object) source == (Object) null)
        return;
      this.m_MagicalSoundLoading = false;
      if ((Object) this.m_MagicalSoundLoop != (Object) null)
        SoundManager.Get().Stop(this.m_MagicalSoundLoop);
      this.m_MagicalSoundLoop = source;
      if (!this.m_Disabled && this.enabled)
        return;
      SoundManager.Get().Stop(this.m_MagicalSoundLoop);
    });
    SoundManager.Get().LoadAndPlay((AssetReference) "card_motion_loop_magical.prefab:b954a7ca45bf25a49970664b7dd90c55", this.gameObject, 0.0f, callback);
  }
}
