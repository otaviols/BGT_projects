using Assets;
using System;
using UnityEngine;

public class UnopenedPack : PegUIElement
{
  public UnopenedPackStack m_SingleStack;
  public UnopenedPackStack m_MultipleStack;
  public GameObject m_LockRibbon;
  public GameObject m_AmountBanner;
  public UberText m_AmountText;
  public UberText m_LockedRibbonText;
  public Spell m_AlertEvent;
  public Spell m_DragStartEvent;
  public Spell m_DragStopEvent;
  public DragRotatorInfo m_DragRotatorInfo = new DragRotatorInfo()
  {
    m_PitchInfo = new DragRotatorAxisInfo()
    {
      m_ForceMultiplier = 3f,
      m_MinDegrees = -55f,
      m_MaxDegrees = 55f,
      m_RestSeconds = 2f
    },
    m_RollInfo = new DragRotatorAxisInfo()
    {
      m_ForceMultiplier = 4.5f,
      m_MinDegrees = -60f,
      m_MaxDegrees = 60f,
      m_RestSeconds = 2f
    }
  };
  private int m_boosterDbId;
  private int m_count;
  private UnopenedPack m_draggedPack;
  private UnopenedPack m_creatorPack;
  private bool m_isRotatedPack;

  protected override void Awake()
  {
    base.Awake();
    this.UpdateState();
  }

  public int GetBoosterId() => this.m_boosterDbId;

  public int GetCount() => this.m_count;

  public void SetBoosterId(int boosterDbId)
  {
    this.m_boosterDbId = boosterDbId;
    if (GameDbf.Booster.GetRecord(boosterDbId) != null)
      this.m_isRotatedPack = GameUtils.IsBoosterRotated((BoosterDbId) boosterDbId, DateTime.UtcNow);
    this.UpdateState();
  }

  public void SetCount(int count)
  {
    this.m_count = count;
    this.UpdateState();
  }

  public void AddBoosters(int numNewBoosters)
  {
    this.m_count += numNewBoosters;
    this.UpdateState();
  }

  public void AddBooster() => this.AddBoosters(1);

  public void RemoveBooster()
  {
    --this.m_count;
    if (this.m_count < 0)
    {
      Debug.LogWarning((object) "UnopenedPack.RemoveBooster(): Removed a booster pack from a stack with no boosters");
      this.m_count = 0;
    }
    this.UpdateState();
  }

  public UnopenedPack AcquireDraggedPack()
  {
    if ((UnityEngine.Object) this.m_draggedPack != (UnityEngine.Object) null)
      return this.m_draggedPack;
    Vector3 position = this.transform.position;
    position.y -= 5000f;
    this.m_draggedPack = UnityEngine.Object.Instantiate<UnopenedPack>(this, position, this.transform.rotation);
    TransformUtil.CopyWorldScale((Component) this.m_draggedPack, (Component) this);
    this.m_draggedPack.transform.parent = this.transform.parent;
    UIBScrollableItem component = this.m_draggedPack.GetComponent<UIBScrollableItem>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.m_active = UIBScrollableItem.ActiveState.Inactive;
    this.m_draggedPack.m_creatorPack = this;
    this.m_draggedPack.gameObject.AddComponent<DragRotator>().SetInfo(this.m_DragRotatorInfo);
    this.m_draggedPack.m_DragStartEvent.Activate();
    return this.m_draggedPack;
  }

