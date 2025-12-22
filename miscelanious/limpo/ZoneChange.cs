using UnityEngine;

public class ZoneChange
{
  private ZoneChangeList m_parentList;
  private PowerTask m_powerTask;
  private Entity m_entity;
  private Zone m_sourceZone;
  private TAG_ZONE m_sourceZoneTag;
  private int? m_sourcePos;
  private int? m_sourceControllerId;
  private Zone m_destinationZone;
  private TAG_ZONE m_destinationZoneTag;
  private int? m_destinationPos;
  private int? m_destinationControllerId;

  public ZoneChangeList GetParentList() => this.m_parentList;

  public void SetParentList(ZoneChangeList parentList) => this.m_parentList = parentList;

  public PowerTask GetPowerTask() => this.m_powerTask;

  public void SetPowerTask(PowerTask powerTask) => this.m_powerTask = powerTask;

  public Entity GetEntity() => this.m_entity;

  public void SetEntity(Entity entity) => this.m_entity = entity;

  public Zone GetDestinationZone() => this.m_destinationZone;

  public void SetDestinationZone(Zone zone) => this.m_destinationZone = zone;

  public TAG_ZONE GetDestinationZoneTag() => this.m_destinationZoneTag;

  public void SetDestinationZoneTag(TAG_ZONE tag) => this.m_destinationZoneTag = tag;

  public int GetDestinationPosition() => this.m_destinationPos.HasValue ? this.m_destinationPos.Value : 0;

  public void SetDestinationPosition(int pos) => this.m_destinationPos = new int?(pos);

  public int GetDestinationControllerId() => this.m_destinationControllerId.HasValue ? this.m_destinationControllerId.Value : 0;

  public void SetDestinationControllerId(int controllerId) => this.m_destinationControllerId = new int?(controllerId);

  public void ClearDestinationControllerId() => this.m_destinationControllerId = new int?();

  public Zone GetSourceZone() => this.m_sourceZone;

  public void SetSourceZone(Zone zone) => this.m_sourceZone = zone;

  public TAG_ZONE GetSourceZoneTag() => this.m_sourceZoneTag;

  public void SetSourceZoneTag(TAG_ZONE tag) => this.m_sourceZoneTag = tag;

  public int GetSourcePosition() => this.m_sourcePos.HasValue ? this.m_sourcePos.Value : 0;

  public void SetSourcePosition(int pos) => this.m_sourcePos = new int?(pos);

  public int GetSourceControllerId() => this.m_sourceControllerId.HasValue ? this.m_sourceControllerId.Value : 0;

  public void SetSourceControllerId(int controllerId) => this.m_sourceControllerId = new int?(controllerId);

  public bool HasSourceZone() => (Object) this.m_sourceZone != (Object) null;

  public bool HasDestinationZone() => (Object) this.m_destinationZone != (Object) null;

  public bool HasDestinationZoneTag() => this.m_destinationZoneTag != 0;

  public bool HasDestinationPosition() => this.m_destinationPos.HasValue;

  public bool HasDestinationControllerId() => this.m_destinationControllerId.HasValue;

  public bool HasDestinationData() => this.HasDestinationZoneTag() || this.HasDestinationPosition() || this.HasDestinationControllerId();

  public bool HasDestinationZoneChange() => this.HasDestinationZoneTag() || this.HasDestinationControllerId();

  public override string ToString() => string.Format("powerTask=[{0}] entity={1} srcZoneTag={2} srcPos={3} dstZoneTag={4} dstPos={5}", (object) this.m_powerTask, (object) this.m_entity, (object) this.m_sourceZoneTag, (object) this.m_sourcePos, (object) this.m_destinationZoneTag, (object) this.m_destinationPos);
}
