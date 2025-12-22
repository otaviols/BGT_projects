using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class DeckTrayCardBackContent : DeckTrayContent
{
  [SerializeField]
  [Header("Positioning")]
  private GameObject m_root;
  [SerializeField]
  private Vector3 m_trayHiddenOffset;
  [SerializeField]
  private GameObject m_cardBackContainer;
  [SerializeField]
  [Header("Animation")]
  private iTween.EaseType m_traySlideSlideInAnimation = iTween.EaseType.easeOutBounce;
  [SerializeField]
  private iTween.EaseType m_traySlideSlideOutAnimation;
  [SerializeField]
  private float m_traySlideAnimationTime = 0.25f;
  [SerializeField]
  private SpellType m_removalSpellType;
  [Header("Sound")]
  [SerializeField]
  private WeakAssetReference m_appearanceSound;
  [SerializeField]
  private WeakAssetReference m_socketSound;
  [SerializeField]
  private WeakAssetReference m_unsocketSound;
  [SerializeField]
  private WeakAssetReference m_pickUpSound;
  private Widget m_rootWidget;
  private DeckDataModel m_deckDataModel;
  private GameObject m_currentCardBack;
  private CollectionDeck m_currentDeck;
  private Vector3 m_originalLocalPosition;
  private bool m_animatingTray;
  private bool m_waitingToLoadCardback;
  private bool m_shouldUpdateLimitToFavoritesSetting;
  private Notification m_dragToRemoveNotification;
  private bool m_shouldShowDragToRemoveNotification;
  private Notification m_randomIsDefaultNotification;
  private bool m_shouldShowRandomIsDefaultNotification;
  private const string CODE_CHECKBOX_TOGGLED = "CODE_CHECKBOX_TOGGLED";
  private const string CODE_TRAY_CLICKED = "CODE_TRAY_CLICKED";
  private const string CODE_TRAY_DRAG_START = "CODE_TRAY_DRAG_START";
  private DeckTrayCardBackContent.AnimatedCardBack m_animData;

  public bool WaitingForCardbackAnimation => this.m_animData != null || this.m_waitingToLoadCardback;

  protected override void Awake()
  {
    base.Awake();
    this.m_rootWidget = (Widget) this.GetComponent<WidgetTemplate>();
    if ((UnityEngine.Object) this.m_rootWidget != (UnityEngine.Object) null)
    {
      this.m_rootWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener));
      this.m_rootWidget.RegisterReadyListener(new Action<object>(this.WidgetReadyListener), (object) null, true);
    }
    this.m_originalLocalPosition = this.transform.localPosition;
    this.transform.localPosition = this.m_originalLocalPosition + this.m_trayHiddenOffset;
    this.m_root.SetActive(false);
    this.m_shouldShowRandomIsDefaultNotification = !GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_HAS_SEEN_UPDATED_CARD_BACK_DECK_TRAY_EMPTY);
    this.m_shouldShowDragToRemoveNotification = !GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_HAS_SEEN_UPDATED_CARD_BACK_DECK_TRAY_ASSIGNED);
  }

  private void WidgetReadyListener(object unused)
  {
    this.m_deckDataModel = new DeckDataModel();
    this.m_deckDataModel.RandomCardBackFavoritesOnly = true;
    this.m_rootWidget.BindDataModel((IDataModel) this.m_deckDataModel);
  }

  private void WidgetEventListener(string eventName)
  {
    if (!(eventName == "CODE_CHECKBOX_TOGGLED"))
    {
      if (!(eventName == "CODE_TRAY_CLICKED"))
      {
        if (!(eventName == "CODE_TRAY_DRAG_START"))
          return;
        this.GrabCardBack();
      }
      else
        this.RemoveCardBack();
    }
    else
      this.ToggleFavoritesOnly(!this.m_deckDataModel.RandomCardBackFavoritesOnly);
  }

  private void ToggleFavoritesOnly(bool enabled)
  {
    int randomCardBackId = CardBackManager.Get().TheRandomCardBackID;
    int? nullable1 = enabled ? new int?() : new int?(randomCardBackId);
    int? cardBackId = this.m_currentDeck.CardBackID;
    int? nullable2 = nullable1;
    if (!(cardBackId.GetValueOrDefault() == nullable2.GetValueOrDefault() & cardBackId.HasValue == nullable2.HasValue))
      this.m_currentDeck.CardBackID = nullable1;
    this.UpdateDatamodel();
    this.m_shouldUpdateLimitToFavoritesSetting = true;
  }

  public bool DeckHasCardBackOverride()
  {
    int randomCardBackId = CardBackManager.Get().TheRandomCardBackID;
    if (!this.m_currentDeck.CardBackID.HasValue)
      return false;
    int? cardBackId = this.m_currentDeck.CardBackID;
    int num = randomCardBackId;
    return !(cardBackId.GetValueOrDefault() == num & cardBackId.HasValue);
  }

  public bool AnimateInCardBack(int cardBackId, GameObject original)
  {
    if (this.m_animData != null || this.m_waitingToLoadCardback)
      return false;
    this.m_waitingToLoadCardback = true;
    if (CardBackManager.Get().LoadCardBackByIndex(cardBackId, (CardBackManager.LoadCardBackData.LoadCardBackCallback) (cardBackData =>
    {
      this.m_waitingToLoadCardback = false;
      this.AnimateCardBackAssignmentFromPageVisual(cardBackData, original);
    }), "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", (object) null))
      return true;
    this.m_waitingToLoadCardback = false;
    Debug.LogError((object) ("Could not load CardBack " + (object) cardBackId));
    return false;
  }

  public void UpdateCardBack(int cardBackId, bool assigning, GameObject obj = null)
  {
    if (this.m_currentDeck == null)
      return;
    if (assigning)
    {
      if (!string.IsNullOrEmpty(this.m_socketSound.AssetString))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_socketSound.AssetString, this.gameObject);
      this.m_currentDeck.CardBackID = new int?(cardBackId);
    }
    this.UpdateDatamodel();
    this.ShowTutorialIfNeeded();
    this.ToggleSparkleEffects(false);
    if ((UnityEngine.Object) obj != (UnityEngine.Object) null)
    {
      this.SetCardBackObject(obj, assigning);
    }
    else
    {
      this.m_waitingToLoadCardback = true;
      if (CardBackManager.Get().LoadCardBackByIndex(cardBackId, (CardBackManager.LoadCardBackData.LoadCardBackCallback) (cardBackData =>
      {
        this.m_waitingToLoadCardback = false;
        if (this.m_currentDeck != null)
        {
          int? cardBackId1 = this.m_currentDeck.CardBackID;
          int num = cardBackId;
          if (cardBackId1.GetValueOrDefault() == num & cardBackId1.HasValue)
          {
            this.SetCardBackObject(cardBackData.m_GameObject, assigning);
            return;
          }
        }
        UnityEngine.Object.Destroy((UnityEngine.Object) cardBackData.m_GameObject);
      }), "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", (object) null))
        return;
      this.m_waitingToLoadCardback = false;
      Debug.LogWarning((object) string.Format("CardBackManager was unable to load card back ID: {0}", (object) cardBackId));
    }
  }

  private void GrabCardBack()
  {
    if (!((UnityEngine.Object) this.m_currentCardBack != (UnityEngine.Object) null))
      return;
    Actor component = this.m_currentCardBack.GetComponent<Actor>();
    int? cardBackId = this.m_currentDeck.CardBackID;
    if (!cardBackId.HasValue || !CollectionInputMgr.Get().GrabCardBackFromSlot(component, cardBackId.Value))
      return;
    this.m_currentDeck.CardBackID = new int?();
    this.ClearCardBackGameObject();
    this.UpdateDatamodel();
    this.ShowTutorialIfNeeded();
    this.ToggleSparkleEffects(true);
  }

  public void ToggleSparkleEffects(bool enabled)
  {
    if (this.m_deckDataModel == null)
      return;
    this.m_deckDataModel.DraggingDeckAssignment = enabled;
  }

  private void RemoveCardBack()
  {
    if (!((UnityEngine.Object) this.m_currentCardBack != (UnityEngine.Object) null) || !this.DeckHasCardBackOverride())
      return;
    Actor component = this.m_currentCardBack.GetComponent<Actor>();
    Spell spell1 = component.GetSpell(this.m_removalSpellType);
    CardBackSummon componentInChildren1 = spell1.gameObject.GetComponentInChildren<CardBackSummon>();
    CardBack componentInChildren2 = component.GetComponentInChildren<CardBack>();
    if ((UnityEngine.Object) componentInChildren1 != (UnityEngine.Object) null && (UnityEngine.Object) componentInChildren2 != (UnityEngine.Object) null)
      componentInChildren1.UpdateEffectWithCardBack(componentInChildren2);
    this.m_currentDeck.CardBackID = new int?();
    this.UpdateDatamodel();
    this.ShowTutorialIfNeeded();
    if (!string.IsNullOrEmpty(this.m_unsocketSound.AssetString))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_unsocketSound.AssetString, this.gameObject);
    if ((UnityEngine.Object) spell1 == (UnityEngine.Object) null)
    {
      this.ClearCardBackGameObject();
    }
    else
    {
      spell1.AddFinishedCallback((Spell.FinishedCallback) ((spell, userData) =>
      {
        SpellManager.Get().ReleaseSpell(spell, true);
        this.ClearCardBackGameObject();
      }));
      spell1.ActivateState(SpellStateType.BIRTH);
    }
  }

  private void ClearCardBackGameObject()
  {
    if (!((UnityEngine.Object) this.m_currentCardBack != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentCardBack);
    this.m_currentCardBack = (GameObject) null;
  }

  private void SetCardBackObject(GameObject go, bool assigning)
  {
    GameUtils.SetParent(go, this.m_cardBackContainer, true);
    CollectionDraggableCardVisual heldCardVisual = CollectionInputMgr.Get()?.GetHeldCardVisual();
    if ((UnityEngine.Object) heldCardVisual != (UnityEngine.Object) null)
      heldCardVisual.InitActorCache();
    Actor component = go.GetComponent<Actor>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
    }
    else
    {
      if (assigning)
      {
        Spell spell = component.GetSpell(SpellType.DEATHREVERSE);
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
          spell.ActivateState(SpellStateType.BIRTH);
      }
      if ((UnityEngine.Object) this.m_currentCardBack != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentCardBack);
      this.m_currentCardBack = go;
      GameObject cardMesh = component.m_cardMesh;
      component.SetCardbackUpdateIgnore(true);
      component.SetUnlit();
      component.UpdateAllComponents();
      if (!((UnityEngine.Object) cardMesh != (UnityEngine.Object) null))
        return;
      Material material = RendererExtension.GetMaterial(cardMesh.GetComponent<Renderer>());
      if (!material.HasProperty("_SpecularIntensity"))
        return;
      material.SetFloat("_SpecularIntensity", 0.0f);
    }
  }

  private void UpdateDatamodel()
  {
    if (this.DeckHasCardBackOverride())
    {
      int id = this.m_currentDeck.CardBackID.Value;
      CardBackDbfRecord record = GameDbf.CardBack.GetRecord(id);
      if (this.m_deckDataModel.CardBack == null)
        this.m_deckDataModel.CardBack = new CardBackDataModel();
      this.m_deckDataModel.CardBack.CardBackId = id;
      this.m_deckDataModel.CardBack.Name = (string) record.Name;
    }
    else
    {
      this.m_deckDataModel.RandomCardBackFavoritesOnly = !this.m_currentDeck.CardBackID.HasValue;
      this.m_deckDataModel.CardBack = (CardBackDataModel) null;
    }
  }

  private void SaveRandomCardBackSelectionPreference()
  {
    if (!this.m_shouldUpdateLimitToFavoritesSetting)
      return;
    int num1 = GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_RANDOM_CARD_BACK_USE_ALL_OWNED) ? 1 : 0;
    bool enableFlag = !this.m_deckDataModel.RandomCardBackFavoritesOnly;
    int num2 = enableFlag ? 1 : 0;
    if (num1 != num2)
      GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_RANDOM_CARD_BACK_USE_ALL_OWNED, enableFlag);
    CardBackManager.Get().LoadRandomCardBackIntoFavoriteSlot(true);
    this.m_shouldUpdateLimitToFavoritesSetting = false;
  }

  public void AnimateCardBackAssignmentFromPageVisual(
    CardBackManager.LoadCardBackData cardBackData,
    GameObject original)
  {
    GameObject gameObject = cardBackData.m_GameObject;
    gameObject.GetComponent<Actor>().GetSpell(SpellType.DEATHREVERSE).Reactivate();
    DeckTrayCardBackContent.AnimatedCardBack animatedCardBack = new DeckTrayCardBackContent.AnimatedCardBack();
    animatedCardBack.CardBackId = cardBackData.m_CardBackIndex;
    animatedCardBack.GameObject = gameObject;
    animatedCardBack.OriginalScale = gameObject.transform.localScale;
    animatedCardBack.OriginalPosition = original.transform.position;
    this.m_animData = animatedCardBack;
    gameObject.transform.position = new Vector3(original.transform.position.x, original.transform.position.y + 0.5f, original.transform.position.z);
    gameObject.transform.localScale = this.m_cardBackContainer.transform.lossyScale;
    Hashtable args = iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) 0.6f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "onupdate", (object) "AnimateNewCardUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "oncomplete", (object) "AnimateNewCardFinished", (object) "oncompleteparams", (object) animatedCardBack, (object) "oncompletetarget", (object) this.gameObject);
    iTween.ValueTo(gameObject, args);
    if (string.IsNullOrEmpty(this.m_pickUpSound.AssetString))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_pickUpSound.AssetString, this.gameObject);
  }

  private void AnimateNewCardUpdate(float val)
  {
    GameObject gameObject = this.m_animData.GameObject;
    Vector3 originalPosition = this.m_animData.OriginalPosition;
    Vector3 position = this.m_cardBackContainer.transform.position;
    if ((double) val <= 0.850000023841858)
    {
      val /= 0.85f;
      gameObject.transform.position = new Vector3(Mathf.Lerp(originalPosition.x, position.x, val), (float) ((double) Mathf.Lerp(originalPosition.y, position.y, val) + (double) Mathf.Sin(val * 3.141593f) * 15.0 + (double) val * 4.0), Mathf.Lerp(originalPosition.z, position.z, val));
    }
    else
    {
      if ((UnityEngine.Object) this.m_currentCardBack != (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentCardBack);
        this.m_currentCardBack = (GameObject) null;
      }
      val = (float) (((double) val - 0.850000023841858) / 0.149999976158142);
      gameObject.transform.position = new Vector3(position.x, position.y + Mathf.Lerp(4f, 0.0f, val), position.z);
    }
  }

  private void AnimateNewCardFinished(DeckTrayCardBackContent.AnimatedCardBack cardBack)
  {
    cardBack.GameObject.transform.localScale = cardBack.OriginalScale;
    this.UpdateCardBack(cardBack.CardBackId, true, cardBack.GameObject);
    this.m_animData = (DeckTrayCardBackContent.AnimatedCardBack) null;
  }

  public override bool PreAnimateContentEntrance()
  {
    this.m_currentDeck = CollectionManager.Get().GetEditedDeck();
    this.m_shouldUpdateLimitToFavoritesSetting = false;
    this.ClearCardBackGameObject();
    if (this.DeckHasCardBackOverride())
      this.UpdateCardBack(this.m_currentDeck.CardBackID.Value, false);
    return true;
  }

  public override bool AnimateContentEntranceStart()
  {
    if (this.m_waitingToLoadCardback)
      return false;
    this.m_root.SetActive(true);
    this.UpdateDatamodel();
    if (!string.IsNullOrEmpty(this.m_appearanceSound.AssetString))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_appearanceSound.AssetString, this.gameObject);
    this.transform.localPosition = this.m_originalLocalPosition;
    this.m_animatingTray = true;
    iTween.MoveFrom(this.gameObject, iTween.Hash((object) "position", (object) (this.m_originalLocalPosition + this.m_trayHiddenOffset), (object) "islocal", (object) true, (object) "time", (object) this.m_traySlideAnimationTime, (object) "easetype", (object) this.m_traySlideSlideInAnimation, (object) "oncomplete", (object) (Action<object>) (o => this.m_animatingTray = false)));
    return true;
  }

  public override bool AnimateContentEntranceEnd()
  {
    if (!this.m_animatingTray)
      this.ShowTutorialIfNeeded();
    return !this.m_animatingTray;
  }

  public override bool AnimateContentExitStart()
  {
    this.HideTutorials();
    this.SaveRandomCardBackSelectionPreference();
    this.transform.localPosition = this.m_originalLocalPosition;
    this.m_animatingTray = true;
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.m_originalLocalPosition + this.m_trayHiddenOffset), (object) "islocal", (object) true, (object) "time", (object) this.m_traySlideAnimationTime, (object) "easetype", (object) this.m_traySlideSlideOutAnimation, (object) "oncomplete", (object) (Action<object>) (o =>
    {
      this.m_animatingTray = false;
      this.m_root.SetActive(false);
    })));
    return true;
  }

  public override bool AnimateContentExitEnd() => !this.m_animatingTray;

  public void ShowTutorialIfNeeded()
  {
    this.HideTutorials();
    bool flag = this.DeckHasCardBackOverride();
    if ((flag ? (this.m_shouldShowDragToRemoveNotification ? 1 : 0) : (this.m_shouldShowRandomIsDefaultNotification ? 1 : 0)) == 0)
      return;
    Transform trayTutorialBone = (CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay).m_cardBackDeckTrayTutorialBone;
    if ((UnityEngine.Object) trayTutorialBone == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "No bone for card back deck tray tutorials. Did you forget a connection in CollectionManagerDisplay?");
    }
    else
    {
      string text = flag ? GameStrings.Get("GLUE_COLLECTION_TUTORIAL_UPDATED_CARD_BACK_DECK_TRAY_ASSIGNED") : GameStrings.Get("GLUE_COLLECTION_TUTORIAL_UPDATED_CARD_BACK_DECK_TRAY_EMPTY");
      Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, trayTutorialBone, text);
      if (!((UnityEngine.Object) popupText != (UnityEngine.Object) null))
        return;
      popupText.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
      popupText.PulseReminderEveryXSeconds(3f);
      if (flag)
      {
        this.m_dragToRemoveNotification = popupText;
        this.m_shouldShowDragToRemoveNotification = false;
        GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_HAS_SEEN_UPDATED_CARD_BACK_DECK_TRAY_ASSIGNED, true);
      }
      else
      {
        this.m_randomIsDefaultNotification = popupText;
        this.m_shouldShowRandomIsDefaultNotification = false;
        GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_HAS_SEEN_UPDATED_CARD_BACK_DECK_TRAY_EMPTY, true);
      }
    }
  }

  public void HideTutorials()
  {
    if ((UnityEngine.Object) this.m_dragToRemoveNotification != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_dragToRemoveNotification);
    if (!((UnityEngine.Object) this.m_randomIsDefaultNotification != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_randomIsDefaultNotification);
  }

  private class AnimatedCardBack
  {
    public int CardBackId;
    public GameObject GameObject;
    public Vector3 OriginalScale;
    public Vector3 OriginalPosition;
  }
}
