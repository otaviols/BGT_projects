using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class MercenaryActorLevelObject : MonoBehaviour
{
  public UberText m_levelText;
  public ProgressBar m_xpBar;
  public GameObject m_xpBarCover;
  public GameObject m_xpBarBacking;
  public PlayMakerFSM m_fsm;
  public float m_randomStartOffsetMin;
  public float m_randomStartOffsetMax = 0.5f;
  private const float DELAY_BEFORE_ANIMATION_COMPLETE_EVENT = 0.5f;
  private const float FULLY_UPGRADED_HOLD_TIME = 1f;
  private int m_displayedLevel;
  private bool m_FullyUpgraded;
  private LettuceMercenaryDataModel m_mercenaryDataModel;
  private bool m_isAnimating;

  private void Awake() => this.m_xpBar.OnProgressBarFilled += new Action(this.OnBarFilled);

  public void SetLevelInfo(
    int initialExperience,
    int finalExperience,
    bool fullyUpgradedFinal,
    Hearthstone.UI.Card mercenaryCardWidget = null)
  {
    if (this.m_isAnimating)
      return;
    IDataModel model;
    if ((UnityEngine.Object) mercenaryCardWidget != (UnityEngine.Object) null && mercenaryCardWidget.Owner.GetDataModel(216, out model))
      this.m_mercenaryDataModel = model as LettuceMercenaryDataModel;
    if (initialExperience < 0)
      initialExperience = 0;
    if (initialExperience == finalExperience || finalExperience == 0)
    {
      this.m_xpBar.SetProgressBar(this.GetAutoWrappedProgressBarValue(GameUtils.GetExperiencePercentageFromExperienceValue(initialExperience)));
      if (fullyUpgradedFinal)
      {
        this.m_FullyUpgraded = fullyUpgradedFinal;
        this.StartCoroutine(this.AnimateFullyUpgraded());
      }
      else if (this.isActiveAndEnabled)
        this.StartCoroutine(this.SendAnimationCompleteEventAfterDelay());
    }
    else
    {
      int experienceDelta = finalExperience - initialExperience;
      float fromExperienceValue = GameUtils.GetExperiencePercentageFromExperienceValue(initialExperience);
      float experiencePercentageDelta = GameUtils.GetExperiencePercentageDelta(initialExperience, experienceDelta);
      this.StartCoroutine(this.AnimateBar(this.GetAutoWrappedProgressBarValue(fromExperienceValue), experiencePercentageDelta));
    }
    this.SetLevelText(GameUtils.GetMercenaryLevelFromExperience(initialExperience));
  }

  public void SetLevelText(int level)
  {
    this.m_displayedLevel = Mathf.Min(level, GameUtils.GetMaxMercenaryLevel());
    this.m_levelText.Text = this.m_displayedLevel.ToString();
  }

  private float GetAutoWrappedProgressBarValue(float value)
  {
    if ((double) value % 1.0 == 0.0)
      value += 0.0001f;
    return value;
  }

  private IEnumerator AnimateBar(float initialValue, float delta)
  {
    this.m_isAnimating = true;
    this.m_xpBar.SetProgressBar(initialValue);
    yield return (object) new WaitForSeconds(UnityEngine.Random.Range(this.m_randomStartOffsetMin, this.m_randomStartOffsetMax));
    this.m_fsm.SendEvent("Birth");
    float progressBarValue = this.GetAutoWrappedProgressBarValue(initialValue + delta);
    this.m_xpBar.AnimateProgress(initialValue, progressBarValue);
    yield return (object) new WaitForSeconds(this.m_xpBar.GetAnimationTime());
    yield return (object) this.SendAnimationCompleteEventAfterDelay();
    this.m_fsm.SendEvent("Death");
    this.m_isAnimating = false;
  }

  private IEnumerator SendAnimationCompleteEventAfterDelay()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MercenaryActorLevelObject actorLevelObject = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      SendEventUpwardStateAction.SendEventUpward(actorLevelObject.gameObject, "XP_BAR_ANIMATION_COMPLETE_FROM_CODE");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(0.5f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void OnBarFilled()
  {
    int num1 = 0;
    int num2 = 0;
    EventDataModel eventData = new EventDataModel();
    if (this.m_mercenaryDataModel != null)
    {
      int mercenaryId = this.m_mercenaryDataModel.MercenaryId;
      LettuceMercenaryLevelUpDataModel levelUpDataModel = new LettuceMercenaryLevelUpDataModel();
      int attack1;
      int health1;
      CollectionUtils.GetMercenaryStatsByLevel(mercenaryId, this.m_displayedLevel, false, out attack1, out health1);
      int attack2;
      int health2;
      CollectionUtils.GetMercenaryStatsByLevel(mercenaryId, this.m_displayedLevel + 1, this.m_FullyUpgraded, out attack2, out health2);
      num1 = attack2 - attack1;
      num2 = health2 - health1;
      levelUpDataModel.AttackIncrease = num1;
      levelUpDataModel.HealthIncrease = num2;
      levelUpDataModel.NewAttackValue = attack2;
      levelUpDataModel.NewHealthValue = health2;
      levelUpDataModel.NewIsMaxLevel = this.m_displayedLevel + 1 >= GameUtils.GetMaxMercenaryLevel();
      eventData.Payload = (object) levelUpDataModel;
    }
    SendEventUpwardStateAction.SendEventUpward(this.gameObject, "LEVEL_UP_FROM_CODE", eventData);
    this.m_fsm.FsmVariables.GetFsmInt("AttackIncrease").Value = num1;
    this.m_fsm.FsmVariables.GetFsmInt("HealthIncrease").Value = num2;
    this.m_fsm.SendEvent("LEVEL UP");
    this.SetLevelText(this.m_displayedLevel + 1);
  }

  private IEnumerator AnimateFullyUpgraded()
  {
    this.m_isAnimating = true;
    yield return (object) new WaitForSeconds(UnityEngine.Random.Range(this.m_randomStartOffsetMin, this.m_randomStartOffsetMax));
    this.m_fsm.SendEvent("Birth");
    this.OnBarFilled();
    yield return (object) new WaitForSeconds(1f);
    yield return (object) this.SendAnimationCompleteEventAfterDelay();
    this.m_fsm.SendEvent("Death");
    this.m_isAnimating = false;
  }
}
