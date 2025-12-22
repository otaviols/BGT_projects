using Assets;
using System;
using System.Collections.Generic;

[Serializable]
public class SoundDuckingDef
{
  public Global.SoundCategory m_TriggerCategory;
  public List<SoundDuckedCategoryDef> m_DuckedCategoryDefs;

  public override string ToString() => string.Format("[SoundDuckingDef: {0}]", (object) this.m_TriggerCategory);
}
