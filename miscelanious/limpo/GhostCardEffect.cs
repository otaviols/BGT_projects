using Blizzard.T5.Core.Utils;
using Blizzard.T5.Services;
using System.Collections;
using UnityEngine;

public class GhostCardEffect : Spell
{
  public GameObject m_Glow;
  public GameObject m_GlowUnique;

  protected override void OnBirth(SpellStateType prevStateType)
  {
    if ((Object) this.m_Glow != (Object) null)
      this.m_Glow.GetComponent<Renderer>().enabled = false;
    if ((Object) this.m_GlowUnique != (Object) null)
      this.m_GlowUnique.GetComponent<Renderer>().enabled = false;
    this.StartCoroutine(this.GhostEffect(prevStateType));
  }

  protected override void OnDeath(SpellStateType prevStateType)
  {
    if ((Object) this.m_Glow != (Object) null)
      this.m_Glow.GetComponent<Renderer>().enabled = false;
    if ((Object) this.m_GlowUnique != (Object) null)
      this.m_GlowUnique.GetComponent<Renderer>().enabled = false;
    base.OnDeath(prevStateType);
    this.OnSpellFinished();
  }

  private IEnumerator GhostEffect(SpellStateType prevStateType)
  {
    GhostCardEffect ghostCardEffect = this;
    Actor actor = GameObjectUtils.FindComponentInParents<Actor>(ghostCardEffect.gameObject);
    if ((Object) actor == (Object) null)
    {
      Debug.LogWarning((object) "GhostCardEffect actor is null");
    }
    else
    {
      GhostCard ghostCard = ghostCardEffect.gameObject.GetComponentInChildren<GhostCard>();
      if ((Object) ghostCard == (Object) null)
      {
        Debug.LogWarning((object) "GhostCardEffect GhostCard is null");
      }
      else
      {
        if ((Object) ghostCardEffect.m_Glow != (Object) null)
        {
          GameObject gameObject = ghostCardEffect.m_Glow;
          if (actor.IsElite() && (Object) ghostCardEffect.m_GlowUnique != (Object) null)
            gameObject = ghostCardEffect.m_GlowUnique;
          gameObject.GetComponent<Renderer>().enabled = true;
        }
        TooltipPanelManager.Get().HideKeywordHelp();
        ghostCard.RenderGhostCard();
        yield return (object) new WaitForEndOfFrame();
        RenderToTexture componentInChildren = ghostCardEffect.gameObject.GetComponentInChildren<RenderToTexture>();
        if ((bool) (Object) componentInChildren)
        {
          componentInChildren.m_RealtimeRender = ServiceManager.Get<IGraphicsManager>().RenderQualityLevel == GraphicsQuality.High && actor.GetPremium() == TAG_PREMIUM.GOLDEN;
          componentInChildren.m_LateUpdate = true;
        }
        ghostCard.RenderGhostCard(true);
        actor.Show();
        TooltipPanelManager.Get().HideKeywordHelp();
        componentInChildren.Render();
        // ISSUE: reference to a compiler-generated method
        ghostCardEffect.\u003C\u003En__0(prevStateType);
        ghostCardEffect.OnSpellFinished();
      }
    }
  }
}
