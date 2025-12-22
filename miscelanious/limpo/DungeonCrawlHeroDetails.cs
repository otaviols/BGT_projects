using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCrawlHeroDetails : MonoBehaviour
{
  public AsyncReference m_heroClassIconsControllerReference;
  public AsyncReference m_playButtonReference;
  public Transform m_hero_Bone;
  public Transform m_heroPower_Bone;
  public UberText m_heroName;
  private Actor m_heroActor;
  private Actor m_heroPowerActor;
  private Actor m_heroPowerBigCard;
  private PegUIElement m_heroPower;
  private EntityDef m_heroEntityDef;
  private DefLoader.DisposableCardDef m_heroCardDef;
  private EntityDef m_heroPowerEntityDef;
  private DefLoader.DisposableCardDef m_heroPowerCardDef;
  private List<DungeonCrawlHeroDetails.HeroPowerUIEvent> m_heroPowerPendingUIEvents = new List<DungeonCrawlHeroDetails.HeroPowerUIEvent>();

  public Actor HeroActor => this.m_heroActor;

  private void Awake()
  {
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Dungeon_Play_Hero.prefab:183cb9cc59697844e911776ec349fe5e", new PrefabCallback<GameObject>(this.OnHeroActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnDestroy()
  {
    this.m_heroCardDef?.Dispose();
    this.m_heroPowerCardDef?.Dispose();
  }

  private void OnHeroActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AdventureHeroDetails.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AdventureHeroDetails.OnHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        GameUtils.SetParent(go, (Component) this.m_hero_Bone);
        go.transform.parent = this.m_hero_Bone.transform;
        go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        go.layer = this.gameObject.layer;
        this.m_heroActor.SetUnlit();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_heroActor.m_attackObject);
        this.m_heroActor.Hide();
        this.RefreshHeroInfo();
      }
    }
  }

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AdventureHeroDetails.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AdventureHeroDetails.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_heroPower = go.AddComponent<PegUIElement>();
        go.AddComponent<BoxCollider>();
        GameUtils.SetParent(go, (Component) this.m_heroPower_Bone);
        go.transform.parent = this.m_heroPower_Bone.transform;
        go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        go.layer = this.gameObject.layer;
        this.m_heroPowerActor.SetUnlit();
        this.m_heroPowerActor.Hide();
        this.m_heroPower.GetComponent<Collider>().enabled = true;
        this.m_heroName.Text = "";
        this.RefreshHeroPowerInfo();
        this.ApplyPendingUIEventsIfLoaded();
      }
    }
  }

  private void OnHeroClassIconsControllerReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      Debug.LogWarning((object) "AdventureDungeonCrawlDisplay.OnHeroIconsControllerReady - widget was null!");
    else if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "AdventureDungeonCrawlDisplay.OnHeroIconsControllerReady - m_heroActor was null!");
    }
    else
    {
      HeroClassIconsDataModel classIconsDataModel = new HeroClassIconsDataModel();
      EntityDef entityDef = this.m_heroActor.GetEntityDef();
      if (entityDef == null)
      {
        Debug.LogWarning((object) "AdventureDungeonCrawlDisplay.OnHeroIconsControllerReady - m_heroActor did not contain an entity def!");
      }
      else
      {
        classIconsDataModel.Classes.Clear();
        entityDef.GetClasses((IList<TAG_CLASS>) classIconsDataModel.Classes);
        widget.BindDataModel((IDataModel) classIconsDataModel);
      }
    }
  }

  private void RefreshHeroInfo()
  {
    if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null || this.m_heroEntityDef == null || this.m_heroCardDef == null)
      return;
    this.m_heroName.Text = this.m_heroEntityDef.GetName();
    this.m_heroActor.SetPremium(TAG_PREMIUM.NORMAL);
    this.m_heroActor.SetEntityDef(this.m_heroEntityDef);
    this.m_heroActor.SetCardDef(this.m_heroCardDef);
    this.m_heroActor.UpdateAllComponents();
    this.m_heroActor.SetUnlit();
    this.m_heroActor.Show();
    this.m_heroClassIconsControllerReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnHeroClassIconsControllerReady));
  }

  private void RefreshHeroPowerInfo()
  {
    if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null || this.m_heroPowerEntityDef == null || this.m_heroPowerCardDef == null)
      return;
    this.m_heroPowerActor.SetPremium(TAG_PREMIUM.NORMAL);
    this.m_heroPowerActor.SetEntityDef(this.m_heroPowerEntityDef);
    this.m_heroPowerActor.SetCardDef(this.m_heroPowerCardDef);
    this.m_heroPowerActor.UpdateAllComponents();
    this.m_heroPowerActor.SetUnlit();
    this.m_heroPowerActor.AlwaysRenderPremiumPortrait = false;
    this.m_heroPowerActor.UpdateMaterials();
    this.m_heroPowerActor.SetUnlit();
    this.m_heroPowerActor.Show();
  }

  private void ApplyPendingUIEventsIfLoaded()
  {
    if ((UnityEngine.Object) this.m_heroPower == (UnityEngine.Object) null)
      return;
    foreach (DungeonCrawlHeroDetails.HeroPowerUIEvent powerPendingUiEvent in this.m_heroPowerPendingUIEvents)
      this.m_heroPower.AddEventListener(powerPendingUiEvent.m_type, powerPendingUiEvent.m_handler);
    this.m_heroPowerPendingUIEvents.Clear();
  }

  public void UpdateHeroInfo(DefLoader.DisposableFullDef fullDef) => this.UpdateHeroInfo(fullDef?.EntityDef, fullDef?.DisposableCardDef);

  public void UpdateHeroInfo(EntityDef entityDef, DefLoader.DisposableCardDef cardDef)
  {
    this.m_heroEntityDef = entityDef;
    this.m_heroCardDef?.Dispose();
    this.m_heroCardDef = cardDef?.Share();
    this.RefreshHeroInfo();
  }

  public void UpdateHeroPowerInfo(DefLoader.DisposableFullDef fullDef) => this.UpdateHeroPowerInfo(fullDef?.EntityDef, fullDef?.DisposableCardDef);

  public void UpdateHeroPowerInfo(EntityDef entityDef, DefLoader.DisposableCardDef cardDef)
  {
    this.m_heroPowerEntityDef = entityDef;
    this.m_heroCardDef?.Dispose();
    this.m_heroPowerCardDef = cardDef?.Share();
    this.RefreshHeroPowerInfo();
  }

  public void AddHeroPowerListener(UIEventType type, UIEvent.Handler handler)
  {
    this.m_heroPowerPendingUIEvents.Add(new DungeonCrawlHeroDetails.HeroPowerUIEvent()
    {
      m_type = type,
      m_handler = handler
    });
    this.ApplyPendingUIEventsIfLoaded();
  }

  private class HeroPowerUIEvent
  {
    public UIEventType m_type;
    public UIEvent.Handler m_handler;
  }
}
