using Hearthstone.DataModels;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeBundleProductPage : ProductPage, IPopupRendering
{
  public GameObject BackgroundSingle;
  public GameObject BackgroundFirst;
  public GameObject BackgroundMiddle;
  public GameObject BackgroundLast;
  public GameObject Divider;
  public GameObject Nameplate;
  public GameObject TraySlider;
  public PlayMakerFSM TurnPagePlayMakerFSM;
  public WidgetTemplate ProductPageWidget;
  public WidgetInstance DetailsFrameWidget;
  public float ItemSpacingX;
  public int ItemsPerPage;
  public float TrayHeight;
  public float TrayOffsetX;
  public float SeamPadding;
  public float FullTrayWidth;
  public Vector3 HeroSkinBasePosition;
  public Vector3 HeroSkinScale;
  public Vector3 BoardSkinBasePosition;
  public Vector3 BoardSkinScale;
  public Vector3 FinisherBasePosition;
  public Vector3 FinisherScale;
  [Tooltip("Number of seconds to wait between animating each emote in sequence (e.g. animate emote 1, pause, animate emote 2, pause...). If only 1 emote is shown, this wait is ignored")]
  [SerializeField]
  [Min(0.0f)]
  private float m_pauseBetweenEmoteAnimations;
  [Min(0.0f)]
  [Tooltip("Number of seconds to wait after animating each emote in sequence (e.g. if 3 emotes are shown, this pause happens after emote 3 finishes animating). If only 1 emote is shown, this wait happens after a single loop.")]
  [SerializeField]
  private float m_pauseBetweenEmoteCycles;
  [Tooltip("Number of seconds to wait after the page is loaded and before playing an emote")]
  [Min(0.0f)]
  [SerializeField]
  private float m_emoteEntranceDelaySeconds;
  [SerializeField]
  [Tooltip("If true, emote will pause on the first frame before starting. If false, emote will pause on the animation's configured display frame before starting")]
  private bool m_shouldStartEmoteOnFirstFrame;
  public Vector3 EmoteBasePosition;
  public Vector3 EmoteScale;
  public Vector3 SummaryBasePosition;
  public Vector3 SummaryScale;
  public Vector3 DividerBasePosition;
  public Vector3 DividerScale;
  public Vector3 NameplateBasePosition;
  public Vector3 NameplateScale;
  private static readonly Quaternion s_backgroundRotation = Quaternion.Euler(90f, 0.0f, 0.0f);
  private static readonly AssetReference s_rewardItemDisplay = new AssetReference("RewardItemDisplay.prefab:1462b7f022881004c888368b9badc81e");
  private static readonly AssetReference s_summaryFiligree = new AssetReference("BaconStoreLargeBundleTitleFiligree.prefab:802bb3d63f19d064591b07dbfb6b7a3e");
  private static readonly AssetReference s_nameplate = new AssetReference("BaconStoreLargeBundleTextBracket.prefab:c441b65d36fb9454493272658ce3192c");
  private static Widget.TriggerEventParameters s_animationStartedEventParameters = new Widget.TriggerEventParameters()
  {
    NoDownwardPropagation = true
  };
  private bool m_sliderIsAnimating;
  private bool m_pageInfoDataModelBound;
  private PageInfoDataModel m_pageInfoDataModel = new PageInfoDataModel();
  private WidgetInstance m_summary;
  private ShopLargeBundleDetailsDataModel m_summaryDataModel = new ShopLargeBundleDetailsDataModel();
  private GameObject m_singleTray;
  private GameObject m_firstTray;
  private GameObject m_lastTray;
  private List<GameObject> m_middleTrays = new List<GameObject>();
  private List<GameObject> m_trays = new List<GameObject>();
  private List<GameObject> m_dividers = new List<GameObject>();
  private List<WidgetInstance> m_rewardItems = new List<WidgetInstance>();
  private readonly List<(Animator animator, float normalizedDisplayTime)> m_visibleEmotesWithDisplayTimes = new List<(Animator, float)>();
  private int m_animatedEmoteIndex;
  private Coroutine m_emoteAnimationCoroutine;
  private List<WidgetInstance> m_nameplates = new List<WidgetInstance>();
  private IPopupRoot m_popupRoot;
  private HashSet<IPopupRendering> m_popupRenderingComponents;

  public float SmallTrayWidth => (float) this.ItemsPerPage * this.ItemSpacingX;

  private Vector3 BackgroundBasePosition => new Vector3(this.TrayOffsetX, -0.1f, 0.6f);

  private Vector3 FullTrayScale => new Vector3(this.FullTrayWidth + this.SeamPadding, this.TrayHeight, 1f);

  private Vector3 FullTrayScale_noPadding => new Vector3(this.FullTrayWidth, this.TrayHeight, 1f);

  private void PaginationEventListener(string eventName)
  {
    if (!(eventName == "PageLeft_code"))
    {
      if (!(eventName == "PageRight_code"))
      {
        if (!(eventName == "AnimationFinished_code"))
          return;
        this.m_sliderIsAnimating = false;
      }
      else
      {
        if (this.m_pageInfoDataModel.PageNumber >= this.m_pageInfoDataModel.TotalPages || this.m_sliderIsAnimating)
          return;
        this.m_sliderIsAnimating = true;
        this.UpdatePageItemDataModelAndButtonsEnabled(this.m_pageInfoDataModel.PageNumber + 1, this.m_pageInfoDataModel.TotalPages);
        this.TurnPagePlayMakerFSM.SendEvent("PageRight");
        this.m_widget.TriggerEvent("AnimationStarted_code", LargeBundleProductPage.s_animationStartedEventParameters);
      }
    }
    else
    {
      if (this.m_pageInfoDataModel.PageNumber <= 1 || this.m_sliderIsAnimating)
        return;
      this.m_sliderIsAnimating = true;
      this.UpdatePageItemDataModelAndButtonsEnabled(this.m_pageInfoDataModel.PageNumber - 1, this.m_pageInfoDataModel.TotalPages);
      this.TurnPagePlayMakerFSM.SendEvent("PageLeft");
      this.m_widget.TriggerEvent("AnimationStarted_code", LargeBundleProductPage.s_animationStartedEventParameters);
    }
  }

  protected override void Start()
  {
    base.Start();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.PaginationEventListener));
  }

  private void UpdatePageItemDataModelAndButtonsEnabled(int pageNumber, int totalPages)
  {
    if (pageNumber == 1)
    {
      this.ProductPageWidget.TriggerEvent("ENABLE_BUTTON_LEFT", new Widget.TriggerEventParameters());
      this.ProductPageWidget.TriggerEvent("DISABLE_BUTTON_LEFT", new Widget.TriggerEventParameters());
    }
    else
      this.ProductPageWidget.TriggerEvent("ENABLE_BUTTON_LEFT", new Widget.TriggerEventParameters());
    if (pageNumber == totalPages)
      this.ProductPageWidget.TriggerEvent("DISABLE_BUTTON_RIGHT", new Widget.TriggerEventParameters());
    else
      this.ProductPageWidget.TriggerEvent("ENABLE_BUTTON_RIGHT", new Widget.TriggerEventParameters());
    this.m_pageInfoDataModel.PageNumber = pageNumber;
    this.m_pageInfoDataModel.TotalPages = totalPages;
    this.m_pageInfoDataModel.InfoText = GameStrings.Format("GLUE_PROGRESSION_REWARD_TRACK_PAGE_NUMBER", (object) pageNumber, (object) totalPages);
    this.StopEmoteAnimation();
    this.UpdateVisibleAnimatedEmotes((object) null);
  }

  protected override void OnProductSet()
  {
    base.OnProductSet();
    if (this.Product.RewardList == null)
    {
      Log.Store.PrintWarning("LargeBundleProductPage Product has no RewardList. Cannot create reward items.");
    }
    else
    {
      this.m_pageInfoDataModel.ItemsPerPage = this.ItemsPerPage;
      int num = this.Product.RewardList.Items.Count + 1;
      foreach (RewardItemDataModel rewardItemDataModel in this.Product.RewardList.Items)
      {
        if (rewardItemDataModel.ItemType == RewardItemType.BATTLEGROUNDS_EMOTE_PILE)
          num += rewardItemDataModel.BGEmotePile.Count - 1;
      }
      this.UpdatePageItemDataModelAndButtonsEnabled(1, num / this.ItemsPerPage + (num % this.ItemsPerPage > 0 ? 1 : 0));
      this.TraySlider.transform.localPosition = new Vector3(0.0f, this.TraySlider.transform.localPosition.y, this.TraySlider.transform.localPosition.z);
      if (!this.m_pageInfoDataModelBound)
      {
        this.m_pageInfoDataModelBound = true;
        this.DetailsFrameWidget.BindDataModel((IDataModel) this.m_pageInfoDataModel, false);
      }
      this.SetTrays();
      this.SetSummary();
      this.SetItems();
    }
  }

  private void SetTrays()
  {
    float halfTrayWidthDifference = (float) (((double) this.FullTrayWidth - (double) this.SmallTrayWidth) / 2.0);
    if (this.m_pageInfoDataModel.TotalPages == 1)
      this.SetSingleTray(halfTrayWidthDifference);
    else
      this.SetMultipleTrays(halfTrayWidthDifference);
  }

  private void SetSingleTray(float halfTrayWidthDifference)
  {
    if ((UnityEngine.Object) this.m_firstTray != (UnityEngine.Object) null)
      this.m_firstTray.SetActive(false);
    foreach (GameObject middleTray in this.m_middleTrays)
      middleTray.SetActive(false);
    if ((UnityEngine.Object) this.m_lastTray != (UnityEngine.Object) null)
      this.m_lastTray.SetActive(false);
    if ((UnityEngine.Object) this.m_singleTray == (UnityEngine.Object) null)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.BackgroundSingle, this.TraySlider.transform);
      gameObject.transform.localPosition = this.BackgroundBasePosition + new Vector3(halfTrayWidthDifference, 0.0f, 0.0f);
      gameObject.transform.localRotation = LargeBundleProductPage.s_backgroundRotation;
      gameObject.transform.localScale = this.FullTrayScale_noPadding;
    }
    else
      this.m_singleTray.SetActive(true);
  }

  private void SetMultipleTrays(float halfTrayWidthDifference)
  {
    Vector3 vector3 = new Vector3(this.SmallTrayWidth + this.SeamPadding, this.TrayHeight, 1f);
    if ((UnityEngine.Object) this.m_singleTray != (UnityEngine.Object) null)
      this.m_singleTray.SetActive(false);
    if ((UnityEngine.Object) this.m_firstTray == (UnityEngine.Object) null)
    {
      this.m_firstTray = UnityEngine.Object.Instantiate<GameObject>(this.BackgroundFirst, this.TraySlider.transform);
      this.m_firstTray.transform.localPosition = this.BackgroundBasePosition;
      this.m_firstTray.transform.localRotation = LargeBundleProductPage.s_backgroundRotation;
      this.m_firstTray.transform.localScale = vector3;
    }
    else
      this.m_firstTray.SetActive(true);
    int index1 = 0;
    for (int index2 = this.m_pageInfoDataModel.TotalPages - 2; index1 < index2; ++index1)
    {
      if (this.m_middleTrays.Count > index1)
      {
        this.m_middleTrays[index1].SetActive(true);
      }
      else
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.BackgroundMiddle, this.TraySlider.transform);
        float x = this.SmallTrayWidth * (float) (index1 + 1);
        gameObject.transform.localPosition = this.BackgroundBasePosition + new Vector3(x, 0.0f, 0.0f);
        gameObject.transform.localRotation = LargeBundleProductPage.s_backgroundRotation;
        gameObject.transform.localScale = vector3;
        this.m_middleTrays.Add(gameObject);
      }
    }
    if ((UnityEngine.Object) this.m_lastTray == (UnityEngine.Object) null)
    {
      this.m_lastTray = UnityEngine.Object.Instantiate<GameObject>(this.BackgroundLast, this.TraySlider.transform);
      this.m_lastTray.transform.localRotation = LargeBundleProductPage.s_backgroundRotation;
      this.m_lastTray.transform.localScale = this.FullTrayScale;
    }
    else
      this.m_lastTray.SetActive(true);
    this.m_lastTray.transform.localPosition = this.BackgroundBasePosition + new Vector3(this.SmallTrayWidth * (float) (this.m_pageInfoDataModel.TotalPages - 1) + halfTrayWidthDifference, 0.0f, 0.0f);
    for (int index3 = this.m_pageInfoDataModel.TotalPages - 2; index3 < this.m_middleTrays.Count; ++index3)
      this.m_middleTrays[index3].SetActive(false);
  }

  private void SetSummary()
  {
    this.m_summaryDataModel.Name = this.Product.Name;
    if (!((UnityEngine.Object) this.m_summary == (UnityEngine.Object) null))
      return;
    this.m_summary = WidgetInstance.Create((string) LargeBundleProductPage.s_summaryFiligree);
    if ((UnityEngine.Object) this.m_summary == (UnityEngine.Object) null)
    {
      Log.Store.PrintError(string.Format("{0} cannot create an instance of {1}. Cannot create summary.", (object) nameof (LargeBundleProductPage), (object) LargeBundleProductPage.s_summaryFiligree));
    }
    else
    {
      this.m_summary.transform.SetParent(this.TraySlider.transform);
      this.m_summary.transform.localPosition = this.SummaryBasePosition;
      this.m_summary.transform.localScale = this.SummaryScale;
      this.m_summary.transform.localRotation = Quaternion.identity;
      this.ProductPageWidget.AddNestedInstance(this.m_summary);
      this.m_summary.BindDataModel((IDataModel) this.m_summaryDataModel, false);
    }
  }

  private void CreateOrReactivateWidgetInstances(
    int rewardItemIndex,
    out WidgetInstance rewardItem,
    out WidgetInstance nameplate)
  {
    if (this.m_rewardItems.Count > rewardItemIndex)
    {
      rewardItem = this.m_rewardItems[rewardItemIndex];
      nameplate = this.m_nameplates[rewardItemIndex];
      rewardItem.gameObject.SetActive(true);
      nameplate.gameObject.SetActive(true);
      if (rewardItemIndex <= 0)
        return;
      this.m_dividers[rewardItemIndex - 1].SetActive(true);
    }
    else
    {
      rewardItem = WidgetInstance.Create((string) LargeBundleProductPage.s_rewardItemDisplay);
      if ((UnityEngine.Object) rewardItem == (UnityEngine.Object) null)
      {
        Log.Store.PrintError(string.Format("{0} cannot create an instance of {1}. Cannot create summary.", (object) nameof (LargeBundleProductPage), (object) LargeBundleProductPage.s_rewardItemDisplay));
      }
      else
      {
        rewardItem.transform.SetParent(this.TraySlider.transform);
        this.ProductPageWidget.AddNestedInstance(rewardItem);
        this.m_rewardItems.Add(rewardItem);
      }
      nameplate = WidgetInstance.Create((string) LargeBundleProductPage.s_nameplate);
      if ((UnityEngine.Object) nameplate == (UnityEngine.Object) null)
      {
        Log.Store.PrintError(string.Format("{0} cannot create an instance of {1}. Cannot create summary.", (object) nameof (LargeBundleProductPage), (object) LargeBundleProductPage.s_nameplate));
      }
      else
      {
        nameplate.transform.SetParent(this.TraySlider.transform);
        nameplate.transform.localPosition = this.NameplateBasePosition + new Vector3(this.ItemSpacingX * (float) rewardItemIndex, 0.0f, 0.0f);
        nameplate.transform.localScale = this.NameplateScale;
        nameplate.transform.localRotation = Quaternion.identity;
        this.ProductPageWidget.AddNestedInstance(nameplate);
        this.m_nameplates.Add(nameplate);
      }
      if (rewardItemIndex <= 0)
        return;
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.Divider, this.TraySlider.transform);
      gameObject.transform.localPosition = this.DividerBasePosition + new Vector3(this.ItemSpacingX * ((float) rewardItemIndex - 0.5f), 0.0f, 0.0f);
      gameObject.transform.localScale = this.DividerScale;
      gameObject.transform.localRotation = LargeBundleProductPage.s_backgroundRotation;
      this.m_dividers.Add(gameObject);
    }
  }

  private void UpdateRewardItemWidgetTransforms(
    WidgetInstance rewardItem,
    RewardItemType itemType,
    int index)
  {
    Vector3 vector3_1;
    Vector3 vector3_2;
    switch (itemType)
    {
      case RewardItemType.BATTLEGROUNDS_HERO_SKIN:
      case RewardItemType.BATTLEGROUNDS_GUIDE_SKIN:
        vector3_1 = this.HeroSkinBasePosition;
        vector3_2 = this.HeroSkinScale;
        break;
      case RewardItemType.BATTLEGROUNDS_BOARD_SKIN:
        vector3_1 = this.BoardSkinBasePosition;
        vector3_2 = this.BoardSkinScale;
        break;
      case RewardItemType.BATTLEGROUNDS_FINISHER:
        vector3_1 = this.FinisherBasePosition;
        vector3_2 = this.FinisherScale;
        break;
      case RewardItemType.BATTLEGROUNDS_EMOTE:
        vector3_1 = this.EmoteBasePosition;
        vector3_2 = this.EmoteScale;
        break;
      default:
        vector3_1 = Vector3.zero;
        vector3_2 = Vector3.one;
        break;
    }
    rewardItem.transform.localPosition = vector3_1 + new Vector3(this.ItemSpacingX * (float) index, 0.0f, 0.0f);
    rewardItem.transform.localScale = vector3_2;
    rewardItem.transform.localRotation = Quaternion.identity;
  }

  private void BindOrReplaceBinding<T>(WidgetInstance widgetInstance, T newDataModel) where T : IDataModel
  {
    IDataModel dataModel = (IDataModel) widgetInstance.GetDataModel<ShopLargeBundleDetailsNameplateDataModel>();
    if (dataModel != null)
      widgetInstance.UnbindDataModel(dataModel.DataModelId);
    widgetInstance.BindDataModel((IDataModel) newDataModel, false);
  }

  private void SetItems()
  {
    this.StopEmoteAnimation();
    DataModelList<RewardItemDataModel> items = this.Product.RewardList.Items;
    if (this.HasRewardItemLists(items))
      items = this.ExpandRewardItemLists(items);
    for (int index = 0; index < items.Count; ++index)
    {
      RewardItemDataModel rewardItemDataModel = items[index];
      WidgetInstance rewardItem;
      WidgetInstance nameplate;
      this.CreateOrReactivateWidgetInstances(index, out rewardItem, out nameplate);
      if ((UnityEngine.Object) rewardItem != (UnityEngine.Object) null)
      {
        this.UpdateRewardItemWidgetTransforms(rewardItem, rewardItemDataModel.ItemType, index);
        this.BindOrReplaceBinding<RewardItemDataModel>(rewardItem, rewardItemDataModel);
      }
      if ((UnityEngine.Object) nameplate != (UnityEngine.Object) null)
        this.BindOrReplaceBinding<ShopLargeBundleDetailsNameplateDataModel>(nameplate, new ShopLargeBundleDetailsNameplateDataModel()
        {
          Name = RewardUtils.GetName(rewardItemDataModel)
        });
    }
    for (int count = items.Count; count < this.m_rewardItems.Count; ++count)
    {
      this.m_rewardItems[count].gameObject.SetActive(false);
      this.m_nameplates[count].gameObject.SetActive(false);
      this.m_dividers[count - 1].SetActive(false);
    }
    if (this.m_popupRoot != null)
      this.m_popupRoot.ApplyPopupRendering(this.transform, this.m_popupRenderingComponents, true, this.gameObject.layer);
    this.gameObject.GetComponent<Widget>().RegisterDoneChangingStatesListener(new Action<object>(this.UpdateVisibleAnimatedEmotes), (object) null, true, true);
  }

  private void StopEmoteAnimation()
  {
    if (this.m_emoteAnimationCoroutine != null)
    {
      this.StopCoroutine(this.m_emoteAnimationCoroutine);
      this.m_emoteAnimationCoroutine = (Coroutine) null;
    }
    foreach ((Animator animator, float normalizedDisplayTime) emotesWithDisplayTime in this.m_visibleEmotesWithDisplayTimes)
      this.PauseEmoteAtNormalizedTime(emotesWithDisplayTime.animator, emotesWithDisplayTime.normalizedDisplayTime);
    this.m_visibleEmotesWithDisplayTimes.Clear();
    this.m_animatedEmoteIndex = 0;
  }

  private void UpdateVisibleAnimatedEmotes(object unused)
  {
    int num1 = this.m_pageInfoDataModel.PageNumber - 1;
    int num2 = num1 == 0 ? this.ItemsPerPage - 1 : this.ItemsPerPage;
    int num3 = num1 == 0 ? 0 : num1 * this.ItemsPerPage - 1;
    for (int index = num3; index < num3 + num2 && index < this.m_rewardItems.Count; ++index)
    {
      WidgetInstance rewardItem = this.m_rewardItems[index];
      if (rewardItem.gameObject.activeInHierarchy && rewardItem.GetDataModel<RewardItemDataModel>().ItemType == RewardItemType.BATTLEGROUNDS_EMOTE)
      {
        Animator componentInChildren = rewardItem.GetComponentInChildren<Animator>();
        if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null || (UnityEngine.Object) componentInChildren.runtimeAnimatorController == (UnityEngine.Object) null)
          Debug.LogError((object) ("LargeBundleProductPage: Failed to find animator for emote on " + rewardItem.name + ". Animation will not play."));
        else
          this.m_visibleEmotesWithDisplayTimes.Add((componentInChildren, componentInChildren.GetCurrentAnimatorStateInfo(0).normalizedTime));
      }
    }
    if (this.m_visibleEmotesWithDisplayTimes.Count <= 0)
      return;
    this.m_emoteAnimationCoroutine = this.StartCoroutine(this.AnimateVisibleEmotes());
  }

  private void PauseEmoteAtNormalizedTime(Animator animator, float normalizedDisplayTime)
  {
    int shortNameHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
    animator.Play(shortNameHash, -1, normalizedDisplayTime);
    animator.Update(0.0f);
    animator.enabled = false;
  }

  private IEnumerator AnimateVisibleEmotes()
  {
    if (this.m_shouldStartEmoteOnFirstFrame && this.m_visibleEmotesWithDisplayTimes.Count > 0)
    {
      foreach ((Animator, float) emotesWithDisplayTime in this.m_visibleEmotesWithDisplayTimes)
        this.PauseEmoteAtNormalizedTime(emotesWithDisplayTime.Item1, 0.0f);
    }
    yield return (object) new WaitForSeconds(this.m_emoteEntranceDelaySeconds);
    while (this.m_visibleEmotesWithDisplayTimes.Count > 0)
    {
      if (this.m_animatedEmoteIndex >= this.m_visibleEmotesWithDisplayTimes.Count)
      {
        this.m_animatedEmoteIndex = 0;
        yield return (object) new WaitForSeconds(this.m_pauseBetweenEmoteCycles);
      }
      (Animator animator, float normalizedDisplayTime) = this.m_visibleEmotesWithDisplayTimes[this.m_animatedEmoteIndex];
      AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
      animator.enabled = true;
      animator.Play(animatorStateInfo.shortNameHash, -1, 0.0f);
      yield return (object) new WaitForSeconds(animatorStateInfo.length);
      if (this.m_visibleEmotesWithDisplayTimes.Count > 1)
      {
        this.PauseEmoteAtNormalizedTime(animator, normalizedDisplayTime);
        if (this.m_animatedEmoteIndex < this.m_visibleEmotesWithDisplayTimes.Count - 1)
          yield return (object) new WaitForSeconds(this.m_pauseBetweenEmoteAnimations);
      }
      ++this.m_animatedEmoteIndex;
      animator = (Animator) null;
    }
  }

  private bool HasRewardItemLists(DataModelList<RewardItemDataModel> items)
  {
    foreach (RewardItemDataModel rewardItemDataModel in items)
    {
      if (rewardItemDataModel.ItemType == RewardItemType.BATTLEGROUNDS_EMOTE_PILE)
        return true;
    }
    return false;
  }

  private DataModelList<RewardItemDataModel> ExpandRewardItemLists(
    DataModelList<RewardItemDataModel> items)
  {
    DataModelList<RewardItemDataModel> dataModelList = new DataModelList<RewardItemDataModel>();
    foreach (RewardItemDataModel rewardItemDataModel1 in items)
    {
      if (rewardItemDataModel1.ItemType == RewardItemType.BATTLEGROUNDS_EMOTE_PILE)
      {
        foreach (BattlegroundsEmoteDataModel battlegroundsEmoteDataModel in rewardItemDataModel1.BGEmotePile)
        {
          RewardItemDataModel rewardItemDataModel2 = new RewardItemDataModel()
          {
            ItemType = RewardItemType.BATTLEGROUNDS_EMOTE,
            ItemId = 0,
            BGEmote = battlegroundsEmoteDataModel
          };
          dataModelList.Add(rewardItemDataModel2);
        }
      }
      else
        dataModelList.Add(rewardItemDataModel1);
    }
    return dataModelList;
  }

  private void OnDisable() => this.DisablePopupRendering();

  public void EnablePopupRendering(IPopupRoot popupRoot) => this.m_popupRoot = popupRoot;

  public void DisablePopupRendering()
  {
    if (this.m_popupRoot == null)
      return;
    if (this.m_popupRenderingComponents != null)
      this.m_popupRoot.CleanupPopupRendering(this.m_popupRenderingComponents);
    this.m_popupRoot = (IPopupRoot) null;
  }

  public bool HandlesChildPropagation() => false;
}
