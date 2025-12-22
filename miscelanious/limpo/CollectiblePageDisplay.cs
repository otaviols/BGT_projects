using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectiblePageDisplay : BookPageDisplay
{
  public GameObject m_cardStartPositionEightCards;
  public UberText m_pageCountText;
  public UberText m_pageNameText;
  public GameObject m_pageFlavorHeader;
  public GameObject m_basePage;
  public Material m_headerMaterial;
  public Material m_pageMaterial;
  public Color m_textColor;
  protected List<CollectionCardVisual> m_collectionCardVisuals = new List<CollectionCardVisual>();
  protected Material m_basePageMaterial;

  public override bool IsLoaded() => true;

  public static int GetMaxCardsPerPage()
  {
    CollectionUtils.CollectionPageLayoutSettings.Variables pageLayoutSettings = CollectionManager.Get().GetCollectibleDisplay().GetCurrentPageLayoutSettings();
    return pageLayoutSettings.m_ColumnCount * pageLayoutSettings.m_RowCount;
  }

  public static int GetMaxCardsPerPage(CollectionUtils.ViewMode viewMode)
  {
    if (CollectionManager.Get() == null || (Object) CollectionManager.Get().GetCollectibleDisplay() == (Object) null)
    {
      Log.CollectionManager.Print("CollectiblePageDisplay.GetMaxCardsPerPage - Null checks failed! mode={0}", (object) viewMode);
      return 0;
    }
    CollectionUtils.CollectionPageLayoutSettings.Variables pageLayoutSettings = CollectionManager.Get().GetCollectibleDisplay().GetPageLayoutSettings(viewMode);
    return pageLayoutSettings.m_ColumnCount * pageLayoutSettings.m_RowCount;
  }

  public CollectionCardVisual GetCardVisual(string cardID, TAG_PREMIUM premium)
  {
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown() && collectionCardVisual.GetVisualType() == CollectionUtils.ViewMode.CARDS)
      {
        Actor actor = collectionCardVisual.GetActor();
        if (actor.GetEntityDef().GetCardId().Equals(cardID) && actor.GetPremium() == premium)
          return collectionCardVisual;
      }
    }
    return (CollectionCardVisual) null;
  }

  public override void Show()
  {
    base.Show();
    MassDisenchant massDisenchant = MassDisenchant.Get();
    if ((Object) massDisenchant != (Object) null && massDisenchant.IsShown())
      return;
    for (int index = 0; index < this.m_collectionCardVisuals.Count; ++index)
    {
      CollectionCardVisual collectionCardVisual = this.GetCollectionCardVisual(index);
      if ((Object) collectionCardVisual.GetActor() != (Object) null)
        collectionCardVisual.Show();
    }
  }

  public override void Hide()
  {
    base.Hide();
    this.MarkAllShownCardsSeen();
    for (int index = 0; index < this.m_collectionCardVisuals.Count; ++index)
      this.GetCollectionCardVisual(index).Hide();
  }

  public void UpdateAllSpecialCaseTransforms()
  {
    for (int index = 0; index < this.m_collectionCardVisuals.Count; ++index)
      this.GetCollectionCardVisual(index).UpdateSpecialCaseTransform();
  }

  public virtual void MarkAllShownCardsSeen()
  {
    for (int index = 0; index < this.m_collectionCardVisuals.Count; ++index)
    {
      CollectionCardVisual collectionCardVisual = this.GetCollectionCardVisual(index);
      if (collectionCardVisual.IsShown())
        collectionCardVisual.MarkAsSeen();
    }
  }

  public virtual void UpdateCollectionItems(
    List<CollectionCardActors> actorList,
    List<ICollectible> nonActorCollectibles,
    CollectionUtils.ViewMode mode)
  {
    Log.CollectionManager.Print("mode={0}", (object) mode);
    int index1;
    for (index1 = 0; index1 < actorList.Count && index1 < CollectiblePageDisplay.GetMaxCardsPerPage(); ++index1)
    {
      CollectionCardVisual collectionCardVisual = this.GetCollectionCardVisual(index1);
      collectionCardVisual.SetActors(actorList[index1], mode);
      collectionCardVisual.Show();
      if (mode == CollectionUtils.ViewMode.HERO_SKINS)
        collectionCardVisual.SetHeroSkinBoxCollider();
      else
        collectionCardVisual.SetDefaultBoxCollider();
    }
    for (int index2 = index1; index2 < this.m_collectionCardVisuals.Count; ++index2)
    {
      CollectionCardVisual collectionCardVisual = this.GetCollectionCardVisual(index2);
      collectionCardVisual.Hide();
      collectionCardVisual.SetActors((CollectionCardActors) null);
    }
    this.UpdateCurrentPageCardLocks();
  }

  public void UpdateBasePage()
  {
    if (!((Object) this.m_basePageMaterial != (Object) null) || !((Object) this.m_basePage != (Object) null))
      return;
    this.m_basePage.GetComponent<MeshRenderer>().SetMaterial(this.m_basePageMaterial);
  }

  public abstract void ShowNoMatchesFound(
    bool show,
    CollectionManager.FindCardsResult findResults = null,
    bool showHints = true);

  public List<CollectionCardVisual> ApplyRuneCardGhostEffectsForCurrentPage(
    RunePattern deckRunePattern)
  {
    List<CollectionCardVisual> collectionCardVisualList = new List<CollectionCardVisual>();
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        Actor actor = collectionCardVisual.GetActor();
        if (!((Object) actor == (Object) null))
        {
          EntityDef entityDef = actor.GetEntityDef();
          if (entityDef != null && !deckRunePattern.CanAddRunes(entityDef.GetRuneCost(), DeckRule_DeathKnightRuneLimit.MaxRuneSlots))
          {
            actor.GhostCardEffect(GhostCard.Type.NOT_VALID, actor.GetPremium());
            collectionCardVisualList.Add(collectionCardVisual);
          }
        }
      }
    }
    return collectionCardVisualList;
  }

  public virtual void UpdateCurrentPageCardLocks(bool playSound = false)
  {
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if (!((Object) collectionDeckTray != (Object) null) || collectionDeckTray.GetCurrentContentType() == DeckTray.DeckContentTypes.Cards)
      return;
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        if (collectionCardVisual.GetVisualType() == CollectionUtils.ViewMode.CARDS)
        {
          Actor actor = collectionCardVisual.GetActor();
          string cardId = actor.GetEntityDef().GetCardId();
          TAG_PREMIUM premium = actor.GetPremium();
          CollectibleCard card = CollectionManager.Get().GetCard(cardId, premium);
          if (!GameUtils.IsCardGameplayEventActive(cardId) && card.OwnedCount > 0)
          {
            collectionCardVisual.ShowLock(CollectionCardVisual.LockType.NOT_PLAYABLE, GameStrings.Get("GLUE_COLLECTION_LOCK_CARD_NOT_PLAYABLE"), playSound);
            continue;
          }
        }
        collectionCardVisual.ShowLock(CollectionCardVisual.LockType.NONE);
      }
    }
  }

  public abstract void SetPageType(FormatType formatType);

  public void SetPageCountText(string text)
  {
    if (!((Object) this.m_pageCountText != (Object) null))
      return;
    this.m_pageCountText.Text = text;
  }

  public void ActivatePageCountText(bool active)
  {
    if (!((Object) this.m_pageCountText != (Object) null))
      return;
    this.m_pageCountText.gameObject.SetActive(active);
  }

  protected CollectionCardVisual GetCollectionCardVisual(int index)
  {
    CollectionUtils.CollectionPageLayoutSettings.Variables pageLayoutSettings = CollectionManager.Get().GetCollectibleDisplay().GetCurrentPageLayoutSettings();
    float columnSpacing = pageLayoutSettings.m_ColumnSpacing;
    int columnCount = pageLayoutSettings.m_ColumnCount;
    float num = columnSpacing * (float) (columnCount - 1);
    float scale = pageLayoutSettings.m_Scale;
    float rowSpacing = pageLayoutSettings.m_RowSpacing;
    Vector3 position = this.m_cardStartPositionEightCards.transform.localPosition + pageLayoutSettings.m_Offset;
    int rowNum = index / columnCount;
    position.x += (float) ((double) (index % columnCount) * (double) columnSpacing - (double) num * 0.5);
    position.z -= rowSpacing * (float) rowNum;
    CollectionCardVisual collectionCardVisual;
    if (index == this.m_collectionCardVisuals.Count)
    {
      collectionCardVisual = (CollectionCardVisual) GameUtils.Instantiate((Component) CollectionManager.Get().GetCollectibleDisplay().GetCardVisualPrefab(), this.gameObject);
      this.m_collectionCardVisuals.Insert(index, collectionCardVisual);
    }
    else
      collectionCardVisual = this.m_collectionCardVisuals[index];
    collectionCardVisual.SetCMRow(rowNum);
    collectionCardVisual.transform.localScale = new Vector3(scale, scale, scale);
    collectionCardVisual.transform.position = this.transform.TransformPoint(position);
    return collectionCardVisual;
  }

  protected void SetPageNameText(string className)
  {
    if (!((Object) this.m_pageNameText != (Object) null))
      return;
    this.m_pageNameText.Text = className;
  }

  public static void SetPageFlavorTextures(GameObject header, UnityEngine.Vector2 offset)
  {
    if ((Object) header == (Object) null)
      return;
    header.GetComponent<Renderer>().GetMaterial().SetTextureOffset("_MainTex", offset);
    if (!((Object) header != (Object) null))
      return;
    header.SetActive(true);
  }
}
