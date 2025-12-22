using Blizzard.T5.AssetManager;
using Hearthstone;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

[CustomEditClass]
public class BaconHeroSkinInfoManager : BaconBaseSkinInfoManager
{
  private static readonly Vector3 HERO_POWER_START_SCALE = new Vector3(0.1f, 0.1f, 0.1f);
  private const float HERO_POWER_TWEEN_TIME = 0.5f;
  public GameObject m_heroPowerParent;
  public GameObject m_heroPowerStartBone;
  public GameObject m_defaultFrame;
  private GameObject m_frameMesh;
  private Actor m_heroActor;
  private Actor m_heroPowerActor;
  private static BaconHeroSkinInfoManager s_instance;
  private static bool s_isReadyingInstance;

  public static BaconHeroSkinInfoManager Get() => BaconHeroSkinInfoManager.s_instance;

  public static void EnterPreviewWhenReady(CollectionCardVisual cardVisual)
  {
    BaconHeroSkinInfoManager heroSkinInfoManager = BaconHeroSkinInfoManager.Get();
    if ((UnityEngine.Object) heroSkinInfoManager != (UnityEngine.Object) null)
      heroSkinInfoManager.EnterPreview(cardVisual);
    else if (BaconHeroSkinInfoManager.s_isReadyingInstance)
    {
      Debug.LogWarning((object) "BaconHeroSkinInfoManager:EnterPreviewWhenReady called while the info manager instance was being readied");
    }
    else
    {
      Widget widget = (Widget) WidgetInstance.Create("BaconHeroSkinInfoManager.prefab:5cf5b98d116cb2543b44577a4b5ab97c");
      if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "BaconHeroSkinInfoManager:EnterPreviewWhenReady failed to create widget instance");
      }
      else
      {
        BaconHeroSkinInfoManager.s_isReadyingInstance = true;
        widget.RegisterReadyListener((Action<object>) (_ =>
        {
          BaconHeroSkinInfoManager.s_instance = widget.GetComponentInChildren<BaconHeroSkinInfoManager>();
          BaconHeroSkinInfoManager.s_isReadyingInstance = false;
          if ((UnityEngine.Object) BaconHeroSkinInfoManager.s_instance == (UnityEngine.Object) null)
            Debug.LogError((object) "BaconHeroSkinInfoManager:EnterPreviewWhenReady created widget instance but failed to get BaconHeroSkinInfoManager component");
          else
            BaconHeroSkinInfoManager.s_instance.EnterPreview(cardVisual);
        }), (object) null, true);
      }
    }
  }

  public static bool IsLoadedAndShowingPreview() => (bool) (UnityEngine.Object) BaconHeroSkinInfoManager.s_instance && BaconHeroSkinInfoManager.s_instance.IsShowingPreview;

  private void OnDestroy()
  {
    this.m_currentHeroCardDef?.Dispose();
    this.m_currentHeroCardDef = (DefLoader.DisposableCardDef) null;
    AssetHandle.SafeDispose<UberShaderAnimation>(ref this.m_currentHeroGoldenAnimation);
    this.CancelPreview();
    BaconHeroSkinInfoManager.s_instance = (BaconHeroSkinInfoManager) null;
  }

  public override void EnterPreview(CollectionCardVisual cardVisual)
  {
    if (this.m_animating)
      return;
    base.EnterPreview(cardVisual);
    if (this.m_currentEntityDef == null)
      return;
    this.SetHeroPower(GameUtils.GetHeroPowerCardIdFromHero(this.m_currentEntityDef.GetCardId()));
    this.m_heroActor = cardVisual.GetActor();
    if ((UnityEngine.Object) this.m_heroActor != (UnityEngine.Object) null && this.m_heroActor.HasCardDef)
    {
      this.LoadFrameMesh();
      if (!((UnityEngine.Object) this.m_heroActor != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_heroActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null))
        return;
      string pickedLine = this.m_heroActor.LegendaryHeroSkinConfig.GetPickedLine();
      if (pickedLine == null)
        return;
      SoundManager.Get().LoadAndPlay((AssetReference) pickedLine);
    }
    else
    {
      this.InstantiateFrameMesh(this.m_defaultFrame);
      this.StartCoroutine(this.WaitForCardDef());
    }
  }

  protected override void Awake()
  {
    base.Awake();
    AssetLoader.Get().InstantiatePrefab((AssetReference) "BaconCollectionDetails_HeroPower.prefab:effbe7f7919e2f34b9535d11fe149d0f", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private IEnumerator WaitForCardDef()
  {
    while ((UnityEngine.Object) this.m_heroActor != (UnityEngine.Object) null && !this.m_heroActor.HasCardDef)
      yield return (object) null;
    this.LoadFrameMesh();
  }

  private void LoadFrameMesh()
  {
    if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      return;
    DefLoader.DisposableCardDef disposableCardDef = this.m_heroActor.ShareDisposableCardDef();
    if (disposableCardDef != null && (UnityEngine.Object) disposableCardDef.CardDef.m_FrameMeshOverride != (UnityEngine.Object) null)
      this.InstantiateFrameMesh(disposableCardDef.CardDef.m_FrameMeshOverride);
    else
      this.InstantiateFrameMesh(this.m_defaultFrame);
  }

  private void InstantiateFrameMesh(GameObject frameObject)
  {
    if ((UnityEngine.Object) this.m_frameMesh != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_frameMesh);
    this.m_frameMesh = UnityEngine.Object.Instantiate<GameObject>(frameObject, this.m_vanillaHeroFrame.transform);
    LayerUtils.SetLayer(this.m_frameMesh, GameLayer.IgnoreFullScreenEffects);
  }

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError(string.Format("CollectionDeckInfo.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
      {
        Log.CollectionManager.PrintError(string.Format("BaconHeroSkinInfoManager.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_heroPowerActor.SetUnlit();
        this.m_heroPowerActor.transform.parent = this.m_heroPowerParent.transform;
        this.RecursivelyCopyLayer(this.m_heroPowerParent, go);
        this.m_heroPowerActor.transform.localScale = Vector3.one;
        this.m_heroPowerActor.transform.localPosition = Vector3.zero;
        go.GetComponent<TokyoDrift>().enabled = true;
        if (!UniversalInputManager.Get().IsTouchMode())
          return;
        this.m_heroPowerActor.TurnOffCollider();
      }
    }
  }

  private void RecursivelyCopyLayer(GameObject source, GameObject dest)
  {
    dest.layer = source.layer;
    foreach (Transform transform in dest.transform)
      this.RecursivelyCopyLayer(dest, transform.gameObject);
  }

  private void SetHeroPower(string heroPowerCardId) => DefLoader.Get().LoadFullDef(heroPowerCardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroPowerFullDefLoaded));

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
      while ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
        yield return (object) null;
      this.SetHeroPowerInfo(heroPowerCardID, def, premium);
    }
  }

  private void SetHeroPowerInfo(
    string heroPowerCardID,
    DefLoader.DisposableFullDef def,
    TAG_PREMIUM premium)
  {
    this.m_heroPowerActor.Show();
    this.m_heroPowerActor.SetFullDef(def);
    this.m_heroPowerActor.UpdateAllComponents();
    this.m_heroPowerActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
    this.m_heroPowerActor.SetUnlit();
    Transform transform = this.m_heroPowerParent.transform;
    Vector3 localPosition = transform.localPosition;
    Vector3 localScale = transform.localScale;
    transform.position = this.m_heroPowerStartBone.transform.position;
    transform.localScale = BaconHeroSkinInfoManager.HERO_POWER_START_SCALE;
    iTween.MoveTo(this.m_heroPowerParent, iTween.Hash((object) "position", (object) localPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.5f));
    iTween.ScaleTo(this.m_heroPowerParent, iTween.Hash((object) "scale", (object) localScale, (object) "isLocal", (object) true, (object) "time", (object) 0.5f));
  }

  protected override void PushNavigateBack() => Navigation.PushUnique(new Navigation.NavigateBackHandler(BaconHeroSkinInfoManager.OnNavigateBack));

  protected override void RemoveNavigateBack() => Navigation.RemoveHandler(new Navigation.NavigateBackHandler(BaconHeroSkinInfoManager.OnNavigateBack));

  private static bool OnNavigateBack()
  {
    BaconHeroSkinInfoManager heroSkinInfoManager = BaconHeroSkinInfoManager.Get();
    if ((UnityEngine.Object) heroSkinInfoManager != (UnityEngine.Object) null)
      heroSkinInfoManager.CancelPreview();
    return true;
  }

  protected override void SetFavoriteHero()
  {
    int dbId = GameUtils.TranslateCardIdToDbId(this.m_currentEntityDef.GetCardId());
    BattlegroundsHeroSkinId favoriteSkinId;
    if (CollectionManager.Get().IsBattlegroundsBaseHeroCardWithSkin(dbId) && CollectionManager.Get().GetFavoriteBattlegroundsHeroSkin(dbId, out favoriteSkinId))
      Network.Get().ClearBattlegroundsFavoriteHeroSkin(favoriteSkinId);
    BattlegroundsHeroSkinId skinId;
    if (!CollectionManager.Get().IsBattlegroundsHeroSkinCard(dbId) || !CollectionManager.Get().GetBattlegroundsHeroSkinIdForSkinCardId(dbId, out skinId))
      return;
    Network.Get().SetBattlegroundsFavoriteHeroSkin(skinId);
  }

  protected override bool CanToggleFavorite() => BaconHeroSkinUtils.CanFavoriteBattlegroundsHeroSkin(this.m_currentEntityDef);

  protected override void AppendDebugTextForCurrentCard(StringBuilder builder)
  {
    base.AppendDebugTextForCurrentCard(builder);
    int dbId = GameUtils.TranslateCardIdToDbId(this.m_currentEntityDef.GetCardId());
    BattlegroundsHeroSkinId skinId;
    if (CollectionManager.Get().GetBattlegroundsHeroSkinIdForSkinCardId(dbId, out skinId))
    {
      builder.Append("Hero Skin Id: ");
      builder.Append(skinId.ToValue());
      builder.AppendLine();
    }
    else
      builder.AppendLine("No Hero Skin Id");
  }
}
