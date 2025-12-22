using System;
using UnityEngine;

[CustomEditClass]
public class AdventureSubSceneDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "UI")]
  public float m_BigCardScale = 1f;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_BossPowerBone;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_HeroPowerBigCardBone;
  protected Actor m_BossActor;
  protected Actor m_HeroPowerActor;
  protected Actor m_BossPowerBigCard;
  protected Actor m_HeroPowerBigCard;
  protected DefLoader.DisposableFullDef m_CurrentBossHeroPowerFullDef;
  protected Vector3 m_BossPowerTweenOrigin;
  private AssetLoadingHelper m_assetLoadingHelper;

  protected virtual void OnDestroy()
  {
    this.m_CurrentBossHeroPowerFullDef?.Dispose();
    this.m_CurrentBossHeroPowerFullDef = (DefLoader.DisposableFullDef) null;
  }

  protected AssetLoadingHelper AssetLoadingHelper
  {
    get
    {
      if (this.m_assetLoadingHelper == null)
      {
        this.m_assetLoadingHelper = new AssetLoadingHelper();
        this.m_assetLoadingHelper.AssetLoadingComplete += new EventHandler(this.OnAssetsLoaded);
      }
      return this.m_assetLoadingHelper;
    }
  }

  private void OnAssetsLoaded(object sender, EventArgs args) => this.OnSubSceneLoaded();

  public static Actor OnActorLoaded(
    string actorName,
    GameObject actorObject,
    GameObject container,
    bool withRotation = false)
  {
    Actor component = actorObject.GetComponent<Actor>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) container != (UnityEngine.Object) null)
      {
        GameUtils.SetParent((Component) component, container, withRotation);
        LayerUtils.SetLayer((Component) component, container.layer);
      }
      component.SetUnlit();
      component.Hide();
    }
    else
      Debug.LogWarning((object) string.Format("ERROR actor \"{0}\" has no Actor component", (object) actorName));
    return component;
  }

  protected bool AddAssetToLoad(int assetCount = 1)
  {
    if (this.IsSubsceneLoaded())
      return false;
    this.AssetLoadingHelper.AddAssetToLoad(assetCount);
    return true;
  }

  protected void AssetLoadCompleted()
  {
    if (this.IsSubsceneLoaded())
      return;
    this.AssetLoadingHelper.AssetLoadCompleted();
  }

  protected virtual void OnSubSceneLoaded()
  {
    AdventureSubScene component = this.GetComponent<AdventureSubScene>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.AddSubSceneTransitionFinishedListener(new AdventureSubScene.SubSceneTransitionFinished(this.OnSubSceneTransitionComplete));
    component.SetIsLoaded(true);
  }

  private bool IsSubsceneLoaded()
  {
    AdventureSubScene component = this.GetComponent<AdventureSubScene>();
    return (UnityEngine.Object) component != (UnityEngine.Object) null && component.IsLoaded();
  }

  protected virtual void OnSubSceneTransitionComplete()
  {
    AdventureSubScene component = this.GetComponent<AdventureSubScene>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.RemoveSubSceneTransitionFinishedListener(new AdventureSubScene.SubSceneTransitionFinished(this.OnSubSceneTransitionComplete));
  }

  protected void ShowBossPowerBigCard()
  {
    Vector3? origin = new Vector3?();
    if ((UnityEngine.Object) this.m_HeroPowerActor != (UnityEngine.Object) null)
      origin = new Vector3?(this.m_HeroPowerActor.gameObject.transform.position);
    BigCardHelper.ShowBigCard(this.m_BossPowerBigCard, this.m_CurrentBossHeroPowerFullDef, this.m_HeroPowerBigCardBone, this.m_BigCardScale, origin);
  }

  protected void HideBossPowerBigCard() => BigCardHelper.HideBigCard(this.m_BossPowerBigCard);
}
