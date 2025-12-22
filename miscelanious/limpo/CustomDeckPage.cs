using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

public class CustomDeckPage : MonoBehaviour
{
  public Vector3 m_customDeckStart;
  public Vector3 m_customDeckScale;
  public float m_customDeckHorizontalSpacing;
  public float m_customDeckVerticalSpacing;
  public CollectionDeckBoxVisual m_deckboxPrefab;
  public Vector3 m_deckCoverOffset;
  public GameObject m_deckboxCoverPrefab;
  public PlayMakerFSM m_vineGlowBurst;
  public GameObject[] m_customVineGlowToggle;
  public int m_maxCustomDecksToDisplay = 9;
  public Material m_multipleDeckSelectionHighlightMaterial;
  [HideInInspector]
  public bool m_isPageForLoanerDecks;
  protected List<GameObject> m_deckCovers = new List<GameObject>();
  protected List<CollectionDeck> m_collectionDecks;
  protected int m_numCustomDecks;
  protected List<CollectionDeckBoxVisual> m_customDecks = new List<CollectionDeckBoxVisual>();
  protected CustomDeckPage.DeckButtonCallback m_deckButtonCallback;
  private Texture m_customTrayMainTexture;
  private Texture m_customTrayTransitionToTexture;
  private Renderer m_renderer;
  private bool m_initialized;
  public const int DEFAULT_MAX_CUSTOM_DECKS_TO_DISPLAY = 9;

  private void Start() => this.m_renderer = this.GetComponent<Renderer>();

  public void Show()
  {
    this.m_renderer.enabled = true;
    for (int index = 0; index < this.m_numCustomDecks; ++index)
    {
      if (index < this.m_customDecks.Count)
        this.m_customDecks[index].Show();
    }
  }

  public void Hide()
  {
    this.m_renderer.enabled = false;
    for (int index = 0; index < this.m_numCustomDecks; ++index)
    {
      if (index < this.m_customDecks.Count)
        this.m_customDecks[index].Hide();
    }
  }

  public virtual bool PageReady() => (Object) this.m_customTrayMainTexture != (Object) null && this.AreAllCustomDecksReady();

  public CollectionDeckBoxVisual GetDeckboxWithDeckID(long deckID)
  {
    if (deckID <= 0L)
      return (CollectionDeckBoxVisual) null;
    foreach (CollectionDeckBoxVisual customDeck in this.m_customDecks)
    {
      if (customDeck.GetDeckID() == deckID)
        return customDeck;
    }
    return (CollectionDeckBoxVisual) null;
  }

  public CollectionDeckBoxVisual GetDeckboxWithDeckTemplateID(
    long deckTemplateID)
  {
    if (deckTemplateID <= 0L)
      return (CollectionDeckBoxVisual) null;
    foreach (CollectionDeckBoxVisual customDeck in this.m_customDecks)
    {
      if (customDeck.m_isLoanerDeck && (long) customDeck.GetDeckTemplateId() == deckTemplateID)
        return customDeck;
    }
    return (CollectionDeckBoxVisual) null;
  }

  public void UpdateTrayTransitionValue(float transitionValue)
  {
    this.GetComponent<Renderer>().GetMaterial().SetFloat("_Transistion", transitionValue);
    foreach (GameObject deckCover in this.m_deckCovers)
    {
      Renderer componentInChildren = deckCover.GetComponentInChildren<Renderer>();
      if ((Object) componentInChildren != (Object) null)
        componentInChildren.GetMaterial().SetFloat("_Transistion", transitionValue);
    }
  }

  public void PlayVineGlowBurst(bool useFX, bool hasValidStandardDeck)
  {
    if (!((Object) this.m_vineGlowBurst != (Object) null))
      return;
    string eventName = !useFX ? (hasValidStandardDeck ? "GlowVinesNoFX" : "GlowVinesCustomNoFX") : (hasValidStandardDeck ? "GlowVines" : "GlowVinesCustom");
    if (string.IsNullOrEmpty(eventName))
      return;
    this.m_vineGlowBurst.SendEvent(eventName);
  }

