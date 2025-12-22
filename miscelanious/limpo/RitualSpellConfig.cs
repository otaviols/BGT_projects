using PegasusGame;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RitualSpellConfig", order = 1)]
public class RitualSpellConfig : ScriptableObject
{
  public List<RitualSpellConfig.ClassSpecificRitualConfig> m_classSpecificRitualConfig = new List<RitualSpellConfig.ClassSpecificRitualConfig>();
  public List<string> m_ritualCardIds = new List<string>();
  public TAG_CARDTYPE m_ritualEntityType = TAG_CARDTYPE.HERO;
  public string m_friendlyBoneName = "FriendlyRitual";
  public string m_opponentBoneName = "OpponentRitual";
  public bool m_hideRitualActor = true;
  public bool m_showAttack;
  public bool m_showHealth;
  public bool m_showArmor;
  public GAME_TAG m_proxyRitualEntityTag;
  public bool m_showRitualVisualsInPlay;
  public string m_portalSpellEventName = "showRitualActor";

  public bool IsRitualEntity(Entity entity) => entity != null && this.IsRitualEntity(entity.GetCardId());

  public bool IsRitualEntity(string cardId) => this.m_ritualCardIds.Contains(cardId);

  public bool IsRitualEntityInPlay(Player controller)
  {
    if (controller == null)
      return false;
    switch (this.m_ritualEntityType)
    {
      case TAG_CARDTYPE.HERO:
        Entity hero = controller.GetHero();
        return hero != null && this.IsRitualEntity(hero);
      case TAG_CARDTYPE.MINION:
        using (List<Card>.Enumerator enumerator = controller.GetBattlefieldZone().GetCards().GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            if (this.IsRitualEntity(enumerator.Current.GetEntity()))
              return true;
          }
          break;
        }
      case TAG_CARDTYPE.SPELL:
        using (List<Card>.Enumerator enumerator = controller.GetSecretZone().GetCards().GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            if (this.IsRitualEntity(enumerator.Current.GetEntity()))
              return true;
          }
          break;
        }
      case TAG_CARDTYPE.WEAPON:
        return this.IsRitualEntity(controller.GetWeaponCard().GetEntity());
    }
    return false;
  }

  public Spell GetRitualActivateSpell(Entity ritualEntity) => !this.IsRitualEntity(ritualEntity) ? (Spell) null : this.GetRitualSpellForClass(ritualEntity.GetClass(), true);

  public Spell GetRitualTriggerSpell(Entity ritualEntity) => !this.IsRitualEntity(ritualEntity) ? (Spell) null : this.GetRitualSpellForClass(ritualEntity.GetClass(), false);

  private Spell GetRitualSpellForClass(TAG_CLASS entityClass, bool isActivate)
  {
    foreach (RitualSpellConfig.ClassSpecificRitualConfig specificRitualConfig in this.m_classSpecificRitualConfig)
    {
      if (specificRitualConfig.m_class == entityClass)
        return isActivate ? specificRitualConfig.m_ritualPortalSpell : specificRitualConfig.m_ritualEffectSpell;
    }
    return (Spell) null;
  }

  public bool DoesTaskListContainRitualEntity(PowerTaskList powerTaskList, int entityID)
  {
    if (powerTaskList.GetBlockType() != HistoryBlock.Type.TRIGGER)
      return false;
    foreach (PowerTask task in powerTaskList.GetTaskList())
    {
      if (task.GetPower() is Network.HistChangeEntity power && power.Entity.ID == entityID && this.IsRitualEntity(power.Entity.CardID))
        return true;
    }
    return false;
  }

  public bool DoesFutureTaskListContainsRitualEntity(
    List<PowerTaskList> futureTaskLists,
    PowerTaskList currentTaskList,
    int entityID)
  {
    foreach (PowerTaskList futureTaskList in futureTaskLists)
    {
      if (futureTaskList != null && futureTaskList.IsDescendantOfBlock(currentTaskList) && this.DoesTaskListContainRitualEntity(futureTaskList, entityID))
        return true;
    }
    return false;
  }

  public Actor LoadRitualActor(Entity entity)
  {
    if (entity == null)
      return (Actor) null;
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetZoneActor(entity, TAG_ZONE.PLAY), AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Log.Spells.PrintError("RitualSpellConfig unable to load Invoke Actor GameObject.");
      return (Actor) null;
    }
    Actor component = gameObject.GetComponent<Actor>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Log.Spells.PrintError("RitualSpellConfig Invoke Actor GameObject contains no Actor component.");
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
      return (Actor) null;
    }
    component.SetEntity(entity);
    component.SetCardDefFromEntity(entity);
    return component;
  }

  public void UpdateAndPositionActor(Actor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    if (this.m_hideRitualActor)
      actor.Hide();
    string name = actor.GetEntity().GetControllerSide() == Player.Side.FRIENDLY ? this.m_friendlyBoneName : this.m_opponentBoneName;
    Transform bone = Board.Get().FindBone(name);
    actor.transform.parent = bone;
    actor.transform.localPosition = Vector3.zero;
  }

  public void UpdateRitualActorComponents(Actor actor)
  {
    actor?.m_attackObject?.SetActive(this.m_showAttack);
    actor?.m_healthObject?.SetActive(this.m_showHealth);
    actor?.m_armorSpellBone?.SetActive(this.m_showArmor);
  }

  [Serializable]
  public class ClassSpecificRitualConfig
  {
    public TAG_CLASS m_class;
    public Spell m_ritualPortalSpell;
    public Spell m_ritualEffectSpell;
  }
}
