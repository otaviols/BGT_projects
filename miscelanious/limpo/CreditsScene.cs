using UnityEngine;

public class CreditsScene : PegasusScene
{
  private bool m_unloading;

  protected override void Awake()
  {
    base.Awake();
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Credits.prefab:4ffef537c5070494eb038d15271a6ebe", new PrefabCallback<GameObject>(this.OnUIScreenLoaded));
    if (InactivePlayerKicker.Get() == null)
      return;
    InactivePlayerKicker.Get().SetShouldCheckForInactivity(false);
  }

  public override bool IsUnloading() => this.m_unloading;

  public override void Unload()
  {
    this.m_unloading = true;
    if (InactivePlayerKicker.Get() != null)
      InactivePlayerKicker.Get().SetShouldCheckForInactivity(true);
    this.m_unloading = false;
  }

  private void OnUIScreenLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if (!((Object) go == (Object) null))
      return;
    Debug.LogError((object) string.Format("CreditsScene.OnUIScreenLoaded() - failed to load screen {0}", (object) assetRef));
  }
}
