using Blizzard.T5.Core;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSocketDecoration : MonoBehaviour
{
  public List<WeaponSocketRequirement> m_VisibilityRequirements;

  public bool IsShown() => this.GetComponent<Renderer>().enabled;

  public void UpdateVisibility()
  {
    if (this.AreVisibilityRequirementsMet())
      this.Show();
    else
      this.Hide();
  }

  public bool AreVisibilityRequirementsMet()
  {
    Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
    if (playerMap == null || this.m_VisibilityRequirements == null)
      return false;
    foreach (WeaponSocketRequirement visibilityRequirement in this.m_VisibilityRequirements)
    {
      bool flag = false;
      foreach (Player player in playerMap.Values)
      {
        if (visibilityRequirement.m_Side == player.GetSide())
        {
          Entity hero = player.GetHero();
          if (hero == null)
          {
            Debug.LogWarning((object) string.Format("WeaponSocketDecoration.AreVisibilityRequirementsMet() - player {0} has no hero", (object) player));
            return false;
          }
          if (visibilityRequirement.m_HasWeapon != WeaponSocketMgr.ShouldSeeWeaponSocket(hero.GetClass()))
            return false;
          flag = true;
        }
      }
      if (!flag)
        return false;
    }
    return true;
  }

  public void Show() => RenderUtils.EnableRenderers(this.gameObject, true);

  public void Hide() => RenderUtils.EnableRenderers(this.gameObject, false);
}
