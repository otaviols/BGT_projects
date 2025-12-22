using Hearthstone.DungeonCrawl;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureDungeonCrawlRewardOption : MonoBehaviour
{
  public UberText m_optionName;
  public GameObject m_lootCrate;
  public AdventureDungeonCrawlDeckTray m_deckTray;
  public UIBButton m_chooseButton;
  public Transform m_bigCardBone;
  public float m_treasureOutroAnimDelay = 0.5f;
  [CustomEditField(Sections = "Animations")]
  public PlayMakerFSM m_lootCrateFSM;
  [CustomEditField(Sections = "Animations")]
  public string m_lootCrateDropAnimName;
  [CustomEditField(Sections = "Animations")]
  public string m_lootCrateSummonAnimName;
  [CustomEditField(Sections = "Animations")]
  public string m_lootCrateBurnAnimName;
  [CustomEditField(Sections = "Animations")]
  public string m_lootCrateAnimDoneStateName;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_treasureCardAppearsSFX;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_treasureCardSelectedSFX;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_treasureCardDissipateWhenNotSelectedSFX;
  [CustomEditField(Sections = "Particles")]
  public PlayNewParticles m_particleScript;
  [CustomEditField(Sections = "Shrines")]
  public GameObject ShrineClassBanner;
  [CustomEditField(Sections = "Shrines")]
  public UberText ShrineClassBannerText;
  [CustomEditField(Sections = "Shrines")]
  public float ShrineClassBannerScalePercent;
  [CustomEditField(Sections = "Shrines")]
  public Vector3 ShrineCardPositionOffset;
  public List<AdventureDungeonCrawlRewardOption.AdventureDungeonCrawlRewardOptionStyleOverride> m_rewardOptionStyle;
  protected IDungeonCrawlData m_dungeonRunData;
  public const float LEFT_MOST_BIG_CARD_X_POS = 0.27f;
  private AdventureDungeonCrawlRewardOption.OptionData m_optionData;
  private Actor m_bigCardActor;
  private const TAG_RARITY TREASURE_CARD_RARITY = TAG_RARITY.RARE;
  private AdventureDungeonCrawlRewardOption.OptionChosenCallback m_optionChosenCallback;
  private bool m_outroSpellIsPlaying;

  public void Initalize(IDungeonCrawlData data)
  {
    this.m_dungeonRunData = data;
    this.SetRewardOptionVisualStyle();
  }

  private void Start() => this.m_chooseButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OptionChosen()));

  public void SetRewardData(
    AdventureDungeonCrawlRewardOption.OptionData optionData)
  {
    this.m_optionData = optionData;
    this.EnableInteraction();
    if ((UnityEngine.Object) this.m_bigCardActor != (UnityEngine.Object) null)
    {
      this.m_bigCardActor.Destroy();
      this.m_bigCardActor = (Actor) null;
    }
    if (AdventureDungeonCrawlRewardOption.OptionTypeIsTreasure(optionData.optionType))
    {
      this.m_lootCrate.gameObject.SetActive(false);
      long treasureDatabaseId = (long) AdventureDungeonCrawlRewardOption.GetTreasureDatabaseID(optionData);
      if (treasureDatabaseId == 0L)
      {
        Log.Adventures.PrintWarning("Treasure choice has no dbId!");
      }
      else
      {
        string cardId = GameUtils.TranslateDbIdToCardId((int) treasureDatabaseId);
        if (cardId == null)
          Log.Adventures.PrintWarning("AdventureDungeonCrawlRewardOption.SetRewardData() - No cardId for dbId {0}!", (object) treasureDatabaseId);
        else
          DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnTreasureFullDefLoaded), (object) optionData);
      }
    }
    else
    {
      if (optionData.optionType != AdventureDungeonCrawlPlayMat.OptionType.LOOT)
        return;
      this.SetLootCrateContents(optionData);
    }
  }

  public Actor GetActorFromCardId(string cardId)
  {
    if ((UnityEngine.Object) this.m_deckTray != (UnityEngine.Object) null)
    {
      DeckTrayDeckTileVisual cardTileVisual = this.m_deckTray.GetCardsContent().GetCardTileVisual(cardId);
      if ((UnityEngine.Object) cardTileVisual != (UnityEngine.Object) null)
        return (Actor) cardTileVisual.GetActor();
    }
    return (UnityEngine.Object) this.m_bigCardActor != (UnityEngine.Object) null && this.m_bigCardActor.GetEntityDef() != null && this.m_bigCardActor.GetEntityDef().GetCardId() == cardId ? this.m_bigCardActor : (Actor) null;
  }

  public int GetTreasureDatabaseID() => AdventureDungeonCrawlRewardOption.GetTreasureDatabaseID(this.m_optionData);

  public static int GetTreasureDatabaseID(
    AdventureDungeonCrawlRewardOption.OptionData optionData)
  {
    return !AdventureDungeonCrawlRewardOption.OptionTypeIsTreasure(optionData.optionType) || optionData.options.Count < 1 ? 0 : (int) optionData.options[0];
  }

  private void OnTreasureFullDefLoaded(
    string cardID,
    DefLoader.DisposableFullDef def,
    object userData)
  {
    AdventureDungeonCrawlRewardOption.OnFullDefLoadedData callbackData = new AdventureDungeonCrawlRewardOption.OnFullDefLoadedData()
    {
      optionData = (AdventureDungeonCrawlRewardOption.OptionData) userData,
      fullDef = def
    };
    AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(def.EntityDef, TAG_PREMIUM.NORMAL), new PrefabCallback<GameObject>(this.OnTreasureActorLoaded), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnTreasureActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    AdventureDungeonCrawlRewardOption.OnFullDefLoadedData fullDefLoadedData = (AdventureDungeonCrawlRewardOption.OnFullDefLoadedData) callbackData;
    using (fullDefLoadedData.fullDef)
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AdventureDungeonCrawlRewardOption.OnActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      }
      else
      {
        Actor component = go.GetComponent<Actor>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) string.Format("AdventureDungeonCrawlRewardOption.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
        }
        else
        {
          component.SetPremium(TAG_PREMIUM.NORMAL);
          component.SetEntityDef(fullDefLoadedData.fullDef.EntityDef);
          component.SetCardDef(fullDefLoadedData.fullDef.DisposableCardDef);
          component.UpdateAllComponents();
          component.ContactShadow(true);
          component.transform.parent = this.m_bigCardBone;
          component.transform.localPosition = Vector3.zero;
          component.transform.localScale = Vector3.one;
          if (fullDefLoadedData.optionData.optionType == AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE)
          {
            this.ShrineClassBanner.SetActive(true);
            GameUtils.SetParent(this.ShrineClassBanner, component.GetCardTypeBannerAnchor(), true);
            this.ShrineClassBannerText.Text = GameStrings.GetClassName(fullDefLoadedData.fullDef.EntityDef.GetClass());
            component.transform.localScale = Vector3.one * this.ShrineClassBannerScalePercent;
            component.transform.localPosition += this.ShrineCardPositionOffset;
          }
          component.Hide();
          CardSelectionHandler selectionHandler = component.GetCollider().gameObject.AddComponent<CardSelectionHandler>();
          selectionHandler.SetActor(component);
          selectionHandler.SetChoiceNum(this.m_optionData.index + 1);
          selectionHandler.SetChosenCallback(new CardSelectionHandler.CardChosenCallback(this.OptionChosen));
          this.m_bigCardActor = component;
        }
      }
    }
  }

  public AdventureDungeonCrawlRewardOption.OptionData GetOptionData() => this.m_optionData;

  public void SetOptionChosenCallback(
    AdventureDungeonCrawlRewardOption.OptionChosenCallback callback)
  {
    this.m_optionChosenCallback = callback;
  }

  public bool IsInitialized() => !AdventureDungeonCrawlRewardOption.OptionTypeIsTreasure(this.m_optionData.optionType) || (UnityEngine.Object) this.m_bigCardActor != (UnityEngine.Object) null;

  public void PlayIntro()
  {
    if (AdventureDungeonCrawlRewardOption.OptionTypeIsTreasure(this.m_optionData.optionType))
    {
      if ((UnityEngine.Object) this.m_bigCardActor == (UnityEngine.Object) null)
      {
        Log.Adventures.PrintError("AdventureDungeonCrawlRewardOption.PlayIntro() - attempting to play intro for TREASURE, but m_bigCardActor is null!");
      }
      else
      {
        this.m_bigCardActor.Show();
        this.m_bigCardActor.ActivateSpellBirthState(SpellType.SUMMON_IN_FORGE);
        this.m_bigCardActor.ActivateSpellBirthState(DraftDisplay.GetSpellTypeForRarity(TAG_RARITY.RARE));
        if (string.IsNullOrEmpty(this.m_treasureCardAppearsSFX))
          return;
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_treasureCardAppearsSFX);
      }
    }
    else
    {
      if (this.m_optionData.optionType != AdventureDungeonCrawlPlayMat.OptionType.LOOT)
        return;
      this.m_lootCrateFSM.SendEvent(this.m_lootCrateDropAnimName);
    }
  }

  public bool IntroIsPlaying() => !AdventureDungeonCrawlRewardOption.OptionTypeIsTreasure(this.m_optionData.optionType) && this.m_optionData.optionType == AdventureDungeonCrawlPlayMat.OptionType.LOOT && this.m_lootCrateFSM.ActiveStateName != this.m_lootCrateAnimDoneStateName;

  private void EnableInteraction()
  {
    if ((UnityEngine.Object) this.m_bigCardActor != (UnityEngine.Object) null && (UnityEngine.Object) this.m_bigCardActor.GetCollider() != (UnityEngine.Object) null)
      this.m_bigCardActor.GetCollider().enabled = true;
    this.m_chooseButton.SetEnabled(true);
  }

  public void DisableInteraction()
  {
    if ((UnityEngine.Object) this.m_bigCardActor != (UnityEngine.Object) null && (UnityEngine.Object) this.m_bigCardActor.GetCollider() != (UnityEngine.Object) null)
      this.m_bigCardActor.GetCollider().enabled = false;
    this.m_chooseButton.SetEnabled(false);
  }

  public void PlayOutro(bool thisOptionSelected)
  {
    if (AdventureDungeonCrawlRewardOption.OptionTypeIsTreasure(this.m_optionData.optionType))
    {
      if ((UnityEngine.Object) this.m_bigCardActor == (UnityEngine.Object) null)
      {
        Log.Adventures.PrintWarning("AdventureDungeonCrawlRewardOption.PlayIntro() - attempting to play outro for TREASURE, but m_bigCardActor is null!");
      }
      else
      {
        this.m_outroSpellIsPlaying = true;
        Spell spell1 = this.m_bigCardActor.GetSpell(DraftDisplay.GetSpellTypeForRarity(TAG_RARITY.RARE));
        if (thisOptionSelected)
        {
          this.m_bigCardActor.GetSpell(SpellType.SUMMON_OUT_FORGE).AddFinishedCallback(new Spell.FinishedCallback(this.OutroSpellFinished), (object) this.m_bigCardActor);
          this.m_bigCardActor.ActivateSpellBirthState(SpellType.SUMMON_OUT_FORGE);
          if ((UnityEngine.Object) spell1 != (UnityEngine.Object) null)
            spell1.ActivateState(SpellStateType.DEATH);
          if (string.IsNullOrEmpty(this.m_treasureCardSelectedSFX))
            return;
          SoundManager.Get().LoadAndPlay((AssetReference) this.m_treasureCardSelectedSFX);
        }
        else
        {
          Spell spell2 = this.m_bigCardActor.GetSpell(SpellType.BURN);
          if ((UnityEngine.Object) spell2 != (UnityEngine.Object) null)
          {
            spell2.AddFinishedCallback(new Spell.FinishedCallback(this.OutroSpellFinished), (object) this.m_bigCardActor);
            this.m_bigCardActor.ActivateSpellBirthState(SpellType.BURN);
          }
          else
            this.OutroSpellFinished((Spell) null, (object) this.m_bigCardActor);
          if ((UnityEngine.Object) spell1 != (UnityEngine.Object) null)
            spell1.ActivateState(SpellStateType.DEATH);
          if (string.IsNullOrEmpty(this.m_treasureCardDissipateWhenNotSelectedSFX))
            return;
          SoundManager.Get().LoadAndPlay((AssetReference) this.m_treasureCardDissipateWhenNotSelectedSFX);
        }
      }
    }
    else
    {
      if (this.m_optionData.optionType != AdventureDungeonCrawlPlayMat.OptionType.LOOT)
        return;
      this.m_lootCrateFSM.SendEvent(thisOptionSelected ? this.m_lootCrateSummonAnimName : this.m_lootCrateBurnAnimName);
    }
  }

  private void OutroSpellFinished(Spell spell, object actorObject) => this.StartCoroutine(this.WaitForAnimToFinishThenDestroy(((Component) actorObject).gameObject));

  private IEnumerator WaitForAnimToFinishThenDestroy(GameObject gameObjectToDestroy)
  {
    yield return (object) new WaitForSeconds(this.m_treasureOutroAnimDelay);
    this.m_outroSpellIsPlaying = false;
    yield return (object) new WaitForSeconds(5f);
    UnityEngine.Object.Destroy((UnityEngine.Object) gameObjectToDestroy);
  }

  public bool OutroIsPlaying()
  {
    if (AdventureDungeonCrawlRewardOption.OptionTypeIsTreasure(this.m_optionData.optionType))
      return this.m_outroSpellIsPlaying;
    return this.m_optionData.optionType == AdventureDungeonCrawlPlayMat.OptionType.LOOT && this.m_lootCrateFSM.ActiveStateName != this.m_lootCrateAnimDoneStateName;
  }

  private void OptionChosen()
  {
    if (this.m_optionChosenCallback == null)
      return;
    this.m_optionChosenCallback();
  }

  private void SetRewardOptionVisualStyle()
  {
    DungeonRunVisualStyle visualStyle = this.m_dungeonRunData.VisualStyle;
    foreach (AdventureDungeonCrawlRewardOption.AdventureDungeonCrawlRewardOptionStyleOverride optionStyleOverride in this.m_rewardOptionStyle)
    {
      if (visualStyle == optionStyleOverride.VisualStyle)
      {
        if (!((UnityEngine.Object) this.m_particleScript != (UnityEngine.Object) null))
          break;
        this.m_particleScript.m_Target = optionStyleOverride.SlamDustEffect;
        break;
      }
    }
  }

  private static bool OptionTypeIsTreasure(AdventureDungeonCrawlPlayMat.OptionType optionType) => optionType == AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE || optionType == AdventureDungeonCrawlPlayMat.OptionType.TREASURE;

  private void SetLootCrateContents(
    AdventureDungeonCrawlRewardOption.OptionData optionData)
  {
    this.m_lootCrate.gameObject.SetActive(true);
    CollectionDeck deck = new CollectionDeck()
    {
      Type = DeckType.CLIENT_ONLY_DECK,
      FormatType = FormatType.FT_WILD,
      HeroCardID = "None"
    };
    for (int index = 0; index < optionData.options.Count; ++index)
    {
      long option = optionData.options[index];
      if (index == 0)
      {
        this.m_optionName.Text = option != 0L ? DefLoader.Get().GetEntityDef((int) option).GetName() : "";
      }
      else
      {
        string cardId = GameUtils.TranslateDbIdToCardId((int) option);
        if (string.IsNullOrEmpty(cardId))
          Log.Adventures.PrintWarning("AdventureDungeonCrawlRewardOption.SetRewardData() - No cardId for dbId {0}!", (object) option);
        else
          deck.InsertSlotWithCard(cardId, TAG_PREMIUM.NORMAL, false, 1);
      }
    }
    this.m_deckTray.SetDungeonCrawlDeck(deck, false);
  }

  [Serializable]
  public class AdventureDungeonCrawlRewardOptionStyleOverride
  {
    public DungeonRunVisualStyle VisualStyle;
    public GameObject SlamDustEffect;
  }

  public delegate void OptionChosenCallback();

  public struct OptionData
  {
    public readonly AdventureDungeonCrawlPlayMat.OptionType optionType;
    public readonly List<long> options;
    public readonly int index;

    public OptionData(
      AdventureDungeonCrawlPlayMat.OptionType optionType,
      List<long> options,
      int index)
    {
      this.optionType = optionType;
      this.options = new List<long>((IEnumerable<long>) options);
      this.index = index;
    }
  }

  private struct OnFullDefLoadedData
  {
    public AdventureDungeonCrawlRewardOption.OptionData optionData;
    public DefLoader.DisposableFullDef fullDef;
  }
}
