using Blizzard.T5.Core;
using PegasusLettuce;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LettuceTeam
{
  public static int DefaultMaxTeamNameCharacters = 24;
  public const long INVALID_TEAM_ID = 0;
  private string m_name;
  private List<LettuceMercenary> m_lettuceMercs = new List<LettuceMercenary>();
  private Map<LettuceMercenary, LettuceMercenary.Loadout> m_loadouts = new Map<LettuceMercenary, LettuceMercenary.Loadout>();
  private bool m_netContentsLoaded;
  private bool m_isSavingContentChanges;
  private bool m_isSavingNameChanges;
  private bool m_isBeingDeleted;
  private uint m_sortOrder;
  private bool m_sortOrderDirty;
  private bool m_dirty;
  public long ID;
  public DeckType Type = DeckType.NORMAL_DECK;
  public bool NeedsName;
  public ulong CreateDate;
  public bool Locked;
  public PegasusLettuce.LettuceTeam.Type TeamType;

  public LettuceTeam()
  {
  }

  public LettuceTeam(uint sortOrder) => this.m_sortOrder = sortOrder;

  public override string ToString() => string.Format("Team [id={0} name=\"{1}\" heroCount={2} needsName={3} sortOrder={4}]", (object) this.ID, (object) this.Name, (object) this.GetMercCount(), (object) this.NeedsName, (object) this.SortOrder);

  public string Name
  {
    get => this.m_name;
    set
    {
      if (value == null)
      {
        Debug.LogError((object) string.Format("LettuceTeam.SetName() - null name given for team {0}", (object) this));
      }
      else
      {
        if (value.Equals(this.m_name, StringComparison.InvariantCultureIgnoreCase))
          return;
        this.m_dirty = true;
        this.m_name = value;
      }
    }
  }

  public uint SortOrder
  {
    set
    {
      if ((int) this.m_sortOrder == (int) value)
        return;
      this.m_sortOrder = value;
      this.m_sortOrderDirty = true;
    }
    get => this.m_sortOrder;
  }

  public LettuceMercenary GetLeader() => this.m_lettuceMercs.Count > 0 && this.m_lettuceMercs[0] != null ? this.m_lettuceMercs[0] : (LettuceMercenary) null;

  public void MarkNetworkContentsLoaded() => this.m_netContentsLoaded = true;

  public bool NetworkContentsLoaded() => this.m_netContentsLoaded;

  public void MarkBeingDeleted() => this.m_isBeingDeleted = true;

  public bool IsBeingDeleted() => this.m_isBeingDeleted;

  public bool IsSavingChanges() => this.m_isSavingNameChanges || this.m_isSavingContentChanges;

  public bool IsBeingEdited() => this == CollectionManager.Get().GetEditingTeam();

  public List<LettuceMercenary> GetMercs() => this.m_lettuceMercs;

  public int GetMercCount() => this.m_lettuceMercs.Count;

  public LettuceMercenary.Loadout GetLoadout(LettuceMercenary merc)
  {
    LettuceMercenary.Loadout loadout;
    this.m_loadouts.TryGetValue(merc, out loadout);
    return loadout;
  }

  public void ClearContents()
  {
    this.m_lettuceMercs.Clear();
    this.m_loadouts.Clear();
  }

  public bool AddMerc(string cardId, int index = -1, LettuceMercenary.Loadout loadout = null)
  {
    if (string.IsNullOrEmpty(cardId))
      return false;
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary(cardId);
    if (mercenary != null)
      return this.AddMerc(mercenary, index, loadout);
    Log.Lettuce.PrintError("No mercenary with cardId = {0} in collection!", (object) cardId);
    return false;
  }

  public bool AddMerc(LettuceMercenary merc, int index = -1, LettuceMercenary.Loadout loadout = null)
  {
    if (merc == null)
    {
      Log.Lettuce.PrintError("LettuceTeam.AddMerc - null mercenary passed!");
      return false;
    }
    if (this.m_lettuceMercs.Find((Predicate<LettuceMercenary>) (m => m.ID == merc.ID)) != null)
      return false;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(merc.GetCardId());
    if (entityDef != null)
      merc.m_mercName = entityDef.GetName();
    if (index >= 0 && index < this.m_lettuceMercs.Count)
      this.m_lettuceMercs.Insert(index, merc);
    else
      this.m_lettuceMercs.Add(merc);
    this.m_loadouts.Add(merc, loadout != null ? loadout : new LettuceMercenary.Loadout(merc.GetBaseLoadout()));
    this.m_dirty = true;
    return true;
  }

  public bool RemoveMerc(int mercId)
  {
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercId);
    int num = this.m_lettuceMercs.Remove(mercenary) ? 1 : 0;
    this.m_loadouts.Remove(mercenary);
    this.m_dirty = true;
    return num != 0;
  }

  public bool IsMercInTeam(string cardID, bool owned = true)
  {
    foreach (LettuceMercenary lettuceMerc in this.m_lettuceMercs)
    {
      if (lettuceMerc.GetCardId().Equals(cardID) && lettuceMerc.m_owned == owned)
        return true;
    }
    return false;
  }

  public bool IsMercInTeam(int mercId, bool owned = true)
  {
    foreach (LettuceMercenary lettuceMerc in this.m_lettuceMercs)
    {
      if (lettuceMerc.ID == mercId && lettuceMerc.m_owned == owned)
        return true;
    }
    return false;
  }

  public bool TryGetMerc(int mercId, out LettuceMercenary result, bool owned = true)
  {
    result = (LettuceMercenary) null;
    foreach (LettuceMercenary lettuceMerc in this.m_lettuceMercs)
    {
      if (lettuceMerc.ID == mercId && lettuceMerc.m_owned == owned)
      {
        result = lettuceMerc;
        return true;
      }
    }
    return false;
  }

  public bool IsValid() => this.m_lettuceMercs.Count > 0;

  public bool IsDirty()
  {
    if (!this.m_dirty)
    {
      foreach (LettuceMercenary.Loadout loadout in this.m_loadouts.Values)
      {
        if (loadout.IsDirty())
          return true;
      }
    }
    return this.m_dirty;
  }

  public void ClearDirty()
  {
    this.m_dirty = false;
    foreach (LettuceMercenary.Loadout loadout in this.m_loadouts.Values)
      loadout.ClearDirty();
  }

  public bool DoesContainDisabledMerc()
  {
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    if (netObject == null)
    {
      Log.Lettuce.PrintError("DoesContainDisabledMerc - Can't access NetCacheMercenariesPlayerInfo");
      return false;
    }
    List<int> disabledMercenaryList = netObject.DisabledMercenaryList;
    if (disabledMercenaryList.Count != 0)
    {
      foreach (LettuceMercenary lettuceMerc in this.m_lettuceMercs)
      {
        if (disabledMercenaryList.Contains(lettuceMerc.ID))
          return true;
      }
    }
    return false;
  }

  public bool SendChanges()
  {
    bool flag = false;
    if (this.IsDirty())
    {
      Network.Get().UpdateMercenariesTeamRequest(this);
      this.ClearDirty();
      flag = true;
    }
    return flag;
  }

  public void SendTeamOrderChanges()
  {
    if (!this.m_sortOrderDirty)
      return;
    Network.Get().MercenariesTeamReorderRequest(this);
    this.m_sortOrderDirty = false;
  }

  public static LettuceTeam Convert(
    PegasusLettuce.LettuceTeam src,
    bool initializeWithBase = true,
    bool checkOwnership = true)
  {
    LettuceTeam lettuceTeam = (LettuceTeam) null;
    if (src == null)
      return lettuceTeam;
    LettuceTeam dest = new LettuceTeam();
    dest.ID = src.TeamId;
    dest.Name = src.Name;
    dest.SortOrder = src.SortOrder;
    dest.TeamType = src.Type_;
    if (!Enum.IsDefined(typeof (PegasusLettuce.LettuceTeam.Type), (object) dest.TeamType))
      dest.TeamType = PegasusLettuce.LettuceTeam.Type.TYPE_INVALID;
    if (!LettuceTeam.PopulateTeamMercenaries(src, dest, initializeWithBase, checkOwnership))
      dest = (LettuceTeam) null;
    dest.ClearDirty();
    return dest;
  }

  public static bool PopulateTeamMercenaries(
    PegasusLettuce.LettuceTeam src,
    LettuceTeam dest,
    bool initializeWithBase = true,
    bool checkOwnership = true)
  {
    if (src == null)
    {
      Log.Lettuce.PrintError("PopulateTeamMercenaries - Src team was null");
      return false;
    }
    if (src.HasMercenaryList && src.MercenaryList.Mercenaries != null)
    {
      foreach (LettuceTeamMercenary mercenary1 in src.MercenaryList.Mercenaries)
      {
        if (mercenary1 == null)
        {
          Log.Lettuce.PrintError(string.Format("PopulateTeamMercenaries - null mercenary found for Team {0}", (object) src.TeamId));
        }
        else
        {
          LettuceMercenary mercenary2 = CollectionManager.Get().GetMercenary((long) mercenary1.MercenaryId);
          if (mercenary2 == null)
            Log.Lettuce.PrintError(string.Format("PopulateTeamMercenaries - Mercenary{0} not found for Team {1}", (object) mercenary1.MercenaryId, (object) src.TeamId));
          else if (dest.m_loadouts.ContainsKey(mercenary2))
          {
            Log.Lettuce.PrintError(string.Format("PopulateTeamMercenaries - Duplicate mercenary{0} found in Team {1}", (object) mercenary1.MercenaryId, (object) src.TeamId));
          }
          else
          {
            dest.m_lettuceMercs.Add(mercenary2);
            LettuceMercenary.Loadout baseLoadout = mercenary2.GetBaseLoadout();
            LettuceMercenary.Loadout loadout = initializeWithBase ? new LettuceMercenary.Loadout(baseLoadout) : new LettuceMercenary.Loadout();
            if (checkOwnership)
            {
              LettuceMercenary.ArtVariation ownedArtVariation = mercenary2.GetOwnedArtVariation(mercenary1.SelectedArtVariationId, (TAG_PREMIUM) mercenary1.SelectedArtVariationPremium);
              loadout.SetArtVariation(ownedArtVariation.m_record, ownedArtVariation.m_premium);
            }
            else
              loadout.SetArtVariation(GameDbf.MercenaryArtVariation.GetRecord(mercenary1.SelectedArtVariationId), (TAG_PREMIUM) mercenary1.SelectedArtVariationPremium);
            if (mercenary1.HasSelectedEquipmentId)
            {
              if (CollectionManager.Get().IsLettuceLoaded() && checkOwnership && !mercenary2.CanSlotEquipment(mercenary1.SelectedEquipmentId))
                Log.Lettuce.PrintError(string.Format("PopulateTeamMercenaries - Could not slot mercenary{0} equipment {1}", (object) mercenary1.MercenaryId, (object) mercenary1.SelectedEquipmentId));
              else
                loadout.SetSlottedEquipment(GameDbf.LettuceEquipment.GetRecord(mercenary1.SelectedEquipmentId));
            }
            dest.m_loadouts.Add(mercenary2, loadout);
          }
        }
      }
    }
    return true;
  }

  public static PegasusLettuce.LettuceTeam Convert(
    LettuceTeam src,
    bool includeDataForRemoteSharing = false)
  {
    PegasusLettuce.LettuceTeam lettuceTeam1 = (PegasusLettuce.LettuceTeam) null;
    if (src == null)
      return lettuceTeam1;
    PegasusLettuce.LettuceTeam lettuceTeam2 = new PegasusLettuce.LettuceTeam();
    lettuceTeam2.TeamId = src.ID;
    lettuceTeam2.Name = src.Name;
    lettuceTeam2.SortOrder = src.SortOrder;
    lettuceTeam2.Type_ = src.TeamType;
    lettuceTeam2.MercenaryList = new LettuceTeamMercenaryList();
    foreach (LettuceMercenary lettuceMerc in src.m_lettuceMercs)
    {
      LettuceTeamMercenary lettuceTeamMercenary = new LettuceTeamMercenary();
      lettuceTeamMercenary.MercenaryId = lettuceMerc.ID;
      LettuceMercenary.Loadout loadout = src.GetLoadout(lettuceMerc);
      if (loadout != null)
      {
        lettuceTeamMercenary.SelectedArtVariationId = loadout.m_artVariationRecord.ID;
        lettuceTeamMercenary.SelectedArtVariationPremium = (int) loadout.m_artVariationPremium;
        if (loadout.m_equipmentRecord != null)
          lettuceTeamMercenary.SelectedEquipmentId = loadout.m_equipmentRecord.ID;
      }
      if (includeDataForRemoteSharing)
      {
        lettuceTeamMercenary.SharedTeamMercenaryXp = lettuceMerc.m_experience;
        lettuceTeamMercenary.SharedTeamMercenaryIsFullyUpgraded = lettuceMerc.m_isFullyUpgraded;
      }
      lettuceTeam2.MercenaryList.Mercenaries.Add(lettuceTeamMercenary);
    }
    return lettuceTeam2;
  }
}
