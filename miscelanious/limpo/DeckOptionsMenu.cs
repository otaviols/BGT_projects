using Blizzard.T5.Core;
using Hearthstone.UI;
using PegasusShared;
using System.Collections;
using UnityEngine;

public class DeckOptionsMenu : MonoBehaviour
{
  [Header("Tray")]
  [SerializeField]
  private GameObject m_root;
  [SerializeField]
  private GameObject m_top;
  [SerializeField]
  private GameObject m_bottom;
  [SerializeField]
  [Header("Buttons")]
  private UberText m_convertText;
  [SerializeField]
  private PegUIElement m_renameButton;
  [SerializeField]
  private PegUIElement m_deleteButton;
  [SerializeField]
  private PegUIElement m_switchFormatButton;
  [SerializeField]
  private PegUIElement m_retireButton;
  [SerializeField]
  private DeckCopyPasteButton m_copyPasteDeckButton;
  [SerializeField]
  private PegUIElement m_deckHelperButton;
  [SerializeField]
  private HighlightState m_highlight;
  [Header("Sound")]
  [SerializeField]
  private WeakAssetReference m_convertToWildSound;
  [SerializeField]
  private WeakAssetReference m_convertToStandardSound;
  [SerializeField]
  [Header("Bones")]
  private Transform m_showBone;
  [SerializeField]
  private Transform m_hideBone;
  [SerializeField]
  private Transform[] m_buttonPositions;
  [SerializeField]
  private Transform[] m_bottomPositions;
  [SerializeField]
  private float[] m_topScales;
  private int m_buttonCount;
  private bool m_shown;
  private CollectionDeck m_deck;
  private CollectionDeckInfo m_deckInfo;
  private bool m_deleteButtonAlertBeingProcessed;

  public bool IsShown => this.m_shown;

