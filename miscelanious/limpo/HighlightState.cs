using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Cysharp.Threading.Tasks;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

public class HighlightState : MonoBehaviour
{
  private readonly string HIGHLIGHT_SHADER_NAME = "Custom/Selection/Highlight";
  private const string FSM_BIRTH_STATE = "Birth";
  private const string FSM_IDLE_STATE = "Idle";
  private const string FSM_DEATH_STATE = "Death";
  private const string FSM_BIRTHTRANSITION_STATE = "BirthTransition";
  private const string FSM_IDLETRANSITION_STATE = "IdleTransition";
  private const string FSM_DEATHTRANSITION_STATE = "DeathTransition";
  public GameObject m_RenderPlane;
  public HighlightStateType m_highlightType;
  public Texture2D m_StaticSilouetteTexture;
  public Texture2D m_StaticSilouetteTextureUnique;
  [NonSerialized]
  public Texture2D m_StaticSilouetteOverride;
  [FormerlySerializedAs("m_MultiClassStaticSilouetteTexture")]
  public Texture2D m_TriClassBannerStaticSilouetteTexture;
  [FormerlySerializedAs("m_MultiClassStaticSilouetteTextureUnique")]
  public Texture2D m_TriClassBannerStaticSilouetteTextureUnique;
  public Texture2D m_BattlegroundQuestSiloutteTexture;
  public Texture2D m_DeathKnightRuneBannerSilhouetteTexture;
  public Texture2D m_DeathKnightRuneBannerSilhouetteTextureUnique;
  public Vector3 m_HistoryTranslation = new Vector3(0.0f, -0.1f, 0.0f);
  public int m_RenderQueue;
  public int m_RenderQueueOffset = 3000;
  public List<HighlightRenderState> m_HighlightStates;
  public ActorStateType m_debugState;
  protected ActorStateType m_PreviousState;
  protected ActorStateType m_CurrentState;
  protected PlayMakerFSM m_FSM;
  private string m_sendEvent;
  private bool m_isDirty;
  private bool m_forceRerender;
  private string m_BirthTransition = "None";
  private string m_SecondBirthTransition = "None";
  private string m_IdleTransition = "None";
  private string m_DeathTransition = "None";
  private bool m_Hide;
  private bool m_VisibilityState;
  private float m_seed;
  private Material m_Material;
  private Renderer m_renderer;
  private HighlightRender m_highlightRender;
  private bool m_RenderersInitialized;
  private CancellationTokenSource m_tokenSource;
  private IGraphicsManager m_graphicsManager;

  private void Awake()
  {
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.TryInitRenderers();
    if ((UnityEngine.Object) this.m_FSM != (UnityEngine.Object) null)
      this.m_FSM.enabled = true;
    if (this.m_highlightType == HighlightStateType.NONE)
    {
      Transform parent = this.transform.parent;
      if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
        this.m_highlightType = !(bool) (UnityEngine.Object) parent.GetComponent<ActorStateMgr>() ? HighlightStateType.HIGHLIGHT : HighlightStateType.CARD;
    }
    if (this.m_highlightType == HighlightStateType.NONE)
    {
      Debug.LogError((object) "m_highlightType is not set!");
      this.enabled = false;
    }
    this.Setup();
    if (this.m_tokenSource != null)
      return;
    this.m_tokenSource = new CancellationTokenSource();
  }

  private void Update()
  {
    if (this.m_debugState != ActorStateType.NONE)
    {
      this.ChangeState(this.m_debugState);
      this.ForceUpdate();
    }
    if (this.m_Hide)
    {
      if ((UnityEngine.Object) this.m_RenderPlane == (UnityEngine.Object) null)
        return;
      this.m_renderer.enabled = false;
    }
    else
    {
      if (!this.m_isDirty || (UnityEngine.Object) this.m_RenderPlane == (UnityEngine.Object) null || !this.m_renderer.enabled)
        return;
      this.UpdateSilouette();
      this.m_isDirty = false;
    }
  }

  private void OnApplicationFocus(bool state)
  {
    this.m_isDirty = true;
    this.m_forceRerender = true;
  }

