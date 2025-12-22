using System.Collections;
using UnityEngine;

public class HistoryItem : MonoBehaviour
{
  public Actor m_tileActor;
  public Actor m_mainCardActor;
  protected bool m_dead;
  protected bool m_burned;
  protected bool m_isPoisonous;
  protected bool m_isCriticalHit;
  protected int m_splatAmount;
  protected Entity m_entity;
  protected Texture m_portraitTexture;
  protected Material m_portraitGoldenMaterial;
  protected DefLoader.DisposableCardDef m_cardDef;
  protected bool m_mainCardActorInitialized;
  protected bool m_fatigue;
  protected static readonly string RENDERER_TAG = "FakeShadow";

  protected virtual void OnDestroy()
  {
    this.m_cardDef?.Dispose();
    this.m_cardDef = (DefLoader.DisposableCardDef) null;
  }

  public Entity GetEntity() => this.m_entity;

  public Texture GetPortraitTexture() => this.m_portraitTexture;

  public Material GetPortraitGoldenMaterial() => this.m_portraitGoldenMaterial;

  public Collider GetTileCollider()
  {
    if ((Object) this.m_tileActor == (Object) null)
      return (Collider) null;
    if ((Object) this.m_tileActor.GetMeshRenderer() == (Object) null)
      return (Collider) null;
    Transform transform = this.m_tileActor.GetMeshRenderer().transform.Find("Collider");
    return (Object) transform == (Object) null ? (Collider) null : transform.GetComponent<Collider>();
  }

  public bool IsMainCardActorInitialized() => this.m_mainCardActorInitialized;

  public void InitializeMainCardActor()
  {
    if (this.m_mainCardActorInitialized)
      return;
    this.m_mainCardActor.TurnOffCollider();
    this.m_mainCardActor.SetActorState(ActorStateType.CARD_HISTORY);
    this.m_mainCardActorInitialized = true;
  }

  public void DisplaySpells()
  {
    if (this.m_fatigue)
      return;
    if (this.m_burned)
    {
      this.DisplayFlameOnActor(this.m_mainCardActor);
    }
    else
    {
      if (!this.m_entity.IsCharacter() && !this.m_entity.IsWeapon())
        return;
      if (this.m_dead && !this.m_isPoisonous)
      {
        this.DisplaySkullOnActor(this.m_mainCardActor);
      }
      else
      {
        if (this.m_splatAmount == 0 && !this.m_isPoisonous)
          return;
        this.DisplaySplatOnActor(this.m_mainCardActor, this.m_splatAmount, this.m_isPoisonous, this.m_isCriticalHit);
      }
    }
  }

  private void DisplaySplatOnActor(Actor actor, int damage, bool isPoisonous, bool isCriticalHit)
  {
    Spell spell = actor.GetSpell(SpellType.DAMAGE);
    if ((Object) spell == (Object) null)
      return;
    DamageSplatSpell damageSplatSpell = (DamageSplatSpell) spell;
    damageSplatSpell.SetDamage(damage);
    damageSplatSpell.SetPoisonous(isPoisonous);
    damageSplatSpell.SetDamageIsCrit(isCriticalHit);
    damageSplatSpell.ActivateState(SpellStateType.IDLE);
    this.FadeHistoryOverlay(spell.gameObject);
  }

  protected void DisplaySkullOnActor(Actor actor)
  {
    Spell spell = actor.GetSpell(SpellType.SKULL);
    if ((Object) spell == (Object) null)
      return;
    spell.Activate();
    this.FadeHistoryOverlay(spell.gameObject);
  }

  private void DisplayFlameOnActor(Actor actor)
  {
    Spell spell = actor.GetSpell(SpellType.FLAME_SYMBOL);
    if ((Object) spell == (Object) null)
      return;
    spell.ActivateState(SpellStateType.IDLE);
    this.FadeHistoryOverlay(spell.gameObject);
  }

  private void FadeHistoryOverlay(GameObject gameObject)
  {
    this.StopAllCoroutines();
    iTween.Stop(gameObject);
    this.StartCoroutine(this.FadeHistoryOverlayCoroutine(gameObject));
  }

  private IEnumerator FadeHistoryOverlayCoroutine(GameObject gameObject)
  {
    iTween.FadeTo(gameObject, 1f, 0.0f);
    yield return (object) new WaitForSeconds(1.5f);
    iTween.FadeTo(gameObject, 0.0f, 0.5f);
  }

  protected void SetCardDef(DefLoader.DisposableCardDef cardDef)
  {
    this.m_cardDef?.Dispose();
    this.m_cardDef = cardDef?.Share();
  }
}
