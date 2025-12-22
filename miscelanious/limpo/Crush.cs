using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class Crush : Spell
{
  public MinionPieces m_minionPieces;
  public Material m_premiumTauntMaterial;
  public Material m_premiumEliteMaterial;
  public UberText m_attack;
  public UberText m_health;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    Entity entity = this.GetSourceCard().GetEntity();
    Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) this);
    GameObject go = this.m_minionPieces.m_main;
    bool flag = entity.HasTag(GAME_TAG.PREMIUM);
    if (flag)
    {
      go = this.m_minionPieces.m_premium;
      RenderUtils.EnableRenderers(this.m_minionPieces.m_main, false);
    }
    GameObject portraitMesh = componentInParents.GetPortraitMesh();
    go.GetComponent<Renderer>().SetMaterial(portraitMesh.GetComponent<Renderer>().GetSharedMaterial());
    go.SetActive(true);
    RenderUtils.EnableRenderers(go, true);
    if (entity.HasTaunt())
    {
      if (flag)
        this.m_minionPieces.m_taunt.GetComponent<Renderer>().SetMaterial(this.m_premiumTauntMaterial);
      this.m_minionPieces.m_taunt.SetActive(true);
      RenderUtils.EnableRenderers(this.m_minionPieces.m_taunt, true);
    }
    if (entity.IsElite())
    {
      if (flag)
        this.m_minionPieces.m_legendary.GetComponent<Renderer>().SetMaterial(this.m_premiumEliteMaterial);
      this.m_minionPieces.m_legendary.SetActive(true);
      RenderUtils.EnableRenderers(this.m_minionPieces.m_legendary, true);
    }
    this.m_attack.SetText(GameStrings.Get(entity.GetATK().ToString()));
    this.m_health.SetText(GameStrings.Get(entity.GetHealth().ToString()));
  }
}
