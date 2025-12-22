using Hearthstone.UI;
using System.Collections.Generic;
using UnityEngine;

public class LoanerDeckDetailsController : MonoBehaviour
{
  public ShopDeckPouchDisplay PouchDisplay;
  public UIBScrollable Scrollbar;
  private Widget m_detailsWidget;
  private ShopCardList m_cardList;

  private void Awake()
  {
    this.m_detailsWidget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_cardList = new ShopCardList(this.m_detailsWidget, this.Scrollbar);
    this.m_cardList.InitInput();
  }

  public void ShowDeckChoiceDetails(DeckTemplateDbfRecord deckRecord)
  {
    if ((Object) this.PouchDisplay == (Object) null)
      Log.Decks.PrintWarning(" Deck Details Widget is missing a ShopDeckPouchDisplay!");
    else if ((Object) this.Scrollbar == (Object) null)
    {
      Log.Decks.PrintWarning(" Deck Details Widget is missing a Scrollbar!");
    }
    else
    {
      this.PouchDisplay.SetDeckPouchData(this.m_detailsWidget, deckRecord);
      this.m_cardList.SetData((IEnumerable<DeckCardDbfRecord>) deckRecord.DeckRecord.Cards, BoosterDbId.INVALID);
      this.Scrollbar.SetScrollImmediate(0.0f);
    }
  }

  private void OnDestroy() => this.m_cardList.RemoveListeners();
}