  public void SetTrayTextures(Texture transitionFromTexture, Texture targetTexture)
  {
    Material material1 = this.GetComponent<Renderer>().GetMaterial();
    material1.mainTexture = transitionFromTexture;
    material1.SetTexture("_MainTex2", targetTexture);
    material1.SetFloat("_Transistion", 0.0f);
    this.m_customTrayMainTexture = transitionFromTexture;
    this.m_customTrayTransitionToTexture = targetTexture;
    foreach (GameObject deckCover in this.m_deckCovers)
    {
      Material material2 = deckCover.GetComponentInChildren<Renderer>().GetMaterial();
      material2.mainTexture = this.m_customTrayMainTexture;
      material2.SetTexture("_MainTex2", this.m_customTrayTransitionToTexture);
      material2.SetFloat("_Transistion", 0.0f);
    }
    if (this.m_isPageForLoanerDecks)
      return;
    this.UpdateDeckVisuals(this.m_collectionDecks);
  }

  public void SetDeckButtonCallback(CustomDeckPage.DeckButtonCallback callback) => this.m_deckButtonCallback = callback;

  public void EnableDeckButtons(bool enable)
  {
    foreach (CollectionDeckBoxVisual customDeck in this.m_customDecks)
    {
      customDeck.SetEnabled(enable, false);
      if (!enable)
        customDeck.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
    }
  }

  public CollectionDeckBoxVisual FindDeckVisual(CollectionDeck deck)
  {
    int index = 0;
    foreach (CollectionDeck collectionDeck in this.m_collectionDecks)
    {
      if (collectionDeck == deck)
        return this.m_customDecks[index];
      ++index;
    }
    return (CollectionDeckBoxVisual) null;
  }

  public void TransitionWildDecks()
  {
    int index = 0;
    foreach (CollectionDeck collectionDeck in this.m_collectionDecks)
    {
      if (collectionDeck.Type == DeckType.NORMAL_DECK)
      {
        CollectionDeckBoxVisual customDeck = this.m_customDecks[index];
        if (collectionDeck.FormatType == FormatType.FT_WILD)
          customDeck.PlayGlowAnim();
        customDeck.UpdateInvalidCardCountIndicator();
        ++index;
      }
    }
  }

  public void UpdateDeckVisuals(List<CollectionDeck> collectionDecks = null)
  {
    int index = 0;
    this.m_numCustomDecks = 0;
    if (collectionDecks == null)
      collectionDecks = this.m_collectionDecks;
    foreach (CollectionDeck collectionDeck in collectionDecks)
    {
      if (collectionDeck.Type == DeckType.NORMAL_DECK)
      {
        if (collectionDeck.FormatType == FormatType.FT_UNKNOWN && !collectionDeck.Locked)
          Debug.LogError((object) ("A deck with an unknown format type was detected. Details: " + collectionDeck.ToString()));
        ++this.m_numCustomDecks;
        CollectionDeckBoxVisual customDeck = this.m_customDecks[index];
        customDeck.SetIsShared(collectionDeck.IsShared);
        customDeck.SetDeckName(collectionDeck.Name);
        if (collectionDeck.IsLoanerDeck)
          customDeck.SetDeckTemplateId(collectionDeck.DeckTemplateId);
        else
          customDeck.SetDeckID(collectionDeck.ID);
        customDeck.SetHeroCardPremiumOverride(collectionDeck.GetDisplayHeroPremiumOverride());
        customDeck.SetHeroCardID(collectionDeck.GetDisplayHeroCardID(!this.m_initialized));
        customDeck.SetFormatType(collectionDeck.FormatType);
        customDeck.UpdateInvalidCardCountIndicator();
        customDeck.m_isLoanerDeck = this.m_isPageForLoanerDecks;
        customDeck.UpdateRuneSlotVisual(collectionDeck);
        customDeck.Show();
        if (index < this.m_deckCovers.Count)
          this.m_deckCovers[index].SetActive(false);
        ++index;
        if (index >= this.m_customDecks.Count)
          break;
      }
    }
    for (; index < this.m_customDecks.Count; ++index)
    {
      this.m_customDecks[index].Hide();
      if (index < this.m_deckCovers.Count)
        this.m_deckCovers[index].SetActive(true);
    }
  }

