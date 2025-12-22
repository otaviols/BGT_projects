using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenResizeDetector : MonoBehaviour
{
  private const float _1x1 = 1f;
  private const float _5x4 = 1.25f;
  private const float _4x3 = 1.333333f;
  private const float _3x2 = 1.5f;
  private const float _16x10 = 1.6f;
  private const float _16x9 = 1.777778f;
  private const float _21x9 = 2.333333f;
  private const float ExtraWide = 2.37037f;
  private const float AspectRatioTolerance = 0.005f;
  private float m_screenWidth;
  private float m_screenHeight;
  private List<ScreenResizeDetector.SizeChangedListener> m_sizeChangedListeners = new List<ScreenResizeDetector.SizeChangedListener>();

  private void Awake()
  {
    this.SaveScreenSize();
    this.UpdateDeviceDataModel();
  }

  private void OnEnable() => RenderPipelineManager.beginFrameRendering += new Action<ScriptableRenderContext, Camera[]>(this.BeginFrameRendering);

  private void OnDisable() => RenderPipelineManager.beginFrameRendering -= new Action<ScriptableRenderContext, Camera[]>(this.BeginFrameRendering);

  private void BeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
  {
    float width = (float) Screen.width;
    float height = (float) Screen.height;
    if (Mathf.Approximately(this.m_screenWidth, width) && Mathf.Approximately(this.m_screenHeight, height))
      return;
    this.SaveScreenSize();
    this.UpdateDeviceDataModel();
    this.FireSizeChangedEvent();
  }

  public bool AddSizeChangedListener(ScreenResizeDetector.SizeChangedCallback callback) => this.AddSizeChangedListener(callback, (object) null);

  public bool AddSizeChangedListener(
    ScreenResizeDetector.SizeChangedCallback callback,
    object userData)
  {
    ScreenResizeDetector.SizeChangedListener sizeChangedListener = new ScreenResizeDetector.SizeChangedListener();
    sizeChangedListener.SetCallback(callback);
    sizeChangedListener.SetUserData(userData);
    if (this.m_sizeChangedListeners.Contains(sizeChangedListener))
      return false;
    this.m_sizeChangedListeners.Add(sizeChangedListener);
    return true;
  }

  public bool RemoveSizeChangedListener(ScreenResizeDetector.SizeChangedCallback callback) => this.RemoveSizeChangedListener(callback, (object) null);

  public bool RemoveSizeChangedListener(
    ScreenResizeDetector.SizeChangedCallback callback,
    object userData)
  {
    ScreenResizeDetector.SizeChangedListener sizeChangedListener = new ScreenResizeDetector.SizeChangedListener();
    sizeChangedListener.SetCallback(callback);
    sizeChangedListener.SetUserData(userData);
    return this.m_sizeChangedListeners.Remove(sizeChangedListener);
  }

  private void SaveScreenSize()
  {
    this.m_screenWidth = (float) Screen.width;
    this.m_screenHeight = (float) Screen.height;
  }

  private void FireSizeChangedEvent()
  {
    foreach (ScreenResizeDetector.SizeChangedListener sizeChangedListener in this.m_sizeChangedListeners.ToArray())
      sizeChangedListener.Fire();
  }

  private void UpdateDeviceDataModel()
  {
    IDataModel model;
    if (!GlobalDataContext.Get().GetDataModel(0, out model))
      return;
    ((DeviceDataModel) model).AspectRatio = this.GetNextBestAspectRatio();
  }

  private AspectRatio GetNextBestAspectRatio()
  {
    float num = PlatformSettings.Screen != ScreenCategory.Phone ? this.m_screenWidth / this.m_screenHeight : Screen.safeArea.width / Screen.safeArea.height;
    if (this.NarrowerThanTargetRatio(num, 1f))
      return AspectRatio.Unknown;
    if (this.NarrowerThanTargetRatio(num, 1.25f))
      return AspectRatio._1x1;
    if (this.NarrowerThanTargetRatio(num, 1.333333f))
      return AspectRatio._5x4;
    if (this.NarrowerThanTargetRatio(num, 1.5f))
      return AspectRatio._4x3;
    if (this.NarrowerThanTargetRatio(num, 1.6f))
      return AspectRatio._3x2;
    if (this.NarrowerThanTargetRatio(num, 1.777778f))
      return AspectRatio._16x10;
    if (this.NarrowerThanTargetRatio(num, 2.333333f))
      return AspectRatio._16x9;
    return this.NarrowerThanTargetRatio(num, 2.37037f) ? AspectRatio._21x9 : AspectRatio.ExtraWide;
  }

  private bool NarrowerThanTargetRatio(float value, float target) => (double) value < (double) target - 0.00499999988824129;

  public delegate void SizeChangedCallback(object userData);

  private class SizeChangedListener : EventListener<ScreenResizeDetector.SizeChangedCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }
}
