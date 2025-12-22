using Assets;
using Blizzard.T5.Core.Utils;
using UnityEngine;

public class ModularBundleNode : MonoBehaviour
{
  private bool m_assetsLoaded;
  private Animator m_animator;
  private string m_entryAnimationTrigger;
  private string m_exitAnimationTrigger;
  private ModularBundleLayoutNodeDbfRecord m_nodeRecord;
  private ModularBundleNodeLayout m_parentLayout;
  private static readonly AssetReference DustJarAssetReference = new AssetReference("DustJar.prefab:2ae627c7666489a43ab8e0d7cd3c2b78");
  private static readonly AssetReference ArenaTicketAssetReference = new AssetReference("ArenaTicket_Store.prefab:4d8c687ff2a4dc7469afd2139f4a1dc6");

  public float DelayBeforeEntryAnimation { get; private set; }

  public void Initialize(
    ModularBundleNodeLayout parent,
    ModularBundleLayoutNodeDbfRecord nodeRecord)
  {
    this.m_nodeRecord = nodeRecord;
    this.m_parentLayout = parent;
    this.gameObject.SetActive(false);
    this.DelayBeforeEntryAnimation = (float) nodeRecord.EntryDelay;
    this.m_entryAnimationTrigger = nodeRecord.EntryAnimation;
    this.m_exitAnimationTrigger = nodeRecord.ExitAnimation;
    this.LoadNodeAsset();
  }

  public bool IsReady() => this.m_assetsLoaded;

  public int GetNodeShakeWeight() => this.m_nodeRecord == null ? 0 : this.m_nodeRecord.ShakeWeight;

  public void AttachLoadedPrefabObjectAsChild(GameObject loadedPrefab, bool withRotation = false)
  {
    if ((Object) loadedPrefab == (Object) null)
      return;
    GameUtils.SetParent(loadedPrefab, (Component) this, withRotation);
    this.m_animator = loadedPrefab.GetComponent<Animator>();
    if ((Object) this.m_animator == (Object) null)
      this.m_animator = loadedPrefab.AddComponent<Animator>();
    ModularBundleSounds component = loadedPrefab.GetComponent<ModularBundleSounds>();
    if ((Object) component != (Object) null)
      component.Initialize(this.m_nodeRecord.EntrySound, this.m_nodeRecord.LandingSound, this.m_nodeRecord.ExitSound);
    loadedPrefab.transform.localRotation = this.transform.localRotation * loadedPrefab.transform.localRotation;
    loadedPrefab.transform.localScale = Vector3.Scale(this.transform.localScale, loadedPrefab.transform.localScale);
    this.transform.localRotation = Quaternion.identity;
    this.transform.localScale = new Vector3(1f, 1f, 1f);
  }

  public void PlayEntryAnimation()
  {
    this.gameObject.SetActive(true);
    if ((Object) this.m_animator == (Object) null || string.IsNullOrEmpty(this.m_entryAnimationTrigger))
      return;
    this.m_animator.enabled = true;
    this.m_animator.SetTrigger(this.m_entryAnimationTrigger);
    this.m_animator.speed = (float) this.m_nodeRecord.AnimSpeedMultiplier;
  }

  public void PlayExitAnimation()
  {
    if ((Object) this.m_animator == (Object) null || string.IsNullOrEmpty(this.m_exitAnimationTrigger))
      return;
    this.m_animator.enabled = true;
    this.m_animator.SetTrigger(this.m_exitAnimationTrigger);
    this.m_animator.speed = (float) this.m_nodeRecord.AnimSpeedMultiplier;
  }

  public void EnterImmediately() => this.gameObject.SetActive(true);

  public void ExitImmediately() => this.gameObject.SetActive(false);

