using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardValueDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_assetCardId;
  [SerializeField]
  private int m_premium;
  [SerializeField]
  private int m_buy;
  [SerializeField]
  private int m_sell;
  [SerializeField]
  private Assets.CardValue.SellState m_sellState;
  [SerializeField]
  private SpecialEventType m_overrideEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");

  [DbfField("BUY")]
  public int Buy => this.m_buy;

  [DbfField("SELL")]
  public int Sell => this.m_sell;

  [DbfField("OVERRIDE_EVENT")]
  public SpecialEventType OverrideEvent => this.m_overrideEvent;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ASSET_CARD_ID":
        return (object) this.m_assetCardId;
      case "BUY":
        return (object) this.m_buy;
      case "ID":
        return (object) this.ID;
      case "OVERRIDE_EVENT":
        return (object) this.m_overrideEvent;
      case "PREMIUM":
        return (object) this.m_premium;
      case "SELL":
        return (object) this.m_sell;
      case "SELL_STATE":
        return (object) this.m_sellState;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1525127986:
        if (!(name == "OVERRIDE_EVENT"))
          break;
        this.m_overrideEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 1548537093:
        if (!(name == "SELL"))
          break;
        this.m_sell = (int) val;
        break;
      case 2819203475:
        if (!(name == "SELL_STATE"))
          break;
        switch (val)
        {
          case null:
            this.m_sellState = Assets.CardValue.SellState.NORMAL;
            return;
          case Assets.CardValue.SellState _:
          case int _:
            this.m_sellState = (Assets.CardValue.SellState) val;
            return;
          case string _:
            this.m_sellState = Assets.CardValue.ParseSellStateValue((string) val);
            return;
          default:
            return;
        }
      case 2952916426:
        if (!(name == "ASSET_CARD_ID"))
          break;
        this.m_assetCardId = (int) val;
        break;
      case 3170845086:
        if (!(name == "PREMIUM"))
          break;
        this.m_premium = (int) val;
        break;
      case 3934976139:
        if (!(name == "BUY"))
          break;
        this.m_buy = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ASSET_CARD_ID":
        return typeof (int);
      case "BUY":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "OVERRIDE_EVENT":
        return typeof (string);
      case "PREMIUM":
        return typeof (int);
      case "SELL":
        return typeof (int);
      case "SELL_STATE":
        return typeof (Assets.CardValue.SellState);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardValueDbfRecords loadRecords = new LoadCardValueDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardValueDbfAsset cardValueDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardValueDbfAsset)) as CardValueDbfAsset;
    if ((UnityEngine.Object) cardValueDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardValueDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardValueDbfAsset.Records.Count; ++index)
      cardValueDbfAsset.Records[index].StripUnusedLocales();
    records = cardValueDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
