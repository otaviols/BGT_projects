using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
  public UberText m_bankAmountText;
  public CreateButton m_buttonCreate;
  public DisenchantButton m_buttonDisenchant;
  public GameObject m_soulboundNotification;
  public UberText m_soulboundTitle;
  public UberText m_soulboundDesc;
  public UberText m_disenchantValue;
  public UberText m_craftValue;
  public GameObject m_wildTheming;
  [SerializeField]
  private GameObject m_createCostBar;
  [SerializeField]
  private GameObject m_disenchantCostBar;
  public float m_disenchantDelayBeforeCardExplodes;
  public float m_disenchantDelayBeforeCardFlips;
  public float m_disenchantDelayBeforeBallsComeOut;
  public float m_craftDelayBeforeConstructSpell;
  public float m_craftDelayBeforeGhostDeath;
  public GameObject m_glowballs;
  public SoundDef m_craftingSound;
  public SoundDef m_disenchantSound;
  public Collider m_mouseOverCollider;
  private Actor m_explodingActor;
  private Actor m_constructingActor;
  private bool m_isAnimating;
  private List<GameObject> m_thingsToDestroy = new List<GameObject>();
  private GameObject m_activeObject;
  private bool m_enabled;
  private bool m_mousedOver;
  private Notification m_craftNotification;
  private bool m_initializedPositions;

  private void Update()
  {
    if (!this.m_enabled)
      return;
    if (this.m_isAnimating)
    {
      this.m_mousedOver = false;
    }
    else
    {
      Ray ray = Camera.main.ScreenPointToRay(InputCollection.GetMousePosition());
      LayerMask layerMask1 = (LayerMask) 512;
      RaycastHit raycastHit;
      ref RaycastHit local = ref raycastHit;
      double farClipPlane = (double) Camera.main.farClipPlane;
      int layerMask2 = (int) layerMask1;
      if (!Physics.Raycast(ray, out local, (float) farClipPlane, layerMask2))
        return;
      if ((Object) raycastHit.collider == (Object) this.m_mouseOverCollider)
        this.NotifyOfMouseOver();
      else
        this.NotifyOfMouseOut();
    }
  }

  private void OnDisable() => this.StopCurrentAnim(true);

  public void UpdateWildTheming()
  {
    if ((Object) this.m_wildTheming == (Object) null)
      return;
    EntityDef entityDef;
    if (!CraftingManager.Get().GetShownCardInfo(out entityDef, out TAG_PREMIUM _))
      this.m_wildTheming.SetActive(false);
    else
      this.m_wildTheming.SetActive(GameUtils.IsWildCard(entityDef));
  }

  public void UpdateCraftingButtonsAndSoulboundText()
  {
    this.UpdateBankText();
    CraftingManager craftingManager = CraftingManager.Get();
    EntityDef entityDef;
    TAG_PREMIUM premium;
    if (!craftingManager.GetShownCardInfo(out entityDef, out premium))
    {
      this.m_buttonDisenchant.DisableButton();
      this.SetDisenchantCostBarActive(false);
      this.m_buttonCreate.DisableButton();
      this.SetCreateCostBarActive(false);
    }
    else
    {
      NetCache.CardDefinition cardDef = new NetCache.CardDefinition()
      {
        Name = entityDef.GetCardId(),
        Premium = premium
      };
      int ownedIncludePending = craftingManager.GetNumOwnedIncludePending();
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      TAG_CARD_SET cardSet1 = entityDef.GetCardSet();
      string cardSetName = GameStrings.GetCardSetName(cardSet1);
      NetCache.CardValue cardValue = craftingManager.GetCardValue(cardDef.Name, cardDef.Premium);
      string str1 = GameStrings.Get("GLUE_CRAFTING_SOULBOUND");
      string str2;
      if (ownedIncludePending <= 0)
      {
        str1 = cardSetName;
        str2 = Network.IsLoggedIn() ? entityDef.GetHowToEarnText(cardDef.Premium) : GameStrings.Get("GLUE_CRAFTING_SOULBOUND_OFFLINE_DESC");
      }
      else
      {
        CardSetDbfRecord cardSet2 = GameDbf.GetIndex().GetCardSet(cardSet1);
        str2 = !cardSet2.IsFeaturedCardSet ? (!cardSet2.IsCoreCardSet ? (Network.IsLoggedIn() ? GameStrings.Get("GLUE_CRAFTING_SOULBOUND_DESC") : GameStrings.Get("GLUE_CRAFTING_SOULBOUND_OFFLINE_DESC")) : GameStrings.Get("GLUE_CRAFTING_SOULBOUND_CORE_DESC")) : GameStrings.Get("GLUE_CRAFTING_SOULBOUND_FEATURED_DESC");
      }
      bool flag1 = craftingManager.GetNumClientTransactions() < 0;
      if (((craftingManager.CanCraftCardRightNow(entityDef, premium) ? 1 : (craftingManager.CanUpgradeCardToGolden(cardDef.Name, cardDef.Premium) ? 1 : 0)) | (flag1 ? 1 : 0)) != 0)
      {
        this.m_buttonCreate.EnableButton();
        this.SetCreateCostBarActive(this.m_buttonCreate.GetCraftingState() != CraftingButton.CraftingState.CreateUpgrade);
      }
      else
      {
        this.m_buttonCreate.DisableButton();
        this.SetCreateCostBarActive(false);
      }
      bool flag2;
      if (cardValue == null)
      {
        flag2 = false;
      }
      else
      {
        bool willBecomeActiveInFuture;
        if (CraftingUI.IsCraftingEventForCardActive(cardDef.Name, premium, out willBecomeActiveInFuture) && Network.IsLoggedIn())
        {
          int clientTransactions = craftingManager.GetNumClientTransactions();
          int num1 = this.m_buttonCreate.GetCraftingState() == CraftingButton.CraftingState.Upgrade ? this.GetUpgradeValue(cardDef) : cardValue.GetBuyValue();
          if (clientTransactions < 0)
            num1 = cardValue.GetSellValue();
          int num2 = cardValue.GetSellValue();
          if (clientTransactions > 0)
            num2 = craftingManager.GetPendingClientTransaction().GetLastTransactionWasUpgrade() ? this.GetUpgradeValue(cardDef) : cardValue.GetBuyValue();
          this.m_disenchantValue.Text = "+" + num2.ToString();
          this.m_craftValue.Text = "-" + num1.ToString();
          flag2 = true;
        }
        else
        {
          flag2 = false;
          if (willBecomeActiveInFuture)
          {
            str1 = GameStrings.Get("GLUE_CRAFTING_EVENT_NOT_ACTIVE_TITLE");
            str2 = GameStrings.Format("GLUE_CRAFTING_EVENT_NOT_ACTIVE_DESCRIPTION", (object) cardSetName);
          }
        }
      }
      this.m_soulboundTitle.Text = str1;
      this.m_soulboundDesc.Text = str2;
      if (!flag2)
      {
        this.m_buttonDisenchant.DisableButton();
        this.SetDisenchantCostBarActive(false);
        this.m_buttonCreate.DisableButton();
        this.SetCreateCostBarActive(false);
        this.m_soulboundNotification.SetActive(true);
        this.m_activeObject = this.m_soulboundNotification;
      }
      else if (!FixedRewardsMgr.Get().CanCraftCard(cardDef.Name, cardDef.Premium))
      {
        this.m_buttonDisenchant.DisableButton();
        this.SetDisenchantCostBarActive(false);
        this.m_buttonCreate.DisableButton();
        this.SetCreateCostBarActive(false);
        this.m_soulboundNotification.SetActive(true);
        this.m_activeObject = this.m_soulboundNotification;
      }
      else
      {
        this.m_soulboundNotification.SetActive(false);
        this.m_activeObject = this.gameObject;
        if (ownedIncludePending <= 0)
        {
          this.m_buttonDisenchant.DisableButton();
          this.SetDisenchantCostBarActive(false);
        }
        else
        {
          this.m_buttonDisenchant.EnableButton();
          this.SetDisenchantCostBarActive(true);
        }
      }
    }
  }

  public void DoDisenchant()
  {
    CraftingManager craftingManager = CraftingManager.Get();
    if (craftingManager.GetNumOwnedIncludePending() <= 0)
      return;
    this.UpdateTips();
    int num = this.m_buttonDisenchant.GetCraftingState() == CraftingButton.CraftingState.Undo ? 1 : 0;
    bool transactionWasUpgrade = craftingManager.GetPendingClientTransaction().GetLastTransactionWasUpgrade();
    bool wasUpgradeFromNormal = craftingManager.GetPendingClientTransaction().GetLastOperation() == CraftingPendingTransaction.Operation.UpgradeToGoldenFromNormal;
    int amount;
    if ((num & (transactionWasUpgrade ? 1 : 0)) != 0)
      craftingManager.TryGetCardUpgradeValue(craftingManager.GetShownActor().GetEntityDef().GetCardId(), out amount);
    else
      craftingManager.TryGetCardSellValue(craftingManager.GetShownActor().GetEntityDef().GetCardId(), craftingManager.GetShownActor().GetPremium(), out amount);
    craftingManager.AdjustUnCommitedArcaneDustChanges(amount);
    Options.Get().SetBool(Option.HAS_DISENCHANTED, true);
    craftingManager.NotifyOfTransaction(-1);
    this.UpdateCraftingButtonsAndSoulboundText();
    if (this.m_isAnimating)
      craftingManager.FinishFlipCurrentActorEarly();
    this.StopCurrentAnim();
    if ((num & (transactionWasUpgrade ? 1 : 0)) != 0)
      this.StartCoroutine(this.DoUndoUpgradeAnims(wasUpgradeFromNormal));
    else
      this.StartCoroutine(this.DoDisenchantAnims());
    craftingManager.StartCoroutine(this.StartCraftCooldown());
  }

  public void CleanUpEffects()
  {
    if ((Object) this.m_explodingActor != (Object) null)
    {
      Spell spell = this.m_explodingActor.GetSpell(SpellType.DECONSTRUCT);
      if ((Object) spell != (Object) null && spell.GetActiveState() != SpellStateType.NONE)
      {
        this.m_explodingActor.GetSpell(SpellType.DECONSTRUCT).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
        this.m_explodingActor.Hide();
      }
    }
    if ((Object) this.m_constructingActor != (Object) null)
    {
      Spell spell1 = this.m_constructingActor.GetSpell(SpellType.CONSTRUCT);
      if ((Object) spell1 != (Object) null && spell1.GetActiveState() != SpellStateType.NONE)
      {
        this.m_constructingActor.GetSpell(SpellType.CONSTRUCT).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
        this.m_constructingActor.Hide();
      }
      Spell spell2 = this.m_constructingActor.GetSpell(SpellType.DEATH_KNIGHT_CONSTRUCT);
      if ((Object) spell2 != (Object) null && spell2.GetActiveState() != SpellStateType.NONE)
      {
        this.m_constructingActor.GetSpell(SpellType.DEATH_KNIGHT_CONSTRUCT).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
        this.m_constructingActor.Hide();
      }
    }
    SoundManager.Get().Stop(this.m_craftingSound.GetComponent<AudioSource>());
    SoundManager.Get().Stop(this.m_disenchantSound.GetComponent<AudioSource>());
    this.GetComponent<PlayMakerFSM>().SendEvent("Cancel");
    this.m_isAnimating = false;
  }

  public void DoCreate(bool isUpgrade)
  {
    CraftingManager craftingManager = CraftingManager.Get();
    if (!craftingManager.GetShownCardInfo(out EntityDef _, out TAG_PREMIUM _))
      return;
    int num;
    if (isUpgrade)
      craftingManager.TryGetCardUpgradeValue(craftingManager.GetShownActor().GetEntityDef().GetCardId(), out num);
    else
      craftingManager.TryGetCardBuyValue(craftingManager.GetShownActor().GetEntityDef().GetCardId(), craftingManager.GetShownActor().GetPremium(), out num);
    craftingManager.AdjustUnCommitedArcaneDustChanges(-num);
    if (!Options.Get().GetBool(Option.HAS_CRAFTED))
      Options.Get().SetBool(Option.HAS_CRAFTED, true);
    this.UpdateTips();
    craftingManager.NotifyOfTransaction(1);
    if (craftingManager.GetNumOwnedIncludePending() > 1)
      craftingManager.ForceNonGhostFlagOn();
    this.UpdateCraftingButtonsAndSoulboundText();
    this.StopCurrentAnim();
    this.StartCoroutine(this.DoCreateAnims());
    craftingManager.StartCoroutine(this.StartDisenchantCooldown());
  }

  public void UpdateBankText()
  {
    this.m_bankAmountText.Text = NetCache.Get().GetArcaneDustBalance().ToString();
    BnetBar.Get().RefreshCurrency();
    if (!(bool) UniversalInputManager.UsePhoneUI || !((Object) CraftingTray.Get() != (Object) null))
      return;
    ArcaneDustAmount.Get().UpdateCurrentDustAmount();
  }

  public void Disable(Vector3 hidePosition)
  {
    this.m_enabled = false;
    iTween.MoveTo(this.m_activeObject, iTween.Hash((object) "time", (object) 0.4f, (object) "position", (object) hidePosition, (object) "oncomplete", (object) "FinishDisable"));
    this.HideTips();
    this.StopCurrentAnim(true);
  }

  public void FinishDisable() => this.m_activeObject.SetActive(this.m_enabled);

  public bool IsEnabled() => this.m_enabled;

  public void Enable(Vector3 showPosition, Vector3 hidePosition)
  {
    if (!this.m_initializedPositions)
    {
      this.transform.position = hidePosition;
      this.m_soulboundNotification.transform.position = this.transform.position;
      this.m_soulboundTitle.Text = GameStrings.Get("GLUE_CRAFTING_SOULBOUND");
      this.m_soulboundDesc.Text = GameStrings.Get("GLUE_CRAFTING_SOULBOUND_DESC");
      this.m_activeObject = this.gameObject;
      this.m_initializedPositions = true;
    }
    this.m_enabled = true;
    this.UpdateCraftingButtonsAndSoulboundText();
    this.UpdateWildTheming();
    this.m_activeObject.SetActive(true);
    iTween.MoveTo(this.m_activeObject, iTween.Hash((object) "time", (object) 0.5f, (object) "position", (object) showPosition));
    this.ShowFirstTimeTips();
  }

  public void SetStartingActive()
  {
    this.m_soulboundNotification.SetActive(false);
    this.gameObject.SetActive(false);
  }

  public void DoUpgradeToGoldenAnimations()
  {
    this.UpdateCraftingButtonsAndSoulboundText();
    this.StopCurrentAnim(true);
    this.StartCoroutine(this.DoCreateAnims());
    CraftingManager.Get().StartCoroutine(this.StartDisenchantCooldown());
  }

  private int GetUpgradeValue(NetCache.CardDefinition cardDef) => CraftingManager.Get().GetCardValue(cardDef.Name, TAG_PREMIUM.NORMAL).BaseUpgradeValue;

  private void ShowFirstTimeTips()
  {
    if ((Object) this.m_activeObject == (Object) this.m_soulboundNotification || Options.Get().GetBool(Option.HAS_CRAFTED) || !UserAttentionManager.CanShowAttentionGrabber("CraftingUI.ShowFirstTimeTips"))
      return;
    this.CreateCraftNotification();
  }

  private void CreateCraftNotification()
  {
    if (!this.m_buttonCreate.IsButtonEnabled() || this.m_buttonCreate.GetCraftingState() != CraftingButton.CraftingState.Create)
      return;
    Vector3 position;
    Notification.PopUpArrowDirection direction;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      position = new Vector3(73.3f, 1f, 55.4f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    else
    {
      position = new Vector3(55f, 1f, -56f);
      direction = Notification.PopUpArrowDirection.Left;
    }
    if ((Object) this.m_craftNotification == (Object) null)
      this.m_craftNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, 16f * Vector3.one, GameStrings.Get("GLUE_COLLECTION_TUTORIAL06"), false);
    if (!((Object) this.m_craftNotification != (Object) null))
      return;
    this.m_craftNotification.ShowPopUpArrow(direction);
  }

  private void UpdateTips()
  {
    if (Options.Get().GetBool(Option.HAS_CRAFTED) || !UserAttentionManager.CanShowAttentionGrabber("CraftingUI.UpdateTips") || this.m_buttonCreate.GetCraftingState() == CraftingButton.CraftingState.Upgrade || this.m_buttonCreate.GetCraftingState() == CraftingButton.CraftingState.CreateUpgrade)
      this.HideTips();
    else if ((Object) this.m_craftNotification == (Object) null)
    {
      this.CreateCraftNotification();
    }
    else
    {
      if (this.m_buttonCreate.IsButtonEnabled())
        return;
      NotificationManager.Get().DestroyNotification(this.m_craftNotification, 0.0f);
    }
  }

  private void HideTips()
  {
    if (!((Object) this.m_craftNotification != (Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_craftNotification);
  }

  private void NotifyOfMouseOver()
  {
    if (this.m_mousedOver)
      return;
    this.m_mousedOver = true;
    this.GetComponent<PlayMakerFSM>().SendEvent("Idle");
  }

  private void NotifyOfMouseOut()
  {
    if (!this.m_mousedOver)
      return;
    this.m_mousedOver = false;
    this.GetComponent<PlayMakerFSM>().SendEvent("IdleCancel");
  }

  public void SetCreateCostBarActive(bool active)
  {
    if ((Object) this.m_createCostBar == (Object) null)
      return;
    this.m_createCostBar.SetActive(active);
  }

  public void SetDisenchantCostBarActive(bool active)
  {
    if ((Object) this.m_disenchantCostBar == (Object) null)
      return;
    this.m_disenchantCostBar.SetActive(active);
  }

  public static bool IsCraftingEventForCardActive(
    string cardID,
    TAG_PREMIUM premium,
    out bool willBecomeActiveInFuture)
  {
    willBecomeActiveInFuture = false;
    if (GameUtils.IsClassicCard(cardID))
      return CraftingUI.IsCraftingEventForCardActive(GameUtils.TranslateDbIdToCardId(GameUtils.GetCardTagValue(cardID, GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID)), premium, out willBecomeActiveInFuture);
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardID);
    if (cardRecord == null)
    {
      Debug.LogWarning((object) string.Format("CraftingUI.IsCraftingEventForCardActive could not find DBF record for card {0}, assuming it cannot be crafted or disenchanted", (object) cardID));
      return false;
    }
    SpecialEventType eventType = cardRecord.CraftingEvent;
    if (premium == TAG_PREMIUM.GOLDEN)
    {
      if (cardRecord.GoldenCraftingEvent != SpecialEventType.UNKNOWN)
      {
        eventType = cardRecord.GoldenCraftingEvent;
      }
      else
      {
        CardSetDbfRecord cardSetRecord = GameUtils.GetCardSetRecord(cardID);
        if (cardSetRecord != null)
          eventType = cardSetRecord.ContentLaunchEvent;
      }
    }
    else if (eventType == SpecialEventType.UNKNOWN)
    {
      CardSetDbfRecord cardSetRecord = GameUtils.GetCardSetRecord(cardID);
      if (cardSetRecord != null)
        eventType = cardSetRecord.ContentLaunchEvent;
    }
    int num = SpecialEventManager.Get().IsEventActive(eventType, true) ? 1 : 0;
    if (num != 0)
      return num != 0;
    willBecomeActiveInFuture = SpecialEventManager.Get().IsStartTimeInTheFuture(eventType);
    return num != 0;
  }

  public bool GetIsAnimating() => this.m_isAnimating;

  private void StopCurrentAnim(bool forceCleanup = false)
  {
    if (!this.m_isAnimating && !forceCleanup)
      return;
    this.StopAllCoroutines();
    this.CleanUpEffects();
    foreach (GameObject gameObject in this.m_thingsToDestroy)
    {
      if (!((Object) gameObject == (Object) null))
      {
        Log.Crafting.Print("StopCurrentAnim: Destroying GameObject {0}", (object) gameObject);
        Object.Destroy((Object) gameObject);
      }
    }
  }

  private IEnumerator StartDisenchantCooldown()
  {
    Collider buttonDisenchatCollider = this.m_buttonDisenchant.GetComponent<Collider>();
    if (buttonDisenchatCollider.enabled)
    {
      buttonDisenchatCollider.enabled = false;
      yield return (object) new WaitForSeconds(1f);
      buttonDisenchatCollider.enabled = true;
    }
  }

  private IEnumerator StartCraftCooldown()
  {
    Collider buttonDisenchatCollider = this.m_buttonDisenchant.GetComponent<Collider>();
    if (buttonDisenchatCollider.enabled)
    {
      buttonDisenchatCollider.enabled = false;
      yield return (object) new WaitForSeconds(1f);
      buttonDisenchatCollider.enabled = true;
    }
  }

  private IEnumerator DoDisenchantAnims()
  {
    CraftingUI craftingUi = this;
    SoundManager.Get().Play(craftingUi.m_disenchantSound.GetComponent<AudioSource>());
    SoundManager.Get().Stop(craftingUi.m_craftingSound.GetComponent<AudioSource>());
    craftingUi.m_isAnimating = true;
    CraftingManager.Get().m_cardCountTab.gameObject.SetActive(false);
    PlayMakerFSM playmaker = craftingUi.GetComponent<PlayMakerFSM>();
    playmaker.SendEvent("Birth");
    yield return (object) new WaitForSeconds(craftingUi.m_disenchantDelayBeforeCardExplodes);
    while ((Object) CraftingManager.Get().GetShownActor() == (Object) null)
      yield return (object) null;
    craftingUi.m_explodingActor = CraftingManager.Get().GetShownActor();
    Actor oldActor = craftingUi.m_explodingActor;
    craftingUi.m_thingsToDestroy.Add(craftingUi.m_explodingActor.gameObject);
    Log.Crafting.Print("Adding {0} to thingsToDestroy", (object) craftingUi.m_explodingActor.gameObject);
    craftingUi.UpdateBankText();
    if (!CraftingManager.Get().IsCancelling())
    {
      CraftingManager.Get().LoadGhostActorIfNecessary();
      craftingUi.m_explodingActor.ActivateSpellBirthState(SpellType.DECONSTRUCT);
      yield return (object) new WaitForSeconds(craftingUi.m_disenchantDelayBeforeCardFlips);
      if (!CraftingManager.Get().IsCancelling())
      {
        CraftingManager.Get().FlipUpsideDownCard(craftingUi.m_explodingActor);
        yield return (object) new WaitForSeconds(craftingUi.m_disenchantDelayBeforeBallsComeOut);
        if (!CraftingManager.Get().IsCancelling())
        {
          playmaker.SendEvent("Action");
          yield return (object) new WaitForSeconds(1f);
          CraftingManager.Get().m_cardCountTab.gameObject.SetActive(true);
          craftingUi.m_isAnimating = false;
          yield return (object) new WaitForSeconds(10f);
          if ((Object) oldActor != (Object) null)
            Object.Destroy((Object) oldActor.gameObject);
        }
      }
    }
  }

  private IEnumerator DoUndoUpgradeAnims(bool wasUpgradeFromNormal)
  {
    CraftingUI craftingUi = this;
    SoundManager.Get().Play(craftingUi.m_disenchantSound.GetComponent<AudioSource>());
    SoundManager.Get().Stop(craftingUi.m_craftingSound.GetComponent<AudioSource>());
    craftingUi.m_isAnimating = true;
    CraftingManager.Get().m_cardCountTab.gameObject.SetActive(false);
    PlayMakerFSM playmaker = craftingUi.GetComponent<PlayMakerFSM>();
    playmaker.SendEvent("Birth");
    yield return (object) new WaitForSeconds(craftingUi.m_disenchantDelayBeforeCardExplodes);
    while ((Object) CraftingManager.Get().GetShownActor() == (Object) null)
      yield return (object) null;
    craftingUi.m_explodingActor = CraftingManager.Get().GetShownActor();
    Actor oldActor = craftingUi.m_explodingActor;
    craftingUi.m_thingsToDestroy.Add(craftingUi.m_explodingActor.gameObject);
    Log.Crafting.Print("Adding {0} to thingsToDestroy", (object) craftingUi.m_explodingActor.gameObject);
    craftingUi.UpdateBankText();
    if (!CraftingManager.Get().IsCancelling())
    {
      CraftingManager.Get().LoadGhostActorIfNecessary();
      craftingUi.m_explodingActor.ActivateSpellBirthState(SpellType.DECONSTRUCT);
      yield return (object) new WaitForSeconds(craftingUi.m_disenchantDelayBeforeCardFlips);
      if (!CraftingManager.Get().IsCancelling())
      {
        if (wasUpgradeFromNormal)
          CraftingManager.Get().SwitchPremiumView(TAG_PREMIUM.NORMAL);
        else
          CraftingManager.Get().FlipUpsideDownCard(craftingUi.m_explodingActor);
        yield return (object) new WaitForSeconds(craftingUi.m_disenchantDelayBeforeBallsComeOut);
        if (!CraftingManager.Get().IsCancelling())
        {
          playmaker.SendEvent("Action");
          yield return (object) new WaitForSeconds(1f);
          CraftingManager.Get().m_cardCountTab.gameObject.SetActive(true);
          craftingUi.m_isAnimating = false;
          if ((Object) oldActor != (Object) null)
            Object.Destroy((Object) oldActor.gameObject);
        }
      }
    }
  }

  private IEnumerator DoCreateAnims()
  {
    CraftingUI craftingUi = this;
    Actor shownActor = CraftingManager.Get().GetShownActor();
    SoundManager.Get().Play(craftingUi.m_craftingSound.GetComponent<AudioSource>());
    SoundManager.Get().Stop(craftingUi.m_disenchantSound.GetComponent<AudioSource>());
    craftingUi.m_isAnimating = true;
    CraftingManager.Get().HideAndDestroyRelatedBigCard();
    CraftingManager.Get().m_cardCountTab.gameObject.SetActive(false);
    CraftingManager.Get().FlipCurrentActor();
    craftingUi.GetComponent<PlayMakerFSM>().SendEvent("Birth");
    yield return (object) new WaitForSeconds(craftingUi.m_craftDelayBeforeConstructSpell);
    if (!CraftingManager.Get().IsCancelling())
    {
      craftingUi.m_constructingActor = CraftingManager.Get().LoadNewActorAndConstructIt();
      craftingUi.UpdateBankText();
      yield return (object) new WaitForSeconds(craftingUi.m_craftDelayBeforeGhostDeath);
      if (!CraftingManager.Get().IsCancelling())
      {
        if (shownActor.HasCardDef && shownActor.PlayEffectDef != null)
          GameUtils.PlayCardEffectDefSounds(shownActor.PlayEffectDef);
        CraftingManager.Get().m_cardCountTab.gameObject.SetActive(true);
        CraftingManager.Get().FinishCreateAnims();
        yield return (object) new WaitForSeconds(1f);
        craftingUi.m_isAnimating = false;
      }
    }
  }
}
