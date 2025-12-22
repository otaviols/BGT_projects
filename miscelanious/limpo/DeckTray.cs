using Blizzard.T5.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeckTray : MonoBehaviour
{
  public DeckTrayCardListContent m_cardsContent;
  public UIBScrollable m_scrollbar;
  public DeckBigCard m_deckBigCard;
  public GameObject m_inputBlocker;
  public GameObject m_topCardPositionBone;
  public List<DeckTray.DeckContentScroll> m_scrollables = new List<DeckTray.DeckContentScroll>();
  protected Map<DeckTray.DeckContentTypes, DeckTrayContent> m_contents = new Map<DeckTray.DeckContentTypes, DeckTrayContent>();
  protected DeckTray.DeckContentTypes m_currentContent = DeckTray.DeckContentTypes.INVALID;
  protected DeckTray.DeckContentTypes m_contentToSet = DeckTray.DeckContentTypes.INVALID;
  protected bool m_settingNewMode;
  protected bool m_updatingTrayMode;
  protected List<DeckTray.ModeSwitched> m_modeSwitchedListeners = new List<DeckTray.ModeSwitched>();

  protected virtual void Start() => SoundManager.Get().Load((AssetReference) "panel_slide_off_deck_creation_screen.prefab:b0d25fc984ec05d4fbea7480b611e5ad");

  public void Initialize()
  {
    DeckTray.DeckContentTypes contentType;
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.LETTUCE_VILLAGE:
      case SceneMgr.Mode.LETTUCE_BOUNTY_BOARD:
      case SceneMgr.Mode.LETTUCE_MAP:
      case SceneMgr.Mode.LETTUCE_PLAY:
      case SceneMgr.Mode.LETTUCE_COLLECTION:
        contentType = DeckTray.DeckContentTypes.Teams;
        break;
      default:
        contentType = DeckTray.DeckContentTypes.Decks;
        break;
    }
    this.SetTrayMode(contentType);
  }

  public DeckTrayCardListContent GetCardsContent() => this.m_cardsContent;

  public DeckTrayContent GetCurrentContent()
  {
    DeckTrayContent currentContent;
    this.m_contents.TryGetValue(this.m_currentContent, out currentContent);
    return currentContent;
  }

  public DeckTray.DeckContentTypes GetCurrentContentType() => this.m_currentContent;

  public DeckBigCard GetDeckBigCard() => this.m_deckBigCard;

  public void SetTrayMode(DeckTray.DeckContentTypes contentType)
  {
    this.m_contentToSet = contentType;
    if (this.m_settingNewMode || this.m_currentContent == contentType)
      return;
    this.StartCoroutine(this.UpdateTrayMode());
  }

  protected abstract IEnumerator UpdateTrayMode();

  public bool IsUpdatingTrayMode() => this.m_updatingTrayMode;

  public void TryEnableScrollbar()
  {
    if ((UnityEngine.Object) this.m_scrollbar == (UnityEngine.Object) null || (UnityEngine.Object) this.GetCurrentContent() == (UnityEngine.Object) null)
      return;
    DeckTray.DeckContentScroll deckContentScroll = this.m_scrollables.Find((Predicate<DeckTray.DeckContentScroll>) (type => this.GetCurrentContentType() == type.m_contentType));
    if (deckContentScroll == null || (UnityEngine.Object) deckContentScroll.m_scrollObject == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "No scrollable object defined.");
    }
    else
    {
      this.m_scrollbar.ScrollObject = deckContentScroll.m_scrollObject;
      this.m_scrollbar.ResetScrollStartPosition(deckContentScroll.GetStartPosition());
      if (deckContentScroll.m_saveScrollPosition)
        this.m_scrollbar.SetScrollSnap(deckContentScroll.GetCurrentScroll());
      this.m_scrollbar.EnableIfNeeded();
    }
  }

  public void SaveScrollbarPosition(DeckTray.DeckContentTypes contentType)
  {
    DeckTray.DeckContentScroll deckContentScroll = this.m_scrollables.Find((Predicate<DeckTray.DeckContentScroll>) (type => contentType == type.m_contentType));
    if (deckContentScroll == null || !deckContentScroll.m_saveScrollPosition)
      return;
    deckContentScroll.SaveCurrentScroll(this.m_scrollbar.GetScroll());
  }

  public void ResetDeckTrayScroll()
  {
    if ((UnityEngine.Object) this.m_scrollbar == (UnityEngine.Object) null)
      return;
    this.m_scrollbar.SetScrollSnap(0.0f);
    foreach (DeckTray.DeckContentScroll scrollable in this.m_scrollables)
      scrollable.SaveCurrentScroll(0.0f);
  }

  protected void TryDisableScrollbar()
  {
    if ((UnityEngine.Object) this.m_scrollbar == (UnityEngine.Object) null || (UnityEngine.Object) this.m_scrollbar.ScrollObject == (UnityEngine.Object) null)
      return;
    this.m_scrollbar.Enable(false);
    this.m_scrollbar.ScrollObject = (GameObject) null;
  }

  public void AllowInput(bool allowed) => this.m_inputBlocker.SetActive(!allowed);

  public bool MouseIsOver() => UniversalInputManager.Get().InputIsOver(this.gameObject) || this.m_cardsContent.MouseIsOverDeckHelperButton(Box.Get().GetCamera());

  public bool MouseIsOver(Camera camera) => UniversalInputManager.Get().ForcedUnblockableInputIsOver(camera, this.gameObject, out RaycastHit _) || this.m_cardsContent.MouseIsOverDeckHelperButton(camera);

  protected abstract void HideUnseenDeckTrays();

  protected void OnTouchScrollStarted()
  {
    if (!((UnityEngine.Object) this.m_deckBigCard != (UnityEngine.Object) null))
      return;
    this.m_deckBigCard.ForceHide();
  }

  protected void OnTouchScrollEnded()
  {
  }

  public static void OnDeckTrayTileScrollVisibleAffected(GameObject obj, bool visible)
  {
    DeckTrayDeckTileVisual component = obj.GetComponent<DeckTrayDeckTileVisual>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null || !component.IsInUse() || visible == component.gameObject.activeSelf)
      return;
    component.gameObject.SetActive(visible);
  }

  protected abstract void ShowDeckBigCard(DeckTrayDeckTileVisual cardTile, float delay = 0.0f);

  protected abstract void HideDeckBigCard(DeckTrayDeckTileVisual cardTile, bool force = false);

  protected abstract void OnCardTilePress(DeckTrayDeckTileVisual cardTile);

  protected abstract void OnCardTileOver(DeckTrayDeckTileVisual cardTile);

  protected abstract void OnCardTileOut(DeckTrayDeckTileVisual cardTile);

  protected abstract void OnCardTileRelease(DeckTrayDeckTileVisual cardTile);

  public bool IsShowingDeckContents() => this.GetCurrentContentType() != 0;

  public bool IsShowingTeamContents() => this.GetCurrentContentType() != DeckTray.DeckContentTypes.Teams;

  protected void OnBusyWithDeck(bool busy)
  {
    if ((UnityEngine.Object) this.m_inputBlocker == (UnityEngine.Object) null)
      Log.All.PrintError("If this happens, please notify JMac and copy your stack trace to bug 21743!");
    else
      this.m_inputBlocker.SetActive(busy);
  }

  protected virtual void OnEditedDeckChanged(
    CollectionDeck newDeck,
    CollectionDeck oldDeck,
    object callbackData)
  {
    bool isNewDeck = callbackData != null && callbackData is bool flag && flag;
    foreach (KeyValuePair<DeckTray.DeckContentTypes, DeckTrayContent> content in this.m_contents)
      content.Value.OnEditedDeckChanged(newDeck, oldDeck, isNewDeck);
  }

  protected virtual void OnEditingTeamChanged(
    LettuceTeam newTeam,
    LettuceTeam oldTeam,
    object callbackData)
  {
    bool isNewTeam = callbackData != null && callbackData is bool flag && flag;
    foreach (KeyValuePair<DeckTray.DeckContentTypes, DeckTrayContent> content in this.m_contents)
      content.Value.OnEditingTeamChanged(newTeam, oldTeam, isNewTeam);
  }

  public abstract bool OnBackOutOfContainerContents();

  protected void FireModeSwitchedEvent()
  {
    foreach (DeckTray.ModeSwitched modeSwitched in this.m_modeSwitchedListeners.ToArray())
      modeSwitched();
  }

  public void RegisterModeSwitchedListener(DeckTray.ModeSwitched callback) => this.m_modeSwitchedListeners.Add(callback);

  public void UnregisterModeSwitchedListener(DeckTray.ModeSwitched callback) => this.m_modeSwitchedListeners.Remove(callback);

  public enum DeckContentTypes
  {
    Decks,
    Cards,
    HeroSkin,
    CardBack,
    Coin,
    Teams,
    Mercs,
    INVALID,
  }

  public delegate void ModeSwitched();

  [Serializable]
  public class DeckContentScroll
  {
    public DeckTray.DeckContentTypes m_contentType;
    public GameObject m_scrollObject;
    public bool m_saveScrollPosition;
    private Vector3 m_startPosition;
    private float m_currentScroll;

    public void SaveStartPosition()
    {
      if (!((UnityEngine.Object) this.m_scrollObject != (UnityEngine.Object) null))
        return;
      this.m_startPosition = this.m_scrollObject.transform.localPosition;
    }

    public Vector3 GetStartPosition() => this.m_startPosition;

    public void SaveCurrentScroll(float scroll) => this.m_currentScroll = scroll;

    public float GetCurrentScroll() => this.m_currentScroll;
  }
}
