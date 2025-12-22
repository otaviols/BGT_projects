using System.Collections.Generic;
using UnityEngine;

public class MagtheridonLinkToHellfireWardersSpell : MouseOverLinkSpell
{
  public static readonly string MagtheridonId = "BT_850";
  public static readonly string HellfireWarderId = "BT_850t";

  protected override void GetAllTargets(Entity source, List<GameObject> targets)
  {
    if (source == null || targets == null)
      return;
    ZoneMgr zoneMgr = ZoneMgr.Get();
    if ((Object) zoneMgr == (Object) null)
      return;
    bool flag1 = false;
    bool flag2 = source.IsControlledByFriendlySidePlayer();
    Player.Side side1;
    Player.Side side2;
    if (source.GetCardId() == MagtheridonLinkToHellfireWardersSpell.MagtheridonId)
    {
      side1 = flag2 ? Player.Side.OPPOSING : Player.Side.FRIENDLY;
      side2 = flag2 ? Player.Side.FRIENDLY : Player.Side.OPPOSING;
      flag1 = true;
    }
    else
    {
      if (!(source.GetCardId() == MagtheridonLinkToHellfireWardersSpell.HellfireWarderId))
        return;
      side1 = flag2 ? Player.Side.FRIENDLY : Player.Side.OPPOSING;
      side2 = flag2 ? Player.Side.OPPOSING : Player.Side.FRIENDLY;
    }
    ZonePlay zoneOfType1 = zoneMgr.FindZoneOfType<ZonePlay>(side1);
    ZonePlay zoneOfType2 = zoneMgr.FindZoneOfType<ZonePlay>(side2);
    int num1 = 0;
    int num2 = 0;
    foreach (Card card in zoneOfType2.GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetCardId() == MagtheridonLinkToHellfireWardersSpell.MagtheridonId && entity.IsDormant())
      {
        if (flag1)
        {
          if ((Object) card.gameObject == (Object) this.m_source)
          {
            targets.Add(card.gameObject);
            ++num1;
          }
        }
        else
        {
          targets.Add(card.gameObject);
          ++num1;
        }
      }
    }
    foreach (Card card in zoneOfType1.GetCards())
    {
      if (card.GetEntity().GetCardId() == MagtheridonLinkToHellfireWardersSpell.HellfireWarderId)
      {
        targets.Add(card.gameObject);
        ++num2;
      }
    }
    if (num1 != 0 && num2 != 0)
      return;
    targets.Clear();
  }
}
