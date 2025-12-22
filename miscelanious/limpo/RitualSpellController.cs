using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualSpellController : SpellController
{
  public Spell m_ritualSpell;
  public float m_noSpellDisplayTime = 3f;
  public string m_friendlyRitualBoneName = "FriendlyRitual";
  public string m_opponentRitualBoneName = "OpponentRitual";
  public bool m_hideRitualActor = true;
  public Spell m_tauntInstantSpell;
  public Spell m_tauntInstantPremiumSpell;
  public bool m_skipIfCthunInPlay;
  public bool m_showBonusAnims;
  public float m_prebuffDisplayTime = 1f;
  private Entity m_ritualEntity;
  private Entity m_ritualEntityClone;
  private bool m_finished;
  private Actor m_ritualActor;
  private Spell m_tauntSpellInstance;
  public UberText m_progressText;
  public Vector3 m_progressTextOffset;
  public float m_cthunShatteredDelay;
  private static Map<int, int> s_lastProgressMap = new Map<int, int>();

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    Entity sourceEntity = taskList.GetSourceEntity();
    Player controller = sourceEntity.GetController();
    if (taskList.IsOrigin())
      return true;
    if (!this.AllowedToSeeCthun(taskList) || !taskList.IsEndOfBlock() || this.m_skipIfCthunInPlay && this.IsCthunInPlay(controller))
      return false;
    this.m_ritualEntity = this.GetProxyEntityFromSourceEntity(sourceEntity);
    if (this.m_ritualEntity == null)
      return false;
    this.m_ritualEntityClone = taskList.GetOrigin().GetRitualEntityClone();
    if (this.m_ritualEntityClone == null)
      return false;
    this.SetSource(sourceEntity.GetCard());
    this.AddTarget(this.m_ritualEntity.GetCard());
    return true;
  }

  protected override void OnProcessTaskList() => this.StartCoroutine(this.DoRitualEffect());

  protected bool AllowedToSeeCthun(PowerTaskList taskList)
  {
    Entity sourceEntity = taskList.GetSourceEntity();
    Player controller = sourceEntity.GetController();
    if (sourceEntity.HasTag(GAME_TAG.PIECE_OF_CTHUN))
    {
      int cthunPiecesPlayed = this.GetNumCthunPiecesPlayed(controller);
      if (cthunPiecesPlayed >= 4 || cthunPiecesPlayed == this.GetLastProgress(controller))
        return false;
    }
    else if (!controller.HasTag(GAME_TAG.SEEN_CTHUN))
      return false;
    return true;
  }

  protected int GetNumCthunPiecesPlayed(Player controller)
  {
    if (controller == null)
      return 0;
    int cthunPiecesPlayed = 0;
    if (controller.GetTag(GAME_TAG.PLAYED_CTHUN_EYE) != 0)
      ++cthunPiecesPlayed;
    if (controller.GetTag(GAME_TAG.PLAYED_CTHUN_BODY) != 0)
      ++cthunPiecesPlayed;
    if (controller.GetTag(GAME_TAG.PLAYED_CTHUN_MAW) != 0)
      ++cthunPiecesPlayed;
    if (controller.GetTag(GAME_TAG.PLAYED_CTHUN_HEART) != 0)
      ++cthunPiecesPlayed;
    return cthunPiecesPlayed;
  }

  private IEnumerator DoRitualEffect()
  {
    RitualSpellController ritualSpellController = this;
    Entity sourceEntity = ritualSpellController.m_taskList.GetSourceEntity();
    Player sourceController = sourceEntity.GetController();
    bool isCthunShattered = sourceEntity.HasTag(GAME_TAG.PIECE_OF_CTHUN);
    if (ritualSpellController.m_taskList.IsOrigin())
    {
      int proxyCreationTask = ritualSpellController.FindLatestProxyCreationTask(isCthunShattered ? GAME_TAG.PROXY_CTHUN_SHATTERED : GAME_TAG.PROXY_CTHUN);
      if (proxyCreationTask >= 0)
      {
        if (isCthunShattered)
          ritualSpellController.UpdateLastProgress(sourceController);
        PowerTask latestCreationTask = ritualSpellController.m_taskList.GetTaskList()[proxyCreationTask];
        ritualSpellController.m_taskList.DoTasks(0, proxyCreationTask + 1);
        while (!latestCreationTask.IsCompleted())
          yield return (object) null;
        latestCreationTask = (PowerTask) null;
      }
      ritualSpellController.m_ritualEntity = ritualSpellController.GetProxyEntityFromSourceEntity(sourceEntity);
      ritualSpellController.m_ritualEntityClone = ritualSpellController.m_ritualEntity.CloneForHistory((HistoryInfo) null);
      ritualSpellController.m_taskList.SetRitualEntityClone(ritualSpellController.m_ritualEntityClone);
      if (!ritualSpellController.m_taskList.IsEndOfBlock())
      {
        ritualSpellController.FinishRitual();
        yield break;
      }
      else if (!ritualSpellController.AllowedToSeeCthun(ritualSpellController.m_taskList))
      {
        ritualSpellController.FinishRitual();
        yield break;
      }
    }
    ritualSpellController.m_taskList.DoAllTasks();
    while (!ritualSpellController.m_taskList.IsComplete())
      yield return (object) null;
    HistoryManager historyManager = HistoryManager.Get();
    int ritualEntId = ritualSpellController.GetSource().GetEntity().GetEntityId();
    while (historyManager.HasBigCard() && historyManager.GetCurrentBigCard().GetEntity().GetEntityId() == ritualEntId)
      yield return (object) null;
    Entity entity = ritualSpellController.m_showBonusAnims ? ritualSpellController.m_ritualEntityClone : ritualSpellController.m_ritualEntity;
    ritualSpellController.m_ritualActor = ritualSpellController.LoadRitualActor(entity);
    if ((Object) ritualSpellController.m_ritualActor == (Object) null)
    {
      ritualSpellController.FinishRitual();
    }
    else
    {
      ritualSpellController.UpdateAndPositionRitualActor();
      if ((Object) ritualSpellController.m_ritualSpell != (Object) null)
      {
        if (isCthunShattered)
          yield return (object) new WaitForSeconds(ritualSpellController.m_cthunShatteredDelay);
        Spell spell = SpellManager.Get().GetSpell(ritualSpellController.m_ritualSpell);
        spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(ritualSpellController.OnRitualSpellStateFinished), (object) ritualSpellController.m_ritualActor);
        spell.AddSpellEventCallback(new Spell.SpellEventCallback(ritualSpellController.OnSpellEvent));
        spell.AddFinishedCallback(new Spell.FinishedCallback(ritualSpellController.OnSpellFinished));
        sourceEntity.SetTag(GAME_TAG.DATABASE_ID, GameUtils.TranslateCardIdToDbId(sourceEntity.GetCardId()));
        spell.AddTarget(sourceEntity.GetCard().gameObject);
        TransformUtil.AttachAndPreserveLocalTransform(spell.transform, ritualSpellController.m_ritualActor.transform);
        ritualSpellController.m_ritualActor.GetHealthText().RenderQueue = 1;
        ritualSpellController.m_ritualActor.GetAttackText().RenderQueue = 1;
        spell.Activate();
        ritualSpellController.m_progressText.Text = string.Format("{0}/4", (object) ritualSpellController.GetNumCthunPiecesPlayed(sourceController));
      }
      if (ritualSpellController.m_showBonusAnims)
      {
        yield return (object) new WaitForSeconds(ritualSpellController.m_prebuffDisplayTime);
        if (!ritualSpellController.m_finished)
        {
          ritualSpellController.m_ritualActor.SetEntity(ritualSpellController.m_ritualEntity);
          if (!ritualSpellController.m_ritualEntityClone.HasTag(GAME_TAG.TAUNT) && ritualSpellController.m_ritualEntity.HasTag(GAME_TAG.TAUNT))
            ritualSpellController.m_ritualActor.ActivateTaunt();
          ritualSpellController.m_ritualActor.UpdateAllComponents();
        }
      }
      if ((Object) ritualSpellController.m_ritualSpell == (Object) null)
      {
        yield return (object) new WaitForSeconds(ritualSpellController.m_showBonusAnims ? Mathf.Max(0.0f, ritualSpellController.m_noSpellDisplayTime - ritualSpellController.m_prebuffDisplayTime) : ritualSpellController.m_noSpellDisplayTime);
        ritualSpellController.m_ritualActor.Destroy();
        ritualSpellController.FinishRitual();
      }
    }
  }

  private int FindLatestProxyCreationTask(GAME_TAG proxyTag)
  {
    int proxyCreationTask = -1;
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      Network.PowerHistory power = taskList[index].GetPower();
      switch (power.Type)
      {
        case Network.PowerType.FULL_ENTITY:
          if (index > proxyCreationTask)
          {
            proxyCreationTask = index;
            break;
          }
          break;
        case Network.PowerType.TAG_CHANGE:
          Network.HistTagChange histTagChange = (Network.HistTagChange) power;
          if ((GAME_TAG) histTagChange.Tag == proxyTag && histTagChange.Value > 0 && index > proxyCreationTask)
          {
            proxyCreationTask = index;
            break;
          }
          break;
      }
    }
    return proxyCreationTask;
  }

  public void OnSpellFinished(Spell spell, object userData) => this.OnFinishedTaskList();

  public void OnSpellEvent(string eventName, object eventData, object userData)
  {
    Entity sourceEntity = this.m_taskList.GetSourceEntity();
    Player controller = sourceEntity.GetController();
    bool flag = sourceEntity.HasTag(GAME_TAG.PIECE_OF_CTHUN);
    if (eventName != "showCthun")
      Debug.LogError((object) ("RitualSpellController received unexpected Spell Event " + eventName));
    if (!this.m_hideRitualActor)
      return;
    this.m_ritualActor.Show();
    if (flag)
    {
      this.UpdateLastProgress(controller);
      this.m_progressText.gameObject.SetActive(true);
    }
    if (!((Object) this.m_tauntSpellInstance != (Object) null))
      return;
    this.m_tauntSpellInstance.Activate();
  }

  private void OnRitualSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    ((Actor) userData).Destroy();
    this.FinishRitual();
  }

  private void FinishRitual()
  {
    this.m_finished = true;
    if (this.m_processingTaskList)
      this.OnFinishedTaskList();
    this.OnFinished();
  }

  private Actor LoadRitualActor(Entity entity)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetZoneActor(entity, TAG_ZONE.PLAY), AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogWarning((object) "RitualSpellController unable to load Ritual Actor GameObject.");
      return (Actor) null;
    }
    Actor component = gameObject.GetComponent<Actor>();
    if ((Object) component == (Object) null)
    {
      Debug.LogWarning((object) "RitualSpellController Ritual Actor GameObject contains no Actor component.");
      Object.Destroy((Object) gameObject);
      return (Actor) null;
    }
    component.SetEntity(entity);
    component.SetCardDefFromEntity(entity);
    return component;
  }

  private void UpdateAndPositionRitualActor()
  {
    if (this.m_ritualActor.GetEntity().HasTag(GAME_TAG.TAUNT))
    {
      Spell spell = this.m_ritualEntity.GetPremiumType() == TAG_PREMIUM.NORMAL ? this.m_tauntInstantSpell : this.m_tauntInstantPremiumSpell;
      if ((Object) spell != (Object) null)
      {
        this.m_tauntSpellInstance = SpellManager.Get().GetSpell(spell);
        TransformUtil.AttachAndPreserveLocalTransform(this.m_tauntSpellInstance.transform, this.m_ritualActor.transform);
        if (!this.m_hideRitualActor)
          this.m_tauntSpellInstance.Activate();
      }
      else
        Debug.LogWarning((object) "RitualSpellController does not have a instant taunt spell hooked up.");
    }
    this.m_ritualActor.UpdateMinionStatsImmediately();
    if (this.m_hideRitualActor)
      this.m_ritualActor.Hide();
    string name = this.m_ritualActor.GetEntity().GetControllerSide() == Player.Side.FRIENDLY ? this.m_friendlyRitualBoneName : this.m_opponentRitualBoneName;
    this.m_ritualActor.transform.parent = Board.Get().FindBone(name);
    this.m_ritualActor.transform.localPosition = Vector3.zero;
    this.m_progressText.transform.parent = this.m_ritualActor.gameObject.transform;
    this.m_progressText.gameObject.transform.localPosition = this.m_progressTextOffset;
  }

  private bool IsCthunInPlay(Player player)
  {
    foreach (Card card in player.GetBattlefieldZone().GetCards())
    {
      if (card.GetController() == player && card.GetEntity().GetCardId() == "OG_280")
        return true;
    }
    return false;
  }

  private Entity GetProxyEntityFromSourceEntity(Entity sourceEntity)
  {
    Player controller = sourceEntity.GetController();
    int id = !sourceEntity.HasTag(GAME_TAG.PIECE_OF_CTHUN) ? controller.GetTag(GAME_TAG.PROXY_CTHUN) : controller.GetTag(GAME_TAG.PROXY_CTHUN_SHATTERED);
    return id == 0 ? (Entity) null : GameState.Get().GetEntity(id);
  }

  private int GetLastProgress(Player player)
  {
    int lastProgress = 0;
    RitualSpellController.s_lastProgressMap.TryGetValue(player.GetEntityId(), out lastProgress);
    return lastProgress;
  }

  private void UpdateLastProgress(Player player)
  {
    int entityId = player.GetEntityId();
    if (RitualSpellController.s_lastProgressMap.ContainsKey(entityId))
      RitualSpellController.s_lastProgressMap[entityId] = this.GetNumCthunPiecesPlayed(player);
    else
      RitualSpellController.s_lastProgressMap.Add(entityId, this.GetNumCthunPiecesPlayed(player));
  }
}
