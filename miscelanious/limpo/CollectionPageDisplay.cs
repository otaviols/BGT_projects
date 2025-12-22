using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionPageDisplay : CollectiblePageDisplay
{
  public GameObject m_favoriteBanner;
  public GameObject m_heroSkinsDecor;
  public GameObject[] m_heroSkinFrames;
  public GameObject m_heroPicker;
  public GameObject m_deckTemplateContainer;
  public GameObject m_noMatchFoundObject;
  public UberText m_noMatchExplanationText;
  public GameObject m_noMatchSetHintObject;
  public GameObject m_noMatchManaHintObject;
  public GameObject m_noMatchCraftingHintObject;
  public Material m_deckTemplatePageMaterial;
  public Color m_standardTitleTextColor;
  public Material m_wildHeaderMaterial;
  public Material m_wildPageMaterial;
  public Color m_wildTextColor;
  public Color m_wildTitleTextColor;
  public Material m_classicHeaderMaterial;
  public Material m_classicPageMaterial;
  public Color m_classicTextColor;
  public Color m_classicTitleTextColor;
  public FormatType m_pageFormatType;
  private MassDisenchant m_massDisenchantVisual;

  public override void UpdateCollectionItems(
    List<CollectionCardActors> actorList,
    List<ICollectible> nonActorCollectibleList,
    CollectionUtils.ViewMode mode)
  {
    this.UpdateAllSpecialCaseTransforms();
    base.UpdateCollectionItems(actorList, nonActorCollectibleList, mode);
    this.DetachAndHideMassDisenchantVisual();
    this.UpdateFavoriteCardBacks(mode);
    this.UpdateFavoriteHeroSkins(mode);
    this.UpdateFavoriteCoin(mode);
    this.UpdateHeroSkinNames(mode);
    this.UpdateHeroPicker(mode);
  }

  public void UpdatePageWithMassDisenchant()
  {
    MassDisenchant disenchantVisual = this.GetMassDisenchantVisual();
    if (!((UnityEngine.Object) disenchantVisual != (UnityEngine.Object) null))
      return;
    disenchantVisual.Show();
  }

  private void DetachAndHideMassDisenchantVisual()
  {
    if (!((UnityEngine.Object) this.m_massDisenchantVisual != (UnityEngine.Object) null))
      return;
    this.m_massDisenchantVisual.Hide();
    this.m_massDisenchantVisual = (MassDisenchant) null;
  }

  public void UpdatePageWithHeroPicker(int[] allHeroCounts, int[] ownedHeroCounts)
  {
    CollectionHeroPickerButtons componentInChildren = this.m_heroPicker.GetComponentInChildren<CollectionHeroPickerButtons>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    componentInChildren.LoadHeroButtonsForFavoriteHeroes();
    componentInChildren.Show();
    componentInChildren.UpdateHeroClassTotals(allHeroCounts, ownedHeroCounts);
  }

  public void HideHeroPicker()
  {
    CollectionHeroPickerButtons componentInChildren = this.m_heroPicker.GetComponentInChildren<CollectionHeroPickerButtons>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null) || !componentInChildren.IsReady())
      return;
    componentInChildren.Hide();
  }

  public void UpdateFavoriteHeroSkins(CollectionUtils.ViewMode mode)
  {
    if (mode != CollectionUtils.ViewMode.HERO_SKINS)
    {
      this.HideHeroSkinsDecor();
    }
    else
    {
      if ((UnityEngine.Object) this.m_heroSkinsDecor != (UnityEngine.Object) null && this.m_heroSkinFrames != null)
      {
        this.m_heroSkinsDecor.SetActive(true);
        this.HideAllHeroSkinFrames();
      }
      int num = 0;
      foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
      {
        if (collectionCardVisual.IsShown())
        {
          Actor actor = collectionCardVisual.GetActor();
          CollectionHeroSkin component = actor.GetComponent<CollectionHeroSkin>();
          if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
          {
            component.ShowShadow(actor.IsShown());
            EntityDef entityDef = actor.GetEntityDef();
            if (entityDef != null)
            {
              TAG_CLASS tagClass = entityDef.GetClass();
              string cardId = entityDef.GetCardId();
              component.SetClass(tagClass);
              bool show = CollectionManager.Get().GetCountOfOwnedHeroesForClass(tagClass) > 1 && CollectionManager.Get().IsFavoriteHero(cardId);
              component.ShowFavoriteBanner(show);
            }
          }
          else
            continue;
        }
        if (num < this.m_heroSkinFrames.Length)
          this.m_heroSkinFrames[num++].SetActive(collectionCardVisual.IsShown());
      }
    }
  }

  public void UpdateHeroSkinNames(CollectionUtils.ViewMode mode)
  {
    if (mode != CollectionUtils.ViewMode.HERO_SKINS)
      return;
    this.StartCoroutine(this.WaitThenUpdateHeroSkinNames(mode));
  }

  private IEnumerator WaitThenUpdateHeroSkinNames(CollectionUtils.ViewMode mode)
  {
    CollectionPageDisplay collectionPageDisplay = this;
    yield return (object) null;
    foreach (CollectionCardVisual collectionCardVisual in collectionPageDisplay.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        CollectionHeroSkin component = collectionCardVisual.GetActor().GetComponent<CollectionHeroSkin>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
          component.ShowCollectionManagerText();
      }
    }
  }

  public void UpdateHeroPicker(CollectionUtils.ViewMode mode)
  {
    if (mode == CollectionUtils.ViewMode.HERO_PICKER)
      return;
    this.HideHeroPicker();
  }

  public void UpdateFavoriteCardBacks(CollectionUtils.ViewMode mode)
  {
    if (mode != CollectionUtils.ViewMode.CARD_BACKS)
      return;
    HashSet<int> favoriteCardBacks = CardBackManager.Get().GetCardBacks().FavoriteCardBacks;
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        CollectionCardBack component = collectionCardVisual.GetActor().GetComponent<CollectionCardBack>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
        {
          bool show = favoriteCardBacks.Contains(component.GetCardBackId());
          component.ShowFavoriteBanner(show);
        }
      }
    }
  }

  public void UpdateFavoriteBattlegroundsGuideSkin(CollectionUtils.ViewMode mode)
  {
    int num = mode == CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS ? 1 : 0;
  }

  public void UpdateFavoriteCoin(CollectionUtils.ViewMode mode)
  {
    string favoriteCoinCardId = CoinManager.Get().GetFavoriteCoinCardId();
    if (mode != CollectionUtils.ViewMode.COINS || favoriteCoinCardId == null)
      return;
    this.HideFavoriteBanner();
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown() && collectionCardVisual.CardId == favoriteCoinCardId)
      {
        FavoriteBanner component;
        if (!this.m_favoriteBanner.TryGetComponent<FavoriteBanner>(out component))
          break;
        component.SetActive(true);
        Actor actor = collectionCardVisual.GetActor();
        component.PinToActor(actor);
        break;
      }
    }
  }

  public void HideFavoriteBanner()
  {
    FavoriteBanner component;
    if (!this.m_favoriteBanner.TryGetComponent<FavoriteBanner>(out component))
      return;
    component.SetActive(false);
  }

  public void UpdateDeckTemplateHeader(GameObject deckTemplateHeader, FormatType pageFormatType)
  {
    Renderer component;
    if ((UnityEngine.Object) deckTemplateHeader == (UnityEngine.Object) null || !deckTemplateHeader.TryGetComponent<Renderer>(out component))
      return;
    Material headerMaterial = this.GetHeaderMaterial(pageFormatType, (Material) null);
    component.SetMaterial(headerMaterial);
  }

  public void UpdateDeckTemplatePage(Component deckTemplatePicker)
  {
    if (!((UnityEngine.Object) deckTemplatePicker != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_deckTemplateContainer != (UnityEngine.Object) null))
      return;
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      collectionCardVisual.Hide();
      collectionCardVisual.SetActors((CollectionCardActors) null);
    }
    if ((UnityEngine.Object) this.m_basePage != (UnityEngine.Object) null)
    {
      MeshRenderer component = this.m_basePage.GetComponent<MeshRenderer>();
      this.m_basePageMaterial = component.GetMaterial();
      component.SetMaterial(this.m_deckTemplatePageMaterial);
    }
    GameUtils.SetParent(deckTemplatePicker, this.m_deckTemplateContainer);
    GameUtils.ResetTransform(deckTemplatePicker);
  }

  public override void ShowNoMatchesFound(
    bool show,
    CollectionManager.FindCardsResult findResults = null,
    bool showHints = true)
  {
    this.m_noMatchFoundObject.SetActive(show);
    this.m_noMatchCraftingHintObject.SetActive(false);
    this.m_noMatchSetHintObject.SetActive(false);
    this.m_noMatchManaHintObject.SetActive(false);
    string key = "GLUE_COLLECTION_NO_RESULTS";
    if (show & showHints && findResults != null)
    {
      if (findResults.m_resultsWithoutManaFilterExist)
      {
        this.m_noMatchManaHintObject.SetActive(true);
        key = "GLUE_COLLECTION_NO_RESULTS_IN_SELECTED_COST";
      }
      else if (findResults.m_resultsWithoutSetFilterExist)
      {
        this.m_noMatchSetHintObject.SetActive(true);
        key = "GLUE_COLLECTION_NO_RESULTS_IN_CURRENT_SET";
      }
      else if (findResults.m_resultsUnownedExist)
      {
        this.m_noMatchCraftingHintObject.SetActive(true);
        key = "GLUE_COLLECTION_NO_RESULTS_BUT_CRAFTABLE";
      }
      else if (findResults.m_resultsInWildExist)
        key = "GLUE_COLLECTION_NO_RESULTS_IN_STANDARD";
    }
    this.m_noMatchExplanationText.Text = GameStrings.Get(key);
  }

  public void HideHeroSkinsDecor()
  {
    if ((UnityEngine.Object) this.m_heroSkinsDecor != (UnityEngine.Object) null)
      this.m_heroSkinsDecor.SetActive(false);
    this.HideAllHeroSkinFrames();
  }

  public void HideAllHeroSkinFrames()
  {
    if (this.m_heroSkinFrames == null || ((IEnumerable<GameObject>) this.m_heroSkinFrames).Count<GameObject>() <= 0)
      return;
    foreach (GameObject heroSkinFrame in this.m_heroSkinFrames)
      heroSkinFrame.SetActive(false);
  }

  public override void UpdateCurrentPageCardLocks(bool playSound = false)
  {
    base.UpdateCurrentPageCardLocks(playSound);
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
      return;
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (!collectionCardVisual.IsShown() || collectionCardVisual.GetVisualType() != CollectionUtils.ViewMode.CARDS)
      {
        collectionCardVisual.ShowLock(CollectionCardVisual.LockType.NONE);
      }
      else
      {
        Actor actor = collectionCardVisual.GetActor();
        if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
        {
          string cardId = actor.GetEntityDef()?.GetCardId();
          TAG_PREMIUM premium = actor.GetPremium();
          CollectibleCard card = CollectionManager.Get().GetCard(cardId, premium);
          if (card != null)
          {
            if (card.OwnedCount <= 0)
            {
              collectionCardVisual.ShowLock(CollectionCardVisual.LockType.NONE);
            }
            else
            {
              DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
              if (deckRuleset != null)
              {
                List<RuleInvalidReason> reasons;
                List<DeckRule> brokenRules;
                if (!deckRuleset.CanAddToDeck(actor.GetEntityDef(), premium, editedDeck, out reasons, out brokenRules, DeckRule.RuleType.DEATHKNIGHT_RUNE_LIMIT))
                {
                  string reason = reasons[0] != null ? reasons[0].DisplayError : "NULL";
                  DeckRule deckRule1 = brokenRules[0];
                  CollectionCardVisual.LockType lockType;
                  if ((deckRule1 != null ? (deckRule1.Type == DeckRule.RuleType.IS_CARD_PLAYABLE ? 1 : 0) : 0) != 0)
                  {
                    lockType = CollectionCardVisual.LockType.NOT_PLAYABLE;
                  }
                  else
                  {
                    DeckRule deckRule2 = brokenRules[0];
                    if ((deckRule2 != null ? (deckRule2.Type == DeckRule.RuleType.PLAYER_OWNS_EACH_COPY ? 1 : 0) : 0) != 0)
                    {
                      lockType = CollectionCardVisual.LockType.NO_MORE_INSTANCES;
                    }
                    else
                    {
                      DeckRule deckRule3 = brokenRules[0];
                      lockType = (deckRule3 != null ? (deckRule3.Type == DeckRule.RuleType.COUNT_COPIES_OF_EACH_CARD ? 1 : 0) : 0) == 0 ? CollectionCardVisual.LockType.BANNED : CollectionCardVisual.LockType.MAX_COPIES_IN_DECK;
                    }
                  }
                  if (brokenRules.Count > 1)
                  {
                    int index = brokenRules.FindIndex((Predicate<DeckRule>) (r => r.Type == DeckRule.RuleType.PLAYER_OWNS_EACH_COPY));
                    if (index >= 0)
                    {
                      int allMatchingSlots1 = editedDeck.GetCardCountAllMatchingSlots(cardId, TAG_PREMIUM.SIGNATURE);
                      int allMatchingSlots2 = editedDeck.GetCardCountAllMatchingSlots(cardId, TAG_PREMIUM.DIAMOND);
                      int allMatchingSlots3 = editedDeck.GetCardCountAllMatchingSlots(cardId, TAG_PREMIUM.GOLDEN);
                      int allMatchingSlots4 = editedDeck.GetCardCountAllMatchingSlots(cardId, TAG_PREMIUM.NORMAL);
                      int ownedCount = card.OwnedCount;
                      bool flag = false;
                      switch (premium)
                      {
                        case TAG_PREMIUM.NORMAL:
                          if (allMatchingSlots4 > ownedCount)
                          {
                            flag = true;
                            break;
                          }
                          break;
                        case TAG_PREMIUM.GOLDEN:
                          if (allMatchingSlots1 + allMatchingSlots2 + allMatchingSlots4 > 0)
                          {
                            flag = true;
                            break;
                          }
                          break;
                        case TAG_PREMIUM.DIAMOND:
                          if (allMatchingSlots1 + allMatchingSlots3 + allMatchingSlots4 > 0)
                          {
                            flag = true;
                            break;
                          }
                          break;
                        case TAG_PREMIUM.SIGNATURE:
                          if (allMatchingSlots2 + allMatchingSlots3 + allMatchingSlots4 > 0)
                          {
                            flag = true;
                            break;
                          }
                          break;
                      }
                      if (flag)
                      {
                        lockType = CollectionCardVisual.LockType.NO_MORE_INSTANCES;
                        reason = reasons[index] != null ? reasons[index].DisplayError : "NULL";
                      }
                    }
                  }
                  collectionCardVisual.ShowLock(lockType, reason, playSound);
                  continue;
                }
              }
              collectionCardVisual.ShowLock(CollectionCardVisual.LockType.NONE, (string) null, playSound);
            }
          }
        }
      }
    }
  }

  public override void SetPageType(FormatType inputFormatType)
  {
    if (inputFormatType == this.m_pageFormatType)
      return;
    this.m_pageFormatType = inputFormatType;
    if ((UnityEngine.Object) this.m_pageFlavorHeader != (UnityEngine.Object) null)
    {
      Material headerMaterial = this.GetHeaderMaterial(inputFormatType, (Material) null);
      if ((UnityEngine.Object) headerMaterial != (UnityEngine.Object) null)
        this.m_pageFlavorHeader.GetComponent<Renderer>().SetMaterial(headerMaterial);
    }
    if ((UnityEngine.Object) this.m_pageCountText != (UnityEngine.Object) null)
      this.m_pageCountText.TextColor = this.GetTextColor(inputFormatType, this.m_textColor);
    Material pageMaterial = this.GetPageMaterial(inputFormatType, (Material) null);
    if (!((UnityEngine.Object) pageMaterial != (UnityEngine.Object) null))
      return;
    this.m_basePageRenderer.SetMaterial(pageMaterial);
  }

  public void SetPageTextColor()
  {
    if (!((UnityEngine.Object) this.m_pageNameText != (UnityEngine.Object) null))
      return;
    this.m_pageNameText.TextColor = this.GetTitleTextColor(CollectionManager.Get().GetThemeShowing(), this.m_textColor);
  }

  public void SetClass(CollectionTabInfo tabInfo)
  {
    if (tabInfo.tagClass == TAG_CLASS.INVALID)
    {
      this.SetPageNameText("");
      if (!((UnityEngine.Object) this.m_pageFlavorHeader != (UnityEngine.Object) null))
        return;
      this.m_pageFlavorHeader.SetActive(false);
    }
    else
    {
      this.SetPageNameText(GameStrings.GetClassName(tabInfo.tagClass));
      CollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, CollectionPageDisplay.TagClassToHeaderClass(tabInfo.tagClass));
    }
  }

  public void SetHeroPicker()
  {
    this.SetPageNameText(GameStrings.Get("GLUE_COLLECTION_MANAGER_HERO_SKINS_TITLE"));
    CollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, CollectionPageDisplay.HEADER_CLASS.HEROSKINS);
  }

  public void SetHeroSkins(TAG_CLASS? classTag)
  {
    if (!classTag.HasValue)
      this.SetPageNameText(GameStrings.Get("GLUE_COLLECTION_MANAGER_HERO_SKINS_TITLE"));
    else
      this.SetPageNameText(GameStrings.GetClassName(classTag.Value));
    CollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, CollectionPageDisplay.HEADER_CLASS.HEROSKINS);
  }

  public void SetCardBacks()
  {
    this.SetPageNameText(GameStrings.Get("GLUE_COLLECTION_MANAGER_CARD_BACKS_TITLE"));
    CollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, CollectionPageDisplay.HEADER_CLASS.CARDBACKS);
  }

  public void SetCoins()
  {
    this.SetPageNameText(GameStrings.Get("GLUE_COLLECTION_MANAGER_COIN_TITLE"));
    CollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, CollectionPageDisplay.HEADER_CLASS.COINS);
  }

  public void SetDeckTemplates()
  {
    this.SetPageNameText(string.Empty);
    if (!((UnityEngine.Object) this.m_pageFlavorHeader != (UnityEngine.Object) null))
      return;
    this.m_pageFlavorHeader.SetActive(false);
  }

  public void SetMassDisenchant()
  {
    this.SetPageNameText(string.Empty);
    if (!((UnityEngine.Object) this.m_pageFlavorHeader != (UnityEngine.Object) null))
      return;
    this.m_pageFlavorHeader.SetActive(false);
  }

  public TAG_CLASS? GetFirstCardClass()
  {
    if (this.m_collectionCardVisuals.Count == 0)
      return new TAG_CLASS?();
    CollectionCardVisual collectionCardVisual = this.m_collectionCardVisuals[0];
    if (!collectionCardVisual.IsShown())
      return new TAG_CLASS?();
    Actor actor = collectionCardVisual.GetActor();
    if (!actor.IsShown())
      return new TAG_CLASS?();
    return actor.GetEntityDef()?.GetClass();
  }

  private MassDisenchant GetMassDisenchantVisual()
  {
    if ((UnityEngine.Object) MassDisenchant.Get() == (UnityEngine.Object) null)
      return (MassDisenchant) null;
    this.m_massDisenchantVisual = MassDisenchant.Get();
    GameUtils.SetParent((Component) this.m_massDisenchantVisual, this.gameObject);
    return this.m_massDisenchantVisual;
  }

  private Material GetHeaderMaterial(FormatType formatType, Material defaultMaterial)
  {
    Material headerMaterial;
    if (!new Map<FormatType, Material>()
    {
      {
        FormatType.FT_STANDARD,
        this.m_headerMaterial
      },
      {
        FormatType.FT_WILD,
        this.m_wildHeaderMaterial
      },
      {
        FormatType.FT_CLASSIC,
        this.m_classicHeaderMaterial
      }
    }.TryGetValue(formatType, out headerMaterial))
      headerMaterial = defaultMaterial;
    return headerMaterial;
  }

  private Material GetPageMaterial(FormatType formatType, Material defaultMaterial)
  {
    Material pageMaterial;
    if (!new Map<FormatType, Material>()
    {
      {
        FormatType.FT_STANDARD,
        this.m_pageMaterial
      },
      {
        FormatType.FT_WILD,
        this.m_wildPageMaterial
      },
      {
        FormatType.FT_CLASSIC,
        this.m_classicPageMaterial
      }
    }.TryGetValue(formatType, out pageMaterial))
      pageMaterial = defaultMaterial;
    return pageMaterial;
  }

  private Color GetTextColor(FormatType formatType, Color defaultColor)
  {
    Color textColor;
    if (!new Map<FormatType, Color>()
    {
      {
        FormatType.FT_STANDARD,
        this.m_textColor
      },
      {
        FormatType.FT_WILD,
        this.m_wildTextColor
      },
      {
        FormatType.FT_CLASSIC,
        this.m_classicTextColor
      }
    }.TryGetValue(formatType, out textColor))
      textColor = defaultColor;
    return textColor;
  }

  private Color GetTitleTextColor(FormatType formatType, Color defaultColor)
  {
    Color titleTextColor;
    if (!new Map<FormatType, Color>()
    {
      {
        FormatType.FT_STANDARD,
        this.m_standardTitleTextColor
      },
      {
        FormatType.FT_WILD,
        this.m_wildTitleTextColor
      },
      {
        FormatType.FT_CLASSIC,
        this.m_classicTitleTextColor
      }
    }.TryGetValue(formatType, out titleTextColor))
      titleTextColor = defaultColor;
    return titleTextColor;
  }

  public static CollectionPageDisplay.HEADER_CLASS TagClassToHeaderClass(
    TAG_CLASS classTag)
  {
    string str = classTag.ToString();
    return Enum.IsDefined(typeof (CollectionPageDisplay.HEADER_CLASS), (object) str) ? (CollectionPageDisplay.HEADER_CLASS) Enum.Parse(typeof (CollectionPageDisplay.HEADER_CLASS), str) : CollectionPageDisplay.HEADER_CLASS.INVALID;
  }

  public static void SetPageFlavorTextures(
    GameObject header,
    CollectionPageDisplay.HEADER_CLASS headerClass)
  {
    if ((UnityEngine.Object) header == (UnityEngine.Object) null)
      return;
    int num;
    float x = (double) (num = (int) headerClass) < 8.0 ? 0.0f : 0.5f;
    float y = (float) (-(double) num / 8.0);
    CollectiblePageDisplay.SetPageFlavorTextures(header, new UnityEngine.Vector2(x, y));
  }

  public enum HEADER_CLASS
  {
    INVALID,
    SHAMAN,
    PALADIN,
    MAGE,
    DRUID,
    HUNTER,
    ROGUE,
    WARRIOR,
    PRIEST,
    WARLOCK,
    HEROSKINS,
    CARDBACKS,
    DEMONHUNTER,
    COINS,
    DEATHKNIGHT,
  }
}
