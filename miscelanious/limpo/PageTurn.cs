using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PageTurn : MonoBehaviour
{
  private readonly string FRONT_PAGE_NAME = "PageTurnFront";
  private readonly string BACK_PAGE_NAME = "PageTurnBack";
  private readonly string WAIT_THEN_COMPLETE_PAGE_TURN_RIGHT_COROUTINE = "WaitThenCompletePageTurnRight";
  private readonly string PAGE_TURN_LEFT_ANIM = "PageTurnLeft";
  private readonly string PAGE_TURN_RIGHT_ANIM = "PageTurnRight";
  private IGraphicsManager m_graphicsManager;
  public Shader m_MaskShader;
  public float m_TurnLeftSpeed = 1.65f;
  public float m_TurnRightSpeed = 1.65f;
  public float m_TurnLeftDelayBeforePositioningPages = 0.44f;
  private Bounds m_RenderBounds;
  private Camera m_OffscreenPageTurnCamera;
  private GameObject m_OffscreenPageTurnCameraGO;
  private RenderTexture m_TempRenderBuffer;
  private GameObject m_MeshGameObject;
  private GameObject m_FrontPageGameObject;
  private GameObject m_BackPageGameObject;
  private GameObject m_TheBoxOuterFrame;
  private float m_RenderOffset = 500f;
  private Vector3 m_initialPosition;
  private bool m_RenderRequested;
  private PageTurn.TurnPageData m_LeftTurnRenderData;

  private void RequestRightTurnRender() => this.m_RenderRequested = true;

  private void RequestLeftTurnRender(PageTurn.TurnPageData data)
  {
    this.m_LeftTurnRenderData = data;
    this.m_RenderRequested = true;
  }

  private bool RenderingIsDone() => !this.m_RenderRequested;

  private void OnBeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
  {
    if (!this.m_RenderRequested)
      return;
    Vector3 vector3_1 = Vector3.zero;
    Vector3 vector3_2 = Vector3.zero;
    if (this.m_LeftTurnRenderData != null)
    {
      GameObject flippingPage = this.m_LeftTurnRenderData.flippingPage;
      GameObject otherPage = this.m_LeftTurnRenderData.otherPage;
      vector3_1 = flippingPage.transform.position;
      vector3_2 = otherPage.transform.position;
      flippingPage.transform.position = vector3_2;
      otherPage.transform.position = vector3_1;
    }
    this.Show(true);
    this.m_FrontPageGameObject.SetActive(true);
    this.m_BackPageGameObject.SetActive(true);
    this.SetCameraSize(this.m_OffscreenPageTurnCamera);
    this.m_OffscreenPageTurnCameraGO.transform.position = this.transform.position;
    Renderer component1 = this.m_FrontPageGameObject.GetComponent<Renderer>();
    int num1 = component1.enabled ? 1 : 0;
    Renderer component2 = this.m_BackPageGameObject.GetComponent<Renderer>();
    int num2 = component2.enabled ? 1 : 0;
    component1.enabled = false;
    component2.enabled = false;
    bool activeSelf = this.m_TheBoxOuterFrame.activeSelf;
    this.m_TheBoxOuterFrame.SetActive(false);
    UniversalRenderPipeline.RenderSingleCamera(context, this.m_OffscreenPageTurnCamera);
    this.m_TheBoxOuterFrame.SetActive(activeSelf);
    if (this.m_LeftTurnRenderData != null)
    {
      this.m_LeftTurnRenderData.flippingPage.transform.position = vector3_1;
      this.m_LeftTurnRenderData.otherPage.transform.position = vector3_2;
    }
    this.m_RenderRequested = false;
    this.m_LeftTurnRenderData = (PageTurn.TurnPageData) null;
  }

  private void Awake()
  {
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.m_initialPosition = this.transform.localPosition;
    Transform transform1 = this.transform.Find(this.FRONT_PAGE_NAME);
    if ((UnityEngine.Object) transform1 != (UnityEngine.Object) null)
      this.m_FrontPageGameObject = transform1.gameObject;
    if ((UnityEngine.Object) this.m_FrontPageGameObject == (UnityEngine.Object) null)
      Debug.LogError((object) ("Failed to find " + this.FRONT_PAGE_NAME + " Object."));
    Transform transform2 = this.transform.Find(this.BACK_PAGE_NAME);
    if ((UnityEngine.Object) transform2 != (UnityEngine.Object) null)
      this.m_BackPageGameObject = transform2.gameObject;
    if ((UnityEngine.Object) this.m_BackPageGameObject == (UnityEngine.Object) null)
      Debug.LogError((object) ("Failed to find " + this.BACK_PAGE_NAME + " Object."));
    this.Show(false);
    this.m_TheBoxOuterFrame = Box.Get().m_OuterFrame;
    this.CreateCamera();
    this.CreateRenderTexture();
    this.SetupMaterial();
  }

  protected void OnEnable()
  {
    if ((UnityEngine.Object) this.m_OffscreenPageTurnCameraGO != (UnityEngine.Object) null)
      this.CreateCamera();
    if ((UnityEngine.Object) this.m_TempRenderBuffer != (UnityEngine.Object) null)
    {
      this.CreateRenderTexture();
      this.SetupMaterial();
    }
    RenderPipelineManager.beginFrameRendering += new Action<ScriptableRenderContext, Camera[]>(this.OnBeginFrameRendering);
  }

  protected void OnDisable()
  {
    if ((UnityEngine.Object) this.m_TempRenderBuffer != (UnityEngine.Object) null)
      RenderTextureTracker.Get().DestroyRenderTexture(this.m_TempRenderBuffer);
    if ((UnityEngine.Object) this.m_OffscreenPageTurnCameraGO != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_OffscreenPageTurnCameraGO);
    if ((UnityEngine.Object) this.m_OffscreenPageTurnCamera != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_OffscreenPageTurnCamera);
    RenderPipelineManager.beginFrameRendering -= new Action<ScriptableRenderContext, Camera[]>(this.OnBeginFrameRendering);
  }

  public void TurnRight(
    GameObject flippingPage,
    GameObject otherPage,
    PageTurn.DelOnPageTurnComplete pageTurnCompleteCallback,
    PageTurn.DelPositionPages positionPagesCallback,
    object callbackData)
  {
    this.RequestRightTurnRender();
    Time.captureFramerate = this.m_graphicsManager.RenderQualityLevel != GraphicsQuality.Low ? (this.m_graphicsManager.RenderQualityLevel != GraphicsQuality.Medium ? 30 : 24) : 18;
    Animation component = this.GetComponent<Animation>();
    component.Stop(this.PAGE_TURN_RIGHT_ANIM);
    this.m_FrontPageGameObject.GetComponent<Renderer>().GetMaterial().SetFloat("_Alpha", 1f);
    this.m_BackPageGameObject.GetComponent<Renderer>().GetMaterial().SetFloat("_Alpha", 1f);
    float num = component[this.PAGE_TURN_RIGHT_ANIM].length / this.m_TurnRightSpeed;
    PageTurn.PageTurningData pageTurningData = new PageTurn.PageTurningData()
    {
      m_secondsToWait = num,
      m_pageTurnCompleteCallback = pageTurnCompleteCallback,
      m_callbackData = callbackData,
      m_positionPagesCallback = positionPagesCallback
    };
    this.StopCoroutine(this.WAIT_THEN_COMPLETE_PAGE_TURN_RIGHT_COROUTINE);
    this.StartCoroutine(this.WAIT_THEN_COMPLETE_PAGE_TURN_RIGHT_COROUTINE, (object) pageTurningData);
  }

  public void TurnLeft(
    GameObject flippingPage,
    GameObject otherPage,
    PageTurn.DelOnPageTurnComplete pageTurnCompleteCallback,
    PageTurn.DelPositionPages positionPagesCallback,
    object callbackData)
  {
    PageTurn.TurnPageData turnPageData = new PageTurn.TurnPageData();
    turnPageData.flippingPage = flippingPage;
    turnPageData.otherPage = otherPage;
    turnPageData.pageTurnCompleteCallback = pageTurnCompleteCallback;
    turnPageData.positionPagesCallback = positionPagesCallback;
    turnPageData.callbackData = callbackData;
    this.StopCoroutine("TurnLeftPage");
    this.StartCoroutine("TurnLeftPage", (object) turnPageData);
  }

  private IEnumerator TurnLeftPage(PageTurn.TurnPageData pageData)
  {
    PageTurn pageTurn = this;
    yield return (object) null;
    yield return (object) null;
    yield return (object) null;
    GameObject flippingPage = pageData.flippingPage;
    GameObject otherPage = pageData.otherPage;
    PageTurn.DelOnPageTurnComplete pageTurnCompleteCallback = pageData.pageTurnCompleteCallback;
    PageTurn.DelPositionPages positionPagesCallback = pageData.positionPagesCallback;
    object callbackData = pageData.callbackData;
    pageTurn.RequestLeftTurnRender(pageData);
    while (!pageTurn.RenderingIsDone())
      yield return (object) null;
    Time.captureFramerate = pageTurn.m_graphicsManager.RenderQualityLevel != GraphicsQuality.Low ? (pageTurn.m_graphicsManager.RenderQualityLevel != GraphicsQuality.Medium ? 30 : 24) : 18;
    Renderer component1 = pageTurn.m_FrontPageGameObject.GetComponent<Renderer>();
    Renderer component2 = pageTurn.m_BackPageGameObject.GetComponent<Renderer>();
    component1.enabled = true;
    component1.GetMaterial().SetFloat("_Alpha", 1f);
    component2.enabled = true;
    component2.GetMaterial().SetFloat("_Alpha", 1f);
    Animation pageAnimation = pageTurn.GetComponent<Animation>();
    pageAnimation.Stop(pageTurn.PAGE_TURN_LEFT_ANIM);
    pageAnimation[pageTurn.PAGE_TURN_LEFT_ANIM].time = 0.22f;
    pageAnimation[pageTurn.PAGE_TURN_LEFT_ANIM].speed = pageTurn.m_TurnLeftSpeed;
    pageAnimation.Play(pageTurn.PAGE_TURN_LEFT_ANIM);
    while ((double) pageAnimation[pageTurn.PAGE_TURN_LEFT_ANIM].time < (double) Math.Min(pageAnimation[pageTurn.PAGE_TURN_LEFT_ANIM].length, pageTurn.m_TurnLeftDelayBeforePositioningPages))
      yield return (object) null;
    if (positionPagesCallback != null)
      positionPagesCallback(callbackData);
    PageTurn.PageTurningData pageTurningData = new PageTurn.PageTurningData()
    {
      m_secondsToWait = 0.0f,
      m_pageTurnCompleteCallback = pageTurnCompleteCallback,
      m_callbackData = callbackData,
      m_animation = pageAnimation[pageTurn.PAGE_TURN_LEFT_ANIM]
    };
    pageTurn.StartCoroutine(pageTurn.WaitThenCompletePageTurnLeft(pageTurningData));
  }

  private IEnumerator WaitThenCompletePageTurnLeft(
    PageTurn.PageTurningData pageTurningData)
  {
    PageTurn pageTurn = this;
    while (pageTurn.GetComponent<Animation>().isPlaying)
      yield return (object) null;
    Time.captureFramerate = 0;
    pageTurn.Show(false);
    if (pageTurningData.m_pageTurnCompleteCallback != null)
      pageTurningData.m_pageTurnCompleteCallback(pageTurningData.m_callbackData);
  }

  private void CreateCamera()
  {
    if (!((UnityEngine.Object) this.m_OffscreenPageTurnCameraGO == (UnityEngine.Object) null))
      return;
    if ((UnityEngine.Object) this.m_OffscreenPageTurnCamera != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_OffscreenPageTurnCamera);
    this.m_OffscreenPageTurnCameraGO = new GameObject();
    this.m_OffscreenPageTurnCamera = this.m_OffscreenPageTurnCameraGO.AddComponent<Camera>();
    this.m_OffscreenPageTurnCameraGO.name = this.name + "_OffScreenPageTurnCamera";
    this.SetupCamera(this.m_OffscreenPageTurnCamera);
  }

  private void SetupCamera(Camera camera)
  {
    camera.orthographic = true;
    camera.transform.parent = this.transform;
    camera.nearClipPlane = -20f;
    camera.farClipPlane = 20f;
    camera.depth = (UnityEngine.Object) Camera.main == (UnityEngine.Object) null ? 0.0f : Camera.main.depth + 100f;
    camera.backgroundColor = Color.clear;
    camera.clearFlags = CameraClearFlags.Color;
    camera.cullingMask = GameLayer.Default.LayerBit() | GameLayer.CardRaycast.LayerBit();
    camera.enabled = false;
    camera.renderingPath = RenderingPath.Forward;
    camera.allowHDR = false;
    UniversalAdditionalCameraData component;
    camera.TryGetComponent<UniversalAdditionalCameraData>(out component);
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      component = camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
    component.SetRenderer(3);
    camera.transform.Rotate(90f, 0.0f, 0.0f);
    GameObjectUtils.SetHideFlags((UnityEngine.Object) camera, HideFlags.HideAndDontSave);
  }

  private void CreateRenderTexture()
  {
    int num1 = 512;
    GraphicsQuality renderQualityLevel = this.m_graphicsManager.RenderQualityLevel;
    switch (renderQualityLevel)
    {
      case GraphicsQuality.Medium:
        num1 = 1024;
        break;
      case GraphicsQuality.High:
        Resolution currentResolution = Screen.currentResolution;
        int width = currentResolution.width;
        currentResolution = Screen.currentResolution;
        int height = currentResolution.height;
        int num2 = Math.Max(width, height);
        if (num2 >= 4096)
        {
          num1 = 4096;
          break;
        }
        if (num2 >= 2048)
        {
          num1 = 2048;
          break;
        }
        if (num2 >= 1024)
        {
          num1 = 1024;
          break;
        }
        break;
    }
    if ((UnityEngine.Object) this.m_TempRenderBuffer == (UnityEngine.Object) null)
    {
      if (renderQualityLevel == GraphicsQuality.High)
      {
        this.m_TempRenderBuffer = RenderTextureTracker.Get().CreateNewTexture(num1, num1, 16, RenderTextureFormat.ARGB32);
      }
      else
      {
        int num3 = !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB1555) ? 0 : (PlatformSettings.RuntimeOS != OSCategory.Mac ? 1 : 0);
        bool flag = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB4444) && PlatformSettings.RuntimeOS != OSCategory.PC;
        this.m_TempRenderBuffer = num3 == 0 ? (!(renderQualityLevel == GraphicsQuality.Low & flag) ? RenderTextureTracker.Get().CreateNewTexture(num1, num1, 16) : RenderTextureTracker.Get().CreateNewTexture(num1, num1, 16, RenderTextureFormat.ARGB4444)) : RenderTextureTracker.Get().CreateNewTexture(num1, num1, 16, RenderTextureFormat.ARGB1555);
      }
      this.m_TempRenderBuffer.Create();
    }
    if (!((UnityEngine.Object) this.m_OffscreenPageTurnCamera != (UnityEngine.Object) null))
      return;
    this.m_OffscreenPageTurnCamera.targetTexture = this.m_TempRenderBuffer;
  }

  private void SetCameraSize(Camera camera) => camera.orthographicSize = PageTurn.GetWorldScale(this.m_FrontPageGameObject.transform).x / 2f;

  public void SetBackPageMaterial(Material material) => this.m_BackPageGameObject.GetComponent<Renderer>().SetMaterial(material);

  private void SetupMaterial()
  {
    Material material = this.m_FrontPageGameObject.GetComponent<Renderer>().GetMaterial();
    material.mainTexture = (Texture) this.m_TempRenderBuffer;
    material.renderQueue = 3001;
    this.m_BackPageGameObject.GetComponent<Renderer>().GetMaterial().renderQueue = 3002;
  }

  private void Show(bool show) => this.transform.localPosition = show ? this.m_initialPosition : Vector3.right * this.m_RenderOffset;

  private IEnumerator WaitThenCompletePageTurnRight(
    PageTurn.PageTurningData pageTurningData)
  {
    PageTurn pageTurn = this;
    while (!pageTurn.RenderingIsDone())
      yield return (object) null;
    pageTurn.m_FrontPageGameObject.GetComponent<Renderer>().enabled = true;
    pageTurn.m_BackPageGameObject.GetComponent<Renderer>().enabled = true;
    Animation component = pageTurn.GetComponent<Animation>();
    component[pageTurn.PAGE_TURN_RIGHT_ANIM].time = 0.0f;
    component[pageTurn.PAGE_TURN_RIGHT_ANIM].speed = pageTurn.m_TurnRightSpeed;
    component.Play(pageTurn.PAGE_TURN_RIGHT_ANIM);
    if (pageTurningData.m_positionPagesCallback != null)
      pageTurningData.m_positionPagesCallback(pageTurningData.m_callbackData);
    yield return (object) new WaitForSeconds(pageTurningData.m_secondsToWait);
    Time.captureFramerate = 0;
    pageTurn.Show(false);
    if (pageTurningData.m_pageTurnCompleteCallback != null)
      pageTurningData.m_pageTurnCompleteCallback(pageTurningData.m_callbackData);
  }

  public static Vector3 GetWorldScale(Transform transform)
  {
    Vector3 a = transform.localScale;
    for (Transform parent = transform.parent; (UnityEngine.Object) parent != (UnityEngine.Object) null; parent = parent.parent)
      a = Vector3.Scale(a, parent.localScale);
    return a;
  }

  public delegate void DelOnPageTurnComplete(object callbackData);

  public delegate void DelPositionPages(object callbackData);

  private class PageTurningData
  {
    public float m_secondsToWait;
    public PageTurn.DelOnPageTurnComplete m_pageTurnCompleteCallback;
    public object m_callbackData;
    public AnimationState m_animation;
    public PageTurn.DelPositionPages m_positionPagesCallback;
  }

  private class TurnPageData
  {
    public GameObject flippingPage;
    public GameObject otherPage;
    public PageTurn.DelOnPageTurnComplete pageTurnCompleteCallback;
    public PageTurn.DelPositionPages positionPagesCallback;
    public object callbackData;
  }
}