  public bool HasWildDeck()
  {
    foreach (CollectionDeck collectionDeck in this.m_collectionDecks)
    {
      if (collectionDeck.FormatType == FormatType.FT_WILD)
        return true;
    }
    return false;
  }

  private bool AreAllCustomDecksReady()
  {
    foreach (CollectionDeckBoxVisual customDeck in this.m_customDecks)
    {
      if (customDeck.IsLoading())
        return false;
    }
    return true;
  }

  public void InitDecks(List<CollectionDeck> decks, bool isLoanerDeckpage = false)
  {
    this.m_collectionDecks = decks;
    if (this.m_initialized)
      return;
    int num = 0;
    if (isLoanerDeckpage)
      num = 3;
    for (int index = num; index < this.m_maxCustomDecksToDisplay; ++index)
      this.CreateDeck(index);
    this.UpdateDeckVisuals(this.m_collectionDecks);
    this.m_initialized = true;
  }

  private void CreateDeck(int index)
  {
    float horizontalSpacing = this.m_customDeckHorizontalSpacing;
    float deckVerticalSpacing = this.m_customDeckVerticalSpacing;
    GameObject go = new GameObject();
    go.name = "DeckParent" + (object) index;
    go.transform.parent = this.gameObject.transform;
    if (index == 0)
    {
      go.transform.localPosition = this.m_customDeckStart;
    }
    else
    {
      float x = this.m_customDeckStart.x - (float) (index % 3) * horizontalSpacing;
      float z = (float) Mathf.CeilToInt((float) (index / 3)) * deckVerticalSpacing + this.m_customDeckStart.z;
      go.transform.localPosition = new Vector3(x, this.m_customDeckStart.y, z);
    }
    CollectionDeckBoxVisual deckBox = Object.Instantiate<CollectionDeckBoxVisual>(this.m_deckboxPrefab);
    CollectionDeckBoxVisual collectionDeckBoxVisual = deckBox;
    collectionDeckBoxVisual.name = collectionDeckBoxVisual.name + " - " + (object) index;
    deckBox.transform.parent = go.transform;
    deckBox.transform.localPosition = Vector3.zero;
    deckBox.StoreOriginalButtonPositionAndRotation();
    go.transform.localScale = this.m_customDeckScale;
    if (this.m_deckButtonCallback == null)
      Debug.LogError((object) "SetDeckButtonCallback() not called in CustomDeckPage!");
    else
      deckBox.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnSelectCustomDeck(deckBox)));
    deckBox.SetEnabled(true, false);
    this.m_customDecks.Add(deckBox);
    this.CreateDeckCover(go);
  }

  private void CreateDeckCover(GameObject go)
  {
    if (!((Object) this.m_deckboxCoverPrefab != (Object) null))
      return;
    GameObject gameObject = Object.Instantiate<GameObject>(this.m_deckboxCoverPrefab);
    gameObject.transform.parent = this.gameObject.transform;
    gameObject.transform.localScale = this.m_customDeckScale;
    gameObject.transform.position = go.transform.position + this.m_deckCoverOffset;
    this.m_deckCovers.Add(gameObject);
  }

  private void OnSelectCustomDeck(CollectionDeckBoxVisual deckbox) => this.m_deckButtonCallback(deckbox);

  public delegate void DeckButtonCallback(CollectionDeckBoxVisual deckbox);
}
