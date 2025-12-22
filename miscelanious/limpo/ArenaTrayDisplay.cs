using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class ArenaTrayDisplay : MonoBehaviour
{
  public int m_Rank;
  public PlayMakerFSM m_RewardPlaymaker;
  [CustomEditField(Sections = "Keys")]
  public GameObject m_TheKeyMesh;
  public GameObject m_TheKeyGlowPlane;
  public GameObject m_TheKeyGlowHoleMesh;
  public GameObject m_TheKeySelectionGlow;
  public GameObject m_TheKeyOldSelectionGlow;
  public float m_TheKeyTransitionDelay = 0.5f;
  public float m_TheKeyTransitionFadeInTime = 1.5f;
  public float m_TheKeyTransitionFadeOutTime = 2f;
  public ParticleSystem m_TheKeyTransitionParticles;
  public string m_TheKeyTransitionSound = "arena_key_transition.prefab:7b4c3a5222405834abd921cbf53bf689";
  [CustomEditField(Sections = "Reward Panel")]
  public UberText m_WinCountUberText;
  public GameObject m_RewardDoorPlates;
  public GameObject m_BehindTheDoors;
  public GameObject m_RewardPaperBone;
  public GameObject m_PaperMain;
  public GameObject m_RewardBoxesBone;
  public GameObject m_InstructionText;
  public GameObject m_InstructionDetailText;
  public List<ArenaTrayDisplay.ArenaKeyVisualData> m_ArenaKeyVisualData;
  private RewardBoxesDisplay m_RewardBoxes;
  private GameObject m_TheKeyIdleEffects;
  private bool m_isTheKeyIdleEffectsLoading;
  private ArenaRewardPaper m_RewardPaper;
  private GameObject m_Paper;
  private AssetHandle<Texture> m_paperTexture;
  private bool m_isReady;
  private static ArenaTrayDisplay s_Instance;

  private void Awake()
  {
    ArenaTrayDisplay.s_Instance = this;
    AssetReference draftPaperTexture = DraftManager.Get().GetDraftPaperTexture();
    AssetLoader.Get().LoadAsset<Texture>(draftPaperTexture, (AssetHandleCallback<Texture>) ((assetRef, loadedTexture, callbackData) =>
    {
      if (loadedTexture == null)
      {
        Debug.LogWarningFormat("ArenaTrayDisplay: Failed to load {0}.", (object) assetRef.ToString());
      }
      else
      {
        AssetHandle.Take<Texture>(ref this.m_paperTexture, loadedTexture);
        RendererExtension.GetMaterial(this.m_PaperMain.GetComponent<Renderer>()).mainTexture = (Texture) this.m_paperTexture;
      }
    }));
    AssetReference rewardPaperPrefab = DraftManager.Get().GetRewardPaperPrefab();
    AssetLoader.Get().InstantiatePrefab(rewardPaperPrefab, (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("ArenaTrayDisplay: Failed to load {0}.", (object) assetRef.ToString()));
      }
      else
      {
        ArenaTrayDisplay.s_Instance.m_RewardPaper = go.GetComponent<ArenaRewardPaper>();
        go.transform.parent = this.gameObject.transform;
        if ((UnityEngine.Object) ArenaTrayDisplay.s_Instance.m_RewardPaperBone != (UnityEngine.Object) null)
        {
          go.transform.position = ArenaTrayDisplay.s_Instance.m_RewardPaperBone.transform.position;
          go.transform.localScale = ArenaTrayDisplay.s_Instance.m_RewardPaperBone.transform.localScale;
        }
        else
          Debug.LogWarning((object) "ArenaTrayDisplay: m_RewardPaperBone is not set, so ArenaRewardPaper may look wrong.");
        ArenaTrayDisplay.s_Instance.m_Paper = go;
        this.ShowPlainPaperBackground();
        if ((UnityEngine.Object) ArenaTrayDisplay.s_Instance.m_RewardPaper == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) string.Format("ArenaTrayDisplay: m_RewardPaper is null! Check the prefab you're loading, {0}.", (object) assetRef.ToString()));
          ArenaTrayDisplay.s_Instance.m_isReady = true;
        }
        else if ((UnityEngine.Object) ArenaTrayDisplay.s_Instance.m_RewardPaper.m_WinsUberText == (UnityEngine.Object) null || (UnityEngine.Object) ArenaTrayDisplay.s_Instance.m_RewardPaper.m_LossesUberText == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) string.Format("ArenaTrayDisplay: m_WinsUberText or m_LossesUberText is null! Check the prefab you're loading, {0}", (object) assetRef.ToString()));
          ArenaTrayDisplay.s_Instance.m_isReady = true;
        }
        else
        {
          ArenaTrayDisplay.s_Instance.m_RewardPaper.m_WinsUberText.Text = GameStrings.Get("GLUE_DRAFT_WINS_LABEL");
          ArenaTrayDisplay.s_Instance.m_RewardPaper.m_LossesUberText.Text = GameStrings.Get("GLUE_DRAFT_LOSSES_LABEL");
          if ((UnityEngine.Object) ArenaTrayDisplay.s_Instance.m_BehindTheDoors == (UnityEngine.Object) null)
          {
            Debug.LogWarning((object) "ArenaTrayDisplay: m_BehindTheDoors is null!");
            ArenaTrayDisplay.s_Instance.m_isReady = true;
          }
          else
          {
            ArenaTrayDisplay.s_Instance.m_BehindTheDoors.SetActive(false);
            if ((UnityEngine.Object) ArenaTrayDisplay.s_Instance.m_RewardDoorPlates == (UnityEngine.Object) null)
            {
              Debug.LogWarning((object) "ArenaTrayDisplay: m_RewardDoorPlates is null!");
              ArenaTrayDisplay.s_Instance.m_isReady = true;
            }
            else
            {
              ArenaTrayDisplay.s_Instance.m_RewardDoorPlates.SetActive(false);
              RenderUtils.EnableColliders(ArenaTrayDisplay.s_Instance.m_TheKeyMesh, false);
              ArenaTrayDisplay.s_Instance.m_isReady = true;
            }
          }
        }
      }
    }));
  }

  private void OnDestroy() => AssetHandle.SafeDispose<Texture>(ref this.m_paperTexture);

  public static ArenaTrayDisplay Get() => ArenaTrayDisplay.s_Instance;

  public bool IsReady() => this.m_isReady;

  public void UpdateTray() => this.UpdateTray(true);

  public void UpdateTray(bool showNewKey)
  {
    this.ShowPlainPaper();
    if ((UnityEngine.Object) this.m_InstructionText != (UnityEngine.Object) null)
      this.m_InstructionText.SetActive(false);
    if ((UnityEngine.Object) this.m_RewardDoorPlates != (UnityEngine.Object) null && !this.m_RewardDoorPlates.activeSelf)
      this.m_RewardDoorPlates.SetActive(true);
    bool flag = false;
    DraftManager draftManager = DraftManager.Get();
    if (draftManager == null)
    {
      Debug.LogError((object) "ArenaTrayDisplay: DraftManager.Get() == null!");
    }
    else
    {
      int wins = draftManager.GetWins();
      int losses = draftManager.GetLosses();
      if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && GameMgr.Get().WasArena() && draftManager.GetIsNewKey())
        flag = true;
      this.m_WinCountUberText.Text = wins.ToString();
      this.m_RewardPaper.m_Xmark1.GetComponent<Renderer>().enabled = losses > 0;
      this.m_RewardPaper.m_Xmark2.GetComponent<Renderer>().enabled = losses > 1;
      this.m_RewardPaper.m_Xmark3.GetComponent<Renderer>().enabled = losses > 2;
      this.UpdateXBoxes();
      if (((!flag ? 0 : (wins > 0 ? 1 : 0)) & (showNewKey ? 1 : 0)) != 0)
      {
        this.UpdateKeyArt(wins - 1);
        this.StartCoroutine(this.AnimateKeyTransition(wins));
      }
      else
        this.UpdateKeyArt(wins);
    }
  }

  public void ShowPlainPaperBackground()
  {
    this.ShowPlainPaper();
    if ((UnityEngine.Object) this.m_InstructionText != (UnityEngine.Object) null)
      this.m_InstructionText.SetActive(true);
    if (!((UnityEngine.Object) this.m_RewardDoorPlates != (UnityEngine.Object) null) || !this.m_RewardDoorPlates.activeSelf)
      return;
    this.m_RewardDoorPlates.SetActive(false);
  }

  public void ActivateKey()
  {
    RenderUtils.EnableColliders(this.m_TheKeyMesh, true);
    Renderer component1 = this.m_TheKeySelectionGlow.GetComponent<Renderer>();
    component1.enabled = true;
    Material sharedMaterial = RendererExtension.GetSharedMaterial(component1);
    sharedMaterial.color = sharedMaterial.color with
    {
      a = 0.0f
    };
    sharedMaterial.SetFloat("_FxIntensity", 1f);
    iTween.FadeTo(this.m_TheKeySelectionGlow, iTween.Hash((object) "alpha", (object) 0.8f, (object) "time", (object) 2f, (object) "easetype", (object) iTween.EaseType.easeInOutBack));
    Material KeyGlowMat = RendererExtension.GetMaterial(component1);
    KeyGlowMat.SetFloat("_FxIntensity", 0.0f);
    iTween.ValueTo(this.m_TheKeySelectionGlow, iTween.Hash((object) "time", (object) 2f, (object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "easetype", (object) iTween.EaseType.easeInOutBack, (object) "onupdate", (object) (Action<object>) (amount => KeyGlowMat.SetFloat("_FxIntensity", (float) amount)), (object) "onupdatetarget", (object) this.m_TheKeySelectionGlow));
    PegUIElement component2 = this.m_TheKeyMesh.GetComponent<PegUIElement>();
    if ((UnityEngine.Object) component2 == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "ArenaTrayDisplay: PegUIElement missing on the Key!");
    }
    else
    {
      component2.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OpenRewardBox));
      Navigation.PushBlockBackingOut();
    }
  }

  public void ShowRewardsOpenAtStart()
  {
    if ((UnityEngine.Object) this.m_RewardPlaymaker == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "ArenaTrayDisplay: Missing Playmaker FSM!");
    }
    else
    {
      this.HidePaper();
      if ((UnityEngine.Object) this.m_InstructionText != (UnityEngine.Object) null)
        this.m_InstructionText.SetActive(false);
      if ((UnityEngine.Object) this.m_InstructionDetailText != (UnityEngine.Object) null)
        this.m_InstructionDetailText.SetActive(false);
      if ((UnityEngine.Object) this.m_WinCountUberText != (UnityEngine.Object) null)
        this.m_WinCountUberText.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_RewardPaper.m_WinsUberText != (UnityEngine.Object) null)
        this.m_RewardPaper.m_WinsUberText.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_RewardPaper.m_LossesUberText != (UnityEngine.Object) null)
        this.m_RewardPaper.m_LossesUberText.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_RewardPaper.m_XmarksRoot != (UnityEngine.Object) null)
        this.m_RewardPaper.m_XmarksRoot.SetActive(false);
      if ((UnityEngine.Object) this.m_TheKeySelectionGlow != (UnityEngine.Object) null)
        this.m_TheKeySelectionGlow.SetActive(false);
      this.m_RewardPaper.m_WinsUberText.gameObject.SetActive(false);
      this.m_RewardPaper.m_LossesUberText.gameObject.SetActive(false);
      this.m_TheKeyMesh.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_BehindTheDoors == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "ArenaTrayDisplay: m_BehindTheDoors is null!");
      }
      else
      {
        this.m_BehindTheDoors.SetActive(true);
        if (DraftManager.Get() == null)
        {
          Debug.LogError((object) "ArenaTrayDisplay: DraftManager.Get() == null!");
        }
        else
        {
          List<RewardData> rewards = DraftManager.Get().GetRewards();
          PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
          {
            this.m_RewardBoxes = go.GetComponent<RewardBoxesDisplay>();
            this.m_RewardBoxes.SetRewards(rewards);
            this.m_RewardBoxes.RegisterDoneCallback(new Action(this.OnRewardBoxesDone));
            TransformUtil.AttachAndPreserveLocalTransform(this.m_RewardBoxes.transform, this.m_RewardBoxesBone.transform);
            this.m_RewardBoxes.DebugLogRewards();
            this.m_RewardBoxes.ShowAlreadyOpenedRewards();
          });
          AssetLoader.Get().InstantiatePrefab((AssetReference) RewardBoxesDisplay.GetPrefab(rewards), callback);
          this.m_RewardPlaymaker.gameObject.SetActive(true);
          this.m_RewardPlaymaker.SendEvent("Death");
          if (!((UnityEngine.Object) this.m_TheKeyMesh.GetComponent<PegUIElement>() == (UnityEngine.Object) null))
            return;
          Debug.LogWarning((object) "ArenaTrayDisplay: PegUIElement missing on the Key!");
        }
      }
    }
  }

  public void AnimateRewards()
  {
    List<RewardData> rewards = DraftManager.Get().GetRewards();
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      this.m_RewardBoxes = go.GetComponent<RewardBoxesDisplay>();
      this.m_RewardBoxes.SetRewards(rewards);
      this.m_RewardBoxes.RegisterDoneCallback(new Action(this.OnRewardBoxesDone));
      TransformUtil.AttachAndPreserveLocalTransform(this.m_RewardBoxes.transform, this.m_RewardBoxesBone.transform);
      this.m_RewardBoxes.AnimateRewards();
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) RewardBoxesDisplay.GetPrefab(rewards), callback);
  }

  public void KeyFXCancel()
  {
    if (!(bool) (UnityEngine.Object) this.m_TheKeyIdleEffects)
      return;
    PlayMakerFSM componentInChildren = this.m_TheKeyIdleEffects.GetComponentInChildren<PlayMakerFSM>();
    if (!(bool) (UnityEngine.Object) componentInChildren)
      return;
    componentInChildren.SendEvent("Cancel");
  }

  private void UpdateKeyArt(int rank)
  {
    if ((UnityEngine.Object) this.m_TheKeyMesh == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "ArenaTrayDisplay: key mesh missing!");
    }
    else
    {
      this.ShowRewardPaper();
      ArenaTrayDisplay.ArenaKeyVisualData arenaKeyVisualData = this.m_ArenaKeyVisualData[rank];
      if ((UnityEngine.Object) arenaKeyVisualData.m_Mesh != (UnityEngine.Object) null)
      {
        MeshFilter component = this.m_TheKeyMesh.GetComponent<MeshFilter>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.mesh = UnityEngine.Object.Instantiate<Mesh>(arenaKeyVisualData.m_Mesh);
      }
      if ((UnityEngine.Object) arenaKeyVisualData.m_Material != (UnityEngine.Object) null)
        RendererExtension.SetSharedMaterial(this.m_TheKeyMesh.GetComponent<Renderer>(), arenaKeyVisualData.m_Material);
      if (arenaKeyVisualData.m_IdleEffectsPrefabPath != string.Empty)
      {
        this.m_isTheKeyIdleEffectsLoading = true;
        AssetLoader.Get().InstantiatePrefab((AssetReference) arenaKeyVisualData.m_IdleEffectsPrefabPath, new PrefabCallback<GameObject>(this.OnIdleEffectsLoaded));
      }
      if ((UnityEngine.Object) arenaKeyVisualData.m_ParticlePrefab != (UnityEngine.Object) null)
      {
        GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(arenaKeyVisualData.m_ParticlePrefab);
        Transform transform1 = gameObject1.transform.Find("FX_Motes");
        if ((UnityEngine.Object) transform1 != (UnityEngine.Object) null)
        {
          GameObject gameObject2 = transform1.gameObject;
          gameObject2.transform.parent = this.m_TheKeyMesh.transform;
          gameObject2.transform.localPosition = Vector3.zero;
          gameObject2.transform.localRotation = Quaternion.identity;
          this.m_RewardPlaymaker.FsmVariables.GetFsmGameObject("FX_Motes").Value = gameObject2;
        }
        Transform transform2 = gameObject1.transform.Find("FX_Motes_glow");
        if ((UnityEngine.Object) transform2 != (UnityEngine.Object) null)
        {
          GameObject gameObject3 = transform2.gameObject;
          gameObject3.transform.parent = this.m_TheKeyMesh.transform;
          gameObject3.transform.localPosition = Vector3.zero;
          gameObject3.transform.localRotation = Quaternion.identity;
          this.m_RewardPlaymaker.FsmVariables.GetFsmGameObject("FX_Motes_glow").Value = gameObject3;
        }
        Transform transform3 = gameObject1.transform.Find("FX_Motes_trail");
        if ((UnityEngine.Object) transform3 != (UnityEngine.Object) null)
        {
          GameObject gameObject4 = transform3.gameObject;
          gameObject4.transform.parent = this.m_TheKeyMesh.transform;
          gameObject4.transform.localPosition = Vector3.zero;
          gameObject4.transform.localRotation = Quaternion.identity;
          this.m_RewardPlaymaker.FsmVariables.GetFsmGameObject("FX_Motes_trail").Value = gameObject4;
        }
      }
      if ((UnityEngine.Object) this.m_TheKeyGlowPlane != (UnityEngine.Object) null && (UnityEngine.Object) arenaKeyVisualData.m_EffectGlowTexture != (UnityEngine.Object) null)
        RendererExtension.GetMaterial(this.m_TheKeyGlowPlane.GetComponent<Renderer>()).mainTexture = arenaKeyVisualData.m_EffectGlowTexture;
      if ((UnityEngine.Object) arenaKeyVisualData.m_KeyHoleGlowMesh != (UnityEngine.Object) null)
      {
        MeshFilter component = this.m_TheKeyGlowHoleMesh.GetComponent<MeshFilter>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.mesh = UnityEngine.Object.Instantiate<Mesh>(arenaKeyVisualData.m_KeyHoleGlowMesh);
      }
      if ((UnityEngine.Object) this.m_TheKeySelectionGlow != (UnityEngine.Object) null && (UnityEngine.Object) arenaKeyVisualData.m_SelectionGlowTexture != (UnityEngine.Object) null)
        RendererExtension.GetMaterial(this.m_TheKeySelectionGlow.GetComponent<Renderer>()).mainTexture = arenaKeyVisualData.m_SelectionGlowTexture;
      LayerUtils.SetLayer(this.m_TheKeyMesh.transform.parent.gameObject, GameLayer.Default);
    }
  }

  private void OnIdleEffectsLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_isTheKeyIdleEffectsLoading = false;
    if ((bool) (UnityEngine.Object) this.m_TheKeyIdleEffects)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_TheKeyIdleEffects);
    this.m_TheKeyIdleEffects = go;
    go.SetActive(true);
    go.transform.parent = this.m_TheKeyMesh.transform;
    go.transform.localPosition = Vector3.zero;
  }

  private IEnumerator AnimateKeyTransition(int rank)
  {
    yield return (object) new WaitForSeconds(this.m_TheKeyTransitionDelay);
    while (this.m_isTheKeyIdleEffectsLoading)
      yield return (object) null;
    ArenaTrayDisplay.ArenaKeyVisualData arenaKeyVisualData = this.m_ArenaKeyVisualData[rank - 1];
    ArenaTrayDisplay.ArenaKeyVisualData keyData = this.m_ArenaKeyVisualData[rank];
    Renderer oldKeySelectionGlowRenderer = this.m_TheKeyOldSelectionGlow.GetComponent<Renderer>();
    if ((UnityEngine.Object) this.m_TheKeyOldSelectionGlow != (UnityEngine.Object) null && (UnityEngine.Object) arenaKeyVisualData.m_EffectGlowTexture != (UnityEngine.Object) null)
      RendererExtension.GetMaterial(oldKeySelectionGlowRenderer).mainTexture = arenaKeyVisualData.m_SelectionGlowTexture;
    oldKeySelectionGlowRenderer.enabled = true;
    Material prevKeyGlowMat = RendererExtension.GetMaterial(oldKeySelectionGlowRenderer);
    prevKeyGlowMat.SetFloat("_FxIntensity", 0.0f);
    iTween.ValueTo(this.m_TheKeyOldSelectionGlow, iTween.Hash((object) "time", (object) this.m_TheKeyTransitionFadeInTime, (object) "from", (object) 0.0f, (object) "to", (object) 1.5f, (object) "easetype", (object) iTween.EaseType.easeInCubic, (object) "onupdate", (object) (Action<object>) (amount => prevKeyGlowMat.SetFloat("_FxIntensity", (float) amount)), (object) "onupdatetarget", (object) this.m_TheKeyOldSelectionGlow));
    if (this.m_TheKeyTransitionSound != string.Empty)
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_TheKeyTransitionSound);
    yield return (object) new WaitForSeconds(this.m_TheKeyTransitionFadeInTime);
    this.m_TheKeyTransitionParticles.Play();
    this.UpdateKeyArt(rank);
    oldKeySelectionGlowRenderer.enabled = false;
    Renderer keySelectionGlowRenderer = this.m_TheKeySelectionGlow.GetComponent<Renderer>();
    if ((UnityEngine.Object) this.m_TheKeySelectionGlow != (UnityEngine.Object) null && (UnityEngine.Object) keyData.m_EffectGlowTexture != (UnityEngine.Object) null)
      RendererExtension.GetMaterial(keySelectionGlowRenderer).mainTexture = keyData.m_SelectionGlowTexture;
    keySelectionGlowRenderer.enabled = true;
    prevKeyGlowMat.SetFloat("_FxIntensity", 0.0f);
    Material KeyGlowMat = RendererExtension.GetMaterial(keySelectionGlowRenderer);
    KeyGlowMat.SetFloat("_FxIntensity", 1.5f);
    iTween.ValueTo(this.m_TheKeySelectionGlow, iTween.Hash((object) "time", (object) this.m_TheKeyTransitionFadeOutTime, (object) "from", (object) 1.5f, (object) "to", (object) 0.0f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "onupdate", (object) (Action<object>) (amount => KeyGlowMat.SetFloat("_FxIntensity", (float) amount)), (object) "onupdatetarget", (object) this.m_TheKeySelectionGlow));
    yield return (object) new WaitForSeconds(this.m_TheKeyTransitionFadeOutTime);
    keySelectionGlowRenderer.enabled = false;
  }

  private void UpdateXBoxes()
  {
    if (!DemoMgr.Get().ArenaIs1WinMode())
      return;
    this.m_RewardPaper.m_XmarkBox[0].SetActive(true);
    this.m_RewardPaper.m_XmarkBox[1].SetActive(false);
    this.m_RewardPaper.m_XmarkBox[2].SetActive(false);
  }

  private void OpenRewardBox(UIEvent e) => this.OpenRewardBox();

  private void OpenRewardBox()
  {
    if ((UnityEngine.Object) this.m_RewardPlaymaker == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "ArenaTrayDisplay: Missing Playmaker FSM!");
    }
    else
    {
      if ((UnityEngine.Object) this.m_RewardPaper.m_EventEndsText != (UnityEngine.Object) null)
        this.m_RewardPaper.m_EventEndsText.Hide();
      if ((UnityEngine.Object) this.m_RewardPaper.m_XmarksRoot != (UnityEngine.Object) null)
        this.m_RewardPaper.m_XmarksRoot.SetActive(false);
      if ((UnityEngine.Object) this.m_TheKeySelectionGlow != (UnityEngine.Object) null)
        this.m_TheKeySelectionGlow.SetActive(false);
      this.m_RewardPaper.m_WinsUberText.gameObject.SetActive(false);
      this.m_RewardPaper.m_LossesUberText.gameObject.SetActive(false);
      RenderUtils.EnableColliders(this.m_TheKeyMesh, false);
      LayerUtils.SetLayer(this.m_TheKeyMesh.transform.parent.gameObject, GameLayer.Default);
      if ((bool) (UnityEngine.Object) this.m_TheKeyIdleEffects)
      {
        PlayMakerFSM componentInChildren = this.m_TheKeyIdleEffects.GetComponentInChildren<PlayMakerFSM>();
        if ((bool) (UnityEngine.Object) componentInChildren)
          componentInChildren.SendEvent("Death");
      }
      if ((UnityEngine.Object) this.m_BehindTheDoors == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "ArenaTrayDisplay: m_BehindTheDoors is null!");
      }
      else
      {
        this.m_BehindTheDoors.SetActive(true);
        this.m_RewardPlaymaker.SendEvent("Birth");
        this.StartCoroutine(this.m_RewardPaper.PlayRewardBurnAway(this.m_RewardPlaymaker));
        this.m_RewardPaper.PlayEmberWipeFX();
      }
    }
  }

  private void OnRewardBoxesDone()
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
      return;
    DraftManager draftManager = DraftManager.Get();
    if (draftManager.GetDraftDeck() == null)
      Log.All.Print("bug 8052, null exception");
    else
      Network.Get().AckDraftRewards(draftManager.GetDraftDeck().ID, draftManager.GetSlot());
    DraftDisplay.Get().OnOpenRewardsComplete();
  }

  private void ShowPlainPaper()
  {
    this.m_Paper.SetActive(false);
    if ((UnityEngine.Object) this.m_PaperMain != (UnityEngine.Object) null)
      this.m_PaperMain.SetActive(true);
    this.m_RewardPaper.m_XmarksRoot.SetActive(false);
    this.m_RewardPaper.m_WinsUberText.Hide();
    this.m_RewardPaper.m_LossesUberText.Hide();
  }

  private void ShowRewardPaper()
  {
    this.m_Paper.SetActive(true);
    if ((UnityEngine.Object) this.m_PaperMain != (UnityEngine.Object) null)
      this.m_PaperMain.SetActive(false);
    this.m_RewardPaper.m_XmarksRoot.SetActive(true);
    this.m_RewardPaper.m_WinsUberText.Show();
    this.m_RewardPaper.m_LossesUberText.Show();
    if (!((UnityEngine.Object) this.m_RewardPaper.m_EventEndsText != (UnityEngine.Object) null))
      return;
    if (DraftManager.Get().CurrentSeasonId == 0)
    {
      this.m_RewardPaper.m_EventEndsText.Text = string.Empty;
    }
    else
    {
      TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
      {
        m_seconds = "GLUE_ARENA_LABEL_SEASON_ENDING_SECONDS",
        m_minutes = "GLUE_ARENA_LABEL_SEASON_ENDING_MINUTES",
        m_hours = "GLUE_ARENA_LABEL_SEASON_ENDING_HOURS",
        m_yesterday = (string) null,
        m_days = "GLUE_ARENA_LABEL_SEASON_ENDING_DAYS",
        m_weeks = "GLUE_ARENA_LABEL_SEASON_ENDING_WEEKS",
        m_monthAgo = "GLUE_ARENA_LABEL_SEASON_ENDING_OVER_1_MONTH"
      };
      this.m_RewardPaper.m_EventEndsText.Text = TimeUtils.GetElapsedTimeString((long) DraftManager.Get().SecondsUntilEndOfSeason, stringSet, true);
    }
  }

  private void HidePaper() => this.m_Paper.SetActive(false);

  [Serializable]
  public class ArenaKeyVisualData
  {
    public Mesh m_Mesh;
    public Material m_Material;
    public Mesh m_KeyHoleGlowMesh;
    public Texture m_EffectGlowTexture;
    public Texture m_SelectionGlowTexture;
    public GameObject m_ParticlePrefab;
    public string m_IdleEffectsPrefabPath;
  }
}
