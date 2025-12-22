using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceAbilityTierDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceAbilityId;
  [SerializeField]
  private int m_tier = 1;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_coinCraftCost = 100;

  [DbfField("LETTUCE_ABILITY_ID")]
  public int LettuceAbilityId => this.m_lettuceAbilityId;

  [DbfField("TIER")]
  public int Tier => this.m_tier;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public CardDbfRecord CardRecord => GameDbf.Card.GetRecord(this.m_cardId);

  [DbfField("COIN_CRAFT_COST")]
  public int CoinCraftCost => this.m_coinCraftCost;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_ABILITY_ID")
      return (object) this.m_lettuceAbilityId;
    if (name == "TIER")
      return (object) this.m_tier;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    return name == "COIN_CRAFT_COST" ? (object) this.m_coinCraftCost : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_ABILITY_ID"))
      {
        if (!(name == "TIER"))
        {
          if (!(name == "CARD_ID"))
          {
            if (!(name == "COIN_CRAFT_COST"))
              return;
            this.m_coinCraftCost = (int) val;
          }
          else
            this.m_cardId = (int) val;
        }
        else
          this.m_tier = (int) val;
      }
      else
        this.m_lettuceAbilityId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_ABILITY_ID")
      return typeof (int);
    if (name == "TIER")
      return typeof (int);
    if (name == "CARD_ID")
      return typeof (int);
    return name == "COIN_CRAFT_COST" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceAbilityTierDbfRecords loadRecords = new LoadLettuceAbilityTierDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceAbilityTierDbfAsset abilityTierDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceAbilityTierDbfAsset)) as LettuceAbilityTierDbfAsset;
    if ((UnityEngine.Object) abilityTierDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceAbilityTierDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < abilityTierDbfAsset.Records.Count; ++index)
      abilityTierDbfAsset.Records[index].StripUnusedLocales();
    records = abilityTierDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
