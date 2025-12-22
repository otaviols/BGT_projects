using System.Collections.Generic;
using UnityEngine;

public class BoundsOverride : MonoBehaviour
{
  public List<ScreenCategory> m_screenCategory = new List<ScreenCategory>();
  public List<Bounds> m_bounds = new List<Bounds>();

  public Bounds bounds => this.m_bounds[PlatformSettings.GetBestScreenMatch(this.m_screenCategory)];

  public void AddCategory() => this.AddCategory(PlatformSettings.Screen);

  public void AddCategory(ScreenCategory screen)
  {
    if (Application.IsPlaying((Object) this))
      return;
    this.m_screenCategory.Add(screen);
    this.m_bounds.Add(new Bounds());
  }
}
