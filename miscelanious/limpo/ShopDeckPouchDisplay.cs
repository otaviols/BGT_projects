using Blizzard.T5.AssetManager;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopDeckPouchDisplay : MonoBehaviour
{
  public Widget deckWidget;
  public RewardItemDataModel m_rewardItemDataModel;
  public Material portraitMaterial;
  private Material m_temporaryPortraitMaterial;
  private AssetHandle<Texture> m_portraitTextureHandle;
  private AssetHandle<Material> m_portraitMaterialHandle;
  private int m_currentDisplayCardId;

  public void OnDestroy()
  {
    this.SafelyDisposeTempPortrait();
    AssetHandle.SafeDispose<Material>(ref this.m_portraitMaterialHandle);
    AssetHandle.SafeDispose<Texture>(ref this.m_portraitTextureHandle);
  }

  private void SafelyDisposeTempPortrait()
  {
    if (!((UnityEngine.Object) this.m_temporaryPortraitMaterial != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_temporaryPortraitMaterial);
    this.m_temporaryPortraitMaterial = (Material) null;
  }

  public void SetRewardItem()
  {
    if ((UnityEngine.Object) this.deckWidget == (UnityEngine.Object) null)
      Log.Store.PrintWarning("[ShopDeckPouchDisplay.SetRewardItem] DeckWidget reference is null!");
    else
      this.deckWidget.RegisterDoneChangingStatesListener(new Action<object>(this.SetupRewardOnWidgetReady), (object) null, true, false);
  }

  private void SetupRewardOnWidgetReady(object _)
  {
    this.deckWidget.RemoveDoneChangingStatesListener(new Action<object>(this.SetupRewardOnWidgetReady));
    this.SetupRewardFromWidget();
  }

  private void SetupRewardFromWidget()
  {
    this.m_rewardItemDataModel = this.deckWidget.GetDataModel<RewardItemDataModel>();
    RewardItemDataModel rewardItemDataModel = this.m_rewardItemDataModel;
    if ((rewardItemDataModel != null ? (rewardItemDataModel.ItemType == RewardItemType.SELLABLE_DECK ? 1 : 0) : 0) == 0)
      return;
    int itemId = this.m_rewardItemDataModel.ItemId;
    SellableDeckDbfRecord record = GameDbf.SellableDeck.GetRecord(itemId);
    if (record == null)
      Log.Store.PrintWarning("[ShopDeckPouchDisplay.SetRewardItem] Failed to find sellable deck DB record {0}!", (object) itemId);
    else if (record.DeckTemplateRecord == null || record.DeckTemplateRecord.DeckRecord == null)
      Log.Store.PrintWarning("[ShopDeckPouchDisplay.SetRewardItem] The DB record {0} does NOT have a deck template with a valid deck record!", (object) record.ID);
    else
      this.SetDeckPouchData(this.deckWidget, record.DeckTemplateRecord);
  }

  public void SetDeckPouchData(Widget widget, DeckTemplateDbfRecord record)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
    {
      Log.Store.PrintWarning("[ShopDeckPouchDisplay.SetDeckPouchData] Deck widget is null!");
    }
    else
    {
      DeckPouchDataModel fromDeckTemplate = this.CreateDeckPouchDataModelFromDeckTemplate(record);
      widget.BindDataModel((IDataModel) fromDeckTemplate);
    }
  }

  public DeckPouchDataModel CreateDeckPouchDataModelFromDeckTemplate(
    DeckTemplateDbfRecord record)
  {
    DeckDbfRecord deckRecord = record?.DeckRecord;
    List<DeckCardDbfRecord> cards = deckRecord?.Cards;
    if (cards == null)
      return new DeckPouchDataModel();
    int[] rarityCounts = ShopDeckPouchDisplay.GetRarityCounts(cards);
    List<ShopDeckPouchDisplay.DKRuneTypes> runesOrNullIfNone = this.GetDKRunesOrNullIfNone(record);
    DeckPouchDataModel fromDeckTemplate = new DeckPouchDataModel()
    {
      Pouch = new AdventureLoadoutOptionDataModel()
      {
        Name = (string) deckRecord.Name,
        DisplayTexture = this.GetPortraitMaterialFromDeckTemplateRecord(record),
        DisplayColor = CollectionPageManager.ColorForClass((TAG_CLASS) record.ClassId)
      },
      Details = new DeckDetailsDataModel()
      {
        Product = new ProductDataModel()
        {
          DescriptionHeader = GameStrings.Format("GLUE_COLLECTION_NEW_DECK_DETAIL_HEADER", (object) GameStrings.GetClassName((TAG_CLASS) record.ClassId)),
          Description = GameStrings.Format("GLUE_COLLECTION_NEW_DECK_DETAIL_DESC", (object) rarityCounts[5], (object) rarityCounts[4], (object) rarityCounts[3], (object) rarityCounts[1])
        }
      },
      Class = (TAG_CLASS) record.ClassId
    };
    if (runesOrNullIfNone != null)
      fromDeckTemplate.Pouch.DKRunes.AddRange((IEnumerable<ShopDeckPouchDisplay.DKRuneTypes>) runesOrNullIfNone);
    return fromDeckTemplate;
  }

  private List<ShopDeckPouchDisplay.DKRuneTypes> GetDKRunesOrNullIfNone(
    DeckTemplateDbfRecord record)
  {
    if (record.ClassId != 1 || record.DKRunes.Count == 0)
      return (List<ShopDeckPouchDisplay.DKRuneTypes>) null;
    List<ShopDeckPouchDisplay.DKRuneTypes> runesOrNullIfNone = new List<ShopDeckPouchDisplay.DKRuneTypes>();
    foreach (DkRuneListDbfRecord dkRune in record.DKRunes)
      runesOrNullIfNone.Add((ShopDeckPouchDisplay.DKRuneTypes) dkRune.Rune);
    return runesOrNullIfNone;
  }

  private Material GetPortraitMaterialFromDeckTemplateRecord(DeckTemplateDbfRecord record)
  {
    if ((UnityEngine.Object) this.portraitMaterial != (UnityEngine.Object) null && record.DisplayCardId != 0)
    {
      using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(record.DisplayCardId))
      {
        if ((UnityEngine.Object) this.m_temporaryPortraitMaterial == (UnityEngine.Object) null || this.m_currentDisplayCardId != record.DisplayCardId)
        {
          this.SafelyDisposeTempPortrait();
          this.m_temporaryPortraitMaterial = new Material(this.portraitMaterial);
        }
        AssetHandle.Set<Texture>(ref this.m_portraitTextureHandle, cardDef?.CardDef.GetPortraitTextureHandle());
        this.m_temporaryPortraitMaterial.mainTexture = (Texture) this.m_portraitTextureHandle;
        this.m_currentDisplayCardId = record.DisplayCardId;
      }
      return this.m_temporaryPortraitMaterial;
    }
    AssetLoader.Get().LoadAsset<Material>(ref this.m_portraitMaterialHandle, (AssetReference) record.DisplayTexture);
    return (Material) this.m_portraitMaterialHandle;
  }

  public static int[] GetRarityCounts(List<DeckCardDbfRecord> cards)
  {
    DefLoader defLoader = DefLoader.Get();
    int[] rarityCounts = new int[6];
    cards.ForEach((Action<DeckCardDbfRecord>) (r =>
    {
      int[] numArray = rarityCounts;
      EntityDef entityDef = defLoader.GetEntityDef(r.CardId);
      int index = entityDef != null ? (int) entityDef.GetRarity() : 0;
      ++numArray[index];
    }));
    return rarityCounts;
  }

  public enum DKRuneTypes
  {
    Blood = 1,
    Frost = 2,
    Unholy = 3,
  }
}
