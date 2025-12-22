using PegasusGame;
using System;
using System.Collections.Generic;

public class PowerTask
{
  private Network.PowerHistory m_power;
  private bool m_completed;
  private PowerTask.TaskCompleteCallback m_onCompleted;

  public Network.PowerHistory GetPower() => this.m_power;

  public void SetPower(Network.PowerHistory power) => this.m_power = power;

  public bool IsCompleted() => this.m_completed;

  public void SetCompleted(bool complete)
  {
    this.m_completed = complete;
    if (!this.m_completed || this.m_onCompleted == null)
      return;
    this.m_onCompleted();
  }

  public void SetTaskCompleteCallback(PowerTask.TaskCompleteCallback onComplete) => this.m_onCompleted = onComplete;

  private bool IsZoneTransition(TAG_ZONE fromZone, TAG_ZONE toZone)
  {
    if (this.IsCompleted())
      return false;
    Network.PowerHistory power = this.GetPower();
    if (power.Type == Network.PowerType.SHOW_ENTITY)
    {
      Network.HistShowEntity histShowEntity = (Network.HistShowEntity) power;
      Entity entity = GameState.Get().GetEntity(histShowEntity.Entity.ID);
      Network.Entity.Tag tag = histShowEntity.Entity.Tags.Find((Predicate<Network.Entity.Tag>) (currTag => currTag.Name == 49));
      if (entity != null && tag != null && entity.GetZone() == fromZone && (TAG_ZONE) tag.Value == toZone)
        return true;
    }
    if (power.Type == Network.PowerType.TAG_CHANGE)
    {
      Network.HistTagChange histTagChange = (Network.HistTagChange) power;
      Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
      if (entity != null && histTagChange.Tag == 49 && entity.GetZone() == fromZone && (TAG_ZONE) histTagChange.Value == toZone)
        return true;
    }
    return false;
  }

  public bool IsCardDraw() => this.IsZoneTransition(TAG_ZONE.DECK, TAG_ZONE.HAND);

  public bool IsCardMill() => this.IsZoneTransition(TAG_ZONE.DECK, TAG_ZONE.GRAVEYARD);

  public bool IsFatigue()
  {
    if (this.IsCompleted())
      return false;
    Network.PowerHistory power = this.GetPower();
    return power.Type == Network.PowerType.BLOCK_START && ((Network.HistBlockStart) power).BlockType == HistoryBlock.Type.FATIGUE;
  }

  public void DoRealTimeTask(List<Network.PowerHistory> powerList, int index)
  {
    GameState gameState = GameState.Get();
    switch (this.m_power.Type)
    {
      case Network.PowerType.FULL_ENTITY:
        Network.HistFullEntity power1 = (Network.HistFullEntity) this.m_power;
        gameState.OnRealTimeFullEntity(power1);
        break;
      case Network.PowerType.SHOW_ENTITY:
        Network.HistShowEntity power2 = (Network.HistShowEntity) this.m_power;
        gameState.OnRealTimeShowEntity(power2);
        break;
      case Network.PowerType.TAG_CHANGE:
        Network.HistTagChange power3 = (Network.HistTagChange) this.m_power;
        gameState.OnRealTimeTagChange(power3);
        break;
      case Network.PowerType.CREATE_GAME:
        Network.HistCreateGame power4 = (Network.HistCreateGame) this.m_power;
        gameState.OnRealTimeCreateGame(powerList, index, power4);
        break;
      case Network.PowerType.CHANGE_ENTITY:
        Network.HistChangeEntity power5 = (Network.HistChangeEntity) this.m_power;
        gameState.OnRealTimeChangeEntity(powerList, index, power5);
        break;
      case Network.PowerType.RESET_GAME:
        Network.HistResetGame power6 = (Network.HistResetGame) this.m_power;
        gameState.OnRealTimeResetGame(power6);
        break;
      case Network.PowerType.VO_SPELL:
        Network.HistVoSpell power7 = (Network.HistVoSpell) this.m_power;
        gameState.OnRealTimeVoSpell(power7);
        break;
    }
  }

  public void DoTask()
  {
    if (this.m_completed)
      return;
    GameState gameState = GameState.Get();
    switch (this.m_power.Type)
    {
      case Network.PowerType.FULL_ENTITY:
        Network.HistFullEntity power1 = (Network.HistFullEntity) this.m_power;
        gameState.OnFullEntity(power1);
        HistoryManager.Get().OnEntityRevealed();
        break;
      case Network.PowerType.SHOW_ENTITY:
        Network.HistShowEntity power2 = (Network.HistShowEntity) this.m_power;
        gameState.OnShowEntity(power2);
        HistoryManager.Get().OnEntityRevealed();
        break;
      case Network.PowerType.HIDE_ENTITY:
        Network.HistHideEntity power3 = (Network.HistHideEntity) this.m_power;
        gameState.OnHideEntity(power3);
        break;
      case Network.PowerType.TAG_CHANGE:
        Network.HistTagChange power4 = (Network.HistTagChange) this.m_power;
        gameState.OnTagChange(power4);
        break;
      case Network.PowerType.META_DATA:
        Network.HistMetaData power5 = (Network.HistMetaData) this.m_power;
        gameState.OnMetaData(power5);
        break;
      case Network.PowerType.CHANGE_ENTITY:
        Network.HistChangeEntity power6 = (Network.HistChangeEntity) this.m_power;
        gameState.OnChangeEntity(power6);
        break;
      case Network.PowerType.RESET_GAME:
        Network.HistResetGame power7 = (Network.HistResetGame) this.m_power;
        gameState.OnResetGame(power7);
        break;
      case Network.PowerType.VO_SPELL:
        Network.HistVoSpell power8 = (Network.HistVoSpell) this.m_power;
        gameState.OnVoSpell(power8);
        break;
      case Network.PowerType.CACHED_TAG_FOR_DORMANT_CHANGE:
        Network.HistCachedTagForDormantChange power9 = (Network.HistCachedTagForDormantChange) this.m_power;
        gameState.OnCachedTagForDormantChange(power9);
        break;
      case Network.PowerType.SHUFFLE_DECK:
        Network.HistShuffleDeck power10 = (Network.HistShuffleDeck) this.m_power;
        gameState.OnShuffleDeck(power10);
        break;
      case Network.PowerType.VO_BANTER:
        Network.HistVoBanter power11 = (Network.HistVoBanter) this.m_power;
        gameState.OnVoBanter(power11);
        break;
    }
    this.SetCompleted(true);
  }

