using UnityEngine;

public class MercenariesDeckCover : DeckCover
{
  public GameObject m_ProtectorGem;
  public UberText m_ProtectorCountText;
  public GameObject m_FighterGem;
  public UberText m_FighterCountText;
  public GameObject m_CasterGem;
  public UberText m_CasterCountText;

  public override void UpdateVisual(Player.Side side)
  {
    int numProtector;
    int numCaster;
    int numFighter;
    this.GetRoleCountInZone((Zone) ZoneMgr.Get().FindZoneOfType<ZoneDeck>(side), out numProtector, out numCaster, out numFighter);
    this.UpdateRoleComponent(numProtector, this.m_ProtectorGem, this.m_ProtectorCountText);
    this.UpdateRoleComponent(numCaster, this.m_CasterGem, this.m_CasterCountText);
    this.UpdateRoleComponent(numFighter, this.m_FighterGem, this.m_FighterCountText);
  }

  private void UpdateRoleComponent(int count, GameObject rootGemObject, UberText gemText)
  {
    if (count == 0)
    {
      rootGemObject.SetActive(false);
    }
    else
    {
      rootGemObject.SetActive(true);
      gemText.Text = count.ToString();
    }
  }

  private void GetRoleCountInZone(
    Zone zone,
    out int numProtector,
    out int numCaster,
    out int numFighter)
  {
    numProtector = 0;
    numCaster = 0;
    numFighter = 0;
    if ((Object) zone == (Object) null)
      return;
    foreach (Card card in zone.GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity != null)
      {
        switch (entity.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE))
        {
          case TAG_ROLE.CASTER:
            ++numCaster;
            continue;
          case TAG_ROLE.FIGHTER:
            ++numFighter;
            continue;
          case TAG_ROLE.TANK:
            ++numProtector;
            continue;
          default:
            continue;
        }
      }
    }
  }
}