  public void ReleaseDraggedPack()
  {
    if ((UnityEngine.Object) this.m_draggedPack == (UnityEngine.Object) null)
      return;
    UnopenedPack draggedPack = this.m_draggedPack;
    this.m_draggedPack = (UnopenedPack) null;
    draggedPack.m_DragStopEvent.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnDragStopSpellStateFinished), (object) draggedPack);
    draggedPack.m_DragStopEvent.Activate();
    this.UpdateState();
  }

  public UnopenedPack GetDraggedPack() => this.m_draggedPack;

  public UnopenedPack GetCreatorPack() => this.m_creatorPack;

  public void PlayAlert() => this.m_AlertEvent.ActivateState(SpellStateType.BIRTH);

  public void StopAlert() => this.m_AlertEvent.ActivateState(SpellStateType.DEATH);

  public bool CanOpenPack()
  {
    string packLockedReason = string.Empty;
    return this.CanOpenPack(out packLockedReason);
  }

  public bool CanOpenPack(out string packLockedReason)
  {
    BoosterDbfRecord record = GameDbf.Booster.GetRecord(this.m_boosterDbId);
    packLockedReason = string.Empty;
    if (record == null)
      return false;
    return this.m_boosterDbId == 629 ? this.CanOpenMercenariesPack(record, out packLockedReason) : this.CanOpenTraditionalHearthstonePack(record, out packLockedReason);
  }

  public void UpdateState()
  {
    string packLockedReason = string.Empty;
    bool flag1 = this.CanOpenPack(out packLockedReason);
    if ((UnityEngine.Object) this.m_LockRibbon != (UnityEngine.Object) null)
      this.m_LockRibbon.SetActive(!flag1);
    if (!flag1 && (UnityEngine.Object) this.m_LockedRibbonText != (UnityEngine.Object) null && !string.IsNullOrEmpty(packLockedReason))
      this.m_LockedRibbonText.Text = packLockedReason;
    bool flag2 = this.GetCount() == 0;
    bool flag3 = this.GetCount() > 1;
    this.m_SingleStack.m_RootObject.SetActive((!flag3 || !flag1) && !flag2);
    this.m_MultipleStack.m_RootObject.SetActive(((!flag3 ? 0 : (!flag2 ? 1 : 0)) & (flag1 ? 1 : 0)) != 0);
    this.m_AmountBanner.SetActive(flag3);
    this.m_AmountText.enabled = flag3;
    if (!flag3)
      return;
    this.m_AmountText.Text = this.GetCount().ToString();
  }

  private void OnDragStopSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) ((Component) userData).gameObject);
  }

  private bool CanOpenPackBasedOnEventTiming(BoosterDbfRecord boosterDBFRecord)
  {
    SpecialEventType openPackEvent = boosterDBFRecord.OpenPackEvent;
    switch (openPackEvent)
    {
      case SpecialEventType.UNKNOWN:
        return false;
      case SpecialEventType.IGNORE:
        return true;
      default:
        SpecialEventManager specialEventManager = SpecialEventManager.Get();
        return specialEventManager.IsEventActive(openPackEvent, false) || GameUtils.AtPrereleaseEvent() && specialEventManager.IsEventActive(boosterDBFRecord.PrereleaseOpenPackEvent, false);
    }
  }

  private bool CanOpenTraditionalHearthstonePack(
    BoosterDbfRecord boosterDBFRecord,
    out string packLockedReason)
  {
    packLockedReason = string.Empty;
    if (!this.m_isRotatedPack || RankMgr.Get().WildCardsAllowedInCurrentLeague())
      return this.CanOpenPackBasedOnEventTiming(boosterDBFRecord);
    packLockedReason = GameStrings.Get("GLUE_NEW_PLAYER_AVAILABLE_AT_LEAGUE_PROMO");
    return false;
  }

  private bool CanOpenMercenariesPack(
    BoosterDbfRecord boosterDBFRecord,
    out string packLockedReason)
  {
    packLockedReason = string.Empty;
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().MercenariesPackOpeningEnabled)
    {
      packLockedReason = GameStrings.Get("GLUE_MERCENARY_PACK_UNAVAILABLE");
      return false;
    }
    if (LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_SHOP_CLAIM_PACK_POPUP))
      return this.CanOpenPackBasedOnEventTiming(boosterDBFRecord);
    packLockedReason = GameStrings.Get("GLUE_MERCENARY_TUTORIAL_INCOMPLETE_FOR_PACK");
    return false;
  }
}
