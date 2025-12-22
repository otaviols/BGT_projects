using Blizzard.T5.Services;
using Hearthstone;
using UnityEngine;

[ExecuteAlways]
public class ShowFPS : MonoBehaviour
{
  private float m_UpdateInterval = 0.5f;
  private double m_LastInterval;
  private int frames;
  private bool m_FrameCountActive;
  private float m_FrameCountTime;
  private float m_FrameCountLastTime;
  private int m_FrameCount;
  private bool m_verbose;
  private string m_fpsText;
  private const int MAX_CAMERA_NUM = 20;
  private Camera[] m_cameras;
  private static ShowFPS s_instance;

  private void Awake()
  {
    ShowFPS.s_instance = this;
    if (HearthstoneApplication.IsPublic())
      Object.DestroyImmediate((Object) this.gameObject);
    this.m_cameras = new Camera[20];
  }

  private void OnDestroy()
  {
    this.m_cameras = (Camera[]) null;
    ShowFPS.s_instance = (ShowFPS) null;
  }

  public static ShowFPS Get() => ShowFPS.s_instance;

  [ContextMenu("Start Frame Count")]
  public void StartFrameCount()
  {
    this.m_FrameCountLastTime = Time.realtimeSinceStartup;
    this.m_FrameCountTime = 0.0f;
    this.m_FrameCount = 0;
    this.m_FrameCountActive = true;
  }

  [ContextMenu("Stop Frame Count")]
  public void StopFrameCount() => this.m_FrameCountActive = false;

  [ContextMenu("Clear Frame Count")]
  public void ClearFrameCount()
  {
    this.m_FrameCountLastTime = 0.0f;
    this.m_FrameCountTime = 0.0f;
    this.m_FrameCount = 0;
    this.m_FrameCountActive = false;
  }

  private void Start()
  {
    this.m_LastInterval = (double) Time.realtimeSinceStartup;
    this.frames = 0;
    this.UpdateEnabled();
    Options.Get().RegisterChangedListener(Option.HUD, new Options.ChangedCallback(this.OnHudOptionChanged));
  }

  private void OnDisable() => Time.captureFramerate = 0;

  private void Update()
  {
    ++this.frames;
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    if ((double) realtimeSinceStartup > this.m_LastInterval + (double) this.m_UpdateInterval)
    {
      float num = (float) this.frames / (realtimeSinceStartup - (float) this.m_LastInterval);
      this.m_fpsText = !this.m_verbose ? string.Format("{0:f2}", (object) num) : string.Format("{0:f2} - {1} frames over {2}sec", (object) num, (object) this.frames, (object) this.m_UpdateInterval);
      this.frames = 0;
      this.m_LastInterval = (double) realtimeSinceStartup;
    }
    if (!this.m_FrameCountActive && this.m_FrameCount <= 0 || !this.m_FrameCountActive)
      return;
    this.m_FrameCountTime += (float) (((double) realtimeSinceStartup - (double) this.m_FrameCountLastTime) / 60.0) * Time.timeScale;
    if ((double) this.m_FrameCountLastTime == 0.0)
      this.m_FrameCountLastTime = realtimeSinceStartup;
    this.m_FrameCount = Mathf.CeilToInt(this.m_FrameCountTime * 60f);
  }

  private void OnGUI()
  {
    int num = 0;
    if (this.m_cameras.Length < Camera.allCamerasCount)
      this.m_cameras = new Camera[Camera.allCamerasCount];
    int allCameras = Camera.GetAllCameras(this.m_cameras);
    for (int index = 0; index < allCameras; ++index)
    {
      FullScreenEffects component;
      if (this.m_cameras[index].TryGetComponent<FullScreenEffects>(out component) && component.IsActive)
        ++num;
    }
    string text = this.m_fpsText;
    if (this.m_FrameCountActive || this.m_FrameCount > 0)
      text = string.Format("{0} - Frame Count: {1}", (object) text, (object) this.m_FrameCount);
    if (num > 0)
      text = string.Format("{0} - FSE (x{1})", (object) text, (object) num);
    ScreenEffectsMgr service;
    if (ServiceManager.TryGet<ScreenEffectsMgr>(out service))
    {
      int screenEffectsCount = service.GetActiveScreenEffectsCount();
      if (screenEffectsCount > 0 && service.IsActive)
        text = string.Format("{0} - ScreenEffects Active: {1}", (object) text, (object) screenEffectsCount);
    }
    GUI.Box(new Rect((float) Screen.width * 0.75f, (float) (Screen.height - 20), (float) Screen.width * 0.25f, 20f), text);
  }

  private void OnHudOptionChanged(Option option, object prevValue, bool existed, object userData) => this.UpdateEnabled();

  private void UpdateEnabled() => this.enabled = Options.Get().GetBool(Option.HUD);
}
