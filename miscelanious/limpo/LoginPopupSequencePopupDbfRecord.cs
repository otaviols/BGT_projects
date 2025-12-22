using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoginPopupSequencePopupDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_loginPopupSequenceId;
  [SerializeField]
  private Assets.LoginPopupSequencePopup.LoginPopupSequencePopupType m_popupType = Assets.LoginPopupSequencePopup.ParseLoginPopupSequencePopupTypeValue("basic");
  [SerializeField]
  private bool m_requiresWildUnlocked;
  [SerializeField]
  private bool m_suppressForReturningPlayer;
  [SerializeField]
  private string m_prefabOverride;
  [SerializeField]
  private string m_backgroundMaterial;
  [SerializeField]
  private DbfLocValue m_headerText;
  [SerializeField]
  private DbfLocValue m_bodyText;
  [SerializeField]
  private DbfLocValue m_buttonText;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_cardPremium;
  [SerializeField]
  private string m_featuredCardsEvent = "never";

  [DbfField("LOGIN_POPUP_SEQUENCE_ID")]
  public int LoginPopupSequenceId => this.m_loginPopupSequenceId;

  [DbfField("POPUP_TYPE")]
  public Assets.LoginPopupSequencePopup.LoginPopupSequencePopupType PopupType => this.m_popupType;

  [DbfField("REQUIRES_WILD_UNLOCKED")]
  public bool RequiresWildUnlocked => this.m_requiresWildUnlocked;

  [DbfField("SUPPRESS_FOR_RETURNING_PLAYER")]
  public bool SuppressForReturningPlayer => this.m_suppressForReturningPlayer;

  [DbfField("PREFAB_OVERRIDE")]
  public string PrefabOverride => this.m_prefabOverride;

  [DbfField("BACKGROUND_MATERIAL")]
  public string BackgroundMaterial => this.m_backgroundMaterial;

  [DbfField("HEADER_TEXT")]
  public DbfLocValue HeaderText => this.m_headerText;

  [DbfField("BODY_TEXT")]
  public DbfLocValue BodyText => this.m_bodyText;

  [DbfField("BUTTON_TEXT")]
  public DbfLocValue ButtonText => this.m_buttonText;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  [DbfField("CARD_PREMIUM")]
  public int CardPremium => this.m_cardPremium;

  [DbfField("FEATURED_CARDS_EVENT")]
  public string FeaturedCardsEvent => this.m_featuredCardsEvent;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BACKGROUND_MATERIAL":
        return (object) this.m_backgroundMaterial;
      case "BODY_TEXT":
        return (object) this.m_bodyText;
      case "BUTTON_TEXT":
        return (object) this.m_buttonText;
      case "CARD_ID":
        return (object) this.m_cardId;
      case "CARD_PREMIUM":
        return (object) this.m_cardPremium;
      case "FEATURED_CARDS_EVENT":
        return (object) this.m_featuredCardsEvent;
      case "HEADER_TEXT":
        return (object) this.m_headerText;
      case "ID":
        return (object) this.ID;
      case "LOGIN_POPUP_SEQUENCE_ID":
        return (object) this.m_loginPopupSequenceId;
      case "POPUP_TYPE":
        return (object) this.m_popupType;
      case "PREFAB_OVERRIDE":
        return (object) this.m_prefabOverride;
      case "REQUIRES_WILD_UNLOCKED":
        return (object) this.m_requiresWildUnlocked;
      case "SUPPRESS_FOR_RETURNING_PLAYER":
        return (object) this.m_suppressForReturningPlayer;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 160774710:
        if (!(name == "POPUP_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_popupType = Assets.LoginPopupSequencePopup.LoginPopupSequencePopupType.INVALID;
            return;
          case Assets.LoginPopupSequencePopup.LoginPopupSequencePopupType _:
          case int _:
            this.m_popupType = (Assets.LoginPopupSequencePopup.LoginPopupSequencePopupType) val;
            return;
          case string _:
            this.m_popupType = Assets.LoginPopupSequencePopup.ParseLoginPopupSequencePopupTypeValue((string) val);
            return;
          default:
            return;
        }
      case 451390141:
        if (!(name == "CARD_ID"))
          break;
        this.m_cardId = (int) val;
        break;
      case 461394230:
        if (!(name == "HEADER_TEXT"))
          break;
        this.m_headerText = (DbfLocValue) val;
        break;
      case 512270051:
        if (!(name == "LOGIN_POPUP_SEQUENCE_ID"))
          break;
        this.m_loginPopupSequenceId = (int) val;
        break;
      case 1015438284:
        if (!(name == "FEATURED_CARDS_EVENT"))
          break;
        this.m_featuredCardsEvent = (string) val;
        break;
      case 1386967833:
        if (!(name == "CARD_PREMIUM"))
          break;
        this.m_cardPremium = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1770463370:
        if (!(name == "REQUIRES_WILD_UNLOCKED"))
          break;
        this.m_requiresWildUnlocked = (bool) val;
        break;
      case 2636340827:
        if (!(name == "BACKGROUND_MATERIAL"))
          break;
        this.m_backgroundMaterial = (string) val;
        break;
      case 2992308412:
        if (!(name == "PREFAB_OVERRIDE"))
          break;
        this.m_prefabOverride = (string) val;
        break;
      case 3083958081:
        if (!(name == "BUTTON_TEXT"))
          break;
        this.m_buttonText = (DbfLocValue) val;
        break;
      case 3294267989:
        if (!(name == "SUPPRESS_FOR_RETURNING_PLAYER"))
          break;
        this.m_suppressForReturningPlayer = (bool) val;
        break;
      case 3679449341:
        if (!(name == "BODY_TEXT"))
          break;
        this.m_bodyText = (DbfLocValue) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "BACKGROUND_MATERIAL":
        return typeof (string);
      case "BODY_TEXT":
        return typeof (DbfLocValue);
      case "BUTTON_TEXT":
        return typeof (DbfLocValue);
      case "CARD_ID":
        return typeof (int);
      case "CARD_PREMIUM":
        return typeof (int);
      case "FEATURED_CARDS_EVENT":
        return typeof (string);
      case "HEADER_TEXT":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "LOGIN_POPUP_SEQUENCE_ID":
        return typeof (int);
      case "POPUP_TYPE":
        return typeof (Assets.LoginPopupSequencePopup.LoginPopupSequencePopupType);
      case "PREFAB_OVERRIDE":
        return typeof (string);
      case "REQUIRES_WILD_UNLOCKED":
        return typeof (bool);
      case "SUPPRESS_FOR_RETURNING_PLAYER":
        return typeof (bool);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLoginPopupSequencePopupDbfRecords loadRecords = new LoadLoginPopupSequencePopupDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LoginPopupSequencePopupDbfAsset sequencePopupDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LoginPopupSequencePopupDbfAsset)) as LoginPopupSequencePopupDbfAsset;
    if ((UnityEngine.Object) sequencePopupDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LoginPopupSequencePopupDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < sequencePopupDbfAsset.Records.Count; ++index)
      sequencePopupDbfAsset.Records[index].StripUnusedLocales();
    records = sequencePopupDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_headerText.StripUnusedLocales();
    this.m_bodyText.StripUnusedLocales();
    this.m_buttonText.StripUnusedLocales();
  }
}
