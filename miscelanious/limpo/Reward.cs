using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reward : MonoBehaviour
{
  public GameObject m_root;
  public bool m_showBanner = true;
  public bool m_playSounds = true;
  public RewardBanner m_rewardBannerPrefab;
  public GameObject m_rewardBannerBone;
  public PegUIElement m_clickCatcher;
  public GameObject m_MeshRoot;
  public Animator m_EchoAnimator;
  public float m_EchoHideMeshDelay = 0.65f;
  public RewardBanner m_rewardBanner;
  public ScreenEffectsHandle ScreenEffectsHandle;
  private RewardData m_data;
  private Reward.Type m_type;
  private bool m_ready = true;
  protected bool m_shown;
  private List<Reward.OnClickedListener> m_clickListeners = new List<Reward.OnClickedListener>();
  private List<Reward.OnHideListener> m_hideListeners = new List<Reward.OnHideListener>();

  protected virtual void Awake()
  {
    this.UpdateBannerObject();
    this.EnableClickCatcher(false);
    SoundManager.Get().Load((AssetReference) "game_end_reward.prefab:6c28275a79f151a478d49afc04533e72");
    this.ScreenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected virtual void Start()
  {
    if ((Object) this.m_clickCatcher != (Object) null)
      this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickReleased));
    this.Hide();
  }

  protected virtual void OnDestroy()
  {
  }

  public Reward.Type RewardType => this.Data.RewardType;

  public RewardData Data => this.m_data;

  public bool IsShown => this.m_shown;

  protected virtual RewardBanner RewardBannerPrefab => this.m_rewardBannerPrefab;

  public void Show(bool updateCacheValues)
  {
    this.Data.AcknowledgeNotices();
    if ((Object) this.m_MeshRoot != (Object) null)
      this.m_MeshRoot.SetActive(true);
    if (this.m_showBanner && (Object) this.m_rewardBanner != (Object) null)
    {
      this.m_rewardBanner.gameObject.SetActive(true);
    }
    else
    {
      if ((Object) this.m_rewardBannerBone != (Object) null)
        this.m_rewardBannerBone.SetActive(false);
      if ((Object) this.m_rewardBanner != (Object) null)
        this.m_rewardBanner.gameObject.SetActive(false);
    }
    if (this.m_playSounds)
      this.PlayShowSounds();
    this.ShowReward(updateCacheValues);
    this.m_shown = true;
  }

  protected virtual void PlayShowSounds()
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Quest_Complete_Jingle.prefab:4b1a4bf5fece033469acee1944305ab1");
    SoundManager.Get().LoadAndPlay((AssetReference) "quest_complete_pop_up.prefab:888f073a3b5d3e8418c2f989f3991bf7");
    SoundManager.Get().LoadAndPlay((AssetReference) "tavern_crowd_play_reaction_positive_random.prefab:708bd64f76a706a45956e5566429c6c6");
  }

  public void HideWithFX() => this.StartCoroutine(this.HideFXAnimation());

  private IEnumerator HideFXAnimation()
  {
    Reward reward = this;
    if ((bool) (Object) reward.m_EchoAnimator)
    {
      reward.m_EchoAnimator.enabled = true;
      yield return (object) new WaitForSeconds(reward.m_EchoHideMeshDelay);
      if ((Object) reward.m_MeshRoot != (Object) null)
        reward.m_MeshRoot.SetActive(false);
    }
    iTween.FadeTo(reward.gameObject, 0.0f, RewardUtils.RewardHideTime);
  }

  public virtual void Hide(bool animate = false)
  {
    if (!animate)
    {
      this.OnHideAnimateComplete();
    }
    else
    {
      iTween.FadeTo(this.gameObject, 0.0f, RewardUtils.RewardHideTime);
      iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) RewardUtils.RewardHiddenScale, (object) "time", (object) RewardUtils.RewardHideTime, (object) "oncomplete", (object) "OnHideAnimateComplete", (object) "oncompletetarget", (object) this.gameObject));
    }
  }

  private void OnHideAnimateComplete()
  {
    this.HideReward();
    this.m_shown = false;
  }

  public void SetData(RewardData data, bool updateVisuals)
  {
    this.m_data = data;
    this.OnDataSet(updateVisuals);
  }

  public void NotifyLoadedWhenReady(
    Reward.LoadRewardCallbackData loadRewardCallbackData)
  {
    this.StartCoroutine(this.WaitThenNotifyLoaded(loadRewardCallbackData));
  }

  public void EnableClickCatcher(bool enabled)
  {
    if (!((Object) this.m_clickCatcher != (Object) null))
      return;
    this.m_clickCatcher.gameObject.SetActive(enabled);
  }

  public bool RegisterClickListener(Reward.OnClickedCallback callback) => this.RegisterClickListener(callback, (object) null);

  public bool RegisterClickListener(Reward.OnClickedCallback callback, object userData)
  {
    Reward.OnClickedListener onClickedListener = new Reward.OnClickedListener();
    onClickedListener.SetCallback(callback);
    onClickedListener.SetUserData(userData);
    if (this.m_clickListeners.Contains(onClickedListener))
      return false;
    this.m_clickListeners.Add(onClickedListener);
    return true;
  }

  public bool RemoveClickListener(Reward.OnClickedCallback callback) => this.RemoveClickListener(callback, (object) null);

  public bool RemoveClickListener(Reward.OnClickedCallback callback, object userData)
  {
    Reward.OnClickedListener onClickedListener = new Reward.OnClickedListener();
    onClickedListener.SetCallback(callback);
    onClickedListener.SetUserData(userData);
    return this.m_clickListeners.Remove(onClickedListener);
  }

  public bool RegisterHideListener(Reward.OnHideCallback callback) => this.RegisterHideListener(callback, (object) null);

  public bool RegisterHideListener(Reward.OnHideCallback callback, object userData)
  {
    Reward.OnHideListener onHideListener = new Reward.OnHideListener();
    onHideListener.SetCallback(callback);
    onHideListener.SetUserData(userData);
    if (this.m_hideListeners.Contains(onHideListener))
      return false;
    this.m_hideListeners.Add(onHideListener);
    return true;
  }

  public void RemoveHideListener(Reward.OnHideCallback callback, object userData)
  {
    Reward.OnHideListener onHideListener = new Reward.OnHideListener();
    onHideListener.SetCallback(callback);
    onHideListener.SetUserData(userData);
    this.m_hideListeners.Remove(onHideListener);
  }

  protected abstract void InitData();

  protected virtual void ShowReward(bool updateCacheValues)
  {
  }

  protected virtual void OnDataSet(bool updateVisuals)
  {
  }

  protected virtual void HideReward() => this.OnHide();

  protected Reward() => this.InitData();

  protected void SetReady(bool ready) => this.m_ready = ready;

  protected void SetRewardText(string headline, string details, string source)
  {
    if ((bool) UniversalInputManager.UsePhoneUI && this.RewardType != Reward.Type.GOLD && this.RewardType != Reward.Type.CARD)
      details = "";
    if (!((Object) this.m_rewardBanner != (Object) null))
      return;
    this.m_rewardBanner.SetText(headline, details, source);
  }

  private IEnumerator WaitThenNotifyLoaded(
    Reward.LoadRewardCallbackData loadRewardCallbackData)
  {
    Reward reward = this;
    if (loadRewardCallbackData.m_callback != null)
    {
      while (!reward.m_ready)
        yield return (object) null;
      loadRewardCallbackData.m_callback(reward, loadRewardCallbackData.m_callbackData);
    }
  }

  private void OnClickReleased(UIEvent e)
  {
    foreach (Reward.OnClickedListener onClickedListener in this.m_clickListeners.ToArray())
      onClickedListener.Fire(this);
  }

  private void OnHide()
  {
    foreach (Reward.OnHideListener onHideListener in this.m_hideListeners.ToArray())
      onHideListener.Fire();
  }

  protected void UpdateBannerObject()
  {
    if ((Object) this.m_rewardBanner != (Object) null)
      Object.Destroy((Object) this.m_rewardBanner.gameObject);
    RewardBanner rewardBannerPrefab = this.RewardBannerPrefab;
    if (!this.m_showBanner || !((Object) rewardBannerPrefab != (Object) null))
      return;
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_rewardBanner = Object.Instantiate<RewardBanner>(rewardBannerPrefab);
      this.m_rewardBanner.gameObject.SetActive(false);
      this.m_rewardBanner.transform.parent = this.m_rewardBannerBone.transform;
      this.m_rewardBanner.transform.localPosition = Vector3.zero;
    }
    else
      this.m_rewardBanner = (RewardBanner) GameUtils.Instantiate((Component) rewardBannerPrefab, this.m_rewardBannerBone);
  }

  public enum Type
  {
    NONE = -1, // 0xFFFFFFFF
    ARCANE_DUST = 0,
    BOOSTER_PACK = 1,
    CARD = 2,
    CARD_BACK = 3,
    CRAFTABLE_CARD = 4,
    FORGE_TICKET = 5,
    GOLD = 6,
    MOUNT = 7,
    CLASS_CHALLENGE = 8,
    EVENT = 9,
    RANDOM_CARD = 10, // 0x0000000A
    BONUS_CHALLENGE = 11, // 0x0000000B
    ADVENTURE_DECK = 12, // 0x0000000C
    ADVENTURE_HERO_POWER = 13, // 0x0000000D
    ARCANE_ORBS = 14, // 0x0000000E
    DECK = 15, // 0x0000000F
    MINI_SET = 16, // 0x00000010
    MERCENARY_COIN = 17, // 0x00000011
    MERCENARY_EXP = 18, // 0x00000012
    MERCENARY_ABILITY_UNLOCK = 19, // 0x00000013
    MERCENARY_EQUIPMENT = 20, // 0x00000014
    REWARD_ITEM = 21, // 0x00000015
    MERCENARY_BOOSTER = 22, // 0x00000016
    MERCENARY_MERCENARY = 23, // 0x00000017
    MERCENARY_RANDOM_MERCENARY = 24, // 0x00000018
    MERCENARY_KNOCKOUT = 25, // 0x00000019
    BATTLEGROUNDS_GUIDE_SKIN = 26, // 0x0000001A
    BATTLEGROUNDS_HERO_SKIN = 27, // 0x0000001B
    BATTLEGROUNDS_FINISHER = 28, // 0x0000001C
    BATTLEGROUNDS_BOARD_SKIN = 29, // 0x0000001D
    BATTLEGROUNDS_EMOTE = 30, // 0x0000001E
    MERCENARY_RENOWN = 31, // 0x0000001F
  }

  public delegate void DelOnRewardLoaded(Reward reward, object callbackData);

  public class LoadRewardCallbackData
  {
    public Reward.DelOnRewardLoaded m_callback;
    public object m_callbackData;
  }

  public delegate void OnClickedCallback(Reward reward, object userData);

  private class OnClickedListener : EventListener<Reward.OnClickedCallback>
  {
    public void Fire(Reward reward) => this.m_callback(reward, this.m_userData);
  }

  public delegate void OnHideCallback(object userData);

  private class OnHideListener : EventListener<Reward.OnHideCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }
}
