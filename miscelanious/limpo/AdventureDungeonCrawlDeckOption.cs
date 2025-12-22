using Hearthstone.UI;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureDungeonCrawlDeckOption : AdventureOptionWidget
{
  private AdventureDeckDbfRecord m_deckRecord;
  private List<long> m_deckContents;

  [CustomEditField(Sections = "Properties (Read-Only)")]
  public long DeckId => this.m_databaseId;

  protected override void OnClickableReady(Clickable clickable)
  {
    base.OnClickableReady(clickable);
    if ((Object) this.m_clickable == (Object) null)
      return;
    this.m_clickable.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Select()));
  }

  private void InitDataModel(AdventureDeckDbfRecord deckRecord)
  {
    if (deckRecord == null)
    {
      Log.Adventures.PrintError("DeckPouch tried to setup its data model with a null deck record!");
    }
    else
    {
      if (string.IsNullOrEmpty(deckRecord.DisplayTexture))
      {
        this.m_dataModel.DisplayTexture = (Material) null;
      }
      else
      {
        ObjectCallback callback = (ObjectCallback) ((assetRef, materialObj, data) => this.m_dataModel.DisplayTexture = materialObj as Material);
        AssetLoader.Get().LoadMaterial((AssetReference) deckRecord.DisplayTexture, callback);
      }
      this.m_dataModel.DisplayColor = CollectionPageManager.ColorForClass((TAG_CLASS) deckRecord.ClassId);
    }
  }

  public void Init(
    AdventureDeckDbfRecord deckRecord,
    bool locked,
    string lockedText,
    bool completed,
    bool newlyUnlocked,
    AdventureOptionWidget.OptionAcknowledgedCallback acknowledgedCallback)
  {
    this.m_deckRecord = deckRecord;
    if (this.m_deckRecord == null)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlDeckOption.Init() called with a null AdventureDeckDbfRecord!");
    }
    else
    {
      string deckName;
      this.m_deckContents = CollectionManager.Get().LoadDeckFromDBF(this.m_deckRecord.DeckId, out deckName, out string _);
      this.m_databaseId = (long) deckRecord.DeckId;
      this.InitWidget(deckName, locked, lockedText, false, completed, newlyUnlocked, acknowledgedCallback);
      this.InitDataModel(deckRecord);
    }
  }

  public override void Select()
  {
    base.Select();
    if (this.m_dataModel == null)
      Log.Adventures.PrintError("Attempting to set deck pouch option selected but data model was null!");
    else if (!(this.m_selectedCallback is AdventureDungeonCrawlDeckOption.DeckOptionSelectedCallback selectedCallback))
      Log.Adventures.PrintError("Attempting to execute a callback for the AdventureDungeonCrawlDeckOption, but no callback was provided!");
    else
      selectedCallback(this.m_deckRecord.DeckId, this.m_deckContents, this.m_dataModel.Locked);
  }

  public delegate void DeckOptionSelectedCallback(
    int deckID,
    List<long> deckContents,
    bool deckIsLocked);
}
