using Blizzard.T5.MaterialService.Extensions;
using System;
using UnityEngine;

public class QuestRewardWidget : MonoBehaviour
{
  public MeshRenderer m_portraitMesh;
  public int portraitIndex;
  public float m_tilingX = 0.69f;
  public float m_tilingY = 0.69f;
  public float m_offsetX = 0.17f;
  public float m_offsetY = 0.2f;
  private bool m_warningSent;
  private Actor m_actor;

  private void Awake()
  {
    this.m_actor = this.gameObject.GetComponent<Actor>();
    if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
      this.m_actor.OnPortraitMaterialUpdated += new Action(this.OnPortraitMaterialUpdated);
    else
      Debug.LogWarning((object) "QuestRewardWidget - Is missing an Actor Component");
  }

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null))
      return;
    this.m_actor.OnPortraitMaterialUpdated -= new Action(this.OnPortraitMaterialUpdated);
  }

  private void OnPortraitMaterialUpdated()
  {
    using (DefLoader.DisposableCardDef disposableCardDef = this.m_actor.GetCard()?.ShareDisposableCardDef())
      this.UpdatePortrait(disposableCardDef);
  }

  private void UpdatePortrait(DefLoader.DisposableCardDef disposableCardDef)
  {
    if (!this.m_actor.IsShown() || disposableCardDef == null)
      return;
    Material portraitMaterial = disposableCardDef.CardDef.GetBattlegroundsQuestRewardPortraitMaterial();
    Material material = this.m_portraitMesh.GetMaterials()[this.portraitIndex];
    if ((UnityEngine.Object) portraitMaterial == (UnityEngine.Object) null)
    {
      if (!this.m_warningSent)
      {
        Debug.LogWarning((object) "QuestRewardWidget.UpdatePortrait() - Missing quest reward Mat");
        this.m_warningSent = true;
      }
      this.SetupDefaultPortraitMaterial(material);
    }
    else
    {
      material.mainTexture = portraitMaterial.mainTexture;
      Texture texture = material.GetTexture("_SecondTex");
      material.CopyPropertiesFromMaterial(portraitMaterial);
      material.SetTexture("_SecondTex", texture);
    }
  }

  private void SetupDefaultPortraitMaterial(Material portraitMaterial)
  {
    portraitMaterial.SetTextureOffset("_MainTex", new Vector2(this.m_offsetX, this.m_offsetY));
    portraitMaterial.SetTextureScale("_MainTex", new Vector2(this.m_tilingX, this.m_tilingY));
  }
}
