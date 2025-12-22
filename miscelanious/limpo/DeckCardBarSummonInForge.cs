using System.Collections;
using UnityEngine;

public class DeckCardBarSummonInForge : SpellImpl
{
  public GameObject m_echoQuad;
  public Material m_echoQuadMaterial;
  public GameObject m_fxEvaporate;
  public Material m_fxEvaporateMaterial;
  private static Color COMMON_COLOR = new Color(1f, 1f, 1f);
  private static Color COMMON_TINT_COLOR = new Color(0.9215686f, 0.945098f, 1f);
  private static Color RARE_COLOR = new Color(0.1647059f, 0.4078431f, 1f);
  private static Color RARE_TINT_COLOR = new Color(0.1647059f, 0.4078431f, 1f);
  private static Color EPIC_COLOR = new Color(0.4156863f, 0.1647059f, 1f);
  private static Color EPIC_TINT_COLOR = new Color(0.4156863f, 0.1647059f, 0.9921569f);
  private static Color LEGENDARY_COLOR = new Color(0.7686275f, 0.5411765f, 0.1490196f);
  private static Color LEGENDARY_TINT_COLOR = new Color(0.6666667f, 0.4745098f, 0.1294118f);

  protected override void OnBirth(SpellStateType prevStateType) => this.StartCoroutine(this.BirthState());

  private IEnumerator BirthState()
  {
    DeckCardBarSummonInForge barSummonInForge = this;
    barSummonInForge.InitActorVariables();
    barSummonInForge.SetAnimationTime(barSummonInForge.m_echoQuad, "Secret_AbilityEchoOut_Forge", 0.0f);
    barSummonInForge.SetVisibility(barSummonInForge.m_echoQuad, true);
    Material material = barSummonInForge.GetMaterial(barSummonInForge.m_echoQuad, barSummonInForge.m_echoQuadMaterial);
    switch (barSummonInForge.m_actor.GetRarity())
    {
      case TAG_RARITY.RARE:
        barSummonInForge.SetMaterialColor(barSummonInForge.m_echoQuad, material, "_Color", DeckCardBarSummonInForge.RARE_COLOR);
        barSummonInForge.SetMaterialColor(barSummonInForge.m_fxEvaporate, barSummonInForge.m_fxEvaporateMaterial, "_TintColor", DeckCardBarSummonInForge.RARE_TINT_COLOR);
        break;
      case TAG_RARITY.EPIC:
        barSummonInForge.SetMaterialColor(barSummonInForge.m_echoQuad, material, "_Color", DeckCardBarSummonInForge.EPIC_COLOR);
        barSummonInForge.SetMaterialColor(barSummonInForge.m_fxEvaporate, barSummonInForge.m_fxEvaporateMaterial, "_TintColor", DeckCardBarSummonInForge.EPIC_TINT_COLOR);
        break;
      case TAG_RARITY.LEGENDARY:
        barSummonInForge.SetMaterialColor(barSummonInForge.m_echoQuad, material, "_Color", DeckCardBarSummonInForge.LEGENDARY_COLOR);
        barSummonInForge.SetMaterialColor(barSummonInForge.m_fxEvaporate, barSummonInForge.m_fxEvaporateMaterial, "_TintColor", DeckCardBarSummonInForge.LEGENDARY_TINT_COLOR);
        break;
      default:
        barSummonInForge.SetMaterialColor(barSummonInForge.m_echoQuad, material, "_Color", DeckCardBarSummonInForge.COMMON_COLOR);
        barSummonInForge.SetMaterialColor(barSummonInForge.m_fxEvaporate, barSummonInForge.m_fxEvaporateMaterial, "_TintColor", DeckCardBarSummonInForge.COMMON_TINT_COLOR);
        break;
    }
    barSummonInForge.SetActorVisibility(true, true);
    barSummonInForge.PlayParticles(barSummonInForge.m_fxEvaporate, false);
    barSummonInForge.SetAnimationSpeed(barSummonInForge.m_echoQuad, "Secret_AbilityEchoOut_Forge", 0.2f);
    barSummonInForge.PlayAnimation(barSummonInForge.m_echoQuad, "Secret_AbilityEchoOut_Forge", PlayMode.StopAll);
    barSummonInForge.OnSpellFinished();
    yield return (object) new WaitForSeconds(1f);
    barSummonInForge.SetVisibility(barSummonInForge.m_echoQuad, false);
  }
}
