using System;

[Serializable]
public class SoundPlaybackLimitClipDef
{
  [CustomEditField(Label = "Clip", T = EditType.AUDIO_CLIP)]
  public string m_Path;
  public int m_Priority;
  [CustomEditField(Range = "0.0-1.0")]
  public float m_ExclusivePlaybackThreshold = 0.1f;
  [NonSerialized]
  private string m_legacyName;

  public string LegacyName
  {
    get
    {
      if (this.m_legacyName == null)
        this.m_legacyName = new AssetReference(this.m_Path).GetLegacyAssetName();
      return this.m_legacyName;
    }
  }
}
