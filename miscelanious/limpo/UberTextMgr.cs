using Blizzard.T5.Fonts;
using Blizzard.T5.Services;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UberTextMgr : MonoBehaviour
{
  public Font m_BelweFont;
  public Font m_BelweOutlineFont;
  public Font m_FranklinGothicFont;
  public Font m_BlizzardGlobal;
  private bool m_Active;
  private List<Font> m_Fonts;
  private Locale m_Locale;
  private string m_AtlasCharacters;
  private string m_AtlasNumbers = "0123456789.";
  private static UberTextMgr s_Instance;
  private IFontTable m_fontTable;

  private void Awake()
  {
    UberTextMgr.s_Instance = this;
    this.m_fontTable = ServiceManager.Get<IFontTable>();
  }

  private void Start()
  {
    this.m_AtlasCharacters = this.BuildCharacterSet();
    Log.UberText.Print("Updating Atlas to include: {0}", (object) this.m_AtlasCharacters);
  }

  private void Update()
  {
    if (!this.m_Active)
      return;
    this.ForceEnglishCharactersInAtlas();
  }

  public static UberTextMgr Get() => UberTextMgr.s_Instance;

  public void StartAtlasUpdate()
  {
    Log.UberText.Print("UberTextMgr.StartAtlasUpdate()");
    this.m_BelweFont = this.GetLocalizedFont(this.m_BelweFont);
    this.m_BelweOutlineFont = this.GetLocalizedFont(this.m_BelweOutlineFont);
    this.m_FranklinGothicFont = this.GetLocalizedFont(this.m_FranklinGothicFont);
    this.m_Active = true;
    Font.textureRebuilt += new Action<Font>(this.LogFontAtlasUpdate);
  }

  private void ForceEnglishCharactersInAtlas()
  {
    if ((UnityEngine.Object) this.m_FranklinGothicFont == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "UberTextMgr: m_FranklinGothicFont == null");
    }
    else
    {
      this.m_FranklinGothicFont.RequestCharactersInTexture(this.m_AtlasCharacters, 40, FontStyle.Normal);
      this.m_FranklinGothicFont.RequestCharactersInTexture(this.m_AtlasCharacters, 40, FontStyle.Italic);
      if ((UnityEngine.Object) this.m_BelweOutlineFont == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "UberTextMgr: m_BelweOutlineFont == null");
      }
      else
      {
        this.m_BelweOutlineFont.RequestCharactersInTexture(this.m_AtlasCharacters, 40, FontStyle.Normal);
        this.m_BelweOutlineFont.RequestCharactersInTexture(this.m_AtlasNumbers, 65, FontStyle.Normal);
      }
    }
  }

  private string BuildCharacterSet()
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 33; index < (int) sbyte.MaxValue; ++index)
      stringBuilder.Append((char) index);
    for (int index = 192; index < 256; ++index)
      stringBuilder.Append((char) index);
    return stringBuilder.ToString();
  }

  private Font GetLocalizedFont(Font font)
  {
    if (this.m_fontTable == null)
    {
      Debug.LogError((object) "UberTextMgr: Error loading FontTable");
      return (Font) null;
    }
    FontDefinition fontDef = this.m_fontTable.GetFontDef(font);
    if (!((UnityEngine.Object) fontDef == (UnityEngine.Object) null))
      return fontDef.m_Font;
    Debug.LogError((object) ("UberTextMgr: Error loading fontDef for: " + font.name));
    return (Font) null;
  }

  private void LogFontAtlasUpdate(Font font)
  {
    if ((UnityEngine.Object) font == (UnityEngine.Object) this.m_BelweFont)
      this.LogBelweAtlasUpdate();
    else if ((UnityEngine.Object) font == (UnityEngine.Object) this.m_BelweOutlineFont)
      this.LogBelweOutlineAtlasUpdate();
    else if ((UnityEngine.Object) font == (UnityEngine.Object) this.m_FranklinGothicFont)
    {
      this.LogFranklinGothicAtlasUpdate();
    }
    else
    {
      if (!((UnityEngine.Object) font == (UnityEngine.Object) this.m_BlizzardGlobal))
        return;
      this.LogBlizzardGlobalAtlasUpdate();
    }
  }

  private void LogBelweAtlasUpdate() => Log.UberText.Print("Belwe Atlas Updated: {0}, {1}", (object) this.m_BelweFont.material.mainTexture.width, (object) this.m_BelweFont.material.mainTexture.height);

  private void LogBelweOutlineAtlasUpdate() => Log.UberText.Print("BelweOutline Atlas Updated: {0}, {1}", (object) this.m_BelweOutlineFont.material.mainTexture.width, (object) this.m_BelweOutlineFont.material.mainTexture.height);

  private void LogFranklinGothicAtlasUpdate() => Log.UberText.Print("Franklin Gothic Atlas Updated: {0}, {1}", (object) this.m_FranklinGothicFont.material.mainTexture.width, (object) this.m_FranklinGothicFont.material.mainTexture.height);

  private void LogBlizzardGlobalAtlasUpdate() => Log.UberText.Print("Blizzard Global Atlas Updated: {0}, {1}", (object) this.m_BlizzardGlobal.material.mainTexture.width, (object) this.m_BlizzardGlobal.material.mainTexture.height);
}
