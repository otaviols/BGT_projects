using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TransformOverride : MonoBehaviour
{
  public List<ScreenCategory> m_screenCategory = new List<ScreenCategory>();
  public List<Vector3> m_localPosition = new List<Vector3>();
  public List<Vector3> m_localScale = new List<Vector3>();
  public List<Quaternion> m_localRotation = new List<Quaternion>();
  public float testVal;

  public void Awake()
  {
    if (!Application.IsPlaying((Object) this))
      return;
    this.UpdateObject();
  }

  public void AddCategory(ScreenCategory screen)
  {
    if (Application.IsPlaying((Object) this))
      return;
    this.m_screenCategory.Add(screen);
    this.m_localPosition.Add(this.transform.localPosition);
    this.m_localScale.Add(this.transform.localScale);
    this.m_localRotation.Add(this.transform.localRotation);
  }

  public void AddCategory() => this.AddCategory(PlatformSettings.Screen);

  public void UpdateObject()
  {
    int bestScreenMatch = PlatformSettings.GetBestScreenMatch(this.m_screenCategory);
    this.transform.localPosition = this.m_localPosition[bestScreenMatch];
    this.transform.localScale = this.m_localScale[bestScreenMatch];
    this.transform.localRotation = this.m_localRotation[bestScreenMatch];
  }
}