  private void LoadNodeAsset()
  {
    this.m_assetsLoaded = false;
    switch (this.m_nodeRecord.DisplayType)
    {
      case ModularBundleLayoutNode.DisplayType.BOOSTER:
        this.LoadNodeAsset_Booster();
        break;
      case ModularBundleLayoutNode.DisplayType.TEXT:
        this.LoadNodeAsset_Text();
        break;
      case ModularBundleLayoutNode.DisplayType.DUST:
        this.LoadNodeAsset_Dust();
        break;
      case ModularBundleLayoutNode.DisplayType.PREFAB:
        this.LoadNodeAsset_Prefab();
        break;
      case ModularBundleLayoutNode.DisplayType.HERO_SKIN:
        this.LoadNodeAsset_HeroSkin();
        break;
      case ModularBundleLayoutNode.DisplayType.CARD_BACK:
        this.LoadNodeAsset_CardBack();
        break;
      case ModularBundleLayoutNode.DisplayType.ARENA_TICKET:
        this.LoadNodeAsset_ArenaTicket();
        break;
      default:
        Debug.LogWarningFormat("ModularBundleNode.LoadNodeAsset() - no load function for display type {0}!", (object) this.m_nodeRecord.DisplayType);
        break;
    }
  }

  private void LoadNodeAsset_Booster()
  {
    GeneralStorePacksContent parent = this.m_parentLayout.GetDisplay().GetParent();
    if ((Object) parent == (Object) null)
    {
      Debug.LogError((object) "ModularBundleNode.LoadNodeAsset_Booster() - no parent display!");
    }
    else
    {
      StorePackId packId;
      packId.Type = StorePackType.BOOSTER;
      packId.Id = this.m_nodeRecord.DisplayData;
      IStorePackDef storePackDef = parent.GetStorePackDef(packId);
      AssetLoader.Get().InstantiatePrefab((AssetReference) storePackDef.GetLowPolyPrefab(), new PrefabCallback<GameObject>(ModularBundleNode.OnNodeAssetLoaded_Booster), (object) new ModularBundleNode.NodeCallbackData()
      {
        requester = this,
        callbackData = (object) packId
      });
    }
  }