  protected void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.m_Material)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_Material);
    this.m_tokenSource?.Cancel();
    this.m_tokenSource?.Dispose();
  }

  private void Setup()
  {
    this.m_seed = UnityEngine.Random.value;
    this.m_CurrentState = ActorStateType.CARD_IDLE;
    Renderer component = this.m_RenderPlane.GetComponent<Renderer>();
    component.enabled = false;
    this.m_VisibilityState = false;
    if ((UnityEngine.Object) this.m_Material == (UnityEngine.Object) null)
    {
      Shader shader = ShaderUtils.FindShader(this.HIGHLIGHT_SHADER_NAME);
      if (!(bool) (UnityEngine.Object) shader)
      {
        Debug.LogError((object) ("Failed to load Highlight Shader: " + this.HIGHLIGHT_SHADER_NAME));
        this.enabled = false;
      }
      this.m_Material = new Material(shader);
    }
    component.SetSharedMaterial(this.m_Material);
  }

  public void Show()
  {
    this.m_Hide = false;
    if (!((UnityEngine.Object) this.m_renderer != (UnityEngine.Object) null) || !this.m_VisibilityState || this.m_renderer.enabled)
      return;
    this.m_renderer.enabled = true;
  }

  public void Hide()
  {
    this.m_Hide = true;
    if ((UnityEngine.Object) this.m_renderer == (UnityEngine.Object) null)
      return;
    this.m_renderer.enabled = false;
  }

  public void SetDirty() => this.m_isDirty = true;

  public void ForceUpdate()
  {
    this.m_isDirty = true;
    this.m_forceRerender = true;
  }

  public void ContinuousUpdate(float updateTime) => this.ContinuousSilouetteRender(updateTime, this.m_tokenSource.Token).Forget();

  public bool IsReady() => (UnityEngine.Object) this.m_Material != (UnityEngine.Object) null;

  public bool ChangeState(ActorStateType stateType)
  {
    if (stateType == this.m_CurrentState)
      return true;
    this.m_PreviousState = this.m_CurrentState;
    this.m_CurrentState = stateType;
    this.TryInitRenderers();
    if ((UnityEngine.Object) this.m_renderer == (UnityEngine.Object) null)
    {
      this.m_VisibilityState = false;
      return true;
    }
    switch (stateType)
    {
      case ActorStateType.NONE:
        this.m_renderer.enabled = false;
        this.m_VisibilityState = false;
        return true;
      case ActorStateType.CARD_IDLE:
      case ActorStateType.HIGHLIGHT_OFF:
        if ((UnityEngine.Object) this.m_FSM == (UnityEngine.Object) null)
        {
          this.m_renderer.enabled = false;
          this.m_VisibilityState = false;
          return true;
        }
        this.m_DeathTransition = this.m_PreviousState.ToString();
        this.SendDataToPlaymaker();
        this.SendPlaymakerDeathEvent();
        return true;
      default:
        foreach (HighlightRenderState highlightState in this.m_HighlightStates)
        {
          if (highlightState.m_StateType == stateType)
          {
            if ((UnityEngine.Object) highlightState.m_Material != (UnityEngine.Object) null && (UnityEngine.Object) this.m_Material != (UnityEngine.Object) null)
            {
              this.m_Material.CopyPropertiesFromMaterial(highlightState.m_Material);
              this.m_renderer.SetSharedMaterial(this.m_Material);
              this.m_renderer.GetSharedMaterial().SetFloat("_Seed", this.m_seed);
              int num = this.RenderSilouette() ? 1 : 0;
              if (stateType == ActorStateType.CARD_HISTORY)
                this.transform.localPosition = this.m_HistoryTranslation;
              else
                this.transform.localPosition = highlightState.m_Offset;
              if ((UnityEngine.Object) this.m_FSM == (UnityEngine.Object) null)
              {
                if (!this.m_Hide)
                  this.m_renderer.enabled = true;
                this.m_VisibilityState = true;
              }
              else
              {
                this.m_BirthTransition = stateType.ToString();
                this.m_SecondBirthTransition = this.m_PreviousState.ToString();
                this.m_IdleTransition = this.m_BirthTransition;
                this.SendDataToPlaymaker();
                this.SendPlaymakerBirthEvent();
              }
              return num != 0;
            }
            this.m_renderer.enabled = false;
            this.m_VisibilityState = false;
            return true;
          }
        }
        if (this.m_highlightType == HighlightStateType.CARD)
          this.m_CurrentState = ActorStateType.CARD_IDLE;
        else if (this.m_highlightType == HighlightStateType.HIGHLIGHT)
          this.m_CurrentState = ActorStateType.HIGHLIGHT_OFF;
        this.m_DeathTransition = this.m_PreviousState.ToString();
        this.SendDataToPlaymaker();
        this.SendPlaymakerDeathEvent();
        this.m_renderer.enabled = false;
        this.m_VisibilityState = false;
        return false;
    }
  }

  public ActorStateType CurrentState => this.m_CurrentState;

  protected void UpdateSilouette() => this.RenderSilouette();

  private bool RenderSilouette()
  {
    this.m_isDirty = false;
    Texture2D texture2D1 = this.m_StaticSilouetteOverride ?? this.m_StaticSilouetteTexture;
    if ((UnityEngine.Object) texture2D1 != (UnityEngine.Object) null)
    {
      Texture2D texture2D2 = texture2D1;
      Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>(this.gameObject);
      if ((UnityEngine.Object) componentInParents != (UnityEngine.Object) null)
      {
        CardSilhouetteOverride silhouetteOverride = componentInParents.CardSilhouetteOverride;
        bool flag1 = componentInParents.IsElite();
        bool flag2 = componentInParents.IsMultiClass() && (componentInParents.GetCardSet() == TAG_CARD_SET.GANGS || componentInParents.GetCardSet() == TAG_CARD_SET.GANGS_RESERVE);
        bool flag3 = componentInParents.IsTradeable();
        bool flag4 = componentInParents.HasRuneCost();
        bool flag5 = componentInParents.UseBGQuestSiloutte();
        switch (silhouetteOverride)
        {
          case CardSilhouetteOverride.SingleClass:
            flag2 = false;
            break;
          case CardSilhouetteOverride.TriClassBanner:
            flag2 = true;
            break;
        }
        if (flag1 && (UnityEngine.Object) this.m_StaticSilouetteTextureUnique != (UnityEngine.Object) null)
          texture2D2 = this.m_StaticSilouetteTextureUnique;
        if (flag2 | flag3 | flag4 && (UnityEngine.Object) this.m_TriClassBannerStaticSilouetteTexture != (UnityEngine.Object) null)
          texture2D2 = this.m_TriClassBannerStaticSilouetteTexture;
        if (flag1 && flag2 | flag3 | flag4 && (UnityEngine.Object) this.m_TriClassBannerStaticSilouetteTextureUnique != (UnityEngine.Object) null)
          texture2D2 = this.m_TriClassBannerStaticSilouetteTextureUnique;
        if (flag4)
          texture2D2 = !flag1 ? this.m_DeathKnightRuneBannerSilhouetteTexture : this.m_DeathKnightRuneBannerSilhouetteTextureUnique;
        if (flag5)
          texture2D2 = this.m_BattlegroundQuestSiloutteTexture;
        if ((bool) (UnityEngine.Object) this.m_StaticSilouetteOverride)
          texture2D2 = this.m_StaticSilouetteOverride;
      }
      Material sharedMaterial = this.m_renderer.GetSharedMaterial();
      sharedMaterial.mainTexture = (Texture) texture2D2;
      sharedMaterial.renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue;
      this.m_forceRerender = false;
      return true;
    }
    if ((UnityEngine.Object) this.m_highlightRender == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unable to find HighlightRender component on m_RenderPlane");
      return false;
    }
    if (this.m_highlightRender.enabled)
    {
      this.m_highlightRender.CreateSilhouetteTexture(this.m_forceRerender);
      Material sharedMaterial = this.m_renderer.GetSharedMaterial();
      sharedMaterial.mainTexture = (Texture) this.m_highlightRender.SilhouetteTexture;
      sharedMaterial.renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue;
    }
    this.m_forceRerender = false;
    return true;
  }

  private async UniTaskVoid ContinuousSilouetteRender(
    float renderTime,
    CancellationToken token)
  {
    if ((UnityEngine.Object) this.m_RenderPlane == (UnityEngine.Object) null || this.m_graphicsManager == null || (UnityEngine.Object) this.m_renderer == (UnityEngine.Object) null)
      return;
    if (this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.Low)
    {
      await UniTask.Delay(TimeSpan.FromSeconds((double) renderTime), cancellationToken: token);
      if (!this.m_renderer.enabled)
        return;
      this.m_isDirty = true;
      this.m_forceRerender = true;
      this.RenderSilouette();
    }
    else
    {
      float endTime = Time.realtimeSinceStartup + renderTime;
      while ((double) Time.realtimeSinceStartup < (double) endTime)
      {
        if (this.m_renderer.enabled)
        {
          this.m_isDirty = true;
          this.m_forceRerender = true;
          this.RenderSilouette();
        }
        await UniTask.Yield(PlayerLoopTiming.Update, token);
      }
    }
  }

  private void SendDataToPlaymaker()
  {
    if ((UnityEngine.Object) this.m_FSM == (UnityEngine.Object) null)
      return;
    FsmMaterial fsmMaterial = this.m_FSM.FsmVariables.GetFsmMaterial("HighlightMaterial");
    if (fsmMaterial != null)
      fsmMaterial.Value = this.m_renderer.GetSharedMaterial();
    FsmString fsmString1 = this.m_FSM.FsmVariables.GetFsmString("CurrentState");
    if (fsmString1 != null)
      fsmString1.Value = this.m_CurrentState.ToString();
    FsmString fsmString2 = this.m_FSM.FsmVariables.GetFsmString("PreviousState");
    if (fsmString2 == null)
      return;
    fsmString2.Value = this.m_PreviousState.ToString();
  }

  private void SendPlaymakerDeathEvent()
  {
    if ((UnityEngine.Object) this.m_FSM == (UnityEngine.Object) null)
      return;
    FsmString fsmString = this.m_FSM.FsmVariables.GetFsmString("DeathTransition");
    if (fsmString != null)
      fsmString.Value = this.m_DeathTransition;
    this.m_FSM.SendEvent("Death");
  }

  private void SendPlaymakerBirthEvent()
  {
    if ((UnityEngine.Object) this.m_FSM == (UnityEngine.Object) null)
      return;
    FsmString fsmString1 = this.m_FSM.FsmVariables.GetFsmString("BirthTransition");
    if (fsmString1 != null)
      fsmString1.Value = this.m_BirthTransition;
    FsmString fsmString2 = this.m_FSM.FsmVariables.GetFsmString("SecondBirthTransition");
    if (fsmString2 != null)
      fsmString2.Value = this.m_SecondBirthTransition;
    FsmString fsmString3 = this.m_FSM.FsmVariables.GetFsmString("IdleTransition");
    if (fsmString3 != null)
      fsmString3.Value = this.m_IdleTransition;
    this.m_FSM.SendEvent("Birth");
  }

  public void OnActionFinished()
  {
  }

  private void TryInitRenderers()
  {
    if (this.m_RenderersInitialized)
      return;
    this.m_RenderersInitialized = true;
    if ((UnityEngine.Object) this.m_RenderPlane == (UnityEngine.Object) null)
    {
      if (!Application.isEditor)
        Debug.LogError((object) "m_RenderPlane is null!");
      this.enabled = false;
    }
    else
    {
      this.m_renderer = this.m_RenderPlane.GetComponent<Renderer>();
      this.m_highlightRender = this.m_RenderPlane.GetComponent<HighlightRender>();
      this.m_renderer.enabled = false;
      this.m_VisibilityState = false;
      this.m_FSM = this.m_RenderPlane.GetComponent<PlayMakerFSM>();
    }
  }
}
