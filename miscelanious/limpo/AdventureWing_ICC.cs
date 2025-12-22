using UnityEngine;

[CustomEditClass]
public class AdventureWing_ICC : AdventureWing
{
  [CustomEditField(Sections = "ICC")]
  public NestedPrefab m_bigChestContainer;
  [CustomEditField(Sections = "ICC")]
  public int m_chestVariation;
  private AdventureWingRewardsChest_ICC m_WingRewardsChest;

  protected override void Awake()
  {
    base.Awake();
    if (!((Object) this.m_bigChestContainer != (Object) null))
      return;
    this.m_WingRewardsChest = this.m_bigChestContainer.PrefabGameObject(true).GetComponentInChildren<AdventureWingRewardsChest_ICC>();
    if (!((Object) this.m_WingRewardsChest != (Object) null))
      return;
    this.m_WingRewardsChest.ActivateChest(this.m_chestVariation);
    PegUIElement component = this.m_WingRewardsChest.GetComponent<PegUIElement>();
    if (!((Object) component != (Object) null))
      return;
    this.m_BigChest = component;
  }
}