  private static void OnNodeAssetLoaded_Booster(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    ModularBundleNode.NodeCallbackData nodeCallbackData = (ModularBundleNode.NodeCallbackData) callbackData;
    if (nodeCallbackData == null || !GeneralUtils.IsObjectAlive((object) nodeCallbackData.requester))
    {
      Object.Destroy((Object) go);
    }
    else
    {
      ModularBundleNode requester = nodeCallbackData.requester;
      StorePackId callbackData1 = (StorePackId) nodeCallbackData.callbackData;
      requester.m_assetsLoaded = true;
      AnimatedLowPolyPack component = go.GetComponent<AnimatedLowPolyPack>();
      if ((Object) component == (Object) null)
      {
        Log.All.PrintWarning("Modular Bundle Error: Layout prefab node expected to be a Pack node but loaded cardPackId={0} assetRef={1} does not have AnimatedLowPolyPack component script dbiNodeId={2} dbiNodeLayoutId={3} text={4} for gameObject in hierarchy:\n{5}", (object) callbackData1.Id, (object) assetRef, (object) requester.m_nodeRecord.ID, (object) requester.m_nodeRecord.NodeLayoutId, (object) requester.m_nodeRecord.DisplayText.GetString(), (object) DebugUtils.GetHierarchyPath((Object) requester));
        Error.AddDevWarning("Modular Bundle Error", string.Format("Layout node={0} expected to be a Pack node but loaded cardPackId={1} does not have AnimatedLowPolyPack component script; layout={3}. See the [All] log for more details.", (object) requester.gameObject.name, (object) callbackData1.Id, (Object) requester.m_parentLayout == (Object) null ? (object) "<null>" : (object) requester.m_parentLayout.gameObject.name));
      }
      else
      {
        component.m_AmountBanner.SetActive(true);
        component.m_AmountBannerText.Text = requester.m_nodeRecord.DisplayCount.ToString();
      }
      LayerUtils.SetLayer(go, GameLayer.PerspectiveUI);
      go.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180f, 0.0f));
      requester.AttachLoadedPrefabObjectAsChild(go, true);
    }
  }

  private void LoadNodeAsset_Text()
  {
    this.m_assetsLoaded = true;
    this.m_animator = this.GetComponentInChildren<Animator>();
    ModularBundleText componentInChildren = this.GetComponentInChildren<ModularBundleText>();
    if ((Object) componentInChildren == (Object) null)
    {
      Log.All.PrintWarning("Modular Bundle Error: Layout prefab node expected to be a Text node but does not have ModularBundleText component script dbiNodeId={0} dbiNodeLayoutId={1} text={2} for gameObject in hierarchy:\n{3}", (object) this.m_nodeRecord.ID, (object) this.m_nodeRecord.NodeLayoutId, (object) this.m_nodeRecord.DisplayText.GetString(), (object) DebugUtils.GetHierarchyPath((Object) this));
      Error.AddDevWarning("Modular Bundle Error", string.Format("Layout node={0} expected to be a Text node but does not have ModularBundleText component; layout={1}. See the [All] log for more details.", (object) this.gameObject.name, (Object) this.m_parentLayout == (Object) null ? (object) "<null>" : (object) this.m_parentLayout.gameObject.name));
    }
    else
    {
      componentInChildren.Text.Text = (string) this.m_nodeRecord.DisplayText;
      componentInChildren.SetGlowSize(this.m_nodeRecord.DisplayTextGlowSize);
    }
  }

  private void LoadNodeAsset_Dust() => AssetLoader.Get().InstantiatePrefab(ModularBundleNode.DustJarAssetReference, new PrefabCallback<GameObject>(ModularBundleNode.OnNodeAssetLoaded_Dust), (object) new ModularBundleNode.NodeCallbackData()
  {
    requester = this
  });

  private static void OnNodeAssetLoaded_Dust(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    ModularBundleNode.NodeCallbackData nodeCallbackData = (ModularBundleNode.NodeCallbackData) callbackData;
    if (nodeCallbackData == null || !GeneralUtils.IsObjectAlive((object) nodeCallbackData.requester))
    {
      Object.Destroy((Object) go);
    }
    else
    {
      ModularBundleNode requester = nodeCallbackData.requester;
      requester.m_assetsLoaded = true;
      LayerUtils.SetLayer(go, GameLayer.PerspectiveUI);
      requester.AttachLoadedPrefabObjectAsChild(go, true);
      ModularBundleDustJar component = go.GetComponent<ModularBundleDustJar>();
      if ((Object) component == (Object) null)
      {
        Log.All.PrintWarning("Modular Bundle Error: Layout prefab node expected to be a DustJar node but loaded assetRef={0} does not have ModularBundleDustJar component script dbiNodeId={1} dbiNodeLayoutId={2} text={3} for gameObject in hierarchy:\n{4}", (object) assetRef, (object) requester.m_nodeRecord.ID, (object) requester.m_nodeRecord.NodeLayoutId, (object) requester.m_nodeRecord.DisplayText.GetString(), (object) DebugUtils.GetHierarchyPath((Object) requester));
        Error.AddDevWarning("Modular Bundle Error", string.Format("Layout node={0} expected to be a DustJar node but does not have ModularBundleDustJar component; layout={1}. See the [All] log for more details.", (object) requester.gameObject.name, (Object) requester.m_parentLayout == (Object) null ? (object) "<null>" : (object) requester.m_parentLayout.gameObject.name));
      }
      else
      {
        component.AmountText.Text = requester.m_nodeRecord.DisplayCount.ToString();
        if (!string.IsNullOrEmpty((string) requester.m_nodeRecord.DisplayText))
        {
          component.HeaderText.Text.Text = (string) requester.m_nodeRecord.DisplayText;
          component.HeaderText.SetGlowSize(requester.m_nodeRecord.DisplayTextGlowSize);
        }
        component.KeepHeaderTextStraight();
      }
    }
  }

  private void LoadNodeAsset_ArenaTicket() => AssetLoader.Get().InstantiatePrefab(ModularBundleNode.ArenaTicketAssetReference, new PrefabCallback<GameObject>(ModularBundleNode.OnNodeAssetLoaded_ArenaTicket), (object) new ModularBundleNode.NodeCallbackData()
  {
    requester = this
  }, AssetLoadingOptions.IgnorePrefabPosition);

  private static void OnNodeAssetLoaded_ArenaTicket(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    ModularBundleNode.NodeCallbackData nodeCallbackData = (ModularBundleNode.NodeCallbackData) callbackData;
    if (nodeCallbackData == null || !GeneralUtils.IsObjectAlive((object) nodeCallbackData.requester))
    {
      Object.Destroy((Object) go);
    }
    else
    {
      ModularBundleNode requester = nodeCallbackData.requester;
      requester.m_assetsLoaded = true;
      LayerUtils.SetLayer(go, GameLayer.PerspectiveUI);
      requester.AttachLoadedPrefabObjectAsChild(go, true);
      ModularBundleArenaTicket component = go.GetComponent<ModularBundleArenaTicket>();
      if ((Object) component == (Object) null)
      {
        Log.All.PrintWarning("Modular Bundle Error: Layout prefab node expected to be a ArenaTicket node but loaded assetRef={0} does not have ModularBundleArenaTicket component script dbiNodeId={1} dbiNodeLayoutId={2} text={3} for gameObject in hierarchy:\n{4}", (object) assetRef, (object) requester.m_nodeRecord.ID, (object) requester.m_nodeRecord.NodeLayoutId, (object) requester.m_nodeRecord.DisplayText.GetString(), (object) DebugUtils.GetHierarchyPath((Object) requester));
        Error.AddDevWarning("Modular Bundle Error", string.Format("Layout node={0} expected to be a ArenaTicket node but does not have ModularBundleArenaTicket component; layout={1}. See the [All] log for more details.", (object) requester.gameObject.name, (Object) requester.m_parentLayout == (Object) null ? (object) "<null>" : (object) requester.gameObject.name));
      }
      else
      {
        component.AmountText.Text = requester.m_nodeRecord.DisplayCount.ToString();
        if (string.IsNullOrEmpty((string) requester.m_nodeRecord.DisplayText))
          return;
        component.HeaderText.Text = (string) requester.m_nodeRecord.DisplayText;
      }
    }
  }

  private void LoadNodeAsset_Prefab()
  {
    string displayPrefab = this.m_nodeRecord.DisplayPrefab;
    AssetLoader.Get().InstantiatePrefab((AssetReference) displayPrefab, new PrefabCallback<GameObject>(ModularBundleNode.OnNodeAssetLoaded_Prefab), (object) new ModularBundleNode.NodeCallbackData()
    {
      requester = this
    });
  }

  private static void OnNodeAssetLoaded_Prefab(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    ModularBundleNode.NodeCallbackData nodeCallbackData = (ModularBundleNode.NodeCallbackData) callbackData;
    if (nodeCallbackData == null || !GeneralUtils.IsObjectAlive((object) nodeCallbackData.requester))
    {
      Object.Destroy((Object) go);
    }
    else
    {
      ModularBundleNode requester = nodeCallbackData.requester;
      requester.m_assetsLoaded = true;
      LayerUtils.SetLayer(go, GameLayer.PerspectiveUI);
      requester.AttachLoadedPrefabObjectAsChild(go, true);
    }
  }

  private void LoadNodeAsset_HeroSkin()
  {
    string assetRef = "Modular_Bundle_Card_Hero_Skin.prefab:ad8fda5915cc96747abd0e15821c9857";
    AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, new PrefabCallback<GameObject>(ModularBundleNode.OnNodeAssetLoaded_HeroSkin), (object) new ModularBundleNode.NodeCallbackData()
    {
      requester = this
    }, AssetLoadingOptions.IgnorePrefabPosition);
  }

  private static void OnNodeAssetLoaded_HeroSkin(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    ModularBundleNode.NodeCallbackData nodeCallbackData = (ModularBundleNode.NodeCallbackData) callbackData;
    if (nodeCallbackData == null || !GeneralUtils.IsObjectAlive((object) nodeCallbackData.requester))
    {
      Object.Destroy((Object) go);
    }
    else
    {
      ModularBundleNode requester = nodeCallbackData.requester;
      if ((Object) go == (Object) null)
      {
        Debug.LogWarningFormat("LoadNodeAsset_HeroSkin - FAILED to load \"{0}\"", (object) assetRef);
        requester.m_assetsLoaded = true;
      }
      else
      {
        Actor component = go.GetComponent<Actor>();
        if ((Object) component == (Object) null)
        {
          requester.m_assetsLoaded = true;
          Log.All.PrintWarning("Modular Bundle Error: Layout prefab node expected to be a HeroSkin node but loaded assetRef={0} does not have Actor component script dbiNodeId={1} dbiNodeLayoutId={2} text={3} for gameObject in hierarchy:\n{4}", (object) assetRef, (object) requester.m_nodeRecord.ID, (object) requester.m_nodeRecord.NodeLayoutId, (object) requester.m_nodeRecord.DisplayText.GetString(), (object) DebugUtils.GetHierarchyPath((Object) requester));
          Error.AddDevWarning("Modular Bundle Error", string.Format("Layout node={0} expected to be a HeroSkin node but does not have Actor component; layout={1}. See the [All] log for more details.", (object) requester.gameObject.name, (Object) requester.m_parentLayout == (Object) null ? (object) "<null>" : (object) requester.gameObject.name));
        }
        else
        {
          string cardId = GameUtils.TranslateDbIdToCardId(requester.m_nodeRecord.DisplayData);
          DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(ModularBundleNode.OnCardFullDefLoaded_HeroSkin), (object) new ModularBundleNode.NodeCallbackData()
          {
            requester = requester,
            callbackData = (object) component
          });
        }
      }
    }
  }

  private static void OnCardFullDefLoaded_HeroSkin(
    string cardID,
    DefLoader.DisposableFullDef fullDef,
    object callbackData)
  {
    using (fullDef)
    {
      ModularBundleNode.NodeCallbackData nodeCallbackData = (ModularBundleNode.NodeCallbackData) callbackData;
      if (nodeCallbackData == null || !GeneralUtils.IsObjectAlive((object) nodeCallbackData.requester))
        return;
      ModularBundleNode requester = nodeCallbackData.requester;
      requester.m_assetsLoaded = true;
      Actor callbackData1 = (Actor) nodeCallbackData.callbackData;
      callbackData1.SetFullDef(fullDef);
      callbackData1.HideAllText();
      callbackData1.UpdateAllComponents();
      CollectionHeroSkin component = callbackData1.GetComponent<CollectionHeroSkin>();
      if ((Object) component != (Object) null)
        component.SetClass(fullDef.EntityDef.GetClass());
      LayerUtils.SetLayer(callbackData1.gameObject, GameLayer.PerspectiveUI);
      requester.AttachLoadedPrefabObjectAsChild(callbackData1.gameObject, true);
    }
  }

  private void LoadNodeAsset_CardBack()
  {
    string actorName = "Modular_Bundle_Card_Back.prefab:939c318747e79d54f81ad2abab4584a2";
    CardBackManager.Get().LoadCardBackByIndex(this.m_nodeRecord.DisplayData, new CardBackManager.LoadCardBackData.LoadCardBackCallback(ModularBundleNode.OnNodeAssetLoaded_CardBack), actorName, (object) new ModularBundleNode.NodeCallbackData()
    {
      requester = this
    });
  }

  private static void OnNodeAssetLoaded_CardBack(CardBackManager.LoadCardBackData cardBackData)
  {
    ModularBundleNode.NodeCallbackData callbackData = (ModularBundleNode.NodeCallbackData) cardBackData.callbackData;
    if (callbackData == null || !GeneralUtils.IsObjectAlive((object) callbackData.requester))
    {
      Object.Destroy((Object) cardBackData.m_GameObject);
    }
    else
    {
      ModularBundleNode requester = callbackData.requester;
      requester.m_assetsLoaded = true;
      LayerUtils.SetLayer(cardBackData.m_GameObject, GameLayer.PerspectiveUI);
      requester.AttachLoadedPrefabObjectAsChild(cardBackData.m_GameObject, true);
    }
  }

  private class NodeCallbackData
  {
    public ModularBundleNode requester;
    public object callbackData;
  }
}
