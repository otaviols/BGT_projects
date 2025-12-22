using Blizzard.T5.Services;
using HutongGames.PlayMaker;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStoreHeroesPane : GeneralStorePane
{
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_heroUnpurchasedFrame;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_heroPurchasedFrame;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_heroAnimationFrame;
  [SerializeField]
  private Vector3 m_unpurchasedHeroButtonSpacing = new Vector3(0.0f, 0.0f, 0.285f);
  [SerializeField]
  private Vector3 m_purchasedHeroButtonSpacing = new Vector3(0.0f, 0.0f, 0.092f);
  [CustomEditField(Sections = "Layout")]
  public float m_unpurchasedHeroButtonHeight = 0.0275f;
  [CustomEditField(Sections = "Layout")]
  public float m_purchasedHeroButtonHeight;
  [CustomEditField(Sections = "Layout")]
  public float m_purchasedHeroButtonHeightPadding = 0.01f;
  [CustomEditField(Sections = "Layout")]
  public float m_maxPurchasedHeightAdd;
  [CustomEditField(Sections = "Layout/Purchased Section")]
  public GameObject m_purchasedSectionTop;
  [CustomEditField(Sections = "Layout/Purchased Section")]
  public GameObject m_purchasedSectionBottom;
  [CustomEditField(Sections = "Layout/Purchased Section")]
  public GameObject m_purchasedSectionMidTemplate;
  [CustomEditField(Sections = "Layout/Purchased Section")]
  public MultiSliceElement m_purchasedSection;
  [CustomEditField(Sections = "Layout/Purchased Section")]
  public GameObject m_purchasedButtonContainer;
  [CustomEditField(Sections = "Layout/Purchased Section")]
  public Vector3 m_purchasedSectionOffset = new Vector3(0.0f, 0.0f, 0.145f);
  [CustomEditField(Sections = "Scroll")]
  public UIBScrollable m_scrollUpdate;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_heroSelectionSound;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_buttonsSlideUpSound;
  [CustomEditField(Sections = "Purchase Flow")]
  public GameObject m_purchaseAnimationBlocker;
  [CustomEditField(Sections = "Animations")]
  public GameObject m_purchaseAnimationEndBone;
  [CustomEditField(Sections = "Animations")]
  public Vector3 m_purchaseAnimationMidPointWorldOffset = new Vector3(0.0f, 0.0f, -7.5f);
  [CustomEditField(Sections = "Animations")]
  public string m_purchaseAnimationName = "HeroSkin_HeroHolderPopOut";
  private List<GeneralStoreHeroesSelectorButton> m_unpurchasedHeroesButtons = new List<GeneralStoreHeroesSelectorButton>();
  private List<GeneralStoreHeroesSelectorButton> m_purchasedHeroesButtons = new List<GeneralStoreHeroesSelectorButton>();
  private GeneralStoreHeroesContent m_heroesContent;
  private bool m_initializeFirstHero;
  private List<GameObject> m_purchasedSectionMidMeshes = new List<GameObject>();
  private int m_currentPurchaseRemovalIdx;

  [CustomEditField(Sections = "Layout")]
  public Vector3 UnpurchasedHeroButtonSpacing
  {
    get => this.m_unpurchasedHeroButtonSpacing;
    set
    {
      this.m_unpurchasedHeroButtonSpacing = value;
      this.PositionAllHeroButtons();
    }
  }

  [CustomEditField(Sections = "Layout")]
  public Vector3 PurchasedHeroButtonSpacing
  {
    get => this.m_purchasedHeroButtonSpacing;
    set
    {
      this.m_purchasedHeroButtonSpacing = value;
      this.PositionAllHeroButtons();
    }
  }

  private void Awake()
  {
    this.m_heroesContent = this.m_parentContent as GeneralStoreHeroesContent;
    this.PopulateHeroes();
    this.m_purchaseAnimationBlocker.SetActive(false);
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnItemPurchased));
    CheatMgr.Get().RegisterCheatHandler("herobuy", new CheatMgr.ProcessCheatCallback(this.OnHeroPurchased_cheat));
  }

  private void OnDestroy()
  {
    CheatMgr service;
    if (ServiceManager.TryGet<CheatMgr>(out service))
      service.UnregisterCheatHandler("herobuy", new CheatMgr.ProcessCheatCallback(this.OnHeroPurchased_cheat));
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnItemPurchased));
  }

  public override void PrePaneSwappedIn() => this.SetupInitialSelectedHero();

  public void RefreshHeroAvailability()
  {
  }

  private void PopulateHeroes()
  {
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    foreach (CardHeroDbfRecord record in GameDbf.CardHero.GetRecords())
    {
      Network.Bundle heroBundle = (Network.Bundle) null;
      if (StoreManager.Get().GetHeroBundleByCardDbId(record.CardId, out heroBundle) && specialEventManager.IsEventActive(heroBundle.ProductEvent, false))
        this.CreateNewHeroButton(record, heroBundle).SetSortOrder(record.StoreSortOrder);
    }
    this.PositionAllHeroButtons();
  }

  private GeneralStoreHeroesSelectorButton CreateNewHeroButton(
    CardHeroDbfRecord cardHero,
    Network.Bundle heroBundle)
  {
    return StoreManager.Get().IsProductAlreadyOwned(heroBundle) ? this.CreatePurchasedHeroButton(cardHero, heroBundle) : this.CreateUnpurchasedHeroButton(cardHero, heroBundle);
  }

  private GeneralStoreHeroesSelectorButton CreateUnpurchasedHeroButton(
    CardHeroDbfRecord cardHero,
    Network.Bundle heroBundle)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_heroUnpurchasedFrame);
    GeneralStoreHeroesSelectorButton component = gameObject.GetComponent<GeneralStoreHeroesSelectorButton>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Prefab does not contain GeneralStoreHeroesSelectorButton component.");
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
      return (GeneralStoreHeroesSelectorButton) null;
    }
    GameUtils.SetParent((Component) component, this.m_paneContainer, true);
    LayerUtils.SetLayer((Component) component, this.m_paneContainer.layer);
    this.m_unpurchasedHeroesButtons.Add(component);
    this.SetupHeroButton(cardHero, component);
    return component;
  }

  public GeneralStoreHeroesSelectorButton CreatePurchasedHeroButton(
    CardHeroDbfRecord cardHero,
    Network.Bundle heroBundle)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_heroPurchasedFrame);
    GeneralStoreHeroesSelectorButton component = gameObject.GetComponent<GeneralStoreHeroesSelectorButton>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Prefab does not contain GeneralStoreHeroesSelectorButton component.");
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
      return (GeneralStoreHeroesSelectorButton) null;
    }
    GameUtils.SetParent((Component) component, this.m_purchasedButtonContainer, true);
    LayerUtils.SetLayer((Component) component, this.m_purchasedButtonContainer.layer);
    this.m_purchasedHeroesButtons.Add(component);
    this.SetupHeroButton(cardHero, component);
    return component;
  }

  private void SetupHeroButton(
    CardHeroDbfRecord cardHero,
    GeneralStoreHeroesSelectorButton heroButton)
  {
    string cardId1 = GameUtils.TranslateDbIdToCardId(cardHero.CardId);
    heroButton.SetCardHeroDbfRecord(cardHero);
    heroButton.SetPurchased(false);
    heroButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.SelectHero(heroButton)));
    DefLoader.Get().LoadFullDef(cardId1, (DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>) ((cardId, fullDef, data) =>
    {
      using (fullDef)
      {
        heroButton.UpdatePortrait(fullDef);
        heroButton.UpdateName(fullDef.EntityDef.GetName());
      }
    }));
  }

  private void UpdatePurchasedSectionLayout()
  {
    if (this.m_purchasedHeroesButtons.Count == 0)
    {
      this.m_purchasedButtonContainer.SetActive(false);
      this.m_purchasedSection.gameObject.SetActive(false);
    }
    else
    {
      this.m_purchasedButtonContainer.SetActive(true);
      this.m_purchasedSection.gameObject.SetActive(true);
      if (this.m_purchasedSectionMidMeshes.Count < this.m_purchasedHeroesButtons.Count)
      {
        int num = this.m_purchasedHeroesButtons.Count - this.m_purchasedSectionMidMeshes.Count;
        for (int index = 0; index < num; ++index)
        {
          GameObject gameObject = (GameObject) GameUtils.Instantiate(this.m_purchasedSectionMidTemplate, this.m_purchasedSection.gameObject, true);
          gameObject.SetActive(true);
          this.m_purchasedSectionMidMeshes.Add(gameObject);
        }
      }
      this.m_purchasedSection.ClearSlices();
      this.m_purchasedSection.AddSlice(this.m_purchasedSectionTop);
      foreach (GameObject purchasedSectionMidMesh in this.m_purchasedSectionMidMeshes)
        this.m_purchasedSection.AddSlice(purchasedSectionMidMesh);
      this.m_purchasedSection.AddSlice(this.m_purchasedSectionBottom);
      this.m_purchasedSection.UpdateSlices();
    }
  }

  private void SelectHero(GeneralStoreHeroesSelectorButton button)
  {
    foreach (GeneralStoreHeroesSelectorButton unpurchasedHeroesButton in this.m_unpurchasedHeroesButtons)
      unpurchasedHeroesButton.Unselect();
    foreach (GeneralStoreHeroesSelectorButton purchasedHeroesButton in this.m_purchasedHeroesButtons)
      purchasedHeroesButton.Unselect();
    button.Select();
    Options.Get().SetInt(Option.LAST_SELECTED_STORE_HERO_ID, button.GetHeroDbId());
    this.m_heroesContent.SelectHero(button.GetCardHeroDbfRecord());
    if (string.IsNullOrEmpty(this.m_heroSelectionSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_heroSelectionSound);
  }

  private void SetupInitialSelectedHero()
  {
    if (this.m_initializeFirstHero)
      return;
    this.m_initializeFirstHero = true;
    int num = Options.Get().GetInt(Option.LAST_SELECTED_STORE_HERO_ID, -1);
    if (num == -1)
      return;
    List<GeneralStoreHeroesSelectorButton> heroesSelectorButtonList = new List<GeneralStoreHeroesSelectorButton>();
    heroesSelectorButtonList.AddRange((IEnumerable<GeneralStoreHeroesSelectorButton>) this.m_unpurchasedHeroesButtons);
    heroesSelectorButtonList.AddRange((IEnumerable<GeneralStoreHeroesSelectorButton>) this.m_purchasedHeroesButtons);
    foreach (GeneralStoreHeroesSelectorButton heroesSelectorButton in heroesSelectorButtonList)
    {
      if (heroesSelectorButton.GetHeroDbId() == num)
      {
        this.m_heroesContent.SelectHero(heroesSelectorButton.GetCardHeroDbfRecord(), false);
        heroesSelectorButton.Select();
        break;
      }
    }
  }

  private void PositionAllHeroButtons()
  {
    this.PositionUnpurchasedHeroButtons();
    this.PositionPurchasedHeroButtons();
  }

  private void PositionUnpurchasedHeroButtons()
  {
    this.m_unpurchasedHeroesButtons.Sort((Comparison<GeneralStoreHeroesSelectorButton>) ((lhs, rhs) =>
    {
      int sortOrder1 = lhs.GetSortOrder();
      int sortOrder2 = rhs.GetSortOrder();
      if (sortOrder1 < sortOrder2)
        return -1;
      return sortOrder1 > sortOrder2 ? 1 : 0;
    }));
    for (int index = 0; index < this.m_unpurchasedHeroesButtons.Count; ++index)
      this.m_unpurchasedHeroesButtons[index].transform.localPosition = this.m_unpurchasedHeroButtonSpacing * (float) index;
  }

  private void PositionPurchasedHeroButtons(bool sortAndSetSectionPos = true)
  {
    if (sortAndSetSectionPos)
    {
      this.m_purchasedHeroesButtons.Sort((Comparison<GeneralStoreHeroesSelectorButton>) ((lhs, rhs) =>
      {
        int sortOrder1 = lhs.GetSortOrder();
        int sortOrder2 = rhs.GetSortOrder();
        if (sortOrder1 < sortOrder2)
          return -1;
        return sortOrder1 > sortOrder2 ? 1 : 0;
      }));
      this.m_purchasedSection.transform.localPosition = this.m_unpurchasedHeroButtonSpacing * (float) (this.m_unpurchasedHeroesButtons.Count - 1) + this.m_purchasedSectionOffset;
    }
    for (int index = 0; index < this.m_purchasedHeroesButtons.Count; ++index)
      this.m_purchasedHeroesButtons[index].transform.localPosition = this.m_purchasedHeroButtonSpacing * (float) index;
    this.UpdatePurchasedSectionLayout();
  }

  private IEnumerator AnimateShowPurchase(int btnIndex)
  {
    GeneralStoreHeroesPane generalStoreHeroesPane = this;
    generalStoreHeroesPane.m_purchaseAnimationBlocker.SetActive(true);
    generalStoreHeroesPane.m_scrollUpdate.Pause(true);
    if (GeneralStore.Get().GetMode() != GeneralStoreMode.HEROES)
    {
      GeneralStore.Get().SetMode(GeneralStoreMode.HEROES);
      yield return (object) new WaitForSeconds(1f);
    }
    GeneralStoreHeroesSelectorButton removeBtn = generalStoreHeroesPane.m_unpurchasedHeroesButtons[btnIndex];
    float percentage = (float) btnIndex / (float) (generalStoreHeroesPane.m_unpurchasedHeroesButtons.Count + generalStoreHeroesPane.m_purchasedHeroesButtons.Count - 1);
    generalStoreHeroesPane.m_scrollUpdate.SetScroll(percentage, iTween.EaseType.easeInOutCirc, 0.2f);
    yield return (object) new WaitForSeconds(0.21f);
    GameObject animateBtnObj = AssetLoader.Get().InstantiatePrefab((AssetReference) generalStoreHeroesPane.m_heroAnimationFrame);
    GeneralStoreHeroesSelectorButton component = animateBtnObj.GetComponent<GeneralStoreHeroesSelectorButton>();
    LayerUtils.SetLayer((Component) component, GameLayer.PerspectiveUI);
    component.transform.position = removeBtn.transform.position;
    component.UpdatePortrait(removeBtn);
    component.UpdateName(removeBtn);
    removeBtn.gameObject.SetActive(false);
    PlayMakerFSM animation = component.GetComponent<PlayMakerFSM>();
    FsmVector3 fsmVector3_1 = animation.FsmVariables.FindFsmVector3("PopStartPos");
    FsmVector3 fsmVector3_2 = animation.FsmVariables.FindFsmVector3("PopMidPos");
    FsmVector3 fsmVector3_3 = animation.FsmVariables.FindFsmVector3("PopEndPos");
    fsmVector3_1.Value = removeBtn.transform.position;
    Vector3 vector3 = removeBtn.transform.position + generalStoreHeroesPane.m_purchaseAnimationMidPointWorldOffset;
    fsmVector3_2.Value = vector3;
    fsmVector3_3.Value = generalStoreHeroesPane.m_purchaseAnimationEndBone.transform.position;
    Camera firstByLayer = CameraUtils.FindFirstByLayer(generalStoreHeroesPane.gameObject.layer);
    if ((UnityEngine.Object) firstByLayer != (UnityEngine.Object) null)
      animation.FsmVariables.FindFsmGameObject("CameraObjectShake").Value = firstByLayer.gameObject;
    animation.FsmVariables.FindFsmString("PopOutAnimationName").Value = generalStoreHeroesPane.m_purchaseAnimationName;
    animation.SendEvent("PopOut");
    yield return (object) new WaitForSeconds(0.5f);
    generalStoreHeroesPane.m_heroesContent.PlayCurrentHeroPurchaseEmote();
    yield return (object) null;
    FsmBool animComplete = animation.FsmVariables.FindFsmBool("AnimationComplete");
    while (!animComplete.Value)
      yield return (object) null;
    generalStoreHeroesPane.CreatePurchasedHeroButton(removeBtn.GetCardHeroDbfRecord(), (Network.Bundle) null).Select();
    generalStoreHeroesPane.m_unpurchasedHeroesButtons.Remove(removeBtn);
    generalStoreHeroesPane.PositionPurchasedHeroButtons(false);
    yield return (object) new WaitForSeconds(0.25f);
    while (!InputCollection.GetMouseButtonDown(0))
      yield return (object) null;
    animation.SendEvent("EchoHero");
    yield return (object) null;
    animComplete = animation.FsmVariables.FindFsmBool("AnimationComplete");
    while (!animComplete.Value)
      yield return (object) null;
    for (int purchaseRemovalIdx = generalStoreHeroesPane.m_currentPurchaseRemovalIdx; purchaseRemovalIdx < generalStoreHeroesPane.m_unpurchasedHeroesButtons.Count; ++purchaseRemovalIdx)
      iTween.MoveTo(generalStoreHeroesPane.m_unpurchasedHeroesButtons[purchaseRemovalIdx].gameObject, iTween.Hash((object) "position", (object) (generalStoreHeroesPane.m_unpurchasedHeroButtonSpacing * (float) purchaseRemovalIdx), (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutCirc, (object) "time", (object) 0.25f));
    iTween.MoveTo(generalStoreHeroesPane.m_purchasedSection.gameObject, iTween.Hash((object) "position", (object) (generalStoreHeroesPane.m_unpurchasedHeroButtonSpacing * (float) (generalStoreHeroesPane.m_unpurchasedHeroesButtons.Count - 1) + generalStoreHeroesPane.m_purchasedSectionOffset), (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutCirc, (object) "time", (object) 0.25f));
    if (!string.IsNullOrEmpty(generalStoreHeroesPane.m_buttonsSlideUpSound))
      SoundManager.Get().LoadAndPlay((AssetReference) generalStoreHeroesPane.m_buttonsSlideUpSound);
    yield return (object) new WaitForSeconds(0.25f);
    UnityEngine.Object.Destroy((UnityEngine.Object) removeBtn.gameObject);
    UnityEngine.Object.Destroy((UnityEngine.Object) animateBtnObj);
    animateBtnObj = (GameObject) null;
    generalStoreHeroesPane.m_scrollUpdate.Pause(false);
    generalStoreHeroesPane.m_purchaseAnimationBlocker.SetActive(false);
  }

  private void OnItemPurchased(Network.Bundle bundle, PaymentMethod purchaseMethod)
  {
    if ((Record) bundle == (Record) null || bundle.Items == null)
      return;
    int heroCardDbId = 0;
    foreach (Network.BundleItem bundleItem in bundle.Items)
    {
      if (bundleItem.ItemType == ProductType.PRODUCT_TYPE_HIDDEN_LICENSE)
        return;
      if ((Record) bundleItem != (Record) null && bundleItem.ItemType == ProductType.PRODUCT_TYPE_HERO)
        heroCardDbId = bundleItem.ProductData;
    }
    if (heroCardDbId == 0)
      return;
    this.OnHeroPurchased(heroCardDbId);
  }

  private void OnHeroPurchased(int heroCardDbId)
  {
    int index = this.m_unpurchasedHeroesButtons.FindIndex((Predicate<GeneralStoreHeroesSelectorButton>) (e => e.GetHeroCardDbId() == heroCardDbId));
    if (index == -1)
      Debug.LogError((object) string.Format("Hero Card DB ID {0} does not exist in button list.", (object) heroCardDbId));
    else
      this.RunHeroPurchaseAnimation(index);
  }

  private void RunHeroPurchaseAnimation(int btnIndex)
  {
    this.m_currentPurchaseRemovalIdx = btnIndex;
    this.StartCoroutine(this.AnimateShowPurchase(btnIndex));
  }

  private bool OnHeroPurchased_cheat(string func, string[] args, string rawArgs)
  {
    if (args.Length == 0)
      return true;
    int result = -1;
    if (int.TryParse(args[0], out result) && result >= 0 && result < this.m_unpurchasedHeroesButtons.Count)
      this.RunHeroPurchaseAnimation(result);
    return true;
  }
}
