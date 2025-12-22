using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureStorymodeChapterStoreHeroController : MonoBehaviour
{
  private const string HERO_BUTTONS_CHANGED_EVENT_NAME = "HERO_BUTTONS_CHANGED";
  public bool m_useHeroesFromManyWings;
  public AsyncReference[] m_storeHeroButtonReferences;
  private VisualController m_visualController;
  private int m_widgetReferencesToLoad;
  private List<GuestHeroPickerButton> m_heroPickerButtons = new List<GuestHeroPickerButton>();
  private List<Widget> m_storeHeroButtons = new List<Widget>();
  private List<DefLoader.DisposableFullDef> m_heroFullDefs = new List<DefLoader.DisposableFullDef>();
  private int m_heroesToLoad;

  private void Start()
  {
    this.m_visualController = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) this.m_visualController == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("AdventureStorymodeChapterStoreHeroController.Start: visual controller does not exist!");
    }
    else
    {
      this.m_visualController.Owner.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
      {
        if (!(eventName == "HERO_BUTTONS_CHANGED"))
          return;
        this.StopCoroutine("RefreshHeroButtonsWhenReady");
        this.StartCoroutine("RefreshHeroButtonsWhenReady");
      }));
      this.m_widgetReferencesToLoad = this.m_storeHeroButtonReferences.Length;
      foreach (AsyncReference heroButtonReference in this.m_storeHeroButtonReferences)
        heroButtonReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnHeroPickerButtonWidgetReady));
      this.StartCoroutine("RefreshHeroButtonsWhenReady");
    }
  }

  private void OnDestroy() => this.ReleaseHeroDefs();

  private void OnHeroPickerButtonWidgetReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("AdventureStorymodeChapterStoreHeroController.OnHeroPickerButtonWidgetReady: the Widget was null!");
    }
    else
    {
      this.m_storeHeroButtons.Add(widget);
      GuestHeroPickerButton componentInChildren = widget.GetComponentInChildren<GuestHeroPickerButton>();
      if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
        Log.Adventures.PrintError("AdventureStorymodeChapterStoreHeroController.OnHeroPickerButtonWidgetReady: the Widget did not have the hero picker button component!");
      else
        this.m_heroPickerButtons.Add(componentInChildren);
    }
  }

  private void OnFullDefLoaded(string cardId, DefLoader.DisposableFullDef fullDef, object userData) => this.m_heroFullDefs.Add(fullDef);

  private IEnumerator RefreshHeroButtonsWhenReady()
  {
    this.LoadHeroFullDefs();
    while (this.m_widgetReferencesToLoad > this.m_storeHeroButtons.Count || this.m_heroesToLoad > this.m_heroFullDefs.Count)
      yield return (object) null;
    this.SetupHeroPickerButtons();
  }

  private void LoadHeroFullDefs()
  {
    AdventureBookPageDataModel bookPageDataModel = this.GetAdventureBookPageDataModel();
    WingDbId wingId = bookPageDataModel != null ? (WingDbId) bookPageDataModel.ChapterData.WingId : WingDbId.INVALID;
    AdventureDataDbfRecord record = AdventureConfig.Get().GetSelectedAdventureDataRecord();
    List<AdventureGuestHeroesDbfRecord> all = GameDbf.AdventureGuestHeroes.GetRecords().FindAll((Predicate<AdventureGuestHeroesDbfRecord>) (x =>
    {
      if (x.AdventureId != record.AdventureId)
        return false;
      return this.m_useHeroesFromManyWings || (WingDbId) x.WingId == wingId;
    }));
    this.m_heroesToLoad = 0;
    this.ReleaseHeroDefs();
    foreach (AdventureGuestHeroesDbfRecord guestHeroesDbfRecord in all)
    {
      GuestHeroDbfRecord record1 = GameDbf.GuestHero.GetRecord(guestHeroesDbfRecord.GuestHeroId);
      if (record1 != null)
      {
        string cardId = GameUtils.TranslateDbIdToCardId(record1.CardId);
        DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded));
        ++this.m_heroesToLoad;
      }
      else
        Log.Adventures.Print("AdventureStoreModeChapterStoreHeroController: Guest Hero with ID={0} not found!", (object) guestHeroesDbfRecord.GuestHeroId);
    }
  }

  private void SetupHeroPickerButtons()
  {
    if (this.m_heroFullDefs.Count > this.m_heroPickerButtons.Count)
      Log.Adventures.Print("AdventureStoreModeChapterStoreHeroController: More hero defs than buttons, only the first {0} heroes will be displayed", (object) this.m_heroPickerButtons.Count);
    for (int index = 0; index < this.m_heroPickerButtons.Count; ++index)
    {
      this.m_heroPickerButtons[index].SetDivotVisible(false);
      if (index < this.m_heroFullDefs.Count)
        this.UpdateHeroData(index);
    }
  }

  private AdventureBookPageDataModel GetAdventureBookPageDataModel()
  {
    if ((UnityEngine.Object) this.m_visualController == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("AdventureStorymodeChapterStoreHeroController.GetDataModel: visual controller does not exist!");
      return (AdventureBookPageDataModel) null;
    }
    IDataModel dataModel;
    this.m_visualController.GetDataModel(2, out dataModel);
    return dataModel as AdventureBookPageDataModel;
  }

  private void UpdateHeroData(int index)
  {
    GuestHeroPickerButton heroPickerButton = this.m_heroPickerButtons[index];
    if ((UnityEngine.Object) heroPickerButton == (UnityEngine.Object) null)
    {
      Debug.LogErrorFormat("AdventureStorymodeChapterStoreHeroController.UpdateHeroData - HeroPickerButton at index {0} is null!", (object) index);
    }
    else
    {
      DefLoader.DisposableFullDef heroFullDef = this.m_heroFullDefs[index];
      if (heroFullDef == null)
      {
        Debug.LogErrorFormat("AdventureStorymodeChapterStoreHeroController.UpdateHeroData - HeroPickerButton at index {0} is null!", (object) index);
      }
      else
      {
        EntityDef entityDef = heroFullDef.EntityDef;
        if (entityDef == null)
        {
          Debug.LogWarning((object) "AdventureStorymodeChapterStoreHeroController.UpdateSelectedHeroClasses - button did not contain an entity def!");
        }
        else
        {
          heroPickerButton.SetGuestHero(GameDbf.GuestHero.GetRecord((Predicate<GuestHeroDbfRecord>) (r => r.CardId == GameUtils.TranslateCardIdToDbId(entityDef.GetCardId()))));
          heroPickerButton.UpdateDisplay(heroFullDef, TAG_PREMIUM.NORMAL);
          heroPickerButton.HideTextAndGradient();
          Widget storeHeroButton = this.m_storeHeroButtons[index];
          if ((UnityEngine.Object) storeHeroButton == (UnityEngine.Object) null)
            return;
          HeroClassIconsDataModel classIconsDataModel = new HeroClassIconsDataModel();
          classIconsDataModel.Classes.Clear();
          entityDef.GetClasses((IList<TAG_CLASS>) classIconsDataModel.Classes);
          storeHeroButton.BindDataModel((IDataModel) classIconsDataModel);
        }
      }
    }
  }

  private void ReleaseHeroDefs() => this.m_heroFullDefs.DisposeValuesAndClear<DefLoader.DisposableFullDef>();
}
