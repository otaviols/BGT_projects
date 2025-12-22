using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GuestHeroDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_shortName;
  [SerializeField]
  private DbfLocValue m_flavorText;
  [SerializeField]
  private SpecialEventType m_unlockEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("none");

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public CardDbfRecord CardRecord => GameDbf.Card.GetRecord(this.m_cardId);

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("SHORT_NAME")]
  public DbfLocValue ShortName => this.m_shortName;

  [DbfField("FLAVOR_TEXT")]
  public DbfLocValue FlavorText => this.m_flavorText;

  [DbfField("UNLOCK_EVENT")]
  public SpecialEventType UnlockEvent => this.m_unlockEvent;

  public void SetCardId(int v) => this.m_cardId = v;

  public void SetName(DbfLocValue v)
  {
    this.m_name = v;
    v.SetDebugInfo(this.ID, "NAME");
  }

  public void SetShortName(DbfLocValue v)
  {
    this.m_shortName = v;
    v.SetDebugInfo(this.ID, "SHORT_NAME");
  }

  public void SetFlavorText(DbfLocValue v)
  {
    this.m_flavorText = v;
    v.SetDebugInfo(this.ID, "FLAVOR_TEXT");
  }

  public void SetUnlockEvent(SpecialEventType v) => this.m_unlockEvent = v;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "NAME")
      return (object) this.m_name;
    if (name == "SHORT_NAME")
      return (object) this.m_shortName;
    if (name == "FLAVOR_TEXT")
      return (object) this.m_flavorText;
    return name == "UNLOCK_EVENT" ? (object) this.m_unlockEvent : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "CARD_ID"))
      {
        if (!(name == "NAME"))
        {
          if (!(name == "SHORT_NAME"))
          {
            if (!(name == "FLAVOR_TEXT"))
            {
              if (!(name == "UNLOCK_EVENT"))
                return;
              this.m_unlockEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
            }
            else
              this.m_flavorText = (DbfLocValue) val;
          }
          else
            this.m_shortName = (DbfLocValue) val;
        }
        else
          this.m_name = (DbfLocValue) val;
      }
      else
        this.m_cardId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "CARD_ID")
      return typeof (int);
    if (name == "NAME")
      return typeof (DbfLocValue);
    if (name == "SHORT_NAME")
      return typeof (DbfLocValue);
    if (name == "FLAVOR_TEXT")
      return typeof (DbfLocValue);
    return name == "UNLOCK_EVENT" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadGuestHeroDbfRecords loadRecords = new LoadGuestHeroDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    GuestHeroDbfAsset guestHeroDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (GuestHeroDbfAsset)) as GuestHeroDbfAsset;
    if ((UnityEngine.Object) guestHeroDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("GuestHeroDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < guestHeroDbfAsset.Records.Count; ++index)
      guestHeroDbfAsset.Records[index].StripUnusedLocales();
    records = guestHeroDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_shortName.StripUnusedLocales();
    this.m_flavorText.StripUnusedLocales();
  }
}
