using Hearthstone.UI;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibbonButtonsUI : MonoBehaviour
{
  public List<RibbonButtonsUI.RibbonButtonObject> m_Ribbons;
  public Transform m_LeftBones;
  public Transform m_RightBones;
  public float m_EaseInTime = 1f;
  public float m_EaseOutTime = 0.4f;
  public GameObject m_rootObject;
  public PegUIElement m_collectionManagerRibbon;
  public PegUIElement m_questLogRibbon;
  public PegUIElement m_packOpeningRibbon;
  public PegUIElement m_storeRibbon;
  public UberText m_packCount;
  public GameObject m_packCountFrame;
  public float m_minAspectRatioAdjustment = 0.24f;
  public float m_wideAspectRatioAdjustment;
  public float m_extraWideAspectRatioAdjustment = 0.24f;
  public float m_minAspectRatioZPos;
  public float m_wideAspectRatioZPos;
  public float m_extraWideAspectRatioZPos = 0.35f;
  public Widget m_journalButtonWidget;
  public GameObject m_legacyQuestButtonGameObject;
  private bool m_shown = true;

  public void Awake()
  {
    this.m_rootObject.SetActive(false);
    float ratioDependentValue = TransformUtil.GetAspectRatioDependentValue(this.m_minAspectRatioAdjustment, this.m_wideAspectRatioAdjustment, this.m_extraWideAspectRatioAdjustment);
    TransformUtil.SetLocalPosX((Component) this.m_LeftBones, this.m_LeftBones.localPosition.x + ratioDependentValue);
    TransformUtil.SetLocalPosX((Component) this.m_RightBones, this.m_RightBones.localPosition.x - ratioDependentValue);
    if (NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>() == null)
      Network.Get().RegisterNetHandler((object) InitialClientState.PacketID.ID, new Network.NetHandler(this.OnInitialClientState));
    else
      this.SetupJournalButton();
  }

  private void Start() => TransformUtil.SetLocalPosZ((Component) this.transform, TransformUtil.GetAspectRatioDependentValue(this.m_minAspectRatioZPos, this.m_wideAspectRatioZPos, this.m_extraWideAspectRatioZPos));

  private void OnDestroy() => Network.Get()?.RemoveNetHandler((object) InitialClientState.PacketID.ID, new Network.NetHandler(this.OnInitialClientState));

  public void Toggle(bool show)
  {
    this.m_shown = show;
    if (show)
      this.StartCoroutine(this.ShowRibbons());
    else
      this.StartCoroutine(this.HideRibbons());
  }

  private IEnumerator ShowRibbons()
  {
    this.m_rootObject.SetActive(false);
    float startDelay = 1f;
    foreach (RibbonButtonsUI.RibbonButtonObject ribbon in this.m_Ribbons)
    {
      if ((double) ribbon.m_AnimateInDelay < (double) startDelay)
        startDelay = ribbon.m_AnimateInDelay;
    }
    yield return (object) new WaitForSeconds(startDelay);
    this.m_rootObject.SetActive(true);
    foreach (RibbonButtonsUI.RibbonButtonObject ribbon in this.m_Ribbons)
    {
      ribbon.m_Ribbon.transform.position = ribbon.m_HiddenBone.position;
      iTween.Stop(ribbon.m_Ribbon.gameObject);
      Hashtable args = iTween.Hash((object) "position", (object) ribbon.m_ShownBone.position, (object) "delay", (object) (float) ((double) ribbon.m_AnimateInDelay - (double) startDelay), (object) "time", (object) this.m_EaseInTime, (object) "easeType", (object) iTween.EaseType.easeOutBack);
      iTween.MoveTo(ribbon.m_Ribbon.gameObject, args);
    }
  }

  private IEnumerator HideRibbons()
  {
    foreach (RibbonButtonsUI.RibbonButtonObject ribbon in this.m_Ribbons)
    {
      ribbon.m_Ribbon.transform.position = ribbon.m_ShownBone.position;
      iTween.Stop(ribbon.m_Ribbon.gameObject);
      Hashtable args = iTween.Hash((object) "position", (object) ribbon.m_HiddenBone.position, (object) "delay", (object) 0.0f, (object) "time", (object) this.m_EaseOutTime, (object) "easeType", (object) iTween.EaseType.easeInOutBack);
      iTween.MoveTo(ribbon.m_Ribbon.gameObject, args);
    }
    yield return (object) new WaitForSeconds(this.m_EaseOutTime);
    if (!this.m_shown)
      this.m_rootObject.SetActive(false);
  }

  public void SetPackCount(int packs)
  {
    if (packs <= 0)
    {
      this.m_packCount.Text = "";
      this.m_packCountFrame.SetActive(false);
    }
    else
    {
      this.m_packCount.Text = GameStrings.Format("GLUE_PACK_OPENING_BOOSTER_COUNT", (object) packs);
      this.m_packCountFrame.SetActive(true);
    }
  }

  private void OnInitialClientState()
  {
    Network.Get().RemoveNetHandler((object) InitialClientState.PacketID.ID, new Network.NetHandler(this.OnInitialClientState));
    this.SetupJournalButton();
  }

  private void SetupJournalButton()
  {
    this.m_journalButtonWidget.Show();
    this.m_legacyQuestButtonGameObject.SetActive(false);
  }

  [Serializable]
  public class RibbonButtonObject
  {
    public PegUIElement m_Ribbon;
    public Transform m_HiddenBone;
    public Transform m_ShownBone;
    public bool m_LeftSide;
    public float m_AnimateInDelay;
  }
}
