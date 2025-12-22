using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionDeckInfo : MonoBehaviour
{
  public GameObject m_root;
  public GameObject m_visualRoot;
  public GameObject m_heroPowerParent;
  public UberText m_heroPowerName;
  public UberText m_heroPowerDescription;
  public UberText m_manaCurveTooltipText;
  public PegUIElement m_offClicker;
  public List<DeckInfoManaBar> m_manaBars;
  private readonly float MANA_COST_TEXT_MIN_LOCAL_Z;
  private readonly float MANA_COST_TEXT_MAX_LOCAL_Z = 5.167298f;
  private Actor m_heroPowerActor;
  private Actor m_goldenHeroPowerActor;
  private DefLoader.DisposableCardDef m_heroCardDef;
  private bool m_wasTouchModeEnabled;
  protected bool m_shown = true;
  private string m_heroPowerID = "";
  private List<CollectionDeckInfo.ShowListener> m_showListeners = new List<CollectionDeckInfo.ShowListener>();
  private List<CollectionDeckInfo.HideListener> m_hideListeners = new List<CollectionDeckInfo.HideListener>();

  private void Awake()
  {
    this.m_manaCurveTooltipText.Text = GameStrings.Get("GLUE_COLLECTION_DECK_INFO_MANA_TOOLTIP");
    foreach (DeckInfoManaBar manaBar in this.m_manaBars)
      manaBar.m_costText.Text = this.GetTextForManaCost(manaBar.m_manaCostID);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_HERO_POWER, TAG_PREMIUM.GOLDEN), new PrefabCallback<GameObject>(this.OnGoldenHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    this.m_wasTouchModeEnabled = true;
  }

  private void Start()
  {
    this.m_offClicker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClosePressed));
    this.m_offClicker.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OverOffClicker));
  }

  private void OnDestroy()
  {
    this.m_heroCardDef?.Dispose();
    this.m_heroCardDef = (DefLoader.DisposableCardDef) null;
  }

  private void Update()
  {
    if (this.m_wasTouchModeEnabled == UniversalInputManager.Get().IsTouchMode())
      return;
    this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
    if (UniversalInputManager.Get().IsTouchMode())
    {
      if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
        this.m_heroPowerActor.TurnOffCollider();
      if ((UnityEngine.Object) this.m_goldenHeroPowerActor != (UnityEngine.Object) null)
        this.m_goldenHeroPowerActor.TurnOffCollider();
      this.m_offClicker.gameObject.SetActive(true);
    }
    else
    {
      if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
        this.m_heroPowerActor.TurnOnCollider();
      if ((UnityEngine.Object) this.m_goldenHeroPowerActor != (UnityEngine.Object) null)
        this.m_goldenHeroPowerActor.TurnOnCollider();
      this.m_offClicker.gameObject.SetActive(true);
    }
  }

  public void Show()
  {
    if (this.m_shown)
      return;
    if ((UnityEngine.Object) CollectionDeckTray.Get() == (UnityEngine.Object) null)
      this.m_visualRoot.SetActive(true);
    else
      this.m_visualRoot.SetActive(!CollectionDeckTray.Get().GetCardsContent().GetEditingDeck().HasUIHeroOverride());
    this.m_root.SetActive(true);
    this.m_shown = true;
    if (UniversalInputManager.Get().IsTouchMode())
      Navigation.Push(new Navigation.NavigateBackHandler(this.GoBackImpl));
    foreach (CollectionDeckInfo.ShowListener showListener in this.m_showListeners.ToArray())
      showListener();
  }

  private bool GoBackImpl()
  {
    this.Hide();
    return true;
  }

  public void Hide()
  {
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.GoBackImpl));
    if (!this.m_shown)
      return;
    this.m_root.SetActive(false);
    this.m_shown = false;
    foreach (CollectionDeckInfo.HideListener hideListener in this.m_hideListeners.ToArray())
      hideListener();
  }

  public void RegisterShowListener(CollectionDeckInfo.ShowListener dlg) => this.m_showListeners.Add(dlg);

  public void UnregisterShowListener(CollectionDeckInfo.ShowListener dlg) => this.m_showListeners.Remove(dlg);

  public void RegisterHideListener(CollectionDeckInfo.HideListener dlg) => this.m_hideListeners.Add(dlg);

  public void UnregisterHideListener(CollectionDeckInfo.HideListener dlg) => this.m_hideListeners.Remove(dlg);

  public bool IsShown() => this.m_shown;

  public void UpdateManaCurve() => this.UpdateManaCurve(CollectionDeckTray.Get().GetCardsContent().GetEditingDeck());

  public void UpdateManaCurve(CollectionDeck deck)
  {
    if (deck == null)
    {
      Debug.LogWarning((object) string.Format("CollectionDeckInfo.UpdateManaCurve(): deck is null."));
    }
    else
    {
      string heroCardId = deck.HeroCardID;
      CardPortraitQuality quality = new CardPortraitQuality(3, TAG_PREMIUM.NORMAL);
      DefLoader.Get().LoadCardDef(heroCardId, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnHeroCardDefLoaded), quality: quality);
      foreach (DeckInfoManaBar manaBar in this.m_manaBars)
        manaBar.m_numCards = 0;
      int num = 0;
      foreach (CollectionDeckSlot slot in deck.GetSlots())
      {
        EntityDef entityDef = DefLoader.Get().GetEntityDef(slot.CardID);
        int manaCost = entityDef.GetCost();
        if (manaCost > 7)
          manaCost = 7;
        DeckInfoManaBar deckInfoManaBar = this.m_manaBars.Find((Predicate<DeckInfoManaBar>) (obj => obj.m_manaCostID == manaCost));
        if (deckInfoManaBar == null)
        {
          Debug.LogWarning((object) string.Format("CollectionDeckInfo.UpdateManaCurve(): Cannot update curve. Could not find mana bar for {0} (cost {1})", (object) entityDef, (object) manaCost));
          return;
        }
        deckInfoManaBar.m_numCards += slot.Count;
        if (deckInfoManaBar.m_numCards > num)
          num = deckInfoManaBar.m_numCards;
      }
      foreach (DeckInfoManaBar manaBar in this.m_manaBars)
      {
        manaBar.m_numCardsText.Text = Convert.ToString(manaBar.m_numCards);
        float t = num == 0 ? 0.0f : (float) manaBar.m_numCards / (float) num;
        Vector3 localPosition = manaBar.m_numCardsText.transform.localPosition with
        {
          z = Mathf.Lerp(this.MANA_COST_TEXT_MIN_LOCAL_Z, this.MANA_COST_TEXT_MAX_LOCAL_Z, t)
        };
        manaBar.m_numCardsText.transform.localPosition = localPosition;
        RendererExtension.GetMaterial(manaBar.m_barFill.GetComponent<Renderer>()).SetFloat("_Percent", t);
      }
    }
  }

  public void SetDeck(CollectionDeck deck)
  {
    if (deck == null)
    {
      Debug.LogWarning((object) string.Format("CollectionDeckInfo.SetDeckID(): deck is null"));
    }
    else
    {
      this.UpdateManaCurve(deck);
      if (!string.IsNullOrEmpty(deck.HeroPowerCardID))
      {
        this.m_heroPowerID = deck.HeroPowerCardID;
      }
      else
      {
        string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(deck.HeroCardID);
        if (string.IsNullOrEmpty(powerCardIdFromHero))
        {
          if (!deck.HeroCardID.Equals("None"))
            Debug.LogWarning((object) ("CollectionDeckInfo.UpdateInfo(): invalid hero power ID with given hero card ID " + deck.HeroCardID));
          this.m_heroPowerID = "";
          return;
        }
        if (powerCardIdFromHero.Equals(this.m_heroPowerID))
          return;
        this.m_heroPowerID = powerCardIdFromHero;
      }
      TAG_PREMIUM userData = CollectionManager.Get().GetHeroPremium(deck.GetClass());
      if (SceneMgr.Get().IsInDuelsMode())
        userData = TAG_PREMIUM.NORMAL;
      DefLoader.Get().LoadFullDef(this.m_heroPowerID, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroPowerFullDefLoaded), (object) userData);
    }
  }

  private string GetTextForManaCost(int manaCostID)
  {
    if (manaCostID < 0 || manaCostID > 7)
    {
      Debug.LogWarning((object) string.Format("CollectionDeckInfo.GetTextForManaCost(): don't know how to handle mana cost ID {0}", (object) manaCostID));
      return "";
    }
    string textForManaCost = Convert.ToString(manaCostID);
    if (manaCostID == 7)
      textForManaCost += GameStrings.Get("GLUE_COLLECTION_PLUS");
    return textForManaCost;
  }

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("CollectionDeckInfo.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("CollectionDeckInfo.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_heroPowerActor.SetUnlit();
        this.m_heroPowerActor.transform.parent = this.m_heroPowerParent.transform;
        this.m_heroPowerActor.transform.localScale = Vector3.one;
        this.m_heroPowerActor.transform.localPosition = Vector3.zero;
        if (!UniversalInputManager.Get().IsTouchMode())
          return;
        this.m_heroPowerActor.TurnOffCollider();
      }
    }
  }

  private void OnGoldenHeroPowerActorLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("CollectionDeckInfo.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_goldenHeroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_goldenHeroPowerActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("CollectionDeckInfo.OnGoldenHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_goldenHeroPowerActor.SetUnlit();
        this.m_goldenHeroPowerActor.transform.parent = this.m_heroPowerParent.transform;
        this.m_goldenHeroPowerActor.transform.localScale = Vector3.one;
        this.m_goldenHeroPowerActor.transform.localPosition = Vector3.zero;
        if (!UniversalInputManager.Get().IsTouchMode())
          return;
        this.m_goldenHeroPowerActor.TurnOffCollider();
      }
    }
  }

  private void OnHeroPowerFullDefLoaded(
    string cardID,
    DefLoader.DisposableFullDef def,
    object userData)
  {
    TAG_PREMIUM premium = (TAG_PREMIUM) userData;
    this.StartCoroutine(this.SetHeroPowerInfoWhenReady(cardID, def, premium));
  }

  private IEnumerator SetHeroPowerInfoWhenReady(
    string heroPowerCardID,
    DefLoader.DisposableFullDef def,
    TAG_PREMIUM premium)
  {
    using (def)
    {
      while ((UnityEngine.Object) this.m_goldenHeroPowerActor == (UnityEngine.Object) null)
        yield return (object) null;
      while ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
        yield return (object) null;
      this.SetHeroPowerInfo(heroPowerCardID, def, premium);
    }
  }

  private void SetHeroPowerInfo(
    string heroPowerCardID,
    DefLoader.DisposableFullDef def,
    TAG_PREMIUM premium)
  {
    if (!heroPowerCardID.Equals(this.m_heroPowerID))
      return;
    if (premium == TAG_PREMIUM.GOLDEN)
    {
      if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
        this.m_heroPowerActor.Hide();
      this.m_goldenHeroPowerActor.Show();
      this.m_goldenHeroPowerActor.SetFullDef(def);
      this.m_goldenHeroPowerActor.SetUnlit();
      this.m_goldenHeroPowerActor.SetPremium(premium);
      this.m_goldenHeroPowerActor.UpdateAllComponents();
    }
    else
    {
      if ((UnityEngine.Object) this.m_goldenHeroPowerActor != (UnityEngine.Object) null)
        this.m_goldenHeroPowerActor.Hide();
      this.m_heroPowerActor.Show();
      this.m_heroPowerActor.SetFullDef(def);
      this.m_heroPowerActor.SetUnlit();
      this.m_heroPowerActor.UpdateAllComponents();
    }
    this.m_heroPowerName.Text = def.EntityDef.GetName();
    this.m_heroPowerDescription.Text = def.EntityDef.GetCardTextInHand();
  }

  private void OnHeroCardDefLoaded(string cardId, DefLoader.DisposableCardDef def, object userData)
  {
    this.m_heroCardDef?.Dispose();
    this.m_heroCardDef = def;
  }

  private void OnClosePressed(UIEvent e) => this.Hide();

  private void OverOffClicker(UIEvent e) => this.Hide();

  public delegate void ShowListener();

  public delegate void HideListener();
}
