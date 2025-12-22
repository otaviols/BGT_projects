using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

[RequireComponent(typeof (MeshFilter), typeof (MeshRenderer))]
public class LegendaryHeroMagicEffect : MonoBehaviour
{
  public LegendaryHeroMagicEffectMaterial EffectConfig;
  private LegendarySkin m_skin;
  private MeshFilter m_meshFilter;
  private Material m_material;
  private LegendaryHeroMagicEffectState m_state;

  private void Start()
  {
    if ((Object) this.EffectConfig == (Object) null)
    {
      this.enabled = false;
    }
    else
    {
      this.m_skin = this.GetComponentInParent<LegendarySkin>();
      this.m_meshFilter = this.GetComponent<MeshFilter>();
      if ((Object) this.m_meshFilter != (Object) null)
        this.m_meshFilter.sharedMesh = this.EffectConfig.Mesh;
      Shader shader = this.EffectConfig.Shader;
      if ((Object) shader == (Object) null)
        shader = Shader.Find("Unlit/Color");
      this.m_material = new Material(shader);
      this.EffectConfig.InitialiseMaterial(this.m_material);
      this.EffectConfig.UpdateMaterialState(this.m_material, in this.m_state);
      MeshRenderer component = this.GetComponent<MeshRenderer>();
      if ((Object) component != (Object) null)
        component.SetSharedMaterial(this.m_material);
      if (!((Object) this.m_skin != (Object) null))
        return;
      this.m_skin.SetDirty();
    }
  }

  private void OnEnable() => this.m_state = new LegendaryHeroMagicEffectState();

  private void Update()
  {
    if ((Object) this.EffectConfig == (Object) null)
      return;
    this.m_state = this.EffectConfig.UpdateState(Time.deltaTime, in this.m_state);
    if (!((Object) this.m_material != (Object) null))
      return;
    this.EffectConfig.UpdateMaterialState(this.m_material, in this.m_state);
  }

  private void OnDrawGizmos()
  {
    if (!((Object) this.EffectConfig != (Object) null))
      return;
    Gizmos.DrawMesh(this.EffectConfig.Mesh, this.transform.position, this.transform.rotation, this.transform.lossyScale);
  }
}
