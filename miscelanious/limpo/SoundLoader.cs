using Blizzard.T5.AssetManager;
using Blizzard.T5.Services;
using System;
using UnityEngine;

public static class SoundLoader
{
  public static bool LoadSound(
    AssetReference assetRef,
    PrefabCallback<GameObject> callback,
    object callbackData = null,
    GameObject fallback = null)
  {
    SoundLoader.LoadSoundCallbackData callbackData1 = new SoundLoader.LoadSoundCallbackData()
    {
      callback = callback,
      callbackData = callbackData,
      fallback = fallback
    };
    return AssetLoader.Get().InstantiatePrefab(assetRef, new PrefabCallback<GameObject>(SoundLoader.LoadSoundCallback), (object) callbackData1);
  }

  public static GameObject LoadSound(AssetReference assetRef)
  {
    if (assetRef == null)
    {
      Error.AddDevFatal("SoundLoader.LoadSound() - An asset request was made but no file name was given.");
      return (GameObject) null;
    }
    GameObject go = AssetLoader.Get().InstantiatePrefab(assetRef);
    if (SoundLoader.LocalizeSoundPrefab(go))
      return go;
    UnityEngine.Object.Destroy((UnityEngine.Object) go);
    return (GameObject) null;
  }

  public static void GetAudioDataForObject(
    GameObject go,
    out AudioSource audioSource,
    out SoundDef soundDef)
  {
    CardSoundSpell component = go.GetComponent<CardSoundSpell>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      audioSource = component.DetermineBestAudioSource();
      if ((UnityEngine.Object) audioSource == (UnityEngine.Object) null)
      {
        Debug.LogError((object) (" No audio source in Object" + go.name + " Please check the object to make sure it has an Audio Source Component"));
        soundDef = (SoundDef) null;
      }
      else
        soundDef = audioSource.gameObject.GetComponent<SoundDef>();
    }
    else
    {
      soundDef = go.GetComponent<SoundDef>();
      audioSource = go.GetComponent<AudioSource>();
    }
  }

  private static bool LocalizeSoundPrefab(GameObject go)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return false;
    AudioSource audioSource;
    SoundDef soundDef;
    SoundLoader.GetAudioDataForObject(go, out audioSource, out soundDef);
    if ((UnityEngine.Object) soundDef == (UnityEngine.Object) null)
    {
      Log.Asset.PrintInfo("LocalizeSoundPrefab: trying to load sound prefab with no SoundDef components: \"{0}\"", (object) go.name);
      return false;
    }
    if (string.IsNullOrEmpty(soundDef.m_AudioClip))
    {
      Log.Asset.PrintInfo("LocalizeSoundPrefab: trying to load sound prefab with an SoundDef that contains no AudoClip: \"{0}\"", (object) go.name);
      return false;
    }
    AssetHandle<AudioClip> clip = (AssetHandle<AudioClip>) null;
    SoundLoader.LoadAudioClipWithFallback(ref clip, audioSource, (AssetReference) soundDef.m_AudioClip);
    if (clip == null)
      return false;
    ServiceManager.Get<DisposablesCleaner>()?.Attach(go, (IDisposable) clip);
    audioSource.clip = (AudioClip) clip;
    return true;
  }

  public static void LoadAudioClipWithFallback(
    ref AssetHandle<AudioClip> clip,
    AudioSource source,
    AssetReference clipAsset)
  {
    AssetLoader.Get().LoadAsset<AudioClip>(ref clip, clipAsset);
    if (clip == null)
    {
      source.volume = 0.0f;
      Log.Sound.PrintWarning("LoadAudioClipWithFallback failed to load {0}. Falling back to muted enUS asset", (object) clipAsset?.ToString());
      AssetLoader.Get().LoadAsset<AudioClip>(ref clip, clipAsset, AssetLoadingOptions.DisableLocalization);
    }
    if (clip != null)
      return;
    Log.Sound.PrintWarning("LoadAudioClipWithFallback failed to load enUS variant of {0}. Falling back to general fallback sound", (object) clipAsset?.ToString());
    AssetLoader.Get().LoadAsset<AudioClip>(ref clip, SoundManager.FallbackSound);
  }

  private static void LoadSoundCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    SoundLoader.LoadSoundCallbackData soundCallbackData = (SoundLoader.LoadSoundCallbackData) callbackData;
    try
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null && (UnityEngine.Object) soundCallbackData.fallback != (UnityEngine.Object) null)
        go = UnityEngine.Object.Instantiate<GameObject>(soundCallbackData.fallback);
      if ((UnityEngine.Object) go != (UnityEngine.Object) null && !SoundLoader.LocalizeSoundPrefab(go))
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) go);
        go = (GameObject) null;
      }
      soundCallbackData.callback(assetRef, go, soundCallbackData.callbackData);
    }
    catch (Exception ex)
    {
      Error.AddDevFatal("LoadSoundCallback failed - assetRef={0}: {1}", (object) assetRef?.ToString(), (object) ex);
      soundCallbackData.callback(assetRef, (GameObject) null, soundCallbackData.callbackData);
    }
  }

  private class LoadSoundCallbackData
  {
    public PrefabCallback<GameObject> callback;
    public object callbackData;
    public GameObject fallback;
  }
}
