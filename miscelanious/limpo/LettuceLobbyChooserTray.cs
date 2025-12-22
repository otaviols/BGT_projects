using Assets;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class LettuceLobbyChooserTray : AccordionMenuTray, IPopupRendering
{
  public string m_pvpChooserButtonPrefab;
  public Material m_activeRunGlowMaterial;
  private LettuceLobbyChooserTray.SelectedOptionInfo m_selectedOption = new LettuceLobbyChooserTray.SelectedOptionInfo();
  private WidgetTemplate m_owningWidget;
  private List<WidgetInstance> m_buttonWidgets = new List<WidgetInstance>();
  private int m_numButtonsLoading;
  private bool m_okayToCreateButtons;
  private IPopupRoot m_popupRoot;
  private HashSet<IPopupRendering> m_popupRenderers = new HashSet<IPopupRendering>();

  public event Action OnModeSelected;

  public LettuceLobbyChooserTray.SelectedOptionInfo GetSelectedModeInfo()
  {
    if (this.m_selectedOption.Mode != SceneMgr.Mode.INVALID)
      return this.m_selectedOption;
    Debug.LogError((object) "LettuceLobbyChooserTray:GetSelectedModeInfo m_selectedOption is being accessed before initialization.");
    return (LettuceLobbyChooserTray.SelectedOptionInfo) null;
  }

  private void Awake() => this.m_owningWidget = this.GetComponentInParent<WidgetTemplate>();

  private void OnEnable()
  {
    if (!this.m_okayToCreateButtons)
    {
      this.m_okayToCreateButtons = true;
    }
    else
    {
      this.RemoveChooserButtons(true);
      this.StartCoroutine(this.InitTrayWhenReady());
    }
  }

  private void OnDestroy() => this.RemoveChooserButtons(false);

  protected IEnumerator InitTrayWhenReady()
  {
    LettuceLobbyChooserTray context = this;
    if ((UnityEngine.Object) context.m_ChooseFrameScroller == (UnityEngine.Object) null || (UnityEngine.Object) context.m_ChooseFrameScroller.ScrollObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "m_ChooseFrameScroller or m_ChooseFrameScroller.m_ScrollObject cannot be null. Unable to create button.", (UnityEngine.Object) context);
    }
    else
    {
      context.InitNormalLayout();
      while (!context.m_buttonWidgets.TrueForAll((Predicate<WidgetInstance>) (w => w.IsReady && !w.IsChangingStates)) || (double) context.m_ChooseFrameScroller.GetPolledScrollHeight() <= 0.0)
        yield return (object) null;
      if ((UnityEngine.Object) context.m_SelectedSubButton != (UnityEngine.Object) null && (UnityEngine.Object) context.m_ChooseFrameScroller != (UnityEngine.Object) null)
        context.m_ChooseFrameScroller.CenterObjectInView(context.m_SelectedSubButton.gameObject, 0.0f, (UIBScrollable.OnScrollComplete) null, iTween.EaseType.easeOutCubic, 0.0f);
    }
  }

  private int CompareBountySets(
    LettuceBountySetDbfRecord a,
    LettuceBountySetDbfRecord b,
    bool hasActiveBountySet,
    int previousBountySetId)
  {
    if (hasActiveBountySet)
    {
      if (a.ID == previousBountySetId)
        return -1;
      if (b.ID == previousBountySetId)
        return 1;
    }
    bool flag1 = MercenariesDataUtil.GetBountySetUnlockStatus(a) == MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED;
    bool flag2 = MercenariesDataUtil.GetBountySetUnlockStatus(b) == MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED;
    if (flag1 == flag2)
      return a.SortOrder.CompareTo(b.SortOrder);
    return !flag1 ? 1 : -1;
  }

  private void InitNormalLayout()
  {
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    bool heroicIsUnlocked = LettuceVillageDataUtil.IsHeroicDifficultyUnlocked();
    List<LettuceBountySetDbfRecord> records = GameDbf.LettuceBountySet.GetRecords((Predicate<LettuceBountySetDbfRecord>) (r => !r.IsTutorial));
    int previousMapSetId = -1;
    LettuceBounty.MercenariesBountyDifficulty previousMapDifficulty = LettuceBounty.MercenariesBountyDifficulty.NORMAL;
    bool hasActiveMap = false;
    PegasusLettuce.LettuceMap map = NetCache.Get().GetNetObject<NetCache.NetCacheLettuceMap>()?.Map;
    if (map != null)
    {
      LettuceBountyDbfRecord bountyRecord = GameDbf.LettuceBounty.GetRecord((int) map.BountyId);
      if (bountyRecord != null && records.Find((Predicate<LettuceBountySetDbfRecord>) (bountySet => bountySet.ID == bountyRecord.BountySetId)) != null)
      {
        hasActiveMap = map.Active;
        previousMapSetId = bountyRecord.BountySetId;
        previousMapDifficulty = bountyRecord.Heroic ? LettuceBounty.MercenariesBountyDifficulty.HEROIC : LettuceBounty.MercenariesBountyDifficulty.NORMAL;
      }
    }
    records.Sort((Comparison<LettuceBountySetDbfRecord>) ((a, b) => this.CompareBountySets(a, b, hasActiveMap, previousMapSetId)));
    bool flag = true;
    foreach (LettuceBountySetDbfRecord bountySetDbfRecord in records)
    {
      LettuceBountySetDbfRecord lettuceBountySetRecord = bountySetDbfRecord;
      bool thisIsFirstButtonInList = flag;
      Dictionary<LettuceBounty.MercenariesBountyDifficulty, int> newBountiesPerDifficulty = new Dictionary<LettuceBounty.MercenariesBountyDifficulty, int>();
      if (netObject != null && netObject.BountyInfoMap != null)
      {
        foreach (LettuceBountyDbfRecord record in GameDbf.LettuceBounty.GetRecords((Predicate<LettuceBountyDbfRecord>) (r => r.BountySetId == lettuceBountySetRecord.ID)))
        {
          if (((netObject.BountyInfoMap.ContainsKey(record.ID) || MercenariesDataUtil.GetBountyUnlockStatus(record) != MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED ? 0 : (MercenariesDataUtil.GetBountySetUnlockStatus(lettuceBountySetRecord) == MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED ? 1 : 0)) | (!netObject.BountyInfoMap.ContainsKey(record.ID) ? (false ? 1 : 0) : (!netObject.BountyInfoMap[record.ID].IsAcknowledged ? 1 : 0))) != 0)
          {
            int num = 0;
            LettuceBounty.MercenariesBountyDifficulty key = record.DifficultyMode;
            if (key == LettuceBounty.MercenariesBountyDifficulty.NONE)
              key = record.Heroic ? LettuceBounty.MercenariesBountyDifficulty.HEROIC : LettuceBounty.MercenariesBountyDifficulty.NORMAL;
            newBountiesPerDifficulty.TryGetValue(key, out num);
            newBountiesPerDifficulty[key] = num + 1;
          }
        }
      }
      this.m_buttonWidgets.Add(this.CreateChooserButton(this.m_DefaultChooserButtonPrefab, (string) lettuceBountySetRecord.Name, (Action<LettuceLobbyChooserButton>) (button =>
      {
        LettuceLobbyChooserTray.BountySetButtonInfo bountySetButtons = this.CreateBountySetButtons(button, lettuceBountySetRecord, newBountiesPerDifficulty);
        if (hasActiveMap)
          this.ConfigureButtonWhenPreviousBountyIsUnfinished(bountySetButtons, previousMapSetId, previousMapDifficulty);
        else if (lettuceBountySetRecord.IsComingSoon)
        {
          button.SetDesaturate(true);
          bountySetButtons.NormalSubButton.LockButton(MercenariesDataUtil.MercenariesBountyLockedReason.COMING_SOON);
          bountySetButtons.HeroicSubButton.LockButton(MercenariesDataUtil.MercenariesBountyLockedReason.COMING_SOON);
        }
        else
        {
          MercenariesDataUtil.MercenariesBountyLockedReason bountySetUnlockStatus = MercenariesDataUtil.GetBountySetUnlockStatus(lettuceBountySetRecord);
          switch (bountySetUnlockStatus)
          {
            case MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_STARTED:
            case MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_ACTIVE:
            case MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_ENDED:
            case MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_COMPLETE:
              this.ConfigureButtonFromEventStatus(bountySetButtons, bountySetUnlockStatus);
              if (!string.IsNullOrEmpty((string) lettuceBountySetRecord.EventComingSoonText))
              {
                bountySetButtons.NormalSubButton.SetCustomLockedText((string) lettuceBountySetRecord.EventComingSoonText);
                bountySetButtons.HeroicSubButton.SetCustomLockedText((string) lettuceBountySetRecord.EventComingSoonText);
                break;
              }
              break;
            case MercenariesDataUtil.MercenariesBountyLockedReason.PREVIOUS_ZONES_INCOMPLETE:
              button.SetDesaturate(true);
              bountySetButtons.NormalSubButton.LockButton(bountySetUnlockStatus);
              bountySetButtons.HeroicSubButton.LockButton(bountySetUnlockStatus);
              break;
            default:
              if (!heroicIsUnlocked)
              {
                bountySetButtons.HeroicSubButton.LockButton(MercenariesDataUtil.MercenariesBountyLockedReason.PVE_BUILDING_NEEDS_UPGRADE);
                break;
              }
              break;
          }
          this.HandleDefaultButtonExpansion(bountySetButtons, previousMapSetId, previousMapDifficulty, thisIsFirstButtonInList);
        }
      })));
      flag = false;
    }
  }

  private LettuceLobbyChooserTray.BountySetButtonInfo CreateBountySetButtons(
    LettuceLobbyChooserButton button,
    LettuceBountySetDbfRecord bountySet,
    Dictionary<LettuceBounty.MercenariesBountyDifficulty, int> newCounts)
  {
    LettuceLobbyChooserTray.BountySetButtonInfo bountySetButtons = new LettuceLobbyChooserTray.BountySetButtonInfo();
    bountySetButtons.ChooserButton = button;
    bountySetButtons.BountySet = bountySet;
    button.m_visualController.SetState(bountySet.ShortGuid);
    if (!string.IsNullOrEmpty(bountySet.TileArtTexture))
      AssetLoader.Get().LoadTexture((AssetReference) bountySet.TileArtTexture, (ObjectCallback) ((assetRef, obj, callbackData) =>
      {
        Renderer portraitRenderer = (Renderer) button.m_PortraitRenderer;
        if (!((UnityEngine.Object) portraitRenderer != (UnityEngine.Object) null))
          return;
        RendererExtension.GetMaterial(portraitRenderer).mainTexture = obj as Texture;
      }));
    int numNew1 = newCounts == null || !newCounts.ContainsKey(LettuceBounty.MercenariesBountyDifficulty.NORMAL) ? 0 : newCounts[LettuceBounty.MercenariesBountyDifficulty.NORMAL];
    int numNew2 = newCounts == null || !newCounts.ContainsKey(LettuceBounty.MercenariesBountyDifficulty.HEROIC) || !LettuceVillageDataUtil.IsHeroicDifficultyUnlocked() ? 0 : newCounts[LettuceBounty.MercenariesBountyDifficulty.HEROIC];
    button.SetNewCount(numNew1 + numNew2);
    bountySetButtons.NormalSubButton = button.CreateLettuceLobbySubButton(GameStrings.Get("GLUE_LETTUCE_NORMAL_SUB_BUTTON"), SceneMgr.Mode.LETTUCE_BOUNTY_BOARD, bountySet, LettuceBounty.MercenariesBountyDifficulty.NORMAL, this.m_DefaultChooserSubButtonPrefab, false, numNew1);
    bountySetButtons.HeroicSubButton = button.CreateLettuceLobbySubButton(GameStrings.Get("GLUE_LETTUCE_HEROIC_SUB_BUTTON"), SceneMgr.Mode.LETTUCE_BOUNTY_BOARD, bountySet, LettuceBounty.MercenariesBountyDifficulty.HEROIC, this.m_DefaultChooserSubButtonPrefab, false, numNew2);
    return bountySetButtons;
  }

  private void ConfigureButtonWhenPreviousBountyIsUnfinished(
    LettuceLobbyChooserTray.BountySetButtonInfo buttonInfo,
    int activeSetId,
    LettuceBounty.MercenariesBountyDifficulty activeDifficulty)
  {
    if (buttonInfo.BountySet.ID == activeSetId)
    {
      buttonInfo.ChooserButton.ToggleButton(true);
      buttonInfo.ChooserButton.GetComponent<UIBHighlight>().AlwaysOver = true;
      RendererExtension.SetMaterial((Renderer) buttonInfo.ChooserButton.m_glowRenderer, this.m_activeRunGlowMaterial);
      LettuceLobbyChooserSubButton chooserSubButton1;
      LettuceLobbyChooserSubButton chooserSubButton2;
      if (activeDifficulty == LettuceBounty.MercenariesBountyDifficulty.HEROIC)
      {
        chooserSubButton1 = buttonInfo.HeroicSubButton;
        chooserSubButton2 = buttonInfo.NormalSubButton;
      }
      else
      {
        chooserSubButton1 = buttonInfo.NormalSubButton;
        chooserSubButton2 = buttonInfo.HeroicSubButton;
      }
      chooserSubButton1.TriggerRelease();
      chooserSubButton2.LockButton(MercenariesDataUtil.MercenariesBountyLockedReason.CURRENT_BOUNTY_UNFINISHED);
      this.m_SelectedSubButton = (ChooserSubButton) chooserSubButton1;
    }
    else
    {
      buttonInfo.ChooserButton.SetDesaturate(true);
      buttonInfo.NormalSubButton.LockButton(MercenariesDataUtil.MercenariesBountyLockedReason.CURRENT_BOUNTY_UNFINISHED);
      buttonInfo.HeroicSubButton.LockButton(MercenariesDataUtil.MercenariesBountyLockedReason.CURRENT_BOUNTY_UNFINISHED);
    }
  }

  private void ConfigureButtonFromEventStatus(
    LettuceLobbyChooserTray.BountySetButtonInfo buttonInfo,
    MercenariesDataUtil.MercenariesBountyLockedReason lockReason)
  {
    buttonInfo.ChooserButton.SetDesaturate(true);
    buttonInfo.NormalSubButton.LockButton(lockReason);
    buttonInfo.HeroicSubButton.LockButton(lockReason);
  }

  private void HandleDefaultButtonExpansion(
    LettuceLobbyChooserTray.BountySetButtonInfo buttonInfo,
    int previousSetId,
    LettuceBounty.MercenariesBountyDifficulty previousDifficulty,
    bool thisIsTheFirstButtonInList)
  {
    if (previousSetId != -1)
    {
      if (buttonInfo.BountySet.ID != previousSetId)
        return;
      buttonInfo.ChooserButton.ToggleButton(true);
      if (previousDifficulty == LettuceBounty.MercenariesBountyDifficulty.HEROIC)
      {
        buttonInfo.HeroicSubButton.TriggerRelease();
        this.m_SelectedSubButton = (ChooserSubButton) buttonInfo.HeroicSubButton;
      }
      else
      {
        buttonInfo.NormalSubButton.TriggerRelease();
        this.m_SelectedSubButton = (ChooserSubButton) buttonInfo.NormalSubButton;
      }
    }
    else
    {
      if (!thisIsTheFirstButtonInList)
        return;
      buttonInfo.ChooserButton.ToggleButton(true);
      buttonInfo.NormalSubButton.TriggerRelease();
      this.m_SelectedSubButton = (ChooserSubButton) buttonInfo.NormalSubButton;
    }
  }

  private void RemoveChooserButtons(bool destroyObjects)
  {
    foreach (WidgetInstance buttonWidget in this.m_buttonWidgets)
    {
      if ((UnityEngine.Object) buttonWidget != (UnityEngine.Object) null)
      {
        this.UnregisterButtonFromOwningWidget(buttonWidget);
        if (destroyObjects)
          UnityEngine.Object.Destroy((UnityEngine.Object) buttonWidget.gameObject);
      }
    }
    this.m_SelectedSubButton = (ChooserSubButton) null;
    this.m_ChooserButtons.Clear();
    this.m_buttonWidgets.Clear();
  }

  private WidgetInstance CreateChooserButton(
    string prefab,
    string chooserButtonName,
    Action<LettuceLobbyChooserButton> OnButtonReadyCallback)
  {
    WidgetInstance widget = WidgetInstance.Create(prefab);
    ++this.m_numButtonsLoading;
    widget.RegisterReadyListener((Action<object>) (_ =>
    {
      LettuceLobbyChooserButton newbutton = widget.transform.GetComponentInChildren<LettuceLobbyChooserButton>();
      if ((UnityEngine.Object) newbutton == (UnityEngine.Object) null)
        return;
      GameUtils.SetParent((Component) widget, this.m_ChooseFrameScroller.ScrollObject);
      if (this.m_popupRoot != null)
        this.m_popupRoot.ApplyPopupRendering(widget.transform, this.m_popupRenderers, true, 31);
      newbutton.SetButtonText(chooserButtonName);
      newbutton.AddVisualUpdatedListener(new ChooserButton.VisualUpdated(((AccordionMenuTray) this).OnButtonVisualUpdated));
      int index = this.m_ChooserButtons.Count;
      newbutton.AddToggleListener((ChooserButton.Toggled) (toggle => this.OnChooserButtonToggled((ChooserButton) newbutton, toggle, index)));
      newbutton.AddModeSelectionListener(new ChooserButton.ModeSelection(this.ButtonModeSelected));
      newbutton.AddExpandedListener(new ChooserButton.Expanded(this.ButtonExpanded));
      this.m_ChooserButtons.Add((ChooserButton) newbutton);
      if (OnButtonReadyCallback != null)
        OnButtonReadyCallback(newbutton);
      newbutton.FireVisualUpdatedEvent();
      --this.m_numButtonsLoading;
    }), (object) null, true);
    this.RegisterButtonWithOwningWidget(widget);
    return widget;
  }

  private void RegisterButtonWithOwningWidget(WidgetInstance instance)
  {
    if (!((UnityEngine.Object) this.m_owningWidget != (UnityEngine.Object) null))
      return;
    this.m_owningWidget.AddNestedInstance(instance);
  }

  private void UnregisterButtonFromOwningWidget(WidgetInstance instance)
  {
    if (!((UnityEngine.Object) this.m_owningWidget != (UnityEngine.Object) null))
      return;
    this.m_owningWidget.RemoveNestedInstance(instance);
  }

  private void ButtonModeSelected(ChooserSubButton btn)
  {
    foreach (ChooserButton chooserButton in this.m_ChooserButtons)
      chooserButton.DisableSubButtonHighlights();
    LettuceLobbyChooserSubButton chooserSubButton = btn as LettuceLobbyChooserSubButton;
    this.m_selectedOption.SetInfo(chooserSubButton.GetMode(), chooserSubButton.GetBountySetRecord(), chooserSubButton.GetDifficulty(), chooserSubButton.GetLockedReason(), chooserSubButton.GetCustomLockedText());
    if (this.OnModeSelected == null)
      return;
    this.OnModeSelected();
  }

  protected void ButtonExpanded(ChooserButton button, bool expand)
  {
    if (!expand)
      return;
    this.ToggleScrollable(true);
  }

  private LettuceLobbyChooserTray.EventStatus GetEventStatus(string eventName)
  {
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    SpecialEventType eventType = specialEventManager.GetEventType(eventName);
    if (eventType == SpecialEventType.UNKNOWN)
      return LettuceLobbyChooserTray.EventStatus.DISABLED;
    if (specialEventManager.IsEventActive(eventType, false))
      return LettuceLobbyChooserTray.EventStatus.ACTIVE;
    if (eventType == SpecialEventType.SPECIAL_EVENT_NEVER || specialEventManager.IsEventForcedInactive(eventType))
      return LettuceLobbyChooserTray.EventStatus.DISABLED;
    return specialEventManager.HasEventEnded(eventType) ? LettuceLobbyChooserTray.EventStatus.ENDED : LettuceLobbyChooserTray.EventStatus.NOT_YET_STARTED;
  }

  public void EnablePopupRendering(IPopupRoot popupRoot) => this.m_popupRoot = popupRoot;

  public void DisablePopupRendering()
  {
    if (this.m_popupRoot == null)
      return;
    this.m_popupRoot.CleanupPopupRendering(this.m_popupRenderers);
  }

  public bool HandlesChildPropagation() => true;

  public class SelectedOptionInfo
  {
    public SceneMgr.Mode Mode;
    public LettuceBountySetDbfRecord BountySetRecord;
    public MercenariesDataUtil.MercenariesBountyLockedReason LockedReason;
    public LettuceBounty.MercenariesBountyDifficulty Difficulty;
    public string CustomLockedText;

    public bool Locked => this.LockedReason != 0;

    public void SetInfo(
      SceneMgr.Mode mode,
      LettuceBountySetDbfRecord record,
      LettuceBounty.MercenariesBountyDifficulty difficulty,
      MercenariesDataUtil.MercenariesBountyLockedReason lockedReason = MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED,
      string customLockedText = null)
    {
      this.Mode = mode;
      this.BountySetRecord = record;
      this.Difficulty = difficulty;
      this.LockedReason = lockedReason;
      this.CustomLockedText = customLockedText;
    }
  }

  private class BountySetButtonInfo
  {
    public LettuceLobbyChooserButton ChooserButton;
    public LettuceBountySetDbfRecord BountySet;
    public LettuceLobbyChooserSubButton NormalSubButton;
    public LettuceLobbyChooserSubButton HeroicSubButton;
  }

  private enum EventStatus
  {
    ACTIVE,
    DISABLED,
    NOT_YET_STARTED,
    ENDED,
  }
}
