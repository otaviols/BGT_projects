using Hearthstone.DataModels;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public abstract class AbsHeroPickerButtons : MonoBehaviour
{
  public GameObject m_rootObject;
  public GameObject m_buttonContainer;
  public List<GameObject> m_heroPickerButtonBonesByHeroCount;
  [SerializeField]
  protected bool m_isMobileLayout;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_heroButtonWidgetPrefab;
  protected int m_HeroPickerButtonCount;
  protected List<HeroPickerButton> m_heroButtons = new List<HeroPickerButton>();
  protected HeroPickerButton m_selectedHeroButton;
  protected List<Transform> m_heroBones;
  protected List<TAG_CLASS> m_validClasses = new List<TAG_CLASS>();
  protected int m_heroDefsLoading = int.MaxValue;
  private WidgetTemplate m_widget;
  protected bool m_hasLoaded;

  [Overridable]
  public bool IsMobileLayout
  {
    get => this.m_isMobileLayout;
    set => this.m_isMobileLayout = value;
  }

  public virtual void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterDoneChangingStatesListener(new Action<object>(this.HandleDoneChangingStates), (object) null, true, false);
  }

  private void HandleDoneChangingStates(object unused)
  {
    if (this.m_hasLoaded)
      return;
    this.StartCoroutine(this.LoadHeroButtons());
    this.m_hasLoaded = true;
  }

  protected virtual void OnHeroButtonReleased(UIEvent e)
  {
  }

  protected virtual void OnHeroMouseOver(UIEvent e)
  {
  }

  protected virtual void OnHeroMouseOut(UIEvent e)
  {
  }

  protected virtual IEnumerator WaitForHeroPickerButtonsLoaded()
  {
    while (this.m_heroButtons.Count < this.m_HeroPickerButtonCount)
      yield return (object) null;
    foreach (HeroPickerButton button in this.m_heroButtons)
    {
      while (button.GetComponent<WidgetTemplate>().IsChangingStates)
        yield return (object) null;
    }
  }

  protected IEnumerator LoadHeroButtons(int? m_cheatOverrideHeroPickerButtonCount = null)
  {
    AbsHeroPickerButtons heroPickerButtons1 = this;
    heroPickerButtons1.m_HeroPickerButtonCount = !m_cheatOverrideHeroPickerButtonCount.HasValue ? heroPickerButtons1.ValidateHeroCount() : m_cheatOverrideHeroPickerButtonCount.Value;
    heroPickerButtons1.SetupHeroLayout();
    foreach (Component heroButton in heroPickerButtons1.m_heroButtons)
      UnityEngine.Object.Destroy((UnityEngine.Object) heroButton.gameObject);
    heroPickerButtons1.m_heroButtons.Clear();
    HeroPickerDataModel heroPickerDataModel = heroPickerButtons1.GetHeroPickerDataModel();
    for (int index = 0; index < heroPickerButtons1.m_HeroPickerButtonCount; ++index)
    {
      AbsHeroPickerButtons heroPickerButtons = heroPickerButtons1;
      WidgetInstance heroPickerButtonWidget = WidgetInstance.Create(heroPickerButtons1.m_heroButtonWidgetPrefab);
      if (heroPickerDataModel != null)
        heroPickerButtonWidget.BindDataModel((IDataModel) heroPickerDataModel, false);
      heroPickerButtonWidget.RegisterReadyListener((Action<object>) (_ => heroPickerButtons.OnHeroPickerButtonWidgetReady(heroPickerButtonWidget)), (object) null, true);
    }
    yield return (object) heroPickerButtons1.StartCoroutine(heroPickerButtons1.WaitForHeroPickerButtonsLoaded());
    heroPickerButtons1.InitHeroPickerButtons();
  }

  protected virtual void InitHeroPickerButtons()
  {
  }

  protected void SetupHeroLayout()
  {
    if (this.m_HeroPickerButtonCount <= 0 || this.m_HeroPickerButtonCount > this.m_heroPickerButtonBonesByHeroCount.Count || (UnityEngine.Object) this.m_heroPickerButtonBonesByHeroCount[this.m_HeroPickerButtonCount] == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintWarning("Deck/Class Picker Instantiated with an unsupported amount of heroes: " + (object) this.m_HeroPickerButtonCount);
    }
    else
    {
      List<Transform> locationsFromLayout = this.GetBoneLocationsFromLayout(this.m_heroPickerButtonBonesByHeroCount[this.m_HeroPickerButtonCount]);
      this.m_heroBones = new List<Transform>();
      this.m_heroBones.AddRange((IEnumerable<Transform>) locationsFromLayout);
      if (this.m_heroBones.Count == this.m_HeroPickerButtonCount)
        return;
      Log.Adventures.PrintWarning("Layout for {0} heroes yielded an incorrect amount of transforms. This will result in errors when displaying heroes!", (object) this.m_HeroPickerButtonCount);
    }
  }

  private List<Transform> GetBoneLocationsFromLayout(GameObject layout)
  {
    List<Transform> locationsFromLayout = new List<Transform>();
    foreach (Transform componentsInChild in layout.GetComponentsInChildren<Transform>())
    {
      if (componentsInChild.childCount == 0)
        locationsFromLayout.Add(componentsInChild);
    }
    return locationsFromLayout;
  }

  protected void OnHeroPickerButtonWidgetReady(WidgetInstance widget)
  {
    HeroPickerButton componentInChildren = widget.GetComponentInChildren<HeroPickerButton>();
    this.m_heroButtons.Add(componentInChildren);
    this.SetUpHeroPickerButton(componentInChildren, this.m_heroButtons.Count - 1);
    componentInChildren.Lock();
    componentInChildren.Activate(false);
    componentInChildren.AddEventListener(UIEventType.TAP, new UIEvent.Handler(this.OnHeroButtonReleased));
    componentInChildren.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnHeroButtonReleased));
    componentInChildren.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnHeroMouseOver));
    componentInChildren.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHeroMouseOut));
    Vector3 pos = (UnityEngine.Object) componentInChildren.m_raiseAndLowerRoot != (UnityEngine.Object) null ? componentInChildren.m_raiseAndLowerRoot.transform.localPosition : this.transform.localPosition;
    componentInChildren.SetOriginalLocalPosition(pos);
  }

  protected void SetUpHeroPickerButton(HeroPickerButton button, int heroCount)
  {
    GameObject gameObject = button.gameObject;
    Transform parent = gameObject.transform.parent;
    gameObject.name = string.Format("{0}_{1}", (object) gameObject.name, (object) heroCount);
    parent.transform.SetParent(this.m_heroBones[heroCount], false);
    parent.transform.localScale = Vector3.one;
    parent.transform.localPosition = Vector3.zero;
    parent.SetParent(this.m_buttonContainer.transform, true);
  }

  protected virtual void UpdateValidHeroClasses()
  {
    this.m_validClasses = new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.ORDERED_HERO_CLASSES);
    if (SceneMgr.Get() == null)
      return;
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    if (Options.GetFormatType() == FormatType.FT_CLASSIC && mode != SceneMgr.Mode.ADVENTURE)
      this.m_validClasses = new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.CLASSIC_ORDERED_HERO_CLASSES);
    ScenarioDbId? nullable1 = new ScenarioDbId?();
    if (mode == SceneMgr.Mode.ADVENTURE)
      nullable1 = new ScenarioDbId?(AdventureConfig.Get().GetMission());
    if (mode == SceneMgr.Mode.TAVERN_BRAWL || mode == SceneMgr.Mode.FIRESIDE_GATHERING && FiresideGatheringManager.Get().InBrawlMode() || FriendChallengeMgr.Get().IsChallengeTavernBrawl())
      nullable1 = new ScenarioDbId?((ScenarioDbId) TavernBrawlManager.Get().CurrentMission().missionId);
    if (!nullable1.HasValue)
      return;
    ScenarioDbId? nullable2 = nullable1;
    ScenarioDbId scenarioDbId = ScenarioDbId.INVALID;
    if (nullable2.GetValueOrDefault() == scenarioDbId & nullable2.HasValue)
      return;
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) nullable1.Value);
    for (int index = 0; index < record.ClassExclusions.Count; ++index)
      this.m_validClasses.Remove((TAG_CLASS) record.ClassExclusions[index].ClassId);
  }

  protected virtual int ValidateHeroCount()
  {
    this.UpdateValidHeroClasses();
    return this.m_validClasses.Count;
  }

  protected virtual IEnumerator SetHeroButtonsEnabled(bool enable)
  {
    AbsHeroPickerButtons heroPickerButtons = this;
    yield return (object) heroPickerButtons.StartCoroutine(heroPickerButtons.WaitForHeroPickerButtonsLoaded());
    foreach (HeroPickerButton heroButton in heroPickerButtons.m_heroButtons)
    {
      if (!heroButton.IsLocked() || !enable)
        heroButton.SetEnabled(enable);
    }
  }

  protected virtual void OnHeroFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef fullDef,
    object userData)
  {
    using (fullDef)
    {
      if (fullDef?.EntityDef != null)
      {
        AbsHeroPickerButtons.HeroFullDefLoadedCallbackData loadedCallbackData = userData as AbsHeroPickerButtons.HeroFullDefLoadedCallbackData;
        TAG_PREMIUM premium = GameUtils.IsVanillaHero(cardId) ? CollectionManager.Get().GetBestCardPremium(cardId) : TAG_PREMIUM.GOLDEN;
        loadedCallbackData.HeroPickerButton.UpdateDisplay(fullDef, premium);
        if (!this.m_hasLoaded)
        {
          Vector3 pos = (UnityEngine.Object) loadedCallbackData.HeroPickerButton.m_raiseAndLowerRoot != (UnityEngine.Object) null ? loadedCallbackData.HeroPickerButton.m_raiseAndLowerRoot.transform.localPosition : loadedCallbackData.HeroPickerButton.transform.localPosition;
          loadedCallbackData.HeroPickerButton.SetOriginalLocalPosition(pos);
        }
      }
      --this.m_heroDefsLoading;
    }
  }

  public void Show() => this.m_rootObject.SetActive(true);

  public void Hide() => this.m_rootObject.SetActive(false);

  public HeroPickerDataModel GetHeroPickerDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (HeroPickerDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(13, out model))
    {
      model = (IDataModel) new HeroPickerDataModel();
      owner.BindDataModel(model);
    }
    return model as HeroPickerDataModel;
  }

  public List<Transform> GetHeroButtonTransforms() => this.m_heroBones;

  protected class HeroFullDefLoadedCallbackData
  {
    public HeroFullDefLoadedCallbackData(HeroPickerButton button, TAG_PREMIUM premium)
    {
      this.HeroPickerButton = button;
      this.Premium = premium;
    }

    public HeroPickerButton HeroPickerButton { get; private set; }

    private TAG_PREMIUM Premium
    {
      set => this.\u003CPremium\u003Ek__BackingField = value;
    }
  }
}
