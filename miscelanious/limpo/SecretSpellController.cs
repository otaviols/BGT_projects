using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretSpellController : SpellController
{
  public List<SecretBannerDef> m_BannerDefs;
  public Spell m_DefaultBannerSpellPrefab;
  private Spell m_bannerSpell;
  private Spell m_triggerSpell;

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    if (!this.HasSourceCard(taskList))
      return false;
    Entity sourceEntity = taskList.GetSourceEntity();
    Card card = sourceEntity.GetCard();
    bool flag = false;
    if (taskList.IsStartOfBlock() && this.InitBannerSpell(sourceEntity))
      flag = true;
    Spell triggerSpell = this.GetTriggerSpell(card);
    if ((Object) triggerSpell != (Object) null && this.InitTriggerSpell(card, triggerSpell))
      flag = true;
    if (!flag)
      return false;
    this.SetSource(card);
    return true;
  }

  protected override void OnProcessTaskList()
  {
    this.GetSource().SetSecretTriggered(true);
    if (this.m_taskList.IsStartOfBlock())
    {
      this.FireSecretActorSpell();
      if (this.FireBannerSpell())
        return;
    }
    if (this.FireTriggerSpell())
      return;
    base.OnProcessTaskList();
  }

  private bool FireSecretActorSpell()
  {
    Card source = this.GetSource();
    if (!source.CanShowSecretTrigger())
      return false;
    source.ShowSecretTrigger();
    return true;
  }

  private Spell GetTriggerSpell(Card card)
  {
    Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
    return card.GetTriggerSpell(blockStart.EffectIndex);
  }

  private bool InitTriggerSpell(Card card, Spell triggerSpell)
  {
    if (triggerSpell.AttachPowerTaskList(this.m_taskList))
      return true;
    Log.Power.Print(string.Format("{0}.InitTriggerSpell() - FAILED to attach task list to trigger spell {1} for {2}", (object) this, (object) this.m_taskList.GetBlockStart().EffectIndex, (object) card));
    return false;
  }

  private bool FireTriggerSpell()
  {
    Card source = this.GetSource();
    Spell triggerSpell = this.GetTriggerSpell(source);
    if ((Object) triggerSpell == (Object) null || triggerSpell.GetPowerTaskList() != this.m_taskList && !this.InitTriggerSpell(source, triggerSpell))
      return false;
    triggerSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnTriggerSpellFinished));
    triggerSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnTriggerSpellStateFinished));
    triggerSpell.SafeActivateState(SpellStateType.ACTION);
    return true;
  }

  private void OnTriggerSpellFinished(Spell triggerSpell, object userData) => this.OnFinishedTaskList();

  private void OnTriggerSpellStateFinished(
    Spell triggerSpell,
    SpellStateType prevStateType,
    object userData)
  {
    if (triggerSpell.GetActiveState() != SpellStateType.NONE)
      return;
    this.OnFinished();
  }

  private Spell DetermineBannerSpellPrefab(Entity sourceEntity)
  {
    if (this.m_BannerDefs == null)
      return (Spell) null;
    TAG_CLASS classTag = sourceEntity.GetClass();
    SpellClassTag spellEnum = SpellUtils.ConvertClassTagToSpellEnum(classTag);
    if (spellEnum == SpellClassTag.NONE)
      Debug.LogWarning((object) string.Format("{0}.DetermineBannerSpellPrefab() - entity {1} has unknown class tag {2}. Using default banner.", (object) this, (object) sourceEntity, (object) classTag));
    else if (this.m_BannerDefs != null && this.m_BannerDefs.Count > 0)
    {
      for (int index = 0; index < this.m_BannerDefs.Count; ++index)
      {
        SecretBannerDef bannerDef = this.m_BannerDefs[index];
        if (spellEnum == bannerDef.m_HeroClass)
          return bannerDef.m_SpellPrefab;
      }
      Log.Asset.Print(string.Format("{0}.DetermineBannerSpellPrefab() - class type {1} has no Banner Def. Using default banner.", (object) this, (object) spellEnum));
    }
    return this.m_DefaultBannerSpellPrefab;
  }

  private bool InitBannerSpell(Entity sourceEntity)
  {
    Spell bannerSpellPrefab = this.DetermineBannerSpellPrefab(sourceEntity);
    if ((Object) bannerSpellPrefab == (Object) null)
      return false;
    this.m_bannerSpell = Object.Instantiate<GameObject>(bannerSpellPrefab.gameObject).GetComponent<Spell>();
    return true;
  }

  private bool FireBannerSpell()
  {
    if ((Object) this.m_bannerSpell == (Object) null)
      return false;
    this.StartCoroutine(this.ContinueWithSecretEvents());
    this.m_bannerSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnBannerSpellStateFinished));
    this.m_bannerSpell.Activate();
    return true;
  }

  private IEnumerator ContinueWithSecretEvents()
  {
    yield return (object) new WaitForSeconds(1f);
    while (!HistoryManager.Get().HasBigCard())
      yield return (object) null;
    HistoryManager.Get().NotifyOfSecretSpellFinished();
    yield return (object) new WaitForSeconds(1f);
    if (!this.FireTriggerSpell())
      base.OnProcessTaskList();
  }

  private void OnBannerSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (this.m_bannerSpell.GetActiveState() != SpellStateType.NONE)
      return;
    Object.Destroy((Object) this.m_bannerSpell.gameObject);
    this.m_bannerSpell = (Spell) null;
  }
}
