using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class SummonOutForge : SpellImpl
{
  public GameObject m_scryLines;
  public Material m_scryLinesMaterial;
  public GameObject m_burstMotes;
  private static Color COMMON_COLOR = new Color(0.7333333f, 0.8235294f, 1f);
  private static Color RARE_COLOR = new Color(0.2f, 0.4745098f, 1f);
  private static Color EPIC_COLOR = new Color(0.5450981f, 0.2313726f, 1f);
  private static Color LEGENDARY_COLOR = new Color(1f, 0.6666667f, 0.2f);

  protected override void OnBirth(SpellStateType prevStateType) => this.StartCoroutine(this.BirthState());

  private IEnumerator BirthState()
  {
    SummonOutForge summonOutForge = this;
    summonOutForge.InitActorVariables();
    summonOutForge.SetActorVisibility(true, false);
    summonOutForge.SetVisibility(summonOutForge.m_scryLines, true);
    TAG_RARITY rarity = summonOutForge.m_actor.GetRarity();
    Material material = summonOutForge.m_scryLines.GetComponent<Renderer>().GetMaterial();
    switch (rarity)
    {
      case TAG_RARITY.RARE:
        summonOutForge.m_scryLinesMaterial.SetColor("_TintColor", SummonOutForge.RARE_COLOR);
        material.SetColor("_TintColor", SummonOutForge.RARE_COLOR);
        break;
      case TAG_RARITY.EPIC:
        summonOutForge.m_scryLinesMaterial.SetColor("_TintColor", SummonOutForge.EPIC_COLOR);
        material.SetColor("_TintColor", SummonOutForge.EPIC_COLOR);
        break;
      case TAG_RARITY.LEGENDARY:
        summonOutForge.m_scryLinesMaterial.SetColor("_TintColor", SummonOutForge.LEGENDARY_COLOR);
        material.SetColor("_TintColor", SummonOutForge.LEGENDARY_COLOR);
        break;
      default:
        summonOutForge.m_scryLinesMaterial.SetColor("_TintColor", SummonOutForge.COMMON_COLOR);
        material.SetColor("_TintColor", SummonOutForge.COMMON_COLOR);
        break;
    }
    summonOutForge.PlayAnimation(summonOutForge.m_scryLines, "AllyInHandScryLines_ForgeOut", PlayMode.StopAll);
    summonOutForge.PlayParticles(summonOutForge.m_burstMotes, false);
    yield return (object) new WaitForSeconds(0.16f);
    summonOutForge.m_rootObject.SetActive(false);
    summonOutForge.OnSpellFinished();
  }
}
