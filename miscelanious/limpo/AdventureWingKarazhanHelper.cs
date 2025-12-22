using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AdventureWing))]
[CustomEditClass]
public class AdventureWingKarazhanHelper : MonoBehaviour
{
  public List<AdventureWingKarazhanHelper.WingSpecificObject> m_WingSpecificObjects = new List<AdventureWingKarazhanHelper.WingSpecificObject>();
  public List<MeshRenderer> m_backgroundRenderers = new List<MeshRenderer>();
  public List<Animator> m_adventureCompleteAnimators = new List<Animator>();
  public PlayMakerFSM m_doorOpenPlayMakerFSM;
  private AdventureWing m_adventureWing;
  private GameObject m_objectForThisWing;
  private float m_backgroundOffsetForThisWing;

  public void Initialize()
  {
    this.m_adventureWing = this.GetComponent<AdventureWing>();
    if ((UnityEngine.Object) this.m_adventureWing == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "AdventureWingKarazhanHelper could not find an AdventureWing component on the same GameObject!");
    }
    else
    {
      WingDbId wingId = this.m_adventureWing.GetWingId();
      for (int index = 0; index < this.m_WingSpecificObjects.Count; ++index)
      {
        AdventureWingKarazhanHelper.WingSpecificObject wingSpecificObject = this.m_WingSpecificObjects[index];
        wingSpecificObject.m_ObjectSpecificToWing.SetActive(false);
        if (wingSpecificObject.m_wingDbId == wingId)
        {
          this.m_objectForThisWing = wingSpecificObject.m_ObjectSpecificToWing;
          foreach (Renderer backgroundRenderer in this.m_backgroundRenderers)
          {
            Material material = backgroundRenderer.GetMaterial();
            material.SetTextureOffset("_MainTex", material.GetTextureOffset("_MainTex") with
            {
              y = wingSpecificObject.m_backgroundOffset
            });
          }
        }
      }
      if ((UnityEngine.Object) this.m_objectForThisWing == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "AdventureWingKarazhanHelper could not find an object for m_objectForThisWing!");
      }
      else
      {
        this.m_objectForThisWing.SetActive(true);
        PegUIElement componentInChildren = this.m_objectForThisWing.GetComponentInChildren<PegUIElement>();
        if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "AdventureWingKarazhanHelper could not find the unlock button!");
        }
        else
        {
          this.m_adventureWing.m_UnlockButton = componentInChildren;
          foreach (PlayMakerFSM componentsInChild in this.m_adventureWing.m_WingEventTable.GetComponentsInChildren<PlayMakerFSM>())
            componentsInChild.FsmVariables.GetFsmGameObject("KnockerRootVar").Value = componentInChildren.gameObject;
          this.m_doorOpenPlayMakerFSM.FsmVariables.GetFsmGameObject("KnockerHeadVar").Value = this.m_objectForThisWing;
          AdventureConfig adventureConfig = AdventureConfig.Get();
          AdventureDbId selectedAdventure = adventureConfig.GetSelectedAdventure();
          AdventureModeDbId selectedMode = adventureConfig.GetSelectedMode();
          if (!AdventureProgressMgr.Get().IsAdventureModeAndSectionComplete(selectedAdventure, selectedMode))
            return;
          foreach (Behaviour completeAnimator in this.m_adventureCompleteAnimators)
            completeAnimator.enabled = true;
        }
      }
    }
  }

  [Serializable]
  public class WingSpecificObject
  {
    public WingDbId m_wingDbId;
    public GameObject m_ObjectSpecificToWing;
    public float m_backgroundOffset;
  }
}
