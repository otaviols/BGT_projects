using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class CollectionDeckBoxVisual : PegUIElement, IDraggableCollectionVisual
{
  public UberText m_deckName;
  public UberText m_deckDesc;
  public GameObject m_labelGradient;
  public PegUIElement m_deleteButton;
  public GameObject m_notificationButton;
  public GameObject m_highlight;
  public List<CollectionDeckBoxVisual.FormatElements> m_formatElements;
  public GameObject m_invalidCardCountIndicator;
  public UberText m_invalidCardCountIndicatorText;
  public int m_topBannerMaterialIndex;
  public GameObject m_pressedBone;
  public CustomDeckBones m_bones;
  public GameObject m_normalDeckVisuals;
  public GameObject m_lockedDeckVisuals;
  public TooltipZone m_tooltipZone;
  public GameObject m_renameVisuals;
  public bool m_neverUseGoldenPortraits;
  public Material m_defaultPortraitMaterial;
  public PlayMakerFSM m_DeckPortraitChangeFSM;
  [HideInInspector]
  public bool m_isLoanerDeck;
  public GameObject m_deckRunes;
  public RuneSlotVisual m_runeSlotVisual;
  public static readonly float POPPED_UP_LOCAL_Z = 0.0f;
  public static readonly Vector3 POPPED_DOWN_LOCAL_POS = new Vector3(0.0f, -0.8598533f, 0.0f);
  public const float DECKBOX_SCALE = 0.95f;
  public static readonly Vector3 SCALED_DOWN_LOCAL_SCALE = new Vector3(0.95f, 0.95f, 0.95f);
  public const float SCALED_UP_LOCAL_Y_OFFSET = 3.238702f;
  public const float SCALED_DOWN_LOCAL_Y_OFFSET = 1.273138f;
  private const float BUTTON_POP_SPEED = 6f;
  private const string DECKBOX_POPUP_ANIM_NAME = "Deck_PopUp";
  private const string DECKBOX_POPDOWN_ANIM_NAME = "Deck_PopDown";
  private const string DECKBOX_DESATURATION_ANIM_NAME = "CustomDeck_Desat";
  private Vector3 SCALED_UP_DECK_OFFSET = new Vector3(0.0f, 0.0f, 0.0f);
  private const float SCALE_TIME = 0.2f;
  private const float ADJUST_Y_OFFSET_ANIM_TIME = 0.05f;
  private static readonly Color DECK_DESC_ENABLED_COLOR = new Color(0.97f, 0.82f, 0.22f);
  private static readonly Color DECK_NAME_ENABLED_COLOR = Color.white;
  private static float DEATH_KNIGHT_EDITED_DECK_BOX_COLLIDER_HEIGHT = 2.75f;
  private long m_deckID = -1;
  private int m_deckTemplateId;
  private bool m_isPoppedUp;
  private bool m_isShown;
  private DefLoader.DisposableFullDef m_fullDef;
  private bool m_isShared;
  private HighlightState m_highlightState;
  private string m_heroCardID = "";
  private TAG_PREMIUM? m_heroCardPremiumOverride;
  private FormatType m_formatType = FormatType.FT_STANDARD;
  private Vector3 m_originalButtonPosition;
  private Quaternion m_originalButtonRotation;
  private bool m_animateButtonPress = true;
  private bool m_wasTouchModeEnabled;
  private int m_positionIndex;
  private bool m_showGlow;
  private bool m_isLocked;
  private bool m_forceSingleLineDeckName;
  private bool m_isSelected;
  private float m_wiggleIntensity;
  private bool m_showBanner = true;
  private bool m_isShowingInvalidCardCount;
  private CollectionDeck.CardCountByStatus m_cardCountByStatus;
  private ILegendaryHeroPortrait m_legendaryHeroPortrait;
  private Transform m_customDeckTransform;
  private IGraphicsManager m_graphicsManager;
  private BoxCollider m_boxCollider;
  private Vector3 m_originalBoxColliderSize;

  public static Vector3 SCALED_UP_LOCAL_SCALE { get; private set; }

  protected override void Awake()
  {
    base.Awake();
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.SetEnabled(false, false);
    this.m_deleteButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeleteButtonPressed));
    this.m_deleteButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDeleteButtonOver));
    this.m_deleteButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnDeleteButtonRollout));
    this.ShowDeleteButton(false);
    this.ShowNotificationButton(false);
    this.UpdateInvalidCardCountIndicator();
    this.m_deckName.RichText = false;
    this.m_deckName.TextColor = CollectionDeckBoxVisual.DECK_NAME_ENABLED_COLOR;
    this.m_deckDesc.TextColor = CollectionDeckBoxVisual.DECK_DESC_ENABLED_COLOR;
    SoundManager.Get().Load((AssetReference) "tiny_button_press_1.prefab:44fc68b7418870b4797b85f0ca88a8db");
    SoundManager.Get().Load((AssetReference) "tiny_button_mouseover_1.prefab:0ab88a13f5168ed43a3b53275114a842");
    this.m_customDeckTransform = this.transform.Find("CustomDeck");
    this.SetHighlightRoot();
    CollectionDeckBoxVisual.SCALED_UP_LOCAL_SCALE = new Vector3(1.126f, 1.126f, 1.126f);
    if (PlatformSettings.s_screen == ScreenCategory.Phone)
    {
      CollectionDeckBoxVisual.SCALED_UP_LOCAL_SCALE = new Vector3(1.1f, 1.1f, 1.1f);
      this.SCALED_UP_DECK_OFFSET = new Vector3(0.0f, -0.2f, 0.0f);
    }
    if ((bool) (UnityEngine.Object) CollectionDeckBoxVisual.TeamsContent)
      this.SetFormatType(FormatType.FT_STANDARD);
    this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
    this.m_boxCollider = this.GetComponent<BoxCollider>();
    this.m_originalBoxColliderSize = this.m_boxCollider.size;
  }

  protected override void OnDestroy()
  {
    this.m_fullDef?.Dispose();
    this.m_fullDef = (DefLoader.DisposableFullDef) null;
    this.m_legendaryHeroPortrait?.Dispose();
    this.m_legendaryHeroPortrait = (ILegendaryHeroPortrait) null;
    base.OnDestroy();
  }

  private void Update()
  {
    if (this.m_wasTouchModeEnabled != UniversalInputManager.Get().IsTouchMode())
    {
      PegUIElement.InteractionState interactionState = this.GetInteractionState();
      if (this.m_wasTouchModeEnabled)
      {
        switch (interactionState)
        {
          case PegUIElement.InteractionState.Over:
            this.OnOverEvent();
            break;
          case PegUIElement.InteractionState.Down:
            this.OnPressEvent();
            break;
        }
      }
      else
      {
        switch (interactionState)
        {
          case PegUIElement.InteractionState.Over:
            this.OnOutEvent();
            break;
          case PegUIElement.InteractionState.Down:
            this.OnReleaseEvent();
            break;
        }
        this.ShowDeleteButton(false);
      }
      this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
    }
    if (!(bool) (UnityEngine.Object) this.ButtonGameObject)
      return;
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    float num4 = 0.0f;
    Vector3 axis = Vector3.zero;
    bool flag = false;
    if ((bool) (UnityEngine.Object) CollectionDeckBoxVisual.DecksContent || (bool) (UnityEngine.Object) CollectionDeckBoxVisual.TeamsContent)
    {
      DeckTrayReorderableContent reorderableContent = (UnityEngine.Object) CollectionDeckBoxVisual.TeamsContent != (UnityEngine.Object) null ? (DeckTrayReorderableContent) CollectionDeckBoxVisual.TeamsContent : (DeckTrayReorderableContent) CollectionDeckBoxVisual.DecksContent;
      num1 = reorderableContent.m_rearrangeStartStopTweenDuration;
      num2 = reorderableContent.m_rearrangeStartStopTweenDuration;
      num3 = reorderableContent.m_rearrangeWiggleFrequency;
      num4 = reorderableContent.m_rearrangeWiggleAmplitude;
      axis = reorderableContent.m_rearrangeWiggleAxis;
      flag = reorderableContent.DraggingDeckBox != null && reorderableContent.DraggingDeckBox != this;
    }
    int num5 = (double) this.m_wiggleIntensity > 0.0 ? 1 : 0;
    this.m_wiggleIntensity = !flag ? Mathf.Clamp01(this.m_wiggleIntensity - Time.deltaTime / num2) : Mathf.Clamp01(this.m_wiggleIntensity + Time.deltaTime / num1);
    int num6 = (double) this.m_wiggleIntensity > 0.0 ? 1 : 0;
    if ((num5 | num6) == 0)
      return;
    this.ButtonGameObject.transform.localRotation = Quaternion.AngleAxis(num4 * this.m_wiggleIntensity * Mathf.Cos((float) this.m_positionIndex + Time.time * num3), axis) * this.m_originalButtonRotation;
  }

  public void Show()
  {
    this.gameObject.SetActive(true);
    this.m_isShown = true;
  }

  public void Hide()
  {
    this.gameObject.SetActive(false);
    this.m_isShown = false;
  }

  public bool IsShown() => this.m_isShown;

  public void SetDeckName(string deckName) => this.m_deckName.Text = deckName;

  public UberText GetDeckNameText() => this.m_deckName;

  public void HideDeckName() => this.m_deckName.gameObject.SetActive(false);

  public void ShowDeckName() => this.m_deckName.gameObject.SetActive(true);

  public void HideRenameVisuals()
  {
    if (!((UnityEngine.Object) this.m_renameVisuals != (UnityEngine.Object) null))
      return;
    this.m_renameVisuals.SetActive(false);
  }

  public void ShowRenameVisuals()
  {
    if (CollectionManagerDisplay.IsSpecialOneDeckMode() || !((UnityEngine.Object) this.m_renameVisuals != (UnityEngine.Object) null))
      return;
    this.m_renameVisuals.SetActive(true);
  }

  public void SetDeckID(long id) => this.m_deckID = id;

  public void SetDeckTemplateId(int id) => this.m_deckTemplateId = id;

  public int GetDeckTemplateId() => this.m_deckTemplateId;

  public long GetDeckID() => this.m_deckID;

  public CollectionDeck GetCollectionDeck()
  {
    if (this.IsShared())
    {
      List<CollectionDeck> sharedDecks = FriendChallengeMgr.Get().GetSharedDecks();
      if (sharedDecks != null)
        return sharedDecks.Find((Predicate<CollectionDeck>) (deck => deck.ID == this.m_deckID));
    }
    FreeDeckMgr freeDeckMgr = FreeDeckMgr.Get();
    if (freeDeckMgr != null && freeDeckMgr.Status == FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD)
    {
      CollectionDeck fromDeckTemplateId = freeDeckMgr.GetLoanerDeckFromDeckTemplateId(this.m_deckTemplateId);
      if (fromDeckTemplateId != null)
        return fromDeckTemplateId;
    }
    return CollectionManager.Get().GetDeck(this.m_deckID);
  }

  public DefLoader.DisposableFullDef SharedDisposableFullDef() => this.m_fullDef?.Share();

  public bool HasFullDef() => this.m_fullDef != null;

  public string GetHeroCardID() => this.m_heroCardID;

  public bool IsLoading() => this.GetDeckID() > 0L && this.m_heroCardID != "None" && !this.HasFullDef();

  public bool SetHeroCardID(string heroCardID, TAG_PREMIUM? premiumOverride = null)
  {
    if (string.IsNullOrEmpty(heroCardID) || heroCardID.Equals("None"))
    {
      this.m_heroCardID = "None";
      return false;
    }
    if (!(this.m_heroCardID != heroCardID))
    {
      TAG_PREMIUM? nullable = premiumOverride;
      TAG_PREMIUM? cardPremiumOverride = this.m_heroCardPremiumOverride;
      if (nullable.GetValueOrDefault() == cardPremiumOverride.GetValueOrDefault() & nullable.HasValue == cardPremiumOverride.HasValue)
        return false;
    }
    this.m_heroCardID = heroCardID;
    this.m_heroCardPremiumOverride = premiumOverride;
    TAG_PREMIUM heroCardPremium = this.GetHeroCardPremium();
    if (this.m_heroCardPremiumOverride.HasValue)
      heroCardPremium = this.m_heroCardPremiumOverride.Value;
    CardPortraitQuality quality = new CardPortraitQuality(3, heroCardPremium);
    DefLoader.Get().LoadFullDef(heroCardID, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroFullDefLoaded), quality: quality);
    return true;
  }

  public bool SetHeroCardIdFromDeck()
  {
    CollectionDeck collectionDeck = this.GetCollectionDeck();
    return collectionDeck != null && this.SetHeroCardID(collectionDeck.GetDisplayHeroCardID(true));
  }

  public void SetHeroCardPremiumOverride(TAG_PREMIUM? premium) => this.m_heroCardPremiumOverride = premium;

  public TAG_PREMIUM GetHeroCardPremium()
  {
    if (this.m_heroCardPremiumOverride.HasValue)
      return this.m_heroCardPremiumOverride.Value;
    TAG_CLASS tagClassFromCardId = GameUtils.GetTagClassFromCardId(this.m_heroCardID);
    return CollectionManager.Get().GetHeroPremium(tagClassFromCardId);
  }

  public void SetShowGlow(bool showGlow)
  {
    this.m_showGlow = showGlow;
    if (!this.m_showGlow)
      return;
    this.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }

  public FormatType GetFormatType() => this.m_formatType;

  public void PlayGlowAnim()
  {
    Animator component;
    if (!this.TryGetComponent<Animator>(out component))
      return;
    component.enabled = true;
    component.Play("CustomDeck_GlowOut", 0, 0.0f);
  }

  public void OnGlowAnimPeak()
  {
    CollectionDeck collectionDeck = this.GetCollectionDeck();
    if (collectionDeck == null || collectionDeck.FormatType != FormatType.FT_WILD)
      return;
    this.m_formatType = collectionDeck.FormatType;
    this.ReparentElements(this.m_formatType);
    CollectionDeckBoxVisual.FormatElements activeFormatElements = this.GetActiveFormatElements();
    CollectionDeckBoxVisual.FormatElements[] inactiveFormatElements = this.GetInactiveFormatElements();
    this.m_highlightState.m_StaticSilouetteTexture = activeFormatElements.highlight;
    foreach (CollectionDeckBoxVisual.FormatElements formatElements in inactiveFormatElements)
    {
      if ((UnityEngine.Object) formatElements.portraitObject != (UnityEngine.Object) null)
        formatElements.portraitObject.SetActive(false);
    }
    if (!((UnityEngine.Object) activeFormatElements.portraitObject != (UnityEngine.Object) null))
      return;
    activeFormatElements.portraitObject.SetActive(true);
    Animator component = activeFormatElements.portraitObject.GetComponent<Animator>();
    if (!(bool) UniversalInputManager.UsePhoneUI)
      component.Play("Wild_RolldownActivate", 0, 1f);
    else
      component.Play("WildActivate", 0, 1f);
  }

  public void SetFormatType(FormatType formatType)
  {
    this.m_formatType = formatType;
    this.ReparentElements(formatType);
    this.UpdateVisualBannerState();
    CollectionDeckBoxVisual.FormatElements activeFormatElements = this.GetActiveFormatElements();
    RendererExtension.SetMaterial(this.m_deleteButton.GetComponent<Renderer>(), activeFormatElements.xButtonMaterial);
    this.m_highlightState.m_StaticSilouetteTexture = activeFormatElements.highlight;
    foreach (CollectionDeckBoxVisual.FormatElements formatElement in this.m_formatElements)
    {
      if ((UnityEngine.Object) formatElement.portraitObject != (UnityEngine.Object) null)
        formatElement.portraitObject.SetActive(formatElement.formatType == formatType);
    }
  }

  public void SetPositionIndex(int idx) => this.m_positionIndex = idx;

  public int GetPositionIndex() => this.m_positionIndex;

  public void UpdateDeckLabel()
  {
    bool flag = false;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER || SceneMgr.Get().IsInLettuceMode() || this.IsShared() || this.m_heroCardPremiumOverride.HasValue || this.m_forceSingleLineDeckName || !this.IsDeckEnabled())
      flag = true;
    else if (this.m_isShowingInvalidCardCount)
    {
      if (this.m_cardCountByStatus.Extra > 0)
        this.m_deckDesc.Text = GameStrings.FormatPlurals("GLUE_COLLECTION_DECK_EXTRA_CARDS_LABEL", GameStrings.MakePlurals(this.m_cardCountByStatus.Extra));
      else
        this.m_deckDesc.Text = GameStrings.FormatPlurals("GLUE_COLLECTION_DECK_MISSING_CARDS_LABEL", GameStrings.MakePlurals(this.m_cardCountByStatus.MissingPlusInvalid));
    }
    else if (this.m_fullDef?.EntityDef != null)
      flag = true;
    if (flag)
    {
      this.SetDeckNameAsSingleLine(false);
    }
    else
    {
      this.m_deckName.transform.position = this.m_bones.m_deckLabelTwoLine.position;
      this.m_labelGradient.transform.parent = this.m_bones.m_gradientTwoLine;
      this.m_labelGradient.transform.localPosition = Vector3.zero;
      this.m_labelGradient.transform.localScale = Vector3.one;
      this.m_deckDesc.gameObject.SetActive(true);
    }
  }

  public void SetDeckNameAsSingleLine(bool forceSingleLine)
  {
    if (forceSingleLine)
      this.m_forceSingleLineDeckName = true;
    if ((UnityEngine.Object) this.m_deckName == (UnityEngine.Object) null || this.m_bones == null || (UnityEngine.Object) this.m_labelGradient == (UnityEngine.Object) null || (UnityEngine.Object) this.m_deckDesc?.gameObject == (UnityEngine.Object) null)
      return;
    this.m_deckName.transform.position = this.m_bones.m_deckLabelOneLine.position;
    this.m_labelGradient.transform.parent = this.m_bones.m_gradientOneLine;
    this.m_labelGradient.transform.localPosition = Vector3.zero;
    this.m_labelGradient.transform.localScale = Vector3.one;
    this.m_deckDesc.gameObject.SetActive(false);
  }

  public bool IsDeckEnabled()
  {
    if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() == (UnityEngine.Object) null)
      return true;
    CollectionDeck collectionDeck = this.GetCollectionDeck();
    return collectionDeck != null && collectionDeck.IsValidForModeAndFormat(SceneMgr.Get().GetMode(), Options.GetInRankedPlayMode(), Options.GetFormatType());
  }

  public bool CanSelectDeck()
  {
    CollectionDeck collectionDeck = this.GetCollectionDeck();
    return collectionDeck != null && this.IsDeckEnabled() && collectionDeck.IsValidForRuleset && (SceneMgr.Get().GetMode() != SceneMgr.Mode.TOURNAMENT || !Options.GetInRankedPlayMode() || collectionDeck.FormatType != FormatType.FT_STANDARD || Options.GetFormatType() != FormatType.FT_WILD || collectionDeck.GetTotalInvalidCardCount(new FormatType?(FormatType.FT_WILD)) <= 0);
  }

  private bool CanShowInvalidCardCountIndicator()
  {
    CollectionDeck collectionDeck = this.GetCollectionDeck();
    return collectionDeck != null && collectionDeck.NetworkContentsLoaded() && !collectionDeck.IsBeingEdited() && GameUtils.IsCardGameplayEventActive(collectionDeck.HeroCardID) && !collectionDeck.GetRuleset().EntityInDeckIgnoresRuleset(collectionDeck) && this.IsDeckEnabled();
  }

  public FormatType? GetFormatTypeToValidateAgainst()
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && Options.GetInRankedPlayMode())
      return new FormatType?(Options.GetFormatType());
    return this.GetCollectionDeck()?.FormatType;
  }

  public void UpdateInvalidCardCountIndicator()
  {
    this.m_isShowingInvalidCardCount = false;
    this.m_cardCountByStatus = (CollectionDeck.CardCountByStatus) null;
    if (this.CanShowInvalidCardCountIndicator())
    {
      this.m_cardCountByStatus = this.GetCollectionDeck().CountCardsByStatus(this.GetFormatTypeToValidateAgainst());
      int num = this.m_cardCountByStatus.Extra <= 0 ? this.m_cardCountByStatus.Valid : this.m_cardCountByStatus.Total;
      if (num != this.m_cardCountByStatus.Max)
      {
        this.m_isShowingInvalidCardCount = true;
        this.m_invalidCardCountIndicatorText.Text = GameStrings.Format("GLUE_COLLECTION_DECK_MISSING_CARDS_INDICATOR", (object) num, (object) this.m_cardCountByStatus.Max);
      }
    }
    this.m_invalidCardCountIndicator.SetActive(this.m_isShowingInvalidCardCount);
    this.UpdateDeckLabel();
  }

  public bool IsShared() => this.m_isShared;

  public void SetIsShared(bool isShared)
  {
    if (this.m_isShared == isShared)
      return;
    this.m_isShared = isShared;
    this.UpdateDeckLabel();
  }

  public bool IsLocked() => this.m_isLocked;

  public void SetIsLocked(bool isLocked)
  {
    if (this.m_isLocked == isLocked)
      return;
    this.m_isLocked = isLocked;
    this.m_normalDeckVisuals.SetActive(!this.m_isLocked);
    this.m_lockedDeckVisuals.SetActive(this.m_isLocked);
    this.SetHighlightRoot();
  }

  public void SetHighlightRoot()
  {
    if (this.m_isLocked)
      this.m_highlightState = this.m_lockedDeckVisuals.GetComponentInChildren<HighlightState>();
    else
      this.m_highlightState = this.m_normalDeckVisuals.GetComponentInChildren<HighlightState>();
  }

  public bool IsSelected() => this.m_isSelected;

  public void SetIsSelected(bool isSelected)
  {
    if (this.m_isSelected == isSelected)
      return;
    this.m_isSelected = isSelected;
    if (this.m_isSelected || !((UnityEngine.Object) this.m_tooltipZone != (UnityEngine.Object) null))
      return;
    this.m_tooltipZone.HideTooltip();
  }

  public void EnableButtonAnimation() => this.m_animateButtonPress = true;

  public void DisableButtonAnimation() => this.m_animateButtonPress = false;

  public void PlayScaleUpAnimation() => this.PlayScaleUpAnimation((CollectionDeckBoxVisual.DelOnAnimationFinished) null);

  public void PlayScaleUpAnimation(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback)
  {
    this.PlayScaleUpAnimation(callback, (object) null);
  }

  public void PlayScaleUpAnimation(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback,
    object callbackData)
  {
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.transform.localPosition with
    {
      y = 3.238702f
    }), (object) "isLocal", (object) true, (object) "time", (object) 0.05f, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "ScaleUpNow", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) new CollectionDeckBoxVisual.OnScaleFinishedCallbackData()
    {
      m_callback = callback,
      m_callbackData = callbackData
    }));
  }

  private void ScaleUpNow(
    CollectionDeckBoxVisual.OnScaleFinishedCallbackData readyToScaleUpData)
  {
    this.ScaleDeckBox(true, readyToScaleUpData.m_callback, readyToScaleUpData.m_callbackData);
  }

  public void PlayScaleDownAnimation() => this.PlayScaleDownAnimation((CollectionDeckBoxVisual.DelOnAnimationFinished) null);

  public void PlayScaleDownAnimation(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback)
  {
    this.PlayScaleDownAnimation(callback, (object) null);
  }

  public void PlayScaleDownAnimation(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback,
    object callbackData)
  {
    this.ScaleDeckBox(false, new CollectionDeckBoxVisual.DelOnAnimationFinished(this.OnScaledDown), (object) new CollectionDeckBoxVisual.OnScaleFinishedCallbackData()
    {
      m_callback = callback,
      m_callbackData = callbackData
    });
  }

  private void OnScaledDown(object callbackData) => iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.transform.localPosition with
  {
    y = 1.273138f
  }), (object) "isLocal", (object) true, (object) "time", (object) 0.05f, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "ScaleDownComplete", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) (callbackData as CollectionDeckBoxVisual.OnScaleFinishedCallbackData)));

  private void ScaleDownComplete(
    CollectionDeckBoxVisual.OnScaleFinishedCallbackData onScaledDownData)
  {
    if (onScaledDownData.m_callback == null)
      return;
    onScaledDownData.m_callback(onScaledDownData.m_callbackData);
  }

  public void PlayPopUpAnimation() => this.PlayPopUpAnimation((CollectionDeckBoxVisual.DelOnAnimationFinished) null);

  public void PlayPopUpAnimation(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback)
  {
    this.PlayPopUpAnimation(callback, (object) null);
  }

  public void PlayPopUpAnimation(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback,
    object callbackData)
  {
    if (this.m_isPoppedUp)
    {
      if (callback == null)
        return;
      callback(callbackData);
    }
    else
    {
      this.m_isPoppedUp = true;
      if ((UnityEngine.Object) this.m_customDeckTransform != (UnityEngine.Object) null)
        this.m_customDeckTransform.localPosition += this.SCALED_UP_DECK_OFFSET;
      Animation component = this.GetComponent<Animation>();
      component["Deck_PopUp"].time = 0.0f;
      component["Deck_PopUp"].speed = 6f;
      this.PlayPopAnimation("Deck_PopUp", callback, callbackData);
    }
  }

  public void PlayDesaturationAnimation()
  {
    Animator component = this.GetComponent<Animator>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.enabled = true;
    component.Play("CustomDeck_Desat", 0, 0.0f);
  }

  public void PlayPopDownAnimation() => this.PlayPopDownAnimation((CollectionDeckBoxVisual.DelOnAnimationFinished) null);

  public void PlayPopDownAnimation(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback)
  {
    this.PlayPopDownAnimation(callback, (object) null);
  }

  public void PlayPopDownAnimation(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback,
    object callbackData)
  {
    if (!this.m_isPoppedUp)
    {
      if (callback == null)
        return;
      callback(callbackData);
    }
    else
    {
      this.m_isPoppedUp = false;
      if ((UnityEngine.Object) this.m_customDeckTransform != (UnityEngine.Object) null)
        this.m_customDeckTransform.localPosition -= this.SCALED_UP_DECK_OFFSET;
      Animation component = this.GetComponent<Animation>();
      component["Deck_PopDown"].time = 0.0f;
      component["Deck_PopDown"].speed = 6f;
      this.PlayPopAnimation("Deck_PopDown", callback, callbackData);
    }
  }

  public void PlayPopDownAnimationImmediately() => this.PlayPopDownAnimationImmediately((CollectionDeckBoxVisual.DelOnAnimationFinished) null);

  public void PlayPopDownAnimationImmediately(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback)
  {
    this.PlayPopDownAnimationImmediately(callback, (object) null);
  }

  public void PlayPopDownAnimationImmediately(
    CollectionDeckBoxVisual.DelOnAnimationFinished callback,
    object callbackData)
  {
    if (!this.m_isPoppedUp)
    {
      if (callback == null)
        return;
      callback(callbackData);
    }
    else
    {
      this.m_isPoppedUp = false;
      Animation component = this.GetComponent<Animation>();
      component["Deck_PopDown"].time = component["Deck_PopDown"].length;
      component["Deck_PopDown"].speed = 1f;
      this.PlayPopAnimation("Deck_PopDown", callback, callbackData);
    }
  }

  public void SetHighlightMaterialForState(Material mat, ActorStateType stateType)
  {
    bool flag = false;
    foreach (HighlightRenderState highlightState in this.m_highlightState.m_HighlightStates)
    {
      if (highlightState.m_StateType == stateType)
      {
        flag = true;
        highlightState.m_Material = mat;
      }
    }
    if (flag)
      return;
    Log.All.PrintWarning("CollectionDeckBoxVisual - Attempting to set new material for state {0}, but no HighlightRenderState object found for that state type!", (object) stateType);
  }

  public void SetHighlightState(ActorStateType stateType)
  {
    if (!((UnityEngine.Object) this.m_highlightState != (UnityEngine.Object) null))
      return;
    if (!this.m_highlightState.IsReady())
      this.StartCoroutine(this.ChangeHighlightStateWhenReady(stateType));
    else
      this.m_highlightState.ChangeState(stateType);
  }

  private IEnumerator ChangeHighlightStateWhenReady(ActorStateType stateType)
  {
    while (!this.m_highlightState.IsReady())
      yield return (object) null;
    this.m_highlightState.ChangeState(stateType);
  }

  public void ShowDeleteButton(bool show)
  {
    this.m_deleteButton.gameObject.SetActive(show);
    if (!this.m_isShowingInvalidCardCount)
      return;
    this.m_invalidCardCountIndicator.SetActive(!show);
  }

  public void ShowNotificationButton(bool show)
  {
    if ((bool) (UnityEngine.Object) this.m_notificationButton)
      this.m_notificationButton.SetActive(show);
    else
      Log.CollectionDeckBox.PrintError("ShowNotificationButton - m_notificationButton is null");
  }

  public void UpdateRuneSlotVisual(CollectionDeck deck)
  {
    if (deck == null || !deck.HasClass(TAG_CLASS.DEATHKNIGHT))
      this.m_runeSlotVisual.Hide();
    else if (CollectionManager.Get().IsInEditMode())
      this.m_runeSlotVisual.Hide();
    else
      this.m_runeSlotVisual.Show(deck.GetRuneOrder());
  }

  public void UpdateColliderHeightForDeathKnight()
  {
    if (!CollectionManager.Get().IsEditingDeathKnightDeck() || (UnityEngine.Object) this.m_boxCollider == (UnityEngine.Object) null)
      return;
    this.m_boxCollider.size = this.m_boxCollider.size with
    {
      y = CollectionDeckBoxVisual.DEATH_KNIGHT_EDITED_DECK_BOX_COLLIDER_HEIGHT
    };
  }

  public void ResetColliderHeight() => this.m_boxCollider.size = this.m_originalBoxColliderSize;

  public void StoreOriginalButtonPositionAndRotation()
  {
    if (!((UnityEngine.Object) this.ButtonGameObject != (UnityEngine.Object) null))
      return;
    this.m_originalButtonPosition = this.ButtonGameObject.transform.localPosition;
    this.m_originalButtonRotation = this.ButtonGameObject.transform.localRotation;
  }

  public void HideBanner() => this.ShowBannerInternal(false);

  public void ShowBanner() => this.ShowBannerInternal(true);

  public void AssignFromCollectionDeck(CollectionDeck deck, bool rerollFavoriteHero)
  {
    if (deck == null)
      return;
    this.SetIsShared(deck.IsShared);
    this.SetDeckName(deck.Name);
    this.SetDeckID(deck.ID);
    this.SetHeroCardPremiumOverride(deck.GetDisplayHeroPremiumOverride());
    this.SetHeroCardID(deck.GetDisplayHeroCardID(rerollFavoriteHero));
    this.SetShowGlow(CollectionDeckBoxVisual.ShouldHighlightDeck(deck));
    this.SetFormatType(CollectionManager.Get().GetThemeShowing(deck));
    this.UpdateInvalidCardCountIndicator();
    this.UpdateRuneSlotVisual(deck);
  }

  public void AssignFromMercenariesTeam(LettuceTeam team, bool suppressFX = false)
  {
    if (team == null)
      return;
    this.SetDeckName(team.Name);
    this.SetDeckID(team.ID);
    LettuceMercenary leader = team.GetLeader();
    string heroCardID = (string) null;
    TAG_PREMIUM tagPremium = TAG_PREMIUM.NORMAL;
    if (leader != null)
    {
      LettuceMercenary.Loadout teamLoadout = leader.GetTeamLoadout(team);
      heroCardID = teamLoadout.GetCardId();
      tagPremium = teamLoadout.m_artVariationPremium;
    }
    bool flag = !string.IsNullOrEmpty(this.GetHeroCardID()) && !suppressFX;
    if (!(this.SetHeroCardID(heroCardID, new TAG_PREMIUM?(tagPremium)) & flag) || !((UnityEngine.Object) this.m_DeckPortraitChangeFSM != (UnityEngine.Object) null))
      return;
    this.m_DeckPortraitChangeFSM.SendEvent("Dissolve");
  }

  private CollectionDeckBoxVisual.FormatElements GetFormatElements(
    FormatType formatType)
  {
    if (this.m_deckID == 0L && formatType == FormatType.FT_UNKNOWN)
      return this.GetStandardFormatElements();
    CollectionDeckBoxVisual.FormatElements formatElements = (CollectionDeckBoxVisual.FormatElements) null;
    int index = 0;
    for (int count = this.m_formatElements.Count; index < count; ++index)
    {
      CollectionDeckBoxVisual.FormatElements formatElement = this.m_formatElements[index];
      if (formatElement.formatType == formatType)
      {
        formatElements = formatElement;
        break;
      }
    }
    if (formatElements != null)
      return formatElements;
    Log.CollectionDeckBox.PrintError("Unsupported format type in CollectionDeckBoxVisual.GetFormatElements: " + formatType.ToString() + ". Will use standard formatting.");
    return this.GetStandardFormatElements();
  }

  private CollectionDeckBoxVisual.FormatElements GetStandardFormatElements() => this.m_formatElements.Where<CollectionDeckBoxVisual.FormatElements>((Func<CollectionDeckBoxVisual.FormatElements, bool>) (x => x.formatType == FormatType.FT_STANDARD)).FirstOrDefault<CollectionDeckBoxVisual.FormatElements>();

  private CollectionDeckBoxVisual.FormatElements GetActiveFormatElements() => this.GetFormatElements(this.m_formatType);

  private CollectionDeckBoxVisual.FormatElements[] GetInactiveFormatElements() => this.m_formatElements.Where<CollectionDeckBoxVisual.FormatElements>((Func<CollectionDeckBoxVisual.FormatElements, bool>) (x => x.formatType != FormatType.FT_UNKNOWN && x.formatType != this.m_formatType)).ToArray<CollectionDeckBoxVisual.FormatElements>();

  private void ShowBannerInternal(bool show)
  {
    this.m_showBanner = show;
    this.UpdateVisualBannerState();
  }

  private void UpdateVisualBannerState()
  {
    CollectionDeckBoxVisual.FormatElements activeFormatElements = this.GetActiveFormatElements();
    bool flag = this.IsDeckEnabled();
    if ((UnityEngine.Object) activeFormatElements.disabledMeshObject != (UnityEngine.Object) null)
      activeFormatElements.disabledMeshObject.SetActive(!flag);
    if ((UnityEngine.Object) activeFormatElements.classObject != (UnityEngine.Object) null)
      activeFormatElements.classObject.SetActive((flag || (bool) UniversalInputManager.UsePhoneUI) && this.m_showBanner);
    if (!((UnityEngine.Object) activeFormatElements.topBannerRenderer != (UnityEngine.Object) null))
      return;
    activeFormatElements.topBannerRenderer.gameObject.SetActive(flag && this.m_showBanner);
  }

  private GameObject ButtonGameObject => this.GetActiveFormatElements()?.portraitObject;

  private void OnDeleteButtonRollout(UIEvent e) => this.ShowDeleteButton(false);

  private void OnDeleteButtonOver(UIEvent e) => SoundManager.Get().LoadAndPlay((AssetReference) "tiny_button_mouseover_1.prefab:0ab88a13f5168ed43a3b53275114a842", this.gameObject);

  private void OnDeleteButtonPressed(UIEvent e)
  {
    bool flag = SceneMgr.Get().IsInLettuceMode();
    if (!flag && CollectionDeckTray.Get().IsShowingDeckContents() || flag && CollectionDeckTray.Get().IsShowingTeamContents())
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "tiny_button_press_1.prefab:44fc68b7418870b4797b85f0ca88a8db", this.gameObject);
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_HEADER");
    info.m_showAlertIcon = false;
    string key = flag ? "GLUE_COLLECTION_DELETE_TEAM_CONFIRM_DESC" : "GLUE_COLLECTION_DELETE_CONFIRM_DESC";
    info.m_text = GameStrings.Get(key);
    info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
    info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnDeleteButtonConfirmationResponse);
    DialogManager.Get().ShowPopup(info);
  }

  private void OnDeleteButtonConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    this.SetEnabled(false, false);
    if (SceneMgr.Get().IsInLettuceMode())
      CollectionDeckBoxVisual.TeamsContent.DeleteTeam(this.GetDeckID());
    else
      CollectionDeckBoxVisual.DecksContent.DeleteDeck(this.GetDeckID());
  }

  private void PlayPopAnimation(string animationName) => this.PlayPopAnimation(animationName, (CollectionDeckBoxVisual.DelOnAnimationFinished) null, (object) null);

  private void PlayPopAnimation(
    string animationName,
    CollectionDeckBoxVisual.DelOnAnimationFinished callback,
    object callbackData)
  {
    this.GetComponent<Animation>().Play(animationName);
    CollectionDeckBoxVisual.OnPopAnimationFinishedCallbackData finishedCallbackData = new CollectionDeckBoxVisual.OnPopAnimationFinishedCallbackData()
    {
      m_callback = callback,
      m_callbackData = callbackData,
      m_animationName = animationName
    };
    this.StopCoroutine("WaitThenCallAnimationCallback");
    this.StartCoroutine("WaitThenCallAnimationCallback", (object) finishedCallbackData);
  }

  private IEnumerator WaitThenCallAnimationCallback(
    CollectionDeckBoxVisual.OnPopAnimationFinishedCallbackData callbackData)
  {
    CollectionDeckBoxVisual collectionDeckBoxVisual = this;
    Animation component = collectionDeckBoxVisual.GetComponent<Animation>();
    yield return (object) new WaitForSeconds(component[callbackData.m_animationName].length / component[callbackData.m_animationName].speed);
    bool enabled = callbackData.m_animationName.Equals("Deck_PopUp");
    collectionDeckBoxVisual.SetEnabled(enabled, false);
    if (callbackData.m_callback != null)
      callbackData.m_callback(callbackData.m_callbackData);
  }

  private void ScaleDeckBox(
    bool scaleUp,
    CollectionDeckBoxVisual.DelOnAnimationFinished callback,
    object callbackData)
  {
    CollectionDeckBoxVisual.OnScaleFinishedCallbackData finishedCallbackData = new CollectionDeckBoxVisual.OnScaleFinishedCallbackData()
    {
      m_callback = callback,
      m_callbackData = callbackData
    };
    Hashtable args = iTween.Hash((object) "scale", (object) (scaleUp ? CollectionDeckBoxVisual.SCALED_UP_LOCAL_SCALE : CollectionDeckBoxVisual.SCALED_DOWN_LOCAL_SCALE), (object) "isLocal", (object) true, (object) "time", (object) 0.2f, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "OnScaleComplete", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) finishedCallbackData, (object) "name", (object) "scale");
    iTween.StopByName(this.gameObject, "scale");
    iTween.ScaleTo(this.gameObject, args);
  }

  private void OnScaleComplete(
    CollectionDeckBoxVisual.OnScaleFinishedCallbackData callbackData)
  {
    if (callbackData.m_callback == null)
      return;
    callbackData.m_callback(callbackData.m_callbackData);
  }

  private void OnHeroFullDefLoaded(string cardID, DefLoader.DisposableFullDef def, object userData)
  {
    Log.CollectionDeckBox.Print("OnHeroFullDefLoaded cardID: {0},  m_heroCardID: {1}", (object) cardID, (object) this.m_heroCardID);
    this.m_fullDef?.Dispose();
    this.m_fullDef = (DefLoader.DisposableFullDef) null;
    this.m_legendaryHeroPortrait?.Dispose();
    this.m_legendaryHeroPortrait = (ILegendaryHeroPortrait) null;
    if (cardID == null || !cardID.Equals(this.m_heroCardID))
    {
      this.SetPortrait(this.m_defaultPortraitMaterial);
      this.m_heroCardID = "None";
    }
    else
    {
      this.m_fullDef = def;
      Material material = (Material) null;
      if (this.m_fullDef != null && (UnityEngine.Object) this.m_fullDef.CardDef != (UnityEngine.Object) null)
        material = this.m_fullDef.CardDef.GetCustomDeckPortrait();
      if ((UnityEngine.Object) material == (UnityEngine.Object) null && (UnityEngine.Object) this.m_defaultPortraitMaterial != (UnityEngine.Object) null)
        material = this.m_defaultPortraitMaterial;
      string legendaryModel = this.m_fullDef?.CardDef?.m_LegendaryModel;
      if (!string.IsNullOrEmpty(legendaryModel))
      {
        LegendaryHeroRenderToTextureService toTextureService = ServiceManager.Get<LegendaryHeroRenderToTextureService>();
        if (toTextureService != null)
        {
          this.m_legendaryHeroPortrait = toTextureService.CreatePortrait(legendaryModel, Player.Side.NEUTRAL);
          if ((UnityEngine.Object) this.m_legendaryHeroPortrait.PortraitTexture != (UnityEngine.Object) null)
          {
            material = UnityEngine.Object.Instantiate<Material>(material);
            material.mainTexture = this.m_legendaryHeroPortrait.PortraitTexture;
          }
          else
          {
            this.m_legendaryHeroPortrait.Dispose();
            this.m_legendaryHeroPortrait = (ILegendaryHeroPortrait) null;
          }
        }
      }
      if (this.m_legendaryHeroPortrait != null)
      {
        this.m_legendaryHeroPortrait.ClearDynamicResolutionControllers();
        foreach (CollectionDeckBoxVisual.FormatElements formatElement in this.m_formatElements)
        {
          GameObject portraitObject = formatElement.portraitObject;
          LegendarySkinDynamicResController controller = portraitObject.GetComponent<LegendarySkinDynamicResController>();
          if (!(bool) (UnityEngine.Object) controller)
            controller = portraitObject.AddComponent<LegendarySkinDynamicResController>();
          this.m_legendaryHeroPortrait.ConnectDynamicResolutionController(controller);
          controller.CacheMaterialProperties(material);
          controller.Renderer = portraitObject.GetComponent<Renderer>();
          controller.MaterialIdx = formatElement.portraitMaterialIndex;
        }
      }
      else
      {
        foreach (CollectionDeckBoxVisual.FormatElements formatElement in this.m_formatElements)
        {
          LegendarySkinDynamicResController component = formatElement.portraitObject.GetComponent<LegendarySkinDynamicResController>();
          if ((bool) (UnityEngine.Object) component)
          {
            component.Skin = (LegendarySkin) null;
            component.Renderer = (Renderer) null;
          }
        }
      }
      this.SetPortrait(material);
      if (this.m_fullDef != null && this.m_fullDef.EntityDef != null && !this.m_fullDef.EntityDef.IsLettuceMercenary())
      {
        TAG_CLASS classTag = this.m_fullDef.EntityDef.GetClass();
        if (classTag == TAG_CLASS.INVALID)
          Log.CollectionDeckBox.PrintError("OnHeroFullDefLoaded heroClass was INVALID for cardID: {0},  heroClass: {1}", (object) cardID, (object) classTag);
        else
          this.SetClassDisplay(classTag);
      }
      this.UpdateDeckLabel();
    }
  }

  private void UpdatePortraitMaterial(
    GameObject portraitObject,
    Material portraitMaterial,
    int portraitMaterialIndex)
  {
    if ((UnityEngine.Object) portraitMaterial == (UnityEngine.Object) null)
      Log.CollectionDeckBox.PrintError("Custom Deck Portrait Material is null!");
    else if ((UnityEngine.Object) portraitObject == (UnityEngine.Object) null)
    {
      Log.CollectionDeckBox.PrintError("Custom Deck Portrait GameObject is null!");
    }
    else
    {
      Renderer component = portraitObject.GetComponent<Renderer>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Log.CollectionDeckBox.PrintError("Custom Deck Portrait GameObject doesnt have a renderer!");
      }
      else
      {
        RendererExtension.SetSharedMaterial(component, portraitMaterialIndex, portraitMaterial);
        if ((UnityEngine.Object) this.m_fullDef?.CardDef == (UnityEngine.Object) null || this.m_neverUseGoldenPortraits)
          return;
        TAG_PREMIUM heroCardPremium = this.GetHeroCardPremium();
        if (heroCardPremium != TAG_PREMIUM.GOLDEN && GameUtils.IsVanillaHero(this.m_fullDef.EntityDef.GetCardId()) || heroCardPremium == TAG_PREMIUM.NORMAL && this.m_fullDef.EntityDef.IsLettuceMercenary() || this.m_graphicsManager.isVeryLowQualityDevice())
          return;
        Material portraitMaterial1 = this.m_fullDef?.CardDef.GetPremiumPortraitMaterial();
        if ((UnityEngine.Object) portraitMaterial1 != (UnityEngine.Object) null)
        {
          Material material1 = RendererExtension.GetMaterial(component, portraitMaterialIndex);
          Texture texture = (Texture) null;
          if (material1.HasProperty("_ShadowTex"))
            texture = material1.GetTexture("_ShadowTex");
          RendererExtension.SetMaterial(component, portraitMaterialIndex, portraitMaterial1);
          RendererExtension.GetMaterial(component, portraitMaterialIndex).SetTexture("_ShadowTex", texture);
          Material material2 = RendererExtension.GetMaterial(component, portraitMaterialIndex);
          material2.mainTextureOffset = material1.mainTextureOffset;
          material2.mainTextureScale = material1.mainTextureScale;
        }
        UberShaderAnimation portraitAnimation = this.m_fullDef?.CardDef.GetPremiumPortraitAnimation();
        if (!((UnityEngine.Object) portraitAnimation != (UnityEngine.Object) null))
          return;
        UberShaderController shaderController = portraitObject.GetComponent<UberShaderController>();
        if ((UnityEngine.Object) shaderController == (UnityEngine.Object) null)
          shaderController = portraitObject.AddComponent<UberShaderController>();
        shaderController.UberShaderAnimation = UnityEngine.Object.Instantiate<UberShaderAnimation>(portraitAnimation);
        shaderController.m_MaterialIndex = portraitMaterialIndex;
      }
    }
  }

  private void SetPortrait(Material portraitMaterial)
  {
    foreach (CollectionDeckBoxVisual.FormatElements formatElement in this.m_formatElements)
      this.UpdatePortraitMaterial(formatElement.portraitObject, portraitMaterial, formatElement.portraitMaterialIndex);
  }

  private void SetClassDisplay(TAG_CLASS classTag)
  {
    foreach (CollectionDeckBoxVisual.FormatElements formatElement in this.m_formatElements)
    {
      if (!((UnityEngine.Object) formatElement.classObject == (UnityEngine.Object) null))
      {
        MeshRenderer component = formatElement.classObject.GetComponent<MeshRenderer>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          Material material1 = RendererExtension.GetMaterial((Renderer) component, formatElement.classIconMaterialIndex);
          Material material2 = RendererExtension.GetMaterial((Renderer) component, formatElement.classBannerMaterialIndex);
          if (!((UnityEngine.Object) material1 == (UnityEngine.Object) null) && !((UnityEngine.Object) material2 == (UnityEngine.Object) null))
          {
            material1.mainTextureOffset = CollectionPageManager.s_classTextureOffsets[classTag];
            material2.color = CollectionPageManager.ColorForClass(classTag);
            if ((UnityEngine.Object) formatElement.topBannerRenderer != (UnityEngine.Object) null)
              RendererExtension.GetMaterial((Renderer) formatElement.topBannerRenderer, this.m_topBannerMaterialIndex).color = CollectionPageManager.ColorForClass(classTag);
          }
        }
      }
    }
  }

  private void MarkRewardedDeckAsSeen(long deckId)
  {
    long collectionDeckId;
    if (!RewardUtils.HasNewRewardedDeck(out collectionDeckId) || deckId != collectionDeckId)
      return;
    RewardUtils.MarkNewestRewardedDeckAsSeen();
  }

  private void MarkDeckAsSeen()
  {
    this.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER);
    CollectionDeck collectionDeck = this.GetCollectionDeck();
    if (collectionDeck != null && collectionDeck.NeedsName)
    {
      Log.CollectionDeckBox.Print(string.Format("Sending deck changes for deck {0}, to clear the NEEDS_NAME flag.", (object) this.m_deckID));
      collectionDeck.SendChanges(CollectionDeck.ChangeSource.MarkDeckAsSeen);
      collectionDeck.NeedsName = false;
    }
    this.MarkRewardedDeckAsSeen(this.m_deckID);
    this.m_showGlow = false;
  }

  protected override void OnPress()
  {
    if (!this.m_animateButtonPress || this.m_isLocked || this.m_isSelected || !this.IsDeckEnabled())
      return;
    this.OnPressEvent();
  }

  protected override void OnHold()
  {
    CollectionDeckTray tray = CollectionDeckTray.Get();
    if ((UnityEngine.Object) tray == (UnityEngine.Object) null)
      return;
    switch (tray.GetCurrentContentType())
    {
      case DeckTray.DeckContentTypes.Decks:
      case DeckTray.DeckContentTypes.Teams:
        this.OnHoldReorderable(tray);
        break;
    }
  }

  private void OnHoldReorderable(CollectionDeckTray tray)
  {
    DeckTrayReorderableContent reorderableContent = tray.GetReorderableContent();
    if ((UnityEngine.Object) reorderableContent == (UnityEngine.Object) null || reorderableContent.IsTouchDragging)
      return;
    if ((UnityEngine.Object) this.m_tooltipZone != (UnityEngine.Object) null)
      this.m_tooltipZone.HideTooltip();
    this.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
    if ((UnityEngine.Object) this.ButtonGameObject != (UnityEngine.Object) null)
      iTween.ScaleTo(this.ButtonGameObject, iTween.Hash((object) "scale", (object) (reorderableContent.m_rearrangeEnlargeScale * Vector3.one), (object) "isLocal", (object) true, (object) "time", (object) reorderableContent.m_rearrangeStartStopTweenDuration, (object) "easeType", (object) iTween.EaseType.linear));
    reorderableContent.StartDragToReorder((IDraggableCollectionVisual) this);
  }

  protected override void OnRelease()
  {
    DeckTrayDeckListContent decksContent = CollectionDeckBoxVisual.DecksContent;
    if ((UnityEngine.Object) decksContent != (UnityEngine.Object) null)
      decksContent.StopDragToReorder();
    if (this.m_isLocked || this.m_isSelected || !this.IsDeckEnabled())
      return;
    if (!SceneMgr.Get().IsInTavernBrawlMode() || UniversalInputManager.Get().IsTouchMode())
    {
      string deckSelectSound = this.GetActiveFormatElements().deckSelectSound;
      if (!string.IsNullOrEmpty(deckSelectSound))
        SoundManager.Get().LoadAndPlay((AssetReference) deckSelectSound, this.gameObject);
    }
    this.OnReleaseEvent();
  }

  public void OnStopDragToReorder()
  {
    if (!UniversalInputManager.Get().IsTouchMode())
      this.ShowDeleteButton(true);
    if ((UnityEngine.Object) this.ButtonGameObject != (UnityEngine.Object) null)
    {
      float num = 0.1f;
      DeckTrayDeckListContent decksContent = CollectionDeckBoxVisual.DecksContent;
      if ((UnityEngine.Object) decksContent != (UnityEngine.Object) null)
        num = decksContent.m_rearrangeStartStopTweenDuration;
      iTween.ScaleTo(this.ButtonGameObject, iTween.Hash((object) "scale", (object) Vector3.one, (object) "isLocal", (object) true, (object) "time", (object) num, (object) "easeType", (object) iTween.EaseType.linear));
    }
    this.OnOutEvent();
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    if ((UnityEngine.Object) this.m_tooltipZone != (UnityEngine.Object) null)
      this.m_tooltipZone.HideTooltip();
    this.OnOutEvent();
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if ((UnityEngine.Object) this.m_tooltipZone != (UnityEngine.Object) null)
    {
      if (this.m_isLocked)
        this.m_tooltipZone.ShowTooltip(GameStrings.Get("GLUE_LOCKED_DECK_HEADER"), GameStrings.Get("GLUE_LOCKED_DECK_DESC"), 4f);
      else if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null)
      {
        if (!this.IsDeckEnabled())
        {
          string headline = GameStrings.Format("GLUE_DISABLED_DECK_HEADER", (object) GameStrings.GetFormatName(this.m_formatType));
          CollectionDeck collectionDeck = this.GetCollectionDeck();
          string bodytext;
          if (collectionDeck == null)
            bodytext = "";
          else if (!GameUtils.HasUnlockedClass(collectionDeck.GetClass()))
            bodytext = GameStrings.Get("GLUE_DISABLED_DECK_IN_CURRENT_MODE_DESC");
          else if (CollectionDeck.DoesModeRequireSpecificFormat(SceneMgr.Get().GetMode(), Options.GetInRankedPlayMode()))
            bodytext = GameStrings.Format("GLUE_DISABLED_DECK_DESC", (object) GameStrings.GetFormatName(Options.GetFormatType()));
          else
            bodytext = GameStrings.Get("GLUE_DISABLED_DECK_IN_CURRENT_MODE_DESC");
          this.m_tooltipZone.ShowTooltip(headline, bodytext, 4f);
        }
        else if (this.m_isShowingInvalidCardCount)
        {
          if (this.m_cardCountByStatus.Extra > 0)
            this.m_tooltipZone.ShowTooltip(GameStrings.FormatPlurals("GLUE_EXTRA_CARDS_DECK_HEADER", GameStrings.MakePlurals(this.m_cardCountByStatus.Extra)), GameStrings.Get("GLUE_EXTRA_CARDS_DECK_DESC"), 4f);
          else
            this.m_tooltipZone.ShowTooltip(GameStrings.FormatPlurals("GLUE_MISSING_CARDS_DECK_HEADER", GameStrings.MakePlurals(this.m_cardCountByStatus.MissingPlusInvalid)), GameStrings.Get("GLUE_MISSING_CARDS_DECK_DESC"), 4f);
        }
      }
    }
    this.OnOverEvent();
  }

  public override void SetEnabled(bool enabled, bool isInternal = false)
  {
    base.SetEnabled(enabled, isInternal);
    if (enabled || !((UnityEngine.Object) this.m_tooltipZone != (UnityEngine.Object) null))
      return;
    this.m_tooltipZone.HideTooltip();
  }

  private void OnPressEvent()
  {
    this.ShowDeleteButton(false);
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.COLLECTIONMANAGER:
        break;
      case SceneMgr.Mode.LETTUCE_COLLECTION:
        break;
      default:
        iTween.MoveTo(this.ButtonGameObject, iTween.Hash((object) "position", (object) this.m_pressedBone.transform.localPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.1, (object) "easeType", (object) iTween.EaseType.linear));
        break;
    }
  }

  private void OnReleaseEvent()
  {
    if (UniversalInputManager.Get().IsTouchMode() && this.m_showGlow)
      this.MarkDeckAsSeen();
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.COLLECTIONMANAGER:
        break;
      case SceneMgr.Mode.LETTUCE_COLLECTION:
        break;
      default:
        iTween.MoveTo(this.ButtonGameObject, iTween.Hash((object) "position", (object) this.m_originalButtonPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.1, (object) "easeType", (object) iTween.EaseType.linear));
        break;
    }
  }

  private void OnOutEvent()
  {
    if (!this.m_isSelected)
      this.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.COLLECTIONMANAGER:
        break;
      case SceneMgr.Mode.LETTUCE_COLLECTION:
        break;
      default:
        iTween.MoveTo(this.ButtonGameObject, iTween.Hash((object) "position", (object) this.m_originalButtonPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.1, (object) "easeType", (object) iTween.EaseType.linear));
        break;
    }
  }

  private void OnOverEvent()
  {
    if (UniversalInputManager.Get().IsTouchMode())
      return;
    DeckTrayDeckListContent decksContent = CollectionDeckBoxVisual.DecksContent;
    if ((UnityEngine.Object) decksContent != (UnityEngine.Object) null && decksContent.DraggingDeckBox != null || this.m_isSelected)
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_hero_mouse_over.prefab:653cc8000b988cd468d2210a209adce6", this.gameObject);
    if (this.m_showGlow)
    {
      this.MarkDeckAsSeen();
    }
    else
    {
      if (!this.IsDeckEnabled())
        return;
      this.SetHighlightState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
    }
  }

  private void ReparentElements(FormatType formatType)
  {
    CollectionDeckBoxVisual.FormatElements formatElements = this.GetFormatElements(formatType);
    Transform transform = formatElements.portraitObject.transform;
    this.m_highlight.transform.parent = transform;
    this.m_deckName.gameObject.transform.parent = transform;
    this.m_deckDesc.gameObject.transform.parent = transform;
    this.m_invalidCardCountIndicator.gameObject.transform.parent = transform;
    if ((UnityEngine.Object) this.m_deckRunes != (UnityEngine.Object) null)
      this.m_deckRunes.gameObject.transform.parent = transform;
    if ((bool) UniversalInputManager.UsePhoneUI)
      formatElements.classObject.transform.parent = transform;
    this.m_bones.m_gradientOneLine.parent = transform;
    this.m_bones.m_gradientTwoLine.parent = transform;
  }

  private static DeckTrayDeckListContent DecksContent
  {
    get
    {
      CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
      return !((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null) ? (DeckTrayDeckListContent) null : collectionDeckTray.GetDecksContent();
    }
  }

  private static bool ShouldHighlightDeck(CollectionDeck deck)
  {
    if (deck.NeedsName)
      return true;
    long collectionDeckId;
    return RewardUtils.HasNewRewardedDeck(out collectionDeckId) && deck.ID == collectionDeckId;
  }

  private static DeckTrayTeamListContent TeamsContent
  {
    get
    {
      CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
      return !((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null) ? (DeckTrayTeamListContent) null : collectionDeckTray.GetTeamsContent();
    }
  }

  [Serializable]
  public class FormatElements
  {
    [SerializeField]
    public FormatType formatType;
    [SerializeField]
    public Texture2D highlight;
    [SerializeField]
    public GameObject portraitObject;
    [SerializeField]
    public int portraitMaterialIndex;
    [SerializeField]
    public GameObject classObject;
    [SerializeField]
    public int classIconMaterialIndex;
    [SerializeField]
    public int classBannerMaterialIndex;
    [SerializeField]
    public MeshRenderer topBannerRenderer;
    [SerializeField]
    public Material xButtonMaterial;
    [CustomEditField(T = EditType.SOUND_PREFAB)]
    public string deckSelectSound;
    [SerializeField]
    public GameObject disabledMeshObject;
  }

  public delegate void DelOnAnimationFinished(object callbackData);

  private class OnPopAnimationFinishedCallbackData
  {
    public string m_animationName;
    public CollectionDeckBoxVisual.DelOnAnimationFinished m_callback;
    public object m_callbackData;
  }

  private class OnScaleFinishedCallbackData
  {
    public CollectionDeckBoxVisual.DelOnAnimationFinished m_callback;
    public object m_callbackData;
  }
}
