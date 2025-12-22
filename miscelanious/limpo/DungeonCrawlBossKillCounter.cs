using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DungeonCrawl;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCrawlBossKillCounter : MonoBehaviour
{
  public UberText[] m_bossWinsText;
  public UberText m_runWinsText;
  public UberText m_runNotCompleteBossWinsHeader;
  public UberText m_runCompleteBossWinsHeader;
  public UberText m_runCompleteRunWinsHeader;
  public UberText m_fullPanelText;
  public GameObject m_runNotCompletedPanel;
  public GameObject m_runCompletedPanel;
  public GameObject[] m_numberHolderShadow;
  public DungeonCrawlBossKillCounter.BossKillCounterStyleOverride[] m_bossKillCounterStyle;
  private long m_bossWins;
  private long m_runWins;
  private TAG_CLASS m_heroClass;
  private string m_bossWinsHeaderRunNotCompletedString;
  private string m_bossWinsHeaderRunCompletedString;
  private IDungeonCrawlData m_dungeonCrawlData;

  private void Awake()
  {
    this.m_runNotCompletedPanel.SetActive(false);
    this.m_runCompletedPanel.SetActive(false);
  }

  public void SetDungeonRunData(IDungeonCrawlData data)
  {
    this.m_dungeonCrawlData = data;
    this.SetBossKillCounterVisualStyle();
  }

  public void SetHeroClass(TAG_CLASS heroClass) => this.m_heroClass = heroClass;

  public void SetBossWins(long bossWins)
  {
    this.m_bossWins = bossWins;
    foreach (UberText uberText in this.m_bossWinsText)
      uberText.Text = this.m_bossWins.ToString();
  }

  public void SetRunWins(long runWins)
  {
    this.m_runWins = runWins;
    this.m_runWinsText.Text = this.m_runWins.ToString();
  }

  public void UpdateLayout()
  {
    AdventureDataDbfRecord adventureDataRecord = this.m_dungeonCrawlData.GetSelectedAdventureDataRecord();
    if (adventureDataRecord != null && adventureDataRecord.DungeonCrawlShowBossKillCount)
    {
      this.m_fullPanelText.gameObject.SetActive(false);
      bool flag = this.m_runWins > 0L;
      this.m_runNotCompletedPanel.SetActive(!flag);
      this.m_runCompletedPanel.SetActive(flag);
      if (!flag && (UnityEngine.Object) this.m_runNotCompleteBossWinsHeader != (UnityEngine.Object) null)
      {
        int fromHeroCardDbId = AdventureUtils.GetGuestHeroIdFromHeroCardDbId(this.m_dungeonCrawlData, (int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
        this.m_runNotCompleteBossWinsHeader.Text = GameStrings.Format(this.m_bossWinsHeaderRunNotCompletedString, (object) (SceneMgr.Get().IsInDuelsMode() ? this.GetDisplayableHeroNameFromGuestHeroId(fromHeroCardDbId) : this.GetDisplayableClassName(true)));
      }
      else
      {
        if (!flag || !((UnityEngine.Object) this.m_runCompleteBossWinsHeader != (UnityEngine.Object) null))
          return;
        this.m_runCompleteBossWinsHeader.Text = GameStrings.Format(this.m_bossWinsHeaderRunCompletedString);
      }
    }
    else
    {
      this.m_runNotCompletedPanel.SetActive(false);
      this.m_runCompletedPanel.SetActive(false);
      this.m_fullPanelText.gameObject.SetActive(true);
      ScenarioDbId missionToPlay = this.m_dungeonCrawlData.GetMissionToPlay();
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) missionToPlay);
      if (record == null)
        return;
      this.m_fullPanelText.Text = (string) record.Description;
    }
  }

  private string GetDisplayableHeroNameFromGuestHeroId(int guestHeroId)
  {
    AdventureGuestHeroesDbfRecord record = GameDbf.AdventureGuestHeroes.GetRecord((Predicate<AdventureGuestHeroesDbfRecord>) (r => r.GuestHeroId == guestHeroId));
    if (record == null)
    {
      Debug.LogError((object) string.Format("GetDisplayableHeroNameFromGuestHeroId: No guest hero found for {0}.", (object) guestHeroId));
      return string.Empty;
    }
    if (record.GuestHeroRecord != null)
      return (string) (!string.IsNullOrEmpty((string) record.GuestHeroRecord.ShortName) ? record.GuestHeroRecord.ShortName : record.GuestHeroRecord.Name);
    Debug.LogError((object) string.Format("GetDisplayableHeroNameFromGuestHeroId: No guest hero record found for {0}.", (object) guestHeroId));
    return string.Empty;
  }

  private string GetDisplayableClassName(bool preferClassNameOverHeroName)
  {
    string displayableClassName = GameStrings.GetClassName(this.m_heroClass);
    if (preferClassNameOverHeroName)
      return displayableClassName;
    AdventureDbId currentAdventure = this.m_dungeonCrawlData.GetSelectedAdventure();
    List<AdventureGuestHeroesDbfRecord> records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == currentAdventure));
    List<CardDbfRecord> cardDbfRecordList = new List<CardDbfRecord>();
    foreach (AdventureGuestHeroesDbfRecord guestHeroesDbfRecord in records)
      cardDbfRecordList.Add(GameDbf.Card.GetRecord(GameUtils.GetCardIdFromGuestHeroDbId(guestHeroesDbfRecord.GuestHeroId)));
    foreach (CardDbfRecord cardDbfRecord in cardDbfRecordList)
    {
      CardDbfRecord cardRecord = cardDbfRecord;
      if (GameUtils.GetTagClassFromCardDbId(cardRecord.ID) == this.m_heroClass)
      {
        GuestHeroDbfRecord record = GameDbf.GuestHero.GetRecord((Predicate<GuestHeroDbfRecord>) (r => r.CardId == cardRecord.ID));
        if (record != null)
        {
          displayableClassName = (string) record.Name;
          break;
        }
      }
    }
    return displayableClassName;
  }

  private void SetBossKillCounterVisualStyle()
  {
    DungeonRunVisualStyle visualStyle = this.m_dungeonCrawlData.VisualStyle;
    foreach (DungeonCrawlBossKillCounter.BossKillCounterStyleOverride counterStyleOverride in this.m_bossKillCounterStyle)
    {
      if (visualStyle == counterStyleOverride.VisualStyle)
      {
        this.m_bossWinsHeaderRunNotCompletedString = counterStyleOverride.BossWinsRunNotCompletedString;
        this.m_bossWinsHeaderRunCompletedString = counterStyleOverride.BossWinsRunCompletedString;
        this.m_runCompleteRunWinsHeader.Text = counterStyleOverride.RunWinsString;
        foreach (GameObject gameObject in this.m_numberHolderShadow)
        {
          MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null && (UnityEngine.Object) counterStyleOverride.NumberHolderShadowMaterial != (UnityEngine.Object) null)
            component.SetMaterial(counterStyleOverride.NumberHolderShadowMaterial);
        }
        this.m_runNotCompleteBossWinsHeader.TextColor = counterStyleOverride.DescriptionTextColor;
        this.m_runCompleteBossWinsHeader.TextColor = counterStyleOverride.DescriptionTextColor;
        this.m_runCompleteRunWinsHeader.TextColor = counterStyleOverride.DescriptionTextColor;
        this.m_fullPanelText.TextColor = counterStyleOverride.DescriptionTextColor;
        break;
      }
    }
  }

  [Serializable]
  public class BossKillCounterStyleOverride
  {
    public DungeonRunVisualStyle VisualStyle;
    public string BossWinsRunNotCompletedString;
    public string BossWinsRunCompletedString;
    public string RunWinsString;
    public Material NumberHolderShadowMaterial;
    public Color DescriptionTextColor;
  }
}
