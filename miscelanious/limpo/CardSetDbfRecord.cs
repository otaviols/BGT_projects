using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardSetDbfRecord : DbfRecord
{
  [SerializeField]
  private bool m_isCollectible = true;
  [SerializeField]
  private bool m_isCoreCardSet;
  [SerializeField]
  private SpecialEventType m_legacyCardSetEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");
  [SerializeField]
  private SpecialEventType m_contentLaunchEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");
  [SerializeField]
  private bool m_isFeaturedCardSet;
  [SerializeField]
  private SpecialEventType m_standardEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");
  [SerializeField]
  private bool m_craftableWhenWild;
  [SerializeField]
  private string m_cardWatermarkTexture;
  [SerializeField]
  private SpecialEventType m_setFilterEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");
  [SerializeField]
  private string m_filterIconTexture;
  [SerializeField]
  private double m_filterIconOffsetX;
  [SerializeField]
  private double m_filterIconOffsetY;
  [SerializeField]
  private int m_releaseOrder;

  [DbfField("IS_COLLECTIBLE")]
  public bool IsCollectible => this.m_isCollectible;

  [DbfField("IS_CORE_CARD_SET")]
  public bool IsCoreCardSet => this.m_isCoreCardSet;

  [DbfField("LEGACY_CARD_SET_EVENT")]
  public SpecialEventType LegacyCardSetEvent => this.m_legacyCardSetEvent;

  [DbfField("CONTENT_LAUNCH_EVENT")]
  public SpecialEventType ContentLaunchEvent => this.m_contentLaunchEvent;

  [DbfField("IS_FEATURED_CARD_SET")]
  public bool IsFeaturedCardSet => this.m_isFeaturedCardSet;

  [DbfField("STANDARD_EVENT")]
  public SpecialEventType StandardEvent => this.m_standardEvent;

  [DbfField("CRAFTABLE_WHEN_WILD")]
  public bool CraftableWhenWild => this.m_craftableWhenWild;

  [DbfField("CARD_WATERMARK_TEXTURE")]
  public string CardWatermarkTexture => this.m_cardWatermarkTexture;

  [DbfField("SET_FILTER_EVENT")]
  public SpecialEventType SetFilterEvent => this.m_setFilterEvent;

  [DbfField("FILTER_ICON_TEXTURE")]
  public string FilterIconTexture => this.m_filterIconTexture;

  [DbfField("FILTER_ICON_OFFSET_X")]
  public double FilterIconOffsetX => this.m_filterIconOffsetX;

  [DbfField("FILTER_ICON_OFFSET_Y")]
  public double FilterIconOffsetY => this.m_filterIconOffsetY;

  [DbfField("RELEASE_ORDER")]
  public int ReleaseOrder => this.m_releaseOrder;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "CARD_WATERMARK_TEXTURE":
        return (object) this.m_cardWatermarkTexture;
      case "CONTENT_LAUNCH_EVENT":
        return (object) this.m_contentLaunchEvent;
      case "CRAFTABLE_WHEN_WILD":
        return (object) this.m_craftableWhenWild;
      case "FILTER_ICON_OFFSET_X":
        return (object) this.m_filterIconOffsetX;
      case "FILTER_ICON_OFFSET_Y":
        return (object) this.m_filterIconOffsetY;
      case "FILTER_ICON_TEXTURE":
        return (object) this.m_filterIconTexture;
      case "ID":
        return (object) this.ID;
      case "IS_COLLECTIBLE":
        return (object) this.m_isCollectible;
      case "IS_CORE_CARD_SET":
        return (object) this.m_isCoreCardSet;
      case "IS_FEATURED_CARD_SET":
        return (object) this.m_isFeaturedCardSet;
      case "LEGACY_CARD_SET_EVENT":
        return (object) this.m_legacyCardSetEvent;
      case "RELEASE_ORDER":
        return (object) this.m_releaseOrder;
      case "SET_FILTER_EVENT":
        return (object) this.m_setFilterEvent;
      case "STANDARD_EVENT":
        return (object) this.m_standardEvent;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 436653950:
        if (!(name == "CARD_WATERMARK_TEXTURE"))
          break;
        this.m_cardWatermarkTexture = (string) val;
        break;
      case 509783156:
        if (!(name == "IS_FEATURED_CARD_SET"))
          break;
        this.m_isFeaturedCardSet = (bool) val;
        break;
      case 1398813849:
        if (!(name == "SET_FILTER_EVENT"))
          break;
        this.m_setFilterEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1737310453:
        if (!(name == "RELEASE_ORDER"))
          break;
        this.m_releaseOrder = (int) val;
        break;
      case 1738766585:
        if (!(name == "LEGACY_CARD_SET_EVENT"))
          break;
        this.m_legacyCardSetEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 2029953557:
        if (!(name == "FILTER_ICON_TEXTURE"))
          break;
        this.m_filterIconTexture = (string) val;
        break;
      case 2191767247:
        if (!(name == "STANDARD_EVENT"))
          break;
        this.m_standardEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 2351058211:
        if (!(name == "CRAFTABLE_WHEN_WILD"))
          break;
        this.m_craftableWhenWild = (bool) val;
        break;
      case 2807272363:
        if (!(name == "IS_CORE_CARD_SET"))
          break;
        this.m_isCoreCardSet = (bool) val;
        break;
      case 3769471775:
        if (!(name == "CONTENT_LAUNCH_EVENT"))
          break;
        this.m_contentLaunchEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 3940822416:
        if (!(name == "IS_COLLECTIBLE"))
          break;
        this.m_isCollectible = (bool) val;
        break;
      case 4124193314:
        if (!(name == "FILTER_ICON_OFFSET_X"))
          break;
        this.m_filterIconOffsetX = (double) val;
        break;
      case 4140970933:
        if (!(name == "FILTER_ICON_OFFSET_Y"))
          break;
        this.m_filterIconOffsetY = (double) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "CARD_WATERMARK_TEXTURE":
        return typeof (string);
      case "CONTENT_LAUNCH_EVENT":
        return typeof (string);
      case "CRAFTABLE_WHEN_WILD":
        return typeof (bool);
      case "FILTER_ICON_OFFSET_X":
        return typeof (double);
      case "FILTER_ICON_OFFSET_Y":
        return typeof (double);
      case "FILTER_ICON_TEXTURE":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "IS_COLLECTIBLE":
        return typeof (bool);
      case "IS_CORE_CARD_SET":
        return typeof (bool);
      case "IS_FEATURED_CARD_SET":
        return typeof (bool);
      case "LEGACY_CARD_SET_EVENT":
        return typeof (string);
      case "RELEASE_ORDER":
        return typeof (int);
      case "SET_FILTER_EVENT":
        return typeof (string);
      case "STANDARD_EVENT":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardSetDbfRecords loadRecords = new LoadCardSetDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardSetDbfAsset cardSetDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardSetDbfAsset)) as CardSetDbfAsset;
    if ((UnityEngine.Object) cardSetDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardSetDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardSetDbfAsset.Records.Count; ++index)
      cardSetDbfAsset.Records[index].StripUnusedLocales();
    records = cardSetDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
