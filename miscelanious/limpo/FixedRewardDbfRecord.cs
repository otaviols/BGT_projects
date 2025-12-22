using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FixedRewardDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private FixedReward.Type m_type = FixedReward.ParseTypeValue("unknown");
  [SerializeField]
  private int m_battlegroundsGuideSkinId;
  [SerializeField]
  private int m_battlegroundsHeroSkinId;
  [SerializeField]
  private int m_battlegroundsBoardSkinId;
  [SerializeField]
  private int m_battlegroundsFinisherId;
  [SerializeField]
  private int m_battlegroundsEmoteId;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_cardPremium;
  [SerializeField]
  private int m_cardBackId;
  [SerializeField]
  private int m_metaActionId;
  [SerializeField]
  private ulong m_metaActionFlags;
  [SerializeField]
  private int m_luckyDrawBoxId;

  [DbfField("TYPE")]
  public FixedReward.Type Type => this.m_type;

  [DbfField("BATTLEGROUNDS_GUIDE_SKIN_ID")]
  public int BattlegroundsGuideSkinId => this.m_battlegroundsGuideSkinId;

  public BattlegroundsGuideSkinDbfRecord BattlegroundsGuideSkinRecord => GameDbf.BattlegroundsGuideSkin.GetRecord(this.m_battlegroundsGuideSkinId);

  [DbfField("BATTLEGROUNDS_HERO_SKIN_ID")]
  public int BattlegroundsHeroSkinId => this.m_battlegroundsHeroSkinId;

  public BattlegroundsHeroSkinDbfRecord BattlegroundsHeroSkinRecord => GameDbf.BattlegroundsHeroSkin.GetRecord(this.m_battlegroundsHeroSkinId);

  [DbfField("BATTLEGROUNDS_BOARD_SKIN_ID")]
  public int BattlegroundsBoardSkinId => this.m_battlegroundsBoardSkinId;

  [DbfField("BATTLEGROUNDS_FINISHER_ID")]
  public int BattlegroundsFinisherId => this.m_battlegroundsFinisherId;

  [DbfField("BATTLEGROUNDS_EMOTE_ID")]
  public int BattlegroundsEmoteId => this.m_battlegroundsEmoteId;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public CardDbfRecord CardRecord => GameDbf.Card.GetRecord(this.m_cardId);

  [DbfField("CARD_PREMIUM")]
  public int CardPremium => this.m_cardPremium;

  [DbfField("CARD_BACK_ID")]
  public int CardBackId => this.m_cardBackId;

  [DbfField("META_ACTION_ID")]
  public int MetaActionId => this.m_metaActionId;

  [DbfField("META_ACTION_FLAGS")]
  public ulong MetaActionFlags => this.m_metaActionFlags;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BATTLEGROUNDS_BOARD_SKIN_ID":
        return (object) this.m_battlegroundsBoardSkinId;
      case "BATTLEGROUNDS_EMOTE_ID":
        return (object) this.m_battlegroundsEmoteId;
      case "BATTLEGROUNDS_FINISHER_ID":
        return (object) this.m_battlegroundsFinisherId;
      case "BATTLEGROUNDS_GUIDE_SKIN_ID":
        return (object) this.m_battlegroundsGuideSkinId;
      case "BATTLEGROUNDS_HERO_SKIN_ID":
        return (object) this.m_battlegroundsHeroSkinId;
      case "CARD_BACK_ID":
        return (object) this.m_cardBackId;
      case "CARD_ID":
        return (object) this.m_cardId;
      case "CARD_PREMIUM":
        return (object) this.m_cardPremium;
      case "ID":
        return (object) this.ID;
      case "LUCKY_DRAW_BOX_ID":
        return (object) this.m_luckyDrawBoxId;
      case "META_ACTION_FLAGS":
        return (object) this.m_metaActionFlags;
      case "META_ACTION_ID":
        return (object) this.m_metaActionId;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "TYPE":
        return (object) this.m_type;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 338683789:
        if (!(name == "TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_type = FixedReward.Type.UNKNOWN;
            return;
          case FixedReward.Type _:
          case int _:
            this.m_type = (FixedReward.Type) val;
            return;
          case string _:
            this.m_type = FixedReward.ParseTypeValue((string) val);
            return;
          default:
            return;
        }
      case 451390141:
        if (!(name == "CARD_ID"))
          break;
        this.m_cardId = (int) val;
        break;
      case 1019664678:
        if (!(name == "BATTLEGROUNDS_FINISHER_ID"))
          break;
        this.m_battlegroundsFinisherId = (int) val;
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
      case 1560548161:
        if (!(name == "CARD_BACK_ID"))
          break;
        this.m_cardBackId = (int) val;
        break;
      case 1795557686:
        if (!(name == "BATTLEGROUNDS_HERO_SKIN_ID"))
          break;
        this.m_battlegroundsHeroSkinId = (int) val;
        break;
      case 1808921124:
        if (!(name == "LUCKY_DRAW_BOX_ID"))
          break;
        this.m_luckyDrawBoxId = (int) val;
        break;
      case 1994457602:
        if (!(name == "BATTLEGROUNDS_EMOTE_ID"))
          break;
        this.m_battlegroundsEmoteId = (int) val;
        break;
      case 2832947347:
        if (!(name == "META_ACTION_FLAGS"))
          break;
        this.m_metaActionFlags = (ulong) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3462221213:
        if (!(name == "META_ACTION_ID"))
          break;
        this.m_metaActionId = (int) val;
        break;
      case 3641954198:
        if (!(name == "BATTLEGROUNDS_GUIDE_SKIN_ID"))
          break;
        this.m_battlegroundsGuideSkinId = (int) val;
        break;
      case 3804092358:
        if (!(name == "BATTLEGROUNDS_BOARD_SKIN_ID"))
          break;
        this.m_battlegroundsBoardSkinId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "BATTLEGROUNDS_BOARD_SKIN_ID":
        return typeof (int);
      case "BATTLEGROUNDS_EMOTE_ID":
        return typeof (int);
      case "BATTLEGROUNDS_FINISHER_ID":
        return typeof (int);
      case "BATTLEGROUNDS_GUIDE_SKIN_ID":
        return typeof (int);
      case "BATTLEGROUNDS_HERO_SKIN_ID":
        return typeof (int);
      case "CARD_BACK_ID":
        return typeof (int);
      case "CARD_ID":
        return typeof (int);
      case "CARD_PREMIUM":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "LUCKY_DRAW_BOX_ID":
        return typeof (int);
      case "META_ACTION_FLAGS":
        return typeof (ulong);
      case "META_ACTION_ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "TYPE":
        return typeof (FixedReward.Type);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadFixedRewardDbfRecords loadRecords = new LoadFixedRewardDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    FixedRewardDbfAsset fixedRewardDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (FixedRewardDbfAsset)) as FixedRewardDbfAsset;
    if ((UnityEngine.Object) fixedRewardDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("FixedRewardDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < fixedRewardDbfAsset.Records.Count; ++index)
      fixedRewardDbfAsset.Records[index].StripUnusedLocales();
    records = fixedRewardDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
