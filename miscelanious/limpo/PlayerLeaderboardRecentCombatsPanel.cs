using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeaderboardRecentCombatsPanel : PlayerLeaderboardInformationPanel
{
  public uint m_maxDisplayItems = 2;
  public List<GameObject> m_recentActionPlaceholders;
  public GameObject m_recentActionsParent;
  public static int NO_DAMAGE_TARGET = 100000;
  private QueueList<PlayerLeaderboardRecentCombatEntry> m_recentCombatEntries = new QueueList<PlayerLeaderboardRecentCombatEntry>();
  private const string RECENT_COMBAT_ENTRY_PREFAB = "Recent_Combat_Entry.prefab:74bf698d81967c9498554a64c9db51fc";
  private int m_triplesCount;
  private int m_winStreakCount;
  private int m_techLevelCount = 1;
  private bool m_racesInitialized;
  public PlayerLeaderboardIcon m_techLevel;
  public PlayerLeaderboardIcon m_winStreak;
  public PlayerLeaderboardIcon m_triples;
  public DamageCapPanel m_damageCap;
  private int m_damageCapValue;
  public List<GameObject> m_raceWrappers;
  public GameObject m_singleTribeWithCountWrapper;
  public UberText m_singleTribeWithCountName;
  public UberText m_singleTribeWithCountNumber;
  public GameObject m_singleTribeWithoutCountWrapper;
  public UberText m_singleTribeWithoutCountName;

  public void Awake()
  {
    for (int index = 0; index < this.m_recentActionPlaceholders.Count; ++index)
    {
      PlayerLeaderboardRecentCombatEntry component = this.m_recentActionPlaceholders[index].GetComponent<PlayerLeaderboardRecentCombatEntry>();
      component.m_iconOpponentSwords.SetActive(false);
      component.m_iconOwnerSwords.SetActive(false);
      component.m_iconOpponentSplat.SetActive(false);
      component.m_iconOwnerSplat.SetActive(false);
      component.m_opponentTileActor.gameObject.SetActive(false);
      component.m_background.gameObject.SetActive(true);
    }
    this.m_techLevel.ClearText();
    this.m_winStreak.ClearText();
    this.m_triples.ClearText();
    this.UpdateDamageCap();
  }

  public bool HasRecentCombats() => this.m_recentCombatEntries.Count > 0;

  public void ClearRecentCombats()
  {
    while (this.m_recentCombatEntries.Count > 0)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_recentCombatEntries.Dequeue().gameObject);
  }

  public int GetTripleCount() => this.m_triplesCount;

  public void SetTriples(int triples)
  {
    this.m_triplesCount = triples;
    this.UpdateLayout();
  }

  public void SetTechLevel(int techLevel)
  {
    this.m_techLevelCount = techLevel;
    this.UpdateLayout();
  }

  public void AddRecentCombat(
    PlayerLeaderboardCard source,
    PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo recentCombatInfo)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Recent_Combat_Entry.prefab:74bf698d81967c9498554a64c9db51fc");
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogWarningFormat("PlayerLeaderboardRecentCombatsPanel.AddRecentCombat() - FAILED to load GameObject \"{0}\"", (object) "Recent_Combat_Entry.prefab:74bf698d81967c9498554a64c9db51fc");
    }
    else
    {
      PlayerLeaderboardRecentCombatEntry component = gameObject.GetComponent<PlayerLeaderboardRecentCombatEntry>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("PlayerLeaderboardRecentCombatsPanel.AddRecentCombat() - ERROR GameObject \"{0}\" has no PlayerLeaderboardRecentCombatEntry component", (object) "Recent_Combat_Entry.prefab:74bf698d81967c9498554a64c9db51fc");
      }
      else
      {
        TransformUtil.Identity((Component) component.transform);
        component.Load(source, recentCombatInfo);
        if ((long) this.m_recentCombatEntries.Count == (long) this.m_maxDisplayItems)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_recentCombatEntries.Dequeue().gameObject);
        this.m_recentCombatEntries.Enqueue(component);
        this.m_winStreakCount = recentCombatInfo.winStreak;
        this.UpdateLayout();
      }
    }
  }

  private void UpdateDamageCap()
  {
    if (!((UnityEngine.Object) this.m_damageCap != (UnityEngine.Object) null))
      return;
    this.m_damageCapValue = GameState.Get().GetGameEntity().GetTag(GAME_TAG.BACON_COMBAT_DAMAGE_CAP);
    this.m_damageCap.gameObject.SetActive(this.m_damageCapValue != 0);
    this.m_damageCap.SetText(this.m_damageCapValue.ToString());
  }

  private void UpdateTechLevelPlaymaker()
  {
    PlayMakerFSM component = this.m_techLevel.GetComponent<PlayMakerFSM>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("No playmaker attached to tech level icon.");
    }
    else
    {
      component.FsmVariables.GetFsmInt("TechLevel").Value = this.m_techLevelCount;
      component.SendEvent("Action");
    }
  }

  private void UpdateLayout()
  {
    if ((UnityEngine.Object) this.m_triples != (UnityEngine.Object) null)
      this.m_triples.SetText(this.m_triplesCount.ToString());
    if ((UnityEngine.Object) this.m_winStreak != (UnityEngine.Object) null)
      this.m_winStreak.SetText(this.m_winStreakCount.ToString());
    if ((UnityEngine.Object) this.m_techLevel != (UnityEngine.Object) null)
      this.UpdateTechLevelPlaymaker();
    this.UpdateDamageCap();
    if (this.m_recentActionPlaceholders == null)
      return;
    for (int index = 0; index < this.m_recentActionPlaceholders.Count; ++index)
    {
      if (this.m_recentCombatEntries.Count > index)
      {
        GameObject actionPlaceholder = this.m_recentActionPlaceholders[Math.Min(this.m_recentActionPlaceholders.Count, this.m_recentCombatEntries.Count) - (1 + index)];
        GameObject gameObject = this.m_recentCombatEntries[index].gameObject;
        gameObject.transform.parent = actionPlaceholder.transform.parent;
        TransformUtil.CopyLocal(gameObject, actionPlaceholder);
        actionPlaceholder.SetActive(false);
      }
      else
        this.m_recentActionPlaceholders[index].SetActive(true);
    }
  }

  internal bool SetRaces(Map<TAG_RACE, int> raceCounts)
  {
    this.InitRaces(raceCounts);
    int num1 = 0;
    if (raceCounts.ContainsKey(TAG_RACE.ALL))
      num1 = raceCounts[TAG_RACE.ALL];
    TAG_RACE tag = TAG_RACE.ALL;
    int num2 = 0;
    int num3 = 0;
    foreach (KeyValuePair<TAG_RACE, int> raceCount in raceCounts)
    {
      if (raceCount.Key != TAG_RACE.ALL)
      {
        int num4 = raceCount.Value + num1;
        if (num4 >= num2 && num4 > 0)
        {
          num3 = num2;
          num2 = num4;
          tag = raceCount.Key;
        }
        else if (num4 >= num3 && num4 > 0)
        {
          num3 = num4;
          int key = (int) raceCount.Key;
        }
      }
    }
    if (tag == TAG_RACE.ALL || num2 == num3)
    {
      if (num2 == 0)
      {
        this.m_singleTribeWithoutCountWrapper.SetActive(false);
        this.m_singleTribeWithCountWrapper.SetActive(false);
      }
      else
      {
        this.m_singleTribeWithoutCountWrapper.SetActive(true);
        this.m_singleTribeWithCountWrapper.SetActive(false);
      }
    }
    else
    {
      this.m_singleTribeWithoutCountWrapper.SetActive(false);
      this.m_singleTribeWithCountWrapper.SetActive(true);
      this.m_singleTribeWithCountNumber.Text = num2.ToString();
      this.m_singleTribeWithCountName.Text = GameStrings.GetRaceNameBattlegrounds(tag);
    }
    return this.m_racesInitialized;
  }

  private void InitRaces(Map<TAG_RACE, int> raceCounts)
  {
    if (this.m_racesInitialized || raceCounts.Count == 0)
      return;
    this.m_racesInitialized = true;
  }

  public struct RecentCombatInfo
  {
    public int ownerId;
    public int opponentId;
    public int damageTarget;
    public int damage;
    public int winStreak;
    public int loseStreak;
    public bool isDefeated;
  }
}
