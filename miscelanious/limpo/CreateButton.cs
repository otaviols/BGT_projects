using UnityEngine;

public class CreateButton : CraftingButton
{
  private bool m_textEnlarged;
  [SerializeField]
  private UberText m_labelTextNoDustJar;

  protected override void OnRelease()
  {
    if (!Network.IsLoggedIn())
    {
      CollectionManager.ShowFeatureDisabledWhileOfflinePopup();
    }
    else
    {
      if (CraftingManager.Get().GetPendingServerTransaction() != null || (Object) CraftingManager.Get().GetShownActor() == (Object) null || CraftingManager.Get().GetShownActor().GetEntityDef() == null)
        return;
      Animation component = this.GetComponent<Animation>();
      if ((bool) UniversalInputManager.UsePhoneUI)
        component.Play("CardExchange_ButtonPress2_phone");
      else
        component.Play("CardExchange_ButtonPress2");
      bool flag = false;
      string cardId = CraftingManager.Get().GetShownActor().GetEntityDef().GetCardId();
      DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
      if (deckRuleset != null)
      {
        CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
        flag = !deckRuleset.Filter(DefLoader.Get().GetEntityDef(cardId), editedDeck);
      }
      else if (!GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_SEEN_WILD_CRAFT_ALERT))
        flag = GameUtils.IsWildCard(cardId);
      if (CraftingManager.Get().GetNumClientTransactions() != 0)
        flag = false;
      if (flag)
      {
        string setFormatAsString = GameUtils.GetCardSetFormatAsString(GameUtils.GetCardSetFromCardID(cardId));
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_CRAFTING_" + setFormatAsString + "_CARD_HEADER"),
          m_cancelText = GameStrings.Get("GLUE_CRAFTING_NONSTANDARD_CARD_WARNING_CANCEL"),
          m_confirmText = GameStrings.Get("GLUE_CRAFTING_NONSTANDARD_CARD_WARNING_CONFIRM"),
          m_showAlertIcon = true,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_responseCallback = new AlertPopup.ResponseCallback(this.OnConfirmCreateResponse)
        };
        if (SceneMgr.Get().IsInTavernBrawlMode())
        {
          info.m_headerText = GameStrings.Get("GLUE_CRAFTING_INVALID_CARD_TAVERN_BRAWL_HEADER");
          info.m_text = GameStrings.Get("GLUE_CRAFTING_INVALID_CARD_TAVERN_BRAWL_DESC");
        }
        else
          info.m_text = !CollectionManager.Get().AccountHasUnlockedWild() ? GameStrings.Get("GLUE_CRAFTING_" + setFormatAsString + "_CARD_FIRST_DESC") : GameStrings.Get("GLUE_CRAFTING_" + setFormatAsString + "_CARD_DESC");
        DialogManager.Get().ShowPopup(info);
      }
      else
        this.DoCreate();
    }
  }

  private void OnConfirmCreateResponse(AlertPopup.Response response, object userData)
  {
    if (response != AlertPopup.Response.CONFIRM)
      return;
    if (GameUtils.IsWildCard(CraftingManager.Get().GetShownActor().GetEntityDef().GetCardId()))
    {
      GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_SEEN_WILD_CRAFT_ALERT, true);
      if (!CollectionManager.Get().AccountHasUnlockedWild())
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_HEADER"),
          m_text = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_INTRO_DESC"),
          m_showAlertIcon = true,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK,
          m_responseCallback = (AlertPopup.ResponseCallback) ((r, data) =>
          {
            this.DoCreate();
            Options.Get().SetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, true);
            SetRotationManager.Get().SetRotationIntroProgress();
            Options.Get().SetBool(Option.NEEDS_TO_MAKE_STANDARD_DECK, false);
            UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
            Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN, true);
            Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK, true);
          })
        };
        DialogManager.Get().ShowPopup(info);
      }
      else
        this.DoCreate();
    }
    else
      this.DoCreate();
  }

  public override void EnableButton()
  {
    if (CraftingManager.Get().GetPendingClientTransaction().GetLastTransactionWasDisenchant())
    {
      this.EnterUndoMode();
    }
    else
    {
      EntityDef entityDef = CraftingManager.Get().GetShownActor().GetEntityDef();
      string cardId = entityDef.GetCardId();
      TAG_PREMIUM premium = CraftingManager.Get().GetShownActor().GetPremium();
      bool golden = CraftingManager.Get().CanUpgradeCardToGolden(cardId, premium);
      bool flag = CraftingManager.Get().CanCraftCardRightNow(entityDef, premium);
      if (golden & flag)
      {
        this.labelText.Text = GameStrings.Get("GLUE_CRAFTING_CREATE_UPGRADE");
        this.SetTextEnlargedForNoDustJarOnPhone(true);
        this.SetCraftingState(CraftingButton.CraftingState.CreateUpgrade);
      }
      else if (!golden & flag)
      {
        this.labelText.Text = GameStrings.Get("GLUE_CRAFTING_CREATE");
        this.SetTextEnlargedForNoDustJarOnPhone(false);
        this.SetCraftingState(CraftingButton.CraftingState.Create);
      }
      else if (golden && !flag)
      {
        this.labelText.Text = GameStrings.Get("GLUE_CRAFTING_UPGRADE");
        this.SetTextEnlargedForNoDustJarOnPhone(false);
        this.SetCraftingState(CraftingButton.CraftingState.Upgrade);
      }
      base.EnableButton();
    }
  }

  public override void EnterUndoMode()
  {
    this.SetTextEnlargedForNoDustJarOnPhone(false);
    base.EnterUndoMode();
  }

  private void DoCreate() => CraftingManager.Get().CreateButtonPressed();

  private void SetTextEnlargedForNoDustJarOnPhone(bool enlarge)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI || !((Object) this.m_labelTextNoDustJar != (Object) null))
      return;
    if (enlarge && !this.m_textEnlarged)
    {
      this.m_textEnlarged = true;
      this.m_labelTextNoDustJar.Text = this.labelText.Text;
      this.labelText.gameObject.SetActive(false);
      this.m_labelTextNoDustJar.gameObject.SetActive(true);
    }
    else
    {
      if (enlarge || !this.m_textEnlarged)
        return;
      this.m_textEnlarged = false;
      this.m_labelTextNoDustJar.gameObject.SetActive(false);
      this.labelText.gameObject.SetActive(true);
    }
  }
}
