using UnityEngine;

public class HistoryInfo
{
  public HistoryInfoType m_infoType;
  public int m_damageChangeAmount;
  public int m_armorChangeAmount;
  public int m_maxHealthChangeAmount;
  public bool m_dontDuplicateUntilEnd;
  public bool m_isBurnedCard;
  public bool m_isPoisonous;
  public bool m_isCriticalHit;
  private Entity m_originalEntity;
  private Entity m_duplicatedEntity;
  private bool m_died;

  public int GetSplatAmount()
  {
    int num = this.m_damageChangeAmount + Mathf.Min((this.m_duplicatedEntity ?? this.m_originalEntity).GetDamage(), Mathf.Max(0, -this.m_maxHealthChangeAmount));
    return this.m_armorChangeAmount <= 0 ? num : num + this.m_armorChangeAmount;
  }

  public int GetCurrentVitality()
  {
    Entity entity = this.m_duplicatedEntity ?? this.m_originalEntity;
    int currentVitality = entity.GetCurrentVitality();
    int num1 = this.m_maxHealthChangeAmount;
    if (num1 < 0)
      num1 = Mathf.Min(0, entity.GetDamage() + this.m_maxHealthChangeAmount);
    int num2 = num1;
    return currentVitality + num2;
  }

  public bool HasValidDisplayEntity()
  {
    switch (this.m_infoType)
    {
      case HistoryInfoType.FATIGUE:
      case HistoryInfoType.BURNED_CARDS:
        return true;
      default:
        return this.GetDuplicatedEntity() != null && (!this.GetDuplicatedEntity().IsHidden() || this.GetDuplicatedEntity().IsSecret());
    }
  }

  public Entity GetDuplicatedEntity() => this.m_duplicatedEntity;

  public Entity GetOriginalEntity() => this.m_originalEntity;

  public void SetOriginalEntity(Entity entity)
  {
    this.m_originalEntity = entity;
    this.DuplicateEntity(false, false);
  }

  public bool HasDied()
  {
    Entity entity = this.m_duplicatedEntity ?? this.m_originalEntity;
    return (entity.IsCharacter() || entity.IsWeapon()) && (this.m_died || this.GetSplatAmount() >= this.GetCurrentVitality());
  }

  public void SetDied(bool set) => this.m_died = set;

  public bool CanDuplicateEntity(bool duplicateHiddenNonSecret, bool isEndOfHistory = false) => this.m_originalEntity != null && this.m_originalEntity.GetLoadState() == Entity.LoadState.DONE && (isEndOfHistory || !this.m_dontDuplicateUntilEnd) && (!this.m_originalEntity.IsHidden() || GameUtils.IsEntityHiddenAfterCurrentTasklist(this.m_originalEntity) && (this.m_originalEntity.IsSecret() || duplicateHiddenNonSecret));

  public void DuplicateEntity(bool duplicateHiddenNonSecret, bool isEndOfHistory)
  {
    if (this.m_duplicatedEntity != null || !this.CanDuplicateEntity(duplicateHiddenNonSecret, isEndOfHistory))
      return;
    this.m_duplicatedEntity = this.m_originalEntity.CloneForHistory(this);
    if (this.m_infoType != HistoryInfoType.CARD_PLAYED && this.m_infoType != HistoryInfoType.WEAPON_PLAYED)
      return;
    this.m_duplicatedEntity.SetTag(GAME_TAG.COST, this.m_originalEntity.GetTag(GAME_TAG.TAG_LAST_KNOWN_COST_IN_HAND));
  }
}
