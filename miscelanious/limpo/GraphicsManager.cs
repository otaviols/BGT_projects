using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class GraphicsManager : IGraphicsManager, IHasUpdate, IService
{
  private const int ANDROID_MIN_DPI_HIGH_RES_TEXTURES = 180;
  private const int DRAGGING_TARGET_FRAMERATE = 60;
  private GraphicsQuality m_GraphicsQuality;
  private bool m_RealtimeShadows;
  private List<GameObject> m_DisableLowQualityObjects;
  private int m_targetFramerate = 60;
  private int m_winPosX;
  private int m_winPosY;
  private bool m_initialPositionSet;
  private bool m_DynamicFps = true;
  private ResizeManager m_resizeManager;
  private bool m_allowMSAA = true;

  private event Action<int, int> m_onResolutionChangedEvent;

  public GraphicsQuality RenderQualityLevel
  {
    get => this.m_GraphicsQuality;
    set
    {
      this.m_GraphicsQuality = value;
      Options.Get().SetInt(Option.GFX_QUALITY, (int) this.m_GraphicsQuality);
      this.UpdateQualitySettings();
    }
  }

  public bool RealtimeShadows => this.m_RealtimeShadows;

  public event Action<int, int> OnResolutionChangedEvent
  {
    add
    {
      this.m_onResolutionChangedEvent -= value;
      this.m_onResolutionChangedEvent += value;
    }
    remove => this.m_onResolutionChangedEvent -= value;
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    GraphicsManager graphicsManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    graphicsManager.InitializeResolution();
    graphicsManager.m_DisableLowQualityObjects = new List<GameObject>();
    if (!Options.Get().HasOption(Option.GFX_QUALITY))
    {
      string intelDeviceName = serviceLocator.Get<ITouchScreenService>().GetIntelDeviceName();
      Log.Graphics.Print("Intel Device Name = {0}", (object) intelDeviceName);
      if (intelDeviceName != null && intelDeviceName.Contains("Haswell") && intelDeviceName.Contains("U28W"))
      {
        if (Screen.currentResolution.height > 1080)
          Options.Get().SetInt(Option.GFX_QUALITY, 0);
      }
      else if (intelDeviceName != null && intelDeviceName.Contains("Crystal-Well"))
        Options.Get().SetInt(Option.GFX_QUALITY, 2);
      else if (intelDeviceName != null && intelDeviceName.Contains("BayTrail"))
        Options.Get().SetInt(Option.GFX_QUALITY, 0);
    }
    graphicsManager.m_GraphicsQuality = (GraphicsQuality) Options.Get().GetInt(Option.GFX_QUALITY);
    graphicsManager.m_resizeManager = new ResizeManager(new Action<int, int>(graphicsManager.OnResolutionChanged));
    graphicsManager.InitializeScreen();
    graphicsManager.UpdateQualitySettings();
    graphicsManager.UpdateFramerateSettings();
    graphicsManager.LogSystemInfo();
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (ITouchScreenService)
  };

  public void Shutdown()
  {
    if (!Screen.fullScreen)
    {
      Options.Get().SetInt(Option.GFX_WIDTH, Screen.width);
      Options.Get().SetInt(Option.GFX_HEIGHT, Screen.height);
      int[] windowPosition = GraphicsManager.GetWindowPosition();
      Options.Get().SetInt(Option.GFX_WIN_POSX, windowPosition[0]);
      Options.Get().SetInt(Option.GFX_WIN_POSY, windowPosition[1]);
    }
    this.m_onResolutionChangedEvent = (Action<int, int>) null;
  }

  public void Update() => this.m_resizeManager.Update();

  public void SetDraggingFramerate(bool isDragging)
  {
    if (!this.m_DynamicFps)
      return;
    if (isDragging)
    {
      if (Application.targetFrameRate >= 60)
        return;
      Application.targetFrameRate = 60;
    }
    else
      Application.targetFrameRate = this.m_targetFramerate;
  }

  public void RegisterLowQualityDisableObject(GameObject lowQualityObject)
  {
    if (this.m_DisableLowQualityObjects.Contains(lowQualityObject))
      return;
    this.m_DisableLowQualityObjects.Add(lowQualityObject);
  }

  public void DeregisterLowQualityDisableObject(GameObject lowQualityObject)
  {
    if (!this.m_DisableLowQualityObjects.Contains(lowQualityObject))
      return;
    this.m_DisableLowQualityObjects.Remove(lowQualityObject);
  }

  public bool isVeryLowQualityDevice() => false;

  public void UpdateTargetFramerate(int rate)
  {
    this.m_targetFramerate = rate;
    Application.targetFrameRate = rate;
  }

  public void UpdateTargetFramerate(int rate, bool dynamicFps)
  {
    this.m_DynamicFps = dynamicFps;
    this.m_targetFramerate = rate;
    Application.targetFrameRate = rate;
    Options.Get().SetInt(Option.GFX_TARGET_FRAME_RATE, this.m_targetFramerate);
    if (rate == Screen.currentResolution.refreshRate)
      QualitySettings.vSyncCount = 1;
    else
      QualitySettings.vSyncCount = 0;
  }

  private void InitializeResolution()
  {
  }

  private void InitializeScreen()
  {
    if (Options.Get().GetBool(Option.GFX_FULLSCREEN) || !Options.Get().HasOption(Option.GFX_WIN_POSX) || !Options.Get().HasOption(Option.GFX_WIN_POSY))
      return;
    int x = Options.Get().GetInt(Option.GFX_WIN_POSX);
    int y = Options.Get().GetInt(Option.GFX_WIN_POSY);
    if (x < 0)
      x = 0;
    if (y < 0)
      y = 0;
    Processor.RunCoroutine(this.SetPos(x, y, 0.6f));
  }

  private void UpdateQualitySettings()
  {
    Log.Graphics.Print("GraphicsManager Update, Graphics Quality: " + this.m_GraphicsQuality.ToString());
    this.UpdateRenderQualitySettings();
    this.UpdateAntiAliasing();
  }

  [DllImport("user32.dll")]
  private static extern bool SetWindowPos(
    IntPtr hwnd,
    int hWndInsertAfter,
    int x,
    int Y,
    int cx,
    int cy,
    int wFlags);

  [DllImport("user32.dll")]
  private static extern IntPtr FindWindow(string className, string windowName);

  [DllImport("user32.dll")]
  private static extern bool EnumWindows(GraphicsManager.EnumWindowsProc enumProc, IntPtr lParam);

  [DllImport("user32.dll", SetLastError = true)]
  private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

  private static bool SetWindowPosition(int x, int y, int resX = 0, int resY = 0)
  {
    if (PlatformFilePaths.IsOptionsFileOverridden())
    {
      GraphicsManager.SetWindowPos(GraphicsManager.GetCurrentProcessWindow(), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
      return true;
    }
    IntPtr activeWindow = GraphicsManager.GetActiveWindow();
    IntPtr window = GraphicsManager.FindWindow((string) null, "Hearthstone");
    if (!(activeWindow == window))
      return false;
    GraphicsManager.SetWindowPos(activeWindow, 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
    return true;
  }

  private static IntPtr GetCurrentProcessWindow()
  {
    IntPtr foundWindow = IntPtr.Zero;
    GraphicsManager.EnumWindows((GraphicsManager.EnumWindowsProc) ((window, param) =>
    {
      uint lpdwProcessId = 0;
      int windowThreadProcessId = (int) GraphicsManager.GetWindowThreadProcessId(window, out lpdwProcessId);
      if ((long) Process.GetCurrentProcess().Id != (long) lpdwProcessId)
        return true;
      foundWindow = window;
      return false;
    }), IntPtr.Zero);
    return foundWindow;
  }

  [DllImport("user32.dll")]
  private static extern IntPtr GetForegroundWindow();

  private static IntPtr GetActiveWindow() => GraphicsManager.GetForegroundWindow();

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetWindowRect(IntPtr hWnd, out GraphicsManager.RECT lpRect);

  public static int[] GetWindowPosition()
  {
    int[] windowPosition = new int[2];
    GraphicsManager.RECT lpRect = new GraphicsManager.RECT();
    GraphicsManager.GetWindowRect(GraphicsManager.GetCurrentProcessWindow(), out lpRect);
    windowPosition[0] = lpRect.Left;
    windowPosition[1] = lpRect.Top;
    return windowPosition;
  }

  public void SetScreenResolution(int width, int height, bool fullscreen) => this.m_resizeManager.SetScreenResolution(width, height, fullscreen);

  private void OnResolutionChanged(int width, int height)
  {
    int[] windowPosition = GraphicsManager.GetWindowPosition();
    int x = windowPosition[0];
    int y = windowPosition[1];
    if (x + width > Screen.currentResolution.width)
      x = Screen.currentResolution.width - width;
    if (y + height > Screen.currentResolution.height)
      y = Screen.currentResolution.height - height;
    if (x < 0 || x > Screen.currentResolution.width)
      x = 0;
    if (y + height > Screen.currentResolution.height)
      y = 0;
    if (y < 0 || y > Screen.currentResolution.height)
      y = 0;
    if (this.m_onResolutionChangedEvent != null && !PlatformSettings.IsMobileRuntimeOS)
      this.m_onResolutionChangedEvent(width, height);
    if (!this.m_initialPositionSet)
      return;
    Processor.RunCoroutine(this.SetPos(x, y));
  }

  private IEnumerator SetPos(int x, int y, float delay = 0.0f)
  {
    if (HearthstoneApplication.IsInternal() && !PlatformFilePaths.IsOptionsFileOverridden())
    {
      this.m_initialPositionSet = true;
    }
    else
    {
      yield return (object) new WaitForSeconds(delay);
      this.m_winPosX = x;
      this.m_winPosY = y;
      int[] currentPos = GraphicsManager.GetWindowPosition();
      int[] newPos = new int[2]
      {
        this.m_winPosX,
        this.m_winPosY
      };
      float startTime = Time.time;
      while (currentPos != newPos && (double) Time.time < (double) startTime + 1.0)
      {
        newPos[0] = this.m_winPosX;
        newPos[1] = this.m_winPosY;
        if (GraphicsManager.SetWindowPosition(this.m_winPosX, this.m_winPosY))
        {
          currentPos = GraphicsManager.GetWindowPosition();
          yield return (object) null;
        }
        else
          break;
      }
      this.m_initialPositionSet = true;
    }
  }

  public bool AllowMSAA() => this.m_allowMSAA;

  private void UpdateAntiAliasing()
  {
    this.m_allowMSAA = true;
    if (this.m_GraphicsQuality == GraphicsQuality.Low)
      this.m_allowMSAA = false;
    ITouchScreenService service;
    if (this.m_GraphicsQuality == GraphicsQuality.Medium && ServiceManager.TryGet<ITouchScreenService>(out service))
    {
      string intelDeviceName = service.GetIntelDeviceName();
      if (intelDeviceName != null && (intelDeviceName.Equals("BayTrail") || intelDeviceName.Equals("Poulsbo") || intelDeviceName.Equals("CloverTrail") || intelDeviceName.Contains("Haswell") && intelDeviceName.Contains("Y6W")))
        this.m_allowMSAA = false;
    }
    if (Options.Get().HasOption(Option.GFX_MSAA))
      this.m_allowMSAA = Options.Get().GetInt(Option.GFX_MSAA) > 0;
    foreach (Camera camera in UnityEngine.Object.FindObjectsOfType(typeof (Camera)) as Camera[])
      camera.allowMSAA = this.m_allowMSAA;
  }

  private void UpdateRenderQualitySettings()
  {
    int num = 101;
    if (this.m_GraphicsQuality == GraphicsQuality.Low)
    {
      this.m_targetFramerate = 60;
      this.m_RealtimeShadows = false;
      this.SetQualityByName("Low");
      num = 101;
    }
    if (this.m_GraphicsQuality == GraphicsQuality.Medium)
    {
      this.m_targetFramerate = 60;
      this.m_RealtimeShadows = false;
      this.SetQualityByName("Medium");
      num = 201;
    }
    if (this.m_GraphicsQuality == GraphicsQuality.High)
    {
      this.m_RealtimeShadows = true;
      this.SetQualityByName("High");
      num = 301;
    }
    Shader.DisableKeyword("LOW_QUALITY");
    foreach (ProjectedShadow projectedShadow in UnityEngine.Object.FindObjectsOfType(typeof (ProjectedShadow)) as ProjectedShadow[])
      projectedShadow.enabled = !this.m_RealtimeShadows || projectedShadow.m_enabledAlongsideRealtimeShadows;
    foreach (RenderToTexture renderToTexture in UnityEngine.Object.FindObjectsOfType(typeof (RenderToTexture)) as RenderToTexture[])
      renderToTexture.ForceTextureRebuild();
    foreach (Shader shader in UnityEngine.Object.FindObjectsOfType(typeof (Shader)) as Shader[])
      shader.maximumLOD = num;
    foreach (GameObject lowQualityObject in this.m_DisableLowQualityObjects)
    {
      if (!((UnityEngine.Object) lowQualityObject == (UnityEngine.Object) null))
      {
        if (this.m_GraphicsQuality == GraphicsQuality.Low)
        {
          Log.Graphics.Print(string.Format("Low Quality Disable: {0}", (object) lowQualityObject.name));
          lowQualityObject.SetActive(false);
        }
        else
        {
          Log.Graphics.Print(string.Format("Low Quality Enable: {0}", (object) lowQualityObject.name));
          lowQualityObject.SetActive(true);
        }
      }
    }
    Shader.globalMaximumLOD = num;
    this.SetScreenEffects();
  }

  private void UpdateFramerateSettings()
  {
    int num1 = 0;
    Options options = Options.Get();
    if (!options.HasOption(Option.GFX_TARGET_FRAME_RATE))
      options.SetInt(Option.GFX_TARGET_FRAME_RATE, 60);
    int num2 = options.GetInt(Option.GFX_TARGET_FRAME_RATE);
    if (num2 == 30)
    {
      this.m_DynamicFps = true;
    }
    else
    {
      this.m_DynamicFps = false;
      num1 = Screen.currentResolution.refreshRate != num2 ? 0 : 1;
    }
    this.m_targetFramerate = num2;
    ITouchScreenService service;
    if (ServiceManager.TryGet<ITouchScreenService>(out service) && service.GetBatteryMode() == PowerSource.BatteryPower && this.m_targetFramerate > 30)
    {
      Log.Graphics.Print("Battery Mode Detected - Clamping Target Frame Rate from {0} to 30", (object) this.m_targetFramerate);
      this.m_targetFramerate = 30;
      this.m_DynamicFps = false;
      options.SetInt(Option.GFX_TARGET_FRAME_RATE, this.m_targetFramerate);
      num1 = 0;
    }
    Application.targetFrameRate = this.m_targetFramerate;
    QualitySettings.vSyncCount = !options.HasOption(Option.GFX_VSYNC) ? num1 : options.GetInt(Option.GFX_VSYNC);
    Log.Graphics.Print(string.Format("Target frame rate: {0}", (object) Application.targetFrameRate));
  }

  private void SetScreenEffects()
  {
    if (ScreenEffectsMgr.Get() == null)
      return;
    if (this.m_GraphicsQuality == GraphicsQuality.Low)
      ScreenEffectsMgr.Get().SetActive(false);
    else
      ScreenEffectsMgr.Get().SetActive(true);
  }

  private void SetQualityByName(string qualityName)
  {
    string[] names = QualitySettings.names;
    int index1 = -1;
    int index2;
    for (index2 = 0; index2 < names.Length; ++index2)
    {
      if (names[index2] == qualityName)
        index1 = index2;
    }
    if (index2 < 0)
      UnityEngine.Debug.LogError((object) string.Format("GraphicsManager: Quality Level not found: {0}", (object) qualityName));
    else
      QualitySettings.SetQualityLevel(index1, true);
  }

  private void LogSystemInfo()
  {
    UnityEngine.Debug.Log((object) "System Info:");
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Device Name: {0}", (object) SystemInfo.deviceName));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Device Model: {0}", (object) SystemInfo.deviceModel));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - OS: {0}", (object) SystemInfo.operatingSystem));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - CPU Type: {0}", (object) SystemInfo.processorType));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - CPU Cores: {0}", (object) SystemInfo.processorCount));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - System Memory: {0}", (object) SystemInfo.systemMemorySize));
    Resolution currentResolution = Screen.currentResolution;
    // ISSUE: variable of a boxed type
    __Boxed<int> width = (ValueType) currentResolution.width;
    currentResolution = Screen.currentResolution;
    // ISSUE: variable of a boxed type
    __Boxed<int> height = (ValueType) currentResolution.height;
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Screen Resolution: {0}x{1}", (object) width, (object) height));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Screen DPI: {0}", (object) Screen.dpi));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - GPU ID: {0}", (object) SystemInfo.graphicsDeviceID));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - GPU Name: {0}", (object) SystemInfo.graphicsDeviceName));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - GPU Vendor: {0}", (object) SystemInfo.graphicsDeviceVendor));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - GPU Memory: {0}", (object) SystemInfo.graphicsMemorySize));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - GPU Shader Level: {0}", (object) SystemInfo.graphicsShaderLevel));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - GPU NPOT Support: {0}", (object) SystemInfo.npotSupport));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics API (version): {0}", (object) SystemInfo.graphicsDeviceVersion));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics API (type): {0}", (object) SystemInfo.graphicsDeviceType));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics Supported Render Target Count: {0}", (object) SystemInfo.supportedRenderTargetCount));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics Supports 3D Textures: {0}", (object) SystemInfo.supports3DTextures));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics Supports Compute Shaders: {0}", (object) SystemInfo.supportsComputeShaders));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics Supports Shadows: {0}", (object) SystemInfo.supportsShadows));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics Supports Sparse Textures: {0}", (object) SystemInfo.supportsSparseTextures));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics RenderTextureFormat.ARGBHalf: {0}", (object) SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)));
    UnityEngine.Debug.Log((object) string.Format("SystemInfo - Graphics Metal Support: {0}", (object) SystemInfo.graphicsDeviceVersion.StartsWith("Metal")));
  }

  private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

  private struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
  }
}
