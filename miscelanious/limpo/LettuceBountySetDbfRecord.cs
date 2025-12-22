using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceBountySetDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private SpecialEventType m_availableAfterEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private DbfLocValue m_eventComingSoonText;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_descriptionNormal;
  [SerializeField]
  private DbfLocValue m_descriptionHeroic;
  [SerializeField]
  private DbfLocValue m_descriptionLegendary;
  [SerializeField]
  private DbfLocValue m_unlockPopupText;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private string m_shortGuid;
  [SerializeField]
  private bool m_isTutorial;
  [SerializeField]
  private string m_watermarkTexture;
  [SerializeField]
  private string m_zoneArtTexture;
  [SerializeField]
  private string m_tileArtTexture;
  [SerializeField]
  private bool m_isComingSoon;
  [SerializeField]
  private int m_requiredCompletedBountyId;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  [DbfField("AVAILABLE_AFTER_EVENT")]
  public SpecialEventType AvailableAfterEvent => this.m_availableAfterEvent;

  [DbfField("EVENT_COMING_SOON_TEXT")]
  public DbfLocValue EventComingSoonText => this.m_eventComingSoonText;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION_NORMAL")]
  public DbfLocValue DescriptionNormal => this.m_descriptionNormal;

  [DbfField("DESCRIPTION_HEROIC")]
  public DbfLocValue DescriptionHeroic => this.m_descriptionHeroic;

  [DbfField("UNLOCK_POPUP_TEXT")]
  public DbfLocValue UnlockPopupText => this.m_unlockPopupText;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("SHORT_GUID")]
  public string ShortGuid => this.m_shortGuid;

  [DbfField("IS_TUTORIAL")]
  public bool IsTutorial => this.m_isTutorial;

  [DbfField("WATERMARK_TEXTURE")]
  public string WatermarkTexture => this.m_watermarkTexture;

  [DbfField("ZONE_ART_TEXTURE")]
  public string ZoneArtTexture => this.m_zoneArtTexture;

  [DbfField("TILE_ART_TEXTURE")]
  public string TileArtTexture => this.m_tileArtTexture;

  [DbfField("IS_COMING_SOON")]
  public bool IsComingSoon => this.m_isComingSoon;

  [DbfField("REQUIRED_COMPLETED_BOUNTY")]
  public int RequiredCompletedBounty => this.m_requiredCompletedBountyId;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "AVAILABLE_AFTER_EVENT":
        return (object) this.m_availableAfterEvent;
      case "DESCRIPTION_HEROIC":
        return (object) this.m_descriptionHeroic;
      case "DESCRIPTION_LEGENDARY":
        return (object) this.m_descriptionLegendary;
      case "DESCRIPTION_NORMAL":
        return (object) this.m_descriptionNormal;
      case "EVENT":
        return (object) this.m_event;
      case "EVENT_COMING_SOON_TEXT":
        return (object) this.m_eventComingSoonText;
      case "ID":
        return (object) this.ID;
      case "IS_COMING_SOON":
        return (object) this.m_isComingSoon;
      case "IS_TUTORIAL":
        return (object) this.m_isTutorial;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "REQUIRED_COMPLETED_BOUNTY":
        return (object) this.m_requiredCompletedBountyId;
      case "SHORT_GUID":
        return (object) this.m_shortGuid;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "TILE_ART_TEXTURE":
        return (object) this.m_tileArtTexture;
      case "UNLOCK_POPUP_TEXT":
        return (object) this.m_unlockPopupText;
      case "WATERMARK_TEXTURE":
        return (object) this.m_watermarkTexture;
      case "ZONE_ART_TEXTURE":
        return (object) this.m_zoneArtTexture;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 32987655:
        if (!(name == "SHORT_GUID"))
          break;
        this.m_shortGuid = (string) val;
        break;
      case 236776447:
        if (!(name == "EVENT"))
          break;
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 286783624:
        if (!(name == "IS_TUTORIAL"))
          break;
        this.m_isTutorial = (bool) val;
        break;
      case 382408230:
        if (!(name == "DESCRIPTION_HEROIC"))
          break;
        this.m_descriptionHeroic = (DbfLocValue) val;
        break;
      case 808259902:
        if (!(name == "AVAILABLE_AFTER_EVENT"))
          break;
        this.m_availableAfterEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 1284395789:
        if (!(name == "IS_COMING_SOON"))
          break;
        this.m_isComingSoon = (bool) val;
        break;
      case 1302558956:
        if (!(name == "REQUIRED_COMPLETED_BOUNTY"))
          break;
        this.m_requiredCompletedBountyId = (int) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1937866654:
        if (!(name == "UNLOCK_POPUP_TEXT"))
          break;
        this.m_unlockPopupText = (DbfLocValue) val;
        break;
      case 2158267761:
        if (!(name == "DESCRIPTION_NORMAL"))
          break;
        this.m_descriptionNormal = (DbfLocValue) val;
        break;
      case 2751077307:
        if (!(name == "TILE_ART_TEXTURE"))
          break;
        this.m_tileArtTexture = (string) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3110253767:
        if (!(name == "DESCRIPTION_LEGENDARY"))
          break;
        this.m_descriptionLegendary = (DbfLocValue) val;
        break;
      case 3251115455:
        if (!(name == "EVENT_COMING_SOON_TEXT"))
          break;
        this.m_eventComingSoonText = (DbfLocValue) val;
        break;
      case 3988050941:
        if (!(name == "WATERMARK_TEXTURE"))
          break;
        this.m_watermarkTexture = (string) val;
        break;
      case 4001119081:
        if (!(name == "ZONE_ART_TEXTURE"))
          break;
        this.m_zoneArtTexture = (string) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "AVAILABLE_AFTER_EVENT":
        return typeof (string);
      case "DESCRIPTION_HEROIC":
        return typeof (DbfLocValue);
      case "DESCRIPTION_LEGENDARY":
        return typeof (DbfLocValue);
      case "DESCRIPTION_NORMAL":
        return typeof (DbfLocValue);
      case "EVENT":
        return typeof (string);
      case "EVENT_COMING_SOON_TEXT":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "IS_COMING_SOON":
        return typeof (bool);
      case "IS_TUTORIAL":
        return typeof (bool);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_DESC":
        return typeof (string);
      case "REQUIRED_COMPLETED_BOUNTY":
        return typeof (int);
      case "SHORT_GUID":
        return typeof (string);
      case "SORT_ORDER":
        return typeof (int);
      case "TILE_ART_TEXTURE":
        return typeof (string);
      case "UNLOCK_POPUP_TEXT":
        return typeof (DbfLocValue);
      case "WATERMARK_TEXTURE":
        return typeof (string);
      case "ZONE_ART_TEXTURE":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceBountySetDbfRecords loadRecords = new LoadLettuceBountySetDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceBountySetDbfAsset bountySetDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceBountySetDbfAsset)) as LettuceBountySetDbfAsset;
    if ((UnityEngine.Object) bountySetDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceBountySetDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < bountySetDbfAsset.Records.Count; ++index)
      bountySetDbfAsset.Records[index].StripUnusedLocales();
    records = bountySetDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_eventComingSoonText.StripUnusedLocales();
    this.m_name.StripUnusedLocales();
    this.m_descriptionNormal.StripUnusedLocales();
    this.m_descriptionHeroic.StripUnusedLocales();
    this.m_descriptionLegendary.StripUnusedLocales();
    this.m_unlockPopupText.StripUnusedLocales();
  }
}
