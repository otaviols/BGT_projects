using System.Collections.Generic;

public class LettuceBossMissionEntity : LettucePvEMissionEntity
{
  public static LettucePvEMissionEntity InstantiateLettuceBountyMissionEntityForBoss(
    List<Network.PowerHistory> powerList)
  {
    switch (LettuceBossMissionEntity.GetBossDesignCode(powerList))
    {
      case "LETL_815H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_815H_VoHandler()));
      case "LETL_816H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_816H_VoHandler()));
      case "LETL_817H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_817H_VoHandler()));
      case "LETL_818H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_818H_VoHandler()));
      case "LETL_819H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_819H_VoHandler()));
      case "LETL_820H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_820H_VoHandler()));
      case "LETL_821H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_821H_VoHandler()));
      case "LETL_822H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_822H_VoHandler()));
      case "LETL_823H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_823H_VoHandler()));
      case "LETL_824H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_824H_VoHandler()));
      case "LETL_825H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_825H_VoHandler()));
      case "LETL_826H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_826H_VoHandler()));
      case "LETL_827H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_827H_VoHandler()));
      case "LETL_828H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_828H_VoHandler()));
      case "LETL_829H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_829H_VoHandler()));
      case "LETL_830H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_830H_VoHandler()));
      case "LETL_831H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_831H_VoHandler()));
      case "LETL_832H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_832H_VoHandler()));
      case "LETL_833H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_833H_VoHandler()));
      case "LETL_834H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_834H_VoHandler()));
      case "LETL_835H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_835H_VoHandler()));
      case "LETL_836H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_836H_VoHandler()));
      case "LETL_837H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_837H_VoHandler()));
      case "LETL_838H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_838H_VoHandler()));
      case "LETL_839H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_839H_VoHandler()));
      case "LETL_840H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_840H_VoHandler()));
      case "LETL_841H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_841H_VoHandler()));
      case "LETL_842H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_842H_VoHandler()));
      case "LETL_843H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_843H_VoHandler()));
      case "LETL_844H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_844H_VoHandler()));
      case "LETL_845H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_845H_VoHandler()));
      case "LETL_846H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_846H_VoHandler()));
      case "LETL_847H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_847H_VoHandler()));
      case "LETL_848H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_848H_VoHandler()));
      case "LETL_848H_Heroic":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_848H_Heroic_VoHandler()));
      case "LETL_849H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_849H_VoHandler()));
      case "LETL_850H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_850H_VoHandler()));
      case "LETL_851H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_851H_VoHandler()));
      case "LETL_852H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_852H_VoHandler()));
      case "LETL_853H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_853H_VoHandler()));
      case "LETL_854H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_854H_VoHandler()));
      case "LETL_855H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_855H_VoHandler()));
      case "LETL_856H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_856H_VoHandler()));
      case "LETL_857H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_857H_VoHandler()));
      case "LETL_858H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_858H_VoHandler()));
      case "LETL_859H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_859H_VoHandler()));
      case "LETL_860H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_860H_VoHandler()));
      case "LETL_861H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_861H_VoHandler()));
      case "LETL_862H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_862H_VoHandler()));
      case "LETL_863H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_863H_VoHandler()));
      case "LETL_864H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_865H_VoHandler()));
      case "LETL_865H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_866H_VoHandler()));
      case "LETL_866H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LETL_864H_VoHandler()));
      case "LT23_800H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_800H_VoHandler()));
      case "LT23_801H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_801H_VoHandler()));
      case "LT23_802H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_802H_VoHandler()));
      case "LT23_803H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_803H_VoHandler()));
      case "LT23_804H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_804H_VoHandler()));
      case "LT23_805H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_805H_VoHandler()));
      case "LT23_806H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_806H_VoHandler()));
      case "LT23_807H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_807H_VoHandler()));
      case "LT23_809H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_809H_VoHandler()));
      case "LT23_811H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_811H_VoHandler()));
      case "LT23_812H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_812H_VoHandler()));
      case "LT23_813H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_813H_VoHandler()));
      case "LT23_815H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_815H_VoHandler()));
      case "LT23_816H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_816H_VoHandler()));
      case "LT23_817H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_817H_VoHandler()));
      case "LT23_818H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_818H_VoHandler()));
      case "LT23_819H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_819H_VoHandler()));
      case "LT23_820H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_820H_VoHandler()));
      case "LT23_821H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_821H_VoHandler()));
      case "LT23_822H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_822H_VoHandler()));
      case "LT23_823H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_823H_VoHandler()));
      case "LT23_825H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_825H_VoHandler()));
      case "LT23_826H1":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT23_826H1_VoHandler()));
      case "LT24_810H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_810H_VoHandler()));
      case "LT24_811H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_811H_VoHandler()));
      case "LT24_812H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_812H_VoHandler()));
      case "LT24_813H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_813H_VoHandler()));
      case "LT24_814H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_814H_VoHandler()));
      case "LT24_814H4":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_814H4_VoHandler()));
      case "LT24_814H6":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_814H6_VoHandler()));
      case "LT24_815H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_815H_VoHandler()));
      case "LT24_816H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_816H_VoHandler()));
      case "LT24_817H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_817H_VoHandler()));
      case "LT24_818H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_818H_VoHandler()));
      case "LT24_819H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_819H_VoHandler()));
      case "LT24_820H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_820H_VoHandler()));
      case "LT24_821H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_821H_VoHandler()));
      case "LT24_822H":
        return new LettucePvEMissionEntity(voHandler: ((VoPlaybackHandler) new LettuceBoss_LT24_822H_VoHandler()));
      default:
        return new LettucePvEMissionEntity();
    }
  }

  protected static string GetBossDesignCode(List<Network.PowerHistory> powerList)
  {
    foreach (Network.PowerHistory power in powerList)
    {
      if (power.Type == Network.PowerType.FULL_ENTITY)
      {
        Network.Entity entity = ((Network.HistFullEntity) power).Entity;
        foreach (Network.Entity.Tag tag in entity.Tags)
        {
          if (tag.Name == 2168 && tag.Value > 0)
            return entity.CardID;
        }
      }
    }
    return string.Empty;
  }
}
