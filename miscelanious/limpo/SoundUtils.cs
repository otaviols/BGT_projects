using Assets;
using System.Collections.Generic;
using UnityEngine;

public class SoundUtils
{
  public static readonly AssetReference SquarePanelSlideOnSFX = new AssetReference("UI_SquarePanel_slide_on.prefab:777a4a40258158040ad5bc27596ba51e");
  public static readonly AssetReference SquarePanelSlideOffSFX = new AssetReference("UI_SquarePanel_slide_off.prefab:9e10f244ba0586e44beca5b547684d3f");
  public static PlatformDependentValue<bool> PlATFORM_CAN_DETECT_VOLUME = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    PC = true,
    Mac = true,
    iOS = false,
    Android = false
  };

  public static bool IsDeviceBackgroundMusicPlaying() => false;

  public static Option GetCategoryEnabledOption(Global.SoundCategory cat)
  {
    Option categoryEnabledOption = Option.INVALID;
    SoundDataTables.s_categoryEnabledOptionMap.TryGetValue(cat, out categoryEnabledOption);
    return categoryEnabledOption;
  }

  public static Option GetCategoryVolumeOption(Global.SoundCategory cat)
  {
    Option categoryVolumeOption = Option.INVALID;
    SoundDataTables.s_categoryVolumeOptionMap.TryGetValue(cat, out categoryVolumeOption);
    return categoryVolumeOption;
  }

  public static float GetOptionVolume(Option option) => Mathf.Clamp01(Options.Get().GetFloat(option)) * (SoundDataTables.s_optionVolumeMaxMap.ContainsKey(option) ? SoundDataTables.s_optionVolumeMaxMap[option] : 1f);

  public static float GetCategoryVolume(Global.SoundCategory cat)
  {
    Cheats cheats = Cheats.Get();
    if (cheats != null && !cheats.IsSoundCategoryEnabled(cat))
      return 0.0f;
    Option categoryVolumeOption = SoundUtils.GetCategoryVolumeOption(cat);
    return categoryVolumeOption == Option.INVALID ? 1f : SoundUtils.GetOptionVolume(categoryVolumeOption);
  }

  public static bool IsMusicCategory(Global.SoundCategory cat) => cat == Global.SoundCategory.MUSIC || cat == Global.SoundCategory.SPECIAL_MUSIC;

  public static bool CanDetectVolume() => (bool) SoundUtils.PlATFORM_CAN_DETECT_VOLUME;

  public static void SetSourceVolumes(Component c, float volume, bool includeInactive = false)
  {
    if (!(bool) (Object) c)
      return;
    SoundUtils.SetSourceVolumes(c.gameObject, volume);
  }

  public static void SetSourceVolumes(GameObject go, float volume, bool includeInactive = false)
  {
    if (!(bool) (Object) go)
      return;
    foreach (AudioSource componentsInChild in go.GetComponentsInChildren<AudioSource>(includeInactive))
      componentsInChild.volume = volume;
  }

  public static string GetRandomClipFromDef(SoundDef def)
  {
    if ((Object) def == (Object) null)
      return (string) null;
    List<RandomAudioClip> randomAudioClipList = def.m_RandomClips;
    if (def is IMultipleRandomClipSoundDef)
      randomAudioClipList = ((IMultipleRandomClipSoundDef) def).GetRandomAudioClips();
    if (randomAudioClipList == null)
      return (string) null;
    if (randomAudioClipList.Count == 0)
      return (string) null;
    float max = 0.0f;
    foreach (RandomAudioClip randomAudioClip in randomAudioClipList)
      max += randomAudioClip.m_Weight;
    float num1 = Random.Range(0.0f, max);
    float num2 = 0.0f;
    int index1 = randomAudioClipList.Count - 1;
    for (int index2 = 0; index2 < index1; ++index2)
    {
      RandomAudioClip randomAudioClip = randomAudioClipList[index2];
      num2 += randomAudioClip.m_Weight;
      if ((double) num1 <= (double) num2)
        return randomAudioClip.m_Clip;
    }
    return randomAudioClipList[index1].m_Clip;
  }

  public static float GetRandomVolumeFromDef(SoundDef def) => (Object) def == (Object) null ? 1f : Random.Range(def.m_RandomVolumeMin, def.m_RandomVolumeMax);

  public static float GetRandomPitchFromDef(SoundDef def) => (Object) def == (Object) null ? 1f : Random.Range(def.m_RandomPitchMin, def.m_RandomPitchMax);

  public static void CopyDuckedCategoryDef(SoundDuckedCategoryDef src, SoundDuckedCategoryDef dst)
  {
    dst.m_Category = src.m_Category;
    dst.m_Volume = src.m_Volume;
    dst.m_BeginSec = src.m_BeginSec;
    dst.m_BeginEaseType = src.m_BeginEaseType;
    dst.m_RestoreSec = src.m_RestoreSec;
    dst.m_RestoreEaseType = src.m_RestoreEaseType;
  }

  public static void CopyAudioSource(AudioSource src, AudioSource dst)
  {
    dst.clip = src.clip;
    dst.outputAudioMixerGroup = src.outputAudioMixerGroup;
    dst.bypassEffects = src.bypassEffects;
    dst.loop = src.loop;
    dst.priority = src.priority;
    dst.volume = src.volume;
    dst.pitch = src.pitch;
    dst.panStereo = src.panStereo;
    dst.spatialBlend = src.spatialBlend;
    dst.reverbZoneMix = src.reverbZoneMix;
    dst.rolloffMode = src.rolloffMode;
    dst.dopplerLevel = src.dopplerLevel;
    dst.minDistance = src.minDistance;
    dst.maxDistance = src.maxDistance;
    dst.spread = src.spread;
    SoundDef component1 = src.GetComponent<SoundDef>();
    if ((Object) component1 == (Object) null)
    {
      SoundDef component2 = dst.GetComponent<SoundDef>();
      if (!((Object) component2 != (Object) null))
        return;
      Object.DestroyImmediate((Object) component2);
    }
    else
    {
      SoundDef dst1 = dst.GetComponent<SoundDef>();
      if ((Object) dst1 == (Object) null)
        dst1 = dst.gameObject.AddComponent<SoundDef>();
      SoundUtils.CopySoundDef(component1, dst1);
    }
  }

  public static void CopySoundDef(SoundDef src, SoundDef dst)
  {
    dst.m_Category = src.m_Category;
    dst.m_RandomClips = new List<RandomAudioClip>();
    if (src.m_RandomClips != null)
    {
      for (int index = 0; index < src.m_RandomClips.Count; ++index)
        dst.m_RandomClips.Add(src.m_RandomClips[index]);
    }
    dst.m_RandomPitchMin = src.m_RandomPitchMin;
    dst.m_RandomPitchMax = src.m_RandomPitchMax;
    dst.m_RandomVolumeMin = src.m_RandomVolumeMin;
    dst.m_RandomVolumeMax = src.m_RandomVolumeMax;
    dst.m_IgnoreDucking = src.m_IgnoreDucking;
  }

  public static bool ChangeAudioSourceSettings(AudioSource source, AudioSourceSettings settings)
  {
    bool flag = false;
    if (source.bypassEffects != settings.m_bypassEffects)
    {
      source.bypassEffects = settings.m_bypassEffects;
      flag = true;
    }
    if (source.loop != settings.m_loop)
    {
      source.loop = settings.m_loop;
      flag = true;
    }
    if (source.priority != settings.m_priority)
    {
      source.priority = settings.m_priority;
      flag = true;
    }
    if (!Mathf.Approximately(source.volume, settings.m_volume))
    {
      source.volume = settings.m_volume;
      flag = true;
    }
    if (!Mathf.Approximately(source.pitch, settings.m_pitch))
    {
      source.pitch = settings.m_pitch;
      flag = true;
    }
    if (!Mathf.Approximately(source.panStereo, settings.m_stereoPan))
    {
      source.panStereo = settings.m_stereoPan;
      flag = true;
    }
    if (!Mathf.Approximately(source.spatialBlend, settings.m_spatialBlend))
    {
      source.spatialBlend = settings.m_spatialBlend;
      flag = true;
    }
    if (!Mathf.Approximately(source.reverbZoneMix, settings.m_reverbZoneMix))
    {
      source.reverbZoneMix = settings.m_reverbZoneMix;
      flag = true;
    }
    if (source.rolloffMode != settings.m_rolloffMode)
    {
      source.rolloffMode = settings.m_rolloffMode;
      flag = true;
    }
    if (!Mathf.Approximately(source.dopplerLevel, settings.m_dopplerLevel))
    {
      source.dopplerLevel = settings.m_dopplerLevel;
      flag = true;
    }
    if (!Mathf.Approximately(source.minDistance, settings.m_minDistance))
    {
      source.minDistance = settings.m_minDistance;
      flag = true;
    }
    if (!Mathf.Approximately(source.maxDistance, settings.m_maxDistance))
    {
      source.maxDistance = settings.m_maxDistance;
      flag = true;
    }
    if (!Mathf.Approximately(source.spread, settings.m_spread))
    {
      source.spread = settings.m_spread;
      flag = true;
    }
    return flag;
  }

  public static bool AddAudioSourceComponents(GameObject go)
  {
    bool flag = false;
    AudioSource source = go.GetComponent<AudioSource>();
    if ((Object) source == (Object) null)
    {
      source = go.AddComponent<AudioSource>();
      SoundUtils.ChangeAudioSourceSettings(source, new AudioSourceSettings());
      flag = true;
    }
    if (source.playOnAwake)
    {
      source.playOnAwake = false;
      flag = true;
    }
    if ((Object) go.GetComponent<SoundDef>() == (Object) null)
    {
      flag = true;
      go.AddComponent<SoundDef>();
    }
    return flag;
  }
}
