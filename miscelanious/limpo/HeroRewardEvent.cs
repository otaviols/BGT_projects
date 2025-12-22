using Assets;
using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class HeroRewardEvent : MonoBehaviour
{
  public PlayMakerFSM m_playmaker;
  public Transform m_heroBone;
  public GameObject m_burningHero;
  private VictoryTwoScoop m_victoryTwoScoop;
  private List<HeroRewardEvent.AnimationDoneListener> m_animationDoneListeners = new List<HeroRewardEvent.AnimationDoneListener>();
  private DefLoader.DisposableCardDef m_VanillaHeroCardDef;
  private DefLoader.DisposableCardDef m_PremiumHeroCardDef;
  private Achievement m_RewardAchieve;
  private QuestToast.DelOnCloseQuestToast m_ContinueCallback;

  private void OnDestroy()
  {
    this.m_VanillaHeroCardDef?.Dispose();
    this.m_PremiumHeroCardDef?.Dispose();
  }

  public void Show()
  {
    this.gameObject.SetActive(true);
    this.m_playmaker.SendEvent("Action");
    this.m_victoryTwoScoop.HideXpBar();
    this.m_victoryTwoScoop.m_bannerLabel.Text = "";
  }

  public void Hide()
  {
    this.m_playmaker.SendEvent("Done");
    SoundManager.Get().LoadAndPlay((AssetReference) "rank_window_shrink.prefab:9c6393a1d207a07439c22f31ef405a7c");
  }

  public void SetHeroBurnAwayTexture(Texture heroTexture) => RendererExtension.GetMaterial(this.m_burningHero.GetComponent<Renderer>()).mainTexture = heroTexture;

  public void HideTwoScoop() => this.m_victoryTwoScoop.Hide();

  public void HideHeroActor() => this.m_victoryTwoScoop.m_heroActor.Hide();

  public void SetVictoryTwoScoop(VictoryTwoScoop twoScoop) => this.m_victoryTwoScoop = twoScoop;

  public void SetRewardAchieve(
    Achievement achieve,
    QuestToast.DelOnCloseQuestToast continueCallback)
  {
    this.m_RewardAchieve = achieve;
    this.m_ContinueCallback = continueCallback;
  }

  public void SwapHeroToVanilla()
  {
    if (this.m_VanillaHeroCardDef == null)
      return;
    this.m_victoryTwoScoop.m_heroActor.SetCardDef(this.m_VanillaHeroCardDef);
    this.m_victoryTwoScoop.m_heroActor.UpdateAllComponents();
  }

  public void SwapHeroToGoldenVanilla()
  {
    if (this.m_VanillaHeroCardDef == null)
      return;
    this.m_victoryTwoScoop.m_heroActor.SetCardDef(this.m_VanillaHeroCardDef);
    this.m_victoryTwoScoop.m_heroActor.SetPremium(TAG_PREMIUM.GOLDEN);
    this.m_victoryTwoScoop.m_heroActor.UpdateAllComponents();
  }

  public void SwapMaterialToPremium()
  {
    this.m_victoryTwoScoop.m_heroActor.SetPremium(TAG_PREMIUM.GOLDEN);
    this.m_victoryTwoScoop.m_heroActor.UpdateAllComponents();
  }

  public void SwapHeroToPremium()
  {
    this.m_victoryTwoScoop.m_heroActor.SetCardDef(this.m_PremiumHeroCardDef);
    this.m_victoryTwoScoop.m_heroActor.UpdateAllComponents();
  }

  public void ShowHeroRewardBanner()
  {
    if (this.m_RewardAchieve == null)
      return;
    QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, this.m_ContinueCallback, false, this.m_RewardAchieve);
  }

  public void AnimationDone() => this.FireAnimationDoneEvent();

  private void FireAnimationDoneEvent()
  {
    foreach (HeroRewardEvent.AnimationDoneListener animationDoneListener in this.m_animationDoneListeners.ToArray())
      animationDoneListener();
  }

  public void RegisterAnimationDoneListener(HeroRewardEvent.AnimationDoneListener listener)
  {
    if (this.m_animationDoneListeners.Contains(listener))
      return;
    this.m_animationDoneListeners.Add(listener);
  }

  public void RemoveAnimationDoneListener(HeroRewardEvent.AnimationDoneListener listener) => this.m_animationDoneListeners.Remove(listener);

  public void LoadHeroCardDefs(string heroCardId)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(heroCardId);
    string heroCardId1 = CollectionManager.GetHeroCardId(entityDef.GetClass(), CardHero.HeroType.HONORED);
    CardPortraitQuality quality1 = new CardPortraitQuality(3, TAG_PREMIUM.GOLDEN);
    DefLoader.Get().LoadCardDef(heroCardId1, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnPremiumHeroCardDefLoaded), quality: quality1);
    string vanillaHero = CollectionManager.GetVanillaHero(entityDef.GetClass());
    CardPortraitQuality quality2 = new CardPortraitQuality(3, TAG_PREMIUM.NORMAL);
    DefLoader.Get().LoadCardDef(vanillaHero, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnVanillaHeroCardDefLoaded), quality: quality2);
  }

  private void OnVanillaHeroCardDefLoaded(
    string cardId,
    DefLoader.DisposableCardDef def,
    object userData)
  {
    this.m_VanillaHeroCardDef?.Dispose();
    this.m_VanillaHeroCardDef = def;
  }

  private void OnPremiumHeroCardDefLoaded(
    string cardId,
    DefLoader.DisposableCardDef def,
    object userData)
  {
    this.m_PremiumHeroCardDef?.Dispose();
    this.m_PremiumHeroCardDef = def;
  }

  public delegate void AnimationDoneListener();
}
