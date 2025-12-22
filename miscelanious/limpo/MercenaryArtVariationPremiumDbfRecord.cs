using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MercenaryArtVariationPremiumDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_mercenaryArtVariationId;
  [SerializeField]
  private MercenaryArtVariationPremium.MercenariesPremium m_premium;
  [SerializeField]
  private bool m_collectible;
  [SerializeField]
  private bool m_rewardTrack;
  [SerializeField]
  private DbfLocValue m_customAcquireText;

  [DbfField("MERCENARY_ART_VARIATION_ID")]
  public int MercenaryArtVariationId => this.m_mercenaryArtVariationId;

  [DbfField("PREMIUM")]
  public MercenaryArtVariationPremium.MercenariesPremium Premium => this.m_premium;

  [DbfField("COLLECTIBLE")]
  public bool Collectible => this.m_collectible;

  [DbfField("REWARD_TRACK")]
  public bool RewardTrack => this.m_rewardTrack;

  [DbfField("CUSTOM_ACQUIRE_TEXT")]
  public DbfLocValue CustomAcquireText => this.m_customAcquireText;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "MERCENARY_ART_VARIATION_ID")
      return (object) this.m_mercenaryArtVariationId;
    if (name == "PREMIUM")
      return (object) this.m_premium;
    if (name == "COLLECTIBLE")
      return (object) this.m_collectible;
    if (name == "REWARD_TRACK")
      return (object) this.m_rewardTrack;
    return name == "CUSTOM_ACQUIRE_TEXT" ? (object) this.m_customAcquireText : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "MERCENARY_ART_VARIATION_ID"))
      {
        if (!(name == "PREMIUM"))
        {
          if (!(name == "COLLECTIBLE"))
          {
            if (!(name == "REWARD_TRACK"))
            {
              if (!(name == "CUSTOM_ACQUIRE_TEXT"))
                return;
              this.m_customAcquireText = (DbfLocValue) val;
            }
            else
              this.m_rewardTrack = (bool) val;
          }
          else
            this.m_collectible = (bool) val;
        }
        else
        {
          switch (val)
          {
            case null:
              this.m_premium = MercenaryArtVariationPremium.MercenariesPremium.PREMIUM_NORMAL;
              break;
            case MercenaryArtVariationPremium.MercenariesPremium _:
            case int _:
              this.m_premium = (MercenaryArtVariationPremium.MercenariesPremium) val;
              break;
            case string _:
              this.m_premium = MercenaryArtVariationPremium.ParseMercenariesPremiumValue((string) val);
              break;
          }
        }
      }
      else
        this.m_mercenaryArtVariationId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "MERCENARY_ART_VARIATION_ID")
      return typeof (int);
    if (name == "PREMIUM")
      return typeof (MercenaryArtVariationPremium.MercenariesPremium);
    if (name == "COLLECTIBLE")
      return typeof (bool);
    if (name == "REWARD_TRACK")
      return typeof (bool);
    return name == "CUSTOM_ACQUIRE_TEXT" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMercenaryArtVariationPremiumDbfRecords loadRecords = new LoadMercenaryArtVariationPremiumDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MercenaryArtVariationPremiumDbfAsset variationPremiumDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MercenaryArtVariationPremiumDbfAsset)) as MercenaryArtVariationPremiumDbfAsset;
    if ((UnityEngine.Object) variationPremiumDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MercenaryArtVariationPremiumDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < variationPremiumDbfAsset.Records.Count; ++index)
      variationPremiumDbfAsset.Records[index].StripUnusedLocales();
    records = variationPremiumDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_customAcquireText.StripUnusedLocales();
}
