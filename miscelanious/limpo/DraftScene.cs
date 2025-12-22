using UnityEngine;

public class DraftScene : PegasusScene
{
  private bool m_unloading;
  private GameObject m_loadedUIScreenObject;
  private static readonly Vector3 DRAFT_SCENE_POSITION = new Vector3(-0.5f, 1.27f, 0.0f);
  private static readonly Vector3 DRAFT_SCENE_POSITION_PHONE = new Vector3(26.1f, 0.0f, -9.88f);
  public static readonly float DRAFT_SCENE_LOCAL_SCALE_INDEX_PHONE = 1.38f;
  private static readonly Vector3 DRAFT_SCENE_LOCAL_SCALE_PHONE = Vector3.one * DraftScene.DRAFT_SCENE_LOCAL_SCALE_INDEX_PHONE;

  protected override void Awake()
  {
    base.Awake();
    if ((bool) UniversalInputManager.UsePhoneUI)
      AssetLoader.Get().InstantiatePrefab((AssetReference) "Draft_phone.prefab:a872557dedbdbe04389e66eda39ae7a7", new PrefabCallback<GameObject>(this.OnPhoneUIScreenLoaded));
    else
      AssetLoader.Get().InstantiatePrefab((AssetReference) "Draft.prefab:b005af870d543804588964c20097e43a", new PrefabCallback<GameObject>(this.OnUIScreenLoaded));
  }

  public override bool IsUnloading() => this.m_unloading;

  public override void Unload()
  {
    this.m_unloading = true;
    DraftDisplay.Get().Unload();
    Object.Destroy((Object) DraftDisplay.Get().gameObject);
    Object.Destroy((Object) this.m_loadedUIScreenObject);
    this.m_unloading = false;
  }

  private void OnUIScreenLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
    {
      Debug.LogError((object) string.Format("DraftScene.OnUIScreenLoaded() - failed to load go {0}", (object) assetRef));
    }
    else
    {
      this.m_loadedUIScreenObject = go;
      go.transform.position = DraftScene.DRAFT_SCENE_POSITION;
    }
  }

  private void OnPhoneUIScreenLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
    {
      Debug.LogError((object) string.Format("DraftScene.OnUIScreenLoaded() - failed to load go {0}", (object) assetRef));
    }
    else
    {
      this.m_loadedUIScreenObject = go;
      go.transform.position = DraftScene.DRAFT_SCENE_POSITION_PHONE;
      go.transform.localScale = DraftScene.DRAFT_SCENE_LOCAL_SCALE_PHONE;
    }
  }
}
