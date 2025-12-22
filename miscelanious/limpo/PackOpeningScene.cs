using UnityEngine;

public class PackOpeningScene : PegasusScene
{
  private PackOpening m_packOpening;

  protected override void Awake()
  {
    base.Awake();
    AssetLoader.Get().InstantiatePrefab((AssetReference) "PackOpening.prefab:1eb13e056b6780048bba1ae1c7a250cf", new PrefabCallback<GameObject>(this.OnUIScreenLoaded));
  }

  private void Update() => Network.Get().ProcessNetwork();

  private void OnUIScreenLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
    {
      Debug.LogError((object) string.Format("PackOpeningScene.OnPackOpeningLoaded() - failed to load {0}", (object) assetRef));
    }
    else
    {
      this.m_packOpening = go.GetComponent<PackOpening>();
      if (!((Object) this.m_packOpening == (Object) null))
        return;
      Debug.LogError((object) string.Format("PackOpeningScene.OnPackOpeningLoaded() - {0} did not have a {1} component", (object) this.name, (object) typeof (PackOpening)));
    }
  }
}
