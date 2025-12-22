using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR08_Nightbane : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_NightbaneTurn1_01.prefab:f978a9a33588ae74ebe60e591e6cf238");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_NightbaneUnstablePortal_01.prefab:48506385c46091f47832e56cb3bb2628");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_NightbaneCorruption_01.prefab:0a9693596358c7b4dbcdf6d6eff5f09e");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_NightbaneWin_01.prefab:ffe915d9bd0f53540beb314b0df007a0");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_NightbaneTurn3_01.prefab:b9cb531bca9116d4f902e157611789e8");
    this.PreloadSound("VO_Moroes_Male_Human_NightbaneTurn3_01.prefab:f4d914a3415bb074c825a09dcc164d86");
    this.PreloadSound("VO_Moroes_Male_Human_NightbaneTurn7_01.prefab:6361ea4ac8ffcf646849993f464a6b06");
    this.PreloadSound("VO_Nightbane_Roar.prefab:d5f389f135f07b547b98e7d58a4fcd20");
  }

  protected override void InitEmoteResponses() => this.m_emoteResponseGroups = new List<MissionEntity.EmoteResponseGroup>()
  {
    new MissionEntity.EmoteResponseGroup()
    {
      m_triggers = new List<EmoteType>((IEnumerable<EmoteType>) MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS),
      m_responses = new List<MissionEntity.EmoteResponse>()
      {
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_Nightbane_Roar.prefab:d5f389f135f07b547b98e7d58a4fcd20",
          m_stringTag = "VO_Nightbane_Roar"
        }
      }
    }
  };

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR08_Nightbane kaR08Nightbane = this;
    while (kaR08Nightbane.m_enemySpeaking)
      yield return (object) null;
    switch (turn)
    {
      case 1:
        yield return (object) kaR08Nightbane.PlayOpeningLine("Curator_BigQuote.prefab:f01875528133988418925bd870aa7b81", "VO_Curator_Male_ArcaneGolem_NightbaneTurn1_01.prefab:f978a9a33588ae74ebe60e591e6cf238");
        break;
      case 4:
        GameState.Get().SetBusy(true);
        yield return (object) kaR08Nightbane.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_NightbaneTurn3_01.prefab:f4d914a3415bb074c825a09dcc164d86");
        yield return (object) kaR08Nightbane.PlayAdventureFlavorLine("Curator_BigQuote.prefab:f01875528133988418925bd870aa7b81", "VO_Curator_Male_ArcaneGolem_NightbaneTurn3_01.prefab:b9cb531bca9116d4f902e157611789e8");
        GameState.Get().SetBusy(false);
        break;
      case 8:
        GameState.Get().SetBusy(true);
        yield return (object) kaR08Nightbane.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_NightbaneTurn7_01.prefab:6361ea4ac8ffcf646849993f464a6b06");
        GameState.Get().SetBusy(false);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    KAR08_Nightbane kaR08Nightbane = this;
    while (kaR08Nightbane.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR08Nightbane.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR08Nightbane.m_playedLines.Add(cardId);
      if (!(cardId == "GVG_003"))
      {
        if (cardId == "OG_133" || cardId == "OG_280" || cardId == "OG_134" || cardId == "OG_042")
        {
          kaR08Nightbane.m_playedLines.Add("OG_133");
          kaR08Nightbane.m_playedLines.Add("OG_280");
          kaR08Nightbane.m_playedLines.Add("OG_134");
          kaR08Nightbane.m_playedLines.Add("OG_042");
          yield return (object) kaR08Nightbane.PlayEasterEggLine("Curator_BigQuote.prefab:f01875528133988418925bd870aa7b81", "VO_Curator_Male_ArcaneGolem_NightbaneCorruption_01.prefab:0a9693596358c7b4dbcdf6d6eff5f09e");
        }
      }
      else
        yield return (object) kaR08Nightbane.PlayEasterEggLine("Curator_BigQuote.prefab:f01875528133988418925bd870aa7b81", "VO_Curator_Male_ArcaneGolem_NightbaneUnstablePortal_01.prefab:48506385c46091f47832e56cb3bb2628");
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR08_Nightbane kaR08Nightbane = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR08Nightbane.PlayClosingLine("Curator_Quote.prefab:ab58be80382875e4cbaa766fda73cd39", "VO_Curator_Male_ArcaneGolem_NightbaneWin_01.prefab:ffe915d9bd0f53540beb314b0df007a0");
    }
  }
}
