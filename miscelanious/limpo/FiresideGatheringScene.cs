using PegasusShared;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class FiresideGatheringScene : PegasusScene
{
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_FiresideGatheringPrefab;
  private bool m_unloading;
  private bool m_collectionLoaded;
  private bool m_tavernBrawlLoaded;
  private BrawlType m_enteredBrawlType;
  private bool m_firesideGatheringPrefabLoaded;

  private void Start()
  {
    AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) this.m_FiresideGatheringPrefab, new PrefabCallback<GameObject>(this.OnFiresideGatheringPrefabLoaded));
    this.m_enteredBrawlType = TavernBrawlManager.Get().CurrentBrawlType;
    TavernBrawlManager.Get().CurrentBrawlType = BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING;
    if (TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING))
      TavernBrawlManager.Get().EnsureAllDataReady(new TavernBrawlManager.CallbackEnsureServerDataReady(this.OnFiresideBrawlServerDataReady));
    else
      this.OnFiresideBrawlServerDataReady();
  }

  private void Update() => Network.Get().ProcessNetwork();

  private void OnDestroy()
  {
    bool flag = SceneMgr.Get() != null && SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY;
    if (TavernBrawlManager.Get() == null || flag)
      return;
    TavernBrawlManager.Get().CurrentBrawlType = this.m_enteredBrawlType;
  }

  public override bool IsUnloading() => this.m_unloading;

  public override void Unload()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      BnetBar.Get().ToggleActive(true);
    this.m_unloading = true;
    this.m_unloading = false;
  }

  public void OnFiresideGatheringPrefabLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_firesideGatheringPrefabLoaded = true;
    if (!((Object) go == (Object) null))
      return;
    Debug.LogError((object) string.Format("TavernBrawlScene.OnTavernBrawlLoaded() - failed to load screen {0}", (object) assetRef));
  }

  private void OnFiresideBrawlServerDataReady()
  {
    this.StartCoroutine(this.NotifySceneLoadedWhenReady());
    TavernBrawlManager.Get().CurrentBrawlType = BrawlType.BRAWL_TYPE_TAVERN_BRAWL;
    if (TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
      TavernBrawlManager.Get().EnsureAllDataReady(new TavernBrawlManager.CallbackEnsureServerDataReady(this.OnTavernBrawlServerDataReady));
    else
      this.OnTavernBrawlServerDataReady();
    CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded(new CollectionManager.DelOnAllDeckContents(this.OnCollectionDataReady));
  }

  private void OnTavernBrawlServerDataReady()
  {
    this.m_tavernBrawlLoaded = true;
    this.OnAllPresenceDataLoaded();
  }

  private void OnCollectionDataReady()
  {
    this.m_collectionLoaded = true;
    this.OnAllPresenceDataLoaded();
  }

  private void OnAllPresenceDataLoaded()
  {
    if (!this.m_collectionLoaded || !this.m_tavernBrawlLoaded)
      return;
    FiresideGatheringManager.Get().UpdateDeckValidity();
  }

  private IEnumerator NotifySceneLoadedWhenReady()
  {
    while (!this.m_firesideGatheringPrefabLoaded)
      yield return (object) 0;
    SceneMgr.Get().NotifySceneLoaded();
  }
}
