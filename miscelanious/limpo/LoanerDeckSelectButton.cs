using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

public class LoanerDeckSelectButton : MonoBehaviour
{
  [HideInInspector]
  public DeckTemplateDbfRecord DeckTemplateRecord;
  [HideInInspector]
  public LoanerDeckDetailsController DeckDetailsController;
  [HideInInspector]
  public LoanerDecksInfoDataModel DataModel;
  public const string BUTTON_SELECTED = "Selected";
  [SerializeField]
  private GameObject m_portraitObject;
  [SerializeField]
  private int m_portraitMaterialIndex;
  [SerializeField]
  private UberText m_deckName;
  private const string ICON_TEXTURE_OVERRIDE_EVENT = "Default";
  private VisualController m_iconTextureController;

  public void OnDeckChoiceButtonClicked(string eventName)
  {
    if (eventName != "Selected" || this.DeckTemplateRecord == null || (Object) this.DeckDetailsController == (Object) null)
      return;
    if (this.DataModel != null)
    {
      this.DataModel.DeckChoiceTemplateId = this.DeckTemplateRecord.ID;
      DeckDbfRecord record = GameDbf.Deck.GetRecord(this.DeckTemplateRecord.DeckId);
      this.DataModel.DeckChoiceName = (string) record.Name;
      this.DataModel.DeckChoiceFlavourText = (string) record.Description;
      this.DataModel.DeckChoiceClassName = GameStrings.GetClassName((TAG_CLASS) this.DeckTemplateRecord.ClassId);
    }
    this.DeckDetailsController.ShowDeckChoiceDetails(this.DeckTemplateRecord);
    LoanerDeckDisplay.Get().SetCurrentlySelectedDeckTemplate(this.DeckTemplateRecord);
  }

  public void SetDeckSelectButtonIcon(CollectionDeck deck)
  {
    if (deck.HeroCardID == null)
      return;
    if ((Object) this.m_deckName != (Object) null)
      this.m_deckName.Text = deck.Name;
    DefLoader.Get().LoadFullDef(deck.GetDisplayHeroCardID(false), new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroFullDefLoaded));
  }

  private void OnHeroFullDefLoaded(string cardID, DefLoader.DisposableFullDef def, object userData)
  {
    Material material = (Material) null;
    if (def != null && (Object) def.CardDef != (Object) null)
      material = def.CardDef.GetCustomDeckPortrait();
    if ((Object) material == (Object) null)
      Log.CollectionDeckBox.PrintError("Custom Deck Portrait Material is null!");
    else if ((Object) this.m_portraitObject == (Object) null)
    {
      Log.CollectionDeckBox.PrintError("Custom Deck Portrait GameObject is null!");
    }
    else
    {
      Renderer component = this.m_portraitObject.GetComponent<Renderer>();
      if ((Object) component == (Object) null)
        Log.CollectionDeckBox.PrintError("Custom Deck Portrait GameObject doesnt have a renderer!");
      else
        component.SetSharedMaterial(this.m_portraitMaterialIndex, material);
    }
  }
}
