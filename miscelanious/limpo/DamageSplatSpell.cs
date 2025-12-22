using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class DamageSplatSpell : Spell
{
  public GameObject m_BloodSplat;
  public GameObject m_PoisonSplat;
  public GameObject m_HealSplat;
  public GameObject m_BloodCritSplat;
  public UberText m_DamageTextMesh;
  private GameObject m_activeSplat;
  private int m_damage;
  private bool m_poison;
  private bool m_damageIsCrit;
  private const float SCALE_IN_TIME = 1f;
  private const float DELAY_ASYNC_ANIM = 1.1f;
  private const float FADE_IN_TIME = 1f;
  private CancellationTokenSource m_animTokenSource;

  protected override void Awake()
  {
    base.Awake();
    this.EnableAllRenderers(false);
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (this.m_animTokenSource == null)
      return;
    this.m_animTokenSource.Cancel();
    this.m_animTokenSource.Dispose();
  }

  public float GetDamage() => (float) this.m_damage;

  public void SetDamage(int damage) => this.m_damage = damage;

  public void SetPoisonous(bool isPoisonous)
  {
    this.m_poison = isPoisonous;
    this.m_DamageTextMesh.gameObject.SetActive(!this.m_poison);
  }

  public bool IsPoisonous() => this.m_poison;

  public void SetDamageIsCrit(bool isCrit) => this.m_damageIsCrit = isCrit;

  public bool IsDamageCritical() => this.m_damageIsCrit;

  public void DoSplatAnims()
  {
    this.StopAllAsyncs();
    iTween.Stop(this.gameObject);
    if (this.m_animTokenSource == null)
      this.m_animTokenSource = new CancellationTokenSource();
    else if (this.m_animTokenSource != null && this.m_animTokenSource.IsCancellationRequested)
    {
      this.m_animTokenSource.Dispose();
      this.m_animTokenSource = new CancellationTokenSource();
    }
    this.SplatAnimAsync(this.m_animTokenSource.Token).Forget();
  }

  private async UniTaskVoid SplatAnimAsync(CancellationToken token)
  {
    DamageSplatSpell damageSplatSpell = this;
    damageSplatSpell.UpdateElements();
    damageSplatSpell.transform.localScale = Vector3.zero;
    await UniTask.Yield(PlayerLoopTiming.Update, token);
    damageSplatSpell.OnSpellFinished();
    damageSplatSpell.SetDamageIsCrit(false);
    iTween.ScaleTo(damageSplatSpell.gameObject, iTween.Hash((object) "scale", (object) Vector3.one, (object) "time", (object) 1f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    float num = 2f;
    if (damageSplatSpell.IsPoisonous())
      num = 0.8f;
    UniTask uniTask = UniTask.Delay(TimeSpan.FromSeconds((double) num), cancellationToken: token);
    await uniTask;
    iTween.FadeTo(damageSplatSpell.gameObject, 0.0f, 1f);
    uniTask = UniTask.Delay(TimeSpan.FromSeconds(1.10000002384186), cancellationToken: token);
    await uniTask;
    damageSplatSpell.EnableAllRenderers(false);
    if (damageSplatSpell.m_activeStateType == SpellStateType.NONE)
      return;
    damageSplatSpell.OnStateFinished();
  }

  protected override void OnIdle(SpellStateType prevStateType)
  {
    this.StopAllAsyncs();
    this.UpdateElements();
    base.OnIdle(prevStateType);
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.UpdateElements();
    base.OnAction(prevStateType);
    this.DoSplatAnims();
  }

  protected override void OnNone(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.m_activeSplat = (GameObject) null;
  }

  protected override void ShowImpl()
  {
    base.ShowImpl();
    if ((UnityEngine.Object) this.m_activeSplat == (UnityEngine.Object) null)
      return;
    RenderUtils.EnableRenderers(this.m_activeSplat.gameObject, true);
    this.m_DamageTextMesh.gameObject.SetActive(!this.m_poison);
  }

  protected override void HideImpl()
  {
    base.HideImpl();
    this.StopAllAsyncs();
    iTween.Stop(this.gameObject);
    this.EnableAllRenderers(false);
  }

  protected override void StopAllAsyncs()
  {
    base.StopAllAsyncs();
    if (this.m_animTokenSource == null || this.m_animTokenSource.IsCancellationRequested)
      return;
    this.m_animTokenSource.Cancel();
  }

  private void UpdateElements()
  {
    iTween.Stop(this.gameObject);
    iTween.FadeTo(this.gameObject, 1f, 0.0f);
    if (this.m_damage < 0 && (UnityEngine.Object) this.m_HealSplat != (UnityEngine.Object) null)
    {
      this.m_activeSplat = this.m_HealSplat;
      if ((UnityEngine.Object) this.m_BloodSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_BloodSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_PoisonSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_PoisonSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_BloodCritSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_BloodCritSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_HealSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_HealSplat.gameObject, true);
      if (!((UnityEngine.Object) this.m_DamageTextMesh != (UnityEngine.Object) null))
        return;
      this.m_DamageTextMesh.Text = string.Format("+{0}", (object) Mathf.Abs(this.m_damage));
      this.m_DamageTextMesh.gameObject.SetActive(true);
    }
    else if (this.m_poison && (UnityEngine.Object) this.m_PoisonSplat != (UnityEngine.Object) null)
    {
      this.m_activeSplat = this.m_PoisonSplat;
      if ((UnityEngine.Object) this.m_BloodSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_BloodSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_PoisonSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_PoisonSplat.gameObject, true);
      if ((UnityEngine.Object) this.m_HealSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_HealSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_BloodCritSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_BloodCritSplat.gameObject, false);
      if (!((UnityEngine.Object) this.m_DamageTextMesh != (UnityEngine.Object) null))
        return;
      this.m_DamageTextMesh.Text = string.Format("-{0}", (object) 0);
      this.m_DamageTextMesh.gameObject.SetActive(false);
    }
    else if (this.m_damageIsCrit && (UnityEngine.Object) this.m_BloodCritSplat != (UnityEngine.Object) null)
    {
      this.m_activeSplat = this.m_BloodCritSplat;
      if ((UnityEngine.Object) this.m_BloodSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_BloodSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_PoisonSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_PoisonSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_HealSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_HealSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_BloodCritSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_BloodCritSplat.gameObject, true);
      if (!((UnityEngine.Object) this.m_DamageTextMesh != (UnityEngine.Object) null))
        return;
      this.m_DamageTextMesh.Text = string.Format("-{0}!", (object) this.m_damage);
      this.m_DamageTextMesh.gameObject.SetActive(true);
    }
    else
    {
      if (!((UnityEngine.Object) this.m_BloodSplat != (UnityEngine.Object) null))
        return;
      this.m_activeSplat = this.m_BloodSplat;
      RenderUtils.EnableRenderers(this.m_BloodSplat.gameObject, true);
      if ((UnityEngine.Object) this.m_PoisonSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_PoisonSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_BloodCritSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_BloodCritSplat.gameObject, false);
      if ((UnityEngine.Object) this.m_HealSplat != (UnityEngine.Object) null)
        RenderUtils.EnableRenderers(this.m_HealSplat.gameObject, false);
      if (!((UnityEngine.Object) this.m_DamageTextMesh != (UnityEngine.Object) null))
        return;
      this.m_DamageTextMesh.Text = string.Format("-{0}", (object) this.m_damage);
      this.m_DamageTextMesh.gameObject.SetActive(true);
    }
  }

  private void EnableAllRenderers(bool enabled)
  {
    if ((UnityEngine.Object) this.m_BloodSplat != (UnityEngine.Object) null)
      RenderUtils.EnableRenderers(this.m_BloodSplat.gameObject, enabled);
    if ((UnityEngine.Object) this.m_HealSplat != (UnityEngine.Object) null)
      RenderUtils.EnableRenderers(this.m_HealSplat.gameObject, enabled);
    if ((UnityEngine.Object) this.m_PoisonSplat != (UnityEngine.Object) null)
      RenderUtils.EnableRenderers(this.m_PoisonSplat.gameObject, enabled);
    if ((UnityEngine.Object) this.m_BloodCritSplat != (UnityEngine.Object) null)
      RenderUtils.EnableRenderers(this.m_BloodCritSplat.gameObject, enabled);
    if (!((UnityEngine.Object) this.m_DamageTextMesh != (UnityEngine.Object) null))
      return;
    this.m_DamageTextMesh.gameObject.SetActive(enabled);
  }
}
