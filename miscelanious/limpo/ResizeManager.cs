using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResizeManager
{
  private Action<int, int> m_onResolutionChanged;
  private bool m_lastFullScreen;
  private int m_lastWindowedWidth;
  private int m_lastWindowedHeight;
  private int m_lastWidth;
  private int m_lastHeight;
  private float m_lastChangedResolutionTime = float.MinValue;

  public ResizeManager(Action<int, int> onResolutionChanged)
  {
    this.m_onResolutionChanged = onResolutionChanged;
    bool fullscreen = Options.Get().GetBool(Option.GFX_FULLSCREEN, true);
    this.m_lastFullScreen = fullscreen;
    int width1;
    int height1;
    if (fullscreen)
    {
      Options options1 = Options.Get();
      Resolution currentResolution = Screen.currentResolution;
      int width2 = currentResolution.width;
      width1 = options1.GetInt(Option.GFX_WIDTH, width2);
      Options options2 = Options.Get();
      currentResolution = Screen.currentResolution;
      int height2 = currentResolution.height;
      height1 = options2.GetInt(Option.GFX_HEIGHT, height2);
      currentResolution = Screen.currentResolution;
      this.m_lastWindowedWidth = (int) ((double) currentResolution.width * 0.75);
      currentResolution = Screen.currentResolution;
      this.m_lastWindowedHeight = (int) ((double) currentResolution.height * 0.75);
      if (!Options.Get().HasOption(Option.GFX_WIDTH) || !Options.Get().HasOption(Option.GFX_HEIGHT))
      {
        string intelDeviceName = ServiceManager.Get<ITouchScreenService>().GetIntelDeviceName();
        if (intelDeviceName != null && (intelDeviceName.Contains("Haswell") && intelDeviceName.Contains("Y6W") || intelDeviceName.Contains("Haswell") && intelDeviceName.Contains("U15W")))
        {
          currentResolution = Screen.currentResolution;
          if (currentResolution.height >= 1080)
          {
            width1 = 1920;
            height1 = 1080;
          }
        }
      }
      int num1 = width1;
      currentResolution = Screen.currentResolution;
      int width3 = currentResolution.width;
      if (num1 == width3)
      {
        int num2 = height1;
        currentResolution = Screen.currentResolution;
        int height3 = currentResolution.height;
        if (num2 == height3 && fullscreen == Screen.fullScreen)
          return;
      }
    }
    else
    {
      Options options3 = Options.Get();
      Resolution currentResolution = Screen.currentResolution;
      int defaultVal1 = (int) ((double) currentResolution.width * 0.75);
      width1 = options3.GetInt(Option.GFX_WIDTH, defaultVal1);
      Options options4 = Options.Get();
      currentResolution = Screen.currentResolution;
      int defaultVal2 = (int) ((double) currentResolution.height * 0.75);
      height1 = options4.GetInt(Option.GFX_HEIGHT, defaultVal2);
      this.m_lastWindowedWidth = width1;
      this.m_lastWindowedHeight = height1;
    }
    this.SetScreenResolution(width1, height1, fullscreen, true);
    this.m_lastWidth = Screen.width;
    this.m_lastHeight = Screen.height;
  }

  public void Update()
  {
    if (Screen.fullScreen && !this.m_lastFullScreen)
    {
      this.m_lastFullScreen = true;
      GraphicsResolution largestResolution = GraphicsResolution.GetLargestResolution();
      Screen.SetResolution(largestResolution.x, largestResolution.y, true);
      this.m_onResolutionChanged(largestResolution.x, largestResolution.y);
      Options.Get().SetBool(Option.GFX_FULLSCREEN, Screen.fullScreen);
      Options.Get().SetInt(Option.GFX_WIDTH, largestResolution.x);
      Options.Get().SetInt(Option.GFX_HEIGHT, largestResolution.y);
    }
    else if (!Screen.fullScreen && this.m_lastFullScreen)
    {
      this.m_lastFullScreen = false;
      if (this.m_lastWindowedWidth > 0 && this.m_lastWindowedHeight > 0)
      {
        Screen.SetResolution(this.m_lastWindowedWidth, this.m_lastWindowedHeight, false);
        this.m_onResolutionChanged(this.m_lastWindowedWidth, this.m_lastWindowedHeight);
        Options.Get().SetBool(Option.GFX_FULLSCREEN, Screen.fullScreen);
        Options.Get().SetInt(Option.GFX_WIDTH, this.m_lastWindowedWidth);
        Options.Get().SetInt(Option.GFX_HEIGHT, this.m_lastWindowedHeight);
      }
      else
      {
        int num1 = (int) ((double) Screen.currentResolution.width * 0.75);
        int num2 = (int) ((double) Screen.currentResolution.height * 0.75);
        if (!GraphicsResolution.IsAspectRatioWithinLimit(num1, num2, !Screen.fullScreen))
        {
          int[] numArray = GraphicsResolution.CalcAspectRatioLimit(num1, num2);
          num1 = numArray[0];
          num2 = numArray[1];
        }
        Screen.SetResolution(num1, num2, false);
        this.m_onResolutionChanged(num1, num2);
        Options.Get().SetBool(Option.GFX_FULLSCREEN, Screen.fullScreen);
        Options.Get().SetInt(Option.GFX_WIDTH, num1);
        Options.Get().SetInt(Option.GFX_HEIGHT, num2);
      }
    }
    else
    {
      int width = Screen.width;
      int height = Screen.height;
      if (!GraphicsResolution.IsAspectRatioWithinLimit(width, height, !Screen.fullScreen))
      {
        int[] numArray = GraphicsResolution.CalcAspectRatioLimit(width, height);
        width = numArray[0];
        height = numArray[1];
      }
      this.m_lastFullScreen = Screen.fullScreen;
      if (this.m_lastFullScreen)
        return;
      if (this.m_lastWidth != width || this.m_lastHeight != height)
        this.m_onResolutionChanged(width, height);
      this.m_lastWidth = width;
      this.m_lastHeight = height;
      this.m_lastWindowedWidth = width;
      this.m_lastWindowedHeight = height;
      if (this.m_lastWidth == Screen.width && this.m_lastHeight == Screen.height)
      {
        this.m_lastChangedResolutionTime = Time.time;
      }
      else
      {
        if ((double) this.m_lastChangedResolutionTime + 1.0 >= (double) Time.time || (double) this.m_lastChangedResolutionTime + 1.0 <= (double) Time.time - (double) Time.deltaTime)
          return;
        this.SetScreenResolution(width, height, Screen.fullScreen);
        Options.Get().SetInt(Option.GFX_WIDTH, width);
        Options.Get().SetInt(Option.GFX_HEIGHT, height);
      }
    }
  }

  public void SetScreenResolution(int width, int height, bool fullscreen) => this.SetScreenResolution(width, height, fullscreen, false);

  public void SetScreenResolution(int width, int height, bool fullscreen, bool fadeToBlack)
  {
    Resolution currentResolution;
    if (height > Screen.currentResolution.height && !fullscreen)
    {
      currentResolution = Screen.currentResolution;
      height = currentResolution.height;
    }
    int num = width;
    currentResolution = Screen.currentResolution;
    int width1 = currentResolution.width;
    if (num > width1 && !fullscreen)
    {
      currentResolution = Screen.currentResolution;
      width = currentResolution.width;
    }
    if (fullscreen && fullscreen != this.m_lastFullScreen)
    {
      currentResolution = Screen.currentResolution;
      height = currentResolution.height;
      currentResolution = Screen.currentResolution;
      width = currentResolution.width;
    }
    Processor.QueueJob("ResizeManager.SetRes", this.Job_SetRes(width, height, fullscreen, fadeToBlack));
  }

  private IEnumerator<IAsyncJobResult> Job_SetRes(
    int width,
    int height,
    bool fullscreen,
    bool fadeToBlack)
  {
    yield return (IAsyncJobResult) ServiceManager.CreateServiceSoftDependency(typeof (SceneMgr));
    SceneMgr service;
    LoadingScreen loadingScreen = !ServiceManager.TryGet<SceneMgr>(out service) ? UnityEngine.Object.FindObjectOfType<LoadingScreen>() : service.LoadingScreen;
    CameraFade cameraFade = loadingScreen.GetCameraFade();
    Camera camera = loadingScreen.GetFxCamera();
    float prevDepth = camera.depth;
    Color prevColor = cameraFade.m_Color;
    float prevFade = cameraFade.m_Fade;
    if (!fadeToBlack)
    {
      cameraFade.m_Color = Color.black;
      cameraFade.m_Fade = 1f;
    }
    yield return (IAsyncJobResult) null;
    if (!GraphicsResolution.IsAspectRatioWithinLimit(width, height, !Screen.fullScreen))
    {
      int[] numArray = GraphicsResolution.CalcAspectRatioLimit(width, height);
      width = numArray[0];
      height = numArray[1];
    }
    if (fullscreen != this.m_lastFullScreen)
    {
      if (fullscreen)
      {
        width = Screen.currentResolution.width;
        height = Screen.currentResolution.height;
      }
      else
      {
        width = this.m_lastWindowedWidth;
        height = this.m_lastWindowedHeight;
      }
    }
    this.m_lastFullScreen = fullscreen;
    Screen.SetResolution(width, height, fullscreen);
    yield return (IAsyncJobResult) null;
    Screen.SetResolution(width, height, fullscreen);
    this.m_lastWidth = Screen.width;
    this.m_lastHeight = Screen.height;
    if (!fullscreen)
    {
      this.m_lastWindowedWidth = width;
      this.m_lastWindowedHeight = height;
    }
    camera.depth = prevDepth;
    cameraFade.m_Color = prevColor;
    cameraFade.m_Fade = prevFade;
    this.m_onResolutionChanged(width, height);
  }
}
