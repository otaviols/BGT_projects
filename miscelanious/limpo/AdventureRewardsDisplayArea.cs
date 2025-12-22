using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureRewardsDisplayArea : MonoBehaviour
{
  private const float BlurVignetteTime = 0.25f;
  [CustomEditField(Sections = "UI")]
  public GameObject m_RewardsCardArea;
  [CustomEditField(Sections = "UI")]
  public Vector3 m_RewardsDefaultOffset;
  [CustomEditField(Sections = "UI")]
  public Vector3 m_RewardsHeroSkinOffset;
  [CustomEditField(Sections = "UI")]
  public float m_RewardsCardMouseOffset;
  [CustomEditField(Sections = "UI")]
  public Vector3 m_RewardsCardScale;
  [CustomEditField(Sections = "UI")]
  public Vector3 m_RewardsHeroSkinScale;
  [CustomEditField(Sections = "UI")]
  public Vector3 m_RewardsCardBackScale;
  [CustomEditField(Sections = "UI")]
  public Vector3 m_RewardsBoosterScale;
  [CustomEditField(Sections = "UI")]
  public float m_RewardsDefaultSpacing = 10f;
  [CustomEditField(Sections = "UI")]
  public Vector3 m_RewardsCardDriftAmount;
  [CustomEditField(Sections = "UI")]
  public bool m_EnableFullscreenMode;
  [CustomEditField(Parent = "m_EnableFullscreenMode", Sections = "UI")]
  public PegUIElement m_FullscreenModeOffClicker;
  [CustomEditField(Parent = "m_EnableFullscreenMode", Sections = "UI")]
  public UIBScrollable m_FullscreenDisableScrollBar;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_CardPreviewAppearSound;
  private List<GameObject> m_CurrentRewards = new List<GameObject>();
  private bool m_FullscreenEnabled;
  private bool m_Showing;
  private List<AdventureRewardsDisplayArea.RewardsHidden> m_RewardsHiddenListeners = new List<AdventureRewardsDisplayArea.RewardsHidden>();
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    if ((UnityEngine.Object) this.m_FullscreenModeOffClicker != (UnityEngine.Object) null)
      this.m_FullscreenModeOffClicker.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.HideRewards()));
    if ((UnityEngine.Object) this.m_FullscreenDisableScrollBar != (UnityEngine.Object) null)
      this.m_FullscreenDisableScrollBar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.HideRewards));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy() => this.DisableFullscreen();

  public bool IsShowing() => this.m_Showing;

  public void ShowRewardsNoFullscreen(
    List<RewardData> rewards,
    Vector3 finalPosition,
    Vector3? origin = null)
  {
    this.DoShowRewards((ICollection<RewardData>) rewards, new Vector3?(finalPosition), origin, true);
  }

  public void ShowRewards(List<RewardData> rewards, Vector3 finalPosition, Vector3? origin = null)
  {
    if (this.m_Showing)
      return;
    this.m_Showing = true;
    if (this.m_EnableFullscreenMode)
      this.DoShowRewards((ICollection<RewardData>) rewards, new Vector3?(), origin, false);
    else
      this.DoShowRewards((ICollection<RewardData>) rewards, new Vector3?(finalPosition), origin, false);
  }

  public void HideRewards()
  {
    this.m_Showing = false;
    foreach (GameObject currentReward in this.m_CurrentRewards)
    {
      if ((UnityEngine.Object) currentReward != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) currentReward);
    }
    this.m_CurrentRewards.Clear();
    this.DisableFullscreen();
    this.FireRewardsHiddenEvent();
  }

  public void AddRewardsHiddenListener(AdventureRewardsDisplayArea.RewardsHidden dlg) => this.m_RewardsHiddenListeners.Add(dlg);

  public void RemoveRewardsHiddenListener(AdventureRewardsDisplayArea.RewardsHidden dlg) => this.m_RewardsHiddenListeners.Remove(dlg);

  public List<GameObject> GetCurrentShownRewards() => !this.m_Showing ? (List<GameObject>) null : this.m_CurrentRewards;

  private void FireRewardsHiddenEvent()
  {
    foreach (AdventureRewardsDisplayArea.RewardsHidden rewardsHidden in this.m_RewardsHiddenListeners.ToArray())
      rewardsHidden();
  }

  private void DoShowRewards(
    ICollection<RewardData> rewards,
    Vector3? finalPosition,
    Vector3? origin,
    bool disableFullscreen)
  {
    int index = 0;
    int count = rewards.Count;
    Vector3 positionOffset = this.m_RewardsDefaultOffset;
    Vector3 scale = Vector3.one;
    foreach (RewardData reward in (IEnumerable<RewardData>) rewards)
    {
      GameObject child = (GameObject) null;
      switch (reward.RewardType)
      {
        case Reward.Type.BOOSTER_PACK:
          BoosterDbfRecord record = GameDbf.Booster.GetRecord(((BoosterPackRewardData) reward).Id);
          if (record != null)
          {
            child = AssetLoader.Get().InstantiatePrefab((AssetReference) record.PackOpeningPrefab, AssetLoadingOptions.IgnorePrefabPosition);
            scale = this.m_RewardsBoosterScale;
            UnopenedPack component = child.GetComponent<UnopenedPack>();
            component.SetBoosterId(((BoosterPackRewardData) reward).Id);
            component.SetCount(((BoosterPackRewardData) reward).Count);
            component.GetComponent<Collider>().enabled = false;
            break;
          }
          continue;
        case Reward.Type.CARD:
          string cardId = ((CardRewardData) reward).CardID;
          using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardId))
          {
            int num = fullDef.EntityDef.IsHeroSkin() ? 1 : 0;
            string assetRef = num != 0 ? ActorNames.GetHeroSkinOrHandActor(fullDef.EntityDef, TAG_PREMIUM.NORMAL) : ActorNames.GetHandActor(fullDef.EntityDef, TAG_PREMIUM.NORMAL);
            child = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, AssetLoadingOptions.IgnorePrefabPosition);
            child.GetComponentInChildren<Collider>().enabled = false;
            Actor component1 = child.GetComponent<Actor>();
            component1.SetFullDef(fullDef);
            component1.CreateBannedRibbon();
            if (num != 0)
            {
              child.GetComponent<CollectionHeroSkin>().SetClass(fullDef.EntityDef.GetClass());
              scale = this.m_RewardsHeroSkinScale;
              positionOffset = this.m_RewardsHeroSkinOffset;
            }
            else
            {
              scale = this.m_RewardsCardScale;
              positionOffset = this.m_RewardsDefaultOffset;
            }
            if ((UnityEngine.Object) component1.m_cardMesh != (UnityEngine.Object) null)
            {
              BoxCollider component2 = component1.m_cardMesh.GetComponent<BoxCollider>();
              if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
              {
                component2.enabled = false;
                break;
              }
              break;
            }
            break;
          }
        case Reward.Type.CARD_BACK:
          CardBackManager.LoadCardBackData loadCardBackData = CardBackManager.Get().LoadCardBackByIndex(((CardBackRewardData) reward).CardBackID);
          scale = this.m_RewardsCardBackScale;
          child = loadCardBackData.m_GameObject;
          break;
        case Reward.Type.RANDOM_CARD:
          child = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Random_Reward.prefab:403211800142ebf4593a290b92655167", AssetLoadingOptions.IgnorePrefabPosition);
          scale = this.m_RewardsCardScale;
          break;
      }
      if (!((UnityEngine.Object) child == (UnityEngine.Object) null))
      {
        this.m_CurrentRewards.Add(child);
        GameUtils.SetParent(child, this.m_RewardsCardArea);
        this.ShowRewardsObject(child, finalPosition, origin, positionOffset, scale, index, count);
        ++index;
      }
    }
    this.EnableFullscreen(disableFullscreen);
  }

  private void ShowRewardsObject(
    GameObject obj,
    Vector3? finalPosition,
    Vector3? origin,
    Vector3 positionOffset,
    Vector3 scale,
    int index,
    int totalCount)
  {
    Vector3 vector3;
    if (finalPosition.HasValue)
    {
      Collider component = this.GetComponent<Collider>();
      Vector3 min = component.bounds.min;
      Vector3 max = component.bounds.max;
      vector3 = finalPosition.Value + positionOffset;
      float num = (float) index * this.m_RewardsDefaultSpacing;
      vector3.z = Mathf.Clamp(vector3.z, min.z, max.z);
      if ((double) vector3.x + (double) this.m_RewardsCardMouseOffset > (double) max.x)
        vector3.x -= this.m_RewardsCardMouseOffset + num;
      else
        vector3.x += this.m_RewardsCardMouseOffset + num;
    }
    else
    {
      vector3 = this.m_RewardsCardArea.transform.position + positionOffset;
      float num = (float) index * this.m_RewardsDefaultSpacing;
      vector3.x += num;
      vector3.x -= (float) ((double) (totalCount - 1) * (double) this.m_RewardsDefaultSpacing * 0.5);
    }
    obj.transform.localScale = scale;
    obj.transform.position = vector3;
    obj.SetActive(true);
    if (this.m_EnableFullscreenMode)
    {
      LayerUtils.SetLayer(obj, GameLayer.IgnoreFullScreenEffects);
      if ((UnityEngine.Object) this.m_FullscreenModeOffClicker != (UnityEngine.Object) null)
        LayerUtils.SetLayer((Component) this.m_FullscreenModeOffClicker, GameLayer.IgnoreFullScreenEffects);
    }
    iTween.StopByName(obj, "REWARD_SCALE_UP");
    iTween.ScaleFrom(obj, iTween.Hash((object) nameof (scale), (object) (Vector3.one * 0.05f), (object) "time", (object) 0.15f, (object) "easeType", (object) iTween.EaseType.easeOutQuart, (object) "name", (object) "REWARD_SCALE_UP"));
    if (origin.HasValue)
    {
      iTween.StopByName(obj, "REWARD_MOVE_FROM_ORIGIN");
      iTween.MoveFrom(obj, iTween.Hash((object) "position", (object) origin.Value, (object) "time", (object) 0.15f, (object) "easeType", (object) iTween.EaseType.easeOutQuart, (object) "name", (object) "REWARD_MOVE_FROM_ORIGIN", (object) "oncomplete", (object) (Action<object>) (o =>
      {
        if (!(this.m_RewardsCardDriftAmount != Vector3.zero))
          return;
        AnimationUtil.DriftObject(obj, this.m_RewardsCardDriftAmount);
      })));
    }
    else if (this.m_RewardsCardDriftAmount != Vector3.zero)
      AnimationUtil.DriftObject(obj, this.m_RewardsCardDriftAmount);
    if (string.IsNullOrEmpty(this.m_CardPreviewAppearSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_CardPreviewAppearSound);
  }

  private void EnableFullscreen(bool disableFullscreen)
  {
    if (!this.m_EnableFullscreenMode || disableFullscreen)
      return;
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = 0.25f
    });
    if ((UnityEngine.Object) this.m_FullscreenModeOffClicker != (UnityEngine.Object) null)
      this.m_FullscreenModeOffClicker.gameObject.SetActive(true);
    this.m_FullscreenEnabled = true;
  }

  private void DisableFullscreen()
  {
    if (!this.m_FullscreenEnabled)
      return;
    if (FullScreenFXMgr.Get() != null)
      this.m_screenEffectsHandle.StopEffect();
    if ((UnityEngine.Object) this.m_FullscreenModeOffClicker != (UnityEngine.Object) null)
      this.m_FullscreenModeOffClicker.gameObject.SetActive(false);
    this.m_FullscreenEnabled = false;
  }

  public delegate void RewardsHidden();
}
