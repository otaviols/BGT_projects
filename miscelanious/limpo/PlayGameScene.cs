using System.Collections;
using UnityEngine;

public abstract class PlayGameScene : PegasusScene
{
  private bool m_deckPickerIsLoaded;
  private AbsDeckPickerTrayDisplay m_deckPickerTrayDisplay;

  protected void Start() => AssetLoader.Get().InstantiatePrefab((AssetReference) this.GetScreenPath(), new PrefabCallback<GameObject>(this.OnUIScreenLoaded));

  protected void Update() => Network.Get().ProcessNetwork();

  public void OnDeckPickerLoaded(AbsDeckPickerTrayDisplay deckPickerTrayDisplay)
  {
    this.m_deckPickerIsLoaded = true;
    this.m_deckPickerTrayDisplay = deckPickerTrayDisplay;
  }

  public abstract string GetScreenPath();

  public override void PreUnload()
  {
    if ((Object) this.m_deckPickerTrayDisplay == (Object) null)
      this.m_deckPickerTrayDisplay = (AbsDeckPickerTrayDisplay) DeckPickerTrayDisplay.Get();
    if (!((Object) this.m_deckPickerTrayDisplay != (Object) null))
      return;
    this.m_deckPickerTrayDisplay.PreUnload();
  }

  public override void Unload()
  {
    if ((Object) this.m_deckPickerTrayDisplay == (Object) null)
      this.m_deckPickerTrayDisplay = (AbsDeckPickerTrayDisplay) DeckPickerTrayDisplay.Get();
    if ((Object) this.m_deckPickerTrayDisplay != (Object) null)
      this.m_deckPickerTrayDisplay.Unload();
    this.m_deckPickerIsLoaded = false;
  }

  private void OnUIScreenLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
      Debug.LogError((object) string.Format("PlayGameScene.OnUIScreenLoaded() - failed to load screen {0}", (object) assetRef));
    else
      this.StartCoroutine(this.WaitForAllToBeLoaded());
  }

  private IEnumerator WaitForAllToBeLoaded()
  {
    while (!this.IsLoaded())
      yield return (object) null;
    SceneMgr.Get().NotifySceneLoaded();
  }

  protected virtual bool IsLoaded() => this.m_deckPickerIsLoaded;
}
