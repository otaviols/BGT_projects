using PegasusShared;
using UnityEngine;

public class TavernBrawlPhoneDeckTray : BasePhoneDeckTray
{
  [CustomEditField(Sections = "Buttons")]
  public StandardPegButtonNew m_RetireButton;
  private static TavernBrawlPhoneDeckTray s_instance;

  protected override void Awake()
  {
    base.Awake();
    this.m_RetireButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRetireClicked));
    TavernBrawlPhoneDeckTray.s_instance = this;
  }

  private void OnDestroy()
  {
    TavernBrawlPhoneDeckTray.s_instance = (TavernBrawlPhoneDeckTray) null;
    CollectionManager.Get().ClearEditedDeck();
  }

  public static TavernBrawlPhoneDeckTray Get() => TavernBrawlPhoneDeckTray.s_instance;

  public void Initialize()
  {
    CollectionDeck tavernBrawlDeck = TavernBrawlManager.Get().CurrentDeck();
    if (tavernBrawlDeck == null)
      return;
    this.OnTavernBrawlDeckInitialized(tavernBrawlDeck);
  }

  private void OnTavernBrawlDeckInitialized(CollectionDeck tavernBrawlDeck)
  {
    if (tavernBrawlDeck == null)
    {
      Debug.LogError((object) "Tavern Brawl deck is null.");
    }
    else
    {
      CollectionManager.Get().SetEditedDeck(tavernBrawlDeck);
      this.OnCardCountUpdated(tavernBrawlDeck.GetTotalCardCount());
      this.m_cardsContent.UpdateCardList();
    }
  }

  private void OnRetireClicked(UIEvent e) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_showAlertIcon = false,
    m_headerText = GameStrings.Get("GLUE_TAVERN_BRAWL_RETIRE_CONFIRM_HEADER"),
    m_text = TavernBrawlManager.Get().CurrentSeasonBrawlMode != TavernBrawlMode.TB_MODE_HEROIC ? GameStrings.Get("GLUE_BRAWLISEUM_RETIRE_CONFIRM_DESC") : GameStrings.Get("GLUE_TAVERN_BRAWL_RETIRE_CONFIRM_DESC"),
    m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
    m_responseCallback = new AlertPopup.ResponseCallback(this.OnRetireButtonConfirmationResponse)
  });

  private void OnRetireButtonConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    Network.Get().TavernBrawlRetire();
  }
}
