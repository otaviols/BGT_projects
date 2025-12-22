using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundDucker : MonoBehaviour
{
  public bool m_DuckAllCategories = true;
  public SoundDuckedCategoryDef m_GlobalDuckDef;
  public List<SoundDuckedCategoryDef> m_DuckedCategoryDefs;
  private bool m_ducking;

  private void Awake() => this.InitDuckedCategoryDefs();

  private void OnDestroy() => this.StopDucking();

  public override string ToString() => string.Format("[SoundDucker: {0}]", (object) this.name);

  public List<SoundDuckedCategoryDef> GetDuckedCategoryDefs() => this.m_DuckedCategoryDefs;

  public void SetDuckedCategoryDefs(List<SoundDuckedCategoryDef> duckedCategoryDef)
  {
    this.m_DuckedCategoryDefs = duckedCategoryDef;
    this.InitDuckedCategoryDefs();
  }

  public bool IsDucking() => this.m_ducking;

  public void StartDucking()
  {
    if (SoundManager.Get() == null || this.m_ducking)
      return;
    this.InitDuckedCategoryDefs();
    this.m_ducking = SoundManager.Get().StartDucking(this);
  }

  public void StopDucking()
  {
    if (SoundManager.Get() == null || !this.m_ducking)
      return;
    this.m_ducking = false;
    SoundManager.Get().StopDucking(this);
  }

  private void InitDuckedCategoryDefs()
  {
    if (!this.m_DuckAllCategories || this.m_GlobalDuckDef == null)
      return;
    this.m_DuckedCategoryDefs = new List<SoundDuckedCategoryDef>();
    foreach (Global.SoundCategory soundCategory in Enum.GetValues(typeof (Global.SoundCategory)))
    {
      if (soundCategory != Global.SoundCategory.NONE)
      {
        SoundDuckedCategoryDef dst = new SoundDuckedCategoryDef();
        SoundUtils.CopyDuckedCategoryDef(this.m_GlobalDuckDef, dst);
        dst.m_Category = soundCategory;
        this.m_DuckedCategoryDefs.Add(dst);
      }
    }
  }
}
