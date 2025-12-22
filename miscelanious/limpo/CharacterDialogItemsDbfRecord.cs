using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterDialogItemsDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_characterDialogId;
  [SerializeField]
  private int m_playOrder;
  [SerializeField]
  private bool m_useInnkeeperQuote;
  [SerializeField]
  private string m_audioName;
  [SerializeField]
  private DbfLocValue m_bubbleText;
  [SerializeField]
  private string m_prefabName;
  [SerializeField]
  private bool m_altBubblePosition;
  [SerializeField]
  private bool m_persistPrefab;
  [SerializeField]
  private bool m_altPosition;
  [SerializeField]
  private string m_bannerPrefabName;
  [SerializeField]
  private bool m_useBannerStyle;
  [SerializeField]
  private CharacterDialogItems.CanvasAnchorType m_bannerAnchorPosition = CharacterDialogItems.CanvasAnchorType.BOTTOM;
  [SerializeField]
  private double m_waitBefore;
  [SerializeField]
  private double m_waitAfter;
  [SerializeField]
  private string m_achieveEventType;
  [SerializeField]
  private double m_minimumDurationSeconds;
  [SerializeField]
  private double m_localeExtraSeconds;

  [DbfField("CHARACTER_DIALOG_ID")]
  public int CharacterDialogId => this.m_characterDialogId;

  [DbfField("PLAY_ORDER")]
  public int PlayOrder => this.m_playOrder;

  [DbfField("USE_INNKEEPER_QUOTE")]
  public bool UseInnkeeperQuote => this.m_useInnkeeperQuote;

  [DbfField("AUDIO_NAME")]
  public string AudioName => this.m_audioName;

  [DbfField("BUBBLE_TEXT")]
  public DbfLocValue BubbleText => this.m_bubbleText;

  [DbfField("PREFAB_NAME")]
  public string PrefabName => this.m_prefabName;

  [DbfField("ALT_BUBBLE_POSITION")]
  public bool AltBubblePosition => this.m_altBubblePosition;

  [DbfField("PERSIST_PREFAB")]
  public bool PersistPrefab => this.m_persistPrefab;

  [DbfField("ALT_POSITION")]
  public bool AltPosition => this.m_altPosition;

  [DbfField("BANNER_PREFAB_NAME")]
  public string BannerPrefabName => this.m_bannerPrefabName;

  [DbfField("USE_BANNER_STYLE")]
  public bool UseBannerStyle => this.m_useBannerStyle;

  [DbfField("BANNER_ANCHOR_POSITION")]
  public CharacterDialogItems.CanvasAnchorType BannerAnchorPosition => this.m_bannerAnchorPosition;

  [DbfField("WAIT_BEFORE")]
  public double WaitBefore => this.m_waitBefore;

  [DbfField("WAIT_AFTER")]
  public double WaitAfter => this.m_waitAfter;

  [DbfField("ACHIEVE_EVENT_TYPE")]
  public string AchieveEventType => this.m_achieveEventType;

  [DbfField("MINIMUM_DURATION_SECONDS")]
  public double MinimumDurationSeconds => this.m_minimumDurationSeconds;

  [DbfField("LOCALE_EXTRA_SECONDS")]
  public double LocaleExtraSeconds => this.m_localeExtraSeconds;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACHIEVE_EVENT_TYPE":
        return (object) this.m_achieveEventType;
      case "ALT_BUBBLE_POSITION":
        return (object) this.m_altBubblePosition;
      case "ALT_POSITION":
        return (object) this.m_altPosition;
      case "AUDIO_NAME":
        return (object) this.m_audioName;
      case "BANNER_ANCHOR_POSITION":
        return (object) this.m_bannerAnchorPosition;
      case "BANNER_PREFAB_NAME":
        return (object) this.m_bannerPrefabName;
      case "BUBBLE_TEXT":
        return (object) this.m_bubbleText;
      case "CHARACTER_DIALOG_ID":
        return (object) this.m_characterDialogId;
      case "ID":
        return (object) this.ID;
      case "LOCALE_EXTRA_SECONDS":
        return (object) this.m_localeExtraSeconds;
      case "MINIMUM_DURATION_SECONDS":
        return (object) this.m_minimumDurationSeconds;
      case "PERSIST_PREFAB":
        return (object) this.m_persistPrefab;
      case "PLAY_ORDER":
        return (object) this.m_playOrder;
      case "PREFAB_NAME":
        return (object) this.m_prefabName;
      case "USE_BANNER_STYLE":
        return (object) this.m_useBannerStyle;
      case "USE_INNKEEPER_QUOTE":
        return (object) this.m_useInnkeeperQuote;
      case "WAIT_AFTER":
        return (object) this.m_waitAfter;
      case "WAIT_BEFORE":
        return (object) this.m_waitBefore;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 582680871:
        if (!(name == "BUBBLE_TEXT"))
          break;
        this.m_bubbleText = (DbfLocValue) val;
        break;
      case 1026926188:
        if (!(name == "PLAY_ORDER"))
          break;
        this.m_playOrder = (int) val;
        break;
      case 1367724989:
        if (!(name == "USE_INNKEEPER_QUOTE"))
          break;
        this.m_useInnkeeperQuote = (bool) val;
        break;
      case 1427503831:
        if (!(name == "CHARACTER_DIALOG_ID"))
          break;
        this.m_characterDialogId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1600411973:
        if (!(name == "BANNER_ANCHOR_POSITION"))
          break;
        switch (val)
        {
          case null:
            this.m_bannerAnchorPosition = CharacterDialogItems.CanvasAnchorType.CENTER;
            return;
          case CharacterDialogItems.CanvasAnchorType _:
          case int _:
            this.m_bannerAnchorPosition = (CharacterDialogItems.CanvasAnchorType) val;
            return;
          case string _:
            this.m_bannerAnchorPosition = CharacterDialogItems.ParseCanvasAnchorTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1788065238:
        if (!(name == "WAIT_BEFORE"))
          break;
        this.m_waitBefore = (double) val;
        break;
      case 1952853119:
        if (!(name == "ALT_BUBBLE_POSITION"))
          break;
        this.m_altBubblePosition = (bool) val;
        break;
      case 1968733929:
        if (!(name == "USE_BANNER_STYLE"))
          break;
        this.m_useBannerStyle = (bool) val;
        break;
      case 1980064440:
        if (!(name == "LOCALE_EXTRA_SECONDS"))
          break;
        this.m_localeExtraSeconds = (double) val;
        break;
      case 2300801615:
        if (!(name == "PREFAB_NAME"))
          break;
        this.m_prefabName = (string) val;
        break;
      case 2303854660:
        if (!(name == "BANNER_PREFAB_NAME"))
          break;
        this.m_bannerPrefabName = (string) val;
        break;
      case 2696302966:
        if (!(name == "PERSIST_PREFAB"))
          break;
        this.m_persistPrefab = (bool) val;
        break;
      case 3448897561:
        if (!(name == "AUDIO_NAME"))
          break;
        this.m_audioName = (string) val;
        break;
      case 3695817082:
        if (!(name == "MINIMUM_DURATION_SECONDS"))
          break;
        this.m_minimumDurationSeconds = (double) val;
        break;
      case 3840082817:
        if (!(name == "WAIT_AFTER"))
          break;
        this.m_waitAfter = (double) val;
        break;
      case 3869064212:
        if (!(name == "ACHIEVE_EVENT_TYPE"))
          break;
        this.m_achieveEventType = (string) val;
        break;
      case 3995750798:
        if (!(name == "ALT_POSITION"))
          break;
        this.m_altPosition = (bool) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACHIEVE_EVENT_TYPE":
        return typeof (string);
      case "ALT_BUBBLE_POSITION":
        return typeof (bool);
      case "ALT_POSITION":
        return typeof (bool);
      case "AUDIO_NAME":
        return typeof (string);
      case "BANNER_ANCHOR_POSITION":
        return typeof (CharacterDialogItems.CanvasAnchorType);
      case "BANNER_PREFAB_NAME":
        return typeof (string);
      case "BUBBLE_TEXT":
        return typeof (DbfLocValue);
      case "CHARACTER_DIALOG_ID":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "LOCALE_EXTRA_SECONDS":
        return typeof (double);
      case "MINIMUM_DURATION_SECONDS":
        return typeof (double);
      case "PERSIST_PREFAB":
        return typeof (bool);
      case "PLAY_ORDER":
        return typeof (int);
      case "PREFAB_NAME":
        return typeof (string);
      case "USE_BANNER_STYLE":
        return typeof (bool);
      case "USE_INNKEEPER_QUOTE":
        return typeof (bool);
      case "WAIT_AFTER":
        return typeof (double);
      case "WAIT_BEFORE":
        return typeof (double);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCharacterDialogItemsDbfRecords loadRecords = new LoadCharacterDialogItemsDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CharacterDialogItemsDbfAsset dialogItemsDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CharacterDialogItemsDbfAsset)) as CharacterDialogItemsDbfAsset;
    if ((UnityEngine.Object) dialogItemsDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CharacterDialogItemsDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < dialogItemsDbfAsset.Records.Count; ++index)
      dialogItemsDbfAsset.Records[index].StripUnusedLocales();
    records = dialogItemsDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_bubbleText.StripUnusedLocales();
}
