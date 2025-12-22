using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalusDeathrattleSpell : Spell
{
  public Spell m_CustomDeathSpell;
  public float m_DelayBeforeStart = 1f;
  public float m_DelayDistanceModifier = 1f;
  public float m_RiseTime = 0.5f;
  public float m_HangTime = 1f;
  public float m_LiftHeightMin = 2f;
  public float m_LiftHeightMax = 3f;
  public float m_LiftRotMin = -15f;
  public float m_LiftRotMax = 15f;
  public float m_SlamTime = 0.15f;
  public float m_Bounceness = 0.2f;
  public float m_DelayAfterSpellFinish = 3f;
  private GameObject[] m_TargetActorGameObjects;
  private Actor[] m_TargetActors;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    List<Card> cardList = new List<Card>();
    List<Entity> targetEntities = new List<Entity>();
    foreach (GameObject visualTarget in this.GetVisualTargets())
    {
      if (!((UnityEngine.Object) visualTarget == (UnityEngine.Object) null))
      {
        Card component = visualTarget.GetComponent<Card>();
        cardList.Add(component);
        targetEntities.Add(component.GetEntity());
      }
    }
    List<Entity> sourceAmongstTargets = GameUtils.GetEntitiesKilledBySourceAmongstTargets(this.GetSourceCard().GetEntity().GetEntityId(), targetEntities);
    foreach (Card card in cardList)
    {
      Card targetCard = card;
      if (sourceAmongstTargets.Exists((Predicate<Entity>) (killedEntity => killedEntity.GetEntityId() == targetCard.GetEntity().GetEntityId())))
        targetCard.OverrideCustomDeathSpell(SpellManager.Get().GetSpell(this.m_CustomDeathSpell));
    }
    this.StartCoroutine(this.AnimateMinions());
  }

  private IEnumerator AnimateMinions()
  {
    AnomalusDeathrattleSpell deathrattleSpell = this;
    if (!((UnityEngine.Object) deathrattleSpell.m_source == (UnityEngine.Object) null))
    {
      yield return (object) new WaitForSeconds(deathrattleSpell.m_DelayBeforeStart);
      float seconds = 0.0f;
      deathrattleSpell.OnSpellFinished();
      deathrattleSpell.m_TargetActorGameObjects = new GameObject[deathrattleSpell.m_targets.Count];
      deathrattleSpell.m_TargetActors = new Actor[deathrattleSpell.m_targets.Count];
      for (int index = 0; index < deathrattleSpell.m_targets.Count; ++index)
      {
        GameObject target = deathrattleSpell.m_targets[index];
        if (!((UnityEngine.Object) target == (UnityEngine.Object) null))
        {
          Card component = target.GetComponent<Card>();
          if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
          {
            Actor actor = component.GetActor();
            if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
            {
              deathrattleSpell.m_TargetActors[index] = actor;
              GameObject gameObject = actor.gameObject;
              if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
              {
                deathrattleSpell.m_TargetActorGameObjects[index] = gameObject;
                Vector3 localPosition = gameObject.transform.localPosition;
                Quaternion localRotation = gameObject.transform.localRotation;
                float num1 = Vector3.Distance(deathrattleSpell.m_source.transform.position, gameObject.transform.position);
                float num2 = num1 * deathrattleSpell.m_DelayDistanceModifier;
                if ((double) seconds < (double) num2)
                  seconds = num2;
                float y = UnityEngine.Random.Range(deathrattleSpell.m_LiftHeightMin, deathrattleSpell.m_LiftHeightMax);
                Hashtable args1 = iTween.Hash((object) "time", (object) deathrattleSpell.m_RiseTime, (object) "delay", (object) (float) ((double) num1 * (double) deathrattleSpell.m_DelayDistanceModifier), (object) "position", (object) new Vector3(0.0f, y, 0.0f), (object) "easetype", (object) iTween.EaseType.easeOutExpo, (object) "islocal", (object) true, (object) "name", (object) string.Format("Lift_{0}_{1}", (object) gameObject.name, (object) index));
                iTween.MoveTo(gameObject, args1);
                Vector3 eulerAngles = localRotation.eulerAngles;
                eulerAngles.x += UnityEngine.Random.Range(deathrattleSpell.m_LiftRotMin, deathrattleSpell.m_LiftRotMax);
                eulerAngles.z += UnityEngine.Random.Range(deathrattleSpell.m_LiftRotMin, deathrattleSpell.m_LiftRotMax);
                Hashtable args2 = iTween.Hash((object) "time", (object) (float) ((double) deathrattleSpell.m_RiseTime + (double) deathrattleSpell.m_HangTime), (object) "delay", (object) (float) ((double) num1 * (double) deathrattleSpell.m_DelayDistanceModifier), (object) "rotation", (object) eulerAngles, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "islocal", (object) true, (object) "name", (object) string.Format("LiftRot_{0}_{1}", (object) gameObject.name, (object) index));
                iTween.RotateTo(gameObject, args2);
              }
            }
          }
        }
      }
      yield return (object) new WaitForSeconds(seconds);
      for (int index = 0; index < deathrattleSpell.m_targets.Count; ++index)
      {
        GameObject targetActorGameObject = deathrattleSpell.m_TargetActorGameObjects[index];
        if (!((UnityEngine.Object) targetActorGameObject == (UnityEngine.Object) null))
        {
          GameObject target = deathrattleSpell.m_targets[index];
          if (!((UnityEngine.Object) target == (UnityEngine.Object) null))
          {
            Card component = target.GetComponent<Card>();
            if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
            {
              if (component.GetZone().m_ServerTag == TAG_ZONE.GRAVEYARD)
              {
                Actor targetActor = deathrattleSpell.m_TargetActors[index];
                if (!((UnityEngine.Object) targetActor == (UnityEngine.Object) null))
                  targetActor.DoCardDeathVisuals();
                else
                  continue;
              }
              float num = 0.0f;
              Hashtable args3 = iTween.Hash((object) "time", (object) deathrattleSpell.m_SlamTime, (object) "delay", (object) (float) ((double) deathrattleSpell.m_DelayAfterSpellFinish + (double) num), (object) "position", (object) Vector3.zero, (object) "easetype", (object) iTween.EaseType.easeInCubic, (object) "islocal", (object) true, (object) "name", (object) string.Format("SlamPos_{0}_{1}", (object) targetActorGameObject.name, (object) index));
              iTween.MoveTo(targetActorGameObject, args3);
              Hashtable args4 = iTween.Hash((object) "time", (object) (float) ((double) deathrattleSpell.m_SlamTime * 0.800000011920929), (object) "delay", (object) (float) ((double) deathrattleSpell.m_DelayAfterSpellFinish + (double) num + (double) deathrattleSpell.m_SlamTime * 0.200000002980232), (object) "rotation", (object) Vector3.zero, (object) "easetype", (object) iTween.EaseType.easeInQuad, (object) "islocal", (object) true, (object) "name", (object) string.Format("SlamRot_{0}_{1}", (object) targetActorGameObject.name, (object) index));
              iTween.RotateTo(targetActorGameObject, args4);
            }
          }
        }
      }
    }
  }
}
