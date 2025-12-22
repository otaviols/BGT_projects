using System.Collections;
using UnityEngine;

public class BaconCollectionHeroSkin : BaconCollectionSkin
{
  private BaconHeroSkinUtils.RotationType m_rotationType;
  private bool m_playerHasEarlyAccessHeroes;
  public GameObject m_heroPowerParent;
  private Actor m_heroPowerActor;
  public GameObject m_unownedStateTextWrapper;
  public UberText m_unownedStateUberText;

  public override void Awake()
  {
    base.Awake();
    if (!((Object) this.gameObject.GetComponent<Actor>() != (Object) null))
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Bacon_Collection_HeroPower.prefab:cba9305dae5005f45814f741f72e532d", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  public void SetHeroPower(string heroPowerCardId) => DefLoader.Get().LoadFullDef(heroPowerCardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroPowerFullDefLoaded));

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
    {
      Debug.LogWarning((object) string.Format("CollectionDeckInfo.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroPowerActor = go.GetComponent<Actor>();
      if ((Object) this.m_heroPowerActor == (Object) null)
      {
        Debug.LogWarning((object) string.Format("BaconCollectionHeroSkin.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
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

  private void OnHeroPowerFullDefLoaded(
    string cardID,
    DefLoader.DisposableFullDef def,
    object userData)
  {
    this.StartCoroutine(this.SetHeroPowerInfoWhenReady(cardID, def, TAG_PREMIUM.NORMAL));
  }

  private IEnumerator SetHeroPowerInfoWhenReady(
    string heroPowerCardID,
    DefLoader.DisposableFullDef def,
    TAG_PREMIUM premium)
  {
    using (def)
    {
      while ((Object) this.m_heroPowerActor == (Object) null)
        yield return (object) null;
      this.SetHeroPowerInfo(heroPowerCardID, def, premium);
    }
  }

  public void AddHeroPowerGhosting(EntityDef def) => this.StartCoroutine(this.AddHeroPowerGhostingWhenReady(def));

  private IEnumerator AddHeroPowerGhostingWhenReady(EntityDef def)
  {
    BaconCollectionHeroSkin collectionHeroSkin = this;
    while ((Object) collectionHeroSkin.m_heroPowerActor == (Object) null)
      yield return (object) null;
    Actor component = collectionHeroSkin.gameObject.GetComponent<Actor>();
    collectionHeroSkin.m_heroPowerActor.gameObject.GetComponent<BaconCollectionHeroPower>()?.HideItemsForGhostView();
    if (def.IsHeroSkin() && HeroSkinUtils.CanBuyHeroSkinFromCollectionManager(def.GetCardId()))
      component.GhostCardEffect(GhostCard.Type.MISSING);
    else
      component.MissingCardEffect();
  }

  private void SetHeroPowerInfo(
    string heroPowerCardID,
    DefLoader.DisposableFullDef def,
    TAG_PREMIUM premium)
  {
    this.m_heroPowerActor.Show();
    this.m_heroPowerActor.SetFullDef(def);
    this.m_heroPowerActor.SetUnlit();
    this.m_heroPowerActor.UpdateAllComponents();
    this.m_heroPowerActor.GetCostTextObject()?.SetActive(false);
    this.m_heroPowerActor.m_manaObject?.SetActive(false);
  }

  protected override void PopulateNameText()
  {
    if (this.m_rotationType == BaconHeroSkinUtils.RotationType.Resting)
    {
      this.GetActiveNameWrapper().SetActive(false);
      this.m_favoriteStateTextWrapper.SetActive(false);
      this.m_unownedStateTextWrapper.SetActive(true);
      this.m_unownedStateUberText.Text = GameStrings.Get("GLUE_BACON_COLLECTION_RESTING");
    }
    else if (!this.m_playerHasEarlyAccessHeroes && this.m_rotationType == BaconHeroSkinUtils.RotationType.Preview)
    {
      this.GetActiveNameWrapper().SetActive(false);
      this.m_favoriteStateTextWrapper.SetActive(false);
      this.m_unownedStateTextWrapper.SetActive(true);
      this.m_unownedStateUberText.Text = GameStrings.Get("GLUE_BACON_COLLECTION_PREVIEWING");
    }
    else
    {
      this.m_unownedStateTextWrapper.SetActive(false);
      base.PopulateNameText();
    }
  }

  public void SetCardStateDisplay(
    CollectibleCard card,
    EntityDef entityDef,
    bool playerHasEarlyAccessHeroes)
  {
    string battlegroundsBaseHeroCardId = CollectionManager.Get().GetBattlegroundsBaseHeroCardId(card.CardId);
    this.m_rotationType = BaconHeroSkinUtils.GetBattleGroundsHeroRotationType(GameUtils.GetCardRecord(battlegroundsBaseHeroCardId), DefLoader.Get().GetEntityDef(battlegroundsBaseHeroCardId));
    this.m_playerHasEarlyAccessHeroes = playerHasEarlyAccessHeroes;
    CollectionManager.Get().GetCollectibleDisplay();
    bool flag1 = this.m_rotationType == BaconHeroSkinUtils.RotationType.Resting;
    bool flag2 = !playerHasEarlyAccessHeroes && this.m_rotationType == BaconHeroSkinUtils.RotationType.Preview;
    if (card.OwnedCount == 0 | flag1 | flag2)
      this.AddHeroPowerGhosting(entityDef);
    this.PopulateNameText();
  }
}
