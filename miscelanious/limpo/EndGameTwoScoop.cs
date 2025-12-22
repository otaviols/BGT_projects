using System.Collections;
using UnityEngine;

public class EndGameTwoScoop : MonoBehaviour
{
  public UberText m_bannerLabel;
  public GameObject m_heroBone;
  public Actor m_heroActor;
  public HeroXPBar m_xpBarPrefab;
  public GameObject m_levelUpTier1;
  public GameObject m_levelUpTier2;
  public GameObject m_levelUpTier3;
  protected bool m_heroActorLoaded;
  protected HeroXPBar m_xpBar;
  private bool m_isShown;
  private static readonly float AFTER_PUNCH_SCALE_VAL = 2.3f;
  protected static readonly float START_SCALE_VAL = 0.01f;
  protected static readonly float END_SCALE_VAL = 2.5f;
  protected static readonly Vector3 START_POSITION = new Vector3(-7.8f, 8.2f, -5f);
  protected static readonly float BAR_ANIMATION_DELAY = 1f;

  public virtual void Awake()
  {
    this.gameObject.SetActive(false);
    AssetLoader.Get().InstantiatePrefab((AssetReference) this.GetActorName(), new PrefabCallback<GameObject>(this.OnHeroActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  public virtual void OnDestroy()
  {
  }

  private void Start()
  {
    LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.ResetPositions();
  }

  public bool IsShown() => this.m_isShown;

  public void Show(bool showXPBar = true)
  {
    this.m_isShown = true;
    this.gameObject.SetActive(true);
    this.ShowImpl();
    if (!showXPBar || GameMgr.Get().IsTraditionalTutorial() || GameMgr.Get().IsSpectator())
      return;
    NetCache.HeroLevel heroLevel = (NetCache.HeroLevel) null;
    int totalLevel = 0;
    Entity startingHero = GameState.Get().GetFriendlySidePlayer().GetStartingHero();
    if (startingHero != null)
    {
      heroLevel = GameUtils.GetHeroLevel(startingHero.GetClass());
      totalLevel = GameUtils.GetTotalHeroLevel() ?? 0;
    }
    if (heroLevel == null)
    {
      this.HideXpBar();
    }
    else
    {
      if (!((Object) this.m_xpBarPrefab != (Object) null))
        return;
      this.m_xpBar = Object.Instantiate<HeroXPBar>(this.m_xpBarPrefab);
      if ((Object) this.m_heroActor.m_xpBarRootObject != (Object) null)
      {
        this.m_xpBar.transform.parent = this.m_heroActor.m_xpBarRootObject.transform;
        this.m_xpBar.transform.localScale = Vector3.one;
        this.m_xpBar.transform.localPosition = Vector3.zero;
      }
      else
      {
        this.m_xpBar.transform.parent = this.m_heroActor.transform;
        this.m_xpBar.transform.localScale = new Vector3(0.9064f, 0.9064f, 0.9064f);
        this.m_xpBar.transform.localPosition = new Vector3(-0.166f, 0.224f, -0.738f);
      }
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      this.m_xpBar.m_soloLevelLimit = netObject == null ? 60 : netObject.XPSoloLimit;
      this.m_xpBar.m_isAnimated = true;
      this.m_xpBar.m_delay = EndGameTwoScoop.BAR_ANIMATION_DELAY;
      this.m_xpBar.m_levelUpCallback = new HeroXPBar.PlayLevelUpEffectCallback(this.PlayLevelUpEffect);
      this.m_xpBar.UpdateDisplay(heroLevel, totalLevel);
    }
  }

  public void Hide() => this.HideAll();

  public virtual bool IsLoaded() => this.m_heroActorLoaded;

  public void HideXpBar()
  {
    if (!((Object) this.m_xpBar != (Object) null))
      return;
    this.m_xpBar.gameObject.SetActive(false);
  }

  public virtual void StopAnimating()
  {
  }

  protected virtual string GetActorName() => "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d";

  protected virtual void ShowImpl()
  {
  }

  protected virtual void ResetPositions()
  {
  }

  protected void SetBannerLabel(string label) => this.m_bannerLabel.Text = label;

  protected void EnableBannerLabel(bool enable) => this.m_bannerLabel.gameObject.SetActive(enable);

  protected void PunchEndGameTwoScoop()
  {
    if ((Object) EndGameScreen.Get() != (Object) null)
      EndGameScreen.Get().SetPlayingBlockingAnim(false);
    iTween.ScaleTo(this.gameObject, new Vector3(EndGameTwoScoop.AFTER_PUNCH_SCALE_VAL, EndGameTwoScoop.AFTER_PUNCH_SCALE_VAL, EndGameTwoScoop.AFTER_PUNCH_SCALE_VAL), 0.15f);
  }

  private void HideAll()
  {
    ScreenEffectsMgr.Get().SetActive(false);
    Hashtable args = iTween.Hash((object) "scale", (object) new Vector3(EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL), (object) "time", (object) 0.25f, (object) "oncomplete", (object) "OnAllHidden", (object) "oncompletetarget", (object) this.gameObject);
    iTween.FadeTo(this.gameObject, 0.0f, 0.25f);
    iTween.ScaleTo(this.gameObject, args);
    this.m_isShown = false;
  }

  private void OnAllHidden()
  {
    iTween.FadeTo(this.gameObject, 0.0f, 0.0f);
    this.gameObject.SetActive(false);
    this.ResetPositions();
  }

  private void OnHeroActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    go.transform.parent = this.transform;
    go.transform.localPosition = this.m_heroBone.transform.localPosition;
    go.transform.localScale = this.m_heroBone.transform.localScale;
    this.m_heroActor = go.GetComponent<Actor>();
    this.m_heroActor.TurnOffCollider();
    this.m_heroActor.m_healthObject.SetActive(false);
    this.m_heroActorLoaded = true;
    Card heroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    if ((Object) heroCard != (Object) null)
      this.m_heroActor.SetPremium(heroCard.GetPremium());
    this.m_heroActor.UpdateAllComponents();
  }

  protected void PlayLevelUpEffect()
  {
    GameObject gameObject = Object.Instantiate<GameObject>(this.m_levelUpTier1);
    if (!(bool) (Object) gameObject)
      return;
    gameObject.transform.parent = this.transform;
    gameObject.GetComponent<PlayMakerFSM>().SendEvent("Birth");
  }
}
