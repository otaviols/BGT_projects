using UnityEngine;

[CustomEditClass]
public class VillagePackOpeningScene : BasicScene
{
  public override void PreUnload()
  {
    base.PreUnload();
    if (!((Object) this.m_displayRoot != (Object) null))
      return;
    VillagePackOpeningDisplay componentInChildren = this.m_displayRoot.GetComponentInChildren<VillagePackOpeningDisplay>();
    if (!((Object) componentInChildren != (Object) null))
      return;
    componentInChildren.PreunloadPackOpeningView();
  }
}