  public void DoEarlyConcedeTask()
  {
    if (this.m_completed)
      return;
    GameState gameState = GameState.Get();
    switch (this.m_power.Type)
    {
      case Network.PowerType.SHOW_ENTITY:
        Network.HistShowEntity power1 = (Network.HistShowEntity) this.m_power;
        gameState.OnEarlyConcedeShowEntity(power1);
        break;
      case Network.PowerType.HIDE_ENTITY:
        Network.HistHideEntity power2 = (Network.HistHideEntity) this.m_power;
        gameState.OnEarlyConcedeHideEntity(power2);
        break;
      case Network.PowerType.TAG_CHANGE:
        Network.HistTagChange power3 = (Network.HistTagChange) this.m_power;
        gameState.OnEarlyConcedeTagChange(power3);
        break;
      case Network.PowerType.CHANGE_ENTITY:
        Network.HistChangeEntity power4 = (Network.HistChangeEntity) this.m_power;
        gameState.OnEarlyConcedeChangeEntity(power4);
        break;
    }
    this.m_completed = true;
  }

  public override string ToString()
  {
    string str = "null";
    if (this.m_power != null)
    {
      switch (this.m_power.Type)
      {
        case Network.PowerType.FULL_ENTITY:
          Network.HistFullEntity power1 = (Network.HistFullEntity) this.m_power;
          str = string.Format("type={0} entity={1} tags={2}", (object) this.m_power.Type, (object) this.GetPrintableEntity(power1.Entity), (object) power1.Entity.Tags);
          break;
        case Network.PowerType.SHOW_ENTITY:
          Network.HistShowEntity power2 = (Network.HistShowEntity) this.m_power;
          str = string.Format("type={0} entity={1} tags={2}", (object) this.m_power.Type, (object) this.GetPrintableEntity(power2.Entity), (object) power2.Entity.Tags);
          break;
        case Network.PowerType.HIDE_ENTITY:
          Network.HistHideEntity power3 = (Network.HistHideEntity) this.m_power;
          str = string.Format("type={0} entity={1} zone={2}", (object) this.m_power.Type, (object) this.GetPrintableEntity(power3.Entity), (object) power3.Zone);
          break;
        case Network.PowerType.TAG_CHANGE:
          Network.HistTagChange power4 = (Network.HistTagChange) this.m_power;
          str = string.Format("type={0} entity={1} {2} {3}", (object) this.m_power.Type, (object) this.GetPrintableEntity(power4.Entity), (object) Tags.DebugTag(power4.Tag, power4.Value), power4.ChangeDef ? (object) "DEF CHANGE" : (object) "");
          break;
        case Network.PowerType.CREATE_GAME:
          str = ((Network.HistCreateGame) this.m_power).ToString();
          break;
        case Network.PowerType.META_DATA:
          str = ((Network.HistMetaData) this.m_power).ToString();
          break;
        case Network.PowerType.CHANGE_ENTITY:
          Network.HistChangeEntity power5 = (Network.HistChangeEntity) this.m_power;
          str = string.Format("type={0} entity={1} tags={2}", (object) this.m_power.Type, (object) this.GetPrintableEntity(power5.Entity), (object) power5.Entity.Tags);
          break;
      }
    }
    return string.Format("power=[{0}] complete={1}", (object) str, (object) this.m_completed);
  }

  private string GetEntityLogName(Entity entity)
  {
    if (entity == null)
      return (string) null;
    string name = entity.GetName();
    if (entity.IsPlayer())
    {
      BnetPlayer bnetPlayer = (entity as Player).GetBnetPlayer();
      if (bnetPlayer != null && bnetPlayer.GetBattleTag() != (BnetBattleTag) null)
        name = bnetPlayer.GetBattleTag().GetName();
    }
    return name;
  }

  private string GetPrintableEntity(int entityId)
  {
    Entity entity = GameState.Get().GetEntity(entityId);
    if (entity == null)
      return entityId.ToString();
    string entityLogName = this.GetEntityLogName(entity);
    return entityLogName == null ? string.Format("[id={0} cardId={1}]", (object) entityId, (object) entity.GetCardId()) : string.Format("[id={0} cardId={1} name={2}]", (object) entityId, (object) entity.GetCardId(), (object) entityLogName);
  }

  private string GetPrintableEntity(Network.Entity netEntity)
  {
    string entityLogName = this.GetEntityLogName(GameState.Get().GetEntity(netEntity.ID));
    return entityLogName == null ? string.Format("[id={0} cardId={1}]", (object) netEntity.ID, (object) netEntity.CardID) : string.Format("[id={0} cardId={1} name={2}]", (object) netEntity.ID, (object) netEntity.CardID, (object) entityLogName);
  }

  public delegate void TaskCompleteCallback();
}
