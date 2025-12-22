using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using UnityEngine;

public class DebugMissionButton : PegUIElement
{
  public int m_missionId;
  public GameObject m_heroImage;
  public UberText m_name;
  public string m_introline;
  public string m_characterPrefabName;
  private GameObject m_heroPowerObject;
  private bool m_mousedOver;
  private DefLoader.DisposableFullDef m_heroPowerDef;
  private DefLoader.DisposableCardDef m_heroDef;
  private Actor m_heroPowerActor;

  private void Start()
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_missionId);
    if (record == null)
    {
      Error.AddDevWarning("Error", "scenario {0} does not exist in the DBF", (object) this.m_missionId);
    }
    else
    {
      if ((Object) this.m_name != (Object) null)
        this.m_name.Text = (string) record.ShortName;
      string missionHeroCardId = GameUtils.GetMissionHeroCardId(this.m_missionId);
      if (missionHeroCardId == null)
        return;
      DefLoader.Get().LoadCardDef(missionHeroCardId, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnHeroCardDefLoaded));
    }
  }

  protected override void OnDestroy()
  {
    this.m_heroPowerDef?.Dispose();
    this.m_heroPowerDef = (DefLoader.DisposableFullDef) null;
    this.m_heroDef?.Dispose();
    this.m_heroDef = (DefLoader.DisposableCardDef) null;
    base.OnDestroy();
  }

  private void OnHeroCardDefLoaded(
    string cardID,
    DefLoader.DisposableCardDef cardDef,
    object userData)
  {
    this.m_heroDef?.Dispose();
    this.m_heroDef = cardDef;
    RendererExtension.GetMaterial(this.m_heroImage.GetComponent<Renderer>()).mainTexture = this.m_heroDef.CardDef.GetPortraitTexture(TAG_PREMIUM.NORMAL);
  }

  protected override void OnRelease()
  {
    if (!string.IsNullOrEmpty(this.m_introline))
    {
      string legacyAssetName = new AssetReference(this.m_introline).GetLegacyAssetName();
      if (string.IsNullOrEmpty(this.m_characterPrefabName))
        NotificationManager.Get().CreateKTQuote(legacyAssetName, this.m_introline);
      else
        NotificationManager.Get().CreateCharacterQuote(this.m_characterPrefabName, GameStrings.Get(legacyAssetName), this.m_introline);
    }
    base.OnRelease();
    long selectedDeckId = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
    GameMgr.Get().FindGame(GameType.GT_VS_AI, FormatType.FT_WILD, this.m_missionId, deckId: selectedDeckId);
    Object.Destroy((Object) this.gameObject);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    this.m_mousedOver = true;
    base.OnOver(oldState);
    if (string.IsNullOrEmpty(GameUtils.GetMissionHeroPowerCardId(this.m_missionId)))
      return;
    DefLoader.Get().LoadFullDef(GameUtils.GetMissionHeroPowerCardId(this.m_missionId), new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroPowerDefLoaded));
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    this.m_mousedOver = false;
    base.OnOut(oldState);
    if (!(bool) (Object) this.m_heroPowerActor)
      return;
    Object.Destroy((Object) this.m_heroPowerActor.gameObject);
  }

  private void OnHeroPowerDefLoaded(
    string cardID,
    DefLoader.DisposableFullDef def,
    object userData)
  {
    this.m_heroPowerDef?.Dispose();
    this.m_heroPowerDef = def;
    if (!this.m_mousedOver)
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "History_HeroPower_Opponent.prefab:a99d23d6e8630f94b96a8e096fffb16f", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if (!this.m_mousedOver)
      Object.Destroy((Object) go);
    if ((bool) (Object) this.m_heroPowerActor)
      Object.Destroy((Object) this.m_heroPowerActor.gameObject);
    if ((Object) this == (Object) null || (Object) this.gameObject == (Object) null)
      Object.Destroy((Object) go);
    if ((Object) go == (Object) null)
      return;
    this.m_heroPowerActor = go.GetComponent<Actor>();
    go.transform.parent = this.gameObject.transform;
    this.m_heroPowerActor.SetCardDef(this.m_heroPowerDef.DisposableCardDef);
    this.m_heroPowerActor.SetEntityDef(this.m_heroPowerDef.EntityDef);
    this.m_heroPowerActor.UpdateAllComponents();
    go.transform.position = this.transform.position + new Vector3(15f, 0.0f, 0.0f);
    go.transform.localScale = Vector3.one;
    iTween.ScaleTo(go, new Vector3(7f, 7f, 7f), 0.5f);
    LayerUtils.SetLayer(go, GameLayer.Tooltip);
  }
}
