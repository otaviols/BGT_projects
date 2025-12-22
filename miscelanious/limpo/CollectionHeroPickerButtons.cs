using Blizzard.T5.Core;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class CollectionHeroPickerButtons : AbsHeroPickerButtons
{
  public GameObject m_heroCountersContainer;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_heroCounterPrefab;
  private const float LABEL_Z_OFFSET = 6.35f;
  private List<TAG_CLASS> m_heroClasses;
  private Map<TAG_CLASS, WidgetInstance> m_heroCounters;
  private int m_loadedCounters;
  private int[] m_allHeroCounts;
  private int[] m_ownedHeroCounts;

  public override void Awake()
  {
    base.Awake();
    this.GenerateHeroCounters();
  }

  private void GenerateHeroCounters()
  {
    this.m_heroClasses = new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.ORDERED_HERO_CLASSES);
    this.m_heroCounters = new Map<TAG_CLASS, WidgetInstance>();
    this.m_loadedCounters = 0;
    foreach (TAG_CLASS heroClass in this.m_heroClasses)
    {
      WidgetInstance widgetInstance = WidgetInstance.Create(this.m_heroCounterPrefab);
      widgetInstance.transform.SetParent(this.m_heroCountersContainer.transform, true);
      widgetInstance.RegisterReadyListener((Action<object>) (_ => this.OnHeroPickerCounterWidgetReady()), (object) null, true);
      this.m_heroCounters[heroClass] = widgetInstance;
    }
  }

  protected override void InitHeroPickerButtons()
  {
    base.InitHeroPickerButtons();
    this.LoadHeroButtonsForFavoriteHeroes();
  }

  protected override void UpdateValidHeroClasses() => this.m_validClasses = new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.ORDERED_HERO_CLASSES);

  protected void OnHeroPickerCounterWidgetReady()
  {
    ++this.m_loadedCounters;
    if (!this.AllCountersLoaded())
      return;
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() != CollectionUtils.ViewMode.HERO_PICKER)
      this.Hide();
    else
      this.UpdateHeroTotalLabels();
  }

  private void PositionHeroTotalLabel(WidgetInstance label, Transform targetTransform)
  {
    Transform parent = label.transform.parent;
    label.transform.SetParent(targetTransform, false);
    label.transform.localPosition = new Vector3(0.0f, 0.0f, 6.35f);
    label.transform.SetParent(this.m_heroCountersContainer.transform, true);
    label.transform.localScale = new Vector3(1f, 1f, 1f);
    label.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
  }

  private void UpdateHeroTotalLabels()
  {
    if (!this.HasCounters())
      return;
    List<Transform> buttonTransforms = this.GetHeroButtonTransforms();
    List<TAG_CLASS> tagClassList = new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.ORDERED_HERO_CLASSES);
    for (int index = 0; index < tagClassList.Count; ++index)
    {
      if (index >= this.m_allHeroCounts.Length || index >= this.m_ownedHeroCounts.Length || index >= buttonTransforms.Count)
      {
        Debug.LogWarning((object) "UpdateHeroTotalLabels: mismatch between collectible hero classes and currently 'valid' classes.");
      }
      else
      {
        WidgetInstance counterForClass = this.GetCounterForClass(tagClassList[index]);
        if (!((UnityEngine.Object) counterForClass == (UnityEngine.Object) null))
        {
          this.PositionHeroTotalLabel(counterForClass, buttonTransforms[index]);
          UberText componentInChildren = counterForClass.GetComponentInChildren<UberText>(true);
          if (!((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null))
          {
            int allHeroCount = this.m_allHeroCounts[index];
            int ownedHeroCount = this.m_ownedHeroCounts[index];
            componentInChildren.Text = ownedHeroCount.ToString() + "/" + (object) allHeroCount;
          }
        }
      }
    }
  }

  protected override void OnHeroButtonReleased(UIEvent e)
  {
    if (e == null)
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "Card_Transition_Out.prefab:aecf5b5837772844b9d2db995744df82");
    HeroPickerButton element = (HeroPickerButton) e.GetElement();
    element.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.SetHeroSkinClass(new TAG_CLASS?(element.m_heroClass));
    collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.HERO_SKINS);
  }

  protected override void OnHeroMouseOver(UIEvent e)
  {
    if (e == null)
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_mouse_over.prefab:0d4e20bc78956bc48b5e2963ec39211c");
    ((HeroPickerButton) e.GetElement()).SetHighlightState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
  }

  protected override void OnHeroMouseOut(UIEvent e)
  {
    if (e == null)
      return;
    HeroPickerButton element = (HeroPickerButton) e.GetElement();
    if ((bool) UniversalInputManager.UsePhoneUI && element.IsSelected())
      return;
    element.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
  }

  public void LoadHeroButtonsForFavoriteHeroes()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return;
    this.m_heroDefsLoading = this.m_validClasses.Count;
    for (int index = 0; index < this.m_validClasses.Count; ++index)
    {
      if (index >= this.m_heroButtons.Count || (UnityEngine.Object) this.m_heroButtons[index] == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "LoadHeroButtonsForFavoriteHeroes: not enough buttons for total heroes.");
        break;
      }
      HeroPickerButton heroButton = this.m_heroButtons[index];
      heroButton.Unlock();
      heroButton.Raise();
      heroButton.Activate(true);
      TAG_CLASS validClass = this.m_validClasses[index];
      CardPortraitQuality quality = new CardPortraitQuality(3, collectionManager.GetHeroPremium(validClass));
      NetCache.CardDefinition randomFavoriteHero = collectionManager.GetRandomFavoriteHero(validClass);
      if (randomFavoriteHero == null)
      {
        Debug.LogWarning((object) ("LoadHeroButtonsForFavoriteHeroes: CCouldn't find Favorite Hero for hero class: " + (object) validClass + " defaulting to Vanilla Hero!"));
        string vanillaHero = CollectionManager.GetVanillaHero(validClass);
        AbsHeroPickerButtons.HeroFullDefLoadedCallbackData userData = new AbsHeroPickerButtons.HeroFullDefLoadedCallbackData(heroButton, quality.PremiumType);
        DefLoader.Get().LoadFullDef(vanillaHero, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(((AbsHeroPickerButtons) this).OnHeroFullDefLoaded), (object) userData, quality);
      }
      else
      {
        AbsHeroPickerButtons.HeroFullDefLoadedCallbackData userData = new AbsHeroPickerButtons.HeroFullDefLoadedCallbackData(heroButton, randomFavoriteHero.Premium);
        DefLoader.Get().LoadFullDef(randomFavoriteHero.Name, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(((AbsHeroPickerButtons) this).OnHeroFullDefLoaded), (object) userData, quality);
      }
      heroButton.SetDivotVisible(false);
    }
  }

  public WidgetInstance GetCounterForClass(TAG_CLASS heroClass) => this.m_heroCounters == null ? (WidgetInstance) null : this.m_heroCounters[heroClass];

  public bool AllCountersLoaded() => this.m_loadedCounters == this.m_heroClasses.Count;

  public bool HasCounters() => (UnityEngine.Object) this.m_heroCountersContainer != (UnityEngine.Object) null && this.m_heroCountersContainer.activeSelf;

  public bool IsReady() => !this.HasCounters() || this.AllCountersLoaded();

  public void UpdateHeroClassTotals(int[] allHeroCounts, int[] ownedHeroCounts)
  {
    this.m_allHeroCounts = allHeroCounts;
    this.m_ownedHeroCounts = ownedHeroCounts;
    this.UpdateHeroTotalLabels();
  }
}
