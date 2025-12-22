using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;

public class ChooseDeckReward : CustomVisualReward
{
  public AsyncReference m_chooseDeckReference;
  public AsyncReference[] m_classButtonReferences;
  private DeckChoiceDataModel m_deckChoiceDataModel;
  private DeckChoiceDataModel[] m_buttonDataModels;
  private Widget[] m_classButtonWidgets;
  private Widget m_chooseDeckWidget;
  private List<DeckTemplateDbfRecord> m_deckTemplates;
  private int m_selectedDeckTemplateId;

  public override void Start()
  {
    this.m_classButtonWidgets = new Widget[this.m_classButtonReferences.Length];
    this.m_buttonDataModels = new DeckChoiceDataModel[this.m_classButtonReferences.Length];
    for (int index = 0; index < this.m_classButtonReferences.Length; ++index)
    {
      int classIndex = index;
      this.m_classButtonReferences[classIndex].RegisterReadyListener<Widget>((Action<Widget>) (w => this.SetupDataModelForButton(w, classIndex)));
    }
    this.m_deckChoiceDataModel = new DeckChoiceDataModel();
    this.m_chooseDeckReference.RegisterReadyListener<Widget>((Action<Widget>) (w =>
    {
      this.m_chooseDeckWidget = w;
      w.BindDataModel((IDataModel) this.m_deckChoiceDataModel);
    }));
    this.m_deckTemplates = GameDbf.DeckTemplate.GetRecords((Predicate<DeckTemplateDbfRecord>) (deckTemplateRecord => deckTemplateRecord.IsFreeReward && SpecialEventManager.Get().IsEventActive(deckTemplateRecord.Event, false)));
    base.Start();
  }

  public void SetSelectedButtonIndex(int index)
  {
    this.m_deckChoiceDataModel.ChoiceClassID = (int) GameUtils.ORDERED_HERO_CLASSES[index];
    this.m_deckChoiceDataModel.ChoiceClassName = GameStrings.GetClassName(GameUtils.ORDERED_HERO_CLASSES[index]);
    DeckTemplateDbfRecord templateDbfRecord = this.m_deckTemplates.Find((Predicate<DeckTemplateDbfRecord>) (record => record.ClassId == this.m_deckChoiceDataModel.ChoiceClassID));
    if (templateDbfRecord == null)
    {
      Log.MissingAssets.PrintError("Could not find a free deck template for class id = {0}", (object) this.m_deckChoiceDataModel.ChoiceClassID);
    }
    else
    {
      this.m_selectedDeckTemplateId = templateDbfRecord.ID;
      this.m_deckChoiceDataModel.DeckDescription = (string) (string.IsNullOrEmpty((string) templateDbfRecord.DeckRecord.AltDescription) ? templateDbfRecord.DeckRecord.Description : templateDbfRecord.DeckRecord.AltDescription);
      this.m_chooseDeckWidget.TriggerEvent("UpdateVisuals");
    }
  }

  public void ChoiceConfirmed()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_FREE_DECK_CONFIRMATION_HEADER"),
      m_text = GameStrings.Format("GLUE_FREE_DECK_CONFIRMATION_TEXT", (object) this.m_deckChoiceDataModel.ChoiceClassName),
      m_showAlertIcon = false,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response == AlertPopup.Response.CONFIRM)
        {
          Network.Get().SendFreeDeckChoice(this.m_selectedDeckTemplateId);
          this.m_chooseDeckWidget.TriggerEvent("COMPLETE");
        }
        else
          this.m_chooseDeckWidget.TriggerEvent("SHOW");
      })
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void SetupDataModelForButton(Widget w, int index)
  {
    string str = GameUtils.ORDERED_HERO_CLASSES[index].ToString();
    DeckChoiceDataModel deckChoiceDataModel = new DeckChoiceDataModel();
    deckChoiceDataModel.ButtonClass = str;
    this.m_classButtonWidgets[index] = w;
    this.m_buttonDataModels[index] = deckChoiceDataModel;
    w.BindDataModel((IDataModel) deckChoiceDataModel);
  }
}