  public void Awake()
  {
    this.m_root.SetActive(false);
    if ((Object) this.m_renameButton != (Object) null)
      this.m_renameButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRenameButtonReleased));
    if ((Object) this.m_deleteButton != (Object) null)
      this.m_deleteButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeleteButtonReleased));
    if ((Object) this.m_switchFormatButton != (Object) null)
      this.m_switchFormatButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSwitchFormatButtonReleased));
    if ((Object) this.m_retireButton != (Object) null)
      this.m_retireButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRetireButtonReleased));
    if ((Object) this.m_copyPasteDeckButton != (Object) null)
      this.m_copyPasteDeckButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCopyButtonReleased));
    if (!((Object) this.m_deckHelperButton != (Object) null))
      return;
    this.m_deckHelperButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeckHelperButtonReleased));
  }

  public void Show()
  {
    if (this.m_shown)
      return;
    iTween.Stop(this.gameObject);
    this.m_root.SetActive(true);
    this.SetSwitchFormatText(this.m_deck.FormatType);
    this.UpdateLayout();
    if (this.m_buttonCount == 0)
    {
      this.m_root.SetActive(false);
    }
    else
    {
      iTween.MoveTo(this.m_root, iTween.Hash((object) "position", (object) this.m_showBone.transform.position, (object) "time", (object) 0.35f, (object) "easeType", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "FinishShow", (object) "oncompletetarget", (object) this.gameObject));
      this.m_shown = true;
    }
  }

  public void Hide(bool animate = true)
  {
    if (!this.m_shown)
      return;
    iTween.Stop(this.gameObject);
    if (!animate)
    {
      this.m_root.SetActive(false);
    }
    else
    {
      this.m_root.SetActive(true);
      iTween.MoveTo(this.m_root, iTween.Hash((object) "position", (object) this.m_hideBone.transform.position, (object) "time", (object) 0.35f, (object) "easeType", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "FinishHide", (object) "oncompletetarget", (object) this.gameObject));
      this.m_shown = false;
    }
  }

  private void FinishHide()
  {
    if (this.m_shown)
      return;
    this.m_root.SetActive(false);
  }

  public void SetDeck(CollectionDeck deck) => this.m_deck = deck;

  public void SetDeckInfo(CollectionDeckInfo deckInfo) => this.m_deckInfo = deckInfo;

  private void OnRenameButtonReleased(UIEvent e)
  {
    this.m_deckInfo.Hide();
    CollectionDeckTray.Get().GetDecksContent().RenameCurrentlyEditingDeck();
  }

  private void OnDeleteButtonReleased(UIEvent e)
  {
    if (this.m_deleteButtonAlertBeingProcessed)
    {
      Debug.LogWarning((object) "DeckOptionsMenu:OnDeleteButtonReleased: Called while a Delete button alert was already being processed");
    }
    else
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
      info.m_headerText = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_HEADER");
      info.m_showAlertIcon = false;
      info.m_text = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_DESC");
      info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
      info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnDeleteButtonConfirmationResponse);
      this.m_deckInfo.Hide();
      this.m_deleteButtonAlertBeingProcessed = true;
      DialogManager.Get().ShowPopup(info, new DialogManager.DialogProcessCallback(this.OnDeleteButtonAlertPopupProcessed));
    }
  }

  private bool OnDeleteButtonAlertPopupProcessed(DialogBase dialog, object userData)
  {
    this.m_deleteButtonAlertBeingProcessed = false;
    return true;
  }

  private void OnDeleteButtonConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    CollectionDeckTray.Get().DeleteEditingDeck();
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((Object) collectibleDisplay != (Object) null))
      return;
    collectibleDisplay.OnDoneEditingDeck();
  }

  private void OnRetireButtonReleased(UIEvent e)
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_showAlertIcon = false;
    info.m_headerText = GameStrings.Get("GLUE_TAVERN_BRAWL_RETIRE_CONFIRM_HEADER");
    info.m_text = TavernBrawlManager.Get().CurrentSeasonBrawlMode != TavernBrawlMode.TB_MODE_HEROIC ? GameStrings.Get("GLUE_BRAWLISEUM_RETIRE_CONFIRM_DESC") : GameStrings.Get("GLUE_TAVERN_BRAWL_RETIRE_CONFIRM_DESC");
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
    info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnRetireButtonConfirmationResponse);
    this.m_deckInfo.Hide();
    DialogManager.Get().ShowPopup(info);
  }

  private void OnRetireButtonConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    Network.Get().TavernBrawlRetire();
  }

  private void OnClosePressed(UIEvent e) => Navigation.GoBack();

  private void OverOffClicker(UIEvent e)
  {
    Debug.Log((object) nameof (OverOffClicker));
    this.Hide();
  }

  private void OnSwitchFormatButtonReleased(UIEvent e) => this.StartCoroutine(this.SwitchFormat());

  private IEnumerator SwitchFormat()
  {
    DeckOptionsMenu deckOptionsMenu = this;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((Object) collectibleDisplay != (Object) null)
      collectibleDisplay.HideConvertTutorial();
    deckOptionsMenu.m_deckInfo.Hide();
    deckOptionsMenu.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    TraySection editingTraySection = CollectionDeckTray.Get().GetDecksContent().GetEditingTraySection();
    switch (deckOptionsMenu.m_deck.FormatType)
    {
      case FormatType.FT_WILD:
        editingTraySection.m_deckFX.Play("DeckTraySectionCollectionDeck_WildGlowOut");
        if (!string.IsNullOrEmpty(deckOptionsMenu.m_convertToStandardSound.AssetString))
          SoundManager.Get().LoadAndPlay((AssetReference) deckOptionsMenu.m_convertToStandardSound.AssetString, deckOptionsMenu.gameObject);
        yield return (object) new WaitForSeconds(0.5f);
        break;
      case FormatType.FT_STANDARD:
        editingTraySection.m_deckFX.Play("DeckTraySectionCollectionDeck_StandardGlowOut");
        if (!string.IsNullOrEmpty(deckOptionsMenu.m_convertToWildSound.AssetString))
          SoundManager.Get().LoadAndPlay((AssetReference) deckOptionsMenu.m_convertToWildSound.AssetString, deckOptionsMenu.gameObject);
        yield return (object) new WaitForSeconds(0.5f);
        break;
      default:
        Debug.LogError((object) ("DeckOptionsMenu.SwitchFormat called with invalid deck format type " + deckOptionsMenu.m_deck.FormatType.ToString()));
        break;
    }
    if (CollectionManager.Get().GetEditedDeck() != deckOptionsMenu.m_deck)
      deckOptionsMenu.m_deck.FormatType = deckOptionsMenu.GetNextFormatType(deckOptionsMenu.m_deck.FormatType);
    else
      deckOptionsMenu.SetDeckFormat(deckOptionsMenu.GetNextFormatType(deckOptionsMenu.m_deck.FormatType));
  }

  private FormatType GetNextFormatType(FormatType formatType)
  {
    if (formatType == FormatType.FT_WILD)
      return FormatType.FT_STANDARD;
    if (formatType == FormatType.FT_STANDARD)
      return FormatType.FT_WILD;
    Debug.LogError((object) ("DeckOptionsMenu.SwitchFormat called with invalid deck format type " + formatType.ToString()));
    return formatType;
  }

  private void SetDeckFormat(FormatType formatType)
  {
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if ((Object) collectionDeckTray == (Object) null)
    {
      Debug.LogError((object) "DeckOptionsMenu.SetDeckFormat: CollectionDeckTray.Get() returned null");
    }
    else
    {
      DeckTrayCardListContent cardsContent = collectionDeckTray.GetCardsContent();
      if ((Object) cardsContent == (Object) null)
      {
        Debug.LogError((object) "DeckOptionsMenu.SetDeckFormat: collectionDeckTray.GetCardsContent() returned null");
      }
      else
      {
        CollectionManager collectionManager = CollectionManager.Get();
        if (collectionManager == null)
        {
          Debug.LogError((object) "DeckOptionsMenu.SetDeckFormat: CollectionManager.Get() returned null");
        }
        else
        {
          CollectionDeckBoxVisual editingDeckBox = collectionDeckTray.GetEditingDeckBox();
          if ((Object) editingDeckBox == (Object) null)
          {
            Debug.LogError((object) "DeckOptionsMenu.SetDeckFormat: collectionDeckTray.GetEditingDeckBox() returned null");
          }
          else
          {
            this.m_deck.FormatType = formatType;
            editingDeckBox.SetFormatType(formatType);
            collectionManager.SetDeckRuleset(DeckRuleset.GetRuleset(formatType));
            CollectionManagerDisplay collectibleDisplay = collectionManager.GetCollectibleDisplay() as CollectionManagerDisplay;
            if ((Object) collectibleDisplay != (Object) null)
            {
              collectibleDisplay.GetPageManager().RefreshCurrentPageContents(BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT);
              collectibleDisplay.UpdateSetFilters(formatType, true);
            }
            cardsContent.UpdateCardList();
            cardsContent.UpdateTileVisuals();
            if (!((Object) collectibleDisplay != (Object) null) || formatType == FormatType.FT_WILD || !collectionManager.ShouldShowWildToStandardTutorial())
              return;
            collectibleDisplay.ShowStandardInfoTutorial(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
          }
        }
      }
    }
  }

  private void SetSwitchFormatText(FormatType formatType)
  {
    if (formatType == FormatType.FT_CLASSIC)
      return;
    FormatType nextFormatType = this.GetNextFormatType(formatType);
    string key;
    if (new Map<FormatType, string>()
    {
      {
        FormatType.FT_STANDARD,
        "GLUE_COLLECTION_TO_STANDARD"
      },
      {
        FormatType.FT_WILD,
        "GLUE_COLLECTION_TO_WILD"
      }
    }.TryGetValue(nextFormatType, out key))
    {
      this.m_convertText.Text = GameStrings.Get(key);
    }
    else
    {
      Debug.LogError((object) ("DeckOptionsMenu.SetSwitchFormatText called with unsupported next format type " + nextFormatType.ToString()));
      this.m_convertText.Text = nextFormatType.ToString();
    }
  }

  private void OnDeckHelperButtonReleased(UIEvent e)
  {
    this.m_deckInfo.Hide();
    CollectionDeckSlot invalidSlot = CollectionDeckTray.Get().GetCardsContent().FindInvalidSlot();
    CollectionDeckTray.Get().GetCardsContent().ShowDeckHelper(invalidSlot, false);
  }

  private void UpdateLayout()
  {
    int buttonCount = this.GetButtonCount();
    if (buttonCount != this.m_buttonCount)
    {
      this.m_buttonCount = buttonCount;
      this.UpdateBackground();
    }
    this.UpdateButtons();
  }

  private void UpdateBackground()
  {
    if (this.m_buttonCount == 0)
      return;
    this.m_top.transform.transform.localScale = new Vector3(1f, 1f, this.m_topScales[this.m_buttonCount - 1]);
    this.m_bottom.transform.transform.position = this.m_bottomPositions[this.m_buttonCount - 1].position;
  }

  private void UpdateButtons()
  {
    int index = 0;
    bool flag1 = this.ShowConvertButton();
    bool flag2 = this.ShowRenameButton();
    bool flag3 = this.ShowDeleteButton();
    bool flag4 = this.ShowCopyPasteDeckButton();
    bool flag5 = this.ShowRetireButton();
    bool flag6 = this.ShowDeckHelperButton();
    this.m_switchFormatButton.gameObject.SetActive(flag1);
    if (flag1)
    {
      if (this.m_deck.FormatType == FormatType.FT_WILD && (Object) this.m_highlight != (Object) null && CollectionManager.Get().ShouldShowWildToStandardTutorial())
        this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
      this.m_switchFormatButton.transform.position = this.m_buttonPositions[index].position;
      ++index;
    }
    this.m_renameButton.gameObject.SetActive(flag2);
    if (flag2)
    {
      this.m_renameButton.transform.position = this.m_buttonPositions[index].position;
      ++index;
    }
    this.m_copyPasteDeckButton.gameObject.SetActive(flag4);
    if (flag4)
    {
      this.m_copyPasteDeckButton.transform.position = this.m_buttonPositions[index].position;
      ++index;
    }
    this.m_deckHelperButton.gameObject.SetActive(flag6);
    if (flag6)
    {
      this.m_deckHelperButton.transform.position = this.m_buttonPositions[index].position;
      ++index;
    }
    this.m_deleteButton.gameObject.SetActive(flag3);
    if (flag3)
    {
      this.m_deleteButton.transform.position = this.m_buttonPositions[index].position;
      ++index;
    }
    this.m_retireButton.gameObject.SetActive(flag5);
    if (!flag5)
      return;
    this.m_retireButton.transform.position = this.m_buttonPositions[index].position;
    int num = index + 1;
  }

  private int GetButtonCount() => 0 + (this.ShowRenameButton() ? 1 : 0) + (this.ShowDeleteButton() ? 1 : 0) + (this.ShowConvertButton() ? 1 : 0) + (this.ShowCopyPasteDeckButton() ? 1 : 0) + (this.ShowRetireButton() ? 1 : 0) + (this.ShowDeckHelperButton() ? 1 : 0);

  private bool ShowCopyPasteDeckButton()
  {
    if (!this.ShowCopyDeckButton())
      return false;
    this.SetUpCopyButton();
    return true;
  }

  private void SetUpCopyButton()
  {
    this.m_copyPasteDeckButton.ButtonText.Text = GameStrings.Get("GLUE_COLLECTION_DECK_COPY");
    this.m_copyPasteDeckButton.TooltipHeaderString = GameStrings.Get("GLUE_COLLECTION_DECK_COPY_TOOLTIP_HEADLINE");
  }

  private void OnCopyButtonReleased(UIEvent e)
  {
    if (!this.m_copyPasteDeckButton.IsClickEnabled())
      return;
    this.m_deckInfo.Hide();
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    ClipboardUtils.CopyToClipboard(editedDeck.GetShareableDeck().Serialize());
    UIStatus.Get().AddInfo(GameStrings.Get("GLUE_COLLECTION_DECK_COPIED_TOAST"));
    TelemetryManager.Client().SendDeckCopied(editedDeck.ID, editedDeck.GetShareableDeck().Serialize(false));
  }

  private bool ShowCopyDeckButton()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    this.m_copyPasteDeckButton.TooltipMessage = string.Empty;
    if (editedDeck.GetTotalCardCount() == 0)
      return false;
    bool enabled = false;
    if (SceneMgr.Get().IsInDuelsMode())
    {
      if ((Object) AdventureDungeonCrawlDisplay.Get() != (Object) null)
        enabled = AdventureDungeonCrawlDisplay.Get().IsDuelsDeckValid();
    }
    else
    {
      DeckRuleViolation topViolation;
      enabled = editedDeck.CanCopyAsShareableDeck(out topViolation);
      this.m_copyPasteDeckButton.TooltipMessage = CollectionDeck.GetUserFriendlyCopyErrorMessageFromDeckRuleViolation(topViolation);
    }
    this.m_copyPasteDeckButton.SetEnabled(enabled, false);
    return true;
  }

  private bool ShowRenameButton()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck != null && editedDeck.Locked)
      return false;
    SceneMgr sceneMgr = SceneMgr.Get();
    return !sceneMgr.IsInDuelsMode() && !sceneMgr.IsInTavernBrawlMode() && UniversalInputManager.Get().IsTouchMode();
  }

  private bool ShowDeleteButton()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck != null && editedDeck.Locked)
      return false;
    SceneMgr sceneMgr = SceneMgr.Get();
    if (sceneMgr.IsInDuelsMode())
      return false;
    return sceneMgr.IsInTavernBrawlMode() ? (bool) UniversalInputManager.UsePhoneUI : UniversalInputManager.Get().IsTouchMode();
  }

  private bool ShowRetireButton()
  {
    if (SceneMgr.Get().IsInTavernBrawlMode() && TavernBrawlManager.Get().IsCurrentSeasonSessionBased)
    {
      TavernBrawlDisplay tavernBrawlDisplay = TavernBrawlDisplay.Get();
      if ((Object) tavernBrawlDisplay != (Object) null && !tavernBrawlDisplay.IsInDeckEditMode())
        return true;
    }
    return false;
  }

  private bool ShowConvertButton()
  {
    SceneMgr sceneMgr = SceneMgr.Get();
    if (sceneMgr.IsInTavernBrawlMode() || sceneMgr.IsInDuelsMode() || !CollectionManager.Get().ShouldAccountSeeStandardWild() || CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE)
      return false;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    return editedDeck == null || editedDeck.FormatType != FormatType.FT_CLASSIC;
  }

  private bool ShowDeckHelperButton()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck != null && editedDeck.Locked || editedDeck.GetTotalValidCardCount() >= CollectionManager.Get().GetDeckSize())
      return false;
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    return (collectibleDisplay != null ? (collectibleDisplay.GetViewMode() != 0 ? 1 : 0) : 1) == 0 && DeckHelper.HasChoicesToOffer(editedDeck);
  }
}
